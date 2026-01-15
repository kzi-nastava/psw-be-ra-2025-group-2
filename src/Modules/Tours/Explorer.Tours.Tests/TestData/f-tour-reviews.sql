-- Recenzija 1: Vezujemo je za ExecutionId -1 (Turista -21)
INSERT INTO tours."TourReviews"(
    "Id", "Rating", "Comment", "TouristId", "TourId", "ReviewDate", "CompletedPercentage", "Images", "ExecutionId") 
VALUES 
(-1, 5, 'Super tura!', -21, -1, '2024-01-01 10:00:00', 100, '{{img1.jpg}}', -1);

-- Recenzija 2: Vezujemo je za ExecutionId -3 (Turista -22)
INSERT INTO tours."TourReviews"(
    "Id", "Rating", "Comment", "TouristId", "TourId", "ReviewDate", "CompletedPercentage", "Images", "ExecutionId") 
VALUES 
(-2, 3, 'Moze bolje', -22, -1, '2024-01-02 10:00:00', 50, '{{}}', -3);