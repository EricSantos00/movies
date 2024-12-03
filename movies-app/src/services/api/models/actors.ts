import { Movie } from "./movies";

export type Actor = {
    id: string;
    name: string;
};

export type ActorDetails = Actor & {
    movies: Movie[];
};