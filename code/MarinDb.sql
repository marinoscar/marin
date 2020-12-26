IF OBJECT_ID('UserProfile', 'U') IS NOT NULl
	DROP TABLE UserProfile
GO

CREATE TABLE UserProfile(
	Id varchar(100) NOT NULL,
	ProviderKey varchar(100) NOT NULL,
	ProviderName varchar(100) NOT NULL,
	Email varchar(150) NOT NULL,
	UserName varchar(150) NULL,
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
