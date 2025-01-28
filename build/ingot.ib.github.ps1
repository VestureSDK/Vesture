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

# ***************************************
# 
#
#      Shell and Script Setup
# 
#
# ***************************************

Add-ExecutionEnvironment -Name "GitHub" -Enabled $(Ingot-IsOnGitHub);

# Do not clutter the Invoke-Build tasks 
# if this environment is not enabled
if (Ingot-IsOnGitHub)
{

# ---------------------------------------
# GitHub Actions Tasks
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
    exec { apt-get update -qq; apt-get install gh -qq }
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
# Override CI Tasks
# ---------------------------------------

# Setup
# ---------------------------------------
task ci-setup-before ci-github-setup

# Test
# ---------------------------------------
task ci-test-finally ci-github-src-test-result-summary

# Coverage
# ---------------------------------------
task ci-coverage-finally ci-github-src-coverage-summary

}
