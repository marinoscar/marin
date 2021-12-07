IF OBJECT_ID('MediaGallery', 'U') IS NOT NULL
BEGIN
	DROP TABLE MediaGallery
END
GO

IF OBJECT_ID('MediaDrive', 'U') IS NOT NULL
BEGIN
	DROP TABLE MediaDrive
END
GO

CREATE TABLE MediaGallery(
	Id varchar(100) NOT NULL,
	
	MediaId varchar(100) NOT NULL,
	MediaType varchar(100) NOT NULL,
	Media256Hash varchar(500) NULL,
	DriveId varchar(100) NULL,
	DrivePath varchar(2500) NULL,

	MediaCreatedTime datetime NULL,
	MediaUpdatedTime datetime NULL,

	[Name] varchar(500) NOT NULL,
	Size decimal NOT NULL,
	WebUrl varchar(100) NOT NULL,
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

	CONSTRAINT PK_MediaGallery
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


