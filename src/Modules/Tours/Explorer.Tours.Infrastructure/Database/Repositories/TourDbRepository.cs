using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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

    public List<Tour> GetAllPublished(int page, int pageSize)
    {
        var tours = _dbSet
            .Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
        return tours;
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
        try
        {
            Console.WriteLine($"=== START GetTourWithKeyPointsAsync for tourId={tourId} ===");

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

                Console.WriteLine("Executing Tour SELECT...");

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var id = reader.GetInt64(0);
                        var name = reader.GetString(1);
                        var description = reader.GetString(2);
                        var difficulty = reader.GetInt32(3);
                        var price = reader.GetDecimal(4);
                        var authorId = reader.GetInt64(5);

                        var statusValue = reader.GetValue(6);
                        Console.WriteLine($"Status value type: {statusValue.GetType().Name}, value: {statusValue}");

                        var status = statusValue is string
                            ? (TourStatus)Enum.Parse(typeof(TourStatus), statusValue.ToString())
                            : (TourStatus)statusValue;

                        DateTime? archivedAt = reader.IsDBNull(7) ? null : reader.GetDateTime(7);

                        Console.WriteLine($"Tour found: {name}, Status={status}");

                        tour = new Tour(name, description, difficulty, authorId);

                        var idProp = typeof(Tour).BaseType?.GetProperty("Id");
                        idProp?.SetValue(tour, id);

                        tour.SetStatus(status);

                        tour.SetPrice(price);

                        if (archivedAt.HasValue)
                        {
                            var archivedAtField = typeof(Tour).GetProperty("ArchivedAt",
                                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            archivedAtField?.SetValue(tour, archivedAt);
                        }

                        Console.WriteLine($"Tour object created with Id={id}, Status={status}");
                    }
                    else
                    {
                        Console.WriteLine("Tour not found in DB");
                        return null;
                    }
                }
            }

            if (tour == null)
                return null;

            var keyPointsList = new List<KeyPoint>();

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

                Console.WriteLine("Executing KeyPoints SELECT...");

                await using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetInt64(0);
                        var ordinalNo = reader.GetInt32(1);
                        var name = reader.GetString(2);
                        var description = reader.GetString(3);
                        var secretText = reader.GetString(4);
                        var imageUrl = reader.GetString(5);
                        var latitude = reader.GetDouble(6);
                        var longitude = reader.GetDouble(7);
                        var authorId = reader.GetInt64(8);

                        Console.WriteLine($"KeyPoint: {name}, Lat={latitude}, Lon={longitude}");

                        var keyPoint = new KeyPoint(
                            ordinalNo, name, description, secretText, imageUrl,
                            latitude, longitude, authorId
                        );

                        var idProperty = typeof(KeyPoint).BaseType?.GetProperty("Id");
                        idProperty?.SetValue(keyPoint, id);

                        keyPointsList.Add(keyPoint);
                    }
                }
            }

            Console.WriteLine($"Total KeyPoints: {keyPointsList.Count}");

            var field = typeof(Tour).GetField("_keyPoints",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(tour, keyPointsList);
                Console.WriteLine("KeyPoints set via reflection");
            }

            Console.WriteLine("=== END GetTourWithKeyPointsAsync ===");
            return tour;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<Tour?> GetTourByKeyPointIdAsync(long keyPointId)
    {
        return await DbContext.Set<Tour>()
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }
}
