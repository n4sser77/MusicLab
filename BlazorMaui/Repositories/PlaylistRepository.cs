using System;
using System.Text.Json;
using BlazorMaui.Helpers;
using BlazorMaui.Models;
using BlazorMaui.Repositories.Interfaces;
using BlazorMaui.Services.Interfaces;

namespace BlazorMaui.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly IAuthService _auth;

    public List<Playlist> Playlists { get; set; } = new();
    private readonly string jsonPath = AudioConstants.playlistJsonFilepath;

    public PlaylistRepository(IAuthService auth)
    {
        _auth = auth;

    }
    public async Task CreatePlaylistAsync(string title)
    {
        
        var pl = new Playlist { Title = title, Songs = new List<Beat>() };
        Playlists.Add(pl);
        await SavePlaylistsAsync();
        Playlists = (List<Playlist>)await GetPlaylistsAsync();
    }

    private async Task SavePlaylistsAsync()
    {

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(Playlists, options);

        await File.WriteAllTextAsync(jsonPath, json);
    }

    public async Task DeletePlaylistAsync(Playlist playlist)
    {
        
        var playlistToRemove = Playlists.FirstOrDefault(p => p.Id == playlist.Id);
        if (playlist == null || !Playlists.Remove(playlistToRemove))
        {
            throw new ArgumentException("Playlist not found");
        }

        await SavePlaylistsAsync();
    }

    public async Task<IEnumerable<Playlist>> GetPlaylistsAsync()
    {
        if (!File.Exists(jsonPath))
        {
            File.WriteAllText(jsonPath, "[]");
        }

        string json = File.ReadAllText(jsonPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        Playlists = JsonSerializer.Deserialize<List<Playlist>>(json, options) ?? new List<Playlist>();
        return Playlists.AsEnumerable();
    }

    public async Task UpdatePlaylistsAsync(Playlist oldPlaylist, Playlist newPlaylist)
    {
        
        var elemtedToBeReplaced = Playlists.FirstOrDefault(oldPlaylist);
        elemtedToBeReplaced = newPlaylist;
        await SavePlaylistsAsync();
    }

    public async Task AddSongToPlaylist(Playlist playlist, Beat song)
    {
        
        playlist.Songs.Add(song);
        await SavePlaylistsAsync();
    }

    public async Task RemoveSongFromPlaylist(Playlist playlist, Beat song)
    {
        
        playlist.Songs.Remove(song);
        await SavePlaylistsAsync();
    }
}
