using BlazorMaui.Models;

namespace BlazorMaui.Services.Interfaces;

public interface IPlaylistService
{
    
    public List<Playlist> Playlists { get; set; }
    public List<Playlist> PlaylistsFromServer { get; set; }
    
    public Playlist PlaylistToEdit { get; set; }

    public Task CreatePlayList(string title);
    public Task UpdatePlayList(Playlist oldPlaylist, Playlist newPlaylist);

    //private Task SavePlaylists();
    public Task LoadPlaylists();
    public Task AddSongToPlaylist(Playlist playlist, Beat song);
    
    public Task RemoveSongFromPlaylist(Playlist playlist, Beat song);
    
    public Task<bool> DeletePlaylist(Playlist playlist);


}