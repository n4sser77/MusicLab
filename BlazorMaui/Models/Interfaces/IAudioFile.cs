public interface IAudioFile
{
    public Guid Guid { get; set; }
    public string Title { get; set; }
    public int Bpm { get; set; }
    public string Genre { get; set; }
    public int Plays { get; set; }
    public string AudioUrl { get; set; }

}