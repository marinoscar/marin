IF OBJECT_ID('UrlShortner', 'U') IS NOT NULL
BEGIN
	DROP TABLE UrlShortner
END
GO

CREATE TABLE UrlShortner(
	Id varchar(100) NOT NULL,
	OriginalUri nvarchar(MAX) NOT NULL,
	RecordCount bigint NOT NULL,
	UtcLastAccesedTime datetime NOT NULL,
	UtcCreatedTime datetime NOT NULL,

	CONSTRAINT PK_UrlShortner
		PRIMARY KEY CLUSTERED (Id)
)