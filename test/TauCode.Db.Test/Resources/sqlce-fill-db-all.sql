/* create some tags */
/* dotnet */
INSERT INTO [tag]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', 'dotnet', N'.NET')
/* ru */
INSERT INTO [tag]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', 'ruby', N'Ruby')
/* en */
INSERT INTO [tag]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', 'sql', N'SQL')
/* de */
INSERT INTO [tag]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', 'dotnetcore', N'.NET Core')

/* create some languages */
/* it */
INSERT INTO [language]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', 'it', N'Italian')
/* ru */
INSERT INTO [language]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', 'ru', N'Russian')
/* en */
INSERT INTO [language]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', 'en', N'English')
/* de */
INSERT INTO [language]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', 'de', N'German')

/* create some fragment types */
/* txt*/
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('207449b8-6f8c-43bc-86a3-000000000001', 'txt', N'Text')
/* code */
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('dee01610-0629-4085-858a-000000000002', 'code', N'Code')

/* create some fragment subtypes */
/* normal text*/
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('5e2e3b16-927e-4294-bd88-000000000003', '207449b8-6f8c-43bc-86a3-000000000001', 'normal-text', 'Normal Text');
/* c# */
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('36e3fd07-206c-454a-ab77-000000000001', 'dee01610-0629-4085-858a-000000000002', 'csharp', 'C#');
/* ruby */
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('8705922c-7b62-43b2-a5e5-000000000002', 'dee01610-0629-4085-858a-000000000002', 'ruby', 'Ruby');

/* create a note */
INSERT INTO [note]([id], [code], [created]) VALUES('b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4', 'hello-world', '2018-10-18 17:30:10')

/* create a note tag */
INSERT INTO [note_tag]([id], [note_id], [tag_id]) VALUES('76048ec5-c596-4e9e-9dff-ce49bff60510', 'b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4', 'ad4eb694-3d64-4320-b7f4-000000000003')

/* create a note translation */
INSERT INTO [note_translation](
    [id],
    [note_id],
    [language_id],
    [is_default],
    [is_published],
    [title],
    [last_updated])
VALUES(
    '4dfdec93-8c4d-46ff-8013-0cd99b5ab979',
    'b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4',
    'f4320164-ac05-44e1-92d1-000000000001',
    1,
    1,
    'Buonasera!',
    '2018-10-19 14:08:08')

/* create a fragment */
INSERT INTO [fragment](
    [id],
    [note_translation_id],
    [fragment_sub_type_id],
    [code],
    [order],
    [content])
VALUES(
    '99ab2c60-c11e-4e5e-8bf5-10d19ed320f6',
    '4dfdec93-8c4d-46ff-8013-0cd99b5ab979',
    '5e2e3b16-927e-4294-bd88-000000000003',
    'hello-block',
    0,
    'Hello folks!')