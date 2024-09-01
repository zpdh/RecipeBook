using FluentMigrator;

namespace RecipeBook.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.TableUser, "Create user table")]
public class Version1 : VersionBase
{
    public override void Up()
    {
        CreateTable("Users")
            .WithColumn("Name").AsString(255).NotNullable()
            .WithColumn("Email").AsString(255).NotNullable()
            .WithColumn("Password").AsString(2000).NotNullable()
            .WithColumn("UserIdentifier").AsGuid().NotNullable();
    }
}