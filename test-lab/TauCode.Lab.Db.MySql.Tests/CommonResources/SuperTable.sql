CREATE TABLE `zeta`.`SuperTable`(
	`TheInt` int NOT NULL PRIMARY KEY AUTO_INCREMENT,
	`TheIntUnsigned` int unsigned NULL,

	`TheBit` bit NULL,
	`TheBit9` bit (9) NULL,

	`TheTinyInt` tinyint NULL,
	`TheTinyIntUnsigned` tinyint unsigned NULL,

	`TheBool` bool NULL,
	`TheBoolean` boolean NULL,

	`TheSmallInt` smallint NULL,
	`TheSmallIntUnsigned` smallint unsigned NULL,
	
	`TheMediumInt` mediumint NULL,
	`TheMediumIntUnsigned` mediumint unsigned NULL,

	`TheBigInt` bigint NULL,
	`TheBigIntUnsigned` bigint unsigned NULL,

	`TheDecimal` decimal(8, 2) NULL,
	`TheNumeric` numeric(10, 3) NULL,

	`TheFloat` float NULL,
	`TheDouble` double NULL,

	`TheDate` date NULL,
	`TheDateTime` datetime NULL,
	`TheTimeStamp` timestamp NULL,
	`TheTime` time NULL,
	`TheYear` year NULL,

	`TheChar` char(100) CHARACTER SET ascii COLLATE ascii_general_ci NULL,
	`TheVarChar` varchar(100) CHARACTER SET utf8mb4 NULL,

	`TheBinary` binary(10) NULL,
	`TheVarBinary` varbinary(20) NULL,

	`TheTinyText` tinytext CHARACTER SET ascii COLLATE ascii_general_ci NULL,
	`TheText` text CHARACTER SET utf8mb4 NULL,
	`TheMediumText` mediumtext CHARACTER SET ascii COLLATE ascii_general_ci NULL,
	`TheLongText` longtext CHARACTER SET utf8mb4 NULL,

	`TheTinyBlob` tinyblob NULL,
	`TheBlob` blob NULL,
	`TheMediumBlob` mediumblob NULL,
	`TheLongBlob` longblob NULL)
