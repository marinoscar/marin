IF OBJECT_ID('MediaItem', 'U') IS NOT NULL
BEGIN
	DROP TABLE MediaItem
END
GO

IF OBJECT_ID('MediaDrive', 'U') IS NOT NULL
BEGIN
	DROP TABLE MediaDrive
END
GO

IF OBJECT_ID('GraphAuthenticationToken', 'U') IS NOT NULL
BEGIN
	DROP TABLE GraphAuthenticationToken
END
GO

CREATE TABLE MediaItem(
	Id varchar(100) NOT NULL,
	
	MediaId varchar(100) NOT NULL,
	MediaType varchar(100) NULL,
	MediaMimeType varchar(100) NULL,
	[Hash] varchar(500) NULL,
	DriveId varchar(100) NULL,
	DrivePath varchar(2500) NULL,

	MediaCreatedTime datetime NULL,
	MediaUpdatedTime datetime NULL,

	[Name] varchar(500) NOT NULL,
	Size decimal NOT NULL,
	WebUrl varchar(100) NOT NULL,
	ThumbSmall varchar(1000) NULL,
	ThumbMid varchar(1000) NULL,
	ThumbLarge varchar(1000) NULL,

	CreatedByApp varchar(100) NULL,
	CreatedByUser varchar(100) NULL,

	FileCreationDateTime datetime NULL,
	FileUpdateDateTime datetime NULL,
	MediaTakenDateTime datetime NULL,

	LocationAltitute float NULL,
	LocationLatitude float NULL,
	Locationlongitude float NULL,

	Country varchar(100) NULL,
	Region varchar(100) NULL,
	City varchar(100) NULL,
	SubRegion varchar(100) NULL,
	PostalCode varchar(100) NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_MediaItem
		PRIMARY KEY (Id)
)

CREATE TABLE MediaDrive(
	Id varchar(100) NOT NULL,
	
	DriveId varchar(100) NOT NULL,
	DriveType varchar(100) NOT NULL,
	[Provider] varchar(50) NOT NULL,
	OnwerAccountEmail varchar(100) NOT NULL,
	[DrivePath] varchar(2500) NULL,
	[Name] varchar(500) NULL,
	WebUrl varchar(100) NULL,
	LookInChildren bit NOT NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_MediaDrive
		PRIMARY KEY (Id)
)

CREATE TABLE GraphAuthenticationToken(
	Id varchar(100) NOT NULL,
	
	UserId varchar(100) NOT NULL,
	PrincipalEmail varchar(100) NOT NULL,
	Token varchar(max) NOT NULL,
	IdToken varchar(max) NOT NULL,
	RefreshToken varchar(max) NOT NULL,
	UtcExpiration datetime NOT NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_GraphAuthenticationToken
		PRIMARY KEY (Id)
)


