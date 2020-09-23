## What is this

This is a test solution that demonstrates messaging between two microservices using rabbitMQ. To demonstrate this, I created a service that enables user to rent equipment. The user can access client application with functional front-end to do following:

* Get a list of rentable equipment
* Select number of days that individual machines are rented for
* Request an invoice for rented equipment

There also exists an inventory service that handles available inventory and invoice creation. Both services can be deployed independently. They communicate with each other using RabbitMQ. 

The solution should be fairly scalable. Initially, I looked into RabbitMQ RPC pattern, where message sender would supply receiver with callback queue and listen for response there. After some research, I discovered that MassTransit library already supports this approach under the hood with async GetResponse methods. For that reason, I decided against implementing this by hand.

Both microsevices can be built and run with .NET Core SDK. They are also supplied with Dockerfiles to easily create local docker images. There's also some scripts that make it easier to build docker images and clean the images up afterwards. Services are configured to work when built to images and images launched with supplied docker-compose.yml

When running inventory service with visual studio, appsettings.json file needs to be modified. Just use different rabbitmq host.

## How to run

It is recommended to run this solution using docker-compose file supplied, this ensures that rabbitMQ is also deployed. 

If you can run shell scripts, build-packages.sh builds whole .net project and creates local docker images (you can also call these commands manually in terminal). After successful docker images are built, you can launch containers by running `docker-compose up -d` command in terminal, .../EquiopmentRental/ directory. This will launch the containers in detached mode so you can stop them easily in the same terminal window. 

After containers were launched, you can monitor container status with `docker ps` command. To access frontend, go to http://localhost/ (assuming that port is free and you haven't changed port mapping in docker-compose file).

## Clean up


After you are done, you can stop the docker containers by running `docker-compose down` in terminal, .../EquipmentRental/ directory.

After stopping containers, you can use cleanup.sh script to clean up the images (Warning! This will remove all images that are not used for any running containers). Alternatively, these commands can be called manually in terminal.

