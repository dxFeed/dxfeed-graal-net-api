#!/bin/sh
# --serve     : Starts a web server at http://localhost:8080
cp README.md index.md
docfx docfx.json "$@"
