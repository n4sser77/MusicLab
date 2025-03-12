namespace BlazorMaui.Models;


public class Playlist
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public List<Beat> Songs { get; set; }

    public Playlist()
    {
        Id = Guid.NewGuid();
    }
}
