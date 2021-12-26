IF OBJECT_ID('ApplicationRoleClaim', 'U') IS NOT NULl
	DROP TABLE ApplicationRoleClaim
GO

IF OBJECT_ID('ApplicationUserRole', 'U') IS NOT NULl
	DROP TABLE ApplicationUserRole
GO

IF OBJECT_ID('ApplicationRole', 'U') IS NOT NULl
	DROP TABLE ApplicationRole
GO

IF OBJECT_ID('ApplicationUser', 'U') IS NOT NULl
	DROP TABLE ApplicationUser
GO

IF OBJECT_ID('SecurityClaim', 'U') IS NOT NULl
	DROP TABLE SecurityClaim
GO

IF OBJECT_ID('SafeItem', 'U') IS NOT NULl
	DROP TABLE SafeItem
GO

CREATE TABLE SafeItem(
	Id varchar(100) NOT NULL,
	ItemName varchar(255) NOT NULL,
	ItemValue varchar(max) NOT NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_SafeItem
		PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_SafeItem
		UNIQUE (ItemName)
)

CREATE TABLE ApplicationUser(
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
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_ApplicationUser
		PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_ApplicationUser
		UNIQUE (ProviderName, Email)
)


CREATE TABLE ApplicationRole(
	Id varchar(100) NOT NULL,
	RoleName varchar(150) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_ApplicationRole
		PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT UQ_ApplicationRole
		UNIQUE (RoleName)
)

CREATE TABLE ApplicationUserRole(
	Id varchar(100) NOT NULL,
	ApplicationRoleId varchar(100) NOT NULL,
	ApplicationUserId varchar(100) NOT NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_ApplicationUserRole
		PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT FK_ApplicationUserRole_ApplicationUser FOREIGN KEY (ApplicationUserId)
		REFERENCES ApplicationUser(Id),
	CONSTRAINT FK_ApplicationUserRole_ApplicationRole FOREIGN KEY (ApplicationRoleId)
		REFERENCES ApplicationRole(Id)
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
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_SecurityClaim
		PRIMARY KEY CLUSTERED (Id)
)

CREATE TABLE ApplicationRoleClaim(
	Id varchar(100) NOT NULL,
	ApplicationRoleId varchar(100) NULL,
	SecurityClaimId varchar(100) NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NOT NULL,
	UpdatedByUserId varchar(100) NOT NULL,

	CONSTRAINT PK_ApplicationRoleClaim
		PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT FK_ApplicationRoleClaim_ApplicationRole FOREIGN KEY (ApplicationRoleId)
		REFERENCES ApplicationRole(Id),
	CONSTRAINT FK_SecurityRoleClaim_SecurityClaim FOREIGN KEY (SecurityClaimId)
		REFERENCES SecurityClaim(Id)
)


/* Sample Records */
DELETE FROM ApplicationUserRole WHERE Id = '7EEF24B4-AA12-4D12-911D-DACDFA8FC264'
GO

DELETE FROM ApplicationUser Where Id = '6094B270-5566-41F6-84CF-2F085D62B441'
GO
/*
INSERT INTO [dbo].[ApplicationUser]
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
           ,[CreatedByUserId]
           ,[UpdatedByUserId])
     VALUES
           (
		    '6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'Microsoft'
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
*/
INSERT INTO [dbo].[ApplicationUser]
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
           ,[CreatedByUserId]
           ,[UpdatedByUserId])
     VALUES
           (
		    '6094B270-5566-41F6-84CF-2F085D62B441'
           ,''
           ,'Microsoft'
           ,'oscar.marin.saenz@outlook.com'
           ,''
           ,''
           ,''
           ,null
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO
DELETE FROM ApplicationRole WHERE Id = '07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
GO
INSERT INTO [dbo].[ApplicationRole]
           ([Id]
           ,[RoleName]
           ,[UtcCreatedOn]
           ,[UtcUpdatedOn]
           ,[CreatedByUserId]
           ,[UpdatedByUserId])
     VALUES
           ('07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
            ,'Admin'
            ,GETUTCDATE()
            ,GETUTCDATE()
            ,'6094B270-5566-41F6-84CF-2F085D62B441'
            ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO

INSERT INTO [dbo].[ApplicationUserRole]
           ([Id]
           ,[ApplicationRoleId]
           ,[ApplicationUserId]
           ,[UtcCreatedOn]
           ,[UtcUpdatedOn]
           ,[CreatedByUserId]
           ,[UpdatedByUserId])
     VALUES
           ('7EEF24B4-AA12-4D12-911D-DACDFA8FC264'
           ,'07AD5D84-9C2C-41F8-B02B-B031C3669CEA'
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,'6094B270-5566-41F6-84CF-2F085D62B441'
           ,'6094B270-5566-41F6-84CF-2F085D62B441')
GO

IF OBJECT_ID('ActivityExecutionStatus', 'U') IS NOT NULl
	DROP TABLE ActivityExecutionStatus
GO

IF OBJECT_ID('ActivitySession', 'U') IS NOT NULl
	DROP TABLE ActivitySession
GO

CREATE TABLE ActivitySession(
	Id varchar(100) NOT NULL,
	RunnerType varchar(500) NOT NULL,
	MachineName varchar(200) NOT NULL,
	UserName varchar(200) NOT NULL,
	[Status] varchar(50) NOT NULL,
	ExceptionReasonType varchar(100) NULL,
	[ExceptionReason] varchar(MAX) NULL,
	UtcStartedOn datetime NOT NULL,
	UtcEndedOn datetime NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,

	CONSTRAINT PK_ActivitySession
		PRIMARY KEY (Id),
	INDEX IX_ActivitySession_UtcStartedOn
		 (UtcStartedOn) 
)

CREATE TABLE ActivityExecutionStatus(
	Id varchar(100) NOT NULL,
	ActivitySessionId varchar(100) NOT NULL,
	ActivityType varchar(500) NOT NULL,
	ActivityName varchar(100) NOT NULL,
	[Status] varchar(50) NOT NULL,
	ExceptionReasonType varchar(100) NULL,
	ExceptionReason varchar(MAX) NULL,
	UtcStartedOn datetime NOT NULL,
	UtcEndedOn datetime NULL,
	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,

	CONSTRAINT PK_ActivityExecutionStatus
		PRIMARY KEY (Id),
	INDEX IX_ActivityExecutionStatus_UtcStartedOn
		 (UtcStartedOn),
	CONSTRAINT FK_ActivityExecutionStatus_ActivitySession FOREIGN KEY (ActivitySessionId)
		REFERENCES ActivitySession (Id)
)

IF OBJECT_ID('TimeSeries', 'U') IS NOT NULl
	DROP TABLE TimeSeries
GO

CREATE TABLE TimeSeries(
	Id bigint NOT NULL IDENTITY(1,1),
	DataLabel varchar(100) NOT NULL,
	UtcTimestamp datetime NOT NULL,
	NumericValue decimal NULL,
	StringValue varchar(250) NULL,
	CreatedByUserId varchar(100) NULL,

	CONSTRAINT PK_TimeSeries
		PRIMARY KEY (Id),
	INDEX IX_TimeSeries_Time (UtcTimestamp),
	INDEX IX_TimeSeries_Label (DataLabel),
)

IF OBJECT_ID('LogMessage', 'U') IS NOT NULl
	DROP TABLE LogMessage
GO

CREATE TABLE LogMessage(
	Id bigint NOT NULL IDENTITY(1,1),
	MachineName varchar(100) NOT NULL,
	UtcTimestamp datetime NOT NULL,
	MessageType int NOT NULL,
	Logger varchar(255) NULL,
	[Message] varchar(max) NULL,
	[Exception] varchar(max) NULL,

	CONSTRAINT PK_LogMessage
		PRIMARY KEY CLUSTERED (Id),
	INDEX IX_LogMessage_Time (UtcTimestamp)
)
