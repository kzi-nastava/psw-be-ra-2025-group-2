-- 1. Sesija za turistu -21 (On već ima recenziju ID -1)
INSERT INTO tours."TourExecutions"(
	"Id", "TouristId", "TourId", "State", "LastActivityTimestamp", "CompletionTimestamp", "KeyPointsCount")
VALUES 
(-1, -21, -1, 1, CURRENT_TIMESTAMP, NULL, 4);

-- 2. Sesija za turistu -23 (OVO KORISTIŠ U TESTU ZA "Creates")
-- On je prešao 50% (2 od 4 tačke) i aktivan je, pa može da oceni.
INSERT INTO tours."TourExecutions"(
	"Id", "TouristId", "TourId", "State", "LastActivityTimestamp", "CompletionTimestamp", "KeyPointsCount")
VALUES 
(-2, -23, -1, 1, CURRENT_TIMESTAMP, NULL, 4);

-- 3. Sesija za turistu -22 (On ima recenziju ID -2)
INSERT INTO tours."TourExecutions"(
	"Id", "TouristId", "TourId", "State", "LastActivityTimestamp", "CompletionTimestamp", "KeyPointsCount")
VALUES 
(-3, -22, -1, 1, CURRENT_TIMESTAMP, NULL, 4);


-- POSETE KLJUČNIM TAČKAMA (KeyPointVisit)

-- Za sesiju -1 (Turista -21) - 100% pređeno
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-100, -1, 1, CURRENT_TIMESTAMP); 
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-101, -1, 2, CURRENT_TIMESTAMP);
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-102, -1, 3, CURRENT_TIMESTAMP);
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-103, -1, 4, CURRENT_TIMESTAMP);

-- Za sesiju -2 (Turista -23 - ZA TVOJ TEST) - 50% pređeno (dovoljno za recenziju)
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-200, -2, 1, CURRENT_TIMESTAMP); 
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-201, -2, 2, CURRENT_TIMESTAMP);

-- Za sesiju -3 (Turista -22) - 50% pređeno
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-300, -3, 1, CURRENT_TIMESTAMP); 
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") VALUES (-301, -3, 2, CURRENT_TIMESTAMP);