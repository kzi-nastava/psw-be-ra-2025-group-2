using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Meetup> Meetups { get; set; }
    public DbSet<TourPreferences> TourPreferences { get; set; }
    public DbSet<AuthorAwards> AuthorAwards { get; set; }
    public DbSet<AppRating> AppRatings { get; set; }
    public DbSet<Club> Clubs { get; set; }

    public DbSet<Diary> Diaries { get; set; }

    public DbSet<TouristPosition> TouristPositions { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<DayEntry> PlannerDayEntries { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        modelBuilder.Entity<Quiz>(builder =>
        {
            builder.OwnsMany(q => q.AvailableOptions, b => b.ToJson());

            builder.Navigation(q => q.AvailableOptions).HasField("_availableOptions").UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Message>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.SenderId).IsRequired();
            builder.Property(m => m.ReceiverId).IsRequired();
            builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.UpdatedAt).IsRequired(false);
            builder.Property(m => m.IsDeleted).IsRequired().HasDefaultValue(false);
        });

        modelBuilder.ApplyConfiguration(new DayEntryConfiguration());

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);
    }
}