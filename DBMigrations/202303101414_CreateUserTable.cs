using FluentMigrator;

[Migration(2023_03_10_14_14, "Create User table")]
public class CreateUserTable : Migration
{
    public const string UserTable = @"""User""";
    public const string UserId = "user_id";
    public const string Username = "username";
    public const string PasswordHash = "password_hash";

    public override void Up()
    {
        Execute.Sql($@"
        CREATE TABLE {UserTable} (
            {UserId} SERIAL PRIMARY KEY,
            {Username} VARCHAR(255) NOT NULL,
            {PasswordHash} TEXT NOT NULL
        );

        CREATE INDEX {Username}_index ON {UserTable}({Username});
        ");
    }

    public override void Down()
    {   
        Execute.Sql($@"
        DROP TABLE IF EXISTS {UserTable};
        ");
    }
}
