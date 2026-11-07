create table [dbo].[Match]
(
	[ID] int not null identity(1, 1),
	[Player1ClientID] uniqueidentifier,
	[Player2ClientID] uniqueidentifier,
	[CurrentPlayer] tinyint,
	[InitMana] int not null default 10,
	[Outcome] smallint, 
	constraint [PK_Match] primary key ([ID]),
	constraint [FK_Match_Player1Client] foreign key ([Player1ClientID]) references [Client]([ID]),
	constraint [FK_Match_Player2Client] foreign key ([Player2ClientID]) references [Client]([ID])
)
