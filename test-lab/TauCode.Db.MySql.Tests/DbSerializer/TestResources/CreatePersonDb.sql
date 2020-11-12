/* Person; `Gender` is enum */
CREATE TABLE `zeta`.`Person`(
	`Id` int NOT NULL,
	`Tag` char(36) CHARACTER SET ascii COLLATE ascii_bin NULL,
	`IsChecked` bool NULL,
	`Birthday` datetime NULL,
	`FirstName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
	`LastName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
	`Initials` char(2) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
	`Gender` tinyint unsigned NULL, -- enum
	CONSTRAINT `PK_person` PRIMARY KEY(`Id`))

/* Index: 'UX_person_tag' on table 'person' */
CREATE UNIQUE INDEX `UX_person_tag` ON `zeta`.`Person`(`Tag`)

/* PersonData */
CREATE TABLE `zeta`.`PersonData`(
	`Id` smallint NOT NULL,
	`PersonId` int NOT NULL,
	`BestAge` tinyint NULL,
	`Hash` bigint NULL,
	`Height` decimal(10, 2) NULL,
	`Weight` numeric(10, 2) NULL,
	`UpdatedAt` datetime NULL,
	`Signature` binary(4) NULL,
	CONSTRAINT `PK_personData` PRIMARY KEY(`Id`),
	CONSTRAINT `FK_personData_person` FOREIGN KEY(`PersonId`) REFERENCES `zeta`.`Person`(`Id`))

/* Photo */
CREATE TABLE `zeta`.`Photo`(
	`Id` char(4) CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
	`PersonDataId` smallint NOT NULL,
	`Content` blob NOT NULL,
	`ContentThumbnail` varbinary(4000) NULL,
	`TakenAt` datetime NULL,
	`ValidUntil` date NULL,
	CONSTRAINT `PK_photo` PRIMARY KEY(`Id`),
	CONSTRAINT `FK_photo_personData` FOREIGN KEY(`PersonDataId`) REFERENCES `zeta`.`PersonData`(`Id`))

/* WorkInfo; `PositionCode` is enum */
CREATE TABLE `zeta`.`WorkInfo`(
	`Id` int NOT NULL,
	`PersonId` int NOT NULL,
	`PositionCode` varchar(100) CHARACTER SET ascii NULL,
	`PositionDescription` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
	`PositionDescriptionEn` text CHARACTER SET ascii NULL,
	`HiredOn` datetime NULL,
	`WorkStartDayTime` time NULL,
	`Salary` decimal(10, 2) NULL,
	`Bonus` decimal(10, 2) NULL,
	`OvertimeCoef` real NULL,
	`WeekendCoef` float NULL,
	`Url` varchar(100) CHARACTER SET ascii NULL,
	CONSTRAINT `PK_workInfo` PRIMARY KEY(`Id`),
	CONSTRAINT `FK_workInfo_person` FOREIGN KEY(`PersonId`) REFERENCES `zeta`.`Person`(`Id`))

/* Index on salary, bonus */
CREATE INDEX `IX_workInfo_salary_bonus` ON `zeta`.`WorkInfo`(`Salary` ASC, `Bonus` DESC)