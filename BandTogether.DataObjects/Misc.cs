namespace BandTogether;

public class appSettings
{
    public List<string> availableFonts { get; set; } = new List<string>();
    public string? ccliNumber { get; set; }
    public bool churchMode { get; set; }
    public bool enableProjectionMode { get; set; } = true;
    public string? lastSetList { get; set; }
    public messageStyle? messageStyleScreen { get; set; } = new messageStyle {
        fontFamily = "Patua One",
        fontSize = 45,
    };
    public messageStyle? messageStyleTablet { get; set; } = new messageStyle {
        fontFamily = "Patua One",
        fontSize = 45,
    };
    public string? projectionModeAspectRatio { get; set; } = "16:9";
    public bool showNonStandardKeys { get; set; }
    public viewStyle projectionStyle { get; set; } = new viewStyle();
    public int? transitionSpeed { get; set; } = 500;

}

public class booleanResponse
{
    public List<string> messages { get; set; } = new List<string>();
    public bool result { get; set; }
}

public class dataLoader
{
    public List<string> backgrounds { get; set; } = new List<string>();
    public List<song> defaultSongs { get; set; } = new List<song>();
    public List<string> fontWoffFiles { get; set; } = new List<string>();
    public List<string> images { get; set; } = new List<string>();
    public List<string> installedFonts { get; set; } = new List<string>();
    public DateOnly released { get; set; }
    public List<Language> languages { get; set; } = new List<Language>();
    public setList setList { get; set; } = new setList();
    public appSettings settings { get; set; } = new appSettings();
    public List<setListFile> setListFilenames { get; set; } = new List<setListFile>();
    public List<sheetMusic> sheetMusic { get; set; } = new List<sheetMusic>();
    public List<songBook> songBooks { get; set; } = new List<songBook>();
    public List<user> users { get; set; } = new List<user>();
    public string version {  get; set; } = "";
    public List<string> videos { get; set; } = new List<string>();
}

public class fileItem
{
    public string fileName { get; set; } = "";
    public string folder { get; set; } = "";
    public Byte[]? value { get; set; }
}

public class keyboardEvent
{
    public string key { get; set; } = "";
    public string code { get; set; } = "";
    public bool ctrlKey { get; set; }
    public bool shiftKey { get; set; }
    public bool altKey { get; set; }
    public bool metaKey { get; set; }
}

public class keyMatrix
{
    public string key { get; set; } = "";
    public bool preferSharp { get; set; }
    public List<string> items { get; set; } = new List<string>();
}

public class message
{
    public bool active { get; set; }
    public string target { get; set; } = "";
    public string text { get; set; } = "";
    public string style { get; set; } = "";
}

public class messages
{
    public message screenMessage { get; set; } = new message { target = "screen", style = "red" };
    public message tabletMessage { get; set; } = new message { target = "tablet", style = "red" };
}

public class screenMessage
{
    // screen or tablet
    public string type { get; set; } = "";
    public string message { get; set; } = "";
    public string theme { get; set; } = "";
}

public class sheetMusic
{
    public string title { get; set; } = "";
    public List<string> parts { get; set; } = new List<string>();
}

public class simplePost
{
    public string? singleItem { get; set; }
    public List<string> Items { get; set; } = new List<string>();
}

public class simpleResponse
{
    public bool result { get; set; }
    public string? message { get; set; }
}

