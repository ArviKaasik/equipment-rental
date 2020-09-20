#!/bin/bash

docker-compose down -d
docker system prune -a
dotnet clean .

read -p "Enter to close!"