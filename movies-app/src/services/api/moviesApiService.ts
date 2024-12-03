import { API_BASE_URL } from "../../constants";
import { BaseApiService } from "./baseApiService";
import { Movie, MovieDetails } from "./models/movies";

export class MoviesApiService extends BaseApiService {
    constructor() {
        super(`${API_BASE_URL}movies/`);
    }

    public async getMovies() {
        return this.fetch<Movie[]>();
    }

    public async getMovieDetails(id: string) {
        return this.fetch<MovieDetails>(id);
    }
}

export const moviesApiServiceFactory = (): MoviesApiService => {
    return new MoviesApiService();
}