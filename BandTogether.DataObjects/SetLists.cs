using System.Text.Json.Serialization;

namespace BandTogether;


public class setList
{
    public string? fileName { get; set; }
    public Guid? activeItem { get; set; }
    public Guid? selectedItem { get; set; }
    public int? activeItemPart { get; set; }
    public List<setListItem> items { get; set; } = new List<setListItem>();
    public string? name { get; set; }
    public bool? saveRequired { get; set; }
}

public class setListFile
{
    public string fileName { get; set; } = "";
    public string name { get; set; } = "";
}

    public class setListItem
{
    public Guid id { get;set; } = Guid.NewGuid();
    public string type { get; set; } = setListItemType.unknown;
    public object? item { get; set; }
    public string? itemJson { get; set; }
    public string? afterItemOption { get; set; }
    public string? transpose { get; set; }
    public List<int>? disabledElements { get; set; }
}

public static class setListAfterItemOption
{
    public const string blankScreen = "blank";
    public const string emptySlide = "emptyslide";
    public const string nextItem = "";
}

public static class setListItemType
{
    public const string audio = "audio";
    public const string clock = "clock";
    public const string countdown = "countdown";
    public const string image = "image";
    public const string sheetmusic = "sheetmusic";
    public const string slideshow = "slideshow";
    public const string song = "song";
    public const string unblank = "unblank";
    public const string unknown = "";
    public const string video = "video";
    public const string youTube = "youtube";
}