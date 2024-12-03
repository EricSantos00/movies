import { createBrowserRouter } from "react-router-dom";
import Layout from "./components/layout";
import { ActorsPage } from "./pages/actors";
import { ActorDetailsPage } from "./pages/actors/details";
import ErrorPage from "./pages/error";
import { MoviesPage } from "./pages/movies";
import { MovieDetailsPage } from "./pages/movies/details";
import { actorsApiServiceFactory } from "./services/api/actorsApiService";
import { moviesApiServiceFactory } from "./services/api/moviesApiService";

const moviesApiService = moviesApiServiceFactory();
const actorsApiService = actorsApiServiceFactory();

const router = createBrowserRouter([{
    path: "/",
    element: <Layout />,
    errorElement: <ErrorPage />,
    children: [
        {
            path: "/",
            element: <MoviesPage getMovies={() => moviesApiService.getMovies()} />
        },
        {
            path: "/movies",
            element: <MoviesPage getMovies={() => moviesApiService.getMovies()} />
        },
        {
            path: "/movies/details/:id",
            element: <MovieDetailsPage getMovieDetails={(id: string) => moviesApiService.getMovieDetails(id)} />
        },
        {
            path: "/actors",
            element: <ActorsPage getActors={() => actorsApiService.getActors()} />
        },
        {
            path: "/actors/details/:id",
            element: <ActorDetailsPage getActorDetails={(id: string) => actorsApiService.getActorDetails(id)} />
        }
    ]
}]);

export default router;