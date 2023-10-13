#/bin/bash

set -e
set -x

dotnet --list-sdks

sln_name="all"
rm ${sln_name}.sln -f
dotnet new sln --name ${sln_name}

find . -name "*.csproj" -exec dotnet sln ${sln_name}.sln add {} \;
