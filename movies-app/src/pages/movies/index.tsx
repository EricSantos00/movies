import { Error } from "@/components/common/error";
import { GridList } from "@/components/common/gridList";
import { MovieCard } from "@/components/movies/movieCard";
import { useMoviesQuery } from "@/hooks/useMoviesQuery";
import { Movie } from "@/services/api/models/movies";
import { Link } from "react-router-dom";

export type MoviesPageProps = {
    getMovies: () => Promise<Movie[]>;
};

export function MoviesPage({ getMovies }: MoviesPageProps) {
    const { isError, data } = useMoviesQuery(getMovies);

    if (isError) {
        return (<Error message="Failed to fetch movies list" />)
    }

    return (
        <div>
            <GridList
                items={data}
                title="Movies List"
                renderItem={(movie) => (
                    <MovieCard
                        title={<Link to={`/movies/details/${movie.id}`}>{movie.title}</Link>}
                        description={movie.description}
                        rating={movie.averageRating}
                        releaseDate={movie.releaseDate.toString()}
                    />
                )}
            />
        </div>
    );
};