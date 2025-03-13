using System.Text.Json;
using BlazorMaui.Helpers;
using BlazorMaui.Models;
using BlazorMaui.Services.Interfaces;

namespace BlazorMaui.Services;

public class PlaylistService : IPlaylistService
{
    private readonly IAuthService _auth;
    public List<Playlist> Playlists { get; set; } = new();
    public List<Playlist> PlaylistsFromServer { get; set; } = new();

    private readonly string jsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "playlist_metadata.json");
    public Playlist PlaylistToEdit { get; set; }
    private static readonly PlaylistService instance = new PlaylistService();

    private PlaylistService() { }

    public static PlaylistService GetService()
    {
        return instance;
    }


    public async Task CreatePlayList(string title)
    {
        var pl = new Playlist { Title = title, Songs = new List<Beat>() };
        Playlists.Add(pl);
        await SavePlaylists();
    }

    private async Task SavePlaylists()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Playlists, options);
        await File.WriteAllTextAsync(jsonFilePath, json);
    }

    public async Task LoadPlaylists()
    {

        // Ensure JSON file exists
        if (!File.Exists(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, "[]"); // Initialize empty JSON array
        }

        // Read and Deserialize Only Required Fields
        string json = File.ReadAllText(jsonFilePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        Playlists = JsonSerializer.Deserialize<List<Playlist>>(json, options) ?? new List<Playlist>();

    }

    public async Task AddSongToPlaylist(Playlist playlist, Beat song)
    {
        playlist.Songs.Add(song);
        await SavePlaylists();
    }
    public async Task RemoveSongFromPlaylist(Playlist playlist, Beat song)
    {
        playlist.Songs.Remove(song);
        await SavePlaylists();
    }

    public async Task<bool> DeletePlaylist(Playlist playlist)
    {
        if (playlist == null || !Playlists.Remove(playlist))
        {
            return false; // Invalid input or playlist not found
        }

        await SavePlaylists(); // Save changes
        return true; // Successfully removed and saved
    }


    public async Task UpdatePlayList(Playlist oldPlaylist, Playlist newPlaylist)
    {
        var elemtedToBeReplaced = Playlists.FirstOrDefault(oldPlaylist);
        elemtedToBeReplaced = newPlaylist;
        await SavePlaylists();
    }
}
