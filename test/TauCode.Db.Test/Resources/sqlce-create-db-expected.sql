/* create table: client */
CREATE TABLE [client](
    [id] [bigint] NOT NULL IDENTITY(14, 88),
    [login] [nvarchar](251) NOT NULL,
    [email] [nvarchar](252) NOT NULL,
    [password_hash] [nvarchar](253) NOT NULL,
    [picture] [image] NULL,
    [birthday] [datetime] NULL,
    [gender] [bit] NOT NULL,
    [rating] [numeric](10, 4) NULL,
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
    CONSTRAINT [PK_client] PRIMARY KEY([id]))

/* create table: color */
CREATE TABLE [color](
    [id] [tinyint] NOT NULL,
    [name] [nvarchar](20) NOT NULL,
    CONSTRAINT [PK_color] PRIMARY KEY([id]))

/* create table: fragment_type */
CREATE TABLE [fragment_type](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_fragmentType] PRIMARY KEY([id]))

/* create unique index UX_fragmentType_code: fragment_type(code) */
CREATE UNIQUE INDEX [UX_fragmentType_code] ON [fragment_type]([code])

/* create table: language */
CREATE TABLE [language](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](2) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_language] PRIMARY KEY([id]))

/* create unique index UX_language_code: language(code) */
CREATE UNIQUE INDEX [UX_language_code] ON [language]([code])

/* create table: note */
CREATE TABLE [note](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [created] [datetime] NOT NULL,
    CONSTRAINT [PK_note] PRIMARY KEY([id]))

/* create unique index UX_note_code: note(code) */
CREATE UNIQUE INDEX [UX_note_code] ON [note]([code])

/* create table: secret */
CREATE TABLE [secret](
    [id_base] [int] NOT NULL,
    [id_value] [nchar](10) NOT NULL,
    [name] [nvarchar](20) NOT NULL,
    [key_start] [bigint] NOT NULL,
    [key_end] [smallint] NOT NULL,
    CONSTRAINT [PK_secret] PRIMARY KEY([id_base], [id_value]))

/* create index IX_secret_name: secret(name) */
CREATE INDEX [IX_secret_name] ON [secret]([name])

/* create unique index UX_secret_keyStart_keyEnd: secret(key_start, key_end) */
CREATE UNIQUE INDEX [UX_secret_keyStart_keyEnd] ON [secret]([key_start], [key_end])

/* create table: tag */
CREATE TABLE [tag](
    [id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_tag] PRIMARY KEY([id]))

/* create unique index UX_tag_code: tag(code) */
CREATE UNIQUE INDEX [UX_tag_code] ON [tag]([code])

/* create table: the_sample */
CREATE TABLE [the_sample](
    [id] [int] NOT NULL,
    [name] [nvarchar](100) NULL,
    [email] [nvarchar](100) NULL,
    [description] [nvarchar](100) NOT NULL,
    [color_id] [tinyint] NULL,
    CONSTRAINT [PK_secretDetail] PRIMARY KEY([id]))

/* create table: user */
CREATE TABLE [user](
    [id] [uniqueidentifier] NOT NULL,
    [login] [nvarchar](255) NOT NULL,
    [email] [nvarchar](255) NOT NULL,
    [password_hash] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_user] PRIMARY KEY([id]))

/* create unique index UX_user_email: user(email) */
CREATE UNIQUE INDEX [UX_user_email] ON [user]([email])

/* create unique index UX_user_login: user(login) */
CREATE UNIQUE INDEX [UX_user_login] ON [user]([login])

/* create table: fragment_sub_type */
CREATE TABLE [fragment_sub_type](
    [id] [uniqueidentifier] NOT NULL,
    [type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NOT NULL,
    [name] [nvarchar](255) NOT NULL,
    CONSTRAINT [PK_fragmentSubType] PRIMARY KEY([id]))

/* create unique index UX_fragmentSubType_typeId_code: fragment_sub_type(type_id, code) */
CREATE UNIQUE INDEX [UX_fragmentSubType_typeId_code] ON [fragment_sub_type]([type_id], [code])

/* create foreign key FK_fragmentSubType_fragmentType: fragment_sub_type(type_id) -> fragment_type(id) */
ALTER TABLE [fragment_sub_type] ADD CONSTRAINT [FK_fragmentSubType_fragmentType] FOREIGN KEY([type_id])
REFERENCES [fragment_type]([id])

/* create table: note_tag */
CREATE TABLE [note_tag](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [tag_id] [uniqueidentifier] NOT NULL,
    CONSTRAINT [PK_noteTag] PRIMARY KEY([id]))

/* create foreign key FK_noteTag_note: note_tag(note_id) -> note(id) */
ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_note] FOREIGN KEY([note_id])
REFERENCES [note]([id])

/* create foreign key FK_noteTag_tag: note_tag(tag_id) -> tag(id) */
ALTER TABLE [note_tag] ADD CONSTRAINT [FK_noteTag_tag] FOREIGN KEY([tag_id])
REFERENCES [tag]([id])

/* create table: note_translation */
CREATE TABLE [note_translation](
    [id] [uniqueidentifier] NOT NULL,
    [note_id] [uniqueidentifier] NOT NULL,
    [language_id] [uniqueidentifier] NOT NULL,
    [is_default] [bit] NOT NULL,
    [is_published] [bit] NOT NULL,
    [title] [nvarchar](255) NOT NULL,
    [last_updated] [datetime] NOT NULL,
    CONSTRAINT [PK_noteTranslation] PRIMARY KEY([id]))

/* create foreign key FK_noteTranslation_language: note_translation(language_id) -> language(id) */
ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_language] FOREIGN KEY([language_id])
REFERENCES [language]([id])

/* create foreign key FK_noteTranslation_note: note_translation(note_id) -> note(id) */
ALTER TABLE [note_translation] ADD CONSTRAINT [FK_noteTranslation_note] FOREIGN KEY([note_id])
REFERENCES [note]([id])

/* create table: secret_detail */
CREATE TABLE [secret_detail](
    [id] [int] NOT NULL,
    [secret_id_base] [int] NOT NULL,
    [secret_id_value] [nchar](10) NOT NULL,
    [description] [nvarchar](100) NOT NULL,
    [color_id] [tinyint] NULL,
    CONSTRAINT [PK_secretDetail] PRIMARY KEY([id]))

/* create index IX_secretDetail_secretIdBase_secretIdValue: secret_detail(secret_id_base, secret_id_value) */
CREATE INDEX [IX_secretDetail_secretIdBase_secretIdValue] ON [secret_detail]([secret_id_base], [secret_id_value])

/* create foreign key FK_secretDetail_color: secret_detail(color_id) -> color(id) */
ALTER TABLE [secret_detail] ADD CONSTRAINT [FK_secretDetail_color] FOREIGN KEY([color_id])
REFERENCES [color]([id])

/* create foreign key FK_secretDetail_secret: secret_detail(secret_id_base, secret_id_value) -> secret(id_base, id_value) */
ALTER TABLE [secret_detail] ADD CONSTRAINT [FK_secretDetail_secret] FOREIGN KEY([secret_id_base], [secret_id_value])
REFERENCES [secret]([id_base], [id_value])

/* create table: fragment */
CREATE TABLE [fragment](
    [id] [uniqueidentifier] NOT NULL,
    [note_translation_id] [uniqueidentifier] NOT NULL,
    [fragment_sub_type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NULL,
    [order] [int] NOT NULL,
    [content] [ntext] NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY([id]))

/* create foreign key FK_fragment_fragmentSubType: fragment(fragment_sub_type_id) -> fragment_sub_type(id) */
ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_fragmentSubType] FOREIGN KEY([fragment_sub_type_id])
REFERENCES [fragment_sub_type]([id])

/* create foreign key FK_fragment_noteTranslation: fragment(note_translation_id) -> note_translation(id) */
ALTER TABLE [fragment] ADD CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id])
REFERENCES [note_translation]([id])
