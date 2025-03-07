﻿
using System.Text.Json;
namespace BlazorMaui.Services.Interfaces;
public interface IBeatsService
{
    public List<Beat> UploadedAudio { get; set; }

    public Beat FileToEdit { get; set; }

    public void LoadUploaded();

    public void UploadAudio(string fullpath, string title);
    public Task<bool> PublishAudio(Beat beat, int userId);

    public void UpdateAudio(Beat newBeat, Beat oldBeat);


}