using BlazorMaui.Repositories.Interfaces;
using BlazorMaui.Services.Interfaces;
using Shared.Dtos;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlazorMaui.Repositories;

class BeatsRepositoryRemote : IBeatsRepository
{
    private readonly IAuthService _auth;
    private List<Beat> beats = new();

    public BeatsRepositoryRemote(IAuthService auth)
    {
        _auth = auth;
    }
    public async Task AddBeatAsync(Beat beat, int userId)
    {
        string filepath = beat.AudioUrl;
        string serverUrl = "https://localhost:5106/api/files/uploadfile";

        using var client = new HttpClient();
        using var form = new MultipartFormDataContent();
        try
        {
            //var token = await _auth.GetTokenAsync();
            var filecontent = new ByteArrayContent(await File.ReadAllBytesAsync(filepath));

            // add from file content headers
            filecontent.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

            var metadata = new MusicMetadataDto
            {
                Title = beat.Title,
                Bpm = beat.Bpm,
                Genre = beat.Genre,
                UserId = userId,

            };
            var metadataJson = JsonSerializer.Serialize(metadata);
            // add file to form
            form.Add(filecontent, "file", Path.GetFileName(filepath));
            // add metadata to form
            form.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "musicmetadataDto");

            var res = await client.PostAsync(serverUrl, form);
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var resObj = JsonSerializer.Deserialize<FileuploadResponseDto>(json);

            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
        }
    }
    public async Task<IEnumerable<Beat>> GetBeatsAsync()
    {
        string serverUrl = "https://localhost:5106/api/audio/getall";
        using var client = new HttpClient();
        try
        {

            var token = await _auth.GetTokenAsync();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync(serverUrl);
            var json = await res.Content.ReadAsStringAsync();
            var filesMetadata = JsonSerializer.Deserialize<List<MusicMetadataDto>>
                (json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (filesMetadata == null || filesMetadata.Count == 0) return new List<Beat>();
            beats.Clear();

            foreach (var file in filesMetadata)
            {
                var b = new Beat
                {
                    Id = file.Id,
                    Title = file.Title,
                    Bpm = file.Bpm,
                    Genre = file.Genre,
                    AudioUrl = file.FilePath
                };
                beats.Add(b);
            }

            return beats.AsEnumerable();


        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
            return beats.AsEnumerable();
        }
    }

    public Task RemoveBeatAsync(Beat beat)
    {
        throw new NotImplementedException();
    }
    public Task UpdateBeatAsync(Beat oldBeats, Beat newBeat)
    {
        throw new NotImplementedException();
    }
}
