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

task Build Src-Clean, Src-Format, Src-Restore, Src-Build

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