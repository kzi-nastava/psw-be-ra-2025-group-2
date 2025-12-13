-- Recenzija 1: Vezujemo je za ExecutionId -1
INSERT INTO tours."TourReviews"(
    "Id", "Rating", "Comment", "TouristId", "TourId", "ExecutionId", "ReviewDate", "CompletedPercentage", "Images") 
VALUES 
(-1, 5, 'Super tura!', -21, -1, -1, '2024-01-01 10:00:00', 100, '{{"img1.jpg"}}');

-- Recenzija 2: Vezujemo je za ExecutionId -3
INSERT INTO tours."TourReviews"(
    "Id", "Rating", "Comment", "TouristId", "TourId", "ExecutionId", "ReviewDate", "CompletedPercentage", "Images") 
VALUES 
(-2, 3, 'Moze bolje', -22, -1, -3, '2024-01-02 10:00:00', 50, '{{}}');