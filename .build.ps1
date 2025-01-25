param(
    [ValidateSet('q', 'quiet', 'm', 'minimal', 'n', 'normal', 'd', 'detailed', 'diag', 'diagnostic')]
    [string] $DotnetVerbosity = 'normal',

    [ValidateSet('Debug', 'Release')]
    [string] $BuildConfiguration = 'Release',

    [string] $SrcDirectory = './src',

    [string] $NupkgDirectory = './dist/nuget',

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

# Synopsis: Runs formatting all the way to packing the source code
task . format, ci-src-linter, pack

# Synopsis: Opens Visual Studio with no projects loaded
task ide {
    & $(& "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property productPath -format json | ConvertFrom-Json)[0].productPath $SrcDirectory/Ingot.sln -donotloadprojects
}

# Synopsis: Format source code
task format {
    
    exec { dotnet csharpier $SrcDirectory }
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

    # Ensures when running in containers the ownership is not dubious
    exec { git config --global --add safe.directory $BuildRoot }
}

# ---------------------------------------
# Src Tasks
# ---------------------------------------

# Synopsis: [CI Specific] Runs csharpier as a linter to validate the formatting of the ./src files
task ci-src-linter {
    
    exec { dotnet csharpier --check $SrcDirectory }
}

# Synopsis: [CI Specific] Restores the ./src code
task ci-src-restore {
    
    exec { dotnet restore $SrcDirectory --verbosity $DotnetVerbosity }
}

# Synopsis: [CI Specific] Builds the ./src code
task ci-src-build ci-src-restore, {
    
    exec { dotnet build $SrcDirectory -c $BuildConfiguration --no-restore --verbosity $DotnetVerbosity }
}

# Synopsis: [CI Specific] Tests the built ./src code
task ci-src-test ci-src-restore, {

    Get-ChildItem $SrcDirectory -recurse | Where-Object {$_.name -like "*Tests.csproj"} | ForEach-Object -Process { 
        exec { dotnet test $_.FullName -c $BuildConfiguration --no-build --verbosity $DotnetVerbosity }
    }    
}

# Synopsis: [CI Specific] Packages the built ./src code into nuget packages *.nupkg
task ci-src-pack ci-src-restore, {
    
    exec { dotnet pack $SrcDirectory --no-build --output $NupkgDirectory --verbosity $DotnetVerbosity }
}

# Synopsis: [CI Specific] Publishes the packaged *.nupkg
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
