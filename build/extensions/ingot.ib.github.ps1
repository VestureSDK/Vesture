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

    Write-Step-Start "Appending Ingot environment variable '${Key}' to GitHub environment";

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

    Write-Log Debug  "Checking if Ingot environment variable '${Key}' needs to be appended to GitHub environment";
    if(Test-Path "env:${Key}")
    {
        Write-Log Information  "Ingot environment variable '${Key}' already present in GitHub environment";
    }
    else
    {
        Write-Log Information  "Ingot environment variable '${Key}' is not present in GitHub environment";

        $kvp = "${Key}=${Value}";
        
        Write-Log Debug  "Appending Ingot environment variable '${Key}' to GitHub environment...";
        "${kvp}" >> $env:GITHUB_ENV;
        Write-Log Information  "Appended Ingot environment variable '${Key}' to GitHub environment";

        $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
        Write-File-Content -File $env:GITHUB_ENV -Content $githubEnvironmentContent;
        
        Write-Log Debug  "Validating Ingot environment variable '${Key}' appended to GitHub environment...";
        if ($githubEnvironmentContent -Match $Key)
        {
            Write-Step-End "Successfully appended Ingot environment variable '${Key}' to GitHub environment";
        }
        else
        {
            Write-Log Error "Ingot environment variable '${Key}' not found in GitHub environment"
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
    Write-Step-Start "Adding Ingot to git safe directories...";
        
    Write-Log Debug  "Ensuring root directory '${BuildRoot}' is a git repository...";
    if (-Not (Test-Path "${BuildRoot}/.git"))
    {
        Write-Error "Root directory '${BuildRoot}' is not a git repository";
    }
    Write-Log Information  "Root directory '${BuildRoot}' is a git repository";

    Write-Log Debug  "Invoking 'git' to add root directory '${BuildRoot}' to git safe directories...";
    exec { git config --global --add safe.directory $BuildRoot }
    Write-Log Information  "Added root directory '${BuildRoot}' to git safe directories";

    Write-Step-End "Successfully added Ingot to git safe directories";

    # To ease handling of configuration within GitHub
    # actions, the configuration of this invoke-build
    # is exposed via environment variables to be re-used
    # within the .yml workflow or action files
    Add-GitHub-EnvironmentVariable -Key "INGOT_DOTNETVERBOSITY" -Value $DotnetVerbosity;
    Add-GitHub-EnvironmentVariable -Key "INGOT_BUILDCONFIGURATION" -Value $BuildConfiguration;
    Add-GitHub-EnvironmentVariable -Key "INGOT_SRCDIRECTORY" -Value $SrcDirectory;
    Add-GitHub-EnvironmentVariable -Key "INGOT_SRCRELEASEGLOB" -Value "./**/bin/*";
    Add-GitHub-EnvironmentVariable -Key "INGOT_NUPKGDIRECTORY" -Value $NupkgDirectory;
    Add-GitHub-EnvironmentVariable -Key "INGOT_NUPKGGLOB" -Value "./**/*.nupkg";
    Add-GitHub-EnvironmentVariable -Key "INGOT_TESTRESULTDIRECTORY" -Value $TestResultDirectory;
    Add-GitHub-EnvironmentVariable -Key "INGOT_TESTRESULTGLOB" -Value "**/test-result/*";
    Add-GitHub-EnvironmentVariable -Key "INGOT_COVERAGEDIRECTORY" -Value $TestCoverageDirectory;
    Add-GitHub-EnvironmentVariable -Key "INGOT_COVERAGEGLOB" -Value "**/test-coverage/*";

    # Set GitHub specific MinVer config
    Add-GitHub-EnvironmentVariable -Key "MinVerDefaultPreReleaseIdentifiers" -Value "alpha.0.$($env:GITHUB_RUN_ID).$($env:GITHUB_RUN_NUMBER)";
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

    $testCoverageReportDirectory = Ingot-Get-CodeCoverageReport-DirectoryPath -ReportType $reportType;
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

# Coverage
# ---------------------------------------
task ci-coverage-finally ci-github-src-coverage-summary

}
