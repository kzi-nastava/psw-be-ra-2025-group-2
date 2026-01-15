INSERT INTO payments."Wallets" ("Id", "TouristId", "Balance")
VALUES 
(-1, -21, 100),
(-2, -22, 0),
(-3, -23, 500);

INSERT INTO payments."Notifications" ("Id", "TouristId", "Message", "CreatedAt", "IsRead", "Type")
VALUES
(-1, -21, 'You received 50 AC', '2024-01-15 10:00:00', false, 0),
(-2, -21, 'You received 100 AC', '2024-01-14 09:00:00', true, 0),
(-3, -21, 'Welcome bonus', '2024-01-13 08:00:00', false, 0),
(-4, -22, 'You received 200 AC', '2024-01-12 07:00:00', false, 0),
(-5, -22, 'Special promotion', '2024-01-11 06:00:00', false, 1),
(-6, -23, 'You received 500 AC', '2024-01-10 05:00:00', true, 0);