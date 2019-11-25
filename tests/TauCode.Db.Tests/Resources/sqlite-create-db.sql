/* user - table */
CREATE TABLE [user](
    [id] blob NOT NULL,
    [login] text NOT NULL,
    [email] text NOT NULL,
    [password_hash] text NOT NULL,
    CONSTRAINT [PK_user] PRIMARY KEY ([id]));

/* user - unique index on [login] */
CREATE UNIQUE INDEX [UX_user_login] ON [user]([login]);

/* user - unique index on [email] */
CREATE UNIQUE INDEX [UX_user_email] ON [user]([email]);

/* language - table */
CREATE TABLE [language](
    [id] blob NOT NULL,
    [code] text NOT NULL,
    [name] text NOT NULL,
    CONSTRAINT [PK_language] PRIMARY KEY ([id]))

/* language - unique index on [code] */
CREATE UNIQUE INDEX [UX_language_code] ON [language] ([code])

/* fragment_type - table */
CREATE TABLE [fragment_type](
    [id] blob NOT NULL,
    [code] text NOT NULL,
    [name] text NOT NULL,
    CONSTRAINT [PK_fragmentType] PRIMARY KEY ([id]))

/* fragment_type - unique index on [code] */
CREATE UNIQUE INDEX [UX_fragmentType_code] ON [fragment_type] ([code])

/* fragment_sub_type - table */
CREATE TABLE [fragment_sub_type](
    [id] blob NOT NULL,
    [type_id] blob NOT NULL,
    [code] text NOT NULL,
    [name] text NOT NULL,
    CONSTRAINT [PK_fragmentSubType] PRIMARY KEY ([id]),
    CONSTRAINT [FK_fragmentSubType_fragmentType] FOREIGN KEY ([type_id]) REFERENCES [fragment_type] ([id]))

/* fragment_sub_type - unique index on [type_id], [code] */
CREATE UNIQUE INDEX [UX_fragmentSubType_typeId_code] ON [fragment_sub_type] ([type_id], [code])

/* tag - table */
CREATE TABLE [tag](
    [id] blob NOT NULL,
    [code] text NOT NULL,
    [name] text NOT NULL,
    CONSTRAINT [PK_tag] PRIMARY KEY ([id]))

/* tag - unique index on [code] */
CREATE UNIQUE INDEX [UX_tag_code] ON [tag]([code])

/* note - table */
CREATE TABLE [note](
    [id] blob NOT NULL,
    [code] text NOT NULL,
    [created] text NOT NULL,
    CONSTRAINT [PK_note] PRIMARY KEY ([id]))

/* note - unique index on [code] */
CREATE UNIQUE INDEX [UX_note_code] ON [note]([code])

/* note_tag - table */
CREATE TABLE [note_tag](
    [id] blob NOT NULL,
    [note_id] blob NOT NULL,
    [tag_id] blob NOT NULL,
    CONSTRAINT [PK_noteTag] PRIMARY KEY ([id]),
    CONSTRAINT [FK_noteTag_note] FOREIGN KEY([note_id]) REFERENCES [note] ([id]),
    CONSTRAINT [FK_noteTag_tag] FOREIGN KEY([tag_id]) REFERENCES [tag] ([id]));

/* note_translation - table */
CREATE TABLE [note_translation](
    [id] blob NOT NULL,
    [note_id] blob NOT NULL,
    [language_id] blob NOT NULL,
    [is_default] integer NOT NULL,
    [is_published] integer NOT NULL,
    [title] text NOT NULL,
    [last_updated] integer NOT NULL,
    CONSTRAINT [PK_noteTranslation] PRIMARY KEY ([id]),
    CONSTRAINT [FK_noteTranslation_language] FOREIGN KEY([language_id]) REFERENCES [language] ([id]),
    CONSTRAINT [FK_noteTranslation_note] FOREIGN KEY([note_id]) REFERENCES [note] ([id]));

/* fragment - table */
CREATE TABLE [fragment](
    [id] blob NOT NULL,
    [note_translation_id] blob NOT NULL,
    [fragment_sub_type_id] blob NOT NULL,
    [code] text NULL,
    [order] integer NOT NULL,
    [content] text NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY ([id]),
    CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id]) REFERENCES [note_translation] ([id]),
    CONSTRAINT [FK_fragment_fragmentSubType] FOREIGN KEY([fragment_sub_type_id]) REFERENCES [fragment_sub_type] ([id]));

/* client - table */
CREATE TABLE [client](
    [id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
    [login] text NOT NULL,
    [email] text NOT NULL,
    [password_hash] text NOT NULL,
    [picture] blob NULL,
    [birthday] integer NULL,
    [gender] integer NOT NULL,
    [rating] numeric NULL,
    [fortune] numeric NOT NULL,
    [signature] blob NOT NULL,
    [visual_age] integer NULL,
    [alternate_rate] numeric NULL,
    [iq_index] real NOT NULL,
    [index16] integer NULL,
    [index32] integer NOT NULL,
    [index64] integer NULL,
    [the_real] real NOT NULL,
    [guid] blob NOT NULL,
    [ansi_char] text NULL,
    [ansi_varchar] text NULL,
    [ansi_text] text NULL,
    [unicode_text] text NULL,
    [fingerprint] blob NULL)

/* secret - table */
CREATE TABLE [secret](
    [id_base] integer NOT NULL,
    [id_value] text NOT NULL,
    [name] text NOT NULL,
    [key_start] integer NOT NULL,
    [key_end] integer NOT NULL,
    [client_id] integer NULL,

    CONSTRAINT [PK_secret] PRIMARY KEY ([id_base], [id_value]),
    CONSTRAINT [FK_client_id] FOREIGN KEY ([client_id]) REFERENCES [client] ([id]));

/* secret - index on name */
CREATE INDEX [IX_secret_name] ON [secret] ([name])

/* secret - unique index on key_start, key_end */
CREATE UNIQUE INDEX UX_secret_keyStart_keyEnd ON [secret] ([key_start], [key_end] DESC)

/* color - create table */
CREATE TABLE [color](
    [id] integer NOT NULL,
    [name] text NOT NULL,

    CONSTRAINT [PK_color] PRIMARY KEY ([id]))

/* secret_detail - table */
CREATE TABLE [secret_detail](
    [id] integer NOT NULL,
    [secret_id_base] integer NOT NULL,
    [secret_id_value] text NOT NULL,
    [description] text NOT NULL,
    [color_id] integer NULL,

    CONSTRAINT [PK_secretDetail] PRIMARY KEY ([id]),
    CONSTRAINT [FK_secretDetail_secret] FOREIGN KEY ([secret_id_base], [secret_id_value]) REFERENCES [secret] ([id_base], [id_value]),
    CONSTRAINT [FK_secretDetail_color] FOREIGN KEY ([color_id]) REFERENCES [color] ([id]))

/* secret_detail - index on secret_id_base, secret_id_value */
CREATE INDEX [IX_secretDetail_secretIdBase_secretIdValue] ON [secret_detail] ([secret_id_base], [secret_id_value])