using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using Radzen.Blazor;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BandTogether.Controllers;

public partial class DataController : ControllerBase
{
    [HttpPost]
    [Route("~/VideoPsalm")]
    public async Task<ActionResult> AgendaItem(VideoPsalmSong item) {
        var songId = Helpers.GuidValue(item.Guid);

        // Convert the song from the VideoPsalm format to the BandTogether format.
        var songContent = new System.Text.StringBuilder();

        if (item.SongVerses != null && item.SongVerses.Any()) {
            bool firstItem = true;
            foreach (var verse in item.SongVerses) {
                if (!firstItem) {
                    songContent.AppendLine();
                }

                songContent.AppendLine(verse.Title);

                if (!String.IsNullOrWhiteSpace(verse.Lyrics)) {
                    var lines = verse.Lyrics.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (var line in lines) {
                        songContent.AppendLine(line.Trim());
                    }
                }

                firstItem = false;
            }
        }

        var song = new song { 
            id = songId,
            songBookId = Guid.Empty,
            title = item.Title,
            artist = item.Author,
            key = item.Key,
            tempo = item.Tempo,
            timeSignature = "",
            copyright = item.Copyright,
            created = null,
            updated = null,
            parts = null,
            saveRequired = null,
            ccliNumber = item.CCLI,
            content = songContent.ToString(),
        };

        var activeItemPart = Helpers.IntValue(item.CurrentItemIndex) - 1;
        var transposition = Helpers.IntValue(item.Transposition);
        String? transposeKey = null;

        if (transposition != 0) {
            transposeKey = Helpers.TransposeKey(item.Key, transposition);
        }

        // Create a new single item set list for the song and push this out to the clients via SignalR.
        // This will only be processed in the Tablet view, all other views will ignore this.
        var setlist = new setList { 
            fileName = "videopsalm_api_call",
            activeItem = songId,
            selectedItem = songId,
            activeItemPart = activeItemPart,
            name = null,
            saveRequired = null,
            items = new List<setListItem> { 
                new setListItem { 
                    id = songId,
                    type = setListItemType.song,
                    item = null,
                    itemJson = System.Text.Json.JsonSerializer.Serialize(song),
                    afterItemOption = null,
                    transpose = transposeKey,
                    disabledElements = null,
                },
            },
        };

        GlobalSettings.CachedSetList = setlist;
        CacheStore.SetCacheItem("setlist", setlist);

        await signalR.Clients.All.SendAsync("setlist", setlist);

        return Ok();
    }
}

public class VideoPsalmSong
{
    public string? Guid { get; set; } // Guid.ToString()
    public string? Id { get; set; } // Songbook song number, 0 when there is no number.
    public string? Type { get; set; } // Type of the agenda item, as a string, "Song".
    public string? Title { get; set; } // Song title.
    public string? Alias { get; set; } // Alternate song title.
    public string? Author { get; set; } // Song authors.
    public string? Composer { get; set; } // Song composers.
    public string? Copyright { get; set; } // Song copyright text.
    public string? CCLI { get; set; } // Song CCLI Id.
    public string? Theme { get; set; } // Song theme.
    public string? Key { get; set; } // Song music key.
    public string? Reference { get; set; } // Song Bible reference.
    public string? Tempo { get; set; } // Song tempo.
    public string? Capo { get; set; } // Song capo.
    public string? Memo1 { get; set; } // Song note.
    public string? Memo2 { get; set; } // Song note 2.
    public string? Memo3 { get; set; } // Song note 3.
    public string? CurrentItemIndex { get; set; } // Currently displayed song verse index, starting at 0.
    public string? Transposition { get; set; }
    public List<VideoPsalmSongVerse> SongVerses { get; set; } = new List<VideoPsalmSongVerse>();
}

public class VideoPsalmSongVerse
{
    public string? Id { get; set; } // 1, 2, etc. (starting with 1, 0 when there is no Id).
    public string? Type { get; set; } // V1, C, etc.
    public string? Title { get; set; } // Verse 1
    public string? Lyrics { get; set; } // Multiline text.
    public string? LyricsOnly { get; set; }
}