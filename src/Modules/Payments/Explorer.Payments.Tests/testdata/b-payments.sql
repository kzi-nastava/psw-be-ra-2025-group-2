-- Test Wallets
INSERT INTO payments."Wallets" ("Id", "TouristId", "Balance")
VALUES 
(-1, -21, 100),
(-2, -22, 0),
(-3, -23, 500);

-- Test Notifications
INSERT INTO payments."Notifications" ("Id", "TouristId", "Message", "CreatedAt", "IsRead", "Type")
VALUES
-- Tourist -21: ima 3 notifikacije (2 unread, 1 read)
(-1, -21, 'You received 50 Adventure Coins! Your wallet has been credited.', '2024-01-15 10:00:00', false, 0),
(-2, -21, 'You received 100 Adventure Coins! Your wallet has been credited.', '2024-01-14 09:00:00', true, 0),
(-3, -21, 'Welcome bonus: 25 Adventure Coins!', '2024-01-13 08:00:00', false, 0),

-- Tourist -22: ima 2 notifikacije (obe unread)
(-4, -22, 'You received 200 Adventure Coins! Your wallet has been credited.', '2024-01-12 07:00:00', false, 0),
(-5, -22, 'Special promotion: 75 Adventure Coins!', '2024-01-11 06:00:00', false, 1),

-- Tourist -23: ima 1 notifikaciju (read)
(-6, -23, 'You received 500 Adventure Coins! Your wallet has been credited.', '2024-01-10 05:00:00', true, 0);
