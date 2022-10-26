namespace IssueTracker.Utils;

public static class ListFormatter
{
    public static string Formatter(List<string> list)
    {
        var lastItem = list.Last();
        list.Insert(list.IndexOf(lastItem), "or");
        var joined = string.Join(", ", list.ToArray());
        return joined.Remove(joined.Length - lastItem.Length - 2, 1);
    }
}