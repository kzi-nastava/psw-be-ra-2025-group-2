namespace Explorer.Tours.Core.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Archived
}

public enum TourEnvironmentType
{
    Urban = 1,
    Nature = 2,
    Mixed = 3
}

public enum FoodType
{
    Vegetarian = 1,
    Vegan = 2,
    GlutenFree = 3,
    LocalCuisine = 4,
    FastFood = 5,
    Finedining = 6,
    StreetFood = 7
}

public enum AdventureLevel
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum ActivityType
{
    Adrenaline = 1,
    Cultural = 2,
    Relaxing = 3
}



public enum SuitableFor
{
    Students = 1,
    Children = 2,
    Elderly = 3,
    Families = 4,
    Adults = 5
}