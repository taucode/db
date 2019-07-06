INSERT INTO [user]([id], [login], [birthday], [email], [password_hash]) VALUES(1, 'ak', '1978-07-05T14:11:12.371', 'ak@mail.net', '1234')
INSERT INTO [user]([id], [login], [birthday], [email], [password_hash]) VALUES(2, 'deserea', NULL, 'deserea@love.org', NULL)
INSERT INTO [user]([id], [login], [birthday], [email], [password_hash]) VALUES(3, 'olia', '1979-06-25T11:22:33.777', NULL, 'o@yahoo.com')

INSERT INTO [item]([id], [user_id], [name], [description], [is_active]) VALUES(1, 1, 'ud', '19.5', 1)
INSERT INTO [item]([id], [user_id], [name], [description], [is_active]) VALUES(2, 2, 'p', 'sweet', 1)