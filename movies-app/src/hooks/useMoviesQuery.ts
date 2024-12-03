import { useQuery } from "@tanstack/react-query";

export const useMoviesQuery = <T>(queryFn: () => Promise<T>) => {
    return useQuery({
        queryKey: ['movies'],
        queryFn
    });
};