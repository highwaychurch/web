USE MASTER
GO

PRINT 'Restoring database ' + N'$(DatabaseName)'

IF (DB_ID(N'$(DatabaseName)') IS NOT NULL
    AND DATABASEPROPERTYEX(N'$(DatabaseName)','Status') <> N'ONLINE')
BEGIN
    RAISERROR(N'The state of the target database, %s, is not set to ONLINE. To deploy to this database, its state must be set to ONLINE.', 16, 127,N'$(DatabaseName)') WITH NOWAIT
    RETURN
END
GO
IF (DB_ID(N'$(DatabaseName)') IS NOT NULL) 
BEGIN
    ALTER DATABASE [$(DatabaseName)]
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$(DatabaseName)];
END
GO
DECLARE @dbBackupLoc NVARCHAR(MAX)
SET @dbBackupLoc =  N'$(BackupLocation)' + N'$(BackupFile)'  --'Angel.bak'
RESTORE DATABASE [$(DatabaseName)]
FROM DISK =  @dbBackupLoc
WITH MOVE 'Angel' TO N'$(DataFile)'  ,
MOVE 'Angel_Log' To  N'$(LogFile)',
REPLACE
