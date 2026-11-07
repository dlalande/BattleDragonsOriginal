/*
Do not change the database path or name variables.
It will be properly coded for build and deployment
This is using sqlcmd variable substitution
*/
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = 'PRIMARY',
		FILENAME = '$(DefaultDataPath)$(DefaultFilePrefix).mdf',
		SIZE = 1024KB,
		FILEGROWTH = 10%
	) TO FILEGROUP [PRIMARY]
	
