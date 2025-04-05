# Contributing to This Project

Thank you for your interest in contributing! We aim to keep the process simple and efficient.

## Setup

Clone the repository and then either follow the **Devcontainer** setup (recommended) or **Local** below.

```bash
git clone https://github.com/stfbln/Ingot.git
```

### Devcontainer

The simplest way is to use the **devcontainer** available. It contains all the required dependencies and on **start**, the container will run the necessary setup (ie. `dotnet tool restore && dotnet ib setup`).

> ***
> **Microsoft Official Documentation:**
>
> *Visual Studio Code Dev Containers:*
>
> * **[Developing inside a Container](https://code.visualstudio.com/docs/devcontainers/containers)**
>
> ***

### Local

#### Prerequisites

If you are working locally (as compared to devcontainer, see above), you require to have **DotNet SDK 9.x** setup.
You can easily check if you have **DotNet SDK 9.x** available or install it if missing by following Microsoft official documentation.\
**Powershell 7+ (pwsh)** is also necessary to use **Invoke-Build** via `dotnet ib ...`.\
To note as well, this repository uses Native AOT publish and you might need to install the prerequisites before being able to validate your changes efficiently.

> ***
> **Microsoft Official Documentation:**
>
> *Dotnet SDK 9.*:*
>
> * **[How to check that .NET is already installed](https://learn.microsoft.com/en-us/dotnet/core/install/how-to-detect-installed-versions)**
> * **[Install .NET on Windows, Linux, and macOS](https://learn.microsoft.com/en-us/dotnet/core/install/)**
>
> ***
>
> *PowerShell 7+ (pwsh):*
>
> * **[Install PowerShell on Windows, Linux, and macOS](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell)**
>
> ***
>
> *Native AOT:*
>
> * **[Native AOT deployment: Prerequisites](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8#prerequisites)**
>
> ***

#### Post-Clone Setup

After installing all the **Prerequisites** and cloning the repository, ensure to run the following commands from the repository root directory to setup the various tools and validate your configuration.

```pwsh
# (Can be run from any shell, not pwsh specifically)

# Restores dotnet tools to have 
# access to Invoke-Build
dotnet tool restore

# Invokes the repository setup
dotnet ib setup

# Runs the full end to end flow, it
# builds the source code, test it, publishes
# nuget packages locally and run the samples.
# This step can take a few minutes. 
dotnet ib full
```

## Using Invoke-Build

To ensure reproducibility between local and CI environment, this repository uses **Invoke-Build** as a **dotnet tool**. You can see the various tasks available by running `dotnet ib ?`. The various commands are:

### Main Commands

| Command | Description |
|:---|:---|
| `dotnet ib format` | Formats source code |
| `dotnet ib linter` | Validates source code format |
| `dotnet ib build` | Builds source code |
| `dotnet ib test` | Tests source code |
| `dotnet ib test-sample` | Tests samples (Native AOT) |
| `dotnet ib coverage -Serve` | Generates code coverage report |
| `dotnet ib pack` | Packages source code |
| `dotnet ib publish` | Publishes packages locally |

### Miscellaneous Commands

| Command | Description |
|:---|:---|
| `dotnet ib setup` | Runs the initial setup for the repo |
| `dotnet ib clean` | Cleans output from previous runs |
| `dotnet ib full` | Runs the full flow |
| `dotnet ib ide` | Opens the repository's solution (.sln) in Visual Studio |
| `dotnet ib update` | Updates the repository's various dependencies |

## Need Help?

If you have any questions, feel free to ask in a discussion or an issue. We’re happy to help!
