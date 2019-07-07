/* user - table */
CREATE TABLE [user](
    [id] integer NOT NULL,
    [login] text NOT NULL,
    [age] numeric NOT NULL,
    [hash] blob NOT NULL,
    [amount] real NOT NULL,
    CONSTRAINT [PK_user] PRIMARY KEY ([id]));
