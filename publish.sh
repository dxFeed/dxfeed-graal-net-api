#!/bin/sh

# Print usage.
if [ "$#" -eq 0 ]; then
    echo "Usage:"
    echo "  <project> [<platform>]"
    echo "Where:"
    echo "  project  - Project name."
    echo "  platform - Platform name (win-x64, linux-x64, osx-x64, osx-arm64)"
    echo "             for self-contained app."
    exit 1
fi

# Publish .NET app.
if [ "$#" -eq 1 ]; then
    dotnet publish \
        src/"$1"/"$1".csproj\
        -p:UseAppHost=false\
        -p:DebugType=None\
        -p:DebugSymbols=false\
        -c Release\
        -o artifacts/Publish/"$1"/
        exit 0
fi

# Publish self-contained app for specified platform.
if [ "$#" -eq 2 ]; then
    dotnet publish -r "$2"\
        src/"$1"/"$1".csproj\
        -p:PublishSingleFile=true\
        -p:IncludeNativeLibrariesForSelfExtract=true\
        -p:AllowedReferenceRelatedFileExtensions=none\
        -p:DebugType=embedded\
        --self-contained true\
        -c Release\
        -o artifacts/Publish/"$2"/"$1"/
        exit 0
fi
