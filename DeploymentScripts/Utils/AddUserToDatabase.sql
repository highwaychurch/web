USE MASTER
GO

IF NOT EXISTS(SELECT name FROM sys.server_principals WHERE name = N'$(UserName)')
BEGIN
	PRINT 'Creating server login ' + N'$(UserName)'
    CREATE LOGIN [$(UserName)] FROM WINDOWS
END
ELSE
BEGIN
	PRINT 'Server login already exists for ' + N'$(UserName)'
END

USE [$(DatabaseName)]
GO
IF NOT EXISTS(SELECT name FROM sys.database_principals WHERE name = N'$(UserName)')
BEGIN
	PRINT 'Creating database user ' + N'$(UserName)' + ' as ' + N'$(DatabaseRole)'
	CREATE USER [$(UserName)] FOR LOGIN [$(UserName)]
	EXEC sp_addrolemember N'$(DatabaseRole)', N'$(UserName)'
END
ELSE
BEGIN
	PRINT 'Database user already exists for ' + N'$(UserName)'
END