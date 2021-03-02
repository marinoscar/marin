# marin
Personal application a place for all utilities I need

# Configuration
## Blob Storage
A connection string and container need to be provided, the app setting for the connection string is `BlobStorage:ConnectionString` and the container name needs to be provided with setting `BlobStorage:Container`
## Authentication and Authorization
Authentication happens with Microsoft Account, for that the ClientId needs to be provided with the setting `Authentication:Microsoft:ClientId` and the Client Secret needs to be provided with the setting `Authentication:Microsoft:ClientSecret`
## Blog
The blog component requires a database connection string with the ConnectionString setting name `UserProfile`
