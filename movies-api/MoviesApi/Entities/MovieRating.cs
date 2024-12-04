using MoviesApi.Validation;

namespace MoviesApi.Entities;

public class MovieRating
{
    public int Rating { get; private set; }

    public MovieRating(int rating)
    {
        if (rating is < 0 or > 5)
            throw new ValidationException(nameof(Rating), "Rating must be between 0 and 5.");

        Rating = rating;
    }
}