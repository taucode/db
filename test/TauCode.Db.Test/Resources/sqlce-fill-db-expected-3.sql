/* fragment_type */
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('207449b8-6f8c-43bc-86a3-000000000001', N'txt', N'Text')
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('dee01610-0629-4085-858a-000000000002', N'code', N'Code')

/* language */
INSERT INTO [language]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', N'it', N'Italian')
INSERT INTO [language]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', N'ru', N'Russian')
INSERT INTO [language]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', N'en', N'English')
INSERT INTO [language]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', N'de', N'German')

/* note */
INSERT INTO [note]([id], [code], [created]) VALUES('b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4', N'hello-world', '2018-10-18 17:30:10')

/* tag */
INSERT INTO [tag]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', N'dotnet', N'.NET')
INSERT INTO [tag]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', N'ruby', N'Ruby')
INSERT INTO [tag]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', N'sql', N'SQL')
INSERT INTO [tag]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', N'dotnetcore', N'.NET Core')

/* the_sample */

/* user */

/* fragment_sub_type */
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('5e2e3b16-927e-4294-bd88-000000000003', '207449b8-6f8c-43bc-86a3-000000000001', N'normal-text', N'Normal Text')
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('36e3fd07-206c-454a-ab77-000000000001', 'dee01610-0629-4085-858a-000000000002', N'csharp', N'C#')
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('8705922c-7b62-43b2-a5e5-000000000002', 'dee01610-0629-4085-858a-000000000002', N'ruby', N'Ruby')

/* note_tag */
INSERT INTO [note_tag]([id], [note_id], [tag_id]) VALUES('76048ec5-c596-4e9e-9dff-ce49bff60510', 'b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4', 'ad4eb694-3d64-4320-b7f4-000000000003')

/* note_translation */
INSERT INTO [note_translation]([id], [note_id], [language_id], [is_default], [is_published], [title], [last_updated]) VALUES('4dfdec93-8c4d-46ff-8013-0cd99b5ab979', 'b7cf8a75-9a80-4d7d-bdbf-5e6f31702fc4', 'f4320164-ac05-44e1-92d1-000000000001', 1, 1, N'Buonasera!', '2018-10-19 14:08:08')

/* fragment */
INSERT INTO [fragment]([id], [note_translation_id], [fragment_sub_type_id], [code], [order], [content]) VALUES('99ab2c60-c11e-4e5e-8bf5-10d19ed320f6', '4dfdec93-8c4d-46ff-8013-0cd99b5ab979', '5e2e3b16-927e-4294-bd88-000000000003', N'hello-block', 0, N'Hello folks!')
