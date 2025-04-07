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

function Test-GitHubActions-ExecutionEnvironment {
    return Test-Path env:GITHUB_ACTIONS;
}

# ---------------------------------------
# Functional
# ---------------------------------------

function Add-GitHub-EnvironmentVariable {

    param (
        $Key,
        $Value
    )

    Write-Step-Start "Appending repository environment variable '${Key}' to GitHub environment";

    Write-Log Debug  "Ensuring GitHub environment file exists...";
    if ((-Not ($env:GITHUB_ENV)) -Or (-Not (Test-Path $env:GITHUB_ENV)))
    {
        Write-Log Error (
            "GitHub environment file '$($env:GITHUB_ENV)' not found`n" +
            "or GITHUB_ENV environment variable undefined"
        );
    }
    Write-Log Information  "GitHub environment file exists '$($env:GITHUB_ENV)'";
    
    $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
    Write-File-Content -File $env:GITHUB_ENV -Content $githubEnvironmentContent;

    Write-Log Debug  "Checking if repository environment variable '${Key}' needs to be appended to GitHub environment";
    if(Test-Path "env:${Key}")
    {
        Write-Log Information  "Repository environment variable '${Key}' already present in GitHub environment";
    }
    else
    {
        Write-Log Information  "Repository environment variable '${Key}' is not present in GitHub environment";

        $kvp = "${Key}=${Value}";
        
        Write-Log Debug  "Appending repository environment variable '${Key}' to GitHub environment...";
        "${kvp}" >> $env:GITHUB_ENV;
        Write-Log Information  "Appended repository environment variable '${Key}' to GitHub environment";

        $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
        Write-File-Content -File $env:GITHUB_ENV -Content $githubEnvironmentContent;
        
        Write-Log Debug  "Validating repository environment variable '${Key}' appended to GitHub environment...";
        if ($githubEnvironmentContent -Match $Key)
        {
            Write-Step-End "Successfully appended repository environment variable '${Key}' to GitHub environment";
        }
        else
        {
            Write-Log Error "Repository environment variable '${Key}' not found in GitHub environment"
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

Add-ExecutionEnvironment -Name "GitHub Actions" -Enabled $(Test-GitHubActions-ExecutionEnvironment);

# Do not clutter the Invoke-Build tasks 
# if this environment is not enabled
if (Test-GitHubActions-ExecutionEnvironment)
{

# ---------------------------------------
# GitHub Actions Tasks
# ---------------------------------------

# Synopsis: [Specific] GitHub actions specific setup
task ci-github-setup {

    # Ensures when running in containers 
    # the ownership is not dubious
    Write-Step-Start "Adding repository to git safe directories...";
        
    Write-Log Debug  "Ensuring root directory '${BuildRoot}' is a git repository...";
    if (-Not (Test-Path "${BuildRoot}/.git"))
    {
        Write-Error "Root directory '${BuildRoot}' is not a git repository";
    }
    Write-Log Information  "Root directory '${BuildRoot}' is a git repository";

    Write-Log Debug  "Invoking 'git' to add root directory '${BuildRoot}' to git safe directories...";
    exec { git config --global --add safe.directory $BuildRoot }
    Write-Log Information  "Added root directory '${BuildRoot}' to git safe directories";

    Write-Step-End "Successfully added repository to git safe directories";

    # To ease handling of configuration within GitHub
    # actions, the configuration of this invoke-build
    # is exposed via environment variables to be re-used
    # within the .yml workflow or action files
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_DOTNETVERBOSITY" -Value $DotnetVerbosity;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_BUILDCONFIGURATION" -Value $BuildConfiguration;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_SRCDIRECTORY" -Value $SrcDirectory;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_SRCRELEASEGLOB" -Value "./**/bin/*";
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_NUPKGDIRECTORY" -Value $NupkgDirectory;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_NUPKGGLOB" -Value "./**/*.nupkg";
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_TESTRESULTDIRECTORY" -Value $TestResultDirectory;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_TESTRESULTGLOB" -Value "**/test-result/*";
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_COVERAGEDIRECTORY" -Value $TestCoverageDirectory;
    Add-GitHub-EnvironmentVariable -Key "REPOSITORY_COVERAGEGLOB" -Value "**/test-coverage/*";

    # Set GitHub specific MinVer config
    Add-GitHub-EnvironmentVariable -Key "MinVerDefaultPreReleaseIdentifiers" -Value "alpha.0.$($env:GITHUB_RUN_ID).$($env:GITHUB_RUN_NUMBER)";
}

# Synopsis: [Specific] GitHub actions test report summary
task ci-github-src-test-summary {

    Write-Step-Start "Adding test result report to GitHub action summary...";

    Write-Log Debug  "Ensuring GitHub action summary file exists...";
    if ((-Not ($env:GITHUB_STEP_SUMMARY)) -Or (-Not (Test-Path $env:GITHUB_STEP_SUMMARY)))
    {
        Write-Log Error (
            "GitHub action summary file '$($env:GITHUB_STEP_SUMMARY)' not found`n" +
            "or GITHUB_STEP_SUMMARY environment variable undefined"
        );
    }
    Write-Log Information  "GitHub action summary file exists '$($env:GITHUB_STEP_SUMMARY)'";

    $testReportFile = "${TestResultDirectory}/test-result-summary.md";
    
    $markdownSummaryContent = Get-Content $testReportFile -Raw;
    Write-File-Content -File $testReportFile -Content $markdownSummaryContent;
    "${markdownSummaryContent}" >> $env:GITHUB_STEP_SUMMARY
    
    Write-Step-End "Successfully added test result report to GitHub action summary";
}

# Synopsis: [Specific] GitHub actions code coverage summary
task ci-github-src-coverage-summary {

    $reportType = "MarkdownSummaryGithub";

    New-CodeCoverage -ReportType $reportType;
    
    Write-Step-Start "Adding code coverage report to GitHub action summary...";

    Write-Log Debug  "Ensuring GitHub action summary file exists...";
    if ((-Not ($env:GITHUB_STEP_SUMMARY)) -Or (-Not (Test-Path $env:GITHUB_STEP_SUMMARY)))
    {
        Write-Log Error (
            "GitHub action summary file '$($env:GITHUB_STEP_SUMMARY)' not found`n" +
            "or GITHUB_STEP_SUMMARY environment variable undefined"
        );
    }
    Write-Log Information  "GitHub action summary file exists '$($env:GITHUB_STEP_SUMMARY)'";

    $testCoverageReportDirectory = Get-CodeCoverageReport-DirectoryPath -ReportType $reportType;
    $testCoverageReportFile = "${testCoverageReportDirectory}/SummaryGithub.md";
    
    $markdownSummaryContent = Get-Content $testCoverageReportFile -Raw;
    Write-File-Content -File $testCoverageReportFile -Content $markdownSummaryContent;
    "${markdownSummaryContent}" >> $env:GITHUB_STEP_SUMMARY
    
    Write-Step-End "Successfully added code coverage report to GitHub action summary";
}

# ---------------------------------------
# Override CI Tasks
# ---------------------------------------

# Setup
# ---------------------------------------
task ci-setup-before ci-github-setup

# Test Result
# ---------------------------------------
# Currently not supported due to liquid incapacity to handle multi targetted tests
# task ci-test-result-finally ci-github-src-test-summary

# Coverage
# ---------------------------------------
task ci-coverage-finally ci-github-src-coverage-summary

}
