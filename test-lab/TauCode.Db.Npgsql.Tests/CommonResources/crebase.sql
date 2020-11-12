/*

DROP TABLE "zeta"."NumericData"

DROP TABLE "zeta"."DateData"

DROP TABLE "zeta"."HealthInfo"

DROP TABLE "zeta"."TaxInfo"

DROP TABLE "zeta"."WorkInfo"

DROP TABLE "zeta"."PersonData"

DROP TABLE "zeta"."Person"

DROP SCHEMA "zeta"

CREATE SCHEMA "zeta"
GO

*/

/*** Person ***/
CREATE TABLE "zeta"."Person"(
	"MetaKey" smallint NOT NULL,
	"OrdNumber" smallint NOT NULL,
	"Id" bigint NOT NULL,
	"FirstName" character varying(100) NOT NULL,
	"LastName" character varying(100) NOT NULL,
	"Birthday" timestamp NOT NULL,
	"Gender" boolean NULL,
	"Initials" character(2) NULL,
	CONSTRAINT "PK_person" PRIMARY KEY("Id", "MetaKey", "OrdNumber")
)

/*** PersonData ***/
CREATE TABLE "zeta"."PersonData"(
	"Id" uuid NOT NULL,
	"Height" integer NULL,
	"Photo" bytea NULL,
	"EnglishDescription" text NOT NULL,
	"UnicodeDescription" text NOT NULL,
	"PersonMetaKey" smallint NOT NULL,
	"PersonOrdNumber" smallint NOT NULL,
	"PersonId" bigint NOT NULL,
	CONSTRAINT "PK_personData" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_personData_Person" FOREIGN KEY("PersonId", "PersonMetaKey", "PersonOrdNumber") REFERENCES "zeta"."Person"("Id", "MetaKey", "OrdNumber")
)

/*** WorkInfo ***/
CREATE TABLE "zeta"."WorkInfo"(
	"Id" uuid NOT NULL,
	"Position" "varchar"(20) NOT NULL,
	"HireDate" timestamp without time zone NOT NULL,
	"Code" character(3) NULL,
	"PersonMetaKey" smallint NOT NULL,
	"DigitalSignature" bytea NOT NULL,
	"PersonId" bigint NOT NULL,
	"PersonOrdNumber" smallint NOT NULL,
	"Hash" uuid NOT NULL,
	"Salary" money NULL,
	"VaryingSignature" bytea NULL,
	CONSTRAINT "PK_workInfo" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_workInfo_Person" FOREIGN KEY("PersonId", "PersonMetaKey", "PersonOrdNumber") REFERENCES "zeta"."Person"("Id", "MetaKey", "OrdNumber")
)

/*** WorkInfo - index on "Hash" ***/
CREATE UNIQUE INDEX "UX_workInfo_Hash" ON "zeta"."WorkInfo"("Hash")

/*** TaxInfo ***/
CREATE TABLE "zeta"."TaxInfo"(
	"Id" uuid NOT NULL,
	"PersonId" bigint NOT NULL,
	"Tax" money NOT NULL,
	"Ratio" double precision NULL,
	"PersonMetaKey" smallint NOT NULL,
	"SmallRatio" real NOT NULL,
	"RecordDate" timestamp without time zone NULL,
	"CreatedAt" timestamp with time zone NOT NULL,
	"PersonOrdNumber" smallint NOT NULL,
	"DueDate" timestamp without time zone  NULL,
	CONSTRAINT "PK_taxInfo" PRIMARY KEY("Id"),
	CONSTRAINT "FK_taxInfo_Person" FOREIGN KEY("PersonId", "PersonMetaKey", "PersonOrdNumber") REFERENCES "zeta"."Person"("Id", "MetaKey", "OrdNumber"))

/*** HealthInfo ***/
CREATE TABLE "zeta"."HealthInfo"(
	"Id" uuid NOT NULL,
	"PersonId" bigint NOT NULL,
	"Weight" decimal(8, 2) NOT NULL,
	"PersonMetaKey" smallint NOT NULL,
	"IQ" numeric(8, 2) NULL,
	"Temper" smallint NULL,
	"PersonOrdNumber" smallint NOT NULL,
	"MetricB" integer NULL,
	"MetricA" integer NULL,
	CONSTRAINT "PK_healthInfo" PRIMARY KEY ("Id"),
	CONSTRAINT "FK_healthInfo_Person" FOREIGN KEY("PersonId", "PersonMetaKey", "PersonOrdNumber") REFERENCES "zeta"."Person"("Id", "MetaKey", "OrdNumber")
)

/*** HealthInfo - index on "MetricA", "MetricB" ***/
CREATE INDEX "IX_healthInfo_metricAmetricB" ON "zeta"."HealthInfo"("MetricA" ASC, "MetricB" DESC)

/*** NumericData ***/
CREATE TABLE "zeta"."NumericData"(
	"Id" serial NOT NULL,
	"BooleanData" boolean NULL,
	"ByteData" smallint NULL,
	"Int16" smallint NULL,
	"Int32" integer NULL,
	"Int64" bigint NULL,
	"NetDouble" double precision NULL,
	"NetSingle" real NULL,
	"NumericData" numeric(10, 6) NULL,
	"DecimalData" numeric(11, 5) NULL,
	"TheSmallSerial" smallserial,
	"TheSerial" serial,
	"TheBigSerial" bigserial,
	"SmallIdentity" smallint GENERATED ALWAYS AS IDENTITY NULL,
	"IntIdentity" integer GENERATED ALWAYS AS IDENTITY NOT NULL,
	"BigIntIdentity" bigint GENERATED ALWAYS AS IDENTITY NOT NULL,
	CONSTRAINT "PK_numericData" PRIMARY KEY ("Id")
)

/*** DateData ***/
CREATE TABLE "zeta"."DateData"(
	"Id" uuid NOT NULL,
	"Moment" timestamp with time zone NULL,
	CONSTRAINT "PK_dateData" PRIMARY KEY ("Id")
)
