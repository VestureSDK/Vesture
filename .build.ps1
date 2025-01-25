param(
    [ValidateSet('q', 'quiet', 'm', 'minimal', 'n', 'normal', 'd', 'detailed', 'diag', 'diagnostic')]
    [string] $DotnetVerbosity = 'normal',

    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Release',

    [string] $SrcDirectory = './src',

    [string] $NupkgDirectory = './dist/nuget',

    [string] $NupkgPushSource = 'https://api.nuget.org/v3/index.json',
    
    [string] $NupkgPushApiKey = '<Set your API key with -NupkgPushApiKey parameter>',

    [bool] $Force = $False
)

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

Enter-Build {

    # Warns the user the setup is not finished if they
    # have not restarted their program with pwsh as default shell
    if (-Not ($BuildTask -eq "setup"))
    {
        if (-Not ($PSVersionTable.PSEdition -eq $pwshPsEdition))
        {
            Write-Build Yellow (
                "---------------------------------------------------------------------- `n" +
                "WARNING: ${pwshLabel} enabled but not active. `n" +
                "Ensure to refresh environment variables by restarting your program. `n" +
                "---------------------------------------------------------------------- `n"
            );
        }
    }
}

# ***************************************
# 
#
#           Local Tasks
# 
#
# ***************************************

# Synopsis: `dotnet ib setup` Runs the initial setup for the repo
task setup {

    # Enable PowerShell Core usage to ensure 
    # users benefit from modern features and
    # consistent experience accross platforms
    Write-Build Magenta "Enabling use of ${pwshLabel}...";
    
    if ($PSVersionTable.PSEdition -eq $pwshPsEdition)
    {
        Write-Build Green "${pwshLabel} already enabled.";
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
            Write-Build Yellow (
                "WARNING: ${pwshLabel} enabled but not active. `n" +
                "Ensure to refresh environment variables by restarting your program."
            );
        }
        else
        {
            Write-Build DarkGray "Checking ${pwshLabel} availability on your system...";
            if (Get-Command $pwshCommand -errorAction SilentlyContinue)
            {
                Write-Build DarkGray "${pwshLabel} is available on your system";

                Write-Build DarkGray "Setting environment variable '${pwshEnvKey}=${pwshEnvValue}' on scope ${pwshEnvScope}...";
                [System.Environment]::SetEnvironmentVariable($pwshEnvKey, $pwshEnvValue, $pwshEnvScope);
                
                Write-Build Yellow (
                    "WARNING: ${pwshLabel} enabled but not active. `n" +
                    "Ensure to refresh environment variables by restarting your program."
                );
            }
            else
            {
                Write-Build Yellow ( 
                    "WARNING: ${pwshLabel} not available in your system. `n" +
                    "You should install ${pwshLabel} to ensure consistent experience " +
                    "across different platforms and on the CI. `n" +
                    "See https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows " +
                    "for more details on how to install PowerShell Core."
                );
            }
        }
    }
}

# Synopsis: `dotnet ib` Runs formatting all the way to packing the source code
task . format, ci-src-linter, pack

# Synopsis: `dotnet ib ide` Opens Visual Studio with no projects loaded
task ide {

    Write-Build Magenta "Searching Visual Studio installation...";

    $programFilesx86 = (Get-ChildItem "env:ProgramFiles(x86)").Value;
    $vsWherePath = "${programFilesx86}\Microsoft Visual Studio\Installer\vswhere.exe";
    if (-Not (Test-Path $vsWherePath))
    {
        Write-Error ( 
            "ERROR: vswhere ('${vsWherePath}') not found. `n" +
            "Could not determine Visual Studio location."
        );
    }
    
    Write-Build DarkGray "Invoking vswhere ('${vsWherePath}') to determine Visual Studio location...";
    $vsWhereOutput = exec { & $vsWherePath -prerelease -latest -property productPath -format json }
    $vsPath = ($vsWhereOutput | ConvertFrom-Json)[0].productPath;
    if (-Not (Test-Path $vsPath ))
    {
        Write-Error "ERROR: vswhere ('${vsWherePath}') returned an invalid Visual Studio location (${vsPath})";
    }
    
    Write-Build Green "Found Visual Studio installation (${vsPath})";
        
    Write-Build Magenta "Opening Visual Studio...";

    $slnFilePath = "${SrcDirectory}/Ingot.sln";
    Write-Build DarkGray "Invoking Visual Studio (${vsPath}) to open Ingot solution (${slnFilePath})";
    exec { & $vsPath $slnFilePath -donotloadprojects }
}

# Synopsis: Format source code
task format {

    Write-Build Magenta "Formatting ${SrcDirectory} source code...";
    
    Write-Build DarkGray "Invoking csharpier on source directory (${SrcDirectory})";
    exec { dotnet csharpier $SrcDirectory }

    Write-Build Green "Successfully formatted source code";
}

# Synopsis: Builds source code
task build ci-src-build

# Synopsis: Tests source code
task test build, ci-src-test

# Synopsis: Packages source code as nuget package
task pack build, ci-src-pack

# ***************************************
# 
#
#           CI Tasks
# 
#
# ***************************************

# ---------------------------------------
# Environment Tasks
# ---------------------------------------

# Synopsis: [CI Specific] Do the setup of the environment and repository
task ci-env-setup {

    if (-Not (Test-Path env:CI) -And -Not $Force)
    {
        Write-Error (
            "${BuildTask} should be run only on CI. `n" +
            "To test this task locally, specify '-Force 1' to force its execution."
        );
    }
    elseif (-Not (Test-Path env:CI))
    {
        Write-Build Yellow (
            "---------------------------------------------------------------------- `n" +
            "WARNING: ${BuildTask} should be run only on CI. `n" +
            "You have specified '-Force 1' to forcefully run this task. `n" +
            "---------------------------------------------------------------------- `n"
        );
    }

    # Ensures when running in containers the ownership is not dubious
    Write-Build Magenta "Adding git repository to safe.directory...";
    
    Write-Build DarkGray "Invoking git to add '${BuildRoot}' as a safe.directory...";
    exec { git config --global --add safe.directory $BuildRoot }

    Write-Build Green "Successfully added git repository to safe.directory";

    # GitHub actions specific setup
    if (-Not (Test-Path env:GITHUB_ACTIONS))
    {
        Write-Build DarkGray "Not running on GitHub actions, skipping GitHub actions setup.";
    }
    else
    {
        # To ease handling of configuration within GitHub
        # actions, the configuration of this invoke-build
        # is exposed via environment variables to be re-used
        # within the .yml workflow or action files

        Write-Build Magenta "Appending Ingot environment variables to GitHub environment...";
        
        if (-Not (Test-Path $env:GITHUB_ENV))
        {
            Write-Error "GitHub environment file $($env:GITHUB_ENV) not found";
        }

        Write-Build DarkGray "Getting original GitHub environment file content...";
        $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
        Ingot-PrintFileContent -File $env:GITHUB_ENV -Content $githubEnvironmentContent;

        Ingot-GitHub-AppendMissingVariable -Key "INGOT_DOTNETVERBOSITY" -Value $DotnetVerbosity;
        Ingot-GitHub-AppendMissingVariable -Key "INGOT_BUILDCONFIGURATION" -Value $BuildConfiguration;
        Ingot-GitHub-AppendMissingVariable -Key "INGOT_SRCDIRECTORY" -Value $SrcDirectory;
        Ingot-GitHub-AppendMissingVariable -Key "INGOT_SRCRELEASEGLOB" -Value "./**/bin/*";
        Ingot-GitHub-AppendMissingVariable -Key "INGOT_NUPKGDIRECTORY" -Value $NupkgDirectory;
        Ingot-GitHub-AppendMissingVariable -Key "INGOT_NUPKGGLOB" -Value "./**/*.nupkg";

        Write-Build Green "Appended Ingot environment variables to GitHub environment successfully";
    }

    # Showing Minver information for debugging purpose
    Write-Build Magenta "Checking MinVer...";

    Write-Build DarkGray "Invoking minver...";
    exec { dotnet minver -v d }

    Write-Build Yellow "WARNING: Invoked minver successfully, manual validation of version required";
}

# ---------------------------------------
# Src Tasks
# ---------------------------------------

# Synopsis: [CI Specific] Runs csharpier as a linter to validate the formatting of the ./src files
task ci-src-linter {
    
    Write-Build Magenta "Validating source code format...";
    
    Write-Build DarkGray "Invoking csharpier check on source directory (${SrcDirectory})...";
    exec { dotnet csharpier --check $SrcDirectory }

    Write-Build Green "Successfully validated source code format";
}

# Synopsis: [CI Specific] Restores the ./src code
task ci-src-restore {
    
    Write-Build Magenta "Restoring nuget dependencies...";

    Write-Build DarkGray "Invoking dotnet restore on source directory (${SrcDirectory})...";
    exec { dotnet restore $SrcDirectory --verbosity $DotnetVerbosity }

    Write-Build Green "Successfully restored nuget dependencies";
}

# Synopsis: [CI Specific] Builds the ./src code
task ci-src-build ci-src-restore, {
    
    Write-Build Magenta "Building source code...";
    
    Write-Build DarkGray "Invoking dotnet build on source directory (${SrcDirectory})...";
    exec { dotnet build $SrcDirectory -c $BuildConfiguration --no-restore --verbosity $DotnetVerbosity }

    Write-Build Green "Successfully built source code";
}

# Synopsis: [CI Specific] Tests the built ./src code
task ci-src-test ci-src-restore, {

    Ingot-EnsureBuildRan

    Write-Build Magenta "Searching for test projects...";

    $testProjectsFilter = "*Tests.csproj";
    Write-Build DarkGray "Getting ${SrcDirectory} projects matching filter '${testProjectsFilter}'...";
    $testProjects = Get-ChildItem $SrcDirectory -recurse | Where-Object {$_.name -like $testProjectsFilter};

    if ($testProjects.Count -eq 0)
    {
        Write-Error "No ${SrcDirectory} projects matching filter '${testProjectsFilter}' found";
    }

    Write-Build Green "Found $($testProjects.Count) test projects: ";
    $testProjects | ForEach-Object -Process {
        Write-Build DarkGray "    - $($_.FullName)";
    }

    $testProjects | ForEach-Object -Process {

        Write-Build Magenta "Running tests declared in $($_.FullName) ...";

        Write-Build DarkGray "Invoking dotnet test on csproj ($($_.FullName))...";
        exec { dotnet test $_.FullName -c $BuildConfiguration --no-build --verbosity $DotnetVerbosity }
        
        Write-Build Green "Successfully ran tests declared in $($_.FullName)";
    }    
}

# Synopsis: [CI Specific] Packages the built ./src code into nuget packages *.nupkg
task ci-src-pack ci-src-restore, {
    
    Ingot-EnsureBuildRan
    
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

    Write-Build Magenta "Creating nuget packages...`n";

    Write-Build DarkGray "Invoking dotnet pack on source directory (${SrcDirectory})...";
    exec { dotnet pack $SrcDirectory --no-build --output $NupkgDirectory --verbosity $DotnetVerbosity }

    Ingot-ValidateNugetPackages
}

# Synopsis: [CI Specific] Publishes the packaged *.nupkg
task ci-src-publish {
    
    Ingot-ValidateNugetPackages

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

# ***************************************
# 
#
#           Functions
# 
#
# ***************************************

function Ingot-PrintFileContent {

    param (
        $File,
        $Content
    )

    Write-Build DarkGray (
        "`n`nFile: ${File} `n" +
        "----------- `n" +
        ${Content} + "`n" +
        "----------- `n`n");
}

function Ingot-GitHub-AppendMissingVariable {

    param (
        $Key,
        $Value
    )

    if(Test-Path "env:${Key}")
    {
        Write-Build DarkGray "Variable ${Key} already available in GitHub environment";
    }
    else
    {
        $kvp = "${Key}=${Value}";
        Write-Build DarkGray "Appending ${kvp} to GitHub environment...";
        echo "${kvp}" >> $env:GITHUB_ENV;
        
        Write-Build DarkGray "Getting GitHub environment file content after append...";
        $githubEnvironmentContent = Get-Content $env:GITHUB_ENV -Raw;
        Ingot-PrintFileContent -File $env:GITHUB_ENV -Content $githubEnvironmentContent;
        
        Write-Build DarkGray "Validating variable ${Key} append to GitHub environment...";
        
        if ($githubEnvironmentContent -Match $Key)
        {
            Write-Build Green "Variable ${Key} appended successfully to GitHub environment";
        }
        else
        {
            Write-Error "${Key} not found in GitHub environment"
        }
    }
}

function Ingot-EnsureBuildRan {

    Write-Build Magenta "Ensuring build has been ran with configuration ${BuildConfiguration}...";
    
    $buildDllFilter = "*bin*${BuildConfiguration}*";
    Write-Build DarkGray "Getting ${SrcDirectory} files matching filter '${buildDllFilter}'...";
    $buildDllFiles = Get-ChildItem $SrcDirectory -recurse | Where-Object {$_.FullName -like $buildDllFilter};

    if ($buildDllFiles.Count -eq 0)
    {
        Write-Error "No ${SrcDirectory} files matching filter '${buildDllFilter}' found";
    }

    Write-Build Green "Found $($buildDllFiles.Count) ${BuildConfiguration} build related files: ";
    $buildDllFiles | ForEach-Object -Process {
        Write-Build DarkGray "    - $($_.FullName)";
    }
}

function Ingot-ValidateNugetPackages {

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