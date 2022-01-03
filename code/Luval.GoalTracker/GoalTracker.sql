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
	ReminderDaysOfWeek varchar(100) NULL,
	Notes varchar(500) NULL,
	IsInactive bit NOT NULL,

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
GO
/* Views */
IF OBJECT_ID('VW_GoalTrackerList', 'V') IS NOT NULL
BEGIN
	DROP VIEW VW_GoalTrackerList
END
GO
CREATE VIEW VW_GoalTrackerList
AS
(
SELECT
	GoalDefinition.Id As GoalId,
	GoalDefinition.[Name] As [Name],
	GoalDefinition.Frequency As Frequency,
	GoalDefinition.ReminderDaysOfWeek,
	MAX(GoalEntry.GoalDateTime) As LastEntry,
	COUNT(GoalEntry.Id) As EntryCount
FROM
	GoalDefinition
	LEFT JOIN GoalEntry ON GoalEntry.GoalDefinitionId = GoalDefinition.Id
GROUP BY
	GoalDefinition.Id, GoalDefinition.[Name],
	GoalDefinition.Frequency, GoalDefinition.ReminderDaysOfWeek
)
GO