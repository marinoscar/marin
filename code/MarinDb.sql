IF OBJECT_ID('SecurityRoleClaim', 'U') IS NOT NULl
	DROP TABLE SecurityRoleClaim
GO

IF OBJECT_ID('UserSecurityRole', 'U') IS NOT NULl
	DROP TABLE UserSecurityRole
GO

IF OBJECT_ID('SecurityRole', 'U') IS NOT NULl
	DROP TABLE SecurityRole
GO

IF OBJECT_ID('UserProfile', 'U') IS NOT NULl
	DROP TABLE UserProfile
GO

IF OBJECT_ID('SecurityClaim', 'U') IS NOT NULl
	DROP TABLE SecurityClaim
GO

CREATE TABLE UserProfile(
	Id varchar(100) NOT NULL,
	ProviderKey varchar(100) NOT NULL,
	ProviderName varchar(100) NOT NULL,
	Email varchar(150) NOT NULL,
	DisplayName varchar(150) NULL,
	FirstName varchar(75) NULL,
	LastName varchar(75) NULL,
	ProfilePicture varchar(500) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserProfileId varchar(100) NOT NULL,
	UpdatedByUserProfileId varchar(100) NOT NULL,

	CONSTRAINT PK_UserProfile
		PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_UserProfile
		UNIQUE (ProviderName, Email)
)


CREATE TABLE SecurityRole(
	Id varchar(100) NOT NULL,
	RoleName varchar(150) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserProfileId varchar(100) NOT NULL,
	UpdatedByUserProfileId varchar(100) NOT NULL,

	CONSTRAINT PK_SecurityRole
		PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_SecurityRole
		UNIQUE (RoleName)
)

CREATE TABLE UserSecurityRole(
	Id varchar(100) NOT NULL,
	SecurityRoleId varchar(100) NOT NULL,
	UserProfileId varchar(100) NOT NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserProfileId varchar(100) NOT NULL,
	UpdatedByUserProfileId varchar(100) NOT NULL,

	CONSTRAINT PK_UserSecurityRole
		PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT FK_UserSecurityRole_UserProfile FOREIGN KEY (UserProfileId)
		REFERENCES UserProfile(Id),
	CONSTRAINT FK_UserSecurityRole_SecurityRole FOREIGN KEY (SecurityRoleId)
		REFERENCES SecurityRole(Id)
)


/* 

	Claim Type: Name of functionality to control
	Claims Values:
		* Read   -> Read content 
		* Write  -> Write content
		* Manage -> Manage basic configuration
		* Admin  -> Super user abilities
*/
CREATE TABLE SecurityClaim(
	Id varchar(100) NOT NULL,
	ClaimType varchar(255) NULL,
	ClaimValue varchar(255) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserProfileId varchar(100) NOT NULL,
	UpdatedByUserProfileId varchar(100) NOT NULL,

	CONSTRAINT PK_SecurityClaim
		PRIMARY KEY CLUSTERED (Id)
)

CREATE TABLE SecurityRoleClaim(
	Id varchar(100) NOT NULL,
	SecurityRoleId varchar(100) NULL,
	SecurityClaimId varchar(100) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserProfileId varchar(100) NOT NULL,
	UpdatedByUserProfileId varchar(100) NOT NULL,

	CONSTRAINT PK_SecurityRoleClaim
		PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT FK_SecurityRoleClaim_SecurityRole FOREIGN KEY (SecurityRoleId)
		REFERENCES SecurityRole(Id),
	CONSTRAINT FK_SecurityRoleClaim_SecurityClaim FOREIGN KEY (SecurityClaimId)
		REFERENCES SecurityClaim(Id)
)


/* Sample Records */
DELETE FROM UserSecurityRole WHERE Id = '7EEF24B4-AA12-4D12-911D-DACDFA8FC264'
GO

DELETE FROM UserProfile Where Id = '6094B270-5566-41F6-84CF-2F085D62B441'
GO
INSERT INTO [dbo].[UserProfile]
           ([Id]
           ,[ProviderKey]
           ,[ProviderName]
           ,[Email]
           ,[DisplayName]
           ,[FirstName]
           ,[LastName]
           ,[ProfilePicture]
           ,[UtcCreatedOn]
           ,[UtcUpdatedOn]
           ,[CreatedByUserProfileId]
           ,[UpdatedByUserProfileId])
     VALUES
           (
		    '6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'Microsft'
           ,'oscar.marin.saenz@outlook.com'
           ,'Oscar Marin'
           ,'Oscar'
           ,'Marin'
           ,null
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO


DELETE FROM SecurityRole WHERE Id = '07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
GO
INSERT INTO [dbo].[SecurityRole]
           ([Id]
           ,[RoleName]
           ,[UtcCreatedOn]
           ,[UtcUpdatedOn]
           ,[CreatedByUserProfileId]
           ,[UpdatedByUserProfileId])
     VALUES
           ('07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
            ,'Admin'
            ,GETUTCDATE()
            ,GETUTCDATE()
            ,'6094B270-5566-41F6-84CF-2F085D62B441'
            ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO

INSERT INTO [dbo].[UserSecurityRole]
           ([Id]
           ,[SecurityRoleId]
           ,[UserProfileId]
           ,[UtcCreatedOn]
           ,[UtcUpdatedOn]
           ,[CreatedByUserProfileId]
           ,[UpdatedByUserProfileId])
     VALUES
           ('7EEF24B4-AA12-4D12-911D-DACDFA8FC264'
           ,'07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO
