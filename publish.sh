#!/bin/sh

if [ "$#" -eq 2 ]; then
    # Publish .NET app.
    dotnet publish\
        src/"$1"/"$1".csproj\
        -p:UseAppHost=false\
        -p:DebugType=None\
        -p:DebugSymbols=false\
        --framework "$2"\
        -c Release\
        -o artifacts/Publish/"$1"/
    exit 0
elif [ "$#" -eq 3 ]; then
    # Publish self-contained app for specified platform.
    dotnet publish -r "$3"\
        src/"$1"/"$1".csproj\
        -p:PublishSingleFile=true\
        -p:IncludeNativeLibrariesForSelfExtract=true\
        -p:AllowedReferenceRelatedFileExtensions=none\
        -p:DebugType=None\
        --self-contained true\
        --framework "$2"\
        -c Release\
        -o artifacts/Publish/"$3"/"$1"/
    exit 0
else
    # Print usage.
    echo "Usage:"
    echo "  <project> <framework> [<platform>]"
    echo ""
    echo "Where:"
    echo "  project     Project name."
    echo "  framework   .NET version (net6.0, net7.0, net8.0)."
    echo "  platform    Platform name (win-x64, linux-x64, osx-x64, osx-arm64)"
    echo "              for self-contained app."
    exit 1
fi
