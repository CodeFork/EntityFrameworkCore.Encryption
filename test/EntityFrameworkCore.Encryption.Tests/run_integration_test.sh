#!/bin/bash

export UseEncryption=false

echo "1. Step: Adding first migration (unencrypted)"
dotnet ef migrations add init
migration1_result=$?

echo "2. Step: Asserting that unencrypted database can be read"
dotnet test --filter "FullyQualifiedName~IntegrationTest&Step=1"
step1_result=$?

echo "3. Step: Activating encryption"
export UseEncryption=true

echo "4. Step: Adding migration for encrypted data"
dotnet ef migrations add init2
migration2_result=$?

echo "5. Step: Asserting that new data will be stored encrypted (but keep old data as it is)"
dotnet test --filter "FullyQualifiedName~IntegrationTest&Step=2"
step2_result=$?

echo "6. Step: Asserting that all data can be encrypted using migrator"
dotnet test --filter "FullyQualifiedName~IntegrationTest&Step=3"
step3_result=$?

echo "Clean up"
rm -r Migrations
find . -name "blogging.db" -type f -delete

echo "Validating result"

if [[ ${step1_result} -eq 0 ]] && [[ ${step2_result} -eq 0 ]] && [[ ${step3_result} -eq 0 ]] && [[ ${migration1_result} -eq 0 ]] && [[ ${migration2_result} -eq 0 ]]
  then 
    echo "Commands succeeded :)"
    exit 0
  else
    echo "At least one step failed :("
    exit 1
fi