using BlazorMaui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorMaui.Repositories.Interfaces
{
    public interface IPlaylistRepository
    {
        Task CreatePlaylistAsync(string title);
        Task<IEnumerable<Playlist>> GetPlaylistsAsync();
        Task SavePlaylistsAsync(List<Playlist> playlists);

        Task DeletePlaylistAsync(Playlist playlist);


    }
}
