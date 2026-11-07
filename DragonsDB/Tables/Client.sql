create table [dbo].[Client]
(
	[ID] uniqueidentifier not null,
	[ClientName] nvarchar(255),
	[IsOnline] bit not null default 0, 
	constraint [PK_Client] primary key ([ID])
)
