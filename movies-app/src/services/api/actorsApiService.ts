import { API_BASE_URL } from "../../constants";
import { BaseApiService } from "./baseApiService";
import { Actor, ActorDetails } from "./models/actors";

export class ActorsApiService extends BaseApiService {
    constructor() {
        super(`${API_BASE_URL}actors/`);
    }

    public async getActors(): Promise<Actor[]> {
        return this.fetch();
    }

    public async getActorDetails(id: string): Promise<ActorDetails> {
        return this.fetch(id);
    }
};

export const actorsApiServiceFactory = (): ActorsApiService => {
    return new ActorsApiService();
}