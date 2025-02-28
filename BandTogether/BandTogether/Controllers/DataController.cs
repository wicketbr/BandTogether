using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using Radzen.Blazor;
using System.Net;
using System.Text;

namespace BandTogether.Controllers;

[ApiController]
public partial class DataController : ControllerBase
{
    private HttpContext? context;
    private readonly IHubContext<signalRhub> signalR;
    private IDataAccess da;

    public DataController(IHttpContextAccessor httpContextAccessor, IHubContext<signalRhub> hubContext, IDataAccess dataAccess)
    {
        context = httpContextAccessor.HttpContext;
        signalR = hubContext;
        da = dataAccess;
    }

    [HttpPost]
    [Route("~/api/AddSong")]
    public async Task<ActionResult<booleanResponse>> AddSong(song song)
    {
        var output = da.AddSong(song);

        if (output.result) {
            await SignalRUpdate(new signalRUpdate {
                updateType = signalRUpdateType.admin,
                message = "reload-songbooks",
            });
        }

        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/AddSongBook")]
    public async Task<ActionResult<booleanResponse>> AddSongBook(simplePost post)
    {
        var output = da.AddSongBook(post.singleItem);

        if (output.result) {
            await SignalRUpdate(new signalRUpdate {
                updateType = signalRUpdateType.admin,
                message = "reload-songbooks",
            });
        }

        return Ok(output);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("~/Background/{filename}")]
    public IActionResult ViewFile(string filename)
    {
        byte[]? fileContent = da.GetBackground(filename);
        string mimeType = Tools.GetMimeType(System.IO.Path.GetExtension(filename));

        if (fileContent == null) {
            return new EmptyResult();
        } else {
            //return new FileStreamResult(new MemoryStream(fileContent), mimeType);
            return File(new MemoryStream(fileContent), mimeType, enableRangeProcessing: true);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("~/Media/{folder}/{filename}")]
    public IActionResult ViewMedia(string folder, string filename)
    {
        byte[]? fileContent = da.GetMediaItem(folder, filename);
        string mimeType = Tools.GetMimeType(System.IO.Path.GetExtension(filename));
        if (fileContent == null) {
            return new EmptyResult();
        } else {
            //return new FileStreamResult(new MemoryStream(fileContent), mimeType);
            return File(new MemoryStream(fileContent), mimeType, enableRangeProcessing: true);
        }
    }

    [HttpGet]
    [Route("~/SheetMusic/{folder}/{filename}")]
    public IActionResult SheetMusic(string folder, string filename)
    {
        byte[]? fileContent = da.GetMediaItem("SheetMusic/" + folder, filename);
        string mimeType = Tools.GetMimeType(System.IO.Path.GetExtension(filename));
        if (fileContent == null) {
            return new EmptyResult();
        } else {
            return new FileStreamResult(new MemoryStream(fileContent), mimeType);
        }
    }

    [HttpGet]
    [Route("~/Slideshow/{folder}/{filename}")]
    public IActionResult Slideshow(string folder, string filename)
    {
        byte[]? fileContent = da.GetMediaItem("Slideshows/" + folder, filename);
        string mimeType = Tools.GetMimeType(System.IO.Path.GetExtension(filename));
        if (fileContent == null) {
            return new EmptyResult();
        } else {
            return new FileStreamResult(new MemoryStream(fileContent), mimeType);
        }
    }

    [HttpPost]
    [Route("~/api/ConvertPowerPointToSlides")]
    public async Task<ActionResult<booleanResponse>> ConvertPowerPointToSlides(fileItem file)
    {
        var output = await da.ConvertPowerPointToSlides(file);
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/DeleteSong")]
    public async Task<ActionResult<booleanResponse>> DeleteSong(song song)
    {
        var output = da.DeleteSong(song);
        if (output.result) {
            await SignalRUpdate(new signalRUpdate {
                updateType = signalRUpdateType.song,
                message = "deleted",
                obj = song,
            });
        }
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/DeleteSongBook")]
    public async Task<ActionResult<booleanResponse>> DeleteSongBook(simplePost post)
    {
        var output = da.DeleteSongBook(post.singleItem);
        if (output.result) {
            await SignalRUpdate(new signalRUpdate {
                updateType = signalRUpdateType.admin,
                message = "reload-songbooks",

            });
        }
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/DeleteUser/{id}")]
    public async Task<ActionResult<booleanResponse>> DeleteUser(Guid id)
    {
        var output = da.DeleteUser(id);

        if (output.result) {
            await signalR.Clients.All.SendAsync("message", "user-deleted", id.ToString());
        }
        
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetAudioFiles")]
    public ActionResult<List<string>> GetAudioFiles()
    {
        var output = da.GetAudioFiles(false);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetBackgrounds")]
    public ActionResult<List<string>> GetBackgrounds()
    {
        var output = da.GetBackgrounds(false);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetCachedMessages")]
    public ActionResult<messages> GetCachedMessages()
    {
        var output = GlobalSettings.CachedMessages;
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetCachedSetList")]
    public ActionResult<setList> GetCachedSetList()
    {
        var output = GlobalSettings.CachedSetList;
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetDataLoader")]
    public ActionResult<dataLoader> GetDataLoader()
    {
        var output = da.GetDataLoader();
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetImages")]
    public ActionResult<List<string>> GetImages()
    {
        var output = da.GetImages(false);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetLanguages")]
    public ActionResult<List<Language>> GetLanguages()
    {
        var output = da.GetLanguages();
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSettings")]
    public ActionResult<appSettings> GetSettings()
    {
        var output = da.GetSettings();
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/GetSetList/")]
    public ActionResult<setList> GetSetList(simplePost post)
    {
        var output = da.GetSetList(String.Empty + post.singleItem);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSetListFilenames")]
    public ActionResult<List<string>> GetSetListFilenames()
    {
        var output = da.GetSetListFilenames(false);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSetLists")]
    public ActionResult<List<setList>> GetSetLists()
    {
        var output = da.GetSetLists();
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSongBooks")]
    public ActionResult<List<songBook>> GetSongBooks()
    {
        var output = da.GetSongBooks();
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSlideshows")]
    public ActionResult<List<string>> GetSlideshows()
    {
        var output = da.GetSlideshows(false);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetUser/{id}")]
    public ActionResult<user> GetUser(Guid id)
    {
        var output = da.GetUser(id);
        if (output == null) {
            output = new user();
        }
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetUsers")]
    public ActionResult<List<user>> GetUsers()
    {
        var output = da.GetUsers();
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetVideos")]
    public ActionResult<List<string>> GetVideos()
    {
        var output = da.GetVideos(false);
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveSettings")]
    public async Task<ActionResult<booleanResponse>> SaveSettings(appSettings settings)
    {
        var output = da.SaveSettings(settings);
     
        await SignalRUpdate(new signalRUpdate {
            updateType = signalRUpdateType.admin,
            message = "reload-settings",
        });
        
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveSetList")]
    public async Task<ActionResult<booleanResponse>> SaveSetList(setList setlist)
    {
        var output = da.SaveSetList(setlist);

        await SignalRUpdate(new signalRUpdate {
            updateType = signalRUpdateType.setList,
            message = "saved",
            obj = setlist,
        });

        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/GetSlideshow/{folder}")]
    public ActionResult<slideshowItem> GetSlideshow(string folder)
    {
        var output = da.GetSlideshow(folder);
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveSong")]
    public async Task<ActionResult<booleanResponse>> SaveSong(song song)
    {
        var songbookId = song.songBookId;

        var output = da.SaveSong(song);

        song.songBookId = songbookId;

        await SignalRUpdate(new signalRUpdate {
            updateType = signalRUpdateType.song,
            message = "saved",
            itemId = song.id,
            obj = song,
        });

        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveSongBook")]
    public async Task<ActionResult<booleanResponse>> SaveSongBook(songBook songBook)
    {
        var output = da.SaveSongBook(songBook);

        await SignalRUpdate(new signalRUpdate {
            updateType = signalRUpdateType.admin,
            message = "reload-songbooks",
        });

        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveUser")]
    public async Task<ActionResult<booleanResponse>> SaveUser(user user)
    {
        var output = da.SaveUser(user);

        await SignalRUpdate(new signalRUpdate {
            updateType = signalRUpdateType.admin,
            message = "reload-users",
        });

        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SaveUserPreferences/{id}")]
    public async Task<ActionResult<booleanResponse>> SaveUserPreferences(Guid id, userPreferences userPreferences)
    {
        var output = da.SaveUserPreferences(id, userPreferences);
        await SignalR_UserUpdate(id);
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SetCachedSetList")]
    public ActionResult<booleanResponse> SetCachedSetList(setList setlist)
    {
        GlobalSettings.CachedSetList = setlist;
        return Ok(new booleanResponse { result = true });
    }

    [HttpPost]
    [Route("~/api/SetUserCapoPosition/")]
    public async Task<ActionResult<booleanResponse>> SetUserCapoPosition(songPreferences pref)
    {
        var output = new booleanResponse();

        var user = da.GetUser(pref.userId);
        if (user != null) {
            var song = user.preferences.songPreferences.FirstOrDefault(x => x.songId == pref.songId && x.songBookId == pref.songBookId);
            if (song != null) {
                song.capo = pref.capo;
                song.title = pref.title;
            } else {
                user.preferences.songPreferences.Add(pref);
            }

            da.SaveUserPreferences(pref.userId, user.preferences);
            await SignalR_UserUpdate(pref.userId);
            output.result = true;
        }

        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/SetUserHideChords/{id}/{hideChords}")]
    public async Task<ActionResult<booleanResponse>> SetUserHideChords(Guid id, bool hideChords)
    {
        var output = da.SetUserHideChords(id, hideChords);
        await SignalR_UserUpdate(id);
        return Ok(output);
    }

    [HttpGet]
    [Route("~/api/SetUserTheme/{id}/{theme}")]
    public async Task<ActionResult<booleanResponse>> SetUserTheme(Guid id, string theme)
    {
        if (theme == "auto") {
            theme = "";
        }

        var output = da.SetUserTheme(id, theme);
        await SignalR_UserUpdate(id);
        return Ok(output);
    }

    [HttpPost]
    [Route("~/api/SetUserViewPreferences")]
    public async Task<ActionResult<booleanResponse>> SetUserViewPreferences(user user)
    {
        var output = da.SetUserViewPreferences(user);
        await SignalR_UserUpdate(user.id);
        return Ok(output);
    }

    private async Task SignalRUpdate(signalRUpdate update)
    {
        if (update.obj != null) {
            update.objectAsString = System.Text.Json.JsonSerializer.Serialize(update.obj);
        }

        await signalR.Clients.All.SendAsync("update", update);
    }

    private async Task SignalR_UserUpdate(Guid userId)
    {
        var user = da.GetUser(userId);
        if (user != null) {
            await signalR.Clients.All.SendAsync("user", user);
        }
    }
}

class MyPushStreamResult :IActionResult
{
    Func<Stream, CancellationToken, Task> _pushAction;
    string _contentType;
 
    public MyPushStreamResult(Func<Stream, CancellationToken, Task> pushAction, string contentType)
    {
        _pushAction = pushAction;
        _contentType = contentType;
    }

    public Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.ContentType = _contentType;
        response.StatusCode = 200;
 

        return _pushAction(response.Body, context.HttpContext.RequestAborted);
    }
}
