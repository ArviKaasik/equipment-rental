#!/bin/bash

dotnet build .
dotnet publish -c Release

cd ./EquipmentRental.Client
docker build -t rental-client -f Dockerfile .

cd ../EquipmentRental.Inventory
docker build -t rental-inventory -f Dockerfile .

read -p "Enter to close!"