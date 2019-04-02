#!/bin/sh

./run_unit_test.sh
unit_test_result=$?

./run_integration_test.sh
integration_test_result=$?

if [[ ${unit_test_result} -eq 0 ]] && [[ ${integration_test_result} -eq 0 ]]
    then
        echo "Both test suites succeeded :)"
        exit 0
    else
        echo "At least one test suite failed :("
        exit 1
fi