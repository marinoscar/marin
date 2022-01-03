IF OBJECT_ID('GoalEntry', 'U') IS NOT NULL
BEGIN
	DROP TABLE GoalEntry
END
GO

IF OBJECT_ID('GoalDefinition', 'U') IS NOT NULL
BEGIN
	DROP TABLE GoalDefinition
END
GO

CREATE TABLE GoalDefinition(
	Id varchar(100) NOT NULL,
	
	[Name] varchar(100) NULL,
	[Question] varchar(255) NULL,
	[Type] varchar(25) NULL,
	Frequency varchar(50) NULL,
	UnitOfMeasure varchar(50) NULL,
	TargetValue float NULL,
	Reminder datetime NULL,
	Notes varchar(500) NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_GoalDefinition
		PRIMARY KEY (Id)
)


CREATE TABLE GoalEntry(
	Id varchar(100) NOT NULL,
	
	GoalDefinitionId varchar(100) NOT NULL,
	
	GoalDateTime datetime NOT NULL,
	NumericValue float NOT NULL,
	StringValue varchar(255) NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_GoalEntry
		PRIMARY KEY (Id),
	CONSTRAINT FK_GoalEntry_GoalDefinition FOREIGN KEY (GoalDefinitionId)
	REFERENCES GoalDefinition(Id)
)