using FluentMigrator;

namespace TauCode.Db.Tests.Data.Rho
{
    [Migration(0)]
    public class Rho_M0_Baseline : AutoReversingMigration
    {
        public override void Up()
        {
            #region user

            this.Create.Table("user")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_user")
                .WithColumn("login")
                    .AsAnsiString(100)
                    .NotNullable()
                    .Unique("UX_user_login")
                .WithColumn("email")
                    .AsString(100)
                    .NotNullable()
                    .Unique("UX_user_email")
                .WithColumn("password_hash")
                    .AsAnsiString()
                    .NotNullable();

            #endregion

            #region language

            this.Create.Table("language")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_language")
                .WithColumn("code")
                    .AsAnsiString(2)
                    .NotNullable()
                    .Unique("UX_language_code")
                .WithColumn("name")
                    .AsString(100)
                    .NotNullable();

            #endregion

            #region fragment_type

            this.Create.Table("fragment_type")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_fragmentType")
                .WithColumn("code")
                    .AsAnsiString(100)
                    .NotNullable()
                    .Unique("UX_fragmentType_code")
                .WithColumn("name")
                    .AsString(100)
                    .NotNullable()
                .WithColumn("is_default")
                    .AsBoolean()
                    .NotNullable();

            #endregion

            #region fragment_sub_type

            this.Create.Table("fragment_sub_type")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_fragmentSubType")
                .WithColumn("type_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_fragmentSubType_fragmentType")
                .WithColumn("code")
                    .AsAnsiString(100)
                    .NotNullable()
                .WithColumn("name")
                    .AsString(100)
                    .NotNullable()
                .WithColumn("is_default")
                    .AsBoolean()
                    .NotNullable();
            
            this.Create.UniqueConstraint("UX_fragmentSubType_typeId_code")
                .OnTable("fragment_sub_type").Columns("type_id", "code");

            this.Create.ForeignKey("FK_fragmentSubType_fragmentType")
                .FromTable("fragment_sub_type").ForeignColumn("type_id")
                .ToTable("fragment_type").PrimaryColumn("id");

            #endregion

            #region tag

            this.Create.Table("tag")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_tag")
                .WithColumn("code")
                    .AsAnsiString(100)
                    .NotNullable()
                    .Unique("UX_tag_code")
                .WithColumn("name")
                    .AsString(100)
                    .NotNullable();

            #endregion

            #region note

            this.Create.Table("note")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_note")
                .WithColumn("code")
                    .AsAnsiString(100)
                    .NotNullable()
                    .Unique("UX_note_code")
                .WithColumn("created_on")
                    .AsDateTime()
                    .NotNullable()
                    .Unique("UX_note_created");

            #endregion

            #region note_tag

            this.Create.Table("note_tag")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_noteTag")
                .WithColumn("note_id")
                    .AsGuid()
                    .Nullable() // actually, never 'null' in commited state, but 'Nullable' is needed as work-around for NHibernate
                    .Indexed("IX_noteTag_note")
                .WithColumn("tag_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_noteTag_tag")
                .WithColumn("order")
                    .AsInt32()
                    .NotNullable();

            this.Create.ForeignKey("FK_noteTag_note")
                .FromTable("note_tag").ForeignColumn("note_id")
                .ToTable("note").PrimaryColumn("id");

            this.Create.ForeignKey("FK_noteTag_tag")
                .FromTable("note_tag").ForeignColumn("tag_id")
                .ToTable("tag").PrimaryColumn("id");

            #endregion

            #region note_translation

            this.Create.Table("note_translation")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_noteTranslation")
                .WithColumn("note_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_noteTranslation_note")
                .WithColumn("language_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_noteTranslation_language")
                .WithColumn("is_default")
                    .AsBoolean()
                    .NotNullable()
                .WithColumn("is_published")
                    .AsBoolean()
                    .NotNullable()
                .WithColumn("title")
                    .AsString(1000)
                    .NotNullable()
                .WithColumn("last_updated_on")
                    .AsDateTime()
                    .NotNullable();

            this.Create.ForeignKey("FK_noteTranslation_note")
                .FromTable("note_translation").ForeignColumn("note_id")
                .ToTable("note").PrimaryColumn("id");

            this.Create.ForeignKey("FK_noteTranslation_language")
                .FromTable("note_translation").ForeignColumn("language_id")
                .ToTable("language").PrimaryColumn("id");

            this.Create.UniqueConstraint("UX_noteTranslation_noteId_languageId")
                .OnTable("note_translation").Columns("note_id", "language_id");

            #endregion

            #region fragment

            this.Create.Table("fragment")
                .WithColumn("id")
                    .AsGuid()
                    .NotNullable()
                    .PrimaryKey("PK_fragment")
                .WithColumn("note_translation_id")
                    .AsGuid()
                    .NotNullable()
                .WithColumn("sub_type_id")
                    .AsGuid()
                    .NotNullable()
                    .Indexed("IX_fragment_fragmentSubType")
                .WithColumn("code")
                    .AsAnsiString(100)
                    .Nullable()
                .WithColumn("order")
                    .AsInt32()
                    .NotNullable()
                .WithColumn("content")
                    .AsString(int.MaxValue)
                    .NotNullable();

            this.Create.ForeignKey("FK_fragment_noteTranslation")
                .FromTable("fragment").ForeignColumn("note_translation_id")
                .ToTable("note_translation").PrimaryColumn("id");

            this.Create.ForeignKey("FK_fragment_fragmentSubType")
                .FromTable("fragment").ForeignColumn("sub_type_id")
                .ToTable("fragment_sub_type").PrimaryColumn("id");

            this.Create.UniqueConstraint("UX_fragment_noteTranslationId_code")
                .OnTable("fragment").Columns("note_translation_id", "code");

            this.Create.UniqueConstraint("UX_fragment_noteTranslationId_order")
                .OnTable("fragment").Columns("note_translation_id", "order");

            #endregion
        }
    }
}
