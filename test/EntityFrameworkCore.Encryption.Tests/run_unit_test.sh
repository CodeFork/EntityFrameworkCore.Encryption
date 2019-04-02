#!/bin/bash

dotnet test --filter "Category!=Integration"
exit $?