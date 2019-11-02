/* user - table */
CREATE TABLE [user](
    [id] integer NOT NULL PRIMARY KEY,
    [login] text NOT NULL,
    [email] text NULL DEFAULT 'ak@deserea.net',
    [password_hash] text NULL DEFAULT 'nohash');