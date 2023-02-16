@echo off

REM Print usage.
if "%1"=="" (
    echo "Usage:"
    echo "  <project> [<platform>]"
    echo "Where:"
    echo "  project  - Project name."
    echo "  platform - Platform name (win-x64, linux-x64, osx-x64, osx-arm64)"
    echo "             for self-contained app."
    exit 1
)

REM Publish .NET app.
if "%2"=="" (
    dotnet publish ^
        src/"%1"/"%1".csproj^
        -p:UseAppHost=false^
        -p:DebugType=None^
        -p:DebugSymbols=false^
        -c Release^
        -o artifacts/Publish/"%1"/
    exit 0
)

REM Publish self-contained app for specified platform.
if "%3"=="" (
    dotnet publish -r "%2"^
        src/"%1"/"%1".csproj^
        -p:PublishSingleFile=true^
        -p:IncludeNativeLibrariesForSelfExtract=true^
        -p:AllowedReferenceRelatedFileExtensions=none^
        -p:DebugType=embedded^
        --self-contained true^
        -c Release^
        -o artifacts/Publish/"%2"/"%1"
    exit 0
)
