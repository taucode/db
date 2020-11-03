/*

DROP TABLE `zeta`.`NumericData`

DROP TABLE `zeta`.`DateData`

DROP TABLE `zeta`.`HealthInfo`

DROP TABLE `zeta`.`TaxInfo`

DROP TABLE `zeta`.`WorkInfo`

DROP TABLE `zeta`.`PersonData`

DROP TABLE `zeta`.`Person`

DROP SCHEMA `zeta`

CREATE SCHEMA `zeta`
GO

*/

/*** Person ***/
CREATE TABLE `zeta`.`Person`(
	`MetaKey` smallint NOT NULL,
	`OrdNumber` tinyint NOT NULL,
	`Id` bigint NOT NULL,
	`FirstName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
	`LastName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
	`Birthday` date NOT NULL,
	`Gender` boolean NULL,
	`Initials` char(2) CHARACTER SET utf8mb4 NULL,
	CONSTRAINT `PK_person` PRIMARY KEY(`Id`, `MetaKey`, `OrdNumber`)
)

/*** PersonData ***/
CREATE TABLE `zeta`.`PersonData`(
	`Id` char(16) binary NOT NULL,
	`Height` int NULL,
	`Photo` blob NULL,
	`EnglishDescription` text CHARACTER SET ascii NOT NULL,
	`UnicodeDescription` text CHARACTER SET utf8mb4 NOT NULL,
	`PersonMetaKey` smallint NOT NULL,
	`PersonOrdNumber` tinyint NOT NULL,
	`PersonId` bigint NOT NULL,
	CONSTRAINT `PK_personData` PRIMARY KEY (`Id`),
	CONSTRAINT `FK_personData_Person` FOREIGN KEY(`PersonId`, `PersonMetaKey`, `PersonOrdNumber`) REFERENCES `zeta`.`Person`(`Id`, `MetaKey`, `OrdNumber`)
)

/*** WorkInfo ***/
CREATE TABLE `zeta`.`WorkInfo`(
	`Id` char(16) binary NOT NULL,
	`Position` varchar(20) CHARACTER SET ascii NOT NULL,
	`HireDate` datetime NOT NULL,
	`Code` char(3) CHARACTER SET ascii NULL,
	`PersonMetaKey` smallint NOT NULL,
	`DigitalSignature` binary(16) NOT NULL,
	`PersonId` bigint NOT NULL,
	`PersonOrdNumber` tinyint NOT NULL,
	`Hash` char(16) binary NOT NULL,
	`Salary` decimal(13, 4) NULL,
	`VaryingSignature` varbinary(100) NULL,
	CONSTRAINT `PK_workInfo` PRIMARY KEY (`Id`),
	CONSTRAINT `FK_workInfo_Person` FOREIGN KEY(`PersonId`, `PersonMetaKey`, `PersonOrdNumber`) REFERENCES `zeta`.`Person`(`Id`, `MetaKey`, `OrdNumber`)
)

/*** WorkInfo - index on `Hash` ***/
CREATE UNIQUE INDEX `UX_workInfo_Hash` ON `zeta`.`WorkInfo`(`Hash`)

/*** TaxInfo ***/
CREATE TABLE `zeta`.`TaxInfo`(
	`Id` char(16) binary NOT NULL,
	`PersonId` bigint NOT NULL,
	`Tax` decimal(13, 4) NOT NULL,
	`Ratio` real NULL,
	`PersonMetaKey` smallint NOT NULL,
	`SmallRatio` float NOT NULL,
	`RecordDate` datetime NULL,
	`CreatedAt` datetime NOT NULL,
	`PersonOrdNumber` tinyint NOT NULL,
	`DueDate` datetime NULL,
	CONSTRAINT `PK_taxInfo` PRIMARY KEY(`Id`),
	CONSTRAINT `FK_taxInfo_Person` FOREIGN KEY(`PersonId`, `PersonMetaKey`, `PersonOrdNumber`) REFERENCES `zeta`.`Person`(`Id`, `MetaKey`, `OrdNumber`))

/*** HealthInfo ***/
CREATE TABLE `zeta`.`HealthInfo`(
	`Id` char(16) binary NOT NULL,
	`PersonId` bigint NOT NULL,
	`Weight` decimal(8, 2) NOT NULL,
	`PersonMetaKey` smallint NOT NULL,
	`IQ` numeric(8, 2) NULL,
	`Temper` smallint NULL,
	`PersonOrdNumber` tinyint NOT NULL,
	`MetricB` int NULL,
	`MetricA` int NULL,
	CONSTRAINT `PK_healthInfo` PRIMARY KEY (`Id`),
	CONSTRAINT `FK_healthInfo_Person` FOREIGN KEY(`PersonId`, `PersonMetaKey`, `PersonOrdNumber`) REFERENCES `zeta`.`Person`(`Id`, `MetaKey`, `OrdNumber`)
)

/*** HealthInfo - index on `MetricA`, `MetricB` ***/
CREATE INDEX `IX_healthInfo_metricAmetricB` ON `zeta`.`HealthInfo`(`MetricA` ASC, `MetricB` DESC)

/*** NumericData ***/
CREATE TABLE `zeta`.`NumericData`(
	`Id` int NOT NULL AUTO_INCREMENT,
	`BooleanData` bool NULL,
	`ByteData` tinyint NULL,
	`Int16` smallint NULL,
	`Int32` int NULL,
	`Int64` bigint NULL,
	`NetDouble` real NULL,
	`NetSingle` float NULL,
	`NumericData` numeric(10, 6) NULL,
	`DecimalData` decimal(11, 5) NULL,
	CONSTRAINT `PK_numericData` PRIMARY KEY (`Id`)
)

/*** DateData ***/
CREATE TABLE `zeta`.`DateData`(
	`Id` char(16) binary NOT NULL,
	`Moment` datetime NULL,
	CONSTRAINT `PK_dateData` PRIMARY KEY (`Id`)
)
