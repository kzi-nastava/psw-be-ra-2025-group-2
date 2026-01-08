-- d-bundles.sql
DELETE FROM tours."Bundles";  -- Obriši stare podatke

-- Bundle za Create test i Publish test (Draft)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-1, 'Test Draft Bundle', 100.00, 'Draft', -11, '-1,-2', NOW(), NULL);

-- Bundle za Archive test (Published)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-2, 'Test Published Bundle', 150.00, 'Published', -11, '-1,-2', NOW(), NOW());

-- Bundle sa 1 turom (za Publish_fails_insufficient test)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-3, 'Test Insufficient Tours Bundle', 50.00, 'Draft', -11, '-1', NOW(), NULL);

-- Bundle za Delete test (Draft)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-4, 'Test Bundle To Delete', 75.00, 'Draft', -11, '-1,-2', NOW(), NULL);

-- Bundle drugog autora (za unauthorized testove)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-5, 'Other Author Bundle', 120.00, 'Draft', -12, '-13,-14', NOW(), NULL);

-- NOVI: Bundle za Update test (Draft)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-6, 'Test Bundle For Update', 90.00, 'Draft', -11, '-1,-2', NOW(), NULL);

-- NOVI: Bundle za Update_fails_published test (Published)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-7, 'Test Published Bundle For Update Fail', 160.00, 'Published', -11, '-1,-2', NOW(), NOW());

-- NOVI: Bundle za Archive_fails_not_published test (Draft)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-8, 'Test Draft Bundle For Archive Fail', 110.00, 'Draft', -11, '-1,-2', NOW(), NULL);

-- NOVI: Bundle za Publish test (Draft sa 2 ture)
INSERT INTO tours."Bundles" ("Id", "Name", "Price", "Status", "AuthorId", "TourIds", "CreatedAt", "UpdatedAt")
VALUES (-9, 'Test Bundle For Publish', 130.00, 'Draft', -11, '-1,-2', NOW(), NULL);