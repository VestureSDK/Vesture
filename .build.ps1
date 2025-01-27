param(
    [ValidateSet('q', 'quiet', 'm', 'minimal', 'n', 'normal', 'd', 'detailed', 'diag', 'diagnostic')]
    [string] $DotnetVerbosity = 'normal',

    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Release',

    [string] $SrcDirectory = './src',

    [string] $NupkgDirectory = './dist/nuget',

    [string] $TestResultDirectory = './dist/test-result',

    [string] $TestCoverageDirectory = './dist/test-coverage',

    [string] $TestCoverageCoberturaFileName = 'test-result.cobertura.coverage',

    [string] $TestCoverageReportDirectory = './dist/test-coverage/report',

    [string] $NupkgPushSource,
    
    [string] $NupkgPushApiKey,

    [bool] $Force = $False
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

function Ingot-IsOnGitHub {
    return Test-Path env:GITHUB_ACTIONS;
}

function Ingot-IsOnCi {
    return ((Test-Path env:CI) -or (Ingot-IsOnGitHub));
}

function Ingot-IsOnLocal {
    return -Not (Ingot-IsOnCi);
}

# ---------------------------------------
# Write
# ---------------------------------------

function Ingot-Write-Build {

    param (
        $Color,
        $Content,
        $AsCard
    )

    if ($AsCard)
    {
        Write-Build $Color "`n----------------------------------------------------------------------`n";
    }

    Write-Build $Color $Content;
    
    if ($AsCard)
    {
        Write-Build $Color "`n----------------------------------------------------------------------`n";
    }
}

function Ingot-Write-Debug {

    param (
        $Content,
        $AsCard
    )

    Ingot-Write-Build -Color DarkGray -Content $Content -AsCard $AsCard;
}


function Ingot-Write-Info {

    param (
        $Content,
        $AsCard
    )

    Ingot-Write-Build -Color Gray -Content $Content -AsCard $AsCard;
}

function Ingot-Write-Warning {

    param (
        $Content,
        $AsCard
    )

    Ingot-Write-Build -Color Yellow -Content $Content -AsCard $AsCard;
}

function Ingot-Write-Success {

    param (
        $Content,
        $AsCard
    )
    
    Ingot-Write-Build -Color Green -Content $Content -AsCard $AsCard;
}

function Ingot-Error {

    param (
        $Content
    )
    
    $errorInfo = (
        "`n`u{2717}  ERROR (exit 1)`n" +
        "----------------------------------------------------------------------`n" +
        "${Content}`n`n" +
        "Kindly check logs for more details.`n"
    );

    return $errorInfo;
}

function Ingot-Write-StepStart {

    param (
        $Content
    )

    Ingot-Write-Build -Color Magenta -Content "`n`u{25A2}  ${Content}";
}

function Ingot-Write-StepEnd-Success {

    param (
        $Content,
        $Color
    )

    Ingot-Write-Build -Color Green -Content "`u{2713}  ${Content}";
}

function Ingot-Write-StepEnd-Warning {

    param (
        $Content,
        $Color
    )

    Ingot-Write-Warning -Content "`n`u{203C}  ${Content}";
}

function Ingot-Write-FileContent {

    param (
        $File,
        $Content
    )

    Ingot-Write-Debug -Content (
        "File '${File}' content:`n" +
        "----------------------------------`n" +
        "${Content}`n" +
        ">> EOF"
    );
}

function Ingot-Write-FileLookup-Start {

    param (
        $FileFilter,
        $Directory
    )

    if (-Not $FileFilter)
    {
        $FileFilter = "*";
    }

    Ingot-Write-Debug "Getting files matching filter '${FileFilter}' in directory '${Directory}'...";
}

function Ingot-Write-FileLookup-End {
    
    param (
        $FileFilter,
        $Directory,
        $Files
    )

    if (-Not $FileFilter)
    {
        $FileFilter = "*";
    }

    Ingot-Write-Info "Found $($Files.Count) files matching filter '${FileFilter}' in directory '${Directory}'"
    if (-Not ($Files.Count -eq 0))
    {
        $Files | ForEach-Object -Process {
            Ingot-Write-Debug "    - $($_.FullName)";
        }
    }
}

# ---------------------------------------
# Functional
# ---------------------------------------

function Ingot-GitHub-AppendMissingVariable {

    param (
        $Key,
        $Value
    )

    Ingot-Write-StepStart "Appending Ingot environment variable '${Key}' to GitHub environment";

    Ingot-Write-Debug "Ensuring GitHub environment file exists...";
    if ((-Not ($env:GITHUB_ENV)) -Or (-Not (Test-Path $env:GITHUB_ENV)))
    {
        throw Ingot-Error (
            "GitHub environment file '$($env:GITHUB_ENV)' not found`n" +
            "or GITHUB_ENV environment variable undefined"
        );
    }
    Ingot-Write-Info "GitHub environment file exists '$($env:GITHUB_ENV)'";
    
    $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
    Ingot-Write-FileContent -File $env:GITHUB_ENV -Content $githubEnvironmentContent;

    Ingot-Write-Debug "Checking if Ingot environment variable '${Key}' needs to be appended to GitHub environment";
    if(Test-Path "env:${Key}")
    {
        Ingot-Write-Info "Ingot environment variable '${Key}' already present in GitHub environment";
    }
    else
    {
        Ingot-Write-Info "Ingot environment variable '${Key}' is not present in GitHub environment";

        $kvp = "${Key}=${Value}";
        
        Ingot-Write-Debug "Appending Ingot environment variable '${Key}' to GitHub environment...";
        "${kvp}" >> $env:GITHUB_ENV;
        Ingot-Write-Info "Appended Ingot environment variable '${Key}' to GitHub environment";

        $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
        Ingot-Write-FileContent -File $env:GITHUB_ENV -Content $githubEnvironmentContent;
        
        Ingot-Write-Debug "Validating Ingot environment variable '${Key}' appended to GitHub environment...";
        if ($githubEnvironmentContent -Match $Key)
        {
            Ingot-Write-StepEnd-Success "Successfully appended Ingot environment variable '${Key}' to GitHub environment";
        }
        else
        {
            throw Ingot-Error "Ingot environment variable '${Key}' not found in GitHub environment"
        }
    }
}

function Ingot-Ensure-FileLookup-NotEmpty
{
    param (
        $FileFilter,
        $Directory,
        $Files
    )

    if ($Files.Count -eq 0)
    {
        throw Ingot-Error "No files matching filter '${FileFilter}' in directory '${Directory}'";
    }
}

function Ingot-Delete-Directory {
    
    param (
        $Directory
    )

    Ingot-Write-Debug "Checking if directory '${Directory}' exists...";
    if (-Not (Test-Path $Directory))
    {
        Ingot-Write-Info "Directory '${Directory}' does not exist. No cleanup necessary";
        return;
    }

    Ingot-Write-Debug "Directory '${Directory}' exists";

    Ingot-Write-FileLookup-Start -Directory $Directory;
    $files = Get-ChildItem $Directory -File -Recurse;
    Ingot-Write-FileLookup-End -Directory $Directory -Files $files;
    
    Ingot-Write-Debug "Deleting directory '${Directory}' and its $($files.Count) files...";
    Remove-Item -Recurse -Force $Directory;
    Ingot-Write-Info "Deleted directory '${Directory}' and its $($files.Count) files"
}

function Ingot-Create-Directory {
    
    param (
        $Directory
    )

    Ingot-Write-Debug "Checking if directory '${Directory}' exists...";
    if (Test-Path $Directory)
    {
        Ingot-Write-Info "Directory '${Directory}' exists. No need to create it";
        return;
    }

    Ingot-Write-Debug "Directory '${Directory}' does not exist";
    
    Ingot-Write-Debug "Creating directory '${Directory}'...";
    New-Item -ItemType Directory -Force -Path $Directory
    Ingot-Write-Info "Created directory '${Directory}'";
}

function Ingot-MergeCodeCoverage {

    param (
        $OutputFormat,
        $OutputFileName,
        $TestCoverageFiles
    )

    Ingot-Write-StepStart "Merging tests code coverage files into 1 '${OutputFormat}' file...";
    
    Ingot-Create-Directory -Directory $TestCoverageDirectory;

    $testCoverageMergedFile = "${TestCoverageDirectory}/${OutputFileName}";
    Ingot-Write-Debug "Invoking 'dotnet-coverage' to merge $($TestCoverageFiles.Count) tests code coverage files`ninto '${testCoverageMergedFile}' with format '${OutputFormat}'...";
    exec { dotnet dotnet-coverage merge --output $testCoverageMergedFile --output-format "${OutputFormat}" $TestCoverageFiles }
    Ingot-Write-Info "Invoked successfully 'dotnet-coverage' to merge $($TestCoverageFiles.Count) tests code coverage files`ninto '${testCoverageMergedFile}' with format '${OutputFormat}'";

    $directory = "${TestCoverageDirectory}";
    $fileFilter = "${OutputFileName}";

    Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
    $codeCoverages = Get-ChildItem $directory -File -Recurse | Where-Object {$_.Name -like $fileFilter};
    Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $codeCoverages;
            
    Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $codeCoverages;

    Ingot-Write-StepEnd-Success "Successfully merged tests code coverage files into 1 '${OutputFormat}' file";
}

function Ingot-Get-CodeCoverageReport-DirectoryPath {
    
    param (
        $ReportType
    )

    $testCoverageCoberturaMergedFile = "${TestCoverageDirectory}/${TestCoverageCoberturaFileName}";
    return "${TestCoverageReportDirectory}/${ReportType}";
}

function Ingot-GenerateCodeCoverageReport
{
    param (
        $ReportType
    )
    
    Ingot-Write-StepStart "Creating code coverage report '${ReportType}'...";

    $testCoverageCoberturaMergedFile = "${TestCoverageDirectory}/${TestCoverageCoberturaFileName}";
    $testCoverageReportAssemblyFilter = "-*.Mocks;-*.Tests;-*.Testing.*";

    $testCoverageReportFileDirectory = Ingot-Get-CodeCoverageReport-DirectoryPath -ReportType $ReportType;
    Write-Build DarkGray "Invoking 'reportgenerator' to create '${ReportType}' code coverage report`nin ${testCoverageReportFileDirectory}...";
    exec { dotnet reportgenerator -reports:$testCoverageCoberturaMergedFile -targetdir:$testCoverageReportFileDirectory -reporttypes:"${ReportType}" -assemblyfilters:"${testCoverageReportAssemblyFilter}" }

    Ingot-Write-StepEnd-Success "Successfully created code coverage report '${ReportType}'";
}

# ***************************************
# 
#
#           Shell Setup
# 
#
# ***************************************

# Set output rendering to use ANSI code
if ($PsStyle)
{
    $PsStyle.OutputRendering = "Ansi";
}

# Helper variables
$pwshPsEdition = "Core";
$pwshCommand = "pwsh";
$pwshLabel = "PowerShell Core (${pwshCommand})";

# ***************************************
# 
#
#           Invoke Build Hooks
# 
#
# ***************************************

Enter-Build {

    # Prints environment information
    Ingot-Write-StepStart "Detecting exection environment..."
    
    Ingot-Write-Build -Color DarkGray (
        "Environment Detection:`n" +
        "----------------------------------"
    )

    if (Ingot-IsOnLocal)
    {
        Ingot-Write-Build -Color Green "`u{2713} .......... Local Environment";
    }
    else
    {
        Ingot-Write-Build -Color Gray "`u{25A2} ........... Local Environment";
    }

    if (Ingot-IsOnCi)
    {
        Ingot-Write-Build -Color Green "`u{2713} .............. CI Environment";
    }
    else
    {
        Ingot-Write-Build -Color Gray  "`u{25A2} .............. CI Environment";
    }

    if (Ingot-IsOnGitHub)
    {
        Ingot-Write-Build -Color Green "`u{2713} .. GitHub Actions Environment";
    }
    else
    {
        Ingot-Write-Build -Color Gray  "`u{25A2} .. GitHub Actions Environment";
    }

    Ingot-Write-Build -Color DarkGray (
        "----------------------------------"
    )

    if (Ingot-IsOnCi)
    {
        Ingot-Write-StepEnd-Warning "CI environment detected. Specific local tasks will be skipped."
    }
    else
    {
        Ingot-Write-StepEnd-Warning "Local environment detected. Specific CI tasks will be skipped."
    }
    
    # Warns the user the setup is not finished if they
    # have not restarted their program with pwsh as default shell
    if (-Not ($BuildTask -eq "setup"))
    {
        Ingot-Write-StepStart "Detecting use of ${pwshLabel}..."

        if (-Not ($PSVersionTable.PSEdition -eq $pwshPsEdition))
        {
            if (Ingot-IsOnCi)
            {
                throw Ingot-Error (
                    "${pwshLabel} enabled but not active`n" +
                    "To ensure consistency between all environment, you must use`n" +
                    "Invoke-Build with ${pwshLabel} either by using --pwsh option`n" +
                    "or by setting environment variable 'pwsh=pwsh'"
                );
            }
            else
            {
                Ingot-Write-StepEnd-Warning (
                    "${pwshLabel} enabled but not active`n" +
                    "Ensure to refresh environment variables by restarting your program"
                );
            }
        }
        else
        {
            Ingot-Write-StepEnd-Success "${pwshLabel} in use";
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
        $msg = ("`n`nTask flow '${BuildTask}' is not allowed to failed and encountered some errors");
        ${*}.Errors | ForEach-Object -Process {
            $msg += "`n----------------------------------------------------------------------`n"
            $msg += "`n'$($_.Task.Name)':`n" +
            "$($_.Error)";
        }

        $msg += "`n----------------------------------------------------------------------`n"
        Ingot-Write-Build -Color Red -Content $msg;
        Write-Error "Task flow '${BuildTask}' failed";
    }
}

# ***************************************
# 
#
#           Local Tasks
# 
#
# ***************************************

# Synopsis: Runs the initial setup for the repo
task setup ci-github-setup, tool-ib-setup, tool-nuget-setup, tool-minver-validate

# Synopsis: full flow (format > build > test > coverage > publish)
task full format, src-linter, src-build, src-test, src-coverage, publish

# Synopsis: Opens ./src/Ingot.sln in Visual Studio
task ide tool-visual-studio-open

# Synopsis: Format source code
task format src-format

# Synopsis: Builds source code
task build src-build

# Synopsis: Tests source code (build > test > coverage)
task test build, src-test, src-coverage

# Synopsis: Packages source code
task pack src-pack

# Synopsis: Publishes packages (pack > publish)
task publish pack, src-publish-local, src-publish-remote

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

# Synopsis: [Specific] GitHub actions specific setup
task ci-github-setup -If (Ingot-IsOnGitHub) {

    # Ensures when running in containers 
    # the ownership is not dubious
    Ingot-Write-StepStart "Adding Ingot to git safe directories...";
        
    Ingot-Write-Debug "Ensuring root directory '${BuildRoot}' is a git repository...";
    if (-Not (Test-Path "${BuildRoot}/.git"))
    {
        Write-Error "Root directory '${BuildRoot}' is not a git repository";
    }
    Ingot-Write-Info "Root directory '${BuildRoot}' is a git repository";

    Ingot-Write-Debug "Invoking 'git' to add root directory '${BuildRoot}' to git safe directories...";
    exec { git config --global --add safe.directory $BuildRoot }
    Ingot-Write-Info "Added root directory '${BuildRoot}' to git safe directories";

    Ingot-Write-StepEnd-Success "Successfully added Ingot to git safe directories";

    # Installs dotnet-trx to benefit from test results PR 
    # and summary within GitHub Actions directly
    Ingot-Write-StepStart "Installing 'trx'...";
    
    Ingot-Write-Debug "Invoking 'dotnet tool' to install 'dotnet-trx'...";
    exec { dotnet tool install --local dotnet-trx }
    Ingot-Write-Info "Successfully installed 'dotnet-trx' in 'dotnet tool'";

    Ingot-Write-Debug "Invoking 'trx' to validate installation...";
    exec { dotnet trx --version }
    Ingot-Write-Info "Successfully invoked 'trx'";

    Ingot-Write-StepEnd-Success "Successfully installed 'trx'";
    
    # dotnet-trx depends on gh to post the PR comments
    Ingot-Write-StepStart "Installing 'gh'...";
    
    Ingot-Write-Debug "Invoking 'apt-get' to install 'gh'...";
    exec { apt-get update -qq && apt-get install gh -qq }
    Ingot-Write-Info "Successfully installed 'gh' via 'apt-get'";

    Ingot-Write-Debug "Invoking 'gh' to validate installation...";
    exec { gh --version }
    Ingot-Write-Info "Successfully invoked 'gh'";

    Ingot-Write-StepEnd-Success "Successfully installed 'gh'";

    # To ease handling of configuration within GitHub
    # actions, the configuration of this invoke-build
    # is exposed via environment variables to be re-used
    # within the .yml workflow or action files
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_DOTNETVERBOSITY" -Value $DotnetVerbosity;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_BUILDCONFIGURATION" -Value $BuildConfiguration;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_SRCDIRECTORY" -Value $SrcDirectory;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_SRCRELEASEGLOB" -Value "./**/bin/*";
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_NUPKGDIRECTORY" -Value $NupkgDirectory;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_NUPKGGLOB" -Value "./**/*.nupkg";
    
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_TESTRESULTDIRECTORY" -Value $TestResultDirectory;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_TESTRESULTGLOB" -Value "**/test-result/*";

    Ingot-GitHub-AppendMissingVariable -Key "INGOT_COVERAGEDIRECTORY" -Value $TestCoverageDirectory;
    Ingot-GitHub-AppendMissingVariable -Key "INGOT_COVERAGEGLOB" -Value "**/test-coverage/*";

    # Set GitHub specific MinVer config
    Ingot-GitHub-AppendMissingVariable -Key "MinVerDefaultPreReleaseIdentifiers" -Value "alpha.0.$($env:GITHUB_RUN_ID).$($env:GITHUB_RUN_NUMBER)";
}

# Synopsis: [Specific] GitHub actions test result summary
task ci-github-src-test-result-summary -If (Ingot-IsOnGitHub) {

    Ingot-Write-StepStart "Adding test results to GitHub action summary...";

    Ingot-Write-Debug "Ensuring GitHub action summary file exists...";
    if ((-Not ($env:GITHUB_STEP_SUMMARY)) -Or (-Not (Test-Path $env:GITHUB_STEP_SUMMARY)))
    {
        throw Ingot-Error (
            "GitHub action summary file '$($env:GITHUB_STEP_SUMMARY)' not found`n" +
            "or GITHUB_STEP_SUMMARY environment variable undefined"
        );
    }
    Ingot-Write-Info "GitHub action summary file exists '$($env:GITHUB_STEP_SUMMARY)'";

    Ingot-Write-Debug "Getting GitHub action summary before invoking 'trx'...";
    $actionSummaryContent = Get-Content $env:GITHUB_STEP_SUMMARY -Raw;
    Ingot-Write-FileContent -File $env:GITHUB_STEP_SUMMARY -Content $actionSummaryContent;

    Ingot-Write-Debug "Invoking 'trx' to publish test results...";
    exec { dotnet trx --path $TestResultDirectory }
    Ingot-Write-Info "Successfully invoked 'trx' to publish test results";

    Ingot-Write-Debug "Getting GitHub action summary after invoking 'trx'...";
    $actionSummaryContentAfter = Get-Content $env:GITHUB_STEP_SUMMARY -Raw;
    Ingot-Write-FileContent -File $env:GITHUB_STEP_SUMMARY -Content $actionSummaryContentAfter;

    if ($actionSummaryContent -eq $actionSummaryContentAfter)
    {
        throw Ingot-Error "GitHub action summary not updated with test results summary"
    }

    Ingot-Write-StepEnd-Success "Successfully added test results to GitHub action summary";
}

# Synopsis: [Specific] GitHub actions code coverage summary
task ci-github-src-coverage-summary -If (Ingot-IsOnGitHub) {

    $reportType = "MarkdownSummaryGithub";

    Ingot-GenerateCodeCoverageReport -ReportType $reportType;
    
    Ingot-Write-StepStart "Adding code coverage report to GitHub action summary...";

    Ingot-Write-Debug "Ensuring GitHub action summary file exists...";
    if ((-Not ($env:GITHUB_STEP_SUMMARY)) -Or (-Not (Test-Path $env:GITHUB_STEP_SUMMARY)))
    {
        throw Ingot-Error (
            "GitHub action summary file '$($env:GITHUB_STEP_SUMMARY)' not found`n" +
            "or GITHUB_STEP_SUMMARY environment variable undefined"
        );
    }
    Ingot-Write-Info "GitHub action summary file exists '$($env:GITHUB_STEP_SUMMARY)'";

    $testCoverageReportDirectory = Ingot-Get-CodeCoverageReport-DirectoryPath -ReportType $reportType;
    $testCoverageReportFile = "${testCoverageReportDirectory}/SummaryGithub.md";
    
    $markdownSummaryContent = Get-Content $testCoverageReportFile -Raw;
    Ingot-Write-FileContent -File $testCoverageReportFile -Content $markdownSummaryContent;
    echo "${markdownSummaryContent}" >> $env:GITHUB_STEP_SUMMARY
    
    Ingot-Write-StepEnd-Success "Successfully added code coverage report to GitHub action summary";
}

# ---------------------------------------
# Tools Tasks
# ---------------------------------------

# Synopsis: [Specific] Configures Invoke-Build to use pwsh by default
task tool-ib-setup -If(-Not (Ingot-IsOnCi)) {

    # Enable PowerShell Core usage to ensure 
    # users benefit from modern features and
    # consistent experience accross platforms
    Ingot-Write-StepStart "Enabling use of ${pwshLabel}...";
    
    if ($PSVersionTable.PSEdition -eq $pwshPsEdition)
    {
        Ingot-Write-StepEnd-Success "${pwshLabel} already enabled";
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
            Ingot-Write-StepEnd-Warning (
                "${pwshLabel} enabled but not active. `n" +
                "Ensure to refresh environment variables by restarting your program."
            );
        }
        else
        {
            Ingot-Write-Debug "Checking ${pwshLabel} availability locally...";
            if (Get-Command $pwshCommand -errorAction SilentlyContinue)
            {
                Ingot-Write-Debug "${pwshLabel} is available locally";

                Ingot-Write-Debug "Setting environment variable '${pwshEnvKey}=${pwshEnvValue}' on scope ${pwshEnvScope}...";
                [System.Environment]::SetEnvironmentVariable($pwshEnvKey, $pwshEnvValue, $pwshEnvScope);
                
                Ingot-Write-StepEnd-Warning (
                    "${pwshLabel} enabled but not active. `n" +
                    "Ensure to refresh environment variables by restarting your program."
                );
            }
            else
            {
                Ingot-Write-StepEnd-Warning ( 
                    "${pwshLabel} not available in your system. `n" +
                    "You should install ${pwshLabel} to ensure consistent experience across different platforms and on the CI.`n" +
                    "See https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows " +
                    "for more details on how to install PowerShell Core."
                );
            }
        }
    }
}

# Synopsis: [Specific] Ensures minver is correctly setup
task tool-minver-validate {

    Ingot-Write-StepStart "Validating MinVer version...";

    Ingot-Write-Debug "Invoking 'minver' to compute version...";
    $version = exec { dotnet minver -v d }
    Ingot-Write-Info "Retrieved MinVer computed version '${version}'";

    $minverDefaultVersion = "0.0.0-alpha.0";
    Ingot-Write-Debug "Validating MinVer computed version '${version}' is not the default fallback '${minverDefaultVersion}'";
    if ($version -like "${minverDefaultVersion}*")
    {
        if ($Force)
        {
            Ingot-Write-StepEnd-Warning (
                "MinVer computed version '${version}' most likely indicates a wrong setup.`n" +
                "You have set '-Force 1' to bypass the MinVer version validation."
            );   
        }
        else
        {
            throw Ingot-Error (
                "MinVer computed version '${version}' most likely indicates a wrong setup.`n" +
                "specify '-Force 1' to bypass this validation."
            );
        }
    }
    else
    {
        Ingot-Write-StepEnd-Success "MinVer computed version '${version}' most likely indicates a proper setup"
    }
}

# Synopsis: [Specific] Configures nuget for local feed
task tool-nuget-setup -If(-Not (Ingot-IsOnCi)) {

    Ingot-Write-StepStart "Creating NuGet config file...";

    $nugetConfigFile = "./nuget.config";
    if (Test-Path $nugetConfigFile)
    {
        Ingot-Write-Debug "NuGet config file '${nugetConfigFile}' exists already. Deleting it...";
        Remove-Item $nugetConfigFile -Force;
        if (Test-Path $nugetConfigFile)
        {
            throw Ingot-Error "Failed to create NuGet config file '${nugetConfigFile}'";
        }
        Ingot-Write-Info "Deleted existing NuGet config file '${nugetConfigFile}'";
    }

    Ingot-Write-Debug "Creating empty NuGet config file '${nugetConfigFile}'...";
    '<configuration></configuration>' > $nugetConfigFile;
    if (-Not (Test-Path $nugetConfigFile))
    {
        throw Ingot-Error "Failed to create NuGet config file '${nugetConfigFile}'";
    }
    Ingot-Write-Info "Created empty NuGet config file '${nugetConfigFile}'";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Ingot-Write-FileContent -File $nugetConfigFile -Content $nugetConfigContent;

    Ingot-Write-StepEnd-Success "Successfully created NuGet config file";

    Ingot-Write-StepStart "Adding local NuGet source...";

    $nugetLocalFeedName = "ingot-local-feed";
    $nugetLocalFeedValue = "${BuildRoot}/dist/local-nuget-feed";

    Ingot-Write-Debug "Invoking 'dotnet nuget' to add local NuGet source '${nugetLocalFeedName}' (${nugetLocalFeedValue})...";
    exec { dotnet nuget add source --configfile $nugetConfigFile --name $nugetLocalFeedName $nugetLocalFeedValue }
    Ingot-Write-Info "Successfully added local NuGet source '${nugetLocalFeedName}' (${nugetLocalFeedValue})";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Ingot-Write-FileContent -File $nugetConfigFile -Content $nugetConfigContent;
    
    Ingot-Write-StepEnd-Success "Successfully added local NuGet source";

    Ingot-Write-StepStart "Setting local NuGet push source...";

    $nugetDefaultPushSourceKey = "defaultPushSource";
    Ingot-Write-Debug "Invoking 'dotnet nuget' to set NuGet configuration '${nugetDefaultPushSourceKey}' (${nugetLocalFeedValue})...";
    exec { dotnet nuget config set --configfile $nugetConfigFile $nugetDefaultPushSourceKey $nugetLocalFeedValue }
    Ingot-Write-Info "Successfully set NuGet configuration '${nugetDefaultPushSourceKey}' (${nugetLocalFeedValue})";

    $nugetConfigContent = Get-Content $nugetConfigFile -Raw;
    Ingot-Write-FileContent -File $nugetConfigFile -Content $nugetConfigContent;
    
    Ingot-Write-StepEnd-Success "Successfully set local NuGet push source";
}

# Synopsis: [Specific] Opens ./src/Ingot.sln in Visual Studio
task tool-visual-studio-open {

    Ingot-Write-StepStart "Searching Visual Studio installation...";

    $programFilesx86 = (Get-ChildItem "env:ProgramFiles(x86)").Value;
    $vsWherePath = "${programFilesx86}\Microsoft Visual Studio\Installer\vswhere.exe";
    Ingot-Write-Debug "Validating vswhere.exe '${vsWherePath}' exists..."
    if (-Not (Test-Path $vsWherePath))
    {
        throw Ingot-Error ( 
            "vswhere.exe '${vsWherePath}' not found. `n" +
            "Could not determine Visual Studio location."
        );
    }
    
    Ingot-Write-Debug "Invoking vswhere.exe to determine Visual Studio installation path...";
    $vsWhereOutput = exec { & $vsWherePath -prerelease -latest -property productPath -format json }
    Ingot-Write-Info "Successfully invoked vswhere.exe"
    Ingot-Write-Debug "vswhere.exe invocation output:`n${vsWhereOutput}";
    
    Ingot-Write-Debug "Parsing vswhere.exe output to find Visual Studio installation path...";
    $vsPath = ($vsWhereOutput | ConvertFrom-Json)[0].productPath;
    Ingot-Write-Info "Parsed vswhere.exe output into Visual Studio installation path '${vsPath}'";

    Ingot-Write-Debug "Validating Visual Studio installation path '${vsPath}'...";
    if (-Not (Test-Path $vsPath ))
    {
        throw Ingot-Error "vswhere.exe returned an invalid Visual Studio installation path`n'${vsPath}'";
    }

    Ingot-Write-StepEnd-Success "Successfully found Visual Studio installation";
    
    Ingot-Write-StepStart "Opening Visual Studio...";

    $slnFilePath = "${SrcDirectory}/Ingot.sln";
    Ingot-Write-Debug "Invoking Visual Studio to open Ingot solution '${slnFilePath}'...";
    exec { & $vsPath $slnFilePath -donotloadprojects }
    Ingot-Write-Info "Invoked Visual Studio successfully";

    Ingot-Write-StepEnd-Success "Successfully opened Visual Studio";
}

# ---------------------------------------
# Src Tasks
# ---------------------------------------

# Synopsis: [Specific] Runs csharpier to format the ./src files
task src-format {
    
    Ingot-Write-StepStart "Formatting source code...";

    Ingot-Write-Debug "Invoking 'csharpier' on source directory '${SrcDirectory}'...";
    exec { dotnet csharpier --loglevel "Debug" $SrcDirectory }
    Ingot-Write-Info "Successfully invoked 'csharpier' on source directory '${SrcDirectory}";

    Ingot-Write-StepEnd-Success "Successfully formatted source code";
}

# Synopsis: [Specific] Runs csharpier as a linter to validate the formatting of the ./src files
task src-linter {
    
    Ingot-Write-StepStart "Validating source code format...";
    
    Ingot-Write-Debug "Invoking 'csharpier' on source directory '${SrcDirectory}'...";
    exec { dotnet csharpier --check --loglevel "Debug" $SrcDirectory }
    Ingot-Write-Info "Successfully invoked 'csharpier' on source directory '${SrcDirectory}";

    Ingot-Write-StepEnd-Success "Successfully validated source code format";
}

# Synopsis: [Specific] Restores the ./src code
task src-restore {
    
    Ingot-Write-StepStart "Restoring nuget dependencies...";

    Ingot-Write-Debug "Invoking 'dotnet restore' on source directory '${SrcDirectory}'...";
    exec { dotnet restore $SrcDirectory --verbosity $DotnetVerbosity }
    Ingot-Write-Info "Successfully invoked 'dotnet restore' on source directory '${SrcDirectory}";

    Ingot-Write-StepEnd-Success "Successfully restored nuget dependencies";
}

# Synopsis: [Specific] Validates the built ./src code
task src-build-validate {

    Ingot-Write-StepStart "Validating '${BuildConfiguration}' source build exists...";
    
    $directory = "${SrcDirectory}";
    $fileFilter = "*libs[\/]*[\/]bin[\/]${BuildConfiguration}[\/]*.dll";

    Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
    $files = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $files;
    
    Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $files;
    Ingot-Write-StepEnd-Success "Successfully validated '${BuildConfiguration}' source build exists"
}

# Synopsis: [Specific] Builds the ./src code
task src-build src-restore, {
    
    Ingot-Write-StepStart "Building '${BuildConfiguration}' source...";

    Ingot-Write-Debug "Invoking 'dotnet build' to build '${SrcDirectory}' with configuration '${BuildConfiguration}'...";
    exec { dotnet build $SrcDirectory -c $BuildConfiguration --no-restore --verbosity $DotnetVerbosity }
    Ingot-Write-Info "Successfully invoked 'dotnet build' to build '${SrcDirectory}' with configuration '${BuildConfiguration}'";

    Ingot-Write-StepEnd-Success "Successfully built '${BuildConfiguration}' source";
}, src-build-validate

# Synopsis: [Specific] Cleans the test result outputs
task src-test-clean {

    Ingot-Write-StepStart "Cleaning test result directory...";

    Ingot-Delete-Directory -Directory $TestResultDirectory;

    Ingot-Write-StepEnd-Success "Successfully cleant test result directory";
}

# Synopsis: [Specific] Tests the built ./src code
task src-test src-test-clean, src-coverage-clean, src-build-validate, src-restore, ?src-test-core, src-test-finally

# Synopsis: [Specific] Core task of task flow src-test
task src-test-core {

    # Allows to fail the build even if some
    # tasks in the flow are allowed to fail
    # to continue subsequent tasks
    $script:propagateErrorsToBuild=$True;

    Ingot-Write-StepStart "Getting test projects...";

    $directory = "${SrcDirectory}";
    $fileFilter = "*Tests.csproj";

    Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
    $testProjects = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $testProjects;
    
    Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $testProjects;

    Ingot-Write-StepEnd-Success "Successfully found $($testProjects.Count) test projects";

    $testProjects | ForEach-Object -Process {

        Ingot-Write-StepStart "Running tests declared in '$($_.Name)'...";

        $codeCoverageOutputFile = "${TestResultDirectory}/$($_.BaseName)";

        Ingot-Write-Debug "Invoking 'dotnet test' on '$($_.FullName)'`nand collecting code coverage to '${codeCoverageOutputFile}'...";
        exec { dotnet test $_.FullName -c $BuildConfiguration --no-build --verbosity $DotnetVerbosity --results-directory $codeCoverageOutputFile --collect "Code Coverage" --logger "trx"; }
        Ingot-Write-Info "Successfully invoked 'dotnet test' on '$($_.FullName)'";
        
        Ingot-Write-StepEnd-Success "Successfully ran tests declared in '$($_.Name)'";
        
        Ingot-Write-StepStart "Validating run of '$($_.Name)' created trx file...";
        
        $directory = "${codeCoverageOutputFile}";
        $fileFilter = "*.trx";

        Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
        $trxs = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
        Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $trxs;
        
        Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $trxs;

        Ingot-Write-StepEnd-Success "Successfully validated run of '$($_.Name)' created trx file";
        
        Ingot-Write-StepStart "Validating run of '$($_.Name)' collected code coverage...";
        
        $directory = "${codeCoverageOutputFile}";
        $fileFilter = "*.coverage";

        Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
        $codeCoverages = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
        Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $codeCoverages;
        
        Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $codeCoverages;

        Ingot-Write-StepEnd-Success "Successfully validated run of '$($_.Name)' collected code coverage";
    }

    Ingot-Write-StepStart "Generating test result summary...";

    $summaryFile = "${TestResultDirectory}/summary.md";

    
    Ingot-Write-Debug "Invoking 'liquid' to generate test result summary '${codeCoverageOutputFile}' from trx files...";
    
    Ingot-Write-Warning ("Test result summary currently not supported, it's not you, its' us.") -AsCard $True;
    # exec { dotnet liquid --inputs "File=${TestResultDirectory}/**.trx;Format=Trx" --output-file "${summaryFile}"; }
    
    Ingot-Write-Info "Successfully invoked 'liquid' to generate test result summary";

    $fileFilter = "summary.md";
    $directory = "${TestResultDirectory}";

    Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
    $summary = Get-ChildItem $directory -File -Recurse | Where-Object {$_.Name -like $fileFilter};
    Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $summary;

    Ingot-Write-StepStart "Successfully generated test result summary";

}

# Synopsis: [Specific] Finally task of task flow src-test
task src-test-finally ci-github-src-test-result-summary

# Synopsis: [Specific] Cleans the code coverage outputs
task src-coverage-clean {

    Ingot-Write-StepStart "Cleaning code coverage directory...";

    Ingot-Delete-Directory -Directory $TestCoverageDirectory;

    Ingot-Write-StepEnd-Success "Successfully cleant code coverage directory";
}

# Synopsis: [Specific] Generates code coverage reports
task src-coverage {

    Ingot-Write-StepStart "Retrieving tests code coverage...";

    $directory = "${TestResultDirectory}";
    $fileFilter = "*.coverage";

    Ingot-Write-FileLookup-Start -FileFilter $fileFilter -Directory $directory;
    $testsCodeCoverage = Get-ChildItem $directory -File -Recurse | Where-Object {$_.FullName -like $fileFilter};
    Ingot-Write-FileLookup-End -FileFilter $fileFilter -Directory $directory -Files $testsCodeCoverage;
    
    Ingot-Ensure-FileLookup-NotEmpty -FileFilter $fileFilter -Directory $directory -Files $testsCodeCoverage;

    Ingot-Write-StepEnd-Success "Successfully found $($testsCodeCoverage.Count) tests code coverage";

    Ingot-MergeCodeCoverage -OutputFormat "coverage" -OutputFileName "test-result.coverage" -TestCoverageFiles $testsCodeCoverage;
    
    Ingot-MergeCodeCoverage -OutputFormat "cobertura" -OutputFileName $TestCoverageCoberturaFileName -TestCoverageFiles $testsCodeCoverage;

    Ingot-GenerateCodeCoverageReport -ReportType "Html";
    Ingot-GenerateCodeCoverageReport -ReportType "JsonSummary";
}, ci-github-src-coverage-summary

# Synopsis: [Specific] Cleans the nuget output folder
task src-pack-clean {

    Write-Build Magenta "Cleaning output directory ${NupkgDirectory}...`n";

    Write-Build DarkGray "Getting output directory ${NupkgDirectory} existing items...";
    if (-Not (Test-Path $NupkgDirectory))
    {
        Write-Build DarkGray "Output directory ${NupkgDirectory} does not exist";

        Write-Build DarkGray "Creating output directory ${NupkgDirectory}...";
        New-Item -ItemType Directory -Force -Path $NupkgDirectory
        Write-Build DarkGray "Created output directory ${NupkgDirectory}";
    }

    $previoulsyCreatedFiles = Get-ChildItem $NupkgDirectory -recurse;
    
    Write-Build DarkGray "Found $($previoulsyCreatedFiles.Count) items already in output directory ${NupkgDirectory}";
    $previoulsyCreatedFiles | ForEach-Object -Process {
        Write-Build DarkGray "    - $($_.FullName)";
    }

    Write-Build DarkGray "Deleting existing items in output directory ${NupkgDirectory}...";
    $previoulsyCreatedFiles | Remove-Item -Recurse -Force
    
    Write-Build Green "Successfully deleted $($previoulsyCreatedFiles.Count) items already in output directory ${NupkgDirectory}`n";
}

# Synopsis: [Specific] Validates the nuget packages
task src-pack-validate {

    Write-Build Magenta "Validating nuget packages...`n";

    $nupkgFilter = "*.nupkg";
    Write-Build DarkGray "Getting ${NupkgDirectory} nuget packages matching filter '${nupkgFilter}'...";
    $nupkgFiles = Get-ChildItem $NupkgDirectory -recurse | Where-Object {$_.name -like $nupkgFilter};

    if ($nupkgFiles.Count -eq 0)
    {
        Write-Error "No ${NupkgDirectory} nuget packages matching filter '${nupkgFilter}' found";
    }

    Write-Build DarkGray "Found $($nupkgFiles.Count) nuget packages: ";
    $nupkgFiles | ForEach-Object -Process {
        Write-Build DarkGray "    - $($_.FullName)";
    }
    Write-Build DarkGray "";

    Write-Build DarkGray "Invoking dotnet minver to retrieve the expected version...";
    $expectedVersion = exec { dotnet minver -v d }

    Write-Build DarkGray "Successfully retrieve expected version ${expectedVersion}`n";

    $nupkgFiles | ForEach-Object -Process {
        
        if ($_.FullName -like "*${expectedVersion}.nupkg")
        {
            Write-Build Green "Nuget package $($_.FullName) matches expected version ${expectedVersion}";
        }
        else
        {
            Write-Error "Nuget package $($_.FullName) does not match expected version ${expectedVersion}";
        }
    }

    Write-Build DarkGray "";
}

# Synopsis: [Specific] Packages the built ./src code into nuget packages *.nupkg
task src-pack src-build-validate, src-restore, src-pack-clean, {
    
    Write-Build Magenta "Creating nuget packages...`n";

    Write-Build DarkGray "Invoking dotnet pack on source directory (${SrcDirectory})...";
    exec { dotnet pack $SrcDirectory --no-build --output $NupkgDirectory --verbosity $DotnetVerbosity }

}, src-pack-validate

# Synopsis: [Specific] Publishes the packaged *.nupkg to a local feed
task src-publish-local -If(-Not (Ingot-IsOnCi)) tool-nuget-setup, src-pack-validate, {

    Write-Build Magenta "Retrieving nuget local push source...`n";

    Write-Build DarkGray "Invoking dotnet nuget config to get 'defaultPushSource'";
    $nugetLocalSource = exec { dotnet nuget config get "defaultPushSource" };
    
    if (-Not $nugetLocalSource)
    {
        Write-Error "Could not retrieve nuget local push source";
    }

    Write-Build Green "Successfully retrieved local push source '${nugetLocalSource}'`n";

    Write-Build Magenta "Pushing nuget packages to ${nugetLocalSource}...`n";

    if (-Not (Test-Path $nugetLocalSource))
    {
        Write-Build DarkGray "${nugetLocalSource} does not exist";

        Write-Build DarkGray "Creating ${nugetLocalSource}...";
        New-Item -ItemType Directory -Force -Path $nugetLocalSource
        
        Write-Build Green "Successfully created ${nugetLocalSource}";
    }

    $nupkgFilter = "*.nupkg";
    Write-Build DarkGray "Getting ${NupkgDirectory} nuget packages matching filter '${nupkgFilter}'...";
    $nupkgFiles = Get-ChildItem $NupkgDirectory -recurse | Where-Object {$_.name -like $nupkgFilter};

    $nupkgFiles | ForEach-Object -Process {
        
        $localFeedFilePath = "${nugetLocalSource}/$($_.Name)";
        if (Test-Path $localFeedFilePath)
        {
            Write-Build DarkGray "Removing existing file ${localFeedFilePath}...";
            Remove-Item $localFeedFilePath;
        }

        Write-Build DarkGray "Invoking dotnet nuget push for nuget $($_.FullName) to source ${NupkgPushSource}...";
        exec { dotnet nuget push $_.FullName }
        
        Write-Build Green "Successfully pushed nuget package $($_.FullName) to source ${NupkgPushSource}`n";
    }
}

# Synopsis: [Specific] Publishes the packaged *.nupkg to a remote feed
task src-publish-remote -If(($NupkgPushSource) -And ($NupkgPushApiKey)) src-pack-validate, {
    
    Write-Build Magenta "Pushing nuget packages to ${NupkgPushSource}...`n";

    Write-Build DarkGray "Invoking dotnet nuget push for nuget directory (${NupkgDirectory}) to source ${NupkgPushSource}...";
    exec { dotnet nuget push "$($NupkgDirectory)/*.nupkg" --api-key $NupkgPushApiKey --source $NupkgPushSource --skip-duplicate }
    
    Write-Build Green "Successfully pushed nuget packages to ${NupkgPushSource}`n";
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
