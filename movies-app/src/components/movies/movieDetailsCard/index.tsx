import { GridList } from "@/components/common/gridList";
import { Link } from "react-router-dom";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "../../ui/card";
import { MovieRating } from "../movieRating";

type MovieDetailsActorProps = {
    id: string;
    name: string;
};

export type MovieDetailsCardProps = {
    title: string;
    description: string;
    rating: number;
    releaseDate: string;
    actors: MovieDetailsActorProps[];
};

export function MovieDetailsCard({ title, rating, description, releaseDate, actors }: MovieDetailsCardProps) {
    return (
        <Card>
            <CardHeader>
                <CardTitle>{title}</CardTitle>
                <CardDescription>
                    <MovieRating rating={rating} />
                </CardDescription>
            </CardHeader>
            <CardContent>
                <p>{description}</p>
                <div>
                    <h4 className="font-semibold text-gray-800 mb-4 mt-4">Actors</h4>
                    <GridList
                        items={actors}
                        renderItem={(actor) => (
                            <Link className="bg-gray-100 rounded-md p-2 shadow-sm" to={`/actors/details/${actor.id}`}>{actor.name}</Link>
                        )}
                    />
                </div>
            </CardContent>
            <CardFooter>
                <p>{releaseDate}</p>
            </CardFooter>
        </Card>
    );
}