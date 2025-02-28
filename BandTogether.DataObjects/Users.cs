namespace BandTogether;

public class user
{
    public Guid id { get; set; }
    public string name { get; set; } = "";
    public userPreferences preferences { get; set; } = new userPreferences();
    public DateTime? lastAccess { get; set; }
    public bool enabled { get; set; }
    public DateTime? created { get; set; }
    public DateTime? cpdated { get; set; }
}

public class userPreferences
{
    public bool autoFollow { get; set; } = true;
    public string cultureCode { get; set; } = "en-US";
    public bool editMode { get; set; }
    public bool hideChords { get; set; } = false;
    public string settingsPreviewSize { get; set; } = "m";
    public string settingsTab { get; set; } = "";
    public string settingsTabFormat { get; set; } = "";
    public bool showExport { get; set; } = false;
    public bool showMessaging { get; set; } = false;
    public int slideshowThumbnailColumns { get; set; } = 4;
    public List<sheetMusicPreference> sheetMusicPreferences { get; set; } = new List<sheetMusicPreference>();
    public List<songPreferences> songPreferences { get; set; } = new List<songPreferences>();
    public string? showPreview { get; set; } = "m";
    public string theme { get; set; } = "";
    public int zoom { get; set; } = 100;
    public double layoutPanelLeft { get; set; } = 20;
    public double layoutPanelSetList { get; set; } = 50;
    public List<Guid> openSongBooks { get; set; } = new List<Guid>();
}