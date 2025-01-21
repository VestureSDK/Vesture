task Setup {
    # Do the setup of the environment and repository
    # It might also be used to migrate an existing
    # setup to a new version of the setup
}

task Build {
    $verbosity = "normal"

    # Builds the solution in Release configuration
    dotnet clean ./src --verbosity $verbosity
    dotnet restore ./src --verbosity $verbosity
    dotnet build ./src -c Release --no-restore --verbosity $verbosity
}

task Test {
    $verbosity = "normal"

    dotnet test ./src --no-build --verbosity $verbosity
}

task Validate {
    $verbosity = "normal"
    
    dotnet format --verify-no-changes --verbosity $verbosity
}

task Package {
    $verbosity = "normal"
    $dist = "./dist/nuget"    

    dotnet pack ./src --no-build --output $dist --verbosity $verbosity
}

task Release {
    $verbosity = "normal"    
    $apiKey = "test"
    $source = "https://api.nuget.org/v3/index.json"
    $dist = "./dist/nuget"    

    Get-ChildItem "$($dist)/*.nupkg" | ForEach-Object -Process { dotnet nuget push "$($dist)/$($_.Name)" --api-key $apiKey --source $source }
}


task Src {
    & $(& "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -prerelease -latest -property productPath -format json | ConvertFrom-Json)[0].productPath ./src/Ingot.sln -donotloadprojects
}

task Src-Full {
    ./src/Ingot.sln
}

task Src-Clean {
    dotnet clean ./src --verbosity detailed
}

task Src-Format {
    dotnet format ./src --verbosity detailed
}

task Src-Restore {
    dotnet restore ./src --verbosity detailed
}

task Src-Build {
    dotnet build ./src --verbosity detailed
}

task Docs-Clean {
    rm -Force -Recurse ./docs/docs/Mediator/References -ErrorAction SilentlyContinue
    rm -Force -Recurse ./docs/docs/Mediator/Advanced/References -ErrorAction SilentlyContinue
    rm -Force -Recurse ./docs/_site -ErrorAction SilentlyContinue
}

task Docs-Build {
    dotnet docfx ./docs/docfx.json
}

task Docs-Serve {
    dotnet docfx serve ./docs/_site --open-browser
}

task Docs Docs-Clean, Docs-Build, Docs-Serve