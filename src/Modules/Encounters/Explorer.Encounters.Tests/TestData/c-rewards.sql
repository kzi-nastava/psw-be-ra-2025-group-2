INSERT INTO encounters."UserRewards"
("UserId", "Level", "CouponCode", "DiscountPercentage", "GrantedAt", "ValidUntil", "IsUsed", "Description")
VALUES
-- ===== USER 1 =====
(-21, 1, 'WELCOME10', 10, NOW(), NOW() + INTERVAL '7 days', false, 'Welcome bonus for new users'),
(-21, 2, 'LEVEL2BONUS', 15, NOW(), NOW() + INTERVAL '10 days', false, 'Level 2 reward'),
(-21, 3, 'LEVEL3BONUS', 20, NOW(), NOW() + INTERVAL '14 days', false, 'Level 3 reward'),

-- ===== USER 2 =====
(-22, 1, 'WELCOME10', 10, NOW(), NOW() + INTERVAL '7 days', false, 'Welcome bonus for new users'),
(-22, 2, 'LEVEL2BONUS', 15, NOW(), NOW() + INTERVAL '10 days', true, 'Level 2 reward already used'),

-- ===== USER 3 =====
(-23, 1, 'WELCOME10', 10, NOW(), NULL, false, 'Welcome bonus with unlimited validity'),
(-23, 2, 'EXPIRED50', 50, NOW() - INTERVAL '30 days', NOW() - INTERVAL '1 day', false, 'Expired reward');
