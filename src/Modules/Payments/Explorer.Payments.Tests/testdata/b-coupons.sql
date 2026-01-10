-- b-coupons.sql
INSERT INTO payments."Coupons" ("Id", "Code", "DiscountPercentage", "AuthorId", "TourId", "ValidUntil")
VALUES
-- Author ID=-11 coupons (za testiranje)
(-1, 'TEST0001', 20, -11, -2, CURRENT_TIMESTAMP + INTERVAL '30 days'),
(-2, 'TEST0002', 15, -11, NULL, CURRENT_TIMESTAMP + INTERVAL '60 days'), -- Author-wide
(-3, 'TEST0003', 25, -11, -11, CURRENT_TIMESTAMP + INTERVAL '45 days'),
(-4, 'EXPIRED1', 20, -11, -2, CURRENT_TIMESTAMP - INTERVAL '5 days'), -- Istekao

-- Author ID=-12 coupons
(-5, 'AUTH5001', 30, -12, -13, CURRENT_TIMESTAMP + INTERVAL '40 days'),
(-6, 'AUTH5002', 10, -12, NULL, CURRENT_TIMESTAMP + INTERVAL '50 days'), -- Author-wide

-- Author ID=-13 coupons
(-7, 'AUTH4001', 15, -13, NULL, CURRENT_TIMESTAMP + INTERVAL '20 days'),

-- Shared/generic coupons
(-8, 'SUMMER25', 25, -11, NULL, CURRENT_TIMESTAMP + INTERVAL '90 days'),
(-9, 'WINTER20', 20, -12, NULL, CURRENT_TIMESTAMP + INTERVAL '120 days');