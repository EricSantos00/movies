import { useSuspenseQuery } from "@tanstack/react-query";

export const useActorsQuery = <T>(queryFn: () => Promise<T>) => {
    return useSuspenseQuery({
        queryKey: ['actors'],
        queryFn
    });
};