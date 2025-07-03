using Microsoft.JSInterop;
using NowPlaying.Models;
using System.Text.Json;

namespace NowPlaying.Services
{
    public class FavouriteService(IJSRuntime jSRuntime)
    {
        private readonly string _localStorageKey = "favourites";

        /// <summary>
        /// Returns a list of movies stored in local storage as favourites.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Movie>> GetFavouritesAsync()
        {
            List<Movie> movies = [];
            try
            {
                var json = await jSRuntime.InvokeAsync<string>("localStorage.getItem", _localStorageKey);
                movies = JsonSerializer.Deserialize<List<Movie>>(json) ?? [];

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error retrieving favourites from local storage.");
            }

            return movies;
        }

        /// <summary>
        /// Save a list of movies as favourites in local storage.
        /// </summary>
        /// <param name="movies"></param>
        /// <returns></returns>
        public async Task SaveFavoritesAsync(List<Movie> movies)
        {
            try
            {
                var json = JsonSerializer.Serialize(movies);

                await jSRuntime.InvokeVoidAsync("localStorage.setItem", _localStorageKey, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error saving favourites to local storage.");
            }
        }

        /// <summary>
        /// Add a movie to the favourites list in local storage.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public async Task AddFavouriteAsync(Movie movie)
        {
            try
            {
                var current = await GetFavouritesAsync();
                if (current.All(mbox => mbox.Id != movie.Id))
                {
                    current.Add(movie);
                    await SaveFavoritesAsync(current);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error adding favourite.");
            }
        }

        /// <summary>
        /// Remove a movie from the favourites list in local storage.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public async Task RemoveFavouriteAsync(Movie movie)
        {
            try
            {
                var current = await GetFavouritesAsync();
                if (current.Any(mbox => mbox.Id == movie.Id))
                {
                    current.RemoveAll(mbox => mbox.Id == movie.Id);
                    await SaveFavoritesAsync(current);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error removing favourite.");
            }
        }

        /// <summary>
        /// Returns true if the movie is in the favourites list in local storage.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public async Task<bool> IsFavouriteAsync(Movie movie)
        {
            try
            {
                var current = await GetFavouritesAsync();
                return current.Any(mbox => mbox.Id == movie.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Error checking if movie is favourite.");
                return false;
            }
        }
    }
}
