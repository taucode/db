CREATE TABLE `zeta`.`TaxInfo`(
    `Id` char(16) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
    `PersonId` bigint NOT NULL,
    `Tax` decimal(13, 4) NOT NULL,
    `Ratio` double NULL,
    `PersonMetaKey` smallint NOT NULL,
    `SmallRatio` float NOT NULL,
    `RecordDate` datetime NULL,
    `CreatedAt` datetime NOT NULL,
    `PersonOrdNumber` tinyint NOT NULL,
    `DueDate` datetime NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY(`Id`),
    CONSTRAINT `FK_taxInfo_Person` FOREIGN KEY(`PersonId`, `PersonMetaKey`, `PersonOrdNumber`) REFERENCES `zeta`.`person`(`Id`, `MetaKey`, `OrdNumber`))