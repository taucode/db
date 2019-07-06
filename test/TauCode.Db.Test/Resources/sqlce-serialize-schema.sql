/* user - table */
CREATE TABLE [user](
    [id] int NOT NULL,
    [login] nvarchar(20) NOT NULL,
    [birthday] datetime NULL,
    [email] nvarchar(20) NULL,
    [password_hash] nvarchar(20) NULL,
    CONSTRAINT [PK_user] PRIMARY KEY ([id]));

/* user - unique index on [login] */
CREATE UNIQUE INDEX [UX_user_login] ON [user]([login]);

/* user - item */
CREATE TABLE [item](
    [id] int NOT NULL,    
    [user_id] int NOT NULL,
    [name] nvarchar(20) NOT NULL,
    [description] nvarchar(20) NOT NULL,
    [is_active] bit NULL,
    CONSTRAINT [PK_item] PRIMARY KEY ([id]));

/* fk */
ALTER TABLE [item] ADD CONSTRAINT FK_item_user FOREIGN KEY ([user_id])
REFERENCES [user]([id])