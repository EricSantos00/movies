import { Actor } from "./actors";

export type Movie = {
    id: string;
    title: string;
    description: string;
    releaseDate: Date;
    averageRating: number;
};

export type MovieDetails = Movie & {
    actors: Actor[];
};