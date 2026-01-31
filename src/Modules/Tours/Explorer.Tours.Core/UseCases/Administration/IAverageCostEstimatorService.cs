using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public interface IAverageCostEstimatorService
    {
        AverageCost Estimate(Tour tour);
    }
}
