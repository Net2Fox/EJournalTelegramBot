namespace EJournalTelegramBot.Util;

public static class CallbackData
{
    private const string Sep = ":";
    
    public const string MainMenuAction = "MM";
    public const string ChooseCourseAction = "CCA";
    public const string SelectAction = "SEL";
    public const string PageAction = "PAGE";

    public const string ChooseCoursePrefix = "CCP";
    public const string TeacherPrefix = "TCH";
    public const string RoomPrefix = "RM";
    public const string GroupPrefix = "GRP";

    public static string Select(string prefix, string value) => $"{prefix}{Sep}{SelectAction}{Sep}{value}";
    
    public static string Page(string prefix, int offset) => $"{prefix}{Sep}{PageAction}{Sep}{offset}";
    
    public static string Action(string action) => $"_{Sep}{action}{Sep}_";

    public static (string Prefix, string Action, string Value) Parse(string callbackData)
    {
        var parts = callbackData.Split(Sep, 3);
        return (parts[0], parts[1], parts[2]);
    }
}