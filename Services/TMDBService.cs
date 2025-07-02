using NowPlaying.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace NowPlaying.Services
{
    public class TMDBService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        private readonly HttpClient _httpClient;

        public TMDBService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            string? tmdbKey = config["TmdbAccessKey"];
            if (string.IsNullOrEmpty(tmdbKey))
            {
                throw new ArgumentException("TMDB API key is not configured.");
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tmdbKey);
            }
        }

        public async Task<MovieListResponse> GetNowPlayingMoviesAsync()
        {
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";
            string url = "https://api.themoviedb.org/3/movie/now_playing?region=CA&language=en-US";

            var response = await _httpClient.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Failed to fetch now playing movies.");

            foreach (var movie in response.Results)
            {
                if (string.IsNullOrEmpty(movie.PosterPath))
                {
                    movie.PosterPath = "/images/poster.png"; // Default image if no poster is available
                }
                else
                {
                    movie.PosterPath = $"{imageBaseUrl}{movie.PosterPath}";
                }
            }

            return response;
        }

        public async Task<MovieListResponse> GetPopularMoviesAsync()
        {
            string imageBaseUrl = "https://image.tmdb.org/t/p/w500";
            string url = "https://api.themoviedb.org/3/movie/popular?region=CA&language=en-US";

            var response = await _httpClient.GetFromJsonAsync<MovieListResponse>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Failed to fetch now playing movies.");

            foreach (var movie in response.Results)
            {
                if (string.IsNullOrEmpty(movie.PosterPath))
                {
                    movie.PosterPath = "/images/poster.png"; // Default image if no poster is available
                }
                else
                {
                    movie.PosterPath = $"{imageBaseUrl}{movie.PosterPath}";
                }
            }

            return response;
        }
    }
}
