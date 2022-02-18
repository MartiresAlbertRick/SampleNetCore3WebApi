# AD CAAPS API

## Required Knowledge

1. Atleast .NET Core 2.0 or higher
2. Entity Framework Core (EFCore)
3. RestAPI
4. API Testing
5. Azure Web App Deployment and Implementation of Azure API Management


## Required Tools to develop and test/run the Web API locally

1. Visual Studio 2015/2017
2. IIS
3. API Testing Tools (e.g. Swagger, Postman, Curl, etc.)


## NuGet Package

* Restore nuget packages


## Configurations


### Logging Directory 

* Install NLog, NLog.Web.AspNetCore and NLog.config from NuGet
* To change, modify NLog.config file "logfile" line

### Environment Settings

* Copy appsettings-sample.json and write the file as appsettings.json
**Note:** *By default, appsettings.json is transformed based on appsettings.Development.json. To change the environment, go to \API\Properties\launchSettings.json then change the value of ASPNETCORE_ENVIRONMENT under profiles section.*

```json
"profiles": {
    "IIS Express": {
        "commandName": "IISExpress",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
        }
    },
    "API": {
        "commandName": "Project",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Development"
        },
        "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
}
```

### Application Settings

* Database connection is required. Please configure the database connection string per client per environment in the ConnectionStrings collection. See example below.

```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=.;Initial Catalog=CAAPS_2018_CAAPS_FROM_RDS;User Id=uid;Password=****",
    "ECL": "Data Source=.;Initial Catalog=ECL_GROUP_2018_CAAPS_FROM_RDS;User Id=uid;Password=****"
}
```


## Web API Usage/Testing using Swagger

1. Make sure that Swashbuckle.AspNetCore is installed in NuGet.
2. Run the API project in IIS Express then on the browser, go to Swagger UI by entering this  URL https://localhost:44308/swagger/index.html
3. Read documentations from  Swagger UI on how to make a successful call to the APIs
4. Requests should always include the client from the query. This is to make sure that it points to a correct client configuration.


## Web API Usage/Testing using Postman

1. Make sure PostMan is installed on your local machine
2. Access the API through this URL http://localhost:63186/api/
3. Requests should always include client either by adding it through the query or adding a header (x-client-id).
** Note: ** *Client is required to make sure that it points to a correct client configurations
* Example
- Adding a parameter in URL http://localhost:63186/api/vendors?client=CAAPS or
- Adding a header x-client-id=CAAPS
4. You can also use OData services through PostMan. Find out basic in OData https://www.odata.org/getting-started/basic-tutorial/

## How to build and deploy

1. Open D:/ad-caaps-api/CAAPS_API.sln in visual studio 2015/2017
2. Build Project/Web App
3. Deploy
