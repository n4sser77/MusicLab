
using System.Text.Json;
namespace BlazorMaui.Services.Interfaces;
public interface IBeatsService
{
    public List<Beat> UploadedAudio { get; set; }
    public List<Beat> UploadedAudioFromServer { get; set; }

    public Beat FileToEdit { get; set; }

    public Task LoadUploaded();

    public Task UploadAudio(string fullpath, string title);
    public Task<bool> PublishAudio(Beat beat, int userId);
    public Task GetAudioFromServer();
    public Task StreamAudioFromServer(string filename);
    public Task<bool> DeleteLocalAudio(Beat beat);

    public Task UpdateAudio(Beat newBeat, Beat oldBeat);


}