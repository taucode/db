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
    "DueDate" timestamp without time zone NULL)