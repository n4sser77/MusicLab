namespace BlazorMaui
{
    public static class DurationTimeHelper
    {

        public static (int, int) CalculateTotalSecoundsToMinutes(double totalSecounds)
        {
            var totalMin = Math.Floor((totalSecounds / 60));
            var remaingingSec = totalSecounds - totalMin * 60;
            return ((int)totalMin, (int)remaingingSec);
        }
    }
}