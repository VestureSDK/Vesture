task Setup {
    # Do the setup of the environment and repository
    # It might also be used to migrate an existing
    # setup to a new version of the setup
}

task Build {
    $verbosity = "normal"

    # Builds the solution in Release configuration
    dotnet clean ./src --verbosity $verbosity

    assert ($LASTEXITCODE -eq 0) "Clean encountered an error"
    
    dotnet restore ./src --verbosity $verbosity
    
    assert ($LASTEXITCODE -eq 0) "Restore encountered an error"
    
    dotnet build ./src -c Release --no-restore --verbosity $verbosity
    
    assert ($LASTEXITCODE -eq 0) "Build encountered an error"
}

task Test {
    $verbosity = "normal"

    dotnet test ./src --no-build --verbosity $verbosity
    
    assert ($LASTEXITCODE -eq 0) "Test encountered an error"
}

task Validate {
    $verbosity = "normal"
    
    dotnet format --verify-no-changes --verbosity $verbosity
    
    assert ($LASTEXITCODE -eq 0) "Format not applied. Run dotnet format ./src"
}

task Package {
    $verbosity = "normal"
    $dist = "./dist/nuget"    

    dotnet pack ./src --no-build --output $dist --verbosity $verbosity
    
    assert ($LASTEXITCODE -eq 0) "Pack nuget packages encountered an error"
}

task Release {    
    $apiKey = "test"
    $source = "https://api.nuget.org/v3/index.json"
    $dist = "./dist/nuget"    

    Get-ChildItem "$($dist)/*.nupkg" | ForEach-Object -Process { dotnet nuget push "$($dist)/$($_.Name)" --api-key $apiKey --source $source --skip-duplicate }

    assert ($LASTEXITCODE -eq 0) "Push nuget packages encountered an error"
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