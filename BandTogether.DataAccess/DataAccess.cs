using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using SkiaSharp;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BandTogether;

public interface IDataAccess
{
    booleanResponse AddSong(song song);
    booleanResponse AddSongBook(string? name);
    string BasePath { get; }
    Task<booleanResponse> ConvertPowerPointToSlides(fileItem file);
    List<song> DefaultSongs { get; }
    booleanResponse DeleteSong(song song);
    booleanResponse DeleteSongBook(string? filename);
    booleanResponse DeleteUser(Guid userId);
    List<string> GetAudioFiles(bool UseCache = true);
    byte[]? GetBackground(string filename);
    List<string> GetBackgrounds(bool UseCache = true);
    dataLoader GetDataLoader();
    string GetDocumentation();
    List<string> GetImages(bool UseCache = true);
    List<Language> GetLanguages();
    byte[]? GetMediaItem(string folder, string filename);
    appSettings GetSettings();
    setList GetSetList(string filename);
    List<setListFile> GetSetListFilenames(bool UseCache = true);
    List<setList> GetSetLists();
    List<sheetMusic> GetSheetMusic(bool UseCache = true);
    slideshowItem GetSlideshow(string folder);
    List<string> GetSlideshows(bool UseCache = true);
    List<songBook> GetSongBooks();
    user? GetUser(Guid id);
    user? GetUser(string name);
    List<user> GetUsers();
    List<string> GetVideos(bool UseCache = true);
    DateOnly Released { get; }
    booleanResponse SaveSettings(appSettings settings);
    booleanResponse SaveSetList(setList setlist);
    booleanResponse SaveSong(song song);
    booleanResponse SaveSongBook(songBook songbook);
    booleanResponse SaveUser(user user);
    booleanResponse SaveUserPreferences(Guid userId, userPreferences userPreferences);
    booleanResponse SaveUserPreferences(string name, userPreferences userPreferences);
    booleanResponse SetUserViewPreferences(user user);
    booleanResponse SetUserHideChords(Guid userId, bool hideChords);
    booleanResponse SetUserHideChords(string name, bool hideChords);
    booleanResponse SetUserTheme(Guid userId, string theme);
    booleanResponse SetUserTheme(string name, string theme);
    string Version { get; }
}

public class DataAccess : IDataAccess
{
    private string _applicationPath = String.Empty;
    private string _basePath = String.Empty;
    private DateOnly _released = DateOnly.FromDateTime(Convert.ToDateTime("3/1/2025"));
    private IServiceProvider? _serviceProvider;
    private string _version = "1.0.3";

    private string _folderAudio = String.Empty;
    private string _folderBackgrounds = String.Empty;
    private string _folderImages = String.Empty;
    private string _folderLanguages = String.Empty;
    private string _folderSetLists = String.Empty;
    private string _folderSheetMusic = String.Empty;
    private string _folderSlideshows = String.Empty;
    private string _folderSongBooks = String.Empty;
    private string _folderUsers = String.Empty;
    private string _folderVideos = String.Empty;

    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
    };

    private static JsonSerializerOptions _jsonOptionsIndented = new JsonSerializerOptions {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    private static JsonSerializerOptions _jsonOptionsPreserveLanguage = new JsonSerializerOptions {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    // Websafe fonts, plus our installed fonts.
    private List<string> _webSafeFonts = new List<string> {
        "Arial",
        "Brush Script MT",
        "Courier New",
        "Garamond",
        "Georgia",
        "Tahoma",
        "Trebuchet MS",
        "Times New Roman",
        "Verdana"
    };

    public DataAccess(string applicationPath, string basePath, IServiceProvider? serviceProvider = null)
    {
        _applicationPath = applicationPath;
        _basePath = basePath;
        _serviceProvider = serviceProvider;

        DataAccessCreateDefaultFolders();
    }

    protected void DataAccessCreateDefaultFolders()
    {
        if (!System.IO.Directory.Exists(_basePath)) {
            System.IO.Directory.CreateDirectory(_basePath);
        }

        _folderAudio = System.IO.Path.Combine(_basePath, "Audio");
        _folderBackgrounds = System.IO.Path.Combine(_basePath, "Backgrounds");
        _folderImages = System.IO.Path.Combine(_basePath, "Images");
        _folderLanguages = System.IO.Path.Combine(_basePath, "Languages");
        _folderSetLists = System.IO.Path.Combine(_basePath, "SetLists");
        _folderSheetMusic = System.IO.Path.Combine(_basePath, "SheetMusic");
        _folderSlideshows = System.IO.Path.Combine(_basePath, "Slideshows");
        _folderSongBooks = System.IO.Path.Combine(_basePath, "SongBooks");
        _folderUsers = System.IO.Path.Combine(_basePath, "Users");
        _folderVideos = System.IO.Path.Combine(_basePath, "Videos");

        if (!GlobalSettings.StartupComplete) {
            if (!System.IO.Directory.Exists(_folderAudio)) {
                System.IO.Directory.CreateDirectory(_folderAudio);
            }

            if (!System.IO.Directory.Exists(_folderBackgrounds)) {
                System.IO.Directory.CreateDirectory(_folderBackgrounds);
                CreateDefaultBackground();
            }

            if (!System.IO.Directory.Exists(_folderImages)) {
                System.IO.Directory.CreateDirectory(_folderImages);
                CreateDefaultImage();
            }

            if (!System.IO.Directory.Exists(_folderLanguages)) {
                System.IO.Directory.CreateDirectory(_folderLanguages);
            }

            if (!System.IO.Directory.Exists(_folderSheetMusic)) {
                System.IO.Directory.CreateDirectory(_folderSheetMusic);
                CreateDefaultSheetMusic();
            }

            if (!System.IO.Directory.Exists(_folderSlideshows)) {
                System.IO.Directory.CreateDirectory(_folderSlideshows);
                CreateDefaultSlideshow();
            }

            if (!System.IO.Directory.Exists(_folderSongBooks)) {
                System.IO.Directory.CreateDirectory(_folderSongBooks);
            }

            if (!System.IO.Directory.Exists(_folderUsers)) {
                System.IO.Directory.CreateDirectory(_folderUsers);
            }

            if (!System.IO.Directory.Exists(_folderVideos)) {
                System.IO.Directory.CreateDirectory(_folderVideos);
            }

            if (!System.IO.Directory.Exists(_folderSetLists)) {
                System.IO.Directory.CreateDirectory(_folderSetLists);
                CreateDefaultSetList();
            }

        }

        GlobalSettings.StartupComplete = true;
    }

    public booleanResponse AddSong(song song)
    {
        var output = new booleanResponse();

        var songbooks = GetSongBooks();
        var sb = songbooks.FirstOrDefault(x => x.id == song.songBookId);

        if (sb != null) {
            sb.songs.Add(song);
            output = SaveSongBook(sb);
        }

        return output;
    }

    public booleanResponse AddSongBook(string? name)
    {
        var output = new booleanResponse();

        if (!String.IsNullOrWhiteSpace(name)) {
            // Make sure the new name is unique.
            var songbooks = GetSongBooks();
            var existing = songbooks.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
            if (existing != null) {
                output.messages.Add(Text.SongBookExists);
            } else {
                var now = DateTime.Now;

                output = SaveSongBook(new songBook { 
                    created = now,
                    fileName = name + ".json",
                    id = Guid.NewGuid(),
                    name = name,
                    songs = new List<song>(),
                    updated = now,
                });
            }
        }


        return output;
    }

    public string BasePath => _basePath;

    private viewStyle CleanStyle(viewStyle style, bool usesHeaderStyle, bool usesFooterStyle)
    {
        if (String.IsNullOrWhiteSpace(style.background)) {
            style.background = null;
        }

        if (String.IsNullOrWhiteSpace(style.backgroundType)) {
            style.backgroundType = null;
        }

        if (String.IsNullOrWhiteSpace(style.headerDisplay)) {
            style.headerDisplay = null;
        }

        if (String.IsNullOrWhiteSpace(style.headerFormat)) {
            style.headerFormat = null;
        }

        if (style.headerOffset == 0) {
            style.headerOffset = null;
        }

        if (String.IsNullOrWhiteSpace(style.footerDisplay)) {
            style.footerDisplay = null;
        }

        if (String.IsNullOrWhiteSpace(style.footerFormat)) {
            style.footerFormat = null;
        }

        if (style.footerOffset == 0) {
            style.footerOffset = null;
        }

        if (style.showPreviousLyrics != true) {
            style.showPreviousLyrics = null;
        }

        if (style.previousLyricsOpacity == 0) {
            style.previousLyricsOpacity = null;
        }

        if (style.previousLyricsOpacityStep == 0) {
            style.previousLyricsOpacityStep = null;
        }

        if (style.showUpcomingLyrics != true) {
            style.showUpcomingLyrics = null;
        }

        if (style.upcomingLyricsOpacity == 0) {
            style.upcomingLyricsOpacity = null;
        }

        if (style.upcomingLyricsOpacityStep == 0) {
            style.upcomingLyricsOpacityStep = null;
        }

        style.lyricsStyle = CleanTextStyle(style.lyricsStyle);

        if (!usesHeaderStyle) {
            style.headerStyle = null;
        } else {
            style.headerStyle = CleanTextStyle(style.headerStyle);
        }

        if (!usesFooterStyle) {
            style.footerStyle = null;
        } else {
            style.footerStyle = CleanTextStyle(style.footerStyle);
        }

        return style;
    }

    private textStyle? CleanTextStyle(textStyle? style)
    {
        if (style != null) {
            if (style.fontBold != true) {
                style.fontBold = null;
            }

            if (String.IsNullOrWhiteSpace(style.fontColor)) {
                style.fontColor = null;
            }

            if (String.IsNullOrWhiteSpace(style.fontFamily)) {
                style.fontFamily = null;
            }

            if (style.fontItalic != true) {
                style.fontItalic = null;
            }

            if (style.fontLineHeight == 0) {
                style.fontLineHeight = null;
            }

            if (String.IsNullOrWhiteSpace(style.fontOutlineColor)) {
                style.fontOutlineColor = null;
            }

            if (style.fontOutlineWidth == 0) {
                style.fontOutlineWidth = null;
            }

            if (style.fontShadow != true) {
                style.fontShadow = null;
            }

            if (String.IsNullOrWhiteSpace(style.fontShadowColor)) {
                style.fontShadowColor = null;
            }

            if (style.fontShadowOffsetX == 0) {
                style.fontShadowOffsetX = null;
            }

            if (style.fontShadowOffsetY == 0) {
                style.fontShadowOffsetY = null;
            }

            if (style.fontShadowBlur == 0) {
                style.fontShadowBlur = null;
            }

            if (style.fontSize == 0) {
                style.fontSize = null;
            }

            if (String.IsNullOrWhiteSpace(style.textAlign)) {
                style.textAlign = null;
            }

            if (String.IsNullOrWhiteSpace(style.verticalAlign)) {
                style.verticalAlign = null;
            }

            if (style.opacity == 1) {
                style.opacity = null;
            }
        }

        return style;
    }

    public async Task<booleanResponse> ConvertPowerPointToSlides(fileItem file)
    {
        var output = new booleanResponse();

        // First, save the file item to the root of the slideshows folder.
        if (file.value != null && !String.IsNullOrWhiteSpace(file.fileName)) {
            var powerpointFile = Path.Combine(_folderSlideshows, file.fileName);
            System.IO.File.WriteAllBytes(powerpointFile, file.value);

            var outputFolder = System.IO.Path.GetDirectoryName(powerpointFile);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(powerpointFile);

            if (!String.IsNullOrWhiteSpace(outputFolder)) { 
                try {
                    // First, get any running instances of PowerPoint.
                    Process[] previousProcesses = Process.GetProcessesByName("powerpnt");

                    outputFolder = System.IO.Path.Combine(outputFolder, fileName);

                    var pptApplication = new Microsoft.Office.Interop.PowerPoint.Application();

                    var pptPresentation = pptApplication.Presentations.Open(
                        powerpointFile, Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoFalse,
                        Microsoft.Office.Core.MsoTriState.msoTrue
                    );

                    // If the directory already exists delete it.
                    // It will get recreated, but this will remove any existing files.
                    if (Directory.Exists(outputFolder)) {
                        Directory.Delete(outputFolder, true);
                    }
                    Directory.CreateDirectory(outputFolder);

                    int slidesCount = pptPresentation.Slides.Count;
                    int counter = 0;
                    for (int i = 1; i <= slidesCount; i++) {
                        bool hiddenSlide = pptPresentation.Slides[i].SlideShowTransition.Hidden == Microsoft.Office.Core.MsoTriState.msoTrue;
                        if (!hiddenSlide) {
                            counter++;
                            string outputPath = Path.Combine(outputFolder, $"Slide{counter}.png");
                            pptPresentation.Slides[i].Export(outputPath, "png");
                        }
                    }

                    pptPresentation.Close();
                    pptApplication.Quit();

                    System.IO.File.Delete(powerpointFile);

                    output.result = true;

                    Process[] currentProcesses = Process.GetProcessesByName("powerpnt");

                    for(int i = 0; i < currentProcesses.Length; i++) {
                        // If this is the new process kill it.
                        if (!previousProcesses.Any(x => x.Id == currentProcesses[i].Id)) {
                            currentProcesses[i].Kill();
                        }
                    }

                } catch (Exception ex) {
                    output.messages.Add(ex.Message);
                    if (ex.InnerException != null && !String.IsNullOrWhiteSpace(ex.InnerException.Message)) {
                        output.messages.Add(ex.InnerException.Message);
                    }
                }

                await System.Threading.Tasks.Task.Delay(2000);
            }
        }

        return output;
    }

    private T? ConvertStaticClassToObject<T>(System.Type staticType)
    {
        //var output = default(T);
        var output = (T)Activator.CreateInstance(typeof(T));

        var propertyNames = GetStaticClassPublicStringProperties(staticType);

        foreach (var item in propertyNames) {
            var prop = staticType.GetProperty(item);
            if (prop != null) {
                var value = prop.GetValue(null);

                if (value != null && !String.IsNullOrWhiteSpace(value.ToString())) {
                    // Find the property in the output object.

                    var outputProperty = output?.GetType().GetProperty(item);
                    if (outputProperty != null) {
                        outputProperty.SetValue(output, value.ToString());
                    }
                }
            }
        }

        if (output == null) {
            return default(T);
        } else {
            return (T)output;
        }
    }

    private void CreateDefaultBackground()
    {
        var files = Directory.GetFiles(Path.Combine(_applicationPath, "wwwroot", "Sample Items", "Backgrounds"));
        foreach (var file in files) {
            System.IO.File.Copy(file, Path.Combine(_folderBackgrounds, System.IO.Path.GetFileName(file)));
        }
    }

    private void CreateDefaultImage()
    {
        var files = Directory.GetFiles(Path.Combine(_applicationPath, "wwwroot", "Sample Items", "Images"));
        foreach (var file in files) {
            System.IO.File.Copy(file, Path.Combine(_folderImages, System.IO.Path.GetFileName(file)));
        }
    }

    private void CreateDefaultSetList()
    {
        song? s = null;

        var songbooks = GetSongBooks();
        if (songbooks.Count > 0) {
            var songbook = songbooks.First();
            if (songbook != null && songbook.songs.Count > 0) {
                s = songbook.songs.First();
            }
        }

        sheetMusicItem? sm = null;
        var sheetmusic = GetSheetMusic();
        if (sheetmusic.Count > 0) {
            var item = sheetmusic.FirstOrDefault(x => x.parts.Count > 0);
            if (item != null) {
                sm = new sheetMusicItem { 
                    title = item.title,
                };
            }
        }

        var setlist = new setList {
            name = "Sample Set List",
            fileName = "Sample_Set_List",
            items = new List<setListItem> {
                new setListItem {
                    id = Guid.NewGuid(),
                    type = setListItemType.image,
                    item = new imageItem {
                        filename = "Welcome to the Show.jpg",
                        name = "Welcome Slide",
                        transitionSpeed = 500,
                    },
                },
                new setListItem {
                    id = Guid.NewGuid(),
                    type = setListItemType.clock,
                    item = new clockItem {
                        showSeconds = true,
                        style = new viewStyle {
                            background = "Blurry Lights.jpg",
                            backgroundType = backgroundType.image,
                            lyricsStyle = new textStyle {
                                fontColor = "#000",
                                fontFamily = "Tahoma",
                                fontSize = 140,
                                fontBold = true,
                                fontOutlineColor = "#fff",
                                fontOutlineWidth = 200,
                                fontShadow = true,
                                fontShadowColor = "#000",
                                fontShadowBlur = 60,
                                fontShadowOffsetX = 15,
                                fontShadowOffsetY = 15,
                                opacity = 0.9,
                                verticalAlign = "middle",
                            },
                        },
                        transitionSpeed = 500,
                    },
                },
                new setListItem {
                    id = Guid.NewGuid(),
                    type = setListItemType.countdown,
                    item = new countdownItem {
                        countdownType = "seconds",
                        seconds = 10,
                        style = new viewStyle {
                            background = "Blurry Lights.jpg",
                            backgroundType = backgroundType.image,
                            lyricsStyle = new textStyle {
                                fontBold = true,
                                fontColor = "#000",
                                fontFamily = "Tahoma",
                                fontOutlineColor = "#fff",
                                fontOutlineWidth = 200,
                                fontShadow = true,
                                fontShadowColor = "#000",
                                fontShadowOffsetX = 15,
                                fontShadowOffsetY = 15,
                                fontShadowBlur = 60,
                                fontSize = 250,
                                verticalAlign = "middle",
                            },
                        },
                        transitionSpeed = 500,
                    },
                },
                new setListItem {
                    id = Guid.NewGuid(),
                    type = setListItemType.countdown,
                    item = new countdownItem {
                        countdownType = "time",
                        toTime = new TimeOnly(9, 0),
                        style = new viewStyle {
                            background = "Blurry Lights.jpg",
                            backgroundType = backgroundType.image,
                            lyricsStyle = new textStyle {
                                fontBold = true,
                                fontColor = "#000",
                                fontFamily = "Tahoma",
                                fontOutlineColor = "#fff",
                                fontOutlineWidth = 200,
                                fontShadow = true,
                                fontShadowColor = "#000",
                                fontShadowOffsetX = 15,
                                fontShadowOffsetY = 15,
                                fontShadowBlur = 60,
                                fontSize = 220,
                                verticalAlign = "middle",
                            },
                        },
                        transitionSpeed = 500,
                    },
                },
                new setListItem {
                     id = Guid.NewGuid(),
                     type = setListItemType.slideshow,
                     item = new slideshowItem {
                        folder = "Sample Presentation",
                        transitionSpeed = 1200,
                     },
                },
            },
        };

        if (s != null) {
            setlist.items.Add(new setListItem {
                id = Guid.NewGuid(),
                type = setListItemType.song,
                item = new song {
                    id = s.id,
                    songBookId = s.songBookId,
                },
            });
        }

        if (sm != null) {
            setlist.items.Add(new setListItem { 
                id = Guid.NewGuid(),
                type = setListItemType.sheetmusic,
                item = sm,
            });
        }

        setlist.items.Add(new setListItem {
            id = Guid.NewGuid(),
            type = setListItemType.youTube,
            item = new youTubeItem {
                name = "Test YouTube Video",
                videoId = "dQw4w9WgXcQ",
                muteInMainWindow = false,
                muteOnScreens = true,
                volume = 10,
                transitionSpeed = 500,
            },
        });
        
        setlist.items.Add(new setListItem {
            id = Guid.NewGuid(),
            type = setListItemType.image,
            item = new imageItem {
                filename = "BandTogether.jpg",
                name = "Closing Slide",
                transitionSpeed = 500,
            },
        });

        SaveSetList(setlist);
    }

    private void CreateDefaultSheetMusic()
    {
        var sampleSheetMusicFolder = Path.Combine(_applicationPath, "wwwroot", "Sample Items", "SheetMusic");
        if (System.IO.Directory.Exists(sampleSheetMusicFolder)) {
            // Get all folders in this folder.
            var folders = Directory.GetDirectories(sampleSheetMusicFolder);
            foreach(var folder in folders) {
                var outputFolder = Path.Combine(_folderSheetMusic, System.IO.Path.GetFileName(folder));
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }

                // Get all the files in the source folder.
                var files = Directory.GetFiles(folder);
                foreach(var file in files) {
                    var fileName  = System.IO.Path.GetFileName(file);
                    var output = Path.Combine(outputFolder, fileName);
                    System.IO.File.Copy(file, output);
                }
            }
        }
    }

    private void CreateDefaultSlideshow()
    {
        var samplePresentationFolder = Path.Combine(_applicationPath, "wwwroot", "Sample Items", "Sample Presentation");
        if (System.IO.Directory.Exists(samplePresentationFolder)) {
            var files = Directory.GetFiles(samplePresentationFolder);
            if (files != null && files.Count() > 0) {
                var outputFolder = Path.Combine(_folderSlideshows, "Sample Presentation");
                if (!System.IO.Directory.Exists(outputFolder)) {
                    System.IO.Directory.CreateDirectory(outputFolder);
                }
                foreach (var file in files) {
                    var fileName = System.IO.Path.GetFileName(file);
                    var output = Path.Combine(outputFolder, fileName);
                    System.IO.File.Copy(file, output);
                }
            }
        }
    }

    public List<song> DefaultSongs
    {
        get {
            var now = DateTime.Now;

            var output = new List<song> { 
                new song {
                    id = Guid.NewGuid(),
                    title = "The Water is Wide",
                    artist = "Traditional",
                    key = "G",
                    tempo = "90",
                    timeSignature = "4/4",
                    copyright = "Public Domain",
                    created = now,
                    updated = now,
                    content =
                        "Verse" + Environment.NewLine +
                        "[G] The water is wide,  [C]  I can't cross [G] o'er," + Environment.NewLine +
                        "nor do I [Em] have [C] light wings to [D] fly." + Environment.NewLine +
                        "Build me a [Bm] boat that can carry [Em] two, [C]" + Environment.NewLine +
                        "and both shall [G] row,  [D] my love and [G] I." + Environment.NewLine +
                        "" + Environment.NewLine +
                        "Verse 2" + Environment.NewLine +
                        "A ship there [G] is,  [C] and sails the [G] sea," + Environment.NewLine +
                        "she's loaded [Em] deep, [C]  as deep can [D] be," + Environment.NewLine +
                        "But not so [Bm] deep as the love I'm [Em] in, [C]" + Environment.NewLine +
                        "and I know not [G] how [D]  I sink or [G] swim." + Environment.NewLine +
                        "" + Environment.NewLine +
                        "Verse 3" + Environment.NewLine +
                        "When love is [G] young, [C]  and love is [G] fine," + Environment.NewLine +
                        "it's like a [Em] gem [C] when first it's [D] new." + Environment.NewLine +
                        "But love grows [Bm] old and waxes [G] cold, [C]" + Environment.NewLine +
                        "and fades a[G]way [D] like the morning [G] dew." + Environment.NewLine +
                        "" + Environment.NewLine +
                        "Verse 4" + Environment.NewLine +
                        "[G] The water is wide,  [C]  I can't cross [G] o'er," + Environment.NewLine +
                        "nor do I [Em] have [C] light wings to [D] fly." + Environment.NewLine +
                        "Build me a [Bm] boat that can carry [Em] two, [C]" + Environment.NewLine +
                        "and both shall [G] row,  [D] my love and [G] I." + Environment.NewLine
                }
            };

            return output;
        }
    }

    public booleanResponse DeleteSong(song song)
    {
        var output = new booleanResponse();

        var songbooks = GetSongBooks();
        var sb = songbooks.FirstOrDefault(x => x.id == song.songBookId);
        if (sb != null) {
            sb.songs = sb.songs.Where(x => x.id != song.id).OrderBy(x => x.title).ThenBy(x => x.artist).ToList();
            SaveSongBook(sb);
            output.result = true;
        } else {
            output.messages.Add("Error Deleting Song: SongBook '" + song.songBookId.ToString() + "' Not Found");
        }

        CacheStore.Clear("song-" + song.songBookId.ToString() + "-" + song.id.ToString());

        return output;
    }

    public booleanResponse DeleteSongBook(string? filename)
    {
        var output = new booleanResponse();

        if (!String.IsNullOrWhiteSpace(filename)) {
            var file = Path.Combine(_folderSongBooks, filename);

            if (System.IO.File.Exists(file)) {
                // Rename the file to .deleted. If a file with that name already exists, delete it.
                var deletedFile = Path.Combine(_folderSongBooks, Tools.SafeFileName(filename) + ".json.deleted");
                if (System.IO.File.Exists(deletedFile)) {
                    System.IO.File.Delete(deletedFile);
                }

                System.IO.File.Move(file, deletedFile);

                output.result = true;

                CacheStore.ClearAll();
            } else {
                output.messages.Add("SongBook '" + filename + "' Not Found");
            }
        }

        return output;
    }

    public booleanResponse DeleteUser(Guid userId)
    {
        var output = new booleanResponse();

        var file = Path.Combine(_folderUsers, userId.ToString() + ".json");

        if (System.IO.File.Exists(file)) {
            // Rename the file to .deleted. If a file with that name already exists, delete it.
            var deletedFile = Path.Combine(_folderUsers, userId.ToString() + ".json.deleted");
            if (System.IO.File.Exists(deletedFile)) {
                System.IO.File.Delete(deletedFile);
            }

            System.IO.File.Move(file, deletedFile);

            output.result = true;

            CacheStore.Clear("user-" + userId.ToString());
        } else {
            output.messages.Add("User '" + userId.ToString() + "' Not Found");
        }

        return output;
    }

    private T? DeserializeObject<T>(string? SerializedObject, JsonSerializerOptions? options = null)
    {
        var output = default(T);
        if (!String.IsNullOrWhiteSpace(SerializedObject)) {
            try {
                if (options == null) {
                    options = _jsonOptions;
                }

                output = System.Text.Json.JsonSerializer.Deserialize<T>(SerializedObject, options);
            } catch { }
        }

        return output;
    }

    public List<string> GetAudioFiles(bool UseCache = true)
    {
        var output = new List<string>();

        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<string>>("audiofiles");
            if (cached != null) {
                return cached;
            }
        }

        var files = Directory.GetFiles(_folderAudio, "*.mp3");
        if (files != null && files.Count() > 0) {
            foreach (var file in files) {
                output.Add(Tools.GetFileName(file));
            }
        }

        CacheStore.SetCacheItem("audiofiles", output);

        return output;
    }

    public byte[]? GetBackground(string filename)
    {
        byte[]? output = null;

        var file = Path.Combine(_folderBackgrounds, filename);
        if (System.IO.File.Exists(file)) {
            output = System.IO.File.ReadAllBytes(file);
        }

        return output;
    }

    public List<string> GetBackgrounds(bool UseCache = true)
    {
        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<string>>("backgrounds");
            if (cached != null) {
                return cached;
            }
        }

        var extensions = Tools.ExtensionsForImages;
        extensions.AddRange(Tools.ExtensionsForVideos);

        var output = GetFilesByExtensions(_folderBackgrounds, extensions);

        if (!output.Any()) {
            CreateDefaultBackground();
            output.Add("BandTogether.jpg");
        }

        CacheStore.SetCacheItem("backgrounds", output);

        return output;
    }

    private string GetCultureNameFromCultureCode(string cultureCode)
    {
        string output = String.Empty;

        var ci = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.AllCultures);
        if (ci != null && ci.Any()) {
            var cc = ci.FirstOrDefault(x => x.Name.ToLower() == cultureCode.ToLower());
            if (cc != null) {
                output = cc.DisplayName;
            }
        }

        return output;
    }

    public dataLoader GetDataLoader()
    {
        var output = new dataLoader { 
            backgrounds = GetBackgrounds(),
            defaultSongs = DefaultSongs,
            fontWoffFiles = GetFontFiles(),
            images = GetImages(),
            installedFonts = GetInstalledFonts(),
            languages = GetLanguages(),
            released = _released,
            settings = GetSettings(),
            setListFilenames = GetSetListFilenames(),
            sheetMusic = GetSheetMusic(),
            songBooks = GetSongBooks(),
            users = GetUsers(),
            version = _version,
            videos = GetVideos(),
        };

        if (!String.IsNullOrWhiteSpace(output.settings.lastSetList) && output.setListFilenames.Any(x => x.fileName == output.settings.lastSetList)) {
            // See if this still exists.
            output.setList = GetSetList(output.settings.lastSetList);
        } else {
            if (output.setListFilenames.Count == 1 && output.setListFilenames.First().fileName.ToLower() == "sample_set_list.json") {
                output.setList = GetSetList("Sample_Set_List.json");
            }
        }

        return output;
    }

    public string GetDocumentation()
    {
        var output = String.Empty;



        return output;
    }

    public List<string> GetImages(bool UseCache = true)
    {
        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<string>>("images");
            if (cached != null) {
                return cached;
            }
        }

        var output = GetFilesByExtensions(_folderImages, Tools.ExtensionsForImages);
        if (!output.Any()) {
            CreateDefaultImage();
            output.Add("BandTogether.jpg");
        }

        CacheStore.SetCacheItem("images", output);

        return output;
    }

    private List<string> GetFilesByExtensions(string? folder, IEnumerable<string>? extensions)
    {
	    var output = new List<string>();
	
	    if (!String.IsNullOrWhiteSpace(folder)) {
		    if (extensions != null && extensions.Count() > 0) {
			    foreach(var ext in extensions) {
				    var filter = String.Empty;
				    if (ext.StartsWith("*.")) {
					    filter = ext;
				    } else if (ext.StartsWith(".")) {
					    filter = "*" + ext;
				    } else {
					    filter = "*." + ext;
				    }
			
				    var files = Directory.GetFiles(folder, filter);
				    if (files != null && files.Count() > 0) {
					    foreach(var file in files) {
                            output.Add(Tools.GetFileName(file));
                        }
				    }
			    }
		    } else {
			    var files = Directory.GetFiles(folder);
			    if (files != null && files.Count() > 0){
				    output = files.ToList();
			    }
		    }
	    }
	
	    return output.OrderBy(x => x).ToList();
    }

    public List<string> GetFontFiles()
    {
        // Get any WOFF files in the fonts folder.
        var output = new List<string>();

        var fontFiles = CacheStore.GetCachedItem<List<string>>("FontWoffFiles");
        if (fontFiles != null && fontFiles.Count > 0) {
            return fontFiles;
        }

        var fontsFolder = Path.Combine(_applicationPath, "wwwroot", "fonts");
        if (System.IO.Directory.Exists(fontsFolder)) {
            var files = Directory.GetFiles(fontsFolder, "*.woff2");
            if (files.Any()) {
                foreach (var file in files) {
                    output.Add(Tools.GetFileNameWithoutExtension(file));
                }
            }
        }

        CacheStore.SetCacheItem("FontWoffFiles", output);

        return output;
    }

    public List<string> GetInstalledFonts()
    {
	    var output = new List<string>();

        var cached = CacheStore.GetCachedItem<List<string>>("fonts");
        if (cached != null) {
            return cached;
        }

        var fm = SKFontManager.CreateDefault();
        foreach(var f in fm.FontFamilies) {
            if (!String.IsNullOrWhiteSpace(f)) {
                output.Add(f);
            }
        }

        // Add any of the missing browser base fonts.
        foreach (var wsf in _webSafeFonts) {
            if (!output.Contains(wsf, StringComparer.OrdinalIgnoreCase)) {
                output.Add(wsf);
            }
        }
        
        if (output.Count > 0) {
            output = output.OrderBy(x => x).ToList();
        }

        CacheStore.SetCacheItem("fonts", output);

        return output;
    }

    public List<string> GetSlideshows(bool UseCache = true)
    {
        var output = new List<string>();

        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<string>>("slideshows");
            if (cached != null) {
                return cached;
            }
        }

        var folders = Directory.GetDirectories(_folderSlideshows);
        if (folders != null && folders.Count() > 0) {
            foreach (var folder in folders) {
                output.Add(Tools.GetFileName(folder));
            }
        }

        CacheStore.SetCacheItem("slideshows", output);

        return output;
    }

    private List<string> GetStaticClassPublicStringProperties(System.Type staticType)
    {
        var output = new List<string>();

        foreach (var prop in staticType.GetProperties().Where(x => x.PropertyType == typeof(String))) {
            var propertyName = prop.Name;

            if (!String.IsNullOrWhiteSpace(propertyName) && !output.Contains(propertyName)) {
                output.Add(propertyName);
            }
        }

        if (output.Any()) {
            output = output.OrderBy(x => x).ToList();
        }

        return output;

    }

    public List<Language> GetLanguages()
    {
        var output = new List<Language>();

        var cached = CacheStore.GetCachedItem<List<Language>>("languages");
        if (cached != null && cached.Any()) {
            return cached;
        }

        bool haveDefault = false;

        var files = Directory.GetFiles(_folderLanguages, "*.json");

        var propertyNames = GetStaticClassPublicStringProperties(typeof(Text));

        foreach (var file in files) {
            if (Tools.GetFileName(file).ToLower() == "en-us.json") {
                haveDefault = true;
            }
        }

        var defaultLanguage = ConvertStaticClassToObject<Language>(typeof(Text));
        if (defaultLanguage != null) {
            if (!haveDefault) {
                // Save the defaults to the en-US.json file.
                var json = SerializeObject(defaultLanguage, true, _jsonOptionsPreserveLanguage);
                System.IO.File.WriteAllText(Path.Combine(_folderLanguages, "en-US.json"), json);

                // Reload the files.
                files = Directory.GetFiles(_folderLanguages, "*.json");
            }

            foreach (var file in files) {
                // For each language file, see if there are any missing properties.
                // If so, update them from the defaultLanguage and save the updated file.
                var json = System.IO.File.ReadAllText(file);
                var language = DeserializeObject<Language>(json, _jsonOptionsPreserveLanguage);

                if (language != null) {
                    // Update from the defaults if any property is missing a value.
                    foreach (var item in propertyNames.Where(x => x != "CultureCode")) {
                        var prop = language.GetType().GetProperty(item);
                        if (prop != null) {
                            var value = prop.GetValue(language);
                            if (value == null || String.IsNullOrWhiteSpace(value.ToString())) {
                                // Get the value from the default language.
                                var defaultProp = defaultLanguage.GetType().GetProperty(item);
                                if (defaultProp != null) {
                                    var defaultValue = defaultProp.GetValue(defaultLanguage);
                                    if (defaultValue != null && !String.IsNullOrWhiteSpace(defaultValue.ToString())) {
                                        prop.SetValue(language, defaultValue.ToString());
                                    }
                                }
                            }
                        }
                    }

                    language.cultureCode = null;
                    language.cultureName = null;

                    var updatedJson = SerializeObject(language, true, _jsonOptionsPreserveLanguage);
                    if (json != updatedJson) {
                        // Something has changed.
                        System.IO.File.WriteAllText(file, updatedJson);
                    }

                    language.cultureCode = Path.GetFileNameWithoutExtension(file);
                    language.cultureName = GetCultureNameFromCultureCode(language.cultureCode);
                    output.Add(language);
                }
            }
        }

        CacheStore.SetCacheItem("languages", output);

        return output;
    }

    public byte[]? GetMediaItem(string folder, string filename)
    {
        byte[]? output = null;

        var file = Path.Combine(_basePath, folder, filename);
        if (System.IO.File.Exists(file)) {
            output = System.IO.File.ReadAllBytes(file);
        }

        return output;
    }

    public appSettings GetSettings()
    {
        var output = new appSettings();

        var file = Path.Combine(_basePath, "Settings.json");
        if (System.IO.File.Exists(file)) {
            var json = System.IO.File.ReadAllText(file);
            var settings = DeserializeObject<appSettings>(json);
            if (settings != null) {
                output = settings;
            }
        } else {
            // Create the default settings.
            output.projectionStyle.background = "Blurry Lights.jpg";
            output.projectionStyle.backgroundType = backgroundType.image;
        }

        if (output.availableFonts.Count == 0) {
            output.availableFonts = _webSafeFonts;
        }

        return output;
    }

    public setList GetSetList(string filename)
    {
        var output = new setList();

        var file = Path.Combine(_folderSetLists, filename);
        if (System.IO.File.Exists(file)) {
            var json = System.IO.File.ReadAllText(file);
            var sl = DeserializeObject<setList>(json);
            if (sl != null) {
                output = sl;
                output.fileName = filename;

                foreach(var setListItem in output.items) {
                    setListItem.itemJson = SerializeObject(setListItem.item);
                    setListItem.item = null;
                }
            }
        }

        return output;
    }

    public List<setListFile> GetSetListFilenames(bool UseCache = true)
    {
        var output = new List<setListFile>();

        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<setListFile>>("setlistfilenames");
            if (cached != null && cached.Any()) {
                return cached;
            }
        }

        var files = Directory.GetFiles(_folderSetLists, "*.json");
        if (files.Any()) {
            foreach(var file in files) {
                var json = System.IO.File.ReadAllText(file);
                var setList = DeserializeObject<setList>(json);
                if (setList != null) {
                    output.Add(new setListFile { 
                        fileName = Tools.GetFileName(file), 
                        name = !String.IsNullOrWhiteSpace(setList.name) ? setList.name : Tools.RevertSaveFileName(Tools.GetFileNameWithoutExtension(file)),
                    });
                }
            }
        }

        CacheStore.SetCacheItem("setlistfilenames", output);

        return output;
    }

    public List<setList> GetSetLists()
    {
        var output = new List<setList>();

        var cached = CacheStore.GetCachedItem<List<setList>>("setlists");
        if (cached != null && cached.Any()) {
            return cached;
        }

        var files = Directory.GetFiles(_folderSetLists, "*.json");
        foreach (var file in files) {
            var json = System.IO.File.ReadAllText(file);
            var setList = DeserializeObject<setList>(json);
            if (setList != null) {
                output.Add(setList);
            }
        }

        if (output.Any()) {
            output = output.OrderBy(x => x.fileName).ToList();
        }

        CacheStore.SetCacheItem("setlists", output);

        return output;
    }

    public List<sheetMusic> GetSheetMusic(bool UseCache = true)
    {
        var output = new List<sheetMusic>();

        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<sheetMusic>>("sheetmusic");
            if (cached != null && cached.Any()) {
                return cached;
            }
        }

        // Get all folders in the Sheet Music folder.
        var folders = Directory.GetDirectories(_folderSheetMusic);
        foreach (var folder in folders) {
            // Get each file in the folder.
            var files = Directory.GetFiles(folder, "*.pdf");
            if (files.Count() > 0) {
                var parts = new List<string>();

                foreach(var file in files) {
                    parts.Add(Tools.GetFileNameWithoutExtension(file));
                }

                output.Add(new sheetMusic {
                    title = Tools.GetFileName(folder),
                    parts = parts,
                });
            }
        }

        CacheStore.SetCacheItem("sheetmusic", output);

        return output;
    }

    public slideshowItem GetSlideshow(string folder)
    {
        var output = new slideshowItem { folder = folder };

        var files = Directory.GetFiles(Path.Combine(_folderSlideshows, folder));
        if (files != null && files.Any()) {
            output.images = new List<string>();
            foreach(var file in files) {
                if (Tools.IsImage(file)) {
                    output.images.Add(Tools.GetFileName(file));
                }
            }
        }

        return output;
    }

    public List<songBook> GetSongBooks()
    {
        var output = new List<songBook>();

        var cached = CacheStore.GetCachedItem<List<songBook>>("songbooks");
        if (cached != null && cached.Any()) {
            return cached;
        }

        var folder = Path.Combine(_basePath, "SongBooks");

        if (!System.IO.Directory.Exists(folder)) {
            System.IO.Directory.CreateDirectory(folder);
        }

        var files = Directory.GetFiles(folder, "*.json");

        if (!files.Any()) {
            // There are no songbooks, so create a default one.
            var now = DateTime.Now;

            var songbook = new songBook {
                id = Guid.NewGuid(),
                name = "Public Domain",
                fileName = "Public Domain.json",
                created = now,
                updated = now,
                songs = DefaultSongs,
            };

            SaveSongBook(songbook);

            files = Directory.GetFiles(folder, "*.json");
        }

        foreach (var file in files) {
            var json = System.IO.File.ReadAllText(file);
            var songbook = DeserializeObject<songBook>(json);
            if (songbook != null) {
                songbook.fileName = Tools.GetFileName(file);
                songbook.songs = songbook.songs.OrderBy(x => x.title).ThenBy(x => x.artist).ToList();

                foreach(var song in songbook.songs) {
                    song.songBookId = songbook.id;
                }

                output.Add(songbook);
            }
        }

        CacheStore.SetCacheItem("songbooks", output);

        return output;
    }

    public user? GetUser(Guid userId)
    {
        user? output = null;

        var users = GetUsers();
        output = users.FirstOrDefault(x => x.id == userId);

        return output;
    }

    public user? GetUser(string name)
    {
        user? output = null;

        var cached = CacheStore.GetCachedItem<user>("user-" + name);
        if (cached != null) {
            return cached;
        }

        var file = Path.Combine(_folderUsers, Tools.SafeFileName(name) + ".json");
        if (System.IO.File.Exists(file)) {
            var json = System.IO.File.ReadAllText(file);
            output = DeserializeObject<user>(System.IO.File.ReadAllText(file));
        }

        if (output != null) {
            CacheStore.SetCacheItem("user-" + name, output);
        }

        return output;
    }

    public List<user> GetUsers()
    {
        var output = new List<user>();

        var cached = CacheStore.GetCachedItem<List<user>>("users");
        if (cached != null && cached.Any()) {
            return cached;
        }

        var files = Directory.GetFiles(_folderUsers, "*.json");

        if (!files.Any()) {
            // Create a default user.
            var user = new user {
                id = Guid.NewGuid(),
                name = "Projection",
                enabled = true,
                preferences = new userPreferences(),
            };

            SaveUser(user);

            files = Directory.GetFiles(_folderUsers, "*.json");
        }

        foreach (var file in files) {
            var user = DeserializeObject<user>(System.IO.File.ReadAllText(file));
            if (user != null) {
                output.Add(user);
            }
        }

        CacheStore.SetCacheItem("users", output);

        return output;
    }

    public List<string> GetVideos(bool UseCache = true)
    {
        var output = new List<string>();

        if (UseCache) {
            var cached = CacheStore.GetCachedItem<List<string>>("videos");
            if (cached != null) {
                return cached;
            }
        }

        var files = GetFilesByExtensions(_folderVideos, Tools.ExtensionsForVideos);
        if (files != null && files.Count() > 0) {
            foreach (var file in files) {
                output.Add(Tools.GetFileName(file));
            }
        }

        CacheStore.SetCacheItem("videos", output);

        return output;
    }

    public DateOnly Released {
        get {
            return _released;
        }
    }

    public booleanResponse SaveSettings(appSettings settings)
    {
        var output = new booleanResponse();

        try {
            // Clear out anything that shouldn't be saved.
            settings.projectionStyle = CleanStyle(settings.projectionStyle, true, true);

            var file = Path.Combine(_basePath, "Settings.json");
            var json = SerializeObject(settings, true, _jsonOptionsIndented);
            System.IO.File.WriteAllText(file, json);
            output.result = true;
        } catch (Exception ex) {
            output.messages.Add("Error Saving Settings");
            output.messages.Add(ex.Message);
        }

        return output;
    }

    public booleanResponse SaveSetList(setList setlist)
    {
        var output = new booleanResponse();

        var file = Path.Combine(_folderSetLists, Tools.SafeFileName(setlist.fileName) + ".json");

        try {
            // Clear out items that shouldn't be serialized.
            setlist.activeItem = null;
            setlist.activeItemPart = null;
            setlist.fileName = null;
            setlist.selectedItem = null;
            setlist.saveRequired = null;

            if (setlist.items.Any()) {
                foreach (var item in setlist.items) {
                    item.item = Tools.SetListItemToObjectFromJson(item);
                    item.itemJson = null;

                    if (String.IsNullOrWhiteSpace(item.afterItemOption)) {
                        item.afterItemOption = null;
                    }

                    if (String.IsNullOrWhiteSpace(item.transpose)) {
                        item.transpose = null;
                    }

                    item.itemJson = null;

                    // Perform any item-specific clean up.
                    if (item.item != null) {
                        switch (item.type) {
                            case setListItemType.slideshow:
                                var slideshow = Tools.SetListItemAsSlideshow(item);
                                if (slideshow != null) {
                                    slideshow.images = null;

                                    item.item = slideshow;
                                }
                                break;

                            case setListItemType.song:
                                var song = Tools.SetListItemAsSong(item);
                                if (song != null) {
                                    song.title = null;
                                    song.artist = null;
                                    song.key = null;
                                    song.tempo = null;
                                    song.timeSignature = null;
                                    song.copyright = null;
                                    song.created = null;
                                    song.updated = null;
                                    song.parts = null;
                                    song.content = null;
                                    song.saveRequired = null;
                                    song.ccliNumber = null;
                                }
                                break;
                        }
                    }
                }
            }

            var json = SerializeObject(setlist, true, _jsonOptionsIndented);
            System.IO.File.WriteAllText(file, json);
            output.result = true;
            CacheStore.ClearAll();
        } catch (Exception ex) {
            output.messages.Add("Error Saving SetList: " + ex.Message);
        }

        return output;
    }

    public booleanResponse SaveSong(song song)
    {
        var output = new booleanResponse();

        // First, get all songbooks.
        var songbooks = GetSongBooks();

        var sb = songbooks.FirstOrDefault(x => x.id == song.songBookId);
        if (sb != null) {
            sb.songs = sb.songs.Where(x => x.id != song.id).ToList();
            song.updated = DateTime.Now;
            sb.songs.Add(song);

            sb.songs = sb.songs.OrderBy(x => x.title).ThenBy(x => x.artist).ToList();

            SaveSongBook(sb);

            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("SongBook '" + song.songBookId.ToString() + "' Not Found");
        }

        return output;
    }

    public booleanResponse SaveSongBook(songBook songbook)
    {
        var output = new booleanResponse();

        try {
            var filename = String.Empty + songbook.fileName;

            // Clear out items that shouldn't be serialized.
            songbook.fileName = null;
            songbook.saveRequired = null;

            foreach(var song in songbook.songs) {
                song.songBookId = null;
                song.saveRequired = null;
                song.parts = null;
            }

            songbook.songs = songbook.songs.OrderBy(x => x.title).ThenBy(x => x.artist).ToList();

            var json = SerializeObject(songbook, true, _jsonOptionsPreserveLanguage);
            System.IO.File.WriteAllText(Path.Combine(_folderSongBooks, Tools.SafeFileName(filename)), json);
            output.result = true;

            CacheStore.ClearAll();
        } catch (Exception ex) {
            output.messages.Add("Error Saving Songbook: " + ex.Message);
        }

        return output;
    }

    public booleanResponse SaveUser(user user)
    {
        var output = new booleanResponse();

        try {
            var file = Path.Combine(_folderUsers, Tools.SafeFileName(user.name) + ".json");
            var json = SerializeObject(user, true, _jsonOptionsIndented);
            System.IO.File.WriteAllText(file, json);

            output.result = true;

            CacheStore.ClearAll();
        } catch (Exception ex) {
            output.messages.Add("Error Saving User: " + ex.Message);
        }

        return output;
    }

    public booleanResponse SaveUserPreferences(Guid userId, userPreferences preferences)
    {
        var output = new booleanResponse();

        var user = GetUser(userId);
        if (user != null) {
            user.preferences = preferences;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + userId.ToString() + "' Not Found");
        }

        return output;
    }

    public booleanResponse SaveUserPreferences(string name, userPreferences preferences)
    {
        var output = new booleanResponse();

        var user = GetUser(name);
        if (user != null) {
            user.preferences = preferences;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + name + "' Not Found");
        }

        return output;
    }

    public booleanResponse SetUserViewPreferences(user user)
    {
        var output = new booleanResponse();

        var u = GetUser(user.name);

        if (u != null) {
            u.preferences.autoFollow = user.preferences.autoFollow;
            u.preferences.hideChords = user.preferences.hideChords;
            u.preferences.zoom = user.preferences.zoom;
            SaveUserPreferences(u.name, u.preferences);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + user.name + "' Not Found");
        }

        return output;
    }
    private string SerializeObject(object? Object, bool formatOutput = true, JsonSerializerOptions? serializer = null)
    {
        string output = String.Empty;

        if (serializer == null) {
            if (formatOutput) {
                serializer = _jsonOptionsIndented;
            } else {
                serializer = _jsonOptions;
            }
        }

        if (Object != null) {
            output += System.Text.Json.JsonSerializer.Serialize(Object, serializer);
        }

        return output;
    }

    public booleanResponse SetUserHideChords(Guid userId, bool hideChords)
    {
        var output = new booleanResponse();

        var user = GetUser(userId);

        if (user != null) {
            user.preferences.hideChords = hideChords;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + userId.ToString() + "' Not Found");
        }

        return output;
    }

    public booleanResponse SetUserHideChords(string name, bool hideChords)
    {
        var output = new booleanResponse();

        var user = GetUser(name);

        if (user != null) {
            user.preferences.hideChords = hideChords;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + name + "' Not Found");
        }

        return output;
    }

    public booleanResponse SetUserTheme(Guid userId, string theme)
    {
        var output = new booleanResponse();

        var user = GetUser(userId);
        if (user != null) {
            user.preferences.theme = theme;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + userId.ToString() + "' Not Found");
        }

        return output;
    }

    public booleanResponse SetUserTheme(string name, string theme)
    {
        var output = new booleanResponse();

        var user = GetUser(name);
        if (user != null) {
            user.preferences.theme = theme;
            SaveUser(user);
            output.result = true;

            CacheStore.ClearAll();
        } else {
            output.messages.Add("User '" + name + "' Not Found");
        }

        return output;
    }

    public string Version {
        get {
            return _version;
        }
    }
}
