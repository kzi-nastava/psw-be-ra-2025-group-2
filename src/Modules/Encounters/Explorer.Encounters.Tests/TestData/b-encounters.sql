INSERT INTO encounters."Encounters"
("Id", "Name", "Description", "Latitude", "Longitude", "XP", "State", "Type")
VALUES
-- ===== ACTIVE (10) =====
(-1, 'Social Challenge 1', 'Talk to five locals.', 45.2671, 19.8335, 100, 1, 0),
(-2, 'Location Visit 1', 'Visit the old fortress.', 44.7866, 20.4489, 120, 1, 1),
(-3, 'Misc Task 1', 'Solve a local riddle.', 43.3209, 21.8958, 80, 1, 2),
(-4, 'Social Challenge 2', 'Help a traveler.', 46.1005, 19.6656, 90, 1, 0),
(-5, 'Location Visit 2', 'Reach the mountain peak.', 43.7234, 20.6871, 150, 1, 1),
(-6, 'Misc Task 2', 'Find a hidden symbol.', 44.0165, 21.0059, 110, 1, 2),
(-7, 'Social Challenge 3', 'Join a local event.', 45.8150, 15.9819, 95, 1, 0),
(-8, 'Location Visit 3', 'Explore the cave.', 44.3896, 19.1025, 140, 1, 1),
(-9, 'Misc Task 3', 'Decode the message.', 45.2517, 19.8369, 105, 1, 2),
(-10, 'Social Challenge 4', 'Guide tourists.', 44.8125, 20.4612, 130, 1, 0),

-- ===== ARCHIVED (5) =====
(-11, 'Location Visit 4', 'Cross the old bridge.', 43.1374, 20.5122, 70, 2, 1),
(-12, 'Misc Task 4', 'Collect ancient coins.', 44.6543, 20.2006, 85, 2, 2),
(-13, 'Social Challenge 5', 'Interview a historian.', 45.7740, 19.1126, 90, 2, 0),
(-14, 'Location Visit 5', 'Map the ruins.', 44.9850, 20.1589, 110, 2, 1),
(-15, 'Social Challenge 6', 'Organize a meetup.', 43.8914, 20.3497, 100, 2, 0),

-- ===== DRAFT (5) =====
(-16, 'Misc Task 5', 'Test a prototype challenge.', 45.4064, 20.0522, 60, 0, 2),
(-17, 'Location Visit 6', 'Scout a new trail.', 44.0592, 20.9100, 120, 0, 1),
(-18, 'Social Challenge 7', 'Prepare community task.', 45.3989, 19.8956, 80, 0, 0),
(-19, 'Misc Task 6', 'Design a puzzle.', 44.2999, 20.5644, 75, 0, 2),
(-20, 'Location Visit 7', 'Evaluate a landmark.', 43.5800, 21.3339, 130, 0, 1);