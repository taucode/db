/* user - table */
CREATE TABLE [user](
    [id] int NOT NULL,
    [login] nvarchar(30) NOT NULL,
    [email] nvarchar(30) NULL DEFAULT 'ak@deserea.net',
    [password_hash] nvarchar(30) NULL DEFAULT 'nohash',
    CONSTRAINT [PK_user] PRIMARY KEY ([id]));
