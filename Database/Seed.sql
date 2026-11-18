USE GameMonitoringPlatform;
GO

-- Remove current wrong review-image links.
-- This removes the God of War image wrongly connected to Elden Ring.
DELETE FROM ReviewImage;
GO

-- Add missing game types.
INSERT INTO GameType (Name)
SELECT 'Singleplayer' WHERE NOT EXISTS (SELECT 1 FROM GameType WHERE Name = 'Singleplayer');

INSERT INTO GameType (Name)
SELECT 'Multiplayer' WHERE NOT EXISTS (SELECT 1 FROM GameType WHERE Name = 'Multiplayer');

INSERT INTO GameType (Name)
SELECT 'Co-op' WHERE NOT EXISTS (SELECT 1 FROM GameType WHERE Name = 'Co-op');
GO

-- Add missing genres.
INSERT INTO Genre (Name)
SELECT v.Name
FROM (VALUES
    ('Action'),
    ('RPG'),
    ('Strategy'),
    ('Adventure'),
    ('Shooter'),
    ('Sports'),
    ('Racing'),
    ('Simulation')
) v(Name)
WHERE NOT EXISTS (
    SELECT 1 FROM Genre g WHERE g.Name = v.Name
);
GO

-- Add extra demo users so several reviews can exist.
-- PasswordHash is reused for testing/demo users.
INSERT INTO [User] (Username, Email, PasswordHash, Role, FirstName, LastName, PhoneNumber)
SELECT v.Username, v.Email, v.PasswordHash, v.Role, v.FirstName, v.LastName, v.PhoneNumber
FROM (VALUES
    ('user2', 'user2@example.com', '$2a$10$hkgaiBrUU3etV7vH8QMEl.T0VDbxQ6RSLPpJ9inZCZjYh6KvUlBl2', 'User', 'Demo', 'User Two', NULL),
    ('user3', 'user3@example.com', '$2a$10$hkgaiBrUU3etV7vH8QMEl.T0VDbxQ6RSLPpJ9inZCZjYh6KvUlBl2', 'User', 'Demo', 'User Three', NULL),
    ('user4', 'user4@example.com', '$2a$10$hkgaiBrUU3etV7vH8QMEl.T0VDbxQ6RSLPpJ9inZCZjYh6KvUlBl2', 'User', 'Demo', 'User Four', NULL)
) v(Username, Email, PasswordHash, Role, FirstName, LastName, PhoneNumber)
WHERE NOT EXISTS (
    SELECT 1 FROM [User] u WHERE u.Username = v.Username
);
GO

-- Add more games without deleting existing ones.
INSERT INTO Game (Name, Description, ReleaseDate, Developer, GameTypeId)
SELECT v.Name, v.Description, v.ReleaseDate, v.Developer, gt.Id
FROM (VALUES
    ('Cyberpunk 2077', 'Open-world sci-fi RPG set in a dystopian futuristic city.', '2020-12-10', 'CD Projekt Red', 'Singleplayer'),
    ('Forza Horizon 5', 'Open-world racing game set in a large Mexico-inspired map.', '2021-11-09', 'Playground Games', 'Multiplayer'),
    ('God of War', 'Story-driven action adventure following Kratos and Atreus.', '2022-01-14', 'Santa Monica Studio', 'Singleplayer'),
    ('Red Dead Redemption 2', 'Open-world western action adventure with strong storytelling.', '2019-12-05', 'Rockstar Games', 'Singleplayer'),
    ('Grand Theft Auto V', 'Open-world action game with singleplayer and online multiplayer modes.', '2015-04-14', 'Rockstar Games', 'Multiplayer'),
    ('Hades', 'Roguelike action game based on Greek mythology.', '2020-09-17', 'Supergiant Games', 'Singleplayer'),
    ('Portal 2', 'Puzzle-platform game built around portals and physics-based challenges.', '2011-04-18', 'Valve', 'Co-op')
) v(Name, Description, ReleaseDate, Developer, GameTypeName)
JOIN GameType gt ON gt.Name = v.GameTypeName
WHERE NOT EXISTS (
    SELECT 1 FROM Game g WHERE g.Name = v.Name
);
GO

-- Add genre connections.
INSERT INTO GameGenre (GameId, GenreId)
SELECT g.Id, ge.Id
FROM (VALUES
    ('Elden Ring', 'Action'),
    ('Elden Ring', 'RPG'),
    ('Elden Ring', 'Adventure'),

    ('Counter-Strike 2', 'Action'),
    ('Counter-Strike 2', 'Shooter'),

    ('The Witcher 3: Wild Hunt', 'RPG'),
    ('The Witcher 3: Wild Hunt', 'Adventure'),

    ('Baldur''s Gate 3', 'RPG'),
    ('Baldur''s Gate 3', 'Strategy'),

    ('Football Manager 2024', 'Strategy'),
    ('Football Manager 2024', 'Sports'),

    ('Civilization VI', 'Strategy'),

    ('Cyberpunk 2077', 'Action'),
    ('Cyberpunk 2077', 'RPG'),

    ('Forza Horizon 5', 'Racing'),
    ('Forza Horizon 5', 'Sports'),

    ('God of War', 'Action'),
    ('God of War', 'Adventure'),

    ('Red Dead Redemption 2', 'Action'),
    ('Red Dead Redemption 2', 'Adventure'),

    ('Grand Theft Auto V', 'Action'),
    ('Grand Theft Auto V', 'Adventure'),

    ('Hades', 'Action'),
    ('Hades', 'RPG'),

    ('Portal 2', 'Strategy'),
    ('Portal 2', 'Adventure')
) v(GameName, GenreName)
JOIN Game g ON g.Name = v.GameName
JOIN Genre ge ON ge.Name = v.GenreName
WHERE NOT EXISTS (
    SELECT 1
    FROM GameGenre gg
    WHERE gg.GameId = g.Id AND gg.GenreId = ge.Id
);
GO

-- Add reviews without violating the one-user-one-review-per-game rule.
INSERT INTO Review (UserId, GameId, Rating, ReviewText, Approved, CreatedAt)
SELECT u.Id, g.Id, v.Rating, v.ReviewText, 1, GETDATE()
FROM (VALUES
    ('user2', 'Elden Ring', 10, 'Beautiful world design and very rewarding combat.'),
    ('user3', 'Counter-Strike 2', 8, 'Great competitive gameplay, but very intense for new players.'),
    ('user2', 'The Witcher 3: Wild Hunt', 9, 'Excellent story, characters, and open-world quests.'),
    ('user3', 'Baldur''s Gate 3', 10, 'Very detailed RPG with meaningful decisions.'),
    ('user2', 'Football Manager 2024', 8, 'Good management simulation with lots of tactical depth.'),
    ('user3', 'Civilization VI', 9, 'Very addictive strategy game with strong replay value.'),
    ('user2', 'Cyberpunk 2077', 8, 'Much better after updates, with great atmosphere.'),
    ('user3', 'Forza Horizon 5', 9, 'Beautiful racing game with smooth driving.'),
    ('user4', 'God of War', 9, 'Strong story and very polished combat.'),
    ('user2', 'Red Dead Redemption 2', 10, 'Amazing world, realism, and storytelling.'),
    ('user3', 'Grand Theft Auto V', 8, 'Still fun and full of content.'),
    ('user4', 'Hades', 9, 'Fast, stylish, and very replayable.'),
    ('user2', 'Portal 2', 10, 'Smart puzzles and excellent co-op design.')
) v(Username, GameName, Rating, ReviewText)
JOIN [User] u ON u.Username = v.Username
JOIN Game g ON g.Name = v.GameName
WHERE NOT EXISTS (
    SELECT 1
    FROM Review r
    WHERE r.UserId = u.Id AND r.GameId = g.Id
);
GO

-- Add correct review images.
-- These are external Steam header images, so they should display without needing local files.
INSERT INTO ReviewImage (ReviewId, Url)
SELECT r.Id, v.Url
FROM (VALUES
    ('Elden Ring', 'https://cdn.akamai.steamstatic.com/steam/apps/1245620/header.jpg'),
    ('Counter-Strike 2', 'https://cdn.akamai.steamstatic.com/steam/apps/730/header.jpg'),
    ('The Witcher 3: Wild Hunt', 'https://cdn.akamai.steamstatic.com/steam/apps/292030/header.jpg'),
    ('Baldur''s Gate 3', 'https://cdn.akamai.steamstatic.com/steam/apps/1086940/header.jpg'),
    ('Football Manager 2024', 'https://cdn.akamai.steamstatic.com/steam/apps/2252570/header.jpg'),
    ('Civilization VI', 'https://cdn.akamai.steamstatic.com/steam/apps/289070/header.jpg'),
    ('Cyberpunk 2077', 'https://cdn.akamai.steamstatic.com/steam/apps/1091500/header.jpg'),
    ('Forza Horizon 5', 'https://cdn.akamai.steamstatic.com/steam/apps/1551360/header.jpg'),
    ('God of War', 'https://cdn.akamai.steamstatic.com/steam/apps/1593500/header.jpg'),
    ('Red Dead Redemption 2', 'https://cdn.akamai.steamstatic.com/steam/apps/1174180/header.jpg'),
    ('Grand Theft Auto V', 'https://cdn.akamai.steamstatic.com/steam/apps/271590/header.jpg'),
    ('Hades', 'https://cdn.akamai.steamstatic.com/steam/apps/1145360/header.jpg'),
    ('Portal 2', 'https://cdn.akamai.steamstatic.com/steam/apps/620/header.jpg')
) v(GameName, Url)
JOIN Game g ON g.Name = v.GameName
CROSS APPLY (
    SELECT TOP 1 r.Id
    FROM Review r
    WHERE r.GameId = g.Id AND r.Approved = 1
    ORDER BY r.Id
) r
WHERE NOT EXISTS (
    SELECT 1
    FROM ReviewImage ri
    WHERE ri.ReviewId = r.Id AND ri.Url = v.Url
);
GO