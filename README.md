# marin
Personal application a place for all utilities I need

# Configuration
Configuration happens in the Startup.cs file with some settings and parameters, also a SQL Server database implementation is required and the script is located [here](https://github.com/marinoscar/marin/blob/main/code/MarinDb.sql) for the storage of the sensitive information here is the secret storage [link](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows)
## Blob Storage
A connection string and container need to be provided, the app setting for the connection string is `BlobStorage:ConnectionString` and the container name needs to be provided with setting `BlobStorage:Container`
## Authentication and Authorization
Authentication happens with [Microsoft Account](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-5.0), for that the ClientId needs to be provided with the setting `Authentication:Microsoft:ClientId` and the Client Secret needs to be provided with the setting `Authentication:Microsoft:ClientSecret` for authorization a sql connection string is required with the connection name `UserProfile`
## Blog
The blog component requires a database connection string with the ConnectionString setting name `UserProfile`
