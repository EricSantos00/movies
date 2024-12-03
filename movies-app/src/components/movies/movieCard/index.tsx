import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "../../ui/card";
import { MovieRating } from "../movieRating/movieRating";

export type MovieCardProps = {
    title: string | React.ReactElement;
    description: string;
    rating: number;
    releaseDate: string;
};

export function MovieCard({ title, rating, description, releaseDate }: MovieCardProps) {
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
            </CardContent>
            <CardFooter>
                <p>{releaseDate}</p>
            </CardFooter>
        </Card>
    );
}