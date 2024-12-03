import { Error } from "@/components/common/error";
import { MovieDetailsCard } from "@/components/movies/movieDetailsCard";
import { useMovieDetailsQuery } from "@/hooks/useMovieDetailsQuery";
import { MovieDetails } from "@/services/api/models/movies";
import { useParams } from "react-router-dom";

export type MovieDetailsPageProps = {
    getMovieDetails: (movieId: string) => Promise<MovieDetails>;
};

export function MovieDetailsPage({ getMovieDetails }: MovieDetailsPageProps) {
    const { id } = useParams<{ id: string }>();
    const { isError, data } = useMovieDetailsQuery(id!, getMovieDetails);

    if (isError) {
        return (<Error message="Failed to fetch actor details" />)
    }

    return (
        <MovieDetailsCard
            title={data.title}
            description={data.description}
            rating={data.averageRating}
            releaseDate={data.releaseDate.toString()}
            actors={data.actors.map((actor) => ({
                id: actor.id,
                name: actor.name
            }))}
        />
    );
};