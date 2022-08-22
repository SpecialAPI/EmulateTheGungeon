using System;

public class AutocompletionSettings
{
    public Func<int, string, string[]> Match;
    public object represented;
    public AutocompletionSettings(Func<string, string[]> match)
    {
        Match = (int index, string key) => index == 0 ? match(key) : new string[0];
        represented = Activator.CreateInstance(GetMTGType("AutocompletionSettings"), match);
    }

    public AutocompletionSettings(Func<int, string, string[]> match)
    {
        Match = match;
        represented = Activator.CreateInstance(GetMTGType("AutocompletionSettings"), match);
    }

    public static bool MatchContains = true;
}

public static class StringAutocompletionExtensions
{
    public static bool AutocompletionMatch(this string text, string match)
    {
        if ((bool)GetStaticMTGField("AutocompletionSettings", "MatchContains"))
        {
            return text.Contains(match);
        }
        else
        {
            return text.StartsWith(match);
        }
    }
}