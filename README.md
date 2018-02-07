# FileSharing
File hosting service built with [.NET Core](https://dotnet.github.io) and EntityFramework Code

## Features

* Possibility to store any file format or size.
* Sharing files publicly or between groups of users.
* Creating user groups administrables by the owner.
* Search box that filters any file accessible by the user.

## Requirements

* [.NET SDK](https://www.microsoft.com/net/learn/get-started/).
* [Bower](https://bower.io).
* A PostgreSQL or SQL Server Database (Check out [vagrant-sqlserver](https://github.com/VidelaRosa/vagrant-sqlserver).
* [Vagrant](https://www.vagrantup.com/downloads.html)
* [JMeter](http://jmeter.apache.org)

## Installation

* `git clone` this repository.
* `cd filesharing`
* Follow the [Usage](#usage) section.

## Usage

### Configure the database connection

* Set the right properties in the `persistence.json` file.
  * `DatabaseEngine` could be `SqlServer` to use a SQL Server database engine, `PostgreSQL` for a PostgreSQL or empty for a memory database (specially used in unit tests).
  * Create a connection string with the name of the database engine choosen (`SqlServer` or `PostgreSQL`).
* If you need to modify any model, it is necessary to add a new migrations. Use `dotnet ef migrations add` follows of the migration name.
* If you want to create/update the database manually (the application is able to run the migrations automatically), run `dotnet ef migrations add` follows of the migration name.
 
### Configure the services

* Set the right properties in the `services.json` file.
  * The `RepositoryPath` indicates the path where the files uploaded will be stored.
  * The `MinutesToExpireSession` is used to expire sessions which have an inactivity of the amount of time indicate.
  * The `Measurements` is an array of strings to choose the unit measeres to be displayed.
  
### Provisioning the Web

```
cd FileSharingWeb
bower install
```

### Executing the tests

* Run `dotnet test UnitTests/UnitTests.csproj` to execute the unit tests.
* Run `dotnet test IntegrationTests/IntegrationTests.csproj` to execute the integration tests.
* It is possible to run load and performance tests using JMeter with the .jmx files existing under JMeterTests.

### Running the app

* Use `dotnet run` to execute the app.
* If the database do not exists, the app will create the schema with the existing migrations not executed yet.

### Deploying the vagrant machine

* To deploy and test the application locally:
  * Publish the project to `./vagrant-filesharing/provision/FileSharing/`.
  * `cd vagrant-filesharing`.
  * `vagrant up`.
  * Open [http://localhost:8080](http://localhost:8080) in your browser.
