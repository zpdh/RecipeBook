using FluentMigrator;

namespace RecipeBook.Infrastructure.Migrations.Versions;

[Migration(DatabaseVersions.ImageRecipesColumn, "Add column to save images on recipe table")]
public class Version3 : VersionBase
{
    public override void Up()
    {
        Alter.Table("Recipes")
            .AddColumn("ImageIdentifier").AsString().Nullable();
    }
}