using BlazorMaui.Helpers;
using BlazorMaui.Services.Interfaces;
using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;
using System.Text.Json;

public class BeatsService : IBeatsService
{
    public List<Beat> UploadedAudio { get; set; } = new();
    string jsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "audio_metadata.json");
    


    //public bool _isPlaying = false;

    public Beat FileToEdit { get; set; }





    public void LoadUploaded()
    {


        // Ensure JSON file exists
        if (!File.Exists(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, "[]"); // Initialize empty JSON array
        }

        // Read and Deserialize Only Required Fields
        string json = File.ReadAllText(jsonFilePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        UploadedAudio = JsonSerializer.Deserialize<List<Beat>>(json, options) ?? new List<Beat>();

        // Scan all files in AppDataDirectory
        var files = Directory.GetFiles(FileSystem.AppDataDirectory).ToList();

        // Remove missing files from JSON
        UploadedAudio.RemoveAll(a => !files.Contains(a.AudioUrl));

        // Add new files that are not in JSON
        foreach (var filePath in files)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (!AudioConstants.SupportedAudioExtensions.Contains(extension))
                continue; // Skip unsupported files

            if (!UploadedAudio.Any(a => a.AudioUrl == filePath))
            {
                string title = Path.GetFileNameWithoutExtension(filePath);
                UploadedAudio.Add(new Beat { AudioUrl = filePath, Title = title });
            }
        }


        // Save updated metadata back to JSON
        File.WriteAllText(jsonFilePath, JsonSerializer.Serialize(UploadedAudio, options));
    }


    public void UploadAudio(string fullpath, string title)
    {
        if (!UploadedAudio.Any(f => f.AudioUrl.Contains(fullpath)))
        {
            string extension = Path.GetExtension(fullpath);
            if (!AudioConstants.SupportedAudioExtensions.Contains(extension))
                return;
            string localPath = Path.Combine(FileSystem.AppDataDirectory, title);
            UploadedAudio.Add(new Beat { AudioUrl = localPath, Title = title });
            File.WriteAllBytes(localPath, File.ReadAllBytes(fullpath));
        }
    }

    public void UpdateAudio(Beat newBeat, Beat oldBeat)
    {


        // Ensure JSON file exists
        if (!File.Exists(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, "[]"); // Initialize empty JSON array
        }


        // if file title is not changed, write other changes then return
        if (oldBeat.AudioUrl == newBeat.AudioUrl)
        {
            var oldFromUploads = UploadedAudio.FirstOrDefault(f => f.AudioUrl == oldBeat.AudioUrl);
            oldFromUploads.Bpm = newBeat.Bpm;
            oldFromUploads.Genre = newBeat.Genre;
            oldFromUploads.Plays = newBeat.Plays;

            var json = JsonSerializer.Serialize(UploadedAudio);
            File.WriteAllText(jsonFilePath, json);
            return;
        }

        // if file name changed
        // Rename (move) the file from the old path to the new path.
        File.Move(oldBeat.AudioUrl, newBeat.AudioUrl);

        // Find the existing beat in the list.
        var beatToUpdate = UploadedAudio.FirstOrDefault(b => b.AudioUrl == oldBeat.AudioUrl);
        if (beatToUpdate != null)
        {
            // Directly update its properties.
            beatToUpdate.Title = newBeat.Title;
            beatToUpdate.Bpm = newBeat.Bpm;
            beatToUpdate.Genre = newBeat.Genre;
            beatToUpdate.AudioUrl = newBeat.AudioUrl;
        }
    }


}


