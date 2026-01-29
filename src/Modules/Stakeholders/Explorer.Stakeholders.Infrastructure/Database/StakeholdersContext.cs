using Explorer.Stakeholders.Core.Domain;

using Explorer.Stakeholders.Core.Domain.Emergency;
using Explorer.Stakeholders.Core.Domain.Help;
using Explorer.Stakeholders.Core.Domain.Planner;
using Explorer.Stakeholders.Core.Domain.Quizzes;
using Explorer.Stakeholders.Infrastructure.Database.Configurations;
using Explorer.Stakeholders.Infrastructure.Database.Configurations.Emergency;
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
    public DbSet<ClubBadge> ClubBadges { get; set; }


    public DbSet<EmergencyDirectory> EmergencyDirectories { get; set; }
    public DbSet<EmergencyPlace> EmergencyPlaces { get; set; }

    public DbSet<Embassy> Embassies { get; set; }
    public DbSet<EmergencyPhrase> EmergencyPhrases { get; set; }

    public DbSet<Diary> Diaries { get; set; }

    public DbSet<TouristPosition> TouristPositions { get; set; }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<OnboardingSlide> OnboardingSlides { get; set; }
    public DbSet<OnboardingProgress> OnboardingProgresses { get; set; }

    public DbSet<DayEntry> PlannerDayEntries { get; set; }

    public DbSet<HelpSettings> HelpSettings { get; set; }
    public DbSet<FaqItem> FaqItems { get; set; }

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

        modelBuilder.Entity<HelpSettings>().HasIndex(x => x.PersonId).IsUnique();
        modelBuilder.ApplyConfiguration(new DayEntryConfiguration());

        modelBuilder.ApplyConfiguration(new EmergencyDirectoryConfiguration());
        modelBuilder.ApplyConfiguration(new EmergencyPlaceConfiguration());
        modelBuilder.ApplyConfiguration(new EmbassyConfiguration());
        modelBuilder.ApplyConfiguration(new EmergencyPhraseConfiguration());



        modelBuilder.Entity<Club>(builder =>
        {
            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.Members)
                   .WithOne()
                   .HasForeignKey(nameof(ClubMember.ClubId))
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Members)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(c => c.JoinRequests)
                   .WithOne()
                   .HasForeignKey(nameof(ClubJoinRequest.ClubId))
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.JoinRequests)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(c => c.Invitations)
                   .WithOne()
                   .HasForeignKey(nameof(ClubInvitation.ClubId))
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Invitations)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.HasMany(c => c.Badges)
                .WithOne()
                .HasForeignKey(nameof(ClubBadge.ClubId))
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(c => c.Badges)
                .HasField("_badges")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

        });


        modelBuilder.Entity<ClubMember>(builder =>
        {
            builder.ToTable("ClubMembers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClubId).IsRequired();
            builder.Property(x => x.TouristId).IsRequired();
            builder.Property(x => x.JoinedAt).IsRequired();

            builder.HasIndex(x => new { x.ClubId, x.TouristId })
                   .IsUnique();
        });

        modelBuilder.Entity<ClubInvitation>(builder =>
        {
            builder.ToTable("ClubInvitations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClubId).IsRequired();
            builder.Property(x => x.TouristId).IsRequired();
            builder.Property(x => x.SentAt).IsRequired();

            builder.HasIndex(x => new { x.ClubId, x.TouristId })
                   .IsUnique();
        });

        modelBuilder.Entity<ClubJoinRequest>(builder =>
        {
            builder.ToTable("ClubJoinRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClubId).IsRequired();
            builder.Property(x => x.TouristId).IsRequired();
            builder.Property(x => x.RequestedAt).IsRequired();

            builder.HasIndex(x => new { x.ClubId, x.TouristId })
                   .IsUnique();
        });
        
        modelBuilder.Entity<ClubBadge>(builder =>
        {
            builder.ToTable("ClubBadges");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ClubId).IsRequired();
            builder.Property(x => x.MilestoneXp).IsRequired();
            builder.Property(x => x.AwardedAt).IsRequired();

            builder.HasIndex(x => new { x.ClubId, x.MilestoneXp })
                .IsUnique();
        });



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