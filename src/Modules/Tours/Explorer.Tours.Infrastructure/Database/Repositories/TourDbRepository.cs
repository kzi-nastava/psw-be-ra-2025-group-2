using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public async Task<Tour> AddAsync(Tour tour)
    {
        await _dbSet.AddAsync(tour);
        await DbContext.SaveChangesAsync();
        return tour;
    }

    public async Task<Tour?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tour>> GetByAuthorAsync(long authorId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .Where(t => t.AuthorId == authorId)
            .ToListAsync();
    }

    public List<Tour> GetAllPublished()
    {
        return _dbSet
            .Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
    }

    public async Task UpdateAsync(Tour tour)
    {
        try
        {
            DbContext.Update(tour);
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
    }

    public async Task DeleteAsync(Tour tour)
    {
        _dbSet.Remove(tour);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Tour?> GetTourWithKeyPointsAsync(long tourId)
    {
        if (DbContext.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await DbContext.Database.OpenConnectionAsync();
        }

        Tour? tour = null;

        await using (var command = DbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = @"
                SELECT 
                    ""Id"",
                    ""Name"",
                    ""Description"",
                    ""Difficulty"",
                    ""Price"",
                    ""AuthorId"",
                    ""Status"",
                    ""ArchivedAt""
                FROM tours.""Tours""
                WHERE ""Id"" = @tourId
            ";

            var param = command.CreateParameter();
            param.ParameterName = "@tourId";
            param.Value = tourId;
            command.Parameters.Add(param);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                if (!await reader.ReadAsync()) return null;

                var id = reader.GetInt64(0);
                var name = reader.GetString(1);
                var description = reader.GetString(2);
                var difficulty = reader.GetInt32(3);
                var price = reader.GetDecimal(4);
                var authorId = reader.GetInt64(5);

                var statusValue = reader.GetValue(6);
                var status = statusValue is string
                    ? (TourStatus)Enum.Parse(typeof(TourStatus), statusValue.ToString()!)
                    : (TourStatus)statusValue;

                DateTime? archivedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7);

                tour = new Tour(name, description, difficulty, authorId);

                typeof(Tour).BaseType?.GetProperty("Id")?.SetValue(tour, id);
                tour.SetStatus(status);
                tour.SetPrice(price);

                if (archivedAt.HasValue)
                {
                    typeof(Tour)
                        .GetProperty("ArchivedAt",
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance)
                        ?.SetValue(tour, archivedAt);
                }
            }
        }

        if (tour == null) return null;

        var keyPoints = new List<KeyPoint>();

        await using (var command = DbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = @"
                SELECT 
                    ""Id"",
                    ""OrdinalNo"",
                    ""Name"",
                    ""Description"",
                    ""SecretText"",
                    ""ImageUrl"",
                    ""Latitude"",
                    ""Longitude"",
                    ""AuthorId""
                FROM tours.""KeyPoint""
                WHERE ""TourId"" = @tourId
                ORDER BY ""OrdinalNo""
            ";

            var param = command.CreateParameter();
            param.ParameterName = "@tourId";
            param.Value = tourId;
            command.Parameters.Add(param);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var keyPoint = new KeyPoint(
                        reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetDouble(6),
                        reader.GetDouble(7),
                        reader.GetInt64(8)
                    );

                    typeof(KeyPoint).BaseType?.GetProperty("Id")
                        ?.SetValue(keyPoint, reader.GetInt64(0));

                    keyPoints.Add(keyPoint);
                }
            }
        }

        typeof(Tour)
            .GetField("_keyPoints",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            ?.SetValue(tour, keyPoints);

        return tour;
    }

    public async Task<Tour?> GetTourByKeyPointIdAsync(long keyPointId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }

    public async Task<IEnumerable<Tour?>> GetAllAsync()
    {
        return await _dbSet.Include(t => t.KeyPoints).ToListAsync();
    }
}
