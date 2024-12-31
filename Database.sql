CREATE DATABASE OnlineLibrary
GO
USE OnlineLibrary
GO

/*
Do not use database modifying (ALTER DATABASE), creating (CREATE DATABASE) or switching (USE) statements 
in this file.
*/

-- Primary
CREATE TABLE [dbo].[Book](
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
    [Title] [nvarchar](256) NOT NULL,
    [Author] [nvarchar](256) NOT NULL,
	[GenreId] [int] NOT NULL,
    [Description] [nvarchar](1024) NOT NULL,    
    [ISBN] [nvarchar](17) NOT NULL,
    [Availability] [nvarchar](50) NOT NULL,
	[ImageID] [int] NULL,
	[DeletedAt] [datetime2](7) NULL,
	CONSTRAINT [PK_Book] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

-- 1-to-N
CREATE TABLE [dbo].[Genre](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](256) NOT NULL,	
    CONSTRAINT [PK_Genre] PRIMARY KEY CLUSTERED 
    (
 		[Id] ASC
    )
)
GO

-- M-to-N
CREATE TABLE [dbo].[Location](
    [Id] [int] IDENTITY(1,1) NOT NULL, 
    [State] [nvarchar](256) NOT NULL,
    [City] [nvarchar](256) NOT NULL,
    [Address] [nvarchar](256) NOT NULL,    
    CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
    (
 		[Id] ASC
    )
)
GO

-- M-to-N-bridge
CREATE TABLE [dbo].[BookLocation](
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [BookID] [int] NOT NULL,
    [LocationID] [int] NOT NULL,
    CONSTRAINT [PK_BookLocation] PRIMARY KEY CLUSTERED
    (
		[Id] ASC
    )
)
GO

-- UserRole
CREATE TABLE [dbo].[UserRole] (
	[Id] [int] IDENTITY(1,1) NOT NULL, 
	[Name] [nvarchar](50) NOT NULL,
	CONSTRAINT [PK_UserRole] PRIMARY KEY (Id)
)
GO

-- User
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,	
	[Username] [nvarchar](50) NOT NULL,
	[PwdHash] [nvarchar](256) NOT NULL,
	[PwdSalt] [nvarchar](256) NOT NULL,
	[FirstName] [nvarchar](256) NOT NULL,
	[LastName] [nvarchar](256) NOT NULL,
	[Email] [nvarchar](256) NOT NULL,
	[Phone] [nvarchar](256) NULL,
	[RoleID] [int] NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

-- M-to-N 
CREATE TABLE [dbo].[Reservation](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BorrowDate] [datetime2](7) NOT NULL,    
    [ReturnDate] [datetime2](7) NULL,       	
	[BookLocationID] [int] NOT NULL,
    CONSTRAINT [PK_Reservation] PRIMARY KEY CLUSTERED 
    (
		[Id] ASC
    )
)
GO

-- User-M-to-N-bridge 
CREATE TABLE [dbo].[UserReservation](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
    [ReservationID] [int] NOT NULL,
    CONSTRAINT [PK_UserReservation] PRIMARY KEY CLUSTERED
    (
		[Id] ASC
    )
)
GO

-- Image
CREATE TABLE [dbo].[Image](
    [Id] [int] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,	
    CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
)
GO

-- Log
CREATE TABLE [dbo].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[Level] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
)
GO

-- Log
ALTER TABLE [dbo].[Log] ADD CONSTRAINT [CHK_Log_Level] CHECK ([Level] BETWEEN 1 AND 5)
GO

-- User
ALTER TABLE [dbo].[User] ADD CONSTRAINT [FK_User_UserRole] FOREIGN KEY (RoleId) 
REFERENCES [dbo].[UserRole] (Id)
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserRole]
GO

ALTER TABLE [dbo].[User] ADD CONSTRAINT [DF_User_RoleId] DEFAULT 2 FOR RoleId
GO

-- Book
ALTER TABLE [dbo].[Book] WITH CHECK ADD CONSTRAINT [FK_Book_Genre] FOREIGN KEY([GenreID])
REFERENCES [dbo].[Genre] ([Id])
GO
ALTER TABLE [dbo].[Book] CHECK CONSTRAINT [FK_Book_Genre]
GO

ALTER TABLE [dbo].[Book] WITH CHECK ADD CONSTRAINT [FK_Book_Image] FOREIGN KEY([ImageID])
REFERENCES [dbo].[Image] ([Id])
GO
ALTER TABLE [dbo].[Book] CHECK CONSTRAINT [FK_Book_Image]
GO

-- BookLocation
ALTER TABLE [dbo].[BookLocation] WITH CHECK ADD CONSTRAINT [FK_BookLocation_Book] FOREIGN KEY([BookID])
REFERENCES [dbo].[Book] ([Id])
GO
ALTER TABLE [dbo].[BookLocation] CHECK CONSTRAINT [FK_BookLocation_Book]
GO

ALTER TABLE [dbo].[BookLocation] WITH CHECK ADD CONSTRAINT [FK_BookLocation_Location] FOREIGN KEY([LocationID])
REFERENCES [dbo].[Location] ([Id])
GO
ALTER TABLE [dbo].[BookLocation] CHECK CONSTRAINT [FK_BookLocation_Location]
GO

-- Reservation
ALTER TABLE [dbo].[Reservation] WITH CHECK ADD CONSTRAINT [FK_Reservation_BookLocation] FOREIGN KEY([BookLocationID])
REFERENCES [dbo].[BookLocation] ([Id])
GO
ALTER TABLE [dbo].[Reservation] CHECK CONSTRAINT [FK_Reservation_BookLocation]
GO

-- UserReservation
ALTER TABLE [dbo].[UserReservation] WITH CHECK ADD CONSTRAINT [FK_UserReservation_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserReservation] CHECK CONSTRAINT [FK_UserReservation_User]
GO

ALTER TABLE [dbo].[UserReservation] WITH CHECK ADD CONSTRAINT [FK_UserReservation_Reservation] FOREIGN KEY([ReservationID])
REFERENCES [dbo].[Reservation] ([Id])
GO
ALTER TABLE [dbo].[UserReservation] CHECK CONSTRAINT [FK_UserReservation_Reservation]
GO
/* Use table data inserting, modifying, deleting and retrieving statements here */
INSERT INTO [dbo].[Genre] ([Name]) VALUES 
('Art'),
('Business'),
('History'),
('Law'),
('Mystery, Thriller & Crime Fiction'),
('Politics'),
('Romance'),
('Science'),
('Science Fiction'),
('Self-Improvement')
GO

INSERT INTO [dbo].[Location] ([State], [City], [Address]) VALUES 
('Hrvatska', 'Zagreb', 'Ilica 10'),
('Hrvatska', 'Split', 'Riva 16'),
('Hrvatska', 'Rijeka', 'Korzo 22'),
('Hrvatska', 'Osijek', 'Europske avenije 7'),
('Hrvatska', 'Zadar', 'Široka ulica 4'),
('Hrvatska', 'Pula', 'Forum 1'),
('Hrvatska', 'Dubrovnik', 'Stradun 3'),
('Hrvatska', 'Varaždin', 'Franjevaèki trg 9'),
('Hrvatska', 'Šibenik', 'Obala dr. Franje Tuðmana 5'),
('Hrvatska', 'Karlovac', 'Radiæeva 12')
GO

INSERT INTO [dbo].[Image] ([Content]) VALUES
('https://cdn.iconscout.com/icon/premium/png-512-thumb/no-image-2840056-2359564.png?f=webp&w=256')
GO

INSERT INTO [dbo].[Book] ([CreatedAt], [Title], [Author], [GenreId], [Description], [ISBN], [Availability], [ImageID], [DeletedAt]) VALUES 
('2024-11-09 14:30:45.123', 'The Art of War', 'Sun Tzu', 1, 'An ancient Chinese military treatise.', '978-158-648-619-7', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Thinking, Fast and Slow', 'Daniel Kahneman', 2, 'A book about the dual systems of thought.', '978-037-453-355-7', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Sapiens: A Brief History of Humankind', 'Yuval Noah Harari', 3, 'An exploration of humanity’s history.', '978-006-231-609-7', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'The 48 Laws of Power', 'Robert Greene', 4, 'A guide to gaining and maintaining power.', '978-014-303-994-2', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'The Da Vinci Code', 'Dan Brown', 5, 'A mystery thriller that explores secret societies.', '978-038-550-420-1', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Becoming', 'Michelle Obama', 6, 'A memoir by the former First Lady of the United States.', '978-152-476-313-8', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Pride and Prejudice', 'Jane Austen', 7, 'A romantic novel about love and social standing.', '978-019-953-556-9', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'The Science of Happiness', 'Dr. Stefan Klein', 8, 'An exploration of what makes us happy.', '978-000-000-000-8', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Dune', 'Frank Herbert', 9, 'A science fiction novel about politics and ecology on a desert planet.', '978-044-101-359-3', 'Available', 1, NULL),
('2024-11-09 14:30:45.123', 'Atomic Habits', 'James Clear', 10, 'A guide to building good habits and breaking bad ones.', '978-184-794-183-1', 'Available', 1, NULL)
GO

INSERT INTO [dbo].[BookLocation] ([BookID], [LocationID]) VALUES
(1,1),(1,2),(1,3),
(2,6),(2,7),(2,8),(2,9),(2,10),
(3,1),(3,2),(3,3),(3,4),(3,5),(3,6),(3,7),(3,8),(3,9),(3,10),
(4,1),(4,2),(4,3),(4,4),(4,5),(4,6),(4,7),(4,8),(4,9),(4,10),
(5,1),(5,2),(5,3),(5,4),(5,5),
(6,1),(6,2),(6,3),(6,4),(6,5),
(7,1),(7,2),(7,3),
(8,1),
(9,1),(9,2),(9,3),(9,4),
(10,5),(10,6),(10,7),(10,8)
GO

INSERT INTO [dbo].[Reservation] ([BorrowDate], [ReturnDate], [BookLocationID]) VALUES
('2024-10-01 10:00:00.1234567', '2024-10-15 10:00:00.1234567', 1), 
('2024-10-03 10:00:00.1234567', '2024-10-17 10:00:00.1234567', 6), 
('2024-10-05 10:00:00.1234567', '2024-10-20 10:00:00.1234567', 14), 
('2024-10-10 10:00:00.1234567', '2024-10-24 10:00:00.1234567', 39), 
('2024-10-12 10:00:00.1234567', NULL, 27)
GO

INSERT INTO [dbo].[UserRole] ([Name]) VALUES
('Admin'),
('User')
GO

INSERT INTO [dbo].[User] ([Username], [PwdHash], [PwdSalt], [FirstName], [LastName], [Email], [Phone], [RoleID]) VALUES
('admin', 'e/guJOH62Pv5WPE2T4/Qb38bkMoxx7xTXfs7p6GFb2w=', 'mHLo5lwiTwQspVDoIdvbxQ==', 'Admin', 'Admin', 'admin.admin@somewhere.com', '0987654321', 1),
('user1', 'fFh/G9IeUs++udV2dfwIMeKUQHnP2iU6pTuqTacsev0=', 'avtKOhbUhJB4IgjW7V9cHg==', 'User', 'One', 'user.one@somewhere.com', '0987654321', 2)
GO