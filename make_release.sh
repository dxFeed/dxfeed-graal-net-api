git reset
git add Directory.Build.props README.md version.txt ReleaseNotes.txt
git commit -m "bump version"
git push origin
version="$(cat version.txt)"
git tag "$version"
git push origin "$version"
