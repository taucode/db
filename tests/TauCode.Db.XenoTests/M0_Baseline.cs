using FluentMigrator;

namespace TauCode.Db.XenoTests
{
    [Migration(0)]
    public class M0_Baseline : AutoReversingMigration
    {
        public override void Up()
        {
            // app
            this.Create.Table("app")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_app")
                .WithColumn("code")
                    .AsAnsiString(128)
                    .NotNullable()
                .WithColumn("name")
                    .AsString(128)
                    .NotNullable()
                .WithColumn("description")
                    .AsString(1024)
                    .Nullable();

            this.Create.UniqueConstraint("UX_app_code")
                .OnTable("app").Column("code");

            // role
            this.Create.Table("role")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_role")
                .WithColumn("app_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_role_appId")
                    .ForeignKey("FK_role_app", "app", "id")
                .WithColumn("code")
                    .AsAnsiString(128)
                    .NotNullable()
                .WithColumn("name")
                    .AsString(128)
                    .NotNullable()
                .WithColumn("description")
                    .AsString(1024)
                    .Nullable();

            this.Create.UniqueConstraint("UX_role_appId_code")
                .OnTable("role")
                .Columns("app_id", "code");

            // user
            this.Create.Table("user")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_user")
                .WithColumn("username")
                    .AsAnsiString(128)
                    .NotNullable()
                    .Unique("UX_user_username")
                .WithColumn("full_name")
                    .AsString(128)
                    .NotNullable()
                .WithColumn("email")
                    .AsString(128)
                    .NotNullable()
                    .Unique("UX_user_email")
                .WithColumn("password_hash")
                    .AsAnsiString(128)
                    .NotNullable();

            // user_role
            this.Create.Table("user_role")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_userRole")
                .WithColumn("user_id")
                    .AsGuid()
                    .Nullable() // actually, should never be null, but nullable for sake of NHibernate. (HasMany functionality)
                    .Indexed("IX_userRole_userId")
                    .ForeignKey("FK_userRole_user", "user", "id")
                .WithColumn("role_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_userRole_roleId")
                    .ForeignKey("FK_userRole_role", "role", "id");

            this.Create.UniqueConstraint("UX_userRole_userId_roleId")
                .OnTable("user_role")
                .Columns("user_id", "role_id");

            this.Insert.IntoTable("user").Row(new
            {
                id = "560c87c7-efa8-4148-9287-8a9e6c363439",
                username = "admin",
                email = "admin@econera.com",
                full_name = "Super Administrator",
                password_hash = "_dummy_",
            });
        }
    }
}
