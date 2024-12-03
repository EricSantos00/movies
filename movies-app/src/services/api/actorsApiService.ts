import { API_BASE_URL } from "../../constants";
import { BaseApiService } from "./baseApiService";
import { Actor } from "./models/actors";

export class ActorsApiService extends BaseApiService {
    constructor() {
        super(`${API_BASE_URL}/actors`);
    }

    public async getActors(): Promise<Actor[]> {
        return this.fetch();
    }
};

export const actorsApiServiceFactory = (): ActorsApiService => {
    return new ActorsApiService();
}