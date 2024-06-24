#!/bin/sh
sed -i "/<AssemblyVersion>/s/>.*</>""$1""</" Directory.Build.props
sed -i "/<FileVersion>/s/>.*</>""$1"".0</" Directory.Build.props
sed -i "/<Version>/s/>.*</>""$1""</" Directory.Build.props
sed -i "s/nuget-.*-blue/nuget-""$1""-blue/g" README.md
echo "$1" > version.txt
printf %"s\n\n" "## Version $1" | cat - ReleaseNotes.txt > temp && mv temp ReleaseNotes.txt
