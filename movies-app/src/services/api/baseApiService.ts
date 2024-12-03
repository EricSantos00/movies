export abstract class BaseApiService {
    constructor(protected readonly baseUri: string) {
    }

    protected async fetch<T>(endpoint?: string): Promise<T> {
        const url = `${this.baseUri}${endpoint ?? ''}`;
        const response = await fetch(url);
        return response.json() as T;
    }
}