dotnet publish -r %1^
 src/DxFeed.Graal.Net.Tools/DxFeed.Graal.Net.Tools.csproj^
 -p:PublishSingleFile=true^
 -p:PublishReadyToRun=true^
 -p:IncludeNativeLibrariesForSelfExtract=true^
 -p:AllowedReferenceRelatedFileExtensions=none^
 -p:DebugType=embedded^
 --self-contained true^
 -c Release^
 -o artifacts/Publish/%1
