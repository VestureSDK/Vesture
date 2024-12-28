task Src-Format {
    dotnet format ./src
}

task Src-Clean {
    dotnet clean ./src
}

task Src-Restore {
    dotnet restore ./src
}

task Src-Build {
    dotnet build ./src
}

task Build Src-Format, Src-Clean, Src-Restore, Src-Build

task Docs-Clean {
    rm -Force -Recurse ./docs/References
    rm -Force -Recurse ./docs/_site
}

task Docs-Build {
    dotnet docfx ./docs/docfx.json
}

task Docs-Serve {
    dotnet docfx serve ./docs/_site --open-browser
}

task Docs Docs-Clean, Docs-Build, Docs-Serve