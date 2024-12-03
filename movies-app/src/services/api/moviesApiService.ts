import { API_BASE_URL } from "../../constants";
import { BaseApiService } from "./baseApiService";
import { Movie } from "./models/movies";

export class MoviesApiService extends BaseApiService {
    constructor() {
        super(`${API_BASE_URL}/movies/`);
    }

    public async getMovies(): Promise<Movie[]> {
        return this.fetch();
    }
}

export const moviesApiServiceFactory = (): MoviesApiService => {
    return new MoviesApiService();
}