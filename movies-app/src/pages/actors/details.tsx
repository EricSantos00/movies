import { Error } from "@/components/common/error";
import { GridList } from "@/components/common/gridList";
import { MovieCard } from "@/components/movies/movieCard";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useActorDetailsQuery } from "@/hooks/useActorDetailsQuery";
import { ActorDetails } from "@/services/api/models/actors";
import { Link, useParams } from "react-router-dom";

export type ActorDetailsPageProps = {
    getActorDetails: (actorId: string) => Promise<ActorDetails>;
};

export function ActorDetailsPage({ getActorDetails }: ActorDetailsPageProps) {
    const { id } = useParams<{ id: string }>();
    const { isError, data } = useActorDetailsQuery(id!, getActorDetails);

    if (isError) {
        return (<Error message="Failed to fetch actor details" />)
    }

    return (
        <Card>
            <CardHeader>
                <CardTitle>{data.name}</CardTitle>
            </CardHeader>
            <CardContent>
                <div>
                    <h4 className="font-semibold text-gray-800 mb-4 mt-4">Movies</h4>
                    <GridList
                        items={data.movies}
                        renderItem={(movie) => (
                            <MovieCard
                                description={movie.description}
                                title={<Link to={`/movies/details/${movie.id}`}>{movie.title}</Link>}
                                rating={movie.averageRating}
                                releaseDate={movie.releaseDate.toString()}
                            />
                        )}
                    />
                </div>
            </CardContent>
        </Card>
    );
};