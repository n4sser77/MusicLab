namespace BlazorMaui.Helpers;
public static class AudioConstants
{
    public static readonly HashSet<string> SupportedAudioExtensions = new()
        {
    ".mp3", ".wav", ".aac", ".ogg", ".flac", ".m4a", ".wma", ".opus", ".aiff"
        };
    public static string audioMetadataJsonFilepath = Path.Combine(FileSystem.AppDataDirectory, "audio_metadata.json");
    public static string playlistJsonFilepath = Path.Combine(FileSystem.AppDataDirectory, "playlist_metadata.json");
}