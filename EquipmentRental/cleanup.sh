#!/bin/bash

docker-compose down -d
docker system prune -a

read -p "Enter to close!"