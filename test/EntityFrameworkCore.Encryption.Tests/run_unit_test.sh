#!/bin/sh

dotnet test --filter "Category!=Integration"
exit $?