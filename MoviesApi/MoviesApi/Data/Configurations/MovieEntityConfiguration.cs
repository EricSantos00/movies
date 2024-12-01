using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoviesApi.Entities;

namespace MoviesApi.Data.Configurations;

public sealed class MovieEntityConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasMany(x => x.Actors)
            .WithMany(x => x.Movies);

        builder.OwnsMany(x => x.Ratings, rating =>
        {
            rating.WithOwner().HasForeignKey("OwnerId");
            rating.Property<int>("Id");
            rating.HasKey("Id");
        });
    }
}