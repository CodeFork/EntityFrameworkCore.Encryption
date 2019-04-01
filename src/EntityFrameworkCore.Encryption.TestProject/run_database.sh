#!/bin/sh

docker run --name encryption-test-postgres -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=Test1234 -p 25432:5432 -d postgres