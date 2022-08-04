IF OBJECT_ID('HabitEntry', 'U') IS NOT NULL
BEGIN
	DROP TABLE HabitEntry
END
GO

IF OBJECT_ID('HabitDefinition', 'U') IS NOT NULL
BEGIN
	DROP TABLE HabitDefinition
END
GO

CREATE TABLE HabitDefinition(
	Id varchar(100) NOT NULL,
	
	[Name] varchar(100) NULL,
	[Question] varchar(255) NULL,
	[Type] varchar(25) NULL,
	Frequency varchar(50) NULL,
	UnitOfMeasure varchar(50) NULL,
	DailyTarget float NULL,
	WeeklyTarget float NULL,
	MonthlyTarget float NULL,
	YearlyTarget float NULL,
	WeeklyProgress float NULL,
	MonthlyProgress float NULL,
	YearlyProgress float NULL,
	CurrentWeek datetime NULL,
	Reminder datetime NULL,
	ReminderDaysOfWeek varchar(100) NULL,
	Notes varchar(500) NULL,
	IsInactive bit NOT NULL,
	Sort int NOT NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_HabitDefinition
		PRIMARY KEY (Id),
	INDEX IX_HabitDefinition_UserId  (CreatedByUserId)
)


CREATE TABLE HabitEntry(
	Id varchar(100) NOT NULL,
	
	HabitDefinitionId varchar(100) NOT NULL,
	
	EntryDateTime datetime NOT NULL,
	NumericValue float NOT NULL,
	Difficulty int NULL,
	Notes varchar(1000) NULL,

	UtcCreatedOn datetime NOT NULL,
	UtcUpdatedOn datetime NOT NULL,
	CreatedByUserId varchar(100) NULL,
	UpdatedByUserId varchar(100) NULL,

	CONSTRAINT PK_HabitEntry
		PRIMARY KEY (Id),
	CONSTRAINT FK_HabitEntry_HabitDefinition FOREIGN KEY (HabitDefinitionId)
	REFERENCES HabitDefinition(Id),
	INDEX IX_HabitEntry_UserId  (CreatedByUserId)
)
GO
/* Views */
IF OBJECT_ID('VW_HabitTrackerList', 'V') IS NOT NULL
BEGIN
	DROP VIEW VW_HabitTrackerList
END
GO
CREATE VIEW VW_HabitTrackerList
AS
(
SELECT
	Def.Id As GoalId,
	Def.[Name] As [Name],
	Def.Frequency As Frequency,
	Def.ReminderDaysOfWeek,
	Def.[Type],
	Def.WeeklyProgress,
	Def.MonthlyProgress,
	Def.YearlyProgress,
	Def.CreatedByUserId,
	Def.Sort,
	MIN([Entry].EntryDateTime) As FirstEntry,
	MAX([Entry].EntryDateTime) As LastEntry,
	COUNT([Entry].Id) As EntryCount
FROM
	HabitDefinition As Def
	LEFT JOIN HabitEntry As [Entry] ON [Entry].HabitDefinitionId = Def.Id
WHERE
	Def.IsInactive = 0
GROUP BY
	Def.Id, Def.[Name],
	Def.Frequency, Def.ReminderDaysOfWeek,
	Def.CreatedByUserId, Def.WeeklyProgress,
	Def.MonthlyProgress, Def.YearlyProgress,
	Def.[Type], Def.Sort
)
GO

/*Helpers*/
/*

--User: 92bb590a-b7ee-448e-9e22-fa49c27c76be

SELECT [Id]
      ,[GoalDefinitionId] As HabitDefinition
      ,[GoalDateTime] As EntryDateTime
      ,[NumericValue]
	  ,NULL As Difficulty
      ,NULL As Notes
      ,[UtcCreatedOn]
      ,[UtcUpdatedOn]
      ,'92bb590a-b7ee-448e-9e22-fa49c27c76be' As [CreatedByUserId]
      ,'92bb590a-b7ee-448e-9e22-fa49c27c76be' As [UpdatedByUserId]
  FROM [dbo].[GoalEntry]

GO

SELECT [Id]
      ,[Name]
      ,[Question]
      ,[Type]
      ,[Frequency]
      ,[UnitOfMeasure]
      ,[DailyTarget]
      ,[WeeklyTarget]
      ,[MonthlyTarget]
      ,[YearlyTarget]
      ,[WeeklyProgress]
      ,[MonthlyProgress]
      ,[YearlyProgress]
      ,[Reminder]
      ,[ReminderDaysOfWeek]
      ,[Notes]
      ,0 As [IsInactive]
      ,[Sort]
      ,[UtcCreatedOn]
      ,[UtcUpdatedOn]
      ,'92bb590a-b7ee-448e-9e22-fa49c27c76be' As [CreatedByUserId]
      ,'92bb590a-b7ee-448e-9e22-fa49c27c76be' As [UpdatedByUserId]
  FROM [dbo].[GoalDefinition]

*/
