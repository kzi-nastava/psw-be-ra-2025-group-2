using System;
using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Domain; // prilagodi ako je drugačije

namespace Explorer.Stakeholders.Core.Domain
{
    public enum TourDifficulty
    {
        Easy = 0,
        Medium = 1,
        Hard = 2
    }

    public class TourPreferences : Entity
    {
        public long TouristId { get; private set; }

        public TourDifficulty PreferredDifficulty { get; private set; }

        public int WalkingScore { get; private set; }    // 0–3
        public int BicycleScore { get; private set; }    // 0–3
        public int CarScore { get; private set; }        // 0–3
        public int BoatScore { get; private set; }       // 0–3

        public List<string> Tags { get; private set; }
        private TourPreferences() { }

        public TourPreferences(
            long touristId,
            TourDifficulty preferredDifficulty,
            int walkingScore,
            int bicycleScore,
            int carScore,
            int boatScore,
            List<string> tags)
        {
            if (touristId <= 0)
                throw new ArgumentException("TouristId must be positive.", nameof(touristId));

            ValidateScore(walkingScore, nameof(walkingScore));
            ValidateScore(bicycleScore, nameof(bicycleScore));
            ValidateScore(carScore, nameof(carScore));
            ValidateScore(boatScore, nameof(boatScore));

            TouristId = touristId;
            PreferredDifficulty = preferredDifficulty;

            WalkingScore = walkingScore;
            BicycleScore = bicycleScore;
            CarScore = carScore;
            BoatScore = boatScore;

            Tags = tags ?? new List<string>();
        }

        private void ValidateScore(int score, string paramName)
        {
            if (score < 0 || score > 3)
                throw new ArgumentOutOfRangeException(paramName, "Score must be between 0 and 3.");
        }

        // Metoda za izmenu preferenci
        public void Update(
            TourDifficulty preferredDifficulty,
            int walkingScore,
            int bicycleScore,
            int carScore,
            int boatScore,
            List<string> tags)
        {
            ValidateScore(walkingScore, nameof(walkingScore));
            ValidateScore(bicycleScore, nameof(bicycleScore));
            ValidateScore(carScore, nameof(carScore));
            ValidateScore(boatScore, nameof(boatScore));

            PreferredDifficulty = preferredDifficulty;
            WalkingScore = walkingScore;
            BicycleScore = bicycleScore;
            CarScore = carScore;
            BoatScore = boatScore;
            Tags = tags ?? new List<string>();
        }
    }
}
