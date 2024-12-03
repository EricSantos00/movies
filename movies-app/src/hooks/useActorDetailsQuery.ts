import { useSuspenseQuery } from "@tanstack/react-query";

export const useActorDetailsQuery = <T>(actorId: string, queryFn: (actorId: string) => Promise<T>) => {
    return useSuspenseQuery({
        queryKey: ['actors', actorId],
        queryFn: () => queryFn(actorId)
    });
};