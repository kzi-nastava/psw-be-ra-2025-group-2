using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}

    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        // Konfigurišem BlogPost
        modelBuilder.Entity<BlogPost>(b =>
        {
            b.HasKey(x => x.Id);

            // Owned collection za BlogImage
            b.OwnsMany(x => x.Images, img =>
            {
                img.WithOwner().HasForeignKey("BlogPostId"); // FK na BlogPost
                img.Property(i => i.Url).IsRequired();
                img.ToTable("BlogImages", "blog"); // opcionalno, zasebna tabela u blog šemi
                img.HasKey("BlogPostId", "Url"); // composite key jer nema ID
            });

            b.Property(x => x.Title).IsRequired();
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.AuthorId).IsRequired();
        });
    }
}