create table [dbo].[MatchLayout]
(
	[MatchID] int not null,
	[Player] tinyint not null,
	[X] smallint not null,
	[Y] smallint not null,
	[PieceKey] varchar(255) not null,
	[Value] int,
	constraint [FK_MatchLayout_Match] foreign key ([MatchID]) references [Match]([ID]), 
	constraint [PK_MatchLayout] primary key ([MatchID], [Player], [X], [Y])
)
