using FluentMigrator;

[Migration(2023_03_10_17_36, "Create User_File table")]
public class CreateUserFileTable : Migration
{
    public const string UserFileTable = @"""User_File""";
    public const string UserFileId = "user_file_id";
    public const string UserTable = CreateUserTable.UserTable;
    public const string UserId = CreateUserTable.UserId;    
    public const string FileTable = CreateFileTable.FileTable;
    public const string FileId = CreateFileTable.FileId;
    
    public override void Up()
    {
        Execute.Sql($@"
        CREATE TABLE {UserFileTable} (
            {UserFileId} SERIAL PRIMARY KEY,
            {UserId} INT NOT NULL REFERENCES {UserTable}({UserId}) ON UPDATE CASCADE,
            {FileId} INT NOT NULL REFERENCES {FileTable}({FileId}) ON UPDATE CASCADE ON DELETE CASCADE
        );
        ");
    }
    
    public override void Down()
    {
        
        Execute.Sql($@"
        DROP TABLE IF EXISTS {UserFileTable};
        ");
    }
}
