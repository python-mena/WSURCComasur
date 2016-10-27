--Create USERS table...
CREATE TABLE [dbo].[USERS](
	[User_Id] [int] IDENTITY(1,1) NOT NULL,	
	[Username] [nvarchar](30) NOT NULL,
	[LastModified] [datetime] NULL,
	[Inactive] [bit] NULL,	
	[Firstname] [nvarchar](50) NULL,
	[Lastname] [nvarchar](50) NULL,
	[Title] [nvarchar](30) NULL,
	[Initial] [nvarchar](3) NULL,
	[EMail] [nvarchar](100) NULL,
 CONSTRAINT [PK_USERS] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[USERS] ADD  CONSTRAINT [DF_USERS_Inactive]  DEFAULT ((0)) FOR [Inactive]
GO

--Create ROLES table...
CREATE TABLE [dbo].[ROLES](
	[Role_Id] [int] IDENTITY(1,1) NOT NULL,	
	[RoleName] [nvarchar](50) NOT NULL,
	[RoleDescription] [nvarchar](250) NULL,
	[IsSysAdmin] [bit] NULL,
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_Roles] PRIMARY KEY CLUSTERED 
(
	[Role_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ROLES] ADD  CONSTRAINT [DF_ROLES_IsSysAdmin]  DEFAULT ((0)) FOR [IsSysAdmin]
GO

ALTER TABLE [dbo].[ROLES] ADD  CONSTRAINT [DF_ROLES_LastModified]  DEFAULT ((GETDATE())) FOR [LastModified]
GO

--Create PERMISSIONS table...
CREATE TABLE [dbo].[PERMISSIONS](
	[Permission_Id] [int] IDENTITY(1,1) NOT NULL,
	[PermissionDescription] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PERMISSIONS] PRIMARY KEY CLUSTERED 
(
	[Permission_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

--Create LNK_USER_ROLE table...
CREATE TABLE [dbo].[LNK_USER_ROLE](
	[User_Id] [int] NOT NULL,
	[Role_Id] [int] NOT NULL,
 CONSTRAINT [PK_LNK_USER_ROLE] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC,
	[Role_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LNK_USER_ROLE]  WITH NOCHECK ADD  CONSTRAINT [FK_LNK_USER_ROLE_ROLES] FOREIGN KEY([Role_Id])
REFERENCES [dbo].[ROLES] ([Role_Id])
GO

ALTER TABLE [dbo].[LNK_USER_ROLE] CHECK CONSTRAINT [FK_LNK_USER_ROLE_ROLES]
GO

ALTER TABLE [dbo].[LNK_USER_ROLE]  WITH NOCHECK ADD  CONSTRAINT [FK_LNK_USER_ROLE_USERS] FOREIGN KEY([User_Id])
REFERENCES [dbo].[USERS] ([User_Id])
GO

ALTER TABLE [dbo].[LNK_USER_ROLE] CHECK CONSTRAINT [FK_LNK_USER_ROLE_USERS]
GO

--Create LNK_ROLE_PERMISSION table...
CREATE TABLE [dbo].[LNK_ROLE_PERMISSION](
	[Role_Id] [int] NOT NULL,
	[Permission_Id] [int] NOT NULL,
 CONSTRAINT [PK_LNK_ROLE_PERMISSION] PRIMARY KEY CLUSTERED 
(
	[Role_Id] ASC,
	[Permission_Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LNK_ROLE_PERMISSION]  WITH NOCHECK ADD  CONSTRAINT [FK_LNK_ROLE_PERMISSION_PERMISSIONS] FOREIGN KEY([Permission_Id])
REFERENCES [dbo].[PERMISSIONS] ([Permission_Id])
GO

ALTER TABLE [dbo].[LNK_ROLE_PERMISSION] CHECK CONSTRAINT [FK_LNK_ROLE_PERMISSION_PERMISSIONS]
GO

ALTER TABLE [dbo].[LNK_ROLE_PERMISSION]  WITH NOCHECK ADD  CONSTRAINT [FK_LNK_ROLE_PERMISSION_ROLES] FOREIGN KEY([Role_Id])
REFERENCES [dbo].[ROLES] ([Role_Id])
GO

ALTER TABLE [dbo].[LNK_ROLE_PERMISSION] CHECK CONSTRAINT [FK_LNK_ROLE_PERMISSION_ROLES]
GO


--Create an 'Administrator' Role setting IsSystemRole=1
INSERT INTO ROLES(RoleName, RoleDescription, IsSysAdmin) VALUES('Administrator', 'Administrator can access all areas of the application by default.', 1)

--Create a 'Standard User' Role setting IsSystemRole=0
INSERT INTO ROLES(RoleName, RoleDescription, IsSysAdmin) VALUES('Standard User', 'Users of the application should be granted this role who are not permitted to undertake administrator duties.  This role must have individual permissions assigned unlike the Administrator role.', 0)

--Create an Application Permission for the action method 'Create' 
--defined in the 'Admin' controller (ie 'admin-create')
INSERT INTO PERMISSIONS(PermissionDescription) VALUES('admin-create')

--Associate the Application Permission 'admin-create' with the
--'Administrator' Role
INSERT INTO LNK_ROLE_PERMISSION VALUES(
	(SELECT Role_Id FROM ROLES WHERE RoleName = 'Administrator'),
	(SELECT Permission_Id FROM PERMISSIONS WHERE PermissionDescription = 'admin-create'))

--Create the user 'swloch'
INSERT INTO USERS(Username, LastModified, Firstname, Lastname, Initial, EMail) VALUES('swloch', GETDATE(), 'Stefan', 'Wloch', 'S', 'name@somedomain.com')

--Associate the 'Administrator' Role with user
INSERT INTO LNK_USER_ROLE VALUES(
	(SELECT User_Id FROM USERS WHERE Username = 'swloch'),
	(SELECT Role_Id FROM ROLES WHERE RoleName = 'Administrator'))