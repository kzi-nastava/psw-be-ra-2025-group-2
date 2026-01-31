using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public enum CostBucket { Tickets, Transport, FoodAndDrink, Other }

    public class AverageCostEstimatorService : IAverageCostEstimatorService
    {
        private static string Norm(string? s) => (s ?? string.Empty).Trim().ToLowerInvariant();

        
        private const decimal FoodPerStop = 600m;
        private const decimal TicketPerStop = 500m;
        private const decimal OtherPerStop = 200m;

        private const decimal TransportPerKm = 35m;

        private const decimal MaxTotal = 50000m;

        private static readonly HashSet<string> FoodAmenityTypes = new()
        {
            "restaurant", "cafe", "fast_food", "bar", "pub"
        };

        private static readonly HashSet<string> TicketTourismTypes = new()
        {
            "museum", "attraction", "gallery", "zoo", "theme_park"
        };

        private static readonly HashSet<string> OtherLeisureTypes = new()
        {
            "park", "garden", "playground"
        };

        public AverageCost Estimate(Tour tour)
        {
            if (tour == null) throw new ArgumentNullException(nameof(tour));

            decimal tickets = 0m;
            decimal transport = EstimateTransport(tour);
            decimal food = 0m;
            decimal other = 0m;

            foreach (var kp in tour.KeyPoints ?? Array.Empty<KeyPoint>())
            {
                var bucket = Classify(kp.OsmClass, kp.OsmType);

                switch (bucket)
                {
                    case CostBucket.FoodAndDrink:
                        food += FoodPerStop;
                        break;

                    case CostBucket.Tickets:
                        tickets += TicketPerStop;
                        break;

                    case CostBucket.Other:
                        other += OtherPerStop;
                        break;

                    case CostBucket.Transport:
                    default:
                        break;
                }
            }

            (tickets, transport, food, other) = ApplyTourModifiers(tour, tickets, transport, food, other);

            tickets = ClampMin0(tickets);
            transport = ClampMin0(transport);
            food = ClampMin0(food);
            other = ClampMin0(other);

            (tickets, transport, food, other) = ApplyTotalCap(tickets, transport, food, other, MaxTotal);

            var breakdown = AverageCostBreakdown.Create(tickets, transport, food, other);

            return AverageCost.Create(
                "RSD",
                breakdown,
                "Informativna procena. Troškovi mogu varirati u zavisnosti od sezone, izbora usluga i ličnih navika."
            );
        }

        private static CostBucket Classify(string? osmClass, string? osmType)
        {
            var c = Norm(osmClass);
            var t = Norm(osmType);

            // amenity=restaurant/cafe/...
            if (c == "amenity" && FoodAmenityTypes.Contains(t))
                return CostBucket.FoodAndDrink;

            // tourism=museum/attraction/...
            if (c == "tourism" && TicketTourismTypes.Contains(t))
                return CostBucket.Tickets;

            // leisure=park/garden/... -> other
            if (c == "leisure" && OtherLeisureTypes.Contains(t))
                return CostBucket.Other;

            // historic=* -> other
            if (c == "historic")
                return CostBucket.Other;

            // tourism=viewpoint -> other
            if (c == "tourism" && t == "viewpoint")
                return CostBucket.Other;

            // natural=* -> other
            if (c == "natural")
                return CostBucket.Other;

            // shop=*  -> food 
            if (c == "shop")
                return CostBucket.FoodAndDrink;

            return CostBucket.Other;
        }
        private static decimal EstimateTransport(Tour tour)
        {
            
            var km = tour.LengthKm ?? 0m;
            return Math.Round(km * TransportPerKm, 0);
        }

        private static (decimal tickets, decimal transport, decimal food, decimal other) ApplyTourModifiers(
            Tour tour, decimal tickets, decimal transport, decimal food, decimal other)
        {
            if (tour.EnvironmentType == TourEnvironmentType.Nature)
            {
                transport = ApplyMultiplier(transport, 1.10m);
                other = ApplyMultiplier(other, 1.05m);
            }
            else if (tour.EnvironmentType == TourEnvironmentType.Urban)
            {
                transport = ApplyMultiplier(transport, 0.95m);
            }
            else if (tour.EnvironmentType == TourEnvironmentType.Mixed)
            {
                
                transport = ApplyMultiplier(transport, 1.02m);
            }

          
            if (tour.AdventureLevel == AdventureLevel.High)
            {
                transport = ApplyMultiplier(transport, 1.20m);
                other = ApplyMultiplier(other, 1.10m);
            }
            else if (tour.AdventureLevel == AdventureLevel.Low)
            {
                transport = ApplyMultiplier(transport, 0.90m);
            }

          
            if (tour.FoodTypes != null && tour.FoodTypes.Any())
            {
                if (tour.FoodTypes.Contains(FoodType.Finedining))
                    food = ApplyMultiplier(food, 1.25m);

                if (tour.FoodTypes.Contains(FoodType.FastFood))
                    food = ApplyMultiplier(food, 0.90m);

                if (tour.FoodTypes.Contains(FoodType.LocalCuisine))
                    food = ApplyMultiplier(food, 1.05m);

                
                if (tour.FoodTypes.Contains(FoodType.Vegan) || tour.FoodTypes.Contains(FoodType.GlutenFree))
                    food = ApplyMultiplier(food, 1.05m);
            }

           
            if (tour.ActivityTypes != null && tour.ActivityTypes.Any())
            {
                if (tour.ActivityTypes.Contains(ActivityType.Cultural))
                    tickets = ApplyMultiplier(tickets, 1.15m);

                if (tour.ActivityTypes.Contains(ActivityType.Adrenaline))
                {
                    transport = ApplyMultiplier(transport, 1.10m);
                    other = ApplyMultiplier(other, 1.20m);
                }

                if (tour.ActivityTypes.Contains(ActivityType.Relaxing))
                    other = ApplyMultiplier(other, 0.95m);
            }

            return (tickets, transport, food, other);
        }

        private static decimal ApplyMultiplier(decimal value, decimal multiplier)
            => Math.Round(value * multiplier, 0);

        private static decimal ClampMin0(decimal v) => v < 0 ? 0 : v;

        private static (decimal tickets, decimal transport, decimal food, decimal other) ApplyTotalCap(
            decimal tickets, decimal transport, decimal food, decimal other, decimal cap)
        {
            if (cap <= 0) return (tickets, transport, food, other);

            var total = tickets + transport + food + other;
            if (total <= cap) return (tickets, transport, food, other);

            
            var ratio = cap / total;

            tickets = Math.Round(tickets * ratio, 0);
            transport = Math.Round(transport * ratio, 0);
            food = Math.Round(food * ratio, 0);
            other = Math.Round(other * ratio, 0);

            return (tickets, transport, food, other);
        }
    }
}
