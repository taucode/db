using NUnit.Framework;
using TauCode.Db.Utils.Building;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Building
{
    [TestFixture]
    public class ScriptBuilderBaseTest
    {
        [Test]
        public void SplitSqlByComments_CorrectSql_SplitsToStatements()
        {
            // Arrange

            // Act
            var sql = this.GetType().Assembly.GetResourceText("sqlce-create-db.sql", true);
            var statements = ScriptBuilderBase
                .SplitSqlByComments(sql);

            // Assert
            Assert.That(statements, Has.Length.EqualTo(33));

            Assert.That(statements[0], Is.EqualTo(
@"CREATE TABLE [user](
    [id] [uniqueidentifier] NOT NULL,
    [login] [nvarchar](255) NOT NULL,
    [email] [nvarchar](255) NOT NULL,
    [password_hash] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_user] PRIMARY KEY ([id]))"));

            Assert.That(statements[1], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_user_login] ON [user]([login])"));

            Assert.That(statements[2], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_user_email] ON [user]([email])"));

            Assert.That(statements[3], Is.EqualTo(
@"CREATE TABLE [language](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](2) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_language] PRIMARY KEY ([id]))"));

            Assert.That(statements[4], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_language_code] ON [language] ([code])"));

            Assert.That(statements[5], Is.EqualTo(
@"CREATE TABLE [fragment_type](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_fragmentType] PRIMARY KEY ([id]))"));

            Assert.That(statements[6], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_fragmentType_code] ON [fragment_type] ([code])"));

            Assert.That(statements[7], Is.EqualTo(
@"CREATE TABLE [fragment_sub_type](
    [id] [uniqueidentifier] NOT NULL,
    [type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_fragmentSubType] PRIMARY KEY ([id]))"));

            Assert.That(statements[8], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_fragmentSubType_typeId_code] ON [fragment_sub_type] ([type_id], [code])"));

            Assert.That(statements[9], Is.EqualTo(
@"ALTER TABLE [fragment_sub_type] ADD CONSTRAINT [FK_fragmentSubType_fragmentType] FOREIGN KEY ([type_id])
REFERENCES [fragment_type] ([id])"));

            Assert.That(statements[10], Is.EqualTo(
@"CREATE TABLE [tag](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_tag] PRIMARY KEY ([id]))"));

            Assert.That(statements[11], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_tag_code] ON [tag]([code])"));

            Assert.That(statements[12], Is.EqualTo(
@"CREATE TABLE [note](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [created] [datetime] NOT NULL,
    CONSTRAINT [PK_note] PRIMARY KEY ([id]))"));

            Assert.That(statements[13], Is.EqualTo(
@"CREATE UNIQUE INDEX [UX_note_code] ON [note]([code])"));

            Assert.That(statements[14], Is.EqualTo(
@"CREATE TABLE [note_tag](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [tag_id] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_noteTag] PRIMARY KEY ([id]))"));

            Assert.That(statements[15], Is.EqualTo(
@"ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_note] FOREIGN KEY([note_id])
REFERENCES [note] ([id])"));

            Assert.That(statements[16], Is.EqualTo(
@"ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_tag] FOREIGN KEY([tag_id])
REFERENCES [tag] ([id])"));

            Assert.That(statements[17], Is.EqualTo(
@"CREATE TABLE [note_translation](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [language_id] [uniqueidentifier] NOT NULL,
    [is_default] [bit] NOT NULL,
    [is_published] [bit] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [last_updated] [datetime] NOT NULL,
    CONSTRAINT [PK_noteTranslation] PRIMARY KEY ([id]))"));

            Assert.That(statements[18], Is.EqualTo(
@"ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_language] FOREIGN KEY([language_id])
REFERENCES [language] ([id])"));

            Assert.That(statements[19], Is.EqualTo(
@"ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_note] FOREIGN KEY([note_id])
REFERENCES [note] ([id])"));

            Assert.That(statements[20], Is.EqualTo(
@"CREATE TABLE [fragment](
    [id] [uniqueidentifier] NOT NULL,
    [note_translation_id] [uniqueidentifier] NOT NULL,
    [fragment_sub_type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NULL,
    [order] [int] NOT NULL,
    [content] [ntext] NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY ([id]))"));

            Assert.That(statements[21], Is.EqualTo(
@"ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id])
REFERENCES [note_translation] ([id])"));

            Assert.That(statements[22], Is.EqualTo(
@"ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_fragmentSubType] FOREIGN KEY([fragment_sub_type_id])
REFERENCES [fragment_sub_type] ([id])"));

            Assert.That(statements[23], Is.EqualTo(
@"CREATE TABLE [client](
    [id] [bigint] NOT NULL IDENTITY(14, 88),
    [login] [nvarchar](251) NOT NULL,
    [email] [nvarchar](252) NOT NULL,
    [password_hash] [nvarchar](253) NOT NULL,
    [picture] [image] NULL,
    [birthday] [datetime] NULL,
    [gender] [bit] NOT NULL,
    [rating] [decimal] (10, 4) NULL,
    [fortune] [money] NOT NULL,
    [signature] [varbinary](254) NOT NULL,
    [visual_age] [tinyint] NULL,
    [alternate_rate] [numeric](6, 1) NULL,
    [iq_index] [float] NOT NULL,
    [index16] [smallint] NULL,
    [index32] [int] NOT NULL,
    [index64] [bigint] NULL,
    [the_real] [real] NOT NULL,
    [guid] [uniqueidentifier] NOT NULL,
    [unicode_text] [ntext] NULL,
    [fingerprint] [varbinary](8) NULL,

    CONSTRAINT [PK_client] PRIMARY KEY ([id]))"));

            Assert.That(statements[24], Is.EqualTo(
@"CREATE TABLE [secret](
    [id_base] int NOT NULL,
    [id_value] nchar(10) NOT NULL,
    [name] nvarchar(20) NOT NULL,
    [key_start] bigint NOT NULL,
    [key_end] smallint NOT NULL,

    CONSTRAINT [PK_secret] PRIMARY KEY ([id_base], [id_value]))"));

            Assert.That(statements[25], Is.EqualTo(
@"CREATE INDEX [IX_secret_name] ON [secret] ([name])"));

            Assert.That(statements[26], Is.EqualTo(
@"CREATE UNIQUE INDEX UX_secret_keyStart_keyEnd ON [secret] ([key_start], [key_end])"));

            Assert.That(statements[27], Is.EqualTo(
@"CREATE TABLE [color](
    [id] tinyint NOT NULL,
    [name] nvarchar(20) NOT NULL,

    CONSTRAINT [PK_color] PRIMARY KEY ([id]))"));

            Assert.That(statements[28], Is.EqualTo(
@"CREATE TABLE [secret_detail](
    [id] int NOT NULL,
    [secret_id_base] int NOT NULL,
    [secret_id_value] nchar(10) NOT NULL,
    [description] nvarchar(100) NOT NULL,
    [color_id] tinyint NULL,

    CONSTRAINT [PK_secretDetail] PRIMARY KEY ([id]))"));

            Assert.That(statements[29], Is.EqualTo(
@"CREATE INDEX [IX_secretDetail_secretIdBase_secretIdValue] ON [secret_detail] ([secret_id_base], [secret_id_value])"));

            Assert.That(statements[30], Is.EqualTo(
@"ALTER TABLE [secret_detail] ADD CONSTRAINT [FK_secretDetail_secret] FOREIGN KEY ([secret_id_base], [secret_id_value])
REFERENCES [secret] ([id_base], [id_value])"));

            Assert.That(statements[31], Is.EqualTo(
@"ALTER TABLE [secret_detail] ADD CONSTRAINT [FK_secretDetail_color] FOREIGN KEY ([color_id])
REFERENCES [color] ([id])"));

            Assert.That(statements[32], Is.EqualTo(
@"CREATE TABLE [the_sample](
    [id] int NOT NULL,
    [name] nvarchar(100) NULL,
    [email] nvarchar(100) NULL,
    [description] nvarchar(100) NOT NULL,
    [color_id] tinyint NULL,

    CONSTRAINT [PK_secretDetail] PRIMARY KEY ([id]))"));

        }

    }
}
