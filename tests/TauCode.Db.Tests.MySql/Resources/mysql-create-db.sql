/* create table: fragment_type */
CREATE TABLE `fragment_type`(
    `id` varchar(32) NOT NULL,
    `code` varchar(100) NOT NULL,
    `name` varchar(100) NOT NULL,
    `is_default` tinyint(1) NOT NULL,
    CONSTRAINT `PK_fragmentType` PRIMARY KEY(`id`));

/* create unique index UX_fragmentType_code: fragment_type(code) */
CREATE UNIQUE INDEX `UX_fragmentType_code` ON `fragment_type`(`code`);

/* create table: language */
CREATE TABLE `language`(
    `id` varchar(32) NOT NULL,
    `code` varchar(2) NOT NULL,
    `name` varchar(100) NOT NULL,
    CONSTRAINT `PK_language` PRIMARY KEY(`id`));

/* create unique index UX_language_code: language(code) */
CREATE UNIQUE INDEX `UX_language_code` ON `language`(`code`);

/* create table: note */
CREATE TABLE `note`(
    `id` varchar(32) NOT NULL,
    `code` varchar(100) NOT NULL,
    `created_on` datetime NOT NULL,
    CONSTRAINT `PK_note` PRIMARY KEY(`id`));

/* create unique index UX_note_code: note(code) */
CREATE UNIQUE INDEX `UX_note_code` ON `note`(`code`);

/* create unique index UX_note_created: note(created_on) */
CREATE UNIQUE INDEX `UX_note_created` ON `note`(`created_on`);

/* create table: tag */
CREATE TABLE `tag`(
    `id` varchar(32) NOT NULL,
    `code` varchar(100) NOT NULL,
    `name` varchar(100) NOT NULL,
    CONSTRAINT `PK_tag` PRIMARY KEY(`id`));

/* create unique index UX_tag_code: tag(code) */
CREATE UNIQUE INDEX `UX_tag_code` ON `tag`(`code`);

/* create table: user */
CREATE TABLE `user`(
    `id` varchar(32) NOT NULL,
    `username` varchar(100) NOT NULL,
    `email` varchar(100) NOT NULL,
    `password_hash` varchar(255) NOT NULL,
    CONSTRAINT `PK_user` PRIMARY KEY(`id`));

/* create unique index UX_user_email: user(email) */
CREATE UNIQUE INDEX `UX_user_email` ON `user`(`email`);

/* create unique index UX_user_username: user(username) */
CREATE UNIQUE INDEX `UX_user_username` ON `user`(`username`);

/* create table: VersionInfo */
CREATE TABLE `VersionInfo`(
    `Version` bigint NOT NULL,
    `AppliedOn` datetime NULL,
    `Description` varchar(1024) NULL);

/* create unique index UC_Version: VersionInfo(Version) */
CREATE UNIQUE INDEX `UC_Version` ON `VersionInfo`(`Version`);

/* create table: fragment_sub_type */
CREATE TABLE `fragment_sub_type`(
    `id` varchar(32) NOT NULL,
    `type_id` varchar(32) NOT NULL,
    `code` varchar(100) NOT NULL,
    `name` varchar(100) NOT NULL,
    `is_default` tinyint(1) NOT NULL,
    CONSTRAINT `PK_fragmentSubType` PRIMARY KEY(`id`));

/* create index IX_fragmentSubType_fragmentType: fragment_sub_type(type_id) */
CREATE INDEX `IX_fragmentSubType_fragmentType` ON `fragment_sub_type`(`type_id`);

/* create unique index UX_fragmentSubType_typeId_code: fragment_sub_type(type_id, code) */
CREATE UNIQUE INDEX `UX_fragmentSubType_typeId_code` ON `fragment_sub_type`(`type_id` DESC, `code` ASC);

/* create foreign key FK_fragmentSubType_fragmentType: fragment_sub_type(type_id) -> fragment_type(id) */
ALTER TABLE `fragment_sub_type` ADD CONSTRAINT `FK_fragmentSubType_fragmentType` FOREIGN KEY(`type_id`)
REFERENCES `fragment_type`(`id`);

/* create table: note_tag */
CREATE TABLE `note_tag`(
    `id` varchar(32) NOT NULL,
    `note_id` varchar(32) NULL,
    `tag_id` varchar(32) NOT NULL,
    `order` int NOT NULL,
    CONSTRAINT `PK_noteTag` PRIMARY KEY(`id`));

/* create index IX_noteTag_note: note_tag(note_id) */
CREATE INDEX `IX_noteTag_note` ON `note_tag`(`note_id`);

/* create index IX_noteTag_tag: note_tag(tag_id) */
CREATE INDEX `IX_noteTag_tag` ON `note_tag`(`tag_id`);

/* create foreign key FK_noteTag_note: note_tag(note_id) -> note(id) */
ALTER TABLE `note_tag` ADD CONSTRAINT `FK_noteTag_note` FOREIGN KEY(`note_id`)
REFERENCES `note`(`id`);

/* create foreign key FK_noteTag_tag: note_tag(tag_id) -> tag(id) */
ALTER TABLE `note_tag` ADD CONSTRAINT `FK_noteTag_tag` FOREIGN KEY(`tag_id`)
REFERENCES `tag`(`id`);

/* create table: note_translation */
CREATE TABLE `note_translation`(
    `id` varchar(32) NOT NULL,
    `note_id` varchar(32) NOT NULL,
    `language_id` varchar(32) NOT NULL,
    `is_default` tinyint(1) NOT NULL,
    `is_published` tinyint(1) NOT NULL,
    `title` varchar(1000) NOT NULL,
    `last_updated_on` datetime NOT NULL,
    CONSTRAINT `PK_noteTranslation` PRIMARY KEY(`id`));

/* create index IX_noteTranslation_language: note_translation(language_id) */
CREATE INDEX `IX_noteTranslation_language` ON `note_translation`(`language_id`);

/* create index IX_noteTranslation_note: note_translation(note_id) */
CREATE INDEX `IX_noteTranslation_note` ON `note_translation`(`note_id`);

/* create unique index UX_noteTranslation_noteId_languageId: note_translation(note_id, language_id) */
CREATE UNIQUE INDEX `UX_noteTranslation_noteId_languageId` ON `note_translation`(`note_id`, `language_id`);

/* create foreign key FK_noteTranslation_language: note_translation(language_id) -> language(id) */
ALTER TABLE `note_translation` ADD CONSTRAINT `FK_noteTranslation_language` FOREIGN KEY(`language_id`)
REFERENCES `language`(`id`);

/* create foreign key FK_noteTranslation_note: note_translation(note_id) -> note(id) */
ALTER TABLE `note_translation` ADD CONSTRAINT `FK_noteTranslation_note` FOREIGN KEY(`note_id`)
REFERENCES `note`(`id`);

/* create table: fragment */
CREATE TABLE `fragment`(
    `id` varchar(32) NOT NULL,
    `note_translation_id` varchar(32) NOT NULL,
    `sub_type_id` varchar(32) NOT NULL,
    `code` varchar(100) NOT NULL,
    `order` int NOT NULL,
    `content` text NOT NULL,
    CONSTRAINT `PK_fragment` PRIMARY KEY(`id`));

/* create index IX_fragment_fragmentSubType: fragment(sub_type_id) */
CREATE INDEX `IX_fragment_fragmentSubType` ON `fragment`(`sub_type_id`);

/* create unique index UX_fragment_noteTranslationId_code: fragment(note_translation_id, code) */
CREATE UNIQUE INDEX `UX_fragment_noteTranslationId_code` ON `fragment`(`note_translation_id`, `code`);

/* create unique index UX_fragment_noteTranslationId_order: fragment(note_translation_id, order) */
CREATE UNIQUE INDEX `UX_fragment_noteTranslationId_order` ON `fragment`(`note_translation_id`, `order`);

/* create foreign key FK_fragment_fragmentSubType: fragment(sub_type_id) -> fragment_sub_type(id) */
ALTER TABLE `fragment` ADD CONSTRAINT `FK_fragment_fragmentSubType` FOREIGN KEY(`sub_type_id`)
REFERENCES `fragment_sub_type`(`id`);

/* create foreign key FK_fragment_noteTranslation: fragment(note_translation_id) -> note_translation(id) */
ALTER TABLE `fragment` ADD CONSTRAINT `FK_fragment_noteTranslation` FOREIGN KEY(`note_translation_id`)
REFERENCES `note_translation`(`id`);