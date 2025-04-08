param(

    [ValidateSet('trace', 'debug', 'information', 'warning', 'error')]
    [string] $Verbosity = 'trace',

    [ValidateSet('q', 'quiet', 'm', 'minimal', 'n', 'normal', 'd', 'detailed', 'diag', 'diagnostic')]
    [string] $DotnetVerbosity = 'normal',

    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Release',

    [string] $IBExtensionsDirectory = './build/extensions',

    [string] $SrcDirectory = './src',

    [string] $MainSolutionFileNameWithExtension = 'Vesture.sln',

    [string] $SamplesDirectory = './samples',

    [string] $NupkgDirectory = './dist/nuget',

    [string] $TestResultDirectory = './dist/test-result',

    [string] $TestCoverageDirectory = './dist/test-coverage',

    [string] $TestCoverageCoberturaFileName = 'test-result.cobertura.coverage',

    [string] $TestCoverageReportDirectory = './dist/test-coverage/report',
    
    [string] $DocsDirectory = './docs',
    
    [string] $DocsOutputDirectory = './dist/docs',

    [string] $NupkgPushSource,
    
    [string] $NupkgPushApiKey,

    [switch] $Serve,
    
    [int] $ServePort = 8187,

    [switch] $Force
)

# ***************************************
# 
#
#           Functions
# 
#
# ***************************************

# ---------------------------------------
# Environment
# ---------------------------------------

# Synposis: Adds an execution environment to '$script:ExecutionEnvironments'
function Add-ExecutionEnvironment {
    
    param (
        $Name,
        $Enabled
    )

    $executionEnvironment = [pscustomobject]@{
        Name = $Name;
        Enabled = $Enabled;
    };

    $script:ExecutionEnvironments += @($executionEnvironment);
}

function Test-CI-ExecutionEnvironment {
    return Test-Path env:CI;
}

function Test-Local-ExecutionEnvironment {
    return -Not (Test-CI-ExecutionEnvironment);
}

function Test-Shell-Is-Pwsh {
    return ($PSVersionTable.PSEdition -eq "Core");
}

# ---------------------------------------
# Logging
# ---------------------------------------

enum LogLevel {
    Error = 1
    Warning = 2
    Information = 4
    Debug = 8
    Trace = 16
 }

function Set-MinimumLogLevel {
    param (
        $Level
    )

    if (-Not $script:MinimumLogLevel)
    {
        $minimumLogLevel = [LogLevel]::Trace;
        $minimumLogLevel += [LogLevel]::Debug;
        $minimumLogLevel += [LogLevel]::Information;
        $minimumLogLevel += [LogLevel]::Warning;
        $minimumLogLevel += [LogLevel]::Error;

        if ($Level -ne 'trace') {  
            $minimumLogLevel -= [LogLevel]::Trace;
            
            if ($Level -ne 'debug') {  
                $minimumLogLevel -= [LogLevel]::Debug;
                
                if ($Level -ne 'information') {  
                    $minimumLogLevel -= [LogLevel]::Information; 
                    
                    if ($Level -ne 'warning') {  
                        $minimumLogLevel -= [LogLevel]::Warning; 
                    }
                }
            }
        }

        $script:MinimumLogLevel = $minimumLogLevel;
    }
}

function Write-Log {
    param (
        [LogLevel] $Level,
        $Content = $undefined,
        $Data = $undefined
    )

    if ($Level -band $script:MinimumLogLevel)
    {
        if ($Level -eq [LogLevel]::Trace) { Write-Build DarkGray $Content; }
        if ($Level -eq [LogLevel]::Debug) { Write-Build DarkGray $Content; }
        if ($Level -eq [LogLevel]::Information) { Write-Build White $Content; }
        if ($Level -eq [LogLevel]::Warning) { Write-Warning $Content; }

        if ($Level -eq [LogLevel]::Error) { 
            
            $errorInfo = (
                "`n`u{2717}  ERROR`n" +
                "----------------------------------------------------------------------`n" +
                "${Content}`n`n" +
                "Kindly check logs for more details.`n"
            );
            
            Write-Error $errorInfo; 
        }

        if ($Data) { $Data }
    }
}

# ---------------------------------------
# Write
# ---------------------------------------

function Write-Step-Start {

    param (
        $Content
    )

    Write-Build Magenta "`n`u{25A2}  ${Content}";
}

function Write-Step-End {

    param (
        $Content,
        $Color
    )

    Write-Build Green "`u{2713}  ${Content}";
}

function Write-File-Content {

    param (
        $File,
        $Content
    )

    Write-Log Trace  -Content (
        "File '${File}' content:`n" +
        "----------------------------------`n" +
        "${Content}`n" +
        "----------------------------------"
    );
}

function Write-Files-Found {
    
    param (
        $Files,
        [string] $Directory,
        [string] $Filter
    )

    if (-Not $Filter) 
    {
        Write-Log Information "Found $($Files.Count) files in directory '${Directory}'" -Data $Files; 
    }
    else 
    {
        Write-Log Information "Found $($Files.Count) files matching filter '${Filter}' in directory '${Directory}'" -Data $Files;
    }
}

# ---------------------------------------
# Functional
# ---------------------------------------

function Assert-Files-Found {
    param (
        $Files,
        [string] $Directory,
        [string] $Filter
    )

    if ($Files.Count -eq 0)
    {
        if (-Not $Filter)
        {
            $Content = "No files found in directory '${Directory}'";
        }
        else 
        {
            $Content = "No files found in directory '${Directory}' matching filter '${Filter}'";
        }

        Write-Log Error $Content;
    }
}

function Remove-Directory {
    param ([string] $Path)

    # If directory does not exist, don't do anything
    Write-Log Trace  "Checking if directory '${Path}' exists...";
    $directory = Get-Item -Path $Path -ErrorAction SilentlyContinue;
    if (-Not ($directory))
    {
        Write-Log Trace  "Directory '${Path}' does not exist";
        return;
    }
    Write-Log Trace  "Directory '${Path}' exists" -Data $directory;

    # Else deletes it and its files
    $files = Get-ChildItem $Path -File -Recurse;
    Write-Log Debug "Deleting directory '${Path}' and $($Files.Count) children files" -Data $files;
    Remove-Item -Recurse -Force $Path;
    Write-Log Information  "Deleted directory '${Path}' and its $($files.Count) files"
}

function Remove-File {
    param ([string] $Path)

    if (Test-Path $Path)
    {
        Write-Log Trace "Removing existing file '${Path}'...";
        Remove-Item $Path -Force;
        Write-Log Debug "File '${Path}' deleted";
    }
    else {
        Write-Log Debug "File '${Path}' does not exist";
    }
}

function New-Directory { 
    param ([string] $Path)

    # If directory exists already, don't do anything
    Write-Log Trace "Checking if directory '${Path}' exists...";
    $directory = Get-Item -Path $Path -ErrorAction SilentlyContinue;
    if ($directory)
    {
        Write-Log Trace "Directory '${Path}' exists already" -Data $directory;
        return;
    }
    Write-Log Trace  "Directory '${Path}' does not exist";

    # Else create the directory
    Write-Log Debug  "Creating directory '${Path}'...";
    $directory = New-Item -ItemType Directory -Force -Path $Path
    Write-Log Information "Created directory '${Path}'";
    Write-Log Trace -Data $directory;
}

# ***************************************
# 
#
#      Shell and Script Setup
# 
#
# ***************************************

# Set output rendering to use ANSI code
if ($PsStyle)
{
    $PsStyle.OutputRendering = "Ansi";
}

# Set the minimum log level
Set-MinimumLogLevel $Verbosity;

# Adds the default execution environments
Add-ExecutionEnvironment -Name "Local" -Enabled $(Test-Local-ExecutionEnvironment);
Add-ExecutionEnvironment -Name "CI" -Enabled $(Test-CI-ExecutionEnvironment);

# ***************************************
# 
#
#           Invoke Build Hooks
# 
#
# ***************************************

Enter-Build {

    # Prints environment information
    Write-Step-Start "Detecting execution environment..."
    
    $script:ExecutionEnvironments | Format-Table;
    
    # Warns the user the setup is not finished if they
    # have not restarted their program with pwsh as default shell
    if (-Not ($BuildTask -eq "setup"))
    {
        Write-Step-Start "Detecting use of PowerShell Core (pwsh)..."

        if (-Not (Test-Shell-Is-Pwsh))
        {
            if (Test-CI-ExecutionEnvironment)
            {
                Write-Log Error (
                    "PowerShell Core (pwsh) enabled but not active`n" +
                    "To ensure consistency between all environment, you must use`n" +
                    "Invoke-Build with PowerShell Core (pwsh) either by using --pwsh option`n" +
                    "or by setting environment variable 'pwsh=pwsh'"
                );
            }
            else
            {
                $msg = (
                    "PowerShell Core (pwsh) enabled but not active`n" +
                    "Ensure to refresh environment variables by restarting your program"
                );
                if($env:TERM_PROGRAM -eq 'vscode') {
                    $msg += "`nIt appears you are using Visual Studio Code (VSCode), ensure to close VSCode completely, not just the terminal.";      
                }

                Write-Log Warning $msg;
            }
        }
        else
        {
            Write-Step-End "PowerShell Core (pwsh) in use";
        }
    }

    Write-Build DarkGray "";
}

Enter-BuildJob {

    # Allows for nicer output
    Write-Build DarkGray "";
}

Exit-BuildJob {
    
    # Allows for nicer output
    Write-Build DarkGray "";
}

Exit-Build {

    if ($propagateErrorsToBuild -And (-Not (${*}.Errors.Count -eq 0)))
    {
        $msg = ("`n`nTask flow '${BuildTask}' is not allowed to fail and encountered some errors");
        ${*}.Errors | ForEach-Object -Process {
            $msg += "`n----------------------------------------------------------------------`n"
            $msg += "`n'$($_.Task.Name)':`n" +
            "$($_.Error)";
        }

        $msg += "`n----------------------------------------------------------------------`n"
        Write-Build Red $msg;
        Write-Error "Task flow '${BuildTask}' failed";
    }
}

# ***************************************
# 
#
#           Invoke Build Tasks
# 
#
# ***************************************
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

# Update
# ---------------------------------------
# Synopsis: Updates the repository's various dependencies
task update `
    repo-dotnet-tools-update, `
    repo-nuget-packages-update
    
# Update
# ---------------------------------------
# Synopsis: Verifies if an update has to be done on the repo
task update-verify `
    update, `
    repo-validate-pristine

# Setup
# ---------------------------------------
# Synopsis: Runs the initial setup for the repo
task setup `
    ci-setup-before, `
    ?ci-setup, `
    ci-setup-finally

# Clean
# ---------------------------------------
# Synopsis: Cleans output from previous runs
task clean `
    ci-clean-before, `
    ?ci-clean, `
    ci-clean-finally

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
    publish, `
    test-sample
    
# IDE
# ---------------------------------------
# Synopsis: Opens repository's solution (.sln) in Visual Studio
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

# Test Samples
# ---------------------------------------
# Synopsis: Tests samples
task test-sample `
    ci-test-sample-before, `
    ?ci-test-sample, `
    ci-test-sample-finally

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

# Docs
# ---------------------------------------
# Synopsis: Builds the docs, use -Serve to open the browser
task docs `
    ci-docs-before, `
    ?ci-docs, `
    ci-docs-finally

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
    tool-minver-setup, `
    tool-minver-validate

# Clean
# ---------------------------------------
task ci-clean-before -If($False)
task ci-clean-finally -If($False) 

task ci-clean `
    src-test-result-clean, `
    src-test-clean, `
    src-coverage-clean, `
    src-pack-clean, `
    src-sample-publish-clean

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
    tool-minver-setup, `
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
    
# Test Samples
# ---------------------------------------
task ci-test-sample-before -If($False) 
task ci-test-sample-finally -If($False) 

task ci-test-sample `
    src-sample-publish, `
    src-sample-validate

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
    tool-minver-setup, `
    src-pack

# Publish
# ---------------------------------------
task ci-publish-before -If($False) 
task ci-publish-finally -If($False) 

task ci-publish `
    tool-minver-setup, `
    src-pack-validate, `
    src-publish-local, `
    src-publish-remote


# Docs
# ---------------------------------------
task ci-docs-before -If($False) 
task ci-docs-finally -If($False) 

task ci-docs `
    src-docs-clean, `
    src-docs-build, `
    src-docs-serve

# ---------------------------------------
# Repository Tasks
# ---------------------------------------

# Synopsis: [Specific] Updates the dotnet tools to latest
task repo-dotnet-tools-update -If(Test-Local-ExecutionEnvironment) {

    # Updates all dotnet tools
    Write-Step-Start "Updating dotnet tools...";
    
    Write-Log Debug  "Invoking 'dotnet' to get a list of tools...";
    $tools = exec { dotnet tool list }
    Write-Log Information  "Successfully retrieved dotnet tools" -Data $tools;

    for (($i = 2); $i -lt $tools.Length; $i++)
    {
        $toolParsed = $tools[$i] -Split " " | Where-Object -FilterScript { $_ -ne "" }
        $tool = $toolParsed[0];
        $version = $toolParsed[1];

        Write-Log Debug  "Invoking 'dotnet' to update tool '$tool'...";
        
        if ($version -like "*-*")
        {
            exec { dotnet tool update $tool --prerelease }
        }
        else {
            exec { dotnet tool update $tool }
        }

        Write-Log Information  "Successfully invoked 'dotnet' to update tool '$tool'";
    }

    Write-Step-End "Successfully updated dotnet tools";
}

# Synopsis: [Specific] Updates the nuget packages to latest
task repo-nuget-packages-update -If(Test-Local-ExecutionEnvironment) {
    
    Write-Step-Start "Updating nuget packages...";

    Write-Log Debug  "Invoking 'dotnet outdated' to update nuget packages...";
    exec { dotnet outdated $SrcDirectory -u }
    Write-Log Information  "Successfully invoked dotnet 'dotnet outdated'";

    Write-Step-End "Successfully updated nuget packages";
}

task repo-validate-pristine -If(Test-Local-ExecutionEnvironment) {

    Write-Step-Start "Ensuring git repository is pristine...";

    Write-Log Debug  "Invoking 'git' to validate status...";
    $files = exec { git status --porcelain }
    Write-Log Information  "Successfully invoked 'git' to retrieved changed files" -Data $files;

    if ($files)
    {
        Write-Error "Git repository is not pristine, check logs for more details";
    }
    else {
        Write-Step-End "Git repository is pristine";
    }
}

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
                    "See https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell " +
                    "for more details on how to install PowerShell Core."
                );
            }
        }
    }
}

# Synopsis: [Specific] Setup minver pre-release identifiers
task tool-minver-setup -If(-Not ($env:MinVerDefaultPreReleaseIdentifiers)) {
    
    Write-Step-Start "Setting up MinVer...";

    if (Test-Local-ExecutionEnvironment)
    {
        Write-Log Debug "Setting MinVer computed pre-release identifiers for local version...";
        $script:MinVerDefaultPreReleaseIdentifiers = "alpha.0.local"
        $env:MinVerDefaultPreReleaseIdentifiers = $script:MinVerDefaultPreReleaseIdentifiers;
        Write-Log Information "Set MinVer computed pre-release identifiers to '$($script:MinVerDefaultPreReleaseIdentifiers)'...";
    }
    
    Write-Step-End "Successfuly setup MinVer"
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
task tool-nuget-setup {

    Write-Step-Start "Retrieving nuget local push source...";

    Write-Log Debug "Invoking 'dotnet nuget' to get config 'defaultPushSource'";
    $nugetLocalSource = exec { dotnet nuget config get "defaultPushSource" };
    Write-Log Information "Successfuly invoked 'dotnet nuget' to get config 'defaultPushSource'";
    
    if (-Not $nugetLocalSource)
    {
        Write-Error "Could not retrieve nuget local push source";
    }

    Write-Step-End "Successfully retrieved local push source '${nugetLocalSource}'";

    Write-Step-Start "Creating local NuGet source...";

    $nugetLocalFeedValue = "./dist/local-nuget-feed";

    New-Directory $nugetLocalFeedValue;

    Write-Step-End "Successfully created local NuGet source";
}

# Synopsis: [Specific] Opens the repository's solution (.sln) in Visual Studio
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

    $slnFilePath = "${SrcDirectory}/${MainSolutionFileNameWithExtension}";
    Write-Log Debug  "Invoking Visual Studio to open the repository's solution (.sln) '${slnFilePath}'...";
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

# Synopsis: [Specific] Validates tests result report
task src-test-result-validate {
    Write-Step-Start "Validating test result summary file...";

    $summaryFile = "${TestResultDirectory}/test-result-summary.md";
    $file = Get-ChildItem -Path $summaryFile;
    
    if ($file)
    {
        Write-Log Trace "Found test result summary file '${summaryFile}'" -Data  $file;
    }
    else
    {
        Write-Log Error "Test result summary file '${summaryFile}' not found";
    }

    Write-Step-End "Successfully validated test result summary file";
}

# Synopsis: [Specific] Generates tests result report
task src-test-result `
     src-test-validate, `
{
    Write-Step-Start "Generating test result summary...";

    $summaryFile = "${TestResultDirectory}/test-result-summary.md";

    Write-Log Debug  "Invoking 'liquid' to generate test result summary '${summaryFile}' from trx files...";
    Write-Log Warning "'liquid' currently does not support multi-targetted test results. Skipping"
    # exec { dotnet liquid --inputs "File=${TestResultDirectory}/**.trx;Format=Trx" --template "./build/liquid/test-report-summary.md" --output-file "${summaryFile}"; }
    Write-Log Information  "Successfully invoked 'liquid' to generate test result summary";

    $fileFilter = "summary.md";
    $directory = "${TestResultDirectory}";

    $summary = Get-ChildItem $directory -File -Recurse | Where-Object {$_.Name -like $fileFilter};
    Write-Files-Found $summary -Directory $directory -Filter $fileFilter;

    Write-Step-Start "Successfully generated test result summary";
}
# , `
    # src-test-result-validate

# Synopsis: [Specific] Cleans test result report
task src-test-result-clean {

    Write-Step-Start "Cleaning test result summary file...";

    $summaryFile = "${TestResultDirectory}/test-result-summary.md";
    $file = Get-ChildItem -Path $summaryFile -ErrorAction SilentlyContinue;
    
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
task src-publish-local `
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

    $nugetLocalFeedName = "local-nuget-feed";
    $nupkgFiles | ForEach-Object -Process {
        
        Write-Step-Start "Publishing nupkg '$($_.Name)' to '${nugetLocalFeedName}'...";

        $localFeedFilePath = "${nugetLocalSource}/$($_.Name)";
        Remove-File $localFeedFilePath;

        Write-Log Debug "Invoking 'dotnet nuget' to push nupkg '$($_.FullName)' to '${localFeedFilePath}'";
        exec { dotnet nuget push $_.FullName };
        Write-Log Information "Sucessfully invoked 'dotnet nuget' to push nupkg '$($_.FullName)' to '${localFeedFilePath}'";
        
        Write-Step-End "Successfully published nupkg '$($_.Name)' to '${nugetLocalFeedName}'";
    }
}, `
    src-sample-update-nuget

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

# Synopsis: [Specific] Updates the samples nuget packages to use latest available package
task src-sample-update-nuget tool-minver-setup, {

    Write-Step-Start "Finding samples projects...";

    $fileFilter = "*.csproj";
    $sampleProjects = Get-ChildItem $SamplesDirectory -Filter $fileFilter -Recurse;
    Write-Files-Found $sampleProjects -Directory $SamplesDirectory -Filter $fileFilter;
    Assert-Files-Found $sampleProjects -Directory $SamplesDirectory -Filter $fileFilter;

    Write-Step-End "Successfully found $($sampleProjects.Count) sample projects";

    Write-Log Debug  "Invoking 'minver' to compute version...";
    $version = exec { dotnet minver -v d }
    Write-Log Information  "Retrieved MinVer computed version '${version}'";

    $sampleProjects | ForEach-Object -Process {

        Write-Step-Start "Update sample project '$($_.Name)' with latest nuget...";

        Write-Log Debug "Invoking 'dotnet restore' to force updating the sample project '$($_.FullName)' with latest local version";
        exec { dotnet restore $($_.FullName) --force }
        Write-Log Information "Successfuly invoked 'dotnet restore' to update the sample project '$($_.FullName)' with latest local version";
        
        Write-Log Debug "Invoking 'dotnet list' to list packages used by the sample project '$($_.FullName)'...";
        $versions = exec { dotnet list $($_.FullName) package }
        
        if ($versions)
        {
            Write-Log Information "Successfully invoked 'dotnet list' to list packages used by the sample project '$($_.FullName)'" -Data $versions;
        }
        else {
            Write-Log Error "Versions returned by 'dotnet list' is empty for the sample project '$($_.FullName)'"
        }

        Write-Log Debug "Ensuring MinVer version '${version}' is present in the sample project '$($_.FullName)' nuget config...";
        if ($($versions -join "") -like "*${version}*")
        {
            Write-Log Information "MinVer version '${version}' is present in the sample project '$($_.FullName)' nuget config";
        }
        else {
            Write-Log Error "MinVer version '${version}' is not present in the sample project '$($_.FullName)' nuget config";
        }

        Write-Step-End "Successfully updated sample project '$($_.Name)' with latest nuget";
        
    }
}

task src-sample-publish-clean {

    Write-Step-Start "Cleaning samples output...";
    
    Remove-Directory "./dist/samples";

    Write-Step-End "Successfully cleant samples output...";
}

task src-sample-publish `
    src-sample-publish-clean, 
{
    Write-Step-Start "Detecting OS...";

    # Gets the OS architectur ein order to determine the proper RID
    # $rid = switch ($osArchitecture) {
    #     "X64"  { "" }
    #     "X86"  { "" }
    #     "Arm"  { "" }
    #     "Arm64" { "" }
    #     Default { "" }
    # }
    $osArchitecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture
    Write-Log Information "Determined OS architecture to be '$osArchitecture'";

    if ($IsWindows)
    {
        $extension = ".exe";
        $rid = switch ($osArchitecture) {
            "X64"  { "win-x64" }
            "X86"  { "win-x86" }
            "Arm"  { "" }
            "Arm64" { "win-arm64" }
            Default { "" }
        }

        if ($rid)
        {
            Write-Log Information "Current platform is Windows, using RID '${rid}'";
        }
        else 
        {
            Write-Error "Current platform is Windows but could not determine RID"
        }
    }

    if ($IsLinux)
    {
        $extension = ".bin";
        $rid = switch ($osArchitecture) {
            "X64"  { "linux-x64" }
            "X86"  { "" }
            "Arm"  { "linux-arm" }
            "Arm64" { "linux-arm64" }
            Default { "" }
        }

        if ($rid)
        {
            Write-Log Information "Current platform is Linux, using RID '${rid}'";
        }
        else 
        {
            Write-Error "Current platform is Linux but could not determine RID"
        }
    }

    if ($IsMacOS)
    {
        $extension = ".bin";
        $rid = switch ($osArchitecture) {
            "X64"  { "osx-x64" }
            "X86"  { "" }
            "Arm"  { "" }
            "Arm64" { "osx-arm64" }
            Default { "" }
        }

        if ($rid)
        {
            Write-Log Information "Current platform is MacOS, using RID '${rid}'";
        }
        else 
        {
            Write-Error "Current platform is MacOS but could not determine RID"
        }
    }

    Write-Step-End "Successfully detected OS";

    $sampleOutputDirectory = "./dist/samples";

    Write-Step-Start "Retrieving sample projects...";

    $fileFilter = "*.csproj";
    $sampleProjects = Get-ChildItem $SamplesDirectory -Filter $fileFilter -Recurse;
    Write-Files-Found $sampleProjects -Directory $SamplesDirectory -Filter $fileFilter;
    Assert-Files-Found $sampleProjects -Directory $SamplesDirectory -Filter $fileFilter;

    Write-Step-End "Successfully found $($sampleProjects.Count) sample projects";

    $sampleProjects | ForEach-Object -Process {

        Write-Step-Start "Publishing sample '$($_.Name)' for runtime '${rid}'...";

        $nameWithoutExtension = $_.Name -replace $_.Extension, "";
        $applicationName = "${nameWithoutExtension}${extension}";
        $outputFolder = "${sampleOutputDirectory}/${nameWithoutExtension}";

        New-Directory $outputFolder;

        Write-Log Debug  "Invoking 'dotnet publish' to publish '$($_.FullName)' for runtime '${rid}'...";
        exec { dotnet publish $_.FullName -c 'Release' --verbosity $DotnetVerbosity -o $outputFolder -r $rid -p "AssemblyName=${applicationName}" }
        Write-Log Information  "Successfully invoked 'dotnet publish' to publish '$($_.FullName)' for runtime '${rid}'";
        
        Write-Step-End "Successfully published sample '$($_.Name)' for runtime '${rid}'";
    }
}

task src-sample-validate {

    Write-Step-Start "Detecting OS...";

    if ($IsWindows)
    {
        $rid = "win-x64";
        $fileFilter = "*.exe";
        Write-Log Information "Current platform is Windows, using RID '${rid}'";
    }

    if ($IsLinux)
    {
        $rid = "linux-x64";
        $fileFilter = "*.bin";
        Write-Log Information "Current platform is Linux, using RID '${rid}'";
    }

    if ($IsMacOS)
    {
        $rid = "osx-x64";
        $fileFilter = "*.bin";
        Write-Log Information "Current platform is MacOS, using RID '${rid}'";
    }

    Write-Step-End "Successfully detected OS";

    $sampleOutputDirectory = "./dist/samples";

    Write-Step-Start "Retrieving sample executables...";

    $sampleExecutables = Get-ChildItem $sampleOutputDirectory -Filter $fileFilter -Recurse;
    Write-Files-Found $sampleExecutables -Directory $sampleOutputDirectory -Filter $fileFilter;
    Assert-Files-Found $sampleExecutables -Directory $sampleOutputDirectory -Filter $fileFilter;

    Write-Step-End "Successfully found $($sampleExecutables.Count) sample executables";

    $exitCodes = 0;
    $sampleExecutables | ForEach-Object -Process {

        Write-Step-Start "Validating sample executable '$($_.Name)'...";
        
        exec { & $($_.FullName) }
        
        if ($LASTEXITCODE -ne 0)
        {
            $exitCodes = 1;
            Write-Log Warning "Failed to validate sample executable '$($_.FullName)', exit code ${exitCode}";
        }
        else {
            Write-Step-End "Successfully validated sample executable '$($_.Name)'";
        }
    }

    if ($exitCodes -ne 0)
    {
        Write-Log Error "Some sample executables have failed. Check logs for more details."
    }
}

# ---------------------------------------
# Docs Tasks
# ---------------------------------------

task src-docs-clean {
    
    Write-Step-Start "Cleaning documentation output '$DocsOutputDirectory'...";

    Remove-Item -Force -Recurse $DocsOutputDirectory -ErrorAction SilentlyContinue
    
    Write-Step-End "Successfully cleant documentation output '$DocsOutputDirectory'";

    Write-Step-Start "Retrieving documentation reference directories...";

    $directoryFilter = "References";
    $referenceDirectories = Get-ChildItem $DocsDirectory -Recurse -Directory -Filter $directoryFilter
    Write-Files-Found $referenceDirectories -Directory $DocsDirectory -Filter $directoryFilter;

    Write-Step-End "Successfully found $($referenceDirectories.Count) sample documentation reference directories";

    $referenceDirectories | ForEach-Object -Process {

        Write-Step-Start "Deleting documentation reference directory '$($_.FullName)'...";
        Remove-Item -Force -Recurse $($_.FullName) -ErrorAction SilentlyContinue;
        Write-Step-End "Successfully deleted documentation reference directory '$($_.FullName)'";
    }
}

task src-docs-build {
    
    Write-Step-Start "Building documentation...";

    Write-Log Debug  "Invoking 'docfx' to build documentation...";
    exec { dotnet docfx "$DocsDirectory/docfx.json" -o $DocsOutputDirectory }
    Write-Log Information  "Successfully invoked 'docfx' to build documentation";

    Write-Step-End "Successfully built documentation";
}

task src-docs-serve -If($Serve) {

    Write-Step-Start "Opening documentation website...";
    
    Write-Log Debug  "Invoking 'docfx' to serve documentation website...";
    exec { dotnet docfx serve $DocsOutputDirectory --open-browser -p $ServePort }
    Write-Log Information  "Successfully invoked 'docfx' to serve documentation website";

    Write-Step-End "Successfully opened documentation website";
}

# Imports extension files
# GitHub
$ibGitHubExtension = Get-ChildItem "./.github/ib.github.ps1" -File;
if ($ibGitHubExtension) {
    . "$($ibGitHubExtension.FullName)";
}