import { Error } from "@/components/common/error";
import { GridList } from "@/components/common/gridList";
import { useActorsQuery } from "@/hooks/useActorsQuery";
import { Actor } from "@/services/api/models/actors";
import { Link } from "react-router-dom";

export type ActorsPageProps = {
    getActors: () => Promise<Actor[]>;
};

export function ActorsPage({ getActors }: ActorsPageProps) {
    const { isError, data } = useActorsQuery(getActors);

    if (isError) {
        return (<Error message="Failed to fetch movies list" />)
    }

    return (
        <div>
            <GridList
                items={data}
                title="Actors List"
                renderItem={(actor) => (
                    <Link to={`/actors/details/${actor.id}`}>{actor.name}</Link>
                )}
            />
        </div>
    );
}