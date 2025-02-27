namespace BandTogether;

public class audioItem
{
    public string? filename { get; set; }
    public int volume { get; set; } = 100;
}

public class audioItemCurrentTime
{
    public double currentTime { get; set; }
    public double totalTime { get; set; }
    public double percentage { get; set; }
}

public class clockItem
{
    public bool showSeconds { get; set; }
    public viewStyle style { get; set; } = new viewStyle {
        background = "#000",
        backgroundType = backgroundType.color,
        footerDisplay = null,
        footerFormat = null,
        footerOffset = null,
        footerStyle = null,
        headerDisplay = null,
        headerFormat = null,
        headerOffset = null,
        headerStyle = null,
        lyricsStyle = new textStyle {
            fontColor = "#000",
            fontOutlineColor = "#fff",
            fontShadowColor = "#000",
            fontShadowBlur = 60,
            fontFamily = "Tahoma",
            fontSize = 140,
            verticalAlign = "middle",
            fontShadowOffsetX = 15,
            fontShadowOffsetY = 15,
            fontBold = true,
            fontOutlineWidth = 200,
        },
        previousLyricsOpacity = null,
        previousLyricsOpacityStep = null,
        showPreviousLyrics = null,
        showUpcomingLyrics = null,
        upcomingLyricsOpacity = null,
        upcomingLyricsOpacityStep = null,
    };
    public int? transitionSpeed { get; set; } = 500;
}

public class countdownItem
{
    public string countdownType { get; set; } = "seconds";
    public double? seconds { get; set; } = 300;
    public TimeOnly? toTime { get; set; }
    public viewStyle style { get; set; } = new viewStyle {
        background = "#000",
        backgroundType = backgroundType.color,
        footerDisplay = null,
        footerFormat = null,
        footerOffset = null,
        footerStyle = null,
        headerDisplay = null,
        headerFormat = null,
        headerOffset = null,
        headerStyle = null,
        lyricsStyle = new textStyle {
            fontColor = "#000",
            fontOutlineColor = "#fff",
            fontShadowColor = "#000",
            fontShadowBlur = 60,
            fontFamily = "Tahoma",
            fontSize = 250,
            verticalAlign = "middle",
            fontShadowOffsetX = 15,
            fontShadowOffsetY = 15,
            fontBold = true,
            fontOutlineWidth = 200,
        },
        previousLyricsOpacity = null,
        previousLyricsOpacityStep = null,
        showPreviousLyrics = null,
        showUpcomingLyrics = null,
        upcomingLyricsOpacity = null,
        upcomingLyricsOpacityStep = null,
    };
    public int? transitionSpeed { get; set; } = 500;
}

public class imageItem
{
    public string? filename { get; set; }
    public string? name { get; set; }
    public int? transitionSpeed { get; set; } = 500;
}

public class sheetMusicItem
{
    public string? title { get; set; }
}

public class slideshowItem
{
    public string folder { get; set; } = "";
    public List<string>? images { get; set; }
    public int? transitionSpeed { get; set; } = 500;
}

public class videoItem
{
    public string? filename { get; set; }
    public bool loop { get; set; }
    public bool muteInMainWindow { get; set; }
    public bool muteOnScreens { get; set; }
    public int volume { get; set; } = 100;
    public int? transitionSpeed { get; set; } = 500;
}

public class youTubeItem
{
    public string name { get; set; } = "";
    public string videoId { get; set; } = "";
    public bool muteInMainWindow { get; set; }
    public bool muteOnScreens { get; set; }
    public int volume { get; set; } = 100;
    public int? transitionSpeed { get; set; } = 500;
}

public static class backgroundType
{
    public const string color = "color";
    public const string image = "image";
    public const string slideshow = "slideshow";
    public const string unknown = "";
    public const string video = "video";
}