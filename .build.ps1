param(
    [ValidateSet('q', 'quiet', 'm', 'minimal', 'n', 'normal', 'd', 'detailed', 'diag', 'diagnostic')]
    [string] $DotnetVerbosity = 'normal',

    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Release',

    [string] $SrcDirectory = './src',

    [string] $NupkgDirectory = './dist',

    [string] $NupkgPushSource = 'https://api.nuget.org/v3/index.json',
    
    [string] $NupkgPushApiKey = '<Set your API key with -NupkgPushApiKey parameter>'
)

# ***************************************
# 
#
#           Local Tasks
# 
#
# ***************************************

# Opens the IDE
task ide {
    & $(& "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property productPath -format json | ConvertFrom-Json)[0].productPath $SrcDirectory/Ingot.sln -donotloadprojects
}

# Format source code
task format src-format

# Build source code
task build ci-src-restore, ci-src-build

# Test source code
task test build, ci-src-test

# Package source code as nuget package
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

# Do the setup of the environment and repository
task ci-env-setup {

    # Ensures when running in containers the ownership is not dubious
    exec { git config --global --add safe.directory $BuildRoot }
}

# ---------------------------------------
# Src Tasks
# ---------------------------------------

# Runs csharpier to fomrat the ./src files
task ci-src-format {
    
    exec { dotnet csharpier $SrcDirectory }
}

# Runs csharpier as a linter to validate the formatting
# of the ./src files
task ci-src-linter {
    
    exec { dotnet csharpier --check $SrcDirectory }
}

# Restores the ./src code
task ci-src-restore {
    
    exec { dotnet restore $SrcDirectory --verbosity $DotnetVerbosity }
}

# Builds the ./src code
task ci-src-build {
    
    exec { dotnet build $SrcDirectory -c $BuildConfiguration --no-restore --verbosity $DotnetVerbosity }
}

# Tests the built ./src code
task ci-src-test {

    Get-ChildItem $SrcDirectory -recurse | Where-Object {$_.name -like "*Tests.csproj"} | ForEach-Object -Process { 
        exec { dotnet test $_.FullName -c $BuildConfiguration --no-build --verbosity $DotnetVerbosity }
    }    
}

# Packages the built ./src code
# into nuget packages *.nupkg
task ci-src-pack {
    
    exec { dotnet pack $SrcDirectory --no-build --output $NupkgDirectory --verbosity $DotnetVerbosity }
}

# Publishes the packaged *.nupkg
task ci-src-publish {    
    
    Get-ChildItem "$($NupkgDirectory)/*.nupkg" | ForEach-Object -Process { 
        exec { dotnet nuget push "$($NupkgDirectory)/$($_.Name)" --api-key $NupkgPushApiKey --source $NupkgPushSource --skip-duplicate }
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
