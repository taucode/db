/* Table: 'Person' */
CREATE TABLE "zeta"."Person"(
    "Id" integer NOT NULL,
    "Tag" uuid NULL,
    "IsChecked" boolean NULL,
    "Birthday" timestamp without time zone NULL,
    "FirstName" character varying(100) NULL,
    "LastName" character varying(100) NULL,
    "Initials" character(2) NULL,
    "Gender" smallint NULL,
    CONSTRAINT "PK_person" PRIMARY KEY("Id"))

/* Index: 'UX_person_tag' on table 'Person' */
CREATE UNIQUE INDEX "UX_person_tag" ON "zeta"."Person"("Tag" ASC)

/* Table: 'PersonData' */
CREATE TABLE "zeta"."PersonData"(
    "Id" smallint NOT NULL,
    "PersonId" integer NOT NULL,
    "BestAge" smallint NULL,
    "Hash" bigint NULL,
    "Height" numeric(10, 2) NULL,
    "Weight" numeric(10, 2) NULL,
    "UpdatedAt" timestamp without time zone NULL,
    "Signature" bytea NULL,
    CONSTRAINT "PK_personData" PRIMARY KEY("Id"),
    CONSTRAINT "FK_personData_person" FOREIGN KEY("PersonId") REFERENCES "zeta"."Person"("Id"))

/* Table: 'WorkInfo' */
CREATE TABLE "zeta"."WorkInfo"(
    "Id" integer NOT NULL,
    "PersonId" integer NOT NULL,
    "PositionCode" character varying(100) NOT NULL,
    "PositionDescription" text NULL,
    "PositionDescriptionEn" text NULL,
    "HiredOn" timestamp without time zone NULL,
    "WorkStartDayTime" time without time zone NULL,
    "Salary" money NULL,
    "Bonus" money NULL,
    "OvertimeCoef" real NULL,
    "WeekendCoef" double precision NULL,
    "Url" character varying(200) NULL,
    CONSTRAINT "PK_workInfo" PRIMARY KEY("Id"),
    CONSTRAINT "FK_workInfo_person" FOREIGN KEY("PersonId") REFERENCES "zeta"."Person"("Id"))

/* Index: 'IX_workInfo_salary_bonus' on table 'WorkInfo' */
CREATE INDEX "IX_workInfo_salary_bonus" ON "zeta"."WorkInfo"("Salary" ASC, "Bonus" DESC)

/* Table: 'Photo' */
CREATE TABLE "zeta"."Photo"(
    "Id" character(4) NOT NULL,
    "PersonDataId" smallint NOT NULL,
    "Content" bytea NOT NULL,
    "ContentThumbnail" bytea NULL,
    "TakenAt" timestamp with time zone NULL,
    "ValidUntil" timestamp without time zone NULL,
    CONSTRAINT "PK_photo" PRIMARY KEY("Id"),
    CONSTRAINT "FK_photo_personData" FOREIGN KEY("PersonDataId") REFERENCES "zeta"."PersonData"("Id"))

