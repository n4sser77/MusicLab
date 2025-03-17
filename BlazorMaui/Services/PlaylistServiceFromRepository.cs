using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using BlazorMaui.Models;
using BlazorMaui.Repositories.Interfaces;
using BlazorMaui.Services.Interfaces;

namespace BlazorMaui.Services;

public class PlaylistServiceFromRepository : IPlaylistService
{

    private readonly IAuthService _auth;
    public List<Playlist> Playlists { get; set; } = new();
    public List<Playlist> PlaylistsFromServer { get; set; } = new();

    private readonly string jsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "playlist_metadata.json");
    public Playlist PlaylistToEdit { get; set; }
    private readonly IPlaylistRepository _playlistRepository;
    private static readonly PlaylistServiceFromRepository instance = new PlaylistServiceFromRepository();

    private PlaylistServiceFromRepository()
    {
        _auth = MauiProgram.ServiceProvider.GetService<IAuthService>();
        _playlistRepository = MauiProgram.ServiceProvider.GetService<IPlaylistRepository>();
    }



    public static PlaylistServiceFromRepository GetService()
    {

        return instance;
    }


    public async Task CreatePlayList(string title)
    {
        await _playlistRepository.CreatePlaylistAsync(title);
    }



    public async Task LoadPlaylists()
    {
        Playlists = (List<Playlist>)await _playlistRepository.GetPlaylistsAsync();
    }

    public async Task AddSongToPlaylist(Playlist playlist, Beat song)
    {

        await _playlistRepository.AddSongToPlaylist(playlist, song);
    }
    public async Task RemoveSongFromPlaylist(Playlist playlist, Beat song)
    {
        await _playlistRepository.RemoveSongFromPlaylist(playlist, song);

    }

    public async Task<bool> DeletePlaylist(Playlist playlist)
    {
        try
        {
            await _playlistRepository.DeletePlaylistAsync(playlist);
            return true;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return false;
        }
    }


    public async Task UpdatePlayList(Playlist oldPlaylist, Playlist newPlaylist)
    {
        await _playlistRepository.UpdatePlaylistsAsync(oldPlaylist, newPlaylist);
    }
}


