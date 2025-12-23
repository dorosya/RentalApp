namespace CinemaRentalCourseworkApp.Services;

public static class IdGenerator
{
    public static int NextId<T>(IEnumerable<T> items, Func<T, int> selector)
    {
        var max = 0;
        foreach (var item in items)
        {
            var id = selector(item);
            if (id > max) max = id;
        }
        return max + 1;
    }
}
