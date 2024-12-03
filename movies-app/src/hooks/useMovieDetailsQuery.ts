import { useSuspenseQuery } from "@tanstack/react-query";

export const useMovieDetailsQuery = <T>(movieId: string, queryFn: (movieId: string) => Promise<T>) => {
    return useSuspenseQuery({
        queryKey: ['movies', movieId],
        queryFn: () => queryFn(movieId)
    });
};