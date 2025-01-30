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
