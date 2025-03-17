using BlazorMaui.Helpers;
using BlazorMaui.Repositories.Interfaces;
using BlazorMaui.Services.Interfaces;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMaui.Repositories;
public class BeatsLocalRepository : IBeatsRepository
{
    private string jsonPath = Helpers.AudioConstants.audioMetadataJsonFilepath;
    private List<Beat> beats;
    public async Task AddBeatAsync(Beat beat, int userId)
    {
        string extension = Path.GetExtension(beat.AudioUrl).ToLower();
        if (string.IsNullOrEmpty(beat.AudioUrl) ||
            !AudioConstants.SupportedAudioExtensions.Contains(extension))
            return;

        if (!beats.Any(b => b.AudioUrl.Contains(beat.AudioUrl)))
        {
            string localPath = Path.Combine(FileSystem.AppDataDirectory, beat.Title);
            beats.Add(beat);
            File.WriteAllBytes(localPath, File.ReadAllBytes(beat.AudioUrl));
        }
    }

    public async Task<IEnumerable<Beat>> GetBeatsAsync()
    {
        if (!File.Exists(jsonPath))
        {
            File.WriteAllText(jsonPath, "[]");
        }
        try
        {

            string json = File.ReadAllText(jsonPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            beats = JsonSerializer.Deserialize<List<Beat>>(json, options) ?? new List<Beat>();

            var files = Directory.GetFiles(FileSystem.AppDataDirectory).ToList();

            // remove deleted files from JSON
            beats.RemoveAll(a => !files.Contains(a.AudioUrl));

            // add new files that are not in JSON
            foreach (var filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();

                if (!AudioConstants.SupportedAudioExtensions.Contains(extension))
                    continue;

                if (!beats.Any(a => a.AudioUrl == filePath))
                {
                    string title = Path.GetFileNameWithoutExtension(filePath);
                    beats.Add(new Beat { AudioUrl = filePath, Title = title });
                }
            }

            File.WriteAllText(jsonPath, JsonSerializer.Serialize(beats, options));
            return beats.AsEnumerable();

        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
            return beats.AsEnumerable();
        }

    }

    public async Task RemoveBeatAsync(Beat beat)
    {
        string filePath = beat.AudioUrl;
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
        }
    }

    public async Task UpdateBeatAsync(Beat oldBeat, Beat newBeat)
    {
        try
        {
            if (!File.Exists(jsonPath)) File.WriteAllText(jsonPath, "[]");

            // if filename is unchanged, write other changes then return
            if (oldBeat.AudioUrl == newBeat.AudioUrl)
            {
                var oldFromUploads = beats.FirstOrDefault(f => f.AudioUrl == oldBeat.AudioUrl);
                oldFromUploads.Bpm = newBeat.Bpm;
                oldFromUploads.Genre = newBeat.Genre;
                oldFromUploads.Plays = newBeat.Plays;

                var json = JsonSerializer.Serialize(beats);
                File.WriteAllText(jsonPath, json);
                return;
            }

            // if file name changes move the file content to new file
            File.Move(oldBeat.AudioUrl, newBeat.AudioUrl);
            var beatToUpdate = beats.FirstOrDefault(b => b.AudioUrl == oldBeat.AudioUrl);
            if (beatToUpdate != null)
            {
                beatToUpdate.Title = newBeat.Title;
                beatToUpdate.Bpm = newBeat.Bpm;
                beatToUpdate.Genre = newBeat.Genre;
                beatToUpdate.AudioUrl = newBeat.AudioUrl;
            }


        }
        catch (Exception e)
        {

            Debug.WriteLine(e.Message);
        }
    }


}
