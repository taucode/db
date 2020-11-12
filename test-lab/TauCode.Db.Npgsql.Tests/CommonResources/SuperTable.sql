CREATE TABLE "zeta"."SuperTable"(
	"Id" serial NOT NULL PRIMARY KEY,

	"TheGuid" uuid NULL,

	"TheBit" boolean NULL,

	"TheTinyInt" smallint NULL,
	"TheSmallInt" smallint NULL,
	"TheInt" integer NULL,
	"TheBigInt" bigint NULL,

	"TheDecimal" decimal(10, 2) NULL,
	"TheNumeric" decimal(10, 2) NULL,

	"TheSmallMoney" money NULL,
	"TheMoney" money NULL,

	"TheReal" real NULL,
	"TheFloat" double precision NULL,

	"TheDate" timestamp without time zone NULL,
	"TheDateTime" timestamp without time zone NULL,
	"TheDateTime2" timestamp without time zone NULL,
	"TheDateTimeOffset" timestamp with time zone NULL,
	"TheSmallDateTime" timestamp without time zone NULL,
	"TheTime" time NULL,

	"TheChar" character(10) NULL,
	"TheVarChar" character varying(100) NULL,
	"TheVarCharMax" text NULL,

	"TheNChar" nchar(10) NULL,
	"TheNVarChar" character varying(100) NULL,
	"TheNVarCharMax" text NULL,

	"TheBinary" bytea NULL,
	"TheVarBinary" bytea NULL,
	"TheVarBinaryMax" bytea NULL)
