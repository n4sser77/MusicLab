using BlazorMaui;
using BlazorMaui.Helpers;
using BlazorMaui.Services.Interfaces;
using CommunityToolkit.Maui.Views;
using System.IO;
using Shared.Dtos;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BlazorMaui.Services;
using NAudio.WaveFormRenderer;
using NAudio.Wave;
using System.Threading.Tasks;



public class BeatsService : IBeatsService
{

    private readonly IAuthService _auth;
    public List<Beat> UploadedAudio { get; set; } = new();
    public List<Beat> UploadedAudioFromServer { get; set; } = new();




    string jsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "audio_metadata.json");

    public BeatsService(IAuthService auth)
    {
        _auth = auth;
    }

    //public bool _isPlaying = false;

    public Beat? FileToEdit { get; set; }

    public async Task LoadUploaded()
    {


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



            // read tags
            var tFile = TagLib.File.Create(filePath);
            var bpm = (int)tFile.Tag.BeatsPerMinute;
            var sampleRate = tFile.Properties.AudioSampleRate;
            var bitRate = tFile.Properties.AudioBitrate;
            var duration = tFile.Properties.Duration;


            if (!UploadedAudio.Any(a => a.AudioUrl == filePath))
            {
                string title = Path.GetFileNameWithoutExtension(filePath);



                UploadedAudio.Add(new Beat { AudioUrl = filePath, Title = title, Bpm = bpm });

            }


            if (bpm != 0)
            {
                var file = UploadedAudio.FirstOrDefault(f => f.AudioUrl == filePath);
                file.Bpm = bpm;
            }


        }


        // Save updated metadata back to JSON
        File.WriteAllText(jsonFilePath, JsonSerializer.Serialize(UploadedAudio, options));
    }


    public async Task UploadAudio(string fullpath, string title)
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

    public async Task UpdateAudio(Beat newBeat, Beat oldBeat)
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
    public async Task<bool> DeleteLocalAudio(Beat beat)
    {
        string filePath = beat.AudioUrl;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }
        return false;
    }

    public async Task StreamAudioFromServer(string filename)
    {
        string serverUrlLocalHost = "http://localhost:5106/api/audio/stream/" + filename;
        var sw = new Stopwatch();
        sw.Start();
        //var generateWaveTask = Task.Run(async () => GenerateWaveformImage(serverUrlLocalHost));
        var loadAudioTask = MainPage.Instance.LoadAndStartAudio(serverUrlLocalHost, stream: true);
        //await Task.WhenAll(generateWaveTask);
        sw.Stop();

    }

    private async Task GenerateWaveformImage(string filepath)
    {

        //  waveform settings for the renderer
        var myRendererSettings = new StandardWaveFormRendererSettings();
        myRendererSettings.Width = 710;
        myRendererSettings.TopHeight = 32;
        myRendererSettings.BottomHeight = 32;
        myRendererSettings.BackgroundColor = System.Drawing.Color.Transparent;


        var maxPeakProvider = new MaxPeakProvider();
        var renderer = new WaveFormRenderer();
        using var audioStream = new AudioFileReader(filepath);
        var image = renderer.Render(audioStream, maxPeakProvider, myRendererSettings);
        using (MemoryStream ms = new MemoryStream())
        {
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            var imageBytes = ms.ToArray();
            var imgBse64 = System.Convert.ToBase64String(imageBytes);
            var f = UploadedAudioFromServer.FirstOrDefault(f => f.AudioUrl == Path.GetFileName(filepath));
            f.WaveFormImageBase64 = imgBse64;
        }
        image.Dispose();

    }

    public async Task GetAudioFromServer()
    {

        string serverUrlLocalHost = "http://localhost:5106/api/audio/getall";
        using var client = new HttpClient();
        try
        {
            var token = await _auth.GetTokenAsync();
            client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
            var res = await client.GetAsync(serverUrlLocalHost);
            var json = await res.Content.ReadAsStringAsync();
            var filesMetadata = JsonSerializer.Deserialize<List<MusicMetadataDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (filesMetadata == null || filesMetadata.Count == 0) return;

            List<Beat> newData = new();
            foreach (var file in filesMetadata)
            {

                var b = new Beat
                {
                    Id = file.Id,
                    Title = file.Title,
                    Bpm = file.Bpm,
                    Genre = file.Genre,
                    AudioUrl = file.FilePath,
                    WaveFormImageBase64 = file.WaveFormImageBase64
                };
                //UploadedAudioFromServer.Add(b);
                newData.Add(b);
            }
            UploadedAudioFromServer = newData;
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);

        }

    }

    public async Task<bool> PublishAudio(Beat beat, int userId)
    {

        string filePath = beat.AudioUrl;
        string serverUrl = "http://192.168.1.174:5106/api/files/uploadfile";
        string serverUrlLocalHost = "http://localhost:5106/api/files/uploadfile";
        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();
        try
        {

            // Read file bytes
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

            // Create DTO and serialize to JSON
            var metadata = new MusicMetadataDto { Title = beat.Title, UserId = userId, Bpm = beat.Bpm, Genre = beat.Genre };
            var metadataJson = JsonSerializer.Serialize(metadata);


            // Add file
            form.Add(fileContent, "file", Path.GetFileName(filePath));


            // Add serialized DTO as a string field
            form.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "musicMetadataDto");

            // Send request
            var response = await client.PostAsync(serverUrlLocalHost, form);
            var responseString = await response.Content.ReadAsStringAsync();
            var res = JsonSerializer.Deserialize<FileuploadResponseDto>(responseString);
            if (string.IsNullOrEmpty(res.message)) return false;

            return true;

        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return false;

        }
    }


}


