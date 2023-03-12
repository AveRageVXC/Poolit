using FluentMigrator;

[Migration(2023_03_10_15_23, "Create File table")]
public class CreateFileTable : Migration
{
    public const string FileTable = @"""File""";
    public const string FileId = "file_id";
    public const string Name = "name";
    public const string RealFileName = "real_file_name";
    public const string Description = "description";
    public const string CreationDate = "creation_date";
    public const string Size = "size";
    public const string OwnerId = "owner_id";
    public const string UserTable = CreateUserTable.UserTable;
    public const string UserId = CreateUserTable.UserId;
    public const string ContentType = "content_type";
    public const string S3Key = "s3_key";
    public const string PoolitKey = "poolit_key";

    public override void Up()
    {
        Execute.Sql($@"
        CREATE TABLE {FileTable} (
            {FileId} SERIAL PRIMARY KEY,
            {Name} VARCHAR(255) NOT NULL,
            {RealFileName} TEXT NOT NULL,
            {Description} TEXT,
            {CreationDate} TIMESTAMP WITH TIME ZONE NOT NULL,
            {Size} INT NOT NULL,
            {OwnerId} INT NOT NULL REFERENCES {UserTable}({UserId}) ON UPDATE CASCADE,
            {ContentType} TEXT NOT NULL,
            {S3Key} TEXT NOT NULL UNIQUE,
            {PoolitKey} TEXT NOT NULL UNIQUE
        );

        CREATE INDEX {Name}_index ON {FileTable}({Name});
        ");
    }
    
    public override void Down()
    {   
        Execute.Sql($@"
        DROP TABLE IF EXISTS {FileTable};
        ");
    }
}
