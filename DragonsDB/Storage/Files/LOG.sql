/*
Do not change the database path or name variables.
It will be properly coded for build and deployment
This is using sqlcmd variable substitution
*/
ALTER DATABASE [$(DatabaseName)]
ADD LOG FILE
(
	NAME = 'LOG',
	FILENAME = '$(DefaultLogPath)$(DefaultFilePrefix).ldf',
	SIZE = 1024 KB,
	FILEGROWTH = 10%
)
