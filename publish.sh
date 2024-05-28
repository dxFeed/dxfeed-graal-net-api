#!/bin/sh

# Define output path for published applications
output_path="artifacts/Publish/"
project_name=""
target_framework=""
target_platform=""

# Function to sign macOS applications
# $1: Path to the application to be signed
sign() {
    path="$1"
    # Check if codesign is available before attempting to sign
    if command -v codesign > /dev/null; then
        codesign -f -s - "$path"
    fi
}

# Function to publish framework-dependent deployment (FDD)
# Publishes the specified .NET project for the given framework
publish_fdd() {
    dotnet publish \
        src/"$project_name"/"$project_name".csproj \
        -p:UseAppHost=false \
        -p:DebugType=None \
        -p:DebugSymbols=false \
        --framework "$target_framework" \
        -c Release \
        -o "$output_path"/"$project_name"/"$target_framework"/
}

# Function to publish self-contained deployment (SCD)
# Publishes the specified .NET project for the given framework and platform
publish_scd() {
    dotnet publish -r "$target_platform" \
        src/"$project_name"/"$project_name".csproj \
        -p:PublishSingleFile=true \
        -p:IncludeNativeLibrariesForSelfExtract=true \
        -p:AllowedReferenceRelatedFileExtensions=none \
        -p:DebugType=None \
        -p:DebugSymbols=false \
        --framework "$target_framework" \
        --self-contained true \
        -c Release \
        -o "$output_path"/"$project_name"/"$target_platform"/

    # Sign the macOS application if the target platform is macOS
    case "$target_platform" in
      *osx*) sign "$output_path/$project_name/$target_platform/$project_name" ;;
    esac
}

# Main function to control the script flow
main() {
    if [ "$#" -eq 0 ]; then
        # Print usage instructions if no arguments are provided
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

    project_name="$1"
    target_framework="$2"
    target_platform="$3"

    # Determine which type of deployment to publish based on the number of arguments
    if [ "$#" -eq 2 ]; then
        publish_fdd
    elif [ "$#" -eq 3 ]; then
        publish_scd
    fi
    exit 0
}

# Execute the main function with all passed arguments
main "$@"
