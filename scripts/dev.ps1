<#
.SYNOPSIS
    Builds, tests and runs MAL Reviewer through Podman, so neither .NET nor Visual Studio is needed on this machine.

.DESCRIPTION
    Every .NET command runs in the "mal-reviewer-dev" container (.devcontainer/Containerfile), with the repository
    mounted at /src. The Windows app is published self-contained, so it runs on the host without .NET installed.

.EXAMPLE
    ./scripts/dev.ps1 test
    ./scripts/dev.ps1 coverage
    ./scripts/dev.ps1 run
#>
[CmdletBinding()]
param(
    # What to do.
    [Parameter(Position = 0)]
    [ValidateSet('image', 'build', 'test', 'coverage', 'publish', 'run', 'shell', 'clean')]
    [string] $Command = 'build',

    # The build configuration of build and test.
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'

$Root = Split-Path -Parent $PSScriptRoot
$Image = 'mal-reviewer-dev'
$Solution = 'MAL_Reviewer.slnx'
$TestProjects = @('MAL_Reviewer_API.Tests', 'MAL_Reviewer_Core.Tests')
$PublishFolder = Join-Path $Root 'artifacts\publish'

function Invoke-Podman([string[]] $Arguments) {
    & podman @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "podman $($Arguments[0]) failed with exit code $LASTEXITCODE."
    }
}

function Build-Image {
    Invoke-Podman @('build', '--tag', $Image, (Join-Path $Root '.devcontainer'))
}

function Confirm-Image {
    & podman image exists $Image

    if ($LASTEXITCODE -ne 0) {
        Build-Image
    }
}

# Runs a bash script in the container, from the solution's folder.
function Invoke-Container([string] $Script, [switch] $Interactive) {
    Confirm-Image

    $podmanArgs = @('run', '--rm')

    if ($Interactive) {
        $podmanArgs += '-it'
    }

    $podmanArgs += @(
        '--volume', "${Root}:/src",
        '--volume', 'mal-nuget:/root/.nuget/packages',
        '--volume', 'mal-dotnet-tools:/root/.dotnet/tools',
        '--workdir', '/src/MAL_Reviewer',
        $Image, 'bash', '-c', $Script
    )

    Invoke-Podman $podmanArgs
}

function Publish-App {
    Invoke-Container ('rm -rf ../artifacts/publish && dotnet publish MAL_Reviewer_UI/MAL_Reviewer_UI.csproj -c Release -r win-x64 ' +
        '--self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ../artifacts/publish')
}

switch ($Command) {
    'image' {
        Build-Image
    }

    'build' {
        Invoke-Container "dotnet build $Solution -c $Configuration"
    }

    'test' {
        Invoke-Container "dotnet test --solution $Solution -c $Configuration"
    }

    'coverage' {
        # Each test project gets its own results folder, so their coverage files don't overwrite each other.
        $runs = ($TestProjects | ForEach-Object {
            "dotnet test --project tests/$_ -c $Configuration --results-directory ../artifacts/test-results/$_ " +
            '--coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml --coverage-settings tests/coverage.config'
        }) -join ' && '

        Invoke-Container ('set -e; rm -rf ../artifacts/test-results ../artifacts/coverage; dotnet tool restore; ' + $runs + '; ' +
            "dotnet reportgenerator '-reports:../artifacts/test-results/*/coverage.cobertura.xml' -targetdir:../artifacts/coverage '-reporttypes:Html;TextSummary'; " +
            'cat ../artifacts/coverage/Summary.txt')

        Write-Host "Coverage report: $(Join-Path $Root 'artifacts\coverage\index.html')"
    }

    'publish' {
        Publish-App
        Write-Host "Published to $PublishFolder"
    }

    'run' {
        Publish-App
        Start-Process -FilePath (Join-Path $PublishFolder 'MAL_Reviewer_UI.exe') -WorkingDirectory $PublishFolder
    }

    'shell' {
        Invoke-Container 'bash' -Interactive
    }

    'clean' {
        Get-ChildItem -Path (Join-Path $Root 'MAL_Reviewer') -Directory -Recurse -Include 'bin', 'obj', 'TestResults' |
            Remove-Item -Recurse -Force
        Remove-Item -Path (Join-Path $Root 'artifacts') -Recurse -Force -ErrorAction SilentlyContinue
    }
}
