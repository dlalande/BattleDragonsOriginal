create table [dbo].[MatchMove]
(
	[ID] int not null identity(1, 1), 
	[MoveKey] varchar(255) not null,
	[Cost] int not null,
	[MoveCoords] varbinary(500), -- 0x0102 => (1, 2)
	constraint [PK_MatchMove] primary key ([ID])
)