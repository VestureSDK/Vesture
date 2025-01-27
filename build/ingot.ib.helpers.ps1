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
