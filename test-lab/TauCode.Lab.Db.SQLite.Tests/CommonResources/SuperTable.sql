CREATE TABLE [zeta].[SuperTable](
	[Id] integer NOT NULL PRIMARY KEY AUTOINCREMENT,
	[TheGuid] uniqueidentifier NULL,
	[TheBigInt] integer NULL,
	[TheDecimal] numeric NULL,
	[TheReal] real NULL,
	[TheDateTime] datetime NULL,
	[TheTime] time NULL,
	[TheChar] text NULL,
	[TheBinary] blob NULL)