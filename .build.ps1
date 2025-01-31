param(

    [ValidateSet('trace', 'debug', 'information', 'warning', 'error')]
    [string] $Verbosity = 'trace',

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

    [switch] $Serve,

    [switch] $Force
)

# ***************************************
# 
#
#      Shell and Script Setup
# 
#
# ***************************************

# Imports helper file
. "${BuildRoot}/build/ingot.ib.helpers.ps1";

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
        $msg = ("`n`nTask flow '${BuildTask}' is not allowed to failed and encountered some errors");
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

# Imports main file
. "${BuildRoot}/build/ingot.ib.main.ps1";

# Imports extension files
. "${BuildRoot}/build/extensions/ingot.ib.github.ps1";