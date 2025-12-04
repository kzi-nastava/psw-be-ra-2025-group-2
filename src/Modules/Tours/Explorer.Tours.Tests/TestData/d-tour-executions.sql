INSERT INTO tours."TourExecutions"(
	"Id", "TouristId", "TourId", "State", "LastActivityTimestamp", "CompletionTimestamp")
VALUES 
(-1, -21, -2, 1, CURRENT_TIMESTAMP, NULL); 


INSERT INTO tours."KeyPointVisit"(
	"Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp")
VALUES 
(-100, -1, 1, CURRENT_TIMESTAMP); -- Pretpostavljamo da tura -3 ima ključnu tačku sa ID -31
INSERT INTO tours."KeyPointVisit"("Id", "TourExecutionId", "KeyPointOrdinal", "ArrivalTimestamp") 
VALUES (-101, -1, 2, CURRENT_TIMESTAMP);