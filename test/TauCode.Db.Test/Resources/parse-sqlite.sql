/* sample 0, original script */
CREATE TABLE [user](
    `id` integer NOT NULL,
    "login" text NOT NULL,
    age numeric NOT NULL,
    [hash] blob NOT NULL,
    [amount] real NOT NULL)

/* sample 1, expected script */
CREATE TABLE [user](
    [id] integer NOT NULL,
    [login] text NOT NULL,
    [age] numeric NOT NULL,
    [hash] blob NOT NULL,
    [amount] real NOT NULL)