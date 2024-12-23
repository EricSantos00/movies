import { useSuspenseQuery } from "@tanstack/react-query";

export const useMoviesQuery = <T>(queryFn: () => Promise<T>) => {
    return useSuspenseQuery({
        queryKey: ['movies'],
        queryFn
    });
};