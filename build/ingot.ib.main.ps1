# ***************************************
# 
#
#           Functions
# 
#
# ***************************************

# ---------------------------------------
# Functional
# ---------------------------------------

function New-MergedCodeCoverage {

    param (
        $OutputFormat,
        $OutputFileName,
        $TestCoverageFiles
    )

    Write-Step-Start "Merging tests code coverage files into 1 '${OutputFormat}' file...";
    
    New-Directory $TestCoverageDirectory;

    $testCoverageMergedFile = "${TestCoverageDirectory}/${OutputFileName}";
    Write-Log Debug  "Invoking 'dotnet-coverage' to merge $($TestCoverageFiles.Count) tests code coverage files`ninto '${testCoverageMergedFile}' with format '${OutputFormat}'...";
    exec { dotnet dotnet-coverage merge --output $testCoverageMergedFile --output-format "${OutputFormat}" $TestCoverageFiles }
    Write-Log Information  "Invoked successfully 'dotnet-coverage' to merge $($TestCoverageFiles.Count) tests code coverage files`ninto '${testCoverageMergedFile}' with format '${OutputFormat}'";

    $directory = "${TestCoverageDirectory}";
    $fileFilter = "${OutputFileName}";

    $codeCoverages = Get-ChildItem $directory -File -Recurse | Where-Object {$_.Name -like $fileFilter};
    Write-Files-Found $codeCoverages -Directory $directory -Filter $fileFilter;
    Assert-Files-Found $codeCoverages -Directory $directory -Filter $fileFilter;

    Write-Step-End "Successfully merged tests code coverage files into 1 '${OutputFormat}' file";
}

function Get-CodeCoverageReport-DirectoryPath {
    
    param (
        $ReportType
    )

    return "${TestCoverageReportDirectory}/${ReportType}";
}

function New-CodeCoverage
{
    param (
        $ReportType
    )
    
    Write-Step-Start "Creating code coverage report '${ReportType}'...";

    $testCoverageCoberturaMergedFile = "${TestCoverageDirectory}/${TestCoverageCoberturaFileName}";
    $testCoverageReportAssemblyFilter = "-*.Mocks;-*.Tests;-*.Testing.*";

    $testCoverageReportFileDirectory = Get-CodeCoverageReport-DirectoryPath -ReportType $ReportType;
    Write-Log Debug "Invoking 'reportgenerator' to create '${ReportType}' code coverage report`nin ${testCoverageReportFileDirectory}...";
    exec { dotnet reportgenerator -reports:$testCoverageCoberturaMergedFile -targetdir:$testCoverageReportFileDirectory -reporttypes:"${ReportType}" -assemblyfilters:"${testCoverageReportAssemblyFilter}" }

    Write-Step-End "Successfully created code coverage report '${ReportType}'";
}

# ***************************************
# 
#
#           Local Tasks
# 
#
# ***************************************

# Setup
# ---------------------------------------
# Synopsis: Runs the initial setup for the repo
task setup `
    ci-setup-before, `
    ?ci-setup, `
    ci-setup-finally

# Full
# ---------------------------------------
# Synopsis: full flow (format > linter > build > test > coverage > pack > publish)
task full `
    format, `
    linter, `
    build, `
    test, `
    test-result, `
    coverage, `
    pack, `
    publish
    
# IDE
# ---------------------------------------
# Synopsis: Opens ./src/Ingot.sln in Visual Studio
task ide `
    tool-visual-studio-open

# Format
# ---------------------------------------
# Synopsis: Format source code
task format `
    src-format

# Linter
# ---------------------------------------
# Synopsis: Validates source code format
task linter `
    ci-linter-before, `
    ?ci-linter, `
    ci-linter-finally

# Build
# ---------------------------------------
# Synopsis: Builds source code
task build `
    ci-build-before, `
    ?ci-build, `
    ci-build-finally

# Test
# ---------------------------------------
# Synopsis: Tests source code
task test `
    ci-test-before, `
    ?ci-test, `
    ci-test-finally

# Test Report
# ---------------------------------------
# Synopsis: Generates tests report
task test-result `
    ci-test-result-before, `
    ?ci-test-result, `
    ci-test-result-finally

# Coverage
# ---------------------------------------
# Synopsis: Generates code coverage report
task coverage `
    ci-coverage-before, `
    ?ci-coverage, `
    ci-coverage-finally

# Pack
# ---------------------------------------
# Synopsis: Packages source code
task pack `
    ci-pack-before, `
    ?ci-pack, `
    ci-pack-finally

# Publish
# ---------------------------------------
# Synopsis: Publishes packages either locally or remotely if you specify ApiKey and Source
task publish `
    ci-publish-before, `
    ?ci-publish, `
    ci-publish-finally

# ***************************************
# 
#
#           Specific Tasks
# 
#
# ***************************************

# ---------------------------------------
# CI Tasks
# ---------------------------------------

# Setup
# ---------------------------------------
task ci-setup-before -If($False)
task ci-setup-finally -If($False) 

task ci-setup `
    tool-ib-setup, `
    tool-nuget-setup, `
    tool-minver-validate

# Linter
# ---------------------------------------
task ci-linter-before -If($False) 
task ci-linter-finally -If($False) 
    
task ci-linter `
    src-linter

# Linter
# ---------------------------------------
task ci-build-before -If($False) 
task ci-build-finally -If($False) 

task ci-build `
    src-build

# Test
# ---------------------------------------
task ci-test-before -If($False) 
task ci-test-finally -If($False) 

task ci-test `
    src-test

# Test Result
# ---------------------------------------
task ci-test-result-before -If($False) 
task ci-test-result-finally -If($False) 

task ci-test-result `
    src-test-result

# Coverage
# ---------------------------------------
task ci-coverage-before -If($False) 
task ci-coverage-finally -If($False) 

task ci-coverage `
    src-coverage

# Pack
# ---------------------------------------
task ci-pack-before -If($False) 
task ci-pack-finally -If($False) 

task ci-pack `
    src-pack

# Publish
# ---------------------------------------
task ci-publish-before -If($False) 
task ci-publish-finally -If($False) 

task ci-publish `
    src-pack-validate, `
    src-publish-local, `
    src-publish-remote

# ---------------------------------------
# Tools Tasks
# ---------------------------------------

# Synopsis: [Specific] Configures Invoke-Build to use pwsh by default
task tool-ib-setup -If(-Not (Test-CI-ExecutionEnvironment)) {

    # Enable PowerShell Core usage to ensure 
    # users benefit from modern features and
    # consistent experience accross platforms
    Write-Step-Start "Enabling use of PowerShell Core (pwsh)...";
    
    if (Test-Shell-Is-Pwsh)
    {
        Write-Step-End "PowerShell Core (pwsh) already enabled";
    }
    else
    {
        # Invoke-Build supports using "pwsh" as default
        # instead of using --pwsh for every command. See:
        # https://github.com/nightroman/Invoke-Build/issues/219
        $pwshEnvScope = [System.EnvironmentVariableTarget]::User;
        $pwshEnvKey = "pwsh";
        $pwshEnvValue = "pwsh";

        # If there is already the environment variable setup, the
        # user has already passed through the setup (see below).
        $pwshEnv = [System.Environment]::GetEnvironmentVariable($pwshEnvKey, $pwshEnvScope);
        if ($pwshEnv -eq $pwshEnvValue)
        {
            $msg = (
                "PowerShell Core (pwsh) enabled but not active. `n" +
                "Ensure to refresh environment variables by restarting your program."
            );

            if($env:TERM_PROGRAM -eq 'vscode') {
                $msg += "`nIt appears you are using Visual Studio Code (VSCode), ensure to close VSCode completely, not just the terminal.";      
            }

            Write-Log Warning $msg;
        }
        else
        {
            Write-Log Debug  "Checking PowerShell Core (pwsh) availability locally...";
            if (Get-Command "pwsh" -errorAction SilentlyContinue)
            {
                Write-Log Debug  "PowerShell Core (pwsh) is available locally";

                Write-Log Debug  "Setting environment variable '${pwshEnvKey}=${pwshEnvValue}' on scope ${pwshEnvScope}...";
                [System.Environment]::SetEnvironmentVariable($pwshEnvKey, $pwshEnvValue, $pwshEnvScope);
                
                $msg = (
                    "PowerShell Core (pwsh) enabled but not active. `n" +
                    "Ensure to refresh environment variables by restarting your program."
                );

                if($env:TERM_PROGRAM -eq 'vscode') {
                    $msg += "`nIt appears you are using Visual Studio Code (VSCode), ensure to close VSCode completely, not just the terminal.";      
                }

                Write-Log Warning $msg;
            }
            else
            {
                Write-Log Warning ( 
                    "PowerShell Core (pwsh) not available in your system. `n" +
                    "You should install PowerShell Core (pwsh) to ensure consistent experience across different platforms and on the CI.`n" +
                    "See https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows " +
                    "for more details on how to install PowerShell Core."
                );
            }
        }
    }
}

# Synopsis: [Specific] Ensures minver is correctly setup
task tool-minver-validate {

    Write-Step-Start "Validating MinVer version...";

    Write-Log Debug  "Invoking 'minver' to compute version...";
    $version = exec { dotnet minver -v d }
    Write-Log Information  "Retrieved MinVer computed version '${version}'";

    $minverDefaultVersion = "0.0.0-alpha.0";
    Write-Log Debug  "Validating MinVer computed version '${version}' is not the default fallback '${minverDefaultVersion}'";
    if ($version -like "${minverDefaultVersion}*")
    {
        if ($Force)
        {
            Write-Log Warning (
                "MinVer computed version '${version}' most likely indicates a wrong setup.`n" +
                "You have set '-Force' to bypass the MinVer version validation."
            );   
        }
        else
        {
            Write-Log Error (
                "MinVer computed version '${version}' most likely indicates a wrong setup.`n" +
                "specify '-Force' to bypass this validation."
            );
        }
    }
    else
    {
        Write-Step-End "MinVer computed version '${version}' most likely indicates a proper setup"
    }
}

# Synopsis: [Specific] Configures nuget for local feed
task tool-nuget-setup -If(-Not (Test-CI-ExecutionEnvironment)) {

    Write-Step-Start "Creating NuGet config file...";

    $nugetConfigFile = "./nuget.config";
    if (Test-Path $nugetConfigFile)
    {
        Write-Log Debug  "NuGet config file '${nugetConfigFile}' exists already. Deleting it...";
        Remove-Item $nugetConfigFile -Force;
        if (Test-Path $nugetConfigFile)
        {
            Write-Log Error "Failed to create NuGet config file '${nugetConfigFile}'";
        }
        Write-Log Information  "Deleted existing NuGet config file '${nugetConfigFile}'";
    }

    Write-Log Debug  "Creating empty NuGet config file '${nugetConfigFile}'...";
    '<configuration></configuration>' > $nugetConfigFile;
    if (-Not (Test-Path $nugetConfigFile))
    {
        Write-Log Error "Failed to create NuGet config file '${nugetConfigFile}'";
    }
    Write-Log Information  "Created empty NuGet config file '${nugetConfigFile}'";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Write-File-Content -File $nugetConfigFile -Content $nugetConfigContent;

    Write-Step-End "Successfully created NuGet config file";

    Write-Step-Start "Adding local NuGet source...";

    $nugetLocalFeedName = "ingot-local-feed";
    $nugetLocalFeedValue = "${BuildRoot}/dist/local-nuget-feed";

    New-Directory $nugetLocalFeedValue;

    Write-Log Debug  "Invoking 'dotnet nuget' to add local NuGet source '${nugetLocalFeedName}' (${nugetLocalFeedValue})...";
    exec { dotnet nuget add source --configfile $nugetConfigFile --name $nugetLocalFeedName $nugetLocalFeedValue }
    Write-Log Information  "Successfully added local NuGet source '${nugetLocalFeedName}' (${nugetLocalFeedValue})";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Write-File-Content -File $nugetConfigFile -Content $nugetConfigContent;
    
    Write-Step-End "Successfully added local NuGet source";

    Write-Step-Start "Setting local NuGet push source...";

    $nugetDefaultPushSourceKey = "defaultPushSource";
    Write-Log Debug  "Invoking 'dotnet nuget' to set NuGet configuration '${nugetDefaultPushSourceKey}' (${nugetLocalFeedValue})...";
    exec { dotnet nuget config set --configfile $nugetConfigFile $nugetDefaultPushSourceKey $nugetLocalFeedValue }
    Write-Log Information  "Successfully set NuGet configuration '${nugetDefaultPushSourceKey}' (${nugetLocalFeedValue})";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Write-File-Content -File $nugetConfigFile -Content $nugetConfigContent;
    
    Write-Step-End "Successfully set local NuGet push source";
}

# Synopsis: [Specific] Opens ./src/Ingot.sln in Visual Studio
task tool-visual-studio-open {

    Write-Step-Start "Searching Visual Studio installation...";

    $programFilesx86 = (Get-ChildItem "env:ProgramFiles(x86)").Value;
    $vsWherePath = "${programFilesx86}\Microsoft Visual Studio\Installer\vswhere.exe";
    Write-Log Debug  "Validating vswhere.exe '${vsWherePath}' exists..."
    if (-Not (Test-Path $vsWherePath))
    {
        Write-Log Error ( 
            "vswhere.exe '${vsWherePath}' not found. `n" +
            "Could not determine Visual Studio location."
        );
    }
    
    Write-Log Debug  "Invoking vswhere.exe to determine Visual Studio installation path...";
    $vsWhereOutput = exec { & $vsWherePath -prerelease -latest -property productPath -format json }
    Write-Log Information  "Successfully invoked vswhere.exe"
    Write-Log Debug  "vswhere.exe invocation output:`n${vsWhereOutput}";
    
    Write-Log Debug  "Parsing vswhere.exe output to find Visual Studio installation path...";
    $vsPath = ($vsWhereOutput | ConvertFrom-Json)[0].productPath;
    Write-Log Information  "Parsed vswhere.exe output into Visual Studio installation path '${vsPath}'";

    Write-Log Debug  "Validating Visual Studio installation path '${vsPath}'...";
    if (-Not (Test-Path $vsPath ))
    {
        Write-Log Error "vswhere.exe returned an invalid Visual Studio installation path`n'${vsPath}'";
    }

    Write-Step-End "Successfully found Visual Studio installation";
    
    Write-Step-Start "Opening Visual Studio...";

    $slnFilePath = "${SrcDirectory}/Ingot.sln";
    Write-Log Debug  "Invoking Visual Studio to open Ingot solution '${slnFilePath}'...";
    exec { & $vsPath $slnFilePath -donotloadprojects }
    Write-Log Information  "Invoked Visual Studio successfully";

    Write-Step-End "Successfully opened Visual Studio";
}

# ---------------------------------------
# Src Tasks
# ---------------------------------------

# Synopsis: [Specific] Runs csharpier to format the ./src files
task src-format {
    
    Write-Step-Start "Formatting source code...";

    Write-Log Debug  "Invoking 'csharpier' on source directory '${SrcDirectory}'...";
    exec { dotnet csharpier --loglevel "Debug" $SrcDirectory }
    Write-Log Information  "Successfully invoked 'csharpier' on source directory '${SrcDirectory}";

    Write-Step-End "Successfully formatted source code";
}, src-linter

# Synopsis: [Specific] Runs csharpier as a linter to validate the formatting of the ./src files
task src-linter {
    
    Write-Step-Start "Validating source code format...";
    
    Write-Log Debug  "Invoking 'csharpier' on source directory '${SrcDirectory}'...";
    exec { dotnet csharpier --check --loglevel "Debug" $SrcDirectory }
    Write-Log Information  "Successfully invoked 'csharpier' on source directory '${SrcDirectory}";

    Write-Step-End "Successfully validated source code format";
}

# Synopsis: [Specific] Restores the ./src code
task src-restore {
    
    Write-Step-Start "Restoring nuget dependencies...";

    Write-Log Debug  "Invoking 'dotnet restore' on source directory '${SrcDirectory}'...";
    exec { dotnet restore $SrcDirectory --verbosity $DotnetVerbosity }
    Write-Log Information  "Successfully invoked 'dotnet restore' on source directory '${SrcDirectory}";

    Write-Step-End "Successfully restored nuget dependencies";
}

# Synopsis: [Specific] Validates the built ./src code
task src-build-validate {

    Write-Step-Start "Validating '${BuildConfiguration}' source build exists...";
    
    $directory = "${SrcDirectory}";
    $fileFilter = "*libs[\/]*[\/]bin[\/]${BuildConfiguration}[\/]*.dll";

    $files = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Write-Files-Found $files -Directory $directory -Filter $fileFilter;
    Assert-Files-Found $files -Directory $directory -Filter $fileFilter;
    
    Write-Step-End "Successfully validated '${BuildConfiguration}' source build exists"
}

# Synopsis: [Specific] Builds the ./src code
task src-build `
    src-restore, `
{
    Write-Step-Start "Building '${BuildConfiguration}' source...";

    Write-Log Debug  "Invoking 'dotnet build' to build '${SrcDirectory}' with configuration '${BuildConfiguration}'...";
    exec { dotnet build $SrcDirectory -c $BuildConfiguration --no-restore --verbosity $DotnetVerbosity }
    Write-Log Information  "Successfully invoked 'dotnet build' to build '${SrcDirectory}' with configuration '${BuildConfiguration}'";

    Write-Step-End "Successfully built '${BuildConfiguration}' source";
}, `
    src-build-validate

# Synopsis: [Specific] Generates tests result report
task src-test-result `
     src-test-validate, `
{
    Write-Step-Start "Generating test result summary...";

    $summaryFile = "${TestResultDirectory}/test-result-summary.md";

    Write-Log Debug  "Invoking 'liquid' to generate test result summary '${summaryFile}' from trx files...";
    Write-Log Trace  "dotnet liquid --inputs `"File=${TestResultDirectory}/**.trx;Format=Trx`" --template `"${BuildRoot}/build/liquid/test-report-summary.md`" --output-file `"${summaryFile}`"";
    exec { dotnet liquid --inputs "File=${TestResultDirectory}/**.trx;Format=Trx" --template "${BuildRoot}/build/liquid/test-report-summary.md" --output-file "${summaryFile}"; }
    Write-Log Information  "Successfully invoked 'liquid' to generate test result summary";

    $fileFilter = "summary.md";
    $directory = "${TestResultDirectory}";

    $summary = Get-ChildItem $directory -File -Recurse | Where-Object {$_.Name -like $fileFilter};
    Write-Files-Found $summary -Directory $directory -Filter $fileFilter;

    Write-Step-Start "Successfully generated test result summary";
}

# Synopsis: [Specific] Cleans test result report
task src-test-result-clean {

    Write-Step-Start "Cleaning test result summary file...";

    $summaryFile = "${TestResultDirectory}/test-result-summary.md";
    $file = Get-ChildItem -Path $summaryFile;
    
    if ($file)
    {
        Write-Log Trace "Found test result summary file '${summaryFile}'" -Data  $file;
        
        Write-Log Debug "Deleting test result summary file '${summaryFile}'...";
        Remove-Item $file -Force;
        Write-Log Information "Successfully deleted test result summary file '${summaryFile}'...";
    }
    else
    {
        Write-Log Information "Test result summary file '${summaryFile}' not found";
    }

    Write-Step-End "Successfully cleant test result summary file";
}

# Synopsis: [Specific] Cleans the test result outputs
task src-test-clean {

    Write-Step-Start "Cleaning test result directory...";

    Remove-Directory $TestResultDirectory;

    Write-Step-End "Successfully cleant test result directory";
}

task src-test-validate {

    Write-Step-Start "Getting test projects...";

    $directory = "${SrcDirectory}";
    $fileFilter = "*Tests.csproj";

    $testProjects = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Write-Files-Found $testProjects -Directory $directory -Filter $fileFilter;
    Assert-Files-Found $testProjects -Directory $directory -Filter $fileFilter;

    Write-Step-End "Successfully found $($testProjects.Count) test projects";
    
    $testProjects | ForEach-Object -Process {

        Write-Step-Start "Validating run of '$($_.Name)' created trx file...";
        
        $directory = "${codeCoverageOutputFile}";
        $fileFilter = "*.trx";

        $trxs = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
        Write-Files-Found $trxs -Directory $directory -Filter $fileFilter;
        Assert-Files-Found $trxs -Directory $directory -Filter $fileFilter;

        Write-Step-End "Successfully validated run of '$($_.Name)' created trx file";
        
        Write-Step-Start "Validating run of '$($_.Name)' collected code coverage...";
        
        $directory = "${codeCoverageOutputFile}";
        $fileFilter = "*.coverage";

        $codeCoverages = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
        Write-Files-Found $codeCoverages -Directory $directory -Filter $fileFilter;
        Assert-Files-Found $codeCoverages -Directory $directory -Filter $fileFilter;

        Write-Step-End "Successfully validated run of '$($_.Name)' collected code coverage";
    }
}

# Synopsis: [Specific] Tests the built ./src code
task src-test `
    src-test-clean, 
    src-coverage-clean, `
    src-build-validate, `
    src-restore, `
{
    # Allows to fail the build even if some
    # tasks in the flow are allowed to fail
    # to continue subsequent tasks
    $script:propagateErrorsToBuild=$True;

    Write-Step-Start "Getting test projects...";

    $directory = "${SrcDirectory}";
    $fileFilter = "*Tests.csproj";

    $testProjects = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Write-Files-Found $testProjects -Directory $directory -Filter $fileFilter;
    Assert-Files-Found $testProjects -Directory $directory -Filter $fileFilter;

    Write-Step-End "Successfully found $($testProjects.Count) test projects";

    $exitCodes = 0;
    $testProjects | ForEach-Object -Process {

        Write-Step-Start "Running tests declared in '$($_.Name)'...";

        $codeCoverageOutputFile = "${TestResultDirectory}/$($_.BaseName)";

        Write-Log Debug  "Invoking 'dotnet test' on '$($_.FullName)'`nand collecting code coverage to '${codeCoverageOutputFile}'...";
        exec { dotnet test $_.FullName -c $BuildConfiguration --no-build --verbosity $DotnetVerbosity --results-directory $codeCoverageOutputFile --collect "Code Coverage" --logger "trx"; }
        if ($LASTEXITCODE -ne 0)
        {
            $exitCodes = 1;
            Write-Log Warning "Failed to invoke 'dotnet test' on '$($_.FullName)', exit code ${exitCode}";
        }
        else {
            Write-Log Information "Successfully invoked 'dotnet test' on '$($_.FullName)'";
        }
        
        Write-Step-End "Successfully ran tests declared in '$($_.Name)'";
    }

    if ($exitCodes -ne 0)
    {
        Write-Log Error "Some tests have failed. Check logs for more details."
    }

}, src-test-validate   

# Synopsis: [Specific] Cleans the code coverage outputs
task src-coverage-clean {

    Write-Step-Start "Cleaning code coverage directory...";

    Remove-Directory $TestCoverageDirectory;

    Write-Step-End "Successfully cleant code coverage directory";
}

# Synopsis: [Specific] Generates code coverage reports
task src-coverage `
    src-coverage-clean, `
     src-test-validate, `
{
    Write-Step-Start "Retrieving tests code coverage...";

    $directory = "${TestResultDirectory}";
    $fileFilter = "*.coverage";

    $testsCodeCoverage = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Write-Files-Found $testsCodeCoverage -Directory $directory -Filter $fileFilter;
    Assert-Files-Found $testsCodeCoverage -Directory $Directory -Filter $fileFilter;
    
    Write-Step-End "Successfully found $($testsCodeCoverage.Count) tests code coverage";

    New-MergedCodeCoverage -OutputFormat "coverage" -OutputFileName "test-result.coverage" -TestCoverageFiles $testsCodeCoverage;
    
    New-MergedCodeCoverage -OutputFormat "cobertura" -OutputFileName $TestCoverageCoberturaFileName -TestCoverageFiles $testsCodeCoverage;

    New-CodeCoverage -ReportType "Html";
    New-CodeCoverage -ReportType "JsonSummary";
}, `
src-coverage-serve

task src-coverage-serve -If($Serve) {
    
    Write-Step-Start "Opening coverage report...";

    $coverageHtmlDirectory = Get-CodeCoverageReport-DirectoryPath -ReportType "Html";
    $coverageHtmlIndex = "${coverageHtmlDirectory}\index.html";

    Write-Log Debug  "Invoking '${coverageHtmlIndex}' to open default browser...";
    exec { . $coverageHtmlIndex }
    Write-Log Information  "Successfully invoked '${coverageHtmlIndex}' opening default browser";
    
    Write-Step-End "Successfully opened coverage report";
}

# Synopsis: [Specific] Cleans the nuget output folder
task src-pack-clean {

    Write-Step-Start "Cleaning output directory ${NupkgDirectory}...";
    
    Remove-Directory $NupkgDirectory;
    
    Write-Step-End "Successfully cleant output directory ${NupkgDirectory}";
}

# Synopsis: [Specific] Validates the nuget packages
task src-pack-validate {

    Write-Step-Start "Retrieving nuget packages...";
    
    $nupkgFilter = "*.nupkg";
    $nupkgFiles = Get-ChildItem $NupkgDirectory -recurse | Where-Object {$_.name -like $nupkgFilter};
    Write-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;
    Assert-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;

    Write-Log Debug "Invoking 'minver' to retrieve expected version...";
    $expectedVersion = exec { dotnet minver -v d }
    Write-Log Information  "Successfully retrieved expected version '${expectedVersion}' from 'minver'";
    
    $nupkgFiles | ForEach-Object -Process {
        
        if ($_.FullName -like "*${expectedVersion}.nupkg")
        {
            Write-Log Information "Nuget package '$($_.FullName)' matches expected version ${expectedVersion}";
        }
        else
        {
            Write-Log Error "Nuget package '$($_.FullName)' does not match expected version ${expectedVersion}";
        }
    }
    
    Write-Step-End "Successfully validated nuget packages";
}

# Synopsis: [Specific] Packages the built ./src code into nuget packages *.nupkg
task src-pack `
    src-build-validate, `
    src-restore, `
    src-pack-clean, `
{
    Write-Step-Start "Creating nuget packages...";

    Write-Log Debug "Invoking 'dotnet pack' on source directory '${SrcDirectory}'...";
    exec { dotnet pack $SrcDirectory --no-build --output $NupkgDirectory --verbosity $DotnetVerbosity }
    Write-Log Information "Successfully invoked 'dotnet pack' on source directory '${SrcDirectory}'";
    
    Write-Step-End "Successfully created nuget packages";
}, `
    src-pack-validate

# Synopsis: [Specific] Publishes the packaged *.nupkg to a local feed
task src-publish-local -If(-Not (Test-CI-ExecutionEnvironment)) `
    tool-nuget-setup, `
    src-pack-validate, `
{
    Write-Step-Start "Retrieving nuget local push source...";

    Write-Log Debug "Invoking 'dotnet nuget' to get config 'defaultPushSource'";
    $nugetLocalSource = exec { dotnet nuget config get "defaultPushSource" };
    Write-Log Information "Successfuly invoked 'dotnet nuget' to get config 'defaultPushSource'";
    
    if (-Not $nugetLocalSource)
    {
        Write-Error "Could not retrieve nuget local push source";
    }

    Write-Step-End "Successfully retrieved local push source '${nugetLocalSource}'";

    Write-Step-Start "Retrieving nupkg files...";

    New-Directory $nugetLocalSource;

    $nupkgFilter = "*.nupkg";
    $nupkgFiles = Get-ChildItem $NupkgDirectory -recurse | Where-Object {$_.name -like $nupkgFilter};
    Write-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;
    Assert-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;

    Write-Step-End "Successfully retrieved $($nupkgFiles.Count) nupkg files";

    $nugetLocalFeedName = "ingot-local-feed";
    $nupkgFiles | ForEach-Object -Process {
        
        Write-Step-Start "Publishing nupkg '$($_.Name)' to '${nugetLocalFeedName}'...";

        $localFeedFilePath = "${nugetLocalSource}/$($_.Name)";
        Remove-File $localFeedFilePath;

        Write-Log Debug "Invoking 'dotnet nuget' to push nupkg '$($_.FullName)' to '${localFeedFilePath}'";
        exec { dotnet nuget push $_.FullName };
        Write-Log Information "Sucessfully invoked 'dotnet nuget' to push nupkg '$($_.FullName)' to '${localFeedFilePath}'";
        
        Write-Step-End "Successfully published nupkg '$($_.Name)' to '${nugetLocalFeedName}'";
    }
}

# Synopsis: [Specific] Publishes the packaged *.nupkg to a remote feed
task src-publish-remote -If(($NupkgPushSource) -And ($NupkgPushApiKey)) `
    src-pack-validate, `
{    
    Write-Step-Start "Retrieving nupkg files...";
    
    $nupkgFilter = "*.nupkg";
    $nupkgFiles = Get-ChildItem $NupkgDirectory -recurse | Where-Object {$_.name -like $nupkgFilter};
    Write-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;
    Assert-Files-Found $nupkgFiles -Directory $NupkgDirectory -Filter $nupkgFilter;

    Write-Step-End "Successfully retrieved $($nupkgFiles.Count) nupkg files";

    $nupkgFiles | ForEach-Object -Process {
        
        Write-Step-Start "Publishing nupkg '$($_.Name)' to '${NupkgPushSource}'...";

        Write-Log Debug "Invoking 'dotnet nuget' to push nupkg '$($_.FullName)' to '${NupkgPushSource}'";
        exec { dotnet nuget push $_.FullName --api-key $NupkgPushApiKey --source $NupkgPushSource --skip-duplicate };
        Write-Log Information "Sucessfully invoked 'dotnet nuget' to push nupkg '$($_.FullName)' to '${NupkgPushSource}'";
        
        Write-Step-End "Successfully published nupkg '$($_.Name)' to '${NupkgPushSource}'";
    }
}

task Docs-Clean {
    REmove-Item -Force -Recurse ./docs/docs/Mediator/References -ErrorAction SilentlyContinue
    REmove-Item -Force -Recurse ./docs/docs/Mediator/Advanced/References -ErrorAction SilentlyContinue
    REmove-Item -Force -Recurse ./docs/_site -ErrorAction SilentlyContinue
}

task Docs-Build {
    dotnet docfx ./docs/docfx.json
}

task Docs-Serve {
    dotnet docfx serve ./docs/_site --open-browser
}

task Docs Docs-Clean, Docs-Build, Docs-Serve
