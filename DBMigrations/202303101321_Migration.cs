using FluentMigrator;

[Migration(2023_03_10_13_21)]
public class TestMigration : Migration
{
    public override void Up()
    {
        Execute.Sql($@"
        -- Some sql code to create smth
        ");
    }

    public override void Down()
    {
        
        Execute.Sql($@"
        -- Some sql code to delete smth
        ");
    }
}
