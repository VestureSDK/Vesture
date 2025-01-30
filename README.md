# ![image](docs/images/banner.png)

## Get Started

[![Ingot Source (Release)](https://github.com/stfbln/Ingot/actions/workflows/ingot-src-release.yml/badge.svg)](https://github.com/stfbln/Ingot/actions/workflows/ingot-src-release.yml)

* ![NuGet Version](https://img.shields.io/nuget/v/Ingot.Mediator?label=Ingot.Mediator&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FIngot.Mediator%2F0.0.2)

## Staging

[![Ingot Source (Staging)](https://github.com/stfbln/Ingot/actions/workflows/ingot-src-staging.yml/badge.svg)](https://github.com/stfbln/Ingot/actions/workflows/ingot-src-staging.yml)

## Dev Setup

Ensure to have **dotnet SDK 9.0** installed in order to benefit from the tooling made available. \
For more information on how to download and install dotnet, kindly refer to:

* [Microsoft Website (https://dotnet.microsoft.com/en-us/download)](https://dotnet.microsoft.com/en-us/download)

***

After cloning the repository, you should run the initial setup.

```bash
git clone ...

# restore the tools
dotnet tool restore

# Use invoke-build (ib) for the initial setup
dotnet ib setup

# Run the full flow task to validate the setup
dotnet ib full

# You can also list all the tasks/targets available
dotnet ib ?
```
