IF OBJECT_ID('GoalEntry', 'U') IS NOT NULL
BEGIN
	DROP TABLE GoalEntry
END
GO

IF OBJECT_ID('GoalTarget', 'U') IS NOT NULL
BEGIN
	DROP TABLE GoalTarget
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
	Frequency varchar(100) NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_GoalDefinition
		PRIMARY KEY (Id)
)


CREATE TABLE GoalTarget(
	Id varchar(100) NOT NULL,
	
	GoalDefinitionId varchar(100) NOT NULL,
	
	TargetDate datetime NOT NULL,
	[Name] varchar(100) NULL,
	OptimalValue float NOT NULL,
	LowerValue float NOT NULL,
	HigherValue float NOT NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_GoalTarget
		PRIMARY KEY (Id),
	CONSTRAINT FK_GoalTarget_GoalDefinition FOREIGN KEY (GoalDefinitionId)
	REFERENCES GoalDefinition(Id)
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