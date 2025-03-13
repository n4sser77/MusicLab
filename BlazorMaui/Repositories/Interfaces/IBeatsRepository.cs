namespace BlazorMaui.Repositories.Interfaces
{
    public interface IBeatsRepository
    {
        Task<IEnumerable<Beat>> GetBeatsAsync();
        Task UpdateBeatAsync(Beat oldBeats, Beat newBeat);
        Task AddBeatAsync(Beat beat, int userId);
        Task RemoveBeatAsync(Beat beat);
    }
}
