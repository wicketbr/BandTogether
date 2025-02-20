namespace BandTogether;

public enum previewSize
{
    off,
    extraSmall,
    small,
    medium,
    large,
    extraLarge
}

public class messageStyle
{
    public string? fontFamily { get; set; } = "Patua One";
    public int? fontSize { get; set; } = 55;
}

public class viewStyle
{
    public string? background { get; set; } = "#000000";
    public string? backgroundType { get; set; } = "color";
    public string? headerDisplay { get; set; } = "first";
    public string? headerFormat { get; set; } = "{title}";
    public double? headerOffset { get; set; } = 0.85;
    public string? footerDisplay { get; set; } = "first";
    public string? footerFormat { get; set; } = "{artist}";
    public double? footerOffset { get; set; } = 1.0;
    public textStyle? lyricsStyle { get; set; } = new textStyle {
        fontColor = "#ffffff",
        fontFamily = "Patua One",
        fontShadowColor = "#000000",
        fontShadowOffsetX = 20,
        fontShadowOffsetY = 20,
        fontLineHeight = 1.0,
        fontOutlineColor = "#000000",
        fontOutlineWidth = 50,
        fontShadow = true,
        fontShadowBlur = 35,
        fontSize = 55,
        textAlign = "center",
        verticalAlign = "middle",
        opacity = 1.0,
    };
    public textStyle? headerStyle { get; set; } = new textStyle {
        fontColor = "#ffffff",
        fontFamily = "Parisienne",
        fontShadowColor = "#000000",
        fontShadowOffsetX = 20,
        fontShadowOffsetY = 20,
        fontLineHeight = 1.0,
        fontOutlineWidth = 0,
        fontShadow = true,
        fontShadowBlur = 65,
        fontSize =60,
        textAlign = "center",
        opacity = 1.0,
    };
    public textStyle? footerStyle { get; set; } = new textStyle {
        fontColor = "#ffffff",
        fontFamily = "Josefin Slab",
        fontShadowColor = "#000000",
        fontShadowOffsetX = 20,
        fontShadowOffsetY = 20,
        fontLineHeight = 1.0,
        fontOutlineColor = "#000000",
        fontOutlineWidth = 5,
        fontShadow = true,
        fontShadowBlur = 35,
        fontSize = 55,
        textAlign = "center",
        opacity = 1.0,
    };
    public bool? showPreviousLyrics { get; set; }
    public double? previousLyricsOpacity { get; set; }
    public double? previousLyricsOpacityStep { get; set; }
    public bool? showUpcomingLyrics { get; set; } = true;
    public double? upcomingLyricsOpacity { get; set; } = 0.3;
    public double? upcomingLyricsOpacityStep { get; set; } = 0.12;
}

public class textStyle
{
    public bool? fontBold { get; set; }
    public string? fontColor { get; set; }
    public string? fontFamily { get; set; }
    public bool? fontItalic { get; set; }
    public double? fontLineHeight { get; set; }
    public string? fontOutlineColor { get; set; }
    public int? fontOutlineWidth { get; set; }
    public bool? fontShadow { get; set; }
    public string? fontShadowColor { get; set; }
    public int? fontShadowOffsetX { get; set; }
    public int? fontShadowOffsetY { get; set; }
    public int? fontShadowBlur { get; set; }
    public int? fontSize { get; set; }
    public string? textAlign { get; set; }
    public string? verticalAlign { get; set; }
    public double? opacity { get; set; }
}

