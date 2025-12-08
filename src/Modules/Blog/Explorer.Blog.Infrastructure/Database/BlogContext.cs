using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        // Konfigurišem BlogPost
        modelBuilder.Entity<BlogPost>(b =>
        {
            b.HasKey(x => x.Id);

            // Owned collection za BlogImage
            b.OwnsMany(p => p.Images, img =>
            {
                img.ToTable("BlogImages", "blog");
                img.WithOwner().HasForeignKey("BlogPostId");
                img.HasKey(i => i.Id);
                img.Property(i => i.Id).ValueGeneratedOnAdd(); // EF generiše ID
                img.Property(i => i.Url).IsRequired();
            });


            b.OwnsMany(p => p.Votes, vote =>
            {
                vote.ToTable("BlogPostVotes", "blog");
                vote.WithOwner().HasForeignKey("BlogPostId");

                vote.Property<int>("Id");
                vote.HasKey("Id");

                vote.Property(v => v.UserId).HasColumnName("UserId").IsRequired();

                vote.Property(v => v.CreatedAt).IsRequired();

                vote.OwnsOne(v => v.Value, voteValue =>
                {
                    voteValue.Property(vv => vv.Value).HasColumnName("VoteValue").IsRequired();
                });

                vote.HasIndex("BlogPostId", "UserId").IsUnique();
            });

            b.Property(x => x.Title).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.AuthorId).IsRequired();
        });
    }
}