namespace PeterKottas.DotNetCore.EnsureRunning.SqlServer
{
    public static class SqlServerSetup
    {
        public static int CurrentVersion = 1;

        public static string InstalSql = $@"
			CREATE TABLE Actions(
			   Id INT IDENTITY(1,1) NOT NULL,
			   ActionId VARCHAR (40) NOT NULL,
			   PRIMARY KEY (Id)
			);

			CREATE TABLE Runners(
			   Id INT IDENTITY(1,1) NOT NULL,
			   ActionId int NOT NULL,
			   LastHeartBeat DATETIME NOT NULL,  
			   PRIMARY KEY (Id),
			   FOREIGN KEY (ActionId) REFERENCES Actions(Id)
			);

			CREATE TABLE Version(
			   Value   INT NOT NULL
			);

			INSERT INTO Version (Value) Values({CurrentVersion});";

        public static string UninstalSql = @"
			DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR

            SET @Cursor = CURSOR FAST_FORWARD FOR
            SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + ']'
            FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
            LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME = rc1.CONSTRAINT_NAME

            OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql

            WHILE (@@FETCH_STATUS = 0)
            BEGIN
            Exec sp_executesql @Sql
            FETCH NEXT FROM @Cursor INTO @Sql
            END

            CLOSE @Cursor DEALLOCATE @Cursor
            GO

            EXEC sp_MSforeachtable 'DROP TABLE ?'
            GO";
    }
}
