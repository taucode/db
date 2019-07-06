/* fragment_type */
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('207449b8-6f8c-43bc-86a3-000000000001', N'txt', N'Text')
INSERT INTO [fragment_type]([id], [code], [name]) VALUES('dee01610-0629-4085-858a-000000000002', N'code', N'Code')

/* language */
INSERT INTO [language]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', N'it', N'Italian')
INSERT INTO [language]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', N'ru', N'Russian')
INSERT INTO [language]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', N'en', N'English')
INSERT INTO [language]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', N'de', N'German')

/* tag */
INSERT INTO [tag]([id], [code], [name]) VALUES('f4320164-ac05-44e1-92d1-000000000001', N'dotnet', N'.NET')
INSERT INTO [tag]([id], [code], [name]) VALUES('a38cadd9-0d0c-47b3-90fc-000000000002', N'ruby', N'Ruby')
INSERT INTO [tag]([id], [code], [name]) VALUES('ad4eb694-3d64-4320-b7f4-000000000003', N'sql', N'SQL')
INSERT INTO [tag]([id], [code], [name]) VALUES('ca427f3d-fb28-4a3c-901f-000000000004', N'dotnetcore', N'.NET Core')

/* fragment_sub_type */
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('5e2e3b16-927e-4294-bd88-000000000003', '207449b8-6f8c-43bc-86a3-000000000001', N'normal-text', N'Normal Text')
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('36e3fd07-206c-454a-ab77-000000000001', 'dee01610-0629-4085-858a-000000000002', N'csharp', N'C#')
INSERT INTO [fragment_sub_type]([id], [type_id], [code], [name]) VALUES('8705922c-7b62-43b2-a5e5-000000000002', 'dee01610-0629-4085-858a-000000000002', N'ruby', N'Ruby')
