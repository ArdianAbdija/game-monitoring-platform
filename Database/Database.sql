/*
    Game Monitoring Platform database setup script.

    Put this file back into the project here:
    Database/Database.sql

    This script creates the SQL Server database structure used by the project
    and inserts sample data for testing and project defense.
*/

IF DB_ID('GameMonitoringPlatform') IS NULL
BEGIN
    CREATE DATABASE GameMonitoringPlatform;
END
GO

USE GameMonitoringPlatform;
GO

-- =========================
-- CLEAN OLD TABLES IF THEY ALREADY EXIST
-- This makes the script easier to rerun while testing.
-- =========================
IF OBJECT_ID('ReviewImage', 'U') IS NOT NULL DROP TABLE ReviewImage;
IF OBJECT_ID('Image', 'U') IS NOT NULL DROP TABLE Image;
IF OBJECT_ID('Review', 'U') IS NOT NULL DROP TABLE Review;
IF OBJECT_ID('GameGenre', 'U') IS NOT NULL DROP TABLE GameGenre;
IF OBJECT_ID('Game', 'U') IS NOT NULL DROP TABLE Game;
IF OBJECT_ID('Genre', 'U') IS NOT NULL DROP TABLE Genre;
IF OBJECT_ID('GameType', 'U') IS NOT NULL DROP TABLE GameType;
IF OBJECT_ID('Log', 'U') IS NOT NULL DROP TABLE [Log];
IF OBJECT_ID('[User]', 'U') IS NOT NULL DROP TABLE [User];
GO

-- =========================
-- GAME TYPE
-- Examples: Singleplayer, Multiplayer, Co-op.
-- One GameType can be connected to many games.
-- =========================
CREATE TABLE GameType (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- =========================
-- GENRE
-- Examples: Action, RPG, Strategy.
-- A game can have multiple genres through the GameGenre bridge table.
-- =========================
CREATE TABLE Genre (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- =========================
-- USER
-- Stores users who can log in and write reviews.
-- Role separates normal users from admins.
-- PasswordHash stores BCrypt hashes, not plain-text passwords.
-- =========================
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    FirstName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(50) NULL
);
GO

-- =========================
-- GAME
-- Main entity of the application.
-- Each game has one GameType.
-- =========================
CREATE TABLE Game (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL UNIQUE,
    Description NVARCHAR(MAX) NOT NULL,
    ReleaseDate DATE NOT NULL,
    Developer NVARCHAR(150) NOT NULL,
    GameTypeId INT NOT NULL,

    CONSTRAINT FK_Game_GameType
        FOREIGN KEY (GameTypeId)
        REFERENCES GameType(Id)
        ON DELETE NO ACTION
);
GO

-- =========================
-- GAME-GENRE BRIDGE TABLE
-- Handles the many-to-many relationship between Game and Genre.
-- =========================
CREATE TABLE GameGenre (
    GameId INT NOT NULL,
    GenreId INT NOT NULL,

    CONSTRAINT PK_GameGenre PRIMARY KEY (GameId, GenreId),

    CONSTRAINT FK_GameGenre_Game
        FOREIGN KEY (GameId)
        REFERENCES Game(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_GameGenre_Genre
        FOREIGN KEY (GenreId)
        REFERENCES Genre(Id)
        ON DELETE CASCADE
);
GO

-- =========================
-- REVIEW
-- Stores user ratings and review text.
-- Approved controls whether the review is visible publicly.
-- UQ_User_Game prevents the same user from reviewing the same game twice.
-- =========================
CREATE TABLE Review (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    GameId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 10),
    ReviewText NVARCHAR(MAX) NULL,
    Approved BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_Review_User
        FOREIGN KEY (UserId)
        REFERENCES [User](Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_Review_Game
        FOREIGN KEY (GameId)
        REFERENCES Game(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_User_Game UNIQUE (UserId, GameId)
);
GO

-- =========================
-- REVIEW IMAGE
-- Stores image paths uploaded together with reviews.
-- The file itself is stored in wwwroot/uploads/reviews.
-- The database stores only the URL/path.
-- =========================
CREATE TABLE ReviewImage (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReviewId INT NOT NULL,
    Url NVARCHAR(500) NOT NULL,

    CONSTRAINT FK_ReviewImage_Review
        FOREIGN KEY (ReviewId)
        REFERENCES Review(Id)
        ON DELETE CASCADE
);
GO

-- =========================
-- IMAGE
-- Optional image table connected directly to a game.
-- This was part of the original project structure.
-- =========================
CREATE TABLE Image (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    GameId INT NOT NULL,
    Url NVARCHAR(255) NOT NULL,

    CONSTRAINT FK_Image_Game
        FOREIGN KEY (GameId)
        REFERENCES Game(Id)
        ON DELETE CASCADE
);
GO

-- =========================
-- LOG
-- Stores application log messages created by the API.
-- =========================
CREATE TABLE [Log] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME NOT NULL DEFAULT GETDATE(),
    Level NVARCHAR(50) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL
);
GO

-- =========================
-- SAMPLE DATA
-- =========================

INSERT INTO GameType (Name) VALUES
('Singleplayer'),
('Multiplayer'),
('Co-op');
GO

INSERT INTO Genre (Name) VALUES
('Action'),
('RPG'),
('Strategy'),
('Adventure'),
('Shooter'),
('Sports');
GO

/*
    Seeded login accounts for testing:

    Admin:
    username: admin
    password: Admin123!

    User:
    username: user1
    password: User123!

    The values below are BCrypt hashes, because the application checks passwords
    with BCrypt.Net.BCrypt.Verify().
*/
INSERT INTO [User] (Username, Email, PasswordHash, Role, FirstName, LastName, PhoneNumber) VALUES
('admin', 'admin@example.com', '$2a$10$6rO39tM3T1g1drh7CKTjNOuPwmgpl7kFnYfX1d1B5yu75048Ozxk6', 'Admin', 'Admin', 'User', NULL),
('user1', 'user1@example.com', '$2a$10$hkgaiBrUU3etV7vH8QMEl.T0VDbxQ6RSLPpJ9inZCZjYh6KvUlBl2', 'User', 'Regular', 'User', NULL);
GO

INSERT INTO Game (Name, Description, ReleaseDate, Developer, GameTypeId) VALUES
('Elden Ring', 'Open-world fantasy action RPG with exploration, boss fights, and character progression.', '2022-02-25', 'FromSoftware', 1),
('Counter-Strike 2', 'Competitive tactical first-person shooter focused on team-based gameplay.', '2023-09-27', 'Valve', 2),
('The Witcher 3: Wild Hunt', 'Story-driven open-world RPG following Geralt of Rivia.', '2015-05-19', 'CD Projekt Red', 1),
('Baldur''s Gate 3', 'Party-based fantasy RPG with turn-based combat and branching decisions.', '2023-08-03', 'Larian Studios', 3),
('Football Manager 2024', 'Sports management simulation focused on tactics, transfers, and team development.', '2023-11-06', 'Sports Interactive', 1),
('Civilization VI', 'Turn-based strategy game where players build and expand a civilization through history.', '2016-10-21', 'Firaxis Games', 2);
GO

INSERT INTO GameGenre (GameId, GenreId) VALUES
(1, 1), -- Elden Ring -> Action
(1, 2), -- Elden Ring -> RPG
(1, 4), -- Elden Ring -> Adventure
(2, 1), -- Counter-Strike 2 -> Action
(2, 5), -- Counter-Strike 2 -> Shooter
(3, 2), -- The Witcher 3 -> RPG
(3, 4), -- The Witcher 3 -> Adventure
(4, 2), -- Baldur's Gate 3 -> RPG
(4, 3), -- Baldur's Gate 3 -> Strategy
(5, 6), -- Football Manager 2024 -> Sports
(5, 3), -- Football Manager 2024 -> Strategy
(6, 3); -- Civilization VI -> Strategy
GO

INSERT INTO Review (UserId, GameId, Rating, ReviewText, Approved, CreatedAt) VALUES
(2, 1, 9, 'Amazing game with strong exploration and boss design.', 1, GETDATE()),
(2, 2, 8, 'Very competitive and fun, especially with friends.', 1, GETDATE());
GO

-- External demo artwork; local user-uploaded files are not included in this repository.
INSERT INTO ReviewImage (ReviewId, Url) VALUES
(1, 'https://cdn.akamai.steamstatic.com/steam/apps/1245620/header.jpg');
GO

-- Optional game image URLs. These are useful if you later display game-level images.
INSERT INTO Image (GameId, Url) VALUES
(1, 'https://cdn.akamai.steamstatic.com/steam/apps/1245620/header.jpg'),
(2, 'https://cdn.akamai.steamstatic.com/steam/apps/730/header.jpg'),
(3, 'https://cdn.akamai.steamstatic.com/steam/apps/292030/header.jpg'),
(4, 'https://cdn.akamai.steamstatic.com/steam/apps/1086940/header.jpg'),
(5, 'https://cdn.akamai.steamstatic.com/steam/apps/2252570/header.jpg'),
(6, 'https://cdn.akamai.steamstatic.com/steam/apps/289070/header.jpg');
GO

INSERT INTO [Log] (Level, Message) VALUES
('Information', 'Database seeded successfully.'),
('Information', 'Sample games, users, reviews, and images were inserted.');
GO
