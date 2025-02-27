using BandTogether.Client.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using Radzen;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace BandTogether;

public static class Helpers
{
    private static string _currentSongExport = "";
    private static Radzen.DialogService DialogService = null!;
    private static HttpClient Http = null!;
    private static bool _initialized = false;
    private static IJSRuntime jsRuntime = null!;
    private static ILocalStorageService LocalStorage = null!;
    private static BlazorDataModel Model = null!;
    private static NavigationManager NavManager = null!;
    private static bool _ShowChordsAsNashvilleNumbers = false;
    private static bool _UsingDoReMi = false;
    private static bool _UsingNashvilleNumbering = false;

    private static bool _savingUserPreferences = false;

    private static string _dialogTop = "10px";

    /// <summary>
    /// Initializes the Helpers static class library by providing the required objects to the library.
    /// </summary>
    /// <param name="jSRuntime">A reference to the IJSRuntime interface.</param>
    /// <param name="model">A reference to the BlazorDataModel.</param>
    /// <param name="localStorage">A reference to the ILocalStorageService interface.</param>
    /// <param name="dialogService">A reference to the Radzen DialogService.</param>
    /// <param name="navigationManager">A reference to the NavigationManager interface.</param>
    public static void Init(
        IJSRuntime jSRuntime,
        BlazorDataModel model,
        HttpClient httpClient,
        ILocalStorageService localStorage,
        Radzen.DialogService dialogService,
        NavigationManager navigationManager
    )
    {
        DialogService = dialogService;
        Http = httpClient;
        jsRuntime = jSRuntime;
        LocalStorage = localStorage;
        Model = model;
        NavManager = navigationManager;

        _initialized = true;
    }

    public static async Task AddClass(string ElementId, string ClassName)
    {
        await jsRuntime.InvokeVoidAsync("AddClass", ElementId, ClassName);
    }

    public static string AspectRatioToCssClass(string? ratio)
    {
        var output = !String.IsNullOrWhiteSpace(ratio)
                ? "ar_" + ratio.Replace(":", "x").Replace(".", "_")
                : "ar_16x9";
        return output;
    }

    public static string AspectRatioToBootstrapResponsive(string? ratio)
    {
        var output = !String.IsNullOrWhiteSpace(ratio)
                ? ratio.Replace(":", "by")
                : "16by9";
        return output;
    }

    public static async Task AutoAdjustFontSize(int zoom)
    {
        await jsRuntime.InvokeVoidAsync("AutoAdjustFontSize", zoom);
    }

    public static string BackgroundStyle(viewStyle style)
    {
        string output = "";

        switch(style.backgroundType) {
            case backgroundType.unknown:
                output = "background-color:#000;";
                break;

            case backgroundType.color:
                output = "background-color:" + style.background + ";";
                break;

            case backgroundType.image:
                //output = "background-image: url('/Background/" + style.Background + "'); background-size:cover;";
                break;

            case backgroundType.video:
                break;
        }

        output += " height:100%;";

        return output;
    }

    /// <summary>
    /// The BaseUri from the NavigationManager
    /// </summary>
    public static string BaseUri {
        get {
            return NavManager.BaseUri;
        }
    }

    /// <summary>
    /// Clears the current export data.
    /// </summary>
    public static void BeginSongRender()
    {
        _currentSongExport = "";
    }

    public static bool BooleanValue(bool? value)
    {
        return value.HasValue && value.Value;
    }

    public static string CapoLabel(int capo)
    {
        if (capo == 0) {
            return Text.NoCapo;
        } else if (capo == 12) {
            // Use Nashville Numbering  
            return "<div class=\"nns-label\">" + Text.NashvilleNumbers + "</div>";
        } else {
            string output = "<div class=\"capo-label\">" + Text.Capo + " <strong>" + Helpers.NumberToRomanNumerals(capo) + "</strong></div>"; ;

            if (!String.IsNullOrWhiteSpace(Model.OriginalMatrixKey)) {
                var newKey = Helpers.TransposeKey(Model.OriginalMatrixKey, 12 - capo);

                if (!String.IsNullOrWhiteSpace(newKey)) {
                    output += " to play in <strong>" + newKey + "</strong>";
                }
            }

            return output;
        }
    }

    public static string CapoText(int capo)
    {
        string output = "";

        if (capo == 0) {
            output = Text.NoCapo;
        } else if (capo == 12) {
            output = Text.NashvilleNumbers;
        } else {
            output = Helpers.NumberToRomanNumerals(capo);

            if (!String.IsNullOrWhiteSpace(Model.OriginalMatrixKey)) {
                var newKey = Helpers.TransposeKey(Model.OriginalMatrixKey, 12 - capo);

                if (!String.IsNullOrWhiteSpace(newKey)) {
                    output += " to play in " + newKey;
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Gets the corrected name of a key as used by the matrix.
    /// </summary>
    /// <param name="chord"></param>
    /// <returns></returns>
    public static string ChordRootMatrix(string? chord)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(chord)) {
            if (chord.Length == 1) {
                return chord;
            }

            string test1 = chord.ToLower();
            string test2 = test1.Substring(0, 2);

            switch (test2) {
                case "ab":
                case "a♭":
                    output = "A♭";
                    break;
                case "am":
                    output = "A";
                    break;
                case "a#":
                case "a♯":
                    output = "A♯";
                    break;
                case "bb":
                case "b♭":
                    output = "B♭";
                    break;
                case "bm":
                    output = "B";
                    break;
                case "cb":
                case "c♭":
                    output = "C♭";
                    break;
                case "cm":
                    output = "C";
                    break;
                case "c#":
                case "c♯":
                    output = "C♯";
                    break;
                case "db":
                case "d♭":
                    output = "D♭";
                    break;
                case "dm":
                    output = "D";
                    break;
                case "d#":
                case "d♯":
                    output = "D♯";
                    break;
                case "eb":
                case "e♭":
                    output = "E♭";
                    break;
                case "em":
                    output = "E";
                    break;
                case "e#":
                case "e♯":
                    output = "E♯";
                    break;
                case "fb":
                case "f♭":
                    output = "F♭";
                    break;
                case "fm":
                    output = "F";
                    break;
                case "f#":
                case "f♯":
                    output = "F♯";
                    break;
                case "gb":
                case "g♭":
                    output = "G♭";
                    break;
                case "gm":
                    output = "G";
                    break;
                case "g#":
                case "g♯":
                    output = "G♯";
                    break;
            }

            if (output == "") {
                output = chord.Substring(0, 1);
            }
        }

        return output;
    }

    /// <summary>
    /// Wraps chords in a table data element.
    /// </summary>
    /// <param name="chords"></param>
    /// <param name="originalKey"></param>
    /// <param name="newKey"></param>
    /// <returns></returns>
    public static string ChordsToTableDataElements(List<string>? chords, string? originalKey, string newKey)
    {
        string output = "";

        if (chords != null && chords.Any()) {
            foreach (var item in chords) {
                output += "<td>" + FormatChord(item, originalKey, newKey) + "</td>";
            }
        }

        return output;
    }

    /// <summary>
    /// Formats an error message or messages in a simple comma separated list with the word "Error" or "Errors" at the beginning.
    /// </summary>
    /// <param name="messages">The message collection.</param>
    /// <returns>The formatted string.</returns>
    public static string CleanErrorMessages(List<string> messages)
    {
        string output = String.Empty;

        int count = messages.Count();

        if (count == 0) {
            output = Text.Error;
        } else if (count == 1) {
            output = Text.Error + " - " + messages[0];
        } else {
            output = Text.Errors + ": ";
            foreach (var message in messages.Index()) {
                if (message.Index > 0) {
                    output += ", ";
                }
                output += message.Item;
            }
        }

        return output;
    }

    /// <summary>
    /// Clears the value for a local storage item.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    public static async Task ClearLocalStorageItem(string key)
    {
        if (LocalStorage != null) {
            await LocalStorage.RemoveItemAsync(key);
        }
    }

    public async static Task ClearVideoPlayers()
    {
        await jsRuntime.InvokeVoidAsync("ClearVideoPlayers");
    }

    /// <summary>
    /// Writes out objects to the console using jsInterop.
    /// </summary>
    /// <param name="objects">Any objects to write out to the console.</param>
    public static async Task ConsoleLog(params object[] objects)
    {
        if (jsRuntime != null) {
            foreach (var obj in objects) {
                await jsRuntime.InvokeVoidAsync("ConsoleLog", GetObjectType(obj), obj);
            }
        }
    }

    /// <summary>
    /// Writes out a message to the console using jsInterop.
    /// </summary>
    /// <param name="message">The message to display</param>
    public static async Task ConsoleLog(string message)
    {
        if (jsRuntime != null) {
            await jsRuntime.InvokeVoidAsync("ConsoleLog", message);
        }
    }

    /// <summary>
    /// Writes out a message and optional objects to the console using jsInterop.
    /// </summary>
    /// <param name="message">The message to display</param>
    /// <param name="objects">An optional collection of objects.</param>
    public static async Task ConsoleLog(string message, params object[] objects)
    {
        if (jsRuntime != null) {
            if (objects != null) {
                if (objects.Length == 1) {
                    var obj = objects[0];
                    await jsRuntime.InvokeVoidAsync("ConsoleLog", message + ": " + GetObjectType(obj), obj);
                } else {
                    int index = -1;
                    foreach (var obj in objects) {
                        index++;
                        await jsRuntime.InvokeVoidAsync("ConsoleLog", message + index.ToString() + ": " + GetObjectType(obj), obj);
                    }
                }
            } else {
                await jsRuntime.InvokeVoidAsync("ConsoleLog", message);
            }
        }
    }

    /// <summary>
    /// Reads a cookie using jsInterop.
    /// </summary>
    /// <typeparam name="T">The type of value stored in the cookie.</typeparam>
    /// <param name="name">The name of the cookie item.</param>
    /// <returns>A nullable T object.</returns>
    public static async Task<T> CookieRead<T>(string name)
    {
        var output = await jsRuntime.InvokeAsync<T>("CookieRead", name);
        return output;
    }

    /// <summary>
    /// Writes a cookie using jsInterop.
    /// </summary>
    /// <param name="name">The name of the cookie item.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="days">Optional number of days until the cookie expires (defaults to 14.)</param>
    public static async Task CookieWrite(string name, string value, int days = 14)
    {
        await jsRuntime.InvokeVoidAsync("CookieWrite", name, value, days);
    }

    /// <summary>
    /// Copies the value to the clipboard.
    /// </summary>
    /// <param name="value">The value to copy to the clipboard.</param>
    public static async Task CopyToClipboard(string value)
    {
        if (jsRuntime != null) {
            await jsRuntime.InvokeVoidAsync("CopyToClipboard", value);
        }
    }

    public static string CurrentThemeIconForUser(user user)
    {
        string output = String.Empty;

        var label = CurrentThemeLabelForUser(user);

        switch (label.ToLower()) {
            case "light":
                output = Icon("ThemeLight");
                break;
            case "dark":
                output = Icon("ThemeDark");
                break;
            default:
                output = Icon("ThemeAuto");
                break;
        }

        return output;
    }

    public static string CurrentThemeLabelForUser(user user)
    {
        string output = "";

        switch (user.preferences.theme) {
            case "light":
                output = Text.ThemeLight;
                break;
            case "dark":
                output = Text.ThemeDark;
                break;
            default:
                output = Text.ThemeAuto;
                break;
        }

        return output;
    }

    /// <summary>
    /// Gets the Uri as a string from the NavigationManager
    /// </summary>
    public static string CurrentUrl {
        get {
            return NavManager.Uri.ToString();
        }
    }

    /// <summary>
    /// Sets the focus to an element as soon as it becomes visible.
    /// </summary>
    /// <param name="elementId">The id of the HTML element.</param>
    public static async Task DelayedFocus(string elementId)
    {
        if (jsRuntime != null) {
            await jsRuntime.InvokeVoidAsync("DelayedFocus", elementId);
        }
    }

    /// <summary>
    /// Sets the focus and selects all text for an element as soon as it becomes visible.
    /// </summary>
    /// <param name="elementId">The id of the HTML element.</param>
    public static async Task DelayedSelect(string elementId)
    {
        if (jsRuntime != null) {
            await jsRuntime.InvokeVoidAsync("DelayedSelect", elementId);
        }
    }

    /// <summary>
    /// Deserializes an object that was serialized as JSON.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="SerializedObject">The serialized object as a JSON string.</param>
    /// <returns>A nullable object of type T.</returns>
    public static T? DeserializeObject<T>(string? SerializedObject)
    {
        var output = default(T);

        if (!String.IsNullOrWhiteSpace(SerializedObject)) {
            try {
                var d = System.Text.Json.JsonSerializer.Deserialize<T>(SerializedObject, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (d != null) {
                    output = d;
                }
            } catch { }
        }

        return output;
    }

    public static async Task DialogOpenSetJSInterop(bool open)
    {
        await jsRuntime.InvokeVoidAsync("DialogOpen", open);
    }

    /// <summary>
    /// Performs the JS Interop function to download the contents of a file to the browser.
    /// </summary>
    /// <param name="FileName">The name of the file.</param>
    /// <param name="FileData">The byte array data of the file contents.</param>
    public static async Task DownloadFileToBrowser(string FileName, byte[]? FileData)
    {
        if (FileData != null && FileData.Length > 0) {
            MemoryStream fileStream = new MemoryStream(FileData);

            using (var streamRef = new DotNetStreamReference(stream: fileStream)) {
                await jsRuntime.InvokeVoidAsync("DownloadFileFromStream", FileName, streamRef);
            }
        }
    }

    /// <summary>
    /// Creates a duplicate copy of an object when you need a copy that no longer references the original so updates to the object don't update the original.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="o">The object to be duplicated.</param>
    /// <returns>A nullable copy of the object.</returns>
    public static T? DuplicateObject<T>(object? o)
    {
        T? output = default(T);

        if (o != null) {
            // To make a new copy serialize the object and then deserialize it back to a new object.
            var serialized = System.Text.Json.JsonSerializer.Serialize(o);
            if (!String.IsNullOrEmpty(serialized)) {
                try {
                    var duplicate = System.Text.Json.JsonSerializer.Deserialize<T>(serialized, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (duplicate != null) {
                        output = duplicate;
                    }
                } catch { }
            }
        }

        return output;
    }

    /// <summary>
    /// Displays a modal message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">An optional title for the dialog.</param>
    public async static Task EditSetListItem(setListItem item)
    {
        var title = item.id == Guid.Empty ? Text.AddSetListItem : Text.EditSetListItem;

        // Load any necessary libraries.
        switch(item.type) {
            case setListItemType.audio:
                await ReloadAudioFiles();
                break;

            case setListItemType.countdown:
                break;

            case setListItemType.image:
                await ReloadImages();
                break;
            
            case setListItemType.slideshow:
                await ReloadSlideshows();
                break;

            case setListItemType.video:
                await ReloadVideos();
                break;

            case setListItemType.youTube:
                break;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Value", item);

        await DialogService.OpenAsync<EditSetListItem>(title, parameters, new Radzen.DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = false,
            ShowClose = false,
            Width = "98%",
            Top = _dialogTop,
        });
    }

    /// <summary>
    /// Returns the current transposed version of the song using the selected capo position.
    /// </summary>
    public static string Export {
        get {
            return _currentSongExport;
        }
    }

    /// <summary>
    /// Uses jsInterop to fade an element in.
    /// </summary>
    /// <param name="elementId">The id of the element.</param>
    /// <param name="duration">The duration of the transition (defaults to 200 ms.)</param>
    /// <param name="display">The display type for the element (if it was faded out with FadeOut, the origianl display will be used; otherwise, defaults to "block".)</param>
    public static async Task FadeIn(string elementId, int duration = 200, string display = "block")
    {
        await jsRuntime.InvokeVoidAsync("FadeElementIn", elementId, duration, display);
    }

    /// <summary>
    /// Uses jsInterop to fade an element in.
    /// </summary>
    /// <param name="elementId">The id of the element.</param>
    /// <param name="duration">The duration of the transition (defaults to 200 ms.)</param>
    public static async Task FadeOut(string elementId, int duration = 200)
    {
        await jsRuntime.InvokeVoidAsync("FadeElementOut", elementId, duration);
    }

    public static string FontStyleToCSS(viewStyle style, string wrapperClass, double Scale = 1.0)
    {
        var output = new StringBuilder();

        if (style.lyricsStyle != null) {
            double startOpacity = 0;
            double opacityStep = .1;
            double step0 = 0;
            double step1 = 0;
            double step2 = 0;

            if (Helpers.BooleanValue(style.showPreviousLyrics)) {
                startOpacity = style.previousLyricsOpacity.HasValue ? style.previousLyricsOpacity.Value : .3;
                opacityStep = style.previousLyricsOpacityStep.HasValue ? style.previousLyricsOpacityStep.Value : .1;

                step0 = startOpacity - opacityStep - opacityStep;
                if (step0 < 0) { step0 = 0; }

                step1 = startOpacity - opacityStep;
                if (step1 < 0) { step1 = 0; }

                step2 = startOpacity;
                if (step2 < 0) { step2 = 0; }

                output.AppendLine(".previous-lyrics.item-0 { opacity: " + step0 + "; }");
                output.AppendLine(".previous-lyrics.item-1 { opacity: " + step1 + "; }");
                output.AppendLine(".previous-lyrics.item-2 { opacity: " + step2 + "; }");
            }

            if (Helpers.BooleanValue(style.showUpcomingLyrics)) {
                startOpacity = style.upcomingLyricsOpacity.HasValue ? style.upcomingLyricsOpacity.Value : .3;
                opacityStep = style.upcomingLyricsOpacityStep.HasValue ? style.upcomingLyricsOpacityStep.Value : .1;

                step0 = startOpacity;
                if (step0 < 0) { step0 = 0; }

                step1 = startOpacity - opacityStep;
                if (step1 < 0) { step1 = 0; }

                step2 = startOpacity - opacityStep - opacityStep;
                if (step2 < 0) { step2 = 0; }

                output.AppendLine(".upcoming-lyrics.item-0 { opacity: " + step0 + "; }");
                output.AppendLine(".upcoming-lyrics.item-1 { opacity: " + step1 + "; }");
                output.AppendLine(".upcoming-lyrics.item-2 { opacity: " + step2 + "; }");
            }

            string align = !String.IsNullOrWhiteSpace(style.lyricsStyle.textAlign) ? style.lyricsStyle.textAlign.ToLower() : "center";
            string valign = !String.IsNullOrWhiteSpace(style.lyricsStyle.verticalAlign) ? style.lyricsStyle.verticalAlign.ToLower() : "middle";

            // Basic slide style for a song
            output.AppendLine("." + wrapperClass + " {");
            output.AppendLine("  position:absolute;");
            output.AppendLine("  top:0;");
            output.AppendLine("  left:0;");
            output.AppendLine("  width:100%;");
            output.AppendLine("  height:100%;");
            output.AppendLine("  display:flex;");

            // justify-content: start, center, end; valign
            // align-items: center; start, center, end - align

            switch (align) {
                case "left":
                    output.AppendLine("  align-items: start;");
                    break;

                case "right":
                    output.AppendLine("  align-items: end;");
                    break;

                default:
                    output.AppendLine("  align-items: center;");
                    break;
            }

            switch (valign) {
                case "top":
                    output.AppendLine("  justify-content:start;");
                    break;

                case "bottom":
                    output.AppendLine("  justify-content:end;");
                    break;

                default:
                    output.AppendLine("  justify-content:center;");
                    break;
            }

            output.AppendLine("  flex-direction: column;");
            output.AppendLine("  flex-wrap: nowrap;");
            output.AppendLine("  align-content: center;");
            output.AppendLine("}");
            output.AppendLine("");

            // Lyric-specific styles
            output.AppendLine("." + wrapperClass + " .view-lyrics-line {");
            output.AppendLine("  font-family:'" + style.lyricsStyle.fontFamily + "';");
            output.AppendLine("  font-size:" + (style.lyricsStyle.fontSize * Model.FontBaseMultiplier * Scale) + "vw;");
            output.AppendLine("  color:" + style.lyricsStyle.fontColor + ";");
            output.AppendLine("  text-align:" + align + ";");

            if (style.lyricsStyle.fontLineHeight >= 0.5) {
                output.AppendLine("  line-height:" + style.lyricsStyle.fontLineHeight.ToString() + "em;");
            } else {
                output.AppendLine("  line-height:1em;");
            }

            if (BooleanValue(style.lyricsStyle.fontBold)) {
                output.AppendLine("  font-weight:bold;");
            }

            if (BooleanValue(style.lyricsStyle.fontItalic)) {
                output.AppendLine("  font-style:italic;");
            }

            if (style.lyricsStyle.fontOutlineWidth > 0) {
                output.AppendLine("  -webkit-text-stroke:" + (style.lyricsStyle.fontOutlineWidth * 0.002 * Scale) + "vw " + style.lyricsStyle.fontOutlineColor + ";");
            }

            if (BooleanValue(style.lyricsStyle.fontShadow)) {
                string shadowColor = !String.IsNullOrWhiteSpace(style.lyricsStyle.fontShadowColor) ? style.lyricsStyle.fontShadowColor + " " : "#000 ";
                var shadowOffsetX = style.lyricsStyle.fontShadowOffsetX != 0 ? (style.lyricsStyle.fontShadowOffsetX * 0.02 * Scale) + "vw " : "0 ";
                var shadowOffsetY = style.lyricsStyle.fontShadowOffsetY != 0 ? (style.lyricsStyle.fontShadowOffsetY * 0.02 * Scale) + "vw " : "0 ";
                var shadowBlur = style.lyricsStyle.fontShadowBlur > 0 ? (style.lyricsStyle.fontShadowBlur * 0.02 * Scale) + "vw " : "";
                var textShadow = shadowColor + shadowOffsetX + shadowOffsetY + shadowBlur;
                output.AppendLine("  text-shadow: " + textShadow + ";");
            }

            output.AppendLine("}");

            if (style.lyricsStyle.opacity < 1) {
                output.AppendLine("");
                output.AppendLine("." + wrapperClass + " .view-lyrics-line:not(.previous-lyrics):not(.upcoming-lyrics) {");
                output.AppendLine("  opacity: " + style.lyricsStyle.opacity + ";");
                output.AppendLine("}");
            }
        }

        if (style.headerStyle != null) {
            // Header-specific styles
            output.AppendLine("");
            var headerAlign = !String.IsNullOrWhiteSpace(style.headerStyle.textAlign) ? style.headerStyle.textAlign.ToLower() : "center";
            output.AppendLine("." + wrapperClass + " .header {");
            output.AppendLine("  font-family:'" + style.headerStyle.fontFamily + "';");
            output.AppendLine("  font-size:" + (style.headerStyle.fontSize * Model.FontBaseMultiplier * Scale) + "vw;");
            output.AppendLine("  color:" + style.headerStyle.fontColor + ";");
            output.AppendLine("  text-align:" + headerAlign + ";");
            output.AppendLine("  width:100%;");
            output.AppendLine("  position:absolute;");
            output.AppendLine("  top:" + (style.headerOffset * 0.5 * Scale) + "vw;");

            if (style.headerStyle.opacity < 1) {
                output.AppendLine("  opacity: " + style.headerStyle.opacity + ";");
            }

            if (style.headerStyle.fontLineHeight >= 0.5) {
                output.AppendLine("  line-height:" + style.headerStyle.fontLineHeight.ToString() + "em;");
            } else {
                output.AppendLine("  line-height:1em;");
            }

            if (BooleanValue(style.headerStyle.fontBold)) {
                output.AppendLine("  font-weight:bold;");
            }

            if (BooleanValue(style.headerStyle.fontItalic)) {
                output.AppendLine("  font-style:italic;");
            }

            if (style.headerStyle.fontOutlineWidth > 0) {
                output.AppendLine("  -webkit-text-stroke:" + (style.headerStyle.fontOutlineWidth * 0.002 * Scale) + "vw " + style.headerStyle.fontOutlineColor + ";");
            }

            if (BooleanValue(style.headerStyle.fontShadow)) {
                string shadowColor = !String.IsNullOrWhiteSpace(style.headerStyle.fontShadowColor) ? style.headerStyle.fontShadowColor + " " : "#000 ";
                var shadowOffsetX = style.headerStyle.fontShadowOffsetX != 0 ? (style.headerStyle.fontShadowOffsetX * 0.02 * Scale) + "vw " : "0 ";
                var shadowOffsetY = style.headerStyle.fontShadowOffsetY != 0 ? (style.headerStyle.fontShadowOffsetY * 0.02 * Scale) + "vw " : "0 ";
                var shadowBlur = style.headerStyle.fontShadowBlur > 0 ? (style.headerStyle.fontShadowBlur * 0.02 * Scale) + "vw " : "";
                var textShadow = shadowColor + shadowOffsetX + shadowOffsetY + shadowBlur;
                output.AppendLine("  text-shadow: " + textShadow + ";");
            }

            output.AppendLine("}");
        }

        if (style.footerStyle != null) {
            // Footer-specific styles
            output.AppendLine("");
            var footerAlign = !String.IsNullOrWhiteSpace(style.footerStyle.textAlign) ? style.footerStyle.textAlign.ToLower() : "center";
            output.AppendLine("." + wrapperClass + " .footer {");
            output.AppendLine("  font-family:'" + style.footerStyle.fontFamily + "';");
            output.AppendLine("  font-size:" + (style.footerStyle.fontSize * Model.FontBaseMultiplier * Scale) + "vw;");
            output.AppendLine("  color:" + style.footerStyle.fontColor + ";");
            output.AppendLine("  text-align:" + footerAlign + ";");
            output.AppendLine("  width:100%;");
            output.AppendLine("  position:absolute;");
            output.AppendLine("  bottom:" + (style.footerOffset * 0.5 * Scale) + "vw;");

            if (style.footerStyle.opacity < 1) {
                output.AppendLine("  opacity: " + style.footerStyle.opacity + ";");
            }

            if (style.footerStyle.fontLineHeight >= 0.5) {
                output.AppendLine("  line-height:" + style.footerStyle.fontLineHeight.ToString() + "em;");
            } else {
                output.AppendLine("  line-height:1em;");
            }

            if (BooleanValue(style.footerStyle.fontBold)) {
                output.AppendLine("  font-weight:bold;");
            }

            if (BooleanValue(style.footerStyle.fontItalic)) {
                output.AppendLine("  font-style:italic;");
            }

            if (style.footerStyle.fontOutlineWidth > 0) {
                output.AppendLine("  -webkit-text-stroke:" + (style.footerStyle.fontOutlineWidth * 0.002 * Scale) + "vw " + style.footerStyle.fontOutlineColor + ";");
            }

            if (BooleanValue(style.footerStyle.fontShadow)) {
                string shadowColor = !String.IsNullOrWhiteSpace(style.footerStyle.fontShadowColor) ? style.footerStyle.fontShadowColor + " " : "#000 ";
                var shadowOffsetX = style.footerStyle.fontShadowOffsetX != 0 ? (style.footerStyle.fontShadowOffsetX * 0.02 * Scale) + "vw " : "0 ";
                var shadowOffsetY = style.footerStyle.fontShadowOffsetY != 0 ? (style.footerStyle.fontShadowOffsetY * 0.02 * Scale) + "vw " : "0 ";
                var shadowBlur = style.footerStyle.fontShadowBlur > 0 ? (style.footerStyle.fontShadowBlur * 0.02 * Scale) + "vw " : "";
                var textShadow = shadowColor + shadowOffsetX + shadowOffsetY + shadowBlur;
                output.AppendLine("  text-shadow: " + textShadow + ";");
            }

            output.AppendLine("}");
        }


        return output.ToString();
    }
    /// <summary>
    /// Formats a DateTime as a short date string.
    /// </summary>
    /// <param name="date">The DateTime value to format.</param>
    /// <param name="ReplaceSpaces">Option to replace spaces with non-breaking HTML characters.</param>
    /// <param name="ToLocalTimezone">Option to convert the DateTime from UTC to local time (defaults to true).</param>
    /// <returns>The formatted date.</returns>
    public static string FormatDate(DateTime? date, bool ReplaceSpaces = false, bool ToLocalTimezone = true)
    {
        string output = String.Empty;

        if (date.HasValue) {
            var d = (DateTime)date;

            if (ToLocalTimezone) {
                d = d.ToLocalTime();
            }

            output = d.ToShortDateString();

            if (ReplaceSpaces) {
                output = output.Replace(" ", "&nbsp;");
            }
        }

        return output;
    }

    public static string FormatAudioPlayTime(audioItemCurrentTime currentTime)
    {
        string output = String.Empty;

        if (currentTime.currentTime > 0) {
            var cs = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(currentTime.currentTime));
            var ts = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(currentTime.totalTime));

            output = cs.ToString("m:ss") + " / " + ts.ToString("m:ss");
        }

        return output;
    }

    /// <summary>
    /// Formats a chord.
    /// </summary>
    /// <param name="chord">The chord to format.</param>
    /// <param name="originalKey">The original key.</param>
    /// <param name="newKey">The selected key.</param>
    /// <returns>The formatted chord.</returns>
    public static string FormatChord(string? chord, string? originalKey, string newKey)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(chord)) {
            if (chord.StartsWith("!")) {
                // This is a comment
                output = "<span class=\"songformat-comment\">" + chord.Substring(1) + "&nbsp;</span>";
            } else {
                if (chord == "||:") {
                    output = "<div class='chord-image'><img src='" + Model.ApplicationUrl + "Images/RepeatStart.svg' /></div>";
                } else if (chord == ":||") {
                    output = "<div class='chord-image'><img src='" + Model.ApplicationUrl + "Images/RepeatEnd.svg' /></div>";
                } else if (chord == "|") {
                    output = "<div class='chord-image'><img src='" + Model.ApplicationUrl + "Images/Pipe.svg' /></div>";
                } else if (!chord.Contains("/")) {
                    // Just a chord, no bass part
                    output = "<span class='songformat-chord'>" + RenderChord(chord, originalKey, newKey) + "</span>";
                } else {
                    // We have a bass element
                    var ch = chord.Split('/');
                    output = "<span class='songformat-chord'>" + RenderChord(ch[0], originalKey, newKey) +
                        "/<span class='songformat-chordbass'>" + RenderChord(ch[1], originalKey, newKey, true) + "</span></span>";
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Formats a chord by replacing the sharp and flat symbols with the HTML equivalent.
    /// </summary>
    /// <param name="chord"></param>
    /// <returns>A formatted chord</returns>
    public static string FormatChordSpecialCharacters(string? chord)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(chord)) {
            var parts = chord.Split('/');
            if (parts != null && parts.Any()) {
                foreach (var part in parts) {
                    if (output != "") {
                        output += "/";
                    }

                    output += part
                        .Replace("sus", "{{SUSPENDED}}")
                        .Replace("maj", "{{MAJOR}}")
                        .Replace("dim", "{{DIMINISHED}}")
                        .Replace("#", "♯")
                        .Replace("b", "♭")
                        .Replace("{{SUSPENDED}}", "sus")
                        .Replace("{{MAJOR}}", "maj")
                        .Replace("{{DIMINISHED}}", "dim"); ;
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Formats a DateTime as a short date string.
    /// </summary>
    /// <param name="value">The string containing a DateTime value to format.</param>
    /// <param name="ReplaceSpaces">Option to replace spaces with non-breaking HTML characters.</param>
    /// <param name="ToLocalTimezone">Option to convert the DateTime from UTC to local time (defaults to true).</param>
    /// <returns>The formatted date.</returns>
    public static string FormatDate(string? value, bool ReplaceSpaces = false, bool ToLocalTimezone = true)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(value)) {
            try {
                var d = Convert.ToDateTime(value);

                if (ToLocalTimezone) {
                    d = d.ToLocalTime();
                }

                output = d.ToShortDateString();

                if (ReplaceSpaces) {
                    output = output.Replace(" ", "&nbsp;");
                }
            } catch { }
        }

        return output;
    }

    /// <summary>
    /// Formats a DateTime as a short date string and short time string.
    /// </summary>
    /// <param name="date">The DateTime to format.</param>
    /// <param name="ReplaceSpaces">Option to replace spaces with non-breaking HTML characters.</param>
    /// <param name="ToLocalTimezone">Option to convert the DateTime from UTC to local time (defaults to true).</param>
    /// <returns>The formatted date and time.</returns>
    public static string FormatDateTime(DateTime? date, bool ReplaceSpaces = false, bool ToLocalTimezone = true)
    {
        string output = String.Empty;

        if (date.HasValue) {
            DateTime d = (DateTime)date;

            if (ToLocalTimezone) {
                d = d.ToLocalTime();
            }

            output = d.ToShortDateString() + " " + d.ToShortTimeString();

            if (ReplaceSpaces) {
                output = output.Replace(" ", "&nbsp;");
            }
        }

        return output;
    }

    /// <summary>
    /// Formats a DateTime as a short date string and short time string.
    /// </summary>
    /// <param name="value">The string containing a DateTime value to format.</param>
    /// <returns>The formatted date and time.</returns>
    public static string FormatDateTime(string? value)
    {
        return FormatDateTime(value, false, true);
    }

    /// <summary>
    /// Formats a DateTime as a short date string and short time string.
    /// </summary>
    /// <param name="value">The string containing a DateTime value to format.</param>
    /// <param name="ReplaceSpaces">Option to replace spaces with non-breaking HTML characters.</param>
    /// <param name="ToLocalTimezone">Option to convert the DateTime from UTC to local time (defaults to true).</param>
    /// <returns>The formatted date and time.</returns>
    public static string FormatDateTime(string? value, bool ReplaceSpaces = false, bool ToLocalTimezone = true)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(value)) {
            try {
                var d = Convert.ToDateTime(value);
                if (ToLocalTimezone) {
                    d = d.ToLocalTime();
                }
                output = d.ToShortDateString() + " " + d.ToShortTimeString();
            } catch { }

            if (ReplaceSpaces) {
                output = output.Replace(" ", "&nbsp;");
            }
        }

        return output;
    }

    /// <summary>
    /// Formats a DateTime in the short time format.
    /// </summary>
    /// <param name="date">The DateTime object to be formatted.</param>
    /// <param name="Compressed">Option to compress the output (remove :00 and change AM to "a" and PM to "p")</param>
    /// <param name="ToLocalTimezone">Option to convert the DateTime from UTC to local time (defaults to true).</param>
    /// <returns>The DateTime in the short time format.</returns>
    public static string FormatTime(DateTime? date, bool Compressed = false, bool ToLocalTimezone = true)
    {
        string output = String.Empty;

        if (date.HasValue) {
            DateTime d = (DateTime)date;

            if (ToLocalTimezone) {
                d = d.ToLocalTime();
            }

            output = d.ToShortTimeString();

            if (Compressed) {
                output = output.Replace(":00", "")
                    .Replace(" ", "")
                    .Replace("AM", "a")
                    .Replace("PM", "p");
            }
        }

        return output;
    }

    public static async Task GetConfirmation(
        Delegate OnConfirmed,
        string Title = "",
        string Instructions = "",
        string ButtonTextOk = "",
        string ButtonTextCancel = "",
        string ButtonClassOk = "",
        string ButtonClassCancel = ""
    )
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        parameters.Add("OnConfirmed", OnConfirmed);

        if (!String.IsNullOrWhiteSpace(Instructions)) {
            parameters.Add("Instructions", Instructions);
        }

        if (!String.IsNullOrWhiteSpace(ButtonTextOk)) {
            parameters.Add("ButtonTextOk", ButtonTextOk);
        }

        if (!String.IsNullOrWhiteSpace(ButtonTextCancel)) {
            parameters.Add("ButtonTextCancel", ButtonTextCancel);
        }

        if (!String.IsNullOrWhiteSpace(ButtonClassOk)) {
            parameters.Add("ButtonClassOk", ButtonClassOk);
        }

        if (!String.IsNullOrWhiteSpace(ButtonClassCancel)) {
            parameters.Add("ButtonClassCancel", ButtonClassCancel);
        }

        await DialogService.OpenAsync<GetConfirmationDialog>(Title, parameters, new Radzen.DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = true,
            ShowClose = true,
        });
    }

    /// <summary>
    /// Gets user input.
    /// </summary>
    /// <param name="OnInputAccepted">The delegate that will be invoke with the results of the user input.</param>
    /// <param name="UserInputType">The type of input to get from the user.</param>
    /// <param name="Title">The title of the input dialog.</param>
    /// <param name="Id">A id to add to the input element.</param>
    /// <param name="DefaultValue">The default value to use in input elements that support it.</param>
    /// <param name="Instructions">Any instructions to show before the input element.</param>
    /// <param name="Class">A class to add to the input elements for elements that support it.</param>
    /// <param name="MultiSelectRows">For multiselect elements the number of rows to show.</param>
    /// <param name="PlaceholderText">Placeholder text for elements that support it.</param>
    /// <param name="SetFocus">Option to set the focus to the element.</param>
    /// <param name="UserInputOptions">Any options for input elements that support options. The first element is the value and the second is the label.</param>
    /// <param name="width">A width for the dialog.</param>
    /// <param name="height">A height for the dialog.</param>
    /// <returns>The results of the user input. Depending on the input type this will either be a string or a list of string.</returns>
    public static async Task GetInput(Delegate OnInputAccepted,
        FreeBlazor.GetInput.InputType UserInputType = FreeBlazor.GetInput.InputType.Text,
        string Title = "",
        string Id = "",
        string DefaultValue = "",
        string Instructions = "",
        string Class = "",
        int? MultiSelectRows = null,
        string PlaceholderText = "",
        bool SetFocus = false,
        Dictionary<string, string>? UserInputOptions = null,
        string width = "",
        string height = "")
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("OnInputAccepted", OnInputAccepted);

        parameters.Add("UserInputType", UserInputType);

        if (!String.IsNullOrWhiteSpace(Id)) {
            parameters.Add("Id", Id);
        }

        if (!String.IsNullOrWhiteSpace(DefaultValue)) {
            parameters.Add("DefaultValue", DefaultValue);
        }

        if (!String.IsNullOrWhiteSpace(Instructions)) {
            parameters.Add("Instructions", Instructions);
        }

        if (!String.IsNullOrWhiteSpace(Class)) {
            parameters.Add("Class", Class);
        }

        if (MultiSelectRows.HasValue) {
            parameters.Add("MultiSelectRows", (int)MultiSelectRows);
        }

        if (!String.IsNullOrWhiteSpace(PlaceholderText)) {
            parameters.Add("PlaceholderText", PlaceholderText);
        }

        parameters.Add("SetFocus", SetFocus);

        if (UserInputOptions != null && UserInputOptions.Any()) {
            parameters.Add("UserInputOptions", UserInputOptions);
        }

        if (width == "auto") {
            width = "";
        }

        if (height == "auto") {
            height = "";
        }

        await DialogService.OpenAsync<GetInputDialog>(Title, parameters, new DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            Width = width,
            Height = height,
        });
    }

    /// <summary>
    /// Gets the key martix for the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>A nullable KeyMatrix object.</returns>
    public static keyMatrix? GetKeyMatrix(string? key)
    {
        keyMatrix? output = null;

        if (!String.IsNullOrWhiteSpace(key)) {
            string matrixKey = FormatChordSpecialCharacters(key);
            if (!String.IsNullOrWhiteSpace(matrixKey)) {
                output = KeyMatrixItems.FirstOrDefault(x => x.key == matrixKey);
            }
        }


        return output;
    }

    /// <summary>
    /// Gets a local storage item.
    /// </summary>
    /// <typeparam name="T">The type of the item.</typeparam>
    /// <param name="key">the key of the item.</param>
    /// <returns>A nullable object of type T.</returns>
    public static async Task<T?> GetLocalStorageItem<T>(string key)
    {
        T? output = default(T);

        if (LocalStorage != null) {
            try {
                var result = await LocalStorage.GetItemAsync<T>(key);
                if (result != null) {
                    output = result;
                }
            } catch (Exception ex) {
                if (ex != null) {

                }
            }
        }

        return output;
    }

    /// <summary>
    /// Gets the typed object for an object that comes back from an API endpoint as a System.Text.Json type of object.
    /// </summary>
    /// <typeparam name="T">The type of object to return.</typeparam>
    /// <param name="obj">The object to cast as a type of T</param>
    /// <returns>If the object is not null then the object is returned as a type of T, otherwise, the default value is returned.</returns>
    public static T? GetObjectAsType<T>(object? obj)
    {
        T? output = default(T);

        if (obj != null) {
            try {
                output = DeserializeObject<T>(SerializeObject(obj));
            } catch { }
        }

        return output;
    }

    public static string GetObjectType(object? o)
    {
        string output = String.Empty;

        if (o != null) {
            output = o.GetType()
                .ToString()
                .Replace("+", ".");
        }

        return output;
    }

    /// <summary>
    /// Gets data from an API endpoint using either the get or post method. If a post object is supplied post will be used. Otherwise, get will be used.
    /// </summary>
    /// <typeparam name="T">The type of the object to return.</typeparam>
    /// <param name="url">The API endpoint URL.</param>
    /// <param name="post">An optional object to post to the endpoint.</param>
    /// <param name="logResults">An option to log the results to the console.</param>
    /// <returns>A nullable object of type T.</returns>
    public static async Task<T?> GetOrPost<T>(string url, object? post = null, bool logResults = false)
    {
        T? output = default(T);

        if (Http != null) {
            try {
                HttpResponseMessage? response = null;

                Http.DefaultRequestHeaders.Clear();

                if (post != null) {
                    response = await Http.PostAsJsonAsync(url, post);
                } else {
                    response = await Http.GetAsync(url);
                }

                if (response != null) {
                    if (response.IsSuccessStatusCode) {
                        var content = await response.Content.ReadAsStringAsync();

                        if (logResults && url.ToLower() != "api/data/getversioninfo") {
                            await ConsoleLog("GetOrPostResult", url, content);
                        }

                        if (!String.IsNullOrWhiteSpace(content)) {
                            if (content.ToUpper().StartsWith("<!DOCTYPE") && typeof(T) != typeof(String)) {
                                await ConsoleLog("Not a valid API endpoint - " + url);
                            } else {
                                if (typeof(T) == typeof(string)) {
                                    output = (T)(object)content;
                                } else {
                                    var result = await response.Content.ReadFromJsonAsync<T>();
                                    if (result != null) {
                                        output = result;
                                    }
                                }
                            }
                        }
                    } else {
                        await ConsoleLog("The Server Returned an Error Calling '" + url + "'");
                        await ConsoleLog("Status Code: " + response.StatusCode.ToString());
                        if (!String.IsNullOrWhiteSpace(response.ReasonPhrase)) {
                            Console.Write("Reason Phrase: " + response.ReasonPhrase);
                        }
                    }
                }
            } catch(Exception ex) {
                await ConsoleLog("An Exception Occurred Calling '" + url + "' - " + ex.Message);
            }
        }

        return output;
    }

    /// <summary>
    /// Gets a value from the querystring.
    /// </summary>
    /// <param name="key">The key of the element.</param>
    /// <returns>The value for the given key.</returns>
    public static string GetQuerystringValue(string? key)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(key)) {
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            output = GetQuerystringValue(uri.ToString(), key);
        }

        if (output.ToLower() == "undefined" || output.ToLower() == "null") {
            output = String.Empty;
        }

        return output;
    }

    /// <summary>
    /// Gets a value from the querystring of the provided url.
    /// </summary>
    /// <param name="fullUrl">The full url.</param>
    /// <param name="key">The key of the element.</param>
    /// <returns>The value for the given key.</returns>
    public static string GetQuerystringValue(string? fullUrl, string? key)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(fullUrl) && !String.IsNullOrWhiteSpace(key)) {
            var qs = GetQuerystringValues(fullUrl);
            if (qs != null && qs.Any()) {
                var item = qs.FirstOrDefault(x => x.Key.ToLower() == key.ToLower());
                if (!String.IsNullOrWhiteSpace(item.Value)) {
                    output = item.Value;
                }
            }
        }

        if (output.ToLower() == "undefined" || output.ToLower() == "null") {
            output = String.Empty;
        }

        return output;
    }

    /// <summary>
    /// Gets all elements from the querystring as a dictionary collection.
    /// </summary>
    /// <param name="fullUrl">The full url.</param>
    /// <returns>A dictionary of string, string where the first element is the key and the second contains any values.</returns>
    public static Dictionary<string, string> GetQuerystringValues(string? fullUrl)
    {
        Dictionary<string, string> output = new Dictionary<string, string>();

        if (!String.IsNullOrWhiteSpace(fullUrl) && fullUrl.Contains("?")) {
            string qs = fullUrl.Substring(fullUrl.IndexOf("?") + 1);
            string[] parts = qs.Split("&");
            foreach (var part in parts) {
                if (part.Contains("=")) {
                    var values = part.Split("=");
                    string key = "";
                    string value = "";

                    try {
                        key = values[0];
                        value = values[1];
                    } catch { }

                    if (!String.IsNullOrWhiteSpace(key) && !String.IsNullOrWhiteSpace(value)) {
                        if (!output.ContainsKey(key)) {
                            output.Add(key, value);
                        }
                    }
                }
            }
        }

        return output;
    }

    public static song? GetSong(Guid songId, Guid? songbookId)
    {
        song? output = null;
        var songbook = GetSongBook(songbookId);
        if (songbook != null) {
            output = songbook.songs.FirstOrDefault(x => x.id == songId);
        }
        return output;
    }

    public static songBook? GetSongBook(Guid? songbookId)
    {
        var output = Model.SongBooks.FirstOrDefault(x => x.id == songbookId);
        return output;
    }

    public static List<string> GetStaticClassPublicStringProperties(System.Type staticType)
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

    /// <summary>
    /// Gets a typed value from a string value.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The value as a type of T.</returns>
    public static T? GetValueFromString<T>(string? value)
    {
        var output = default(T);

        if (!String.IsNullOrWhiteSpace(value)) {
            try {
                if (typeof(T) == typeof(string)) {
                    output = (T)(object)value;
                } else if (typeof(T) == typeof(bool)) {
                    bool boolValue = value.ToLower() == "true";
                    output = (T)(object)boolValue;
                } else if (typeof(T) == typeof(int)) {
                    output = (T)(object)Convert.ToInt32(value);
                } else if (typeof(T) == typeof(Int32)) {
                    output = (T)(object)Convert.ToInt32(value);
                } else if (typeof(T) == typeof(Int64)) {
                    output = (T)(object)Convert.ToInt64(value);
                } else if (typeof(T) == typeof(long)) {
                    output = (T)(object)Convert.ToInt64(value);
                } else if (typeof(T) == typeof(decimal)) {
                    output = (T)(object)Convert.ToDecimal(value);
                } else {
                    output = (T)(object)value;
                }
            } catch { }
        }

        return output;
    }

    /// <summary>
    /// Gets the Guid value or Guid.Empty if there is no value.
    /// </summary>
    /// <param name="value">A nullable Guid value.</param>
    /// <returns>The Guid value or Guid.Empty if there is no value.</returns>
    public static Guid GuidValue(Guid? value)
    {
        var output = value.HasValue ? value.Value : Guid.Empty;
        return output;
    }

    /// <summary>
    /// Gets the Guid value from a string that might contain a Guid.
    /// </summary>
    /// <param name="value">A string that might be a Guid value.</param>
    /// <returns>The Guid value or Guid.Empty if there was no value.</returns>
    public static Guid GuidValue(string? value)
    {
        Guid output = Guid.Empty;

        if (!String.IsNullOrWhiteSpace(value)) {
            try {
                output = new Guid(value);
            } catch { }
        }

        return output;
    }

    /// <summary>
    /// Hides any menus that are showing.
    /// </summary>
    public static async Task HideMenus()
    {
        await jsRuntime.InvokeVoidAsync("HideMenu");
    }

    /// <summary>
    /// Highlights HTML elements that have a given class name.
    /// </summary>
    /// <param name="className">The class name to find.</param>
    public static async Task HighlightElementByClass(string className)
    {
        await jsRuntime.InvokeVoidAsync("HighlightElementByClass", className);
    }

    /// <summary>
    /// Converts hyphenated lyrics to a table.
    /// </summary>
    /// <param name="lyrics"></param>
    /// <returns></returns>
    public static string HyphenatedLyricsToTable(string? lyrics)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(lyrics)) {
            output = "<table class=\"hyphenated-lyrics-table\"><tbody><tr>";

            if (lyrics == "-") {
                output += "<td class=\"hyphen\"><div class=\"hyphen\"></div></td>";
            } else {
                var items = lyrics.Split('-');
                foreach (var item in items) {
                    if (!String.IsNullOrWhiteSpace(item)) {
                        //output += "<td>" + item.Trim() + "</td><td class=\"hyphen\"><div class=\"hyphen\"></div></td>";
                        output += "<td class=\"hyphen\"><div class=\"hyphen\"></div></td><td>" + item.Trim() + "</td>";
                    }
                }
            }

            output += "</tr></tbody></table>";
        }

        return output;
    }

    /// <summary>
    /// Returns the value for a given icon.
    /// </summary>
    /// <param name="name">The name of the icon.</param>
    /// <param name="WrapInElement">Option to wrap the icon in an HTML element for rendering.</param>
    /// <returns>The icon.</returns>
    public static string Icon(string? name, bool WrapInElement = true, string AddClass = "")
    {
        string output = String.Empty;

        string _addClass = !String.IsNullOrWhiteSpace(AddClass) ? " " + AddClass : "";

        if (!String.IsNullOrWhiteSpace(name)) {
            var icon = Icons.FirstOrDefault(x => x.Value.Contains(name.Trim(), StringComparer.InvariantCultureIgnoreCase));
            if (!String.IsNullOrWhiteSpace(icon.Key)) {
                string key = icon.Key;
                string source = String.Empty;

                if (key.StartsWith("google:")) {
                    source = "google";
                    key = key.Substring(7);
                } else if (key.StartsWith("fa:")) {
                    source = "fa";
                    key = key.Substring(3);
                } else if (key.StartsWith("bi:")) {
                    source = "bi";
                    key = key.Substring(3);
                } else if (key.StartsWith("svg:")) {
                    source = "svg";
                    key = key.Substring(4);
                }

                key += " " + name.ToLower();

                if (WrapInElement) {
                    switch (source) {
                        case "bi":
                        case "fa":
                            output = "<i class=\"icon " + key + _addClass + "\"></i>";
                            break;

                        case "google":
                            output = "<i class=\"icon material-symbols-outlined" + _addClass + "\">" + key + "</i>";
                            break;

                        case "svg":
                            output = "<i class=\"icon svg" + _addClass + "\"><img src=\"" + Model.ApplicationUrl + "images/icons/svg/" + key + ".svg\" /></i>";
                            break;
                    }


                } else {
                    output = key;
                }
            }
        }

        return output;
    }

    /// <summary>
    /// The collection of icons for the application.
    /// </summary>
    public static Dictionary<string, List<string>> Icons {
        get {
            // Icons names are listed as the first item, and any matching text is included in the List<string> object.
            // Icons can come from various sources, so the first part of the icon name indicates the source,
            // then the second part indicates the source (eg: google:home, fa:fa fa-home, etc.)
            Dictionary<string, List<string>> icons =    new Dictionary<string, List<string>> {
                // https://icons.getbootstrap.com/
                

                // https://fontawesome.com/search?ic=free
                { "fa:fa-brands fa-youtube",                     new List<string> { "YouTube" }},
                { "fa:fa-regular fa-file-audio",                 new List<string> { "Audio" }},
                { "fa:fa-regular fa-circle-check",               new List<string> { "Done", "OK", "Insert", "Select", "Selected" }},
                { "fa:fa-regular fa-circle-left",                new List<string> { "Back" }},
                { "fa:fa-regular fa-circle-right",               new List<string> { "Continue" }},
                { "fa:fa-regular fa-circle-xmark",               new List<string> { "Cancel", "Close", "CloseDialog", "Hide", "ZoomReset" }},
                { "fa:fa-regular fa-copy",                       new List<string> { "Copy", "CopyToClipboard", "Duplicate" }},
                { "fa:fa-regular fa-rectangle-list",             new List<string> { "NewSetList", "SetList" }},
                { "fa:fa-regular fa-sun",                        new List<string> { "Theme", "ThemeLight" }},
                { "fa:fa-solid fa-align-center",                 new List<string> { "AlignCenter" }},
                { "fa:fa-solid fa-align-left",                   new List<string> { "AlignLeft" }},
                { "fa:fa-solid fa-align-right",                  new List<string> { "AlignRight" }},
                { "fa:fa-solid fa-arrows-down-to-line",          new List<string> { "AlignBottom" }},
                { "fa:fa-solid fa-arrows-rotate",                new List<string> { "Refresh", "Reload" }},
                { "fa:fa-solid fa-arrows-up-to-line",            new List<string> { "AlignTop" }},
                { "fa:fa-solid fa-ban",                          new List<string> { "Blank" }},
                { "fa:fa-solid fa-bold",                         new List<string> { "Bold" }},
                { "fa:fa-solid fa-book",                         new List<string> { "SongBook" }},
                { "fa:fa-solid fa-book-open",                    new List<string> { "AddSongBook", "SongBookOpen" }},
                { "fa:fa-solid fa-broom",                        new List<string> { "Clear", "Reset" }},
                { "fa:fa-solid fa-circle-half-stroke",           new List<string> { "ThemeAuto" }},
                { "fa:fa-solid fa-circle-info",                  new List<string> { "About", "Info" }},
                { "fa:fa-solid fa-clock",                        new List<string> { "Clock" }},
                { "fa:fa-solid fa-file-import",                  new List<string> { "Import" }},
                { "fa:fa-solid fa-file-lines",                   new List<string> { "SheetMusic" }},
                { "fa:fa-solid fa-file-medical",                 new List<string> { "AddToSetList" }},
                { "fa:fa-solid fa-film",                         new List<string> { "Video" }},
                { "fa:fa-solid fa-floppy-disk",                  new List<string> { "Save" }},
                { "fa:fa-solid fa-folder",                       new List<string> { "FolderClosed" }},
                { "fa:fa-solid fa-folder-open",                  new List<string> { "FolderOpen" }},
                { "fa:fa-solid fa-grip-lines",                   new List<string> { "AlignMiddle" }},
                { "fa:fa-solid fa-images",                       new List<string> { "Slideshow" }},
                { "fa:fa-solid fa-image",                        new List<string> { "Image" }},
                { "fa:fa-solid fa-italic",                       new List<string> { "Italic" }},
                { "fa:fa-solid fa-language",                     new List<string> { "Language" }},
                { "fa:fa-solid fa-magnifying-glass",             new List<string> { "Search", "View" }},
                { "fa:fa-solid fa-magnifying-glass-minus",       new List<string> { "ZoomOut" }},
                { "fa:fa-solid fa-magnifying-glass-plus",        new List<string> { "ZoomIn" }},
                { "fa:fa-solid fa-message",                      new List<string> { "Messaging" }},
                { "fa:fa-solid fa-moon",                         new List<string> { "ThemeDark" }},
                { "fa:fa-solid fa-music",                        new List<string> { "AddSong", "Chords", "Music", "Song" }},
                { "fa:fa-solid fa-paintbrush",                   new List<string> { "Style" }},
                { "fa:fa-solid fa-paper-plane",                  new List<string> { "Send" }},
                { "fa:fa-solid fa-pause",                        new List<string> { "Pause" }},
                { "fa:fa-solid fa-pen",                          new List<string> { "EditMode" }},
                { "fa:fa-solid fa-pen-to-square",                new List<string> { "Edit" }},
                { "fa:fa-solid fa-play",                         new List<string> { "Play" }},
                { "fa:fa-solid fa-rectangle-list",               new List<string> { "List" }},
                { "fa:fa-solid fa-sliders",                      new List<string> { "Settings" }},
                { "fa:fa-solid fa-square-plus",                  new List<string> { "Add", "AddItem", "Collapsed" }},
                { "fa:fa-solid fa-square-minus",                 new List<string> { "Expanded" }},
                { "fa:fa-solid fa-stop",                         new List<string> { "Stop" }},
                { "fa:fa-solid fa-stopwatch",                    new List<string> { "Countdown" }},
                { "fa:fa-solid fa-tablet-screen-button",         new List<string> { "Tablet" }},
                { "fa:fa-solid fa-text-height",                  new List<string> { "AutoTextSize" }},
                { "fa:fa-solid fa-text-slash",                   new List<string> { "HideText" }},
                { "fa:fa-solid fa-trash-can",                    new List<string> { "ConfirmDelete", "Delete" }},
                { "fa:fa-solid fa-tv",                           new List<string> { "Present", "Screen" }},
                { "fa:fa-solid fa-up-down-left-right",           new List<string> { "Move" }},
                { "fa:fa-solid fa-user",                         new List<string> { "User" }},

            };

            return icons;
        }
    }

    public static async Task Import(
        Delegate OnImportComplete,
        string Title,
        string Type,
        string UploadInstructions = "",
        bool AllowMultipleUploads = false,
        List<string>? SupportedFileTypes = null)
    {
        string _title = !String.IsNullOrWhiteSpace(Title) ? Title : Text.Import;

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("OnUploadComplete", OnImportComplete);
        parameters.Add("Type", Type);

        if (!String.IsNullOrWhiteSpace(UploadInstructions)) {
            parameters.Add("UploadInstructions", UploadInstructions);
        }
        
        if (SupportedFileTypes != null && SupportedFileTypes.Any()) {
            parameters.Add("SupportedFileTypes", SupportedFileTypes);
        }

        var options = new Radzen.DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = true,
            ShowClose = true,
            Width = "98%",
            Top = _dialogTop,
        };

        if (AllowMultipleUploads) {
            await DialogService.OpenAsync<ImportItem<IReadOnlyList<IBrowserFile>>>(_title, parameters, options);
        } else {
            await DialogService.OpenAsync<ImportItem<IBrowserFile>>(_title, parameters, options);
        }
    }

    public static async Task ImportPowerPoint()
    {
        // First, close the EditSetListItem dialog.
        DialogService.Close();

        Delegate onImportComplete = async (fileItem file) => {
            if (file.value != null && !String.IsNullOrWhiteSpace(file.fileName)) {
                var result = await GetOrPost<booleanResponse>("api/ConvertPowerPointToSlides", file);

                if (result != null) {
                    if (result.result) {
                        // Add the new slideshow to the set list.
                        InsertSetListItem(new setListItem { 
                            id = Guid.NewGuid(),
                            item = new slideshowItem { 
                                folder = Tools.GetFileNameWithoutExtension(file.fileName),
                                transitionSpeed = Model.Settings.transitionSpeed,
                            },
                            type = setListItemType.slideshow,
                        });
                    } else {
                        if (result.messages.Count > 0) {
                            Model.ErrorMessages(result.messages);
                        } else {
                            Model.UnknownError();
                        }
                    }
                } else {
                    Model.UnknownError();
                }
            }
        };

        await Helpers.Import(onImportComplete, Text.ImportPowerPoint, setListItemType.slideshow, Text.ImportPowerPointInfo, true, new List<string> { ".ppt", ".pptx" });
    }

    /// <summary>
    /// Indicates if this helpers class has been initialized.
    /// </summary>
    public static bool Initialized {
        get {
            return _initialized;
        }
    }

    /// <summary>
    /// Uses Javascript Interop to insert a value at the cursor in a given element.
    /// </summary>
    /// <param name="elementId">The id of the element.</param>
    /// <param name="value">The value to insert.</param>
    public static async Task InsertAtCursor(string? elementId, string? value)
    {
        if (!String.IsNullOrWhiteSpace(elementId) && !String.IsNullOrWhiteSpace(value)) {
            await jsRuntime.InvokeVoidAsync("InsertAtCursor", elementId, value);
        }
    }

    public static void InsertSetListItem(setListItem addItem)
    {
        // Insert the item after the currently selected item, or at the end of the list if no item is selected.
        bool added = false;
        bool setListWasEmpty = Model.EmptySetList;

        if (addItem.id == Guid.Empty) {
            addItem.id = Guid.NewGuid();
        }

        var items = new List<setListItem>();

        if (Model.SetList.selectedItem.HasValue) {
            foreach(var item in Model.SetList.items) {
                items.Add(item);

                if (item.id == Model.SetList.selectedItem) {
                    items.Add(addItem);
                    added = true;
                }
            }
        }

        if (!added) {
            // Just add to the end.
            items.Add(addItem);
        }

        Model.SetList.items = items;

        if (setListWasEmpty) {
            Model.SetList.selectedItem = addItem.id;
            Model.SetList.activeItem = addItem.id;
        }

        Model.SetList.saveRequired = true;

        Model.TriggerUpdate();
    }

    public static void InsertSetListItems(List<setListItem> items)
    {
        foreach(var item in items) {
            InsertSetListItem(item);
        }
    }

    public static int IntValue(int? value)
    {
        return value.HasValue ? value.Value : 0;
    }

    /// <summary>
    /// Indicates if the value contains a valid DateTime.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is a valid DateTime.</returns>
    public static bool IsDate(string value)
    {
        bool output = false;

        if (!String.IsNullOrWhiteSpace(value)) {
            try {
                DateTime d = Convert.ToDateTime(value);
                output = true;
            } catch { }
        }

        return output;
    }

    /// <summary>
    /// Indicates if the value contains a valid Guid.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value is a valid Guid.</returns>
    public static bool IsGuid(string value)
    {
        bool output = false;

        if (!String.IsNullOrWhiteSpace(value)) {
            Guid? g = null;
            try {
                g = new Guid(value);
            } catch { }
            output = g != null;
        }

        return output;
    }

    /// <summary>
    /// Indicates if the value contains a valid integer.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value contains a valid integer.</returns>
    public static bool IsInt(string value)
    {
        bool output = false;

        if (!String.IsNullOrWhiteSpace(value) && !value.Contains(".")) {
            int? v = null;

            try {
                v = Convert.ToInt32(value);
            } catch { }

            output = v != null;
        }

        return output;
    }

    /// <summary>
    /// Indicates if the value contains a valid number.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True if the value contains a valid number.</returns>
    public static bool IsNumeric(string? value)
    {
        bool output = false;

        if (!String.IsNullOrWhiteSpace(value)) {
            if (value.Contains(".")) {
                try {
                    decimal d = Convert.ToDecimal(value);
                    output = true;
                } catch { }
            }

            if (!output) {
                try {
                    int i = Convert.ToInt32(value);
                    output = true;
                } catch { }
            }
        }

        return output;
    }

    public static string ItemType(object? item)
    {
        if (item != null) {
            try {
                var type = item.GetType();

                if (type == typeof(song)) {
                    return setListItemType.song;
                } else if (type == typeof(setListItem)) {
                    var setListItem = (setListItem)item;
                    if (setListItem != null) {
                        return setListItem.type;
                    }
                }

                // Eventually there will be other types.
            } catch { }
        }

        return String.Empty;
    }

    /// <summary>
    /// The key matrix that stores chords for each key.
    /// </summary>
    public static List<keyMatrix> KeyMatrixItems {
        get {
            var output = new List<keyMatrix> {
                new keyMatrix{ key = "A♭", preferSharp = false, items = new List<string>{ "{A♭}", "{A}", "{B♭}{A♯}", "{B}", "{C}", "{D♭}{C♯}", "{D}", "{E♭}{D♯}", "{E}", "{F}", "{G♭}{F♯}", "{G}" }},
                new keyMatrix{ key = "A", preferSharp = true,  items = new List<string>{ "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}" }},
                new keyMatrix{ key = "A♯", preferSharp = true,  items = new List<string>{ "{A♯}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}" }},
                new keyMatrix{ key = "B♭", preferSharp = false, items = new List<string>{ "{B♭}", "{B}", "{C}", "{D♭}{C♯}", "{D}", "{E♭}{D♯}", "{E}", "{F}", "{G♭}{F♯}", "{G}", "{A♭}{G♯}", "{A}" }},
                new keyMatrix{ key = "B", preferSharp = true,  items = new List<string>{ "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}" }},
                new keyMatrix{ key = "B♯",preferSharp = true,  items = new List<string>{ "{B♯}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}" }},
                new keyMatrix{ key = "C♭", preferSharp = false, items = new List<string>{ "{C♭}", "{C}", "{D♭}{C♯}", "{D}", "{E♭}{D♯}", "{E}", "{F}", "{G♭}{F♯}", "{G}", "{A♭}{G♯}", "{A}", "{B♭}{A♯}" }},
                new keyMatrix{ key = "C", preferSharp = true,  items = new List<string>{ "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}" }},
                new keyMatrix{ key = "C♯", preferSharp = true,  items = new List<string>{ "{C♯}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}" }},
                new keyMatrix{ key = "D♭", preferSharp = false, items = new List<string>{ "{D♭}", "{D}", "{E♭}{D♯}", "{E}", "{F}", "{G♭}{F♯}", "{G}", "{A♭}{G♯}", "{A}", "{B♭}{A♯}", "{B}", "{C}" }},
                new keyMatrix{ key = "D", preferSharp = true,  items = new List<string>{ "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}" }},
                new keyMatrix{ key = "D♯", preferSharp = true,  items = new List<string>{ "{D♯}", "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}" }},
                new keyMatrix{ key = "E♭", preferSharp = false, items = new List<string>{ "{E♭}", "{E}", "{F}", "{G♭}{F♯}", "{G}", "{A♭}{G♯}", "{A}", "{B♭}{A♯}", "{B}", "{C}", "{D♭}{C♯}", "{D}" }},
                new keyMatrix{ key = "E", preferSharp = true,  items = new List<string>{ "{E}", "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}" }},
                new keyMatrix{ key = "E♯", preferSharp = true,  items = new List<string>{ "{E♯}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}" }},
                new keyMatrix{ key = "F♭", preferSharp = false, items = new List<string>{ "{F♭}", "{G♭}{F♯}", "{G}", "{A♭}{G♯}", "{A}", "{B♭}{A♯}", "{B}", "{C}", "{D♭}{C♯}", "{D}", "{E♭}{D♯}", "{E}" }},
                new keyMatrix{ key = "F", preferSharp = true,  items = new List<string>{ "{F}", "{F♯}{G♭}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}" }},
                new keyMatrix{ key = "F♯", preferSharp = true,  items = new List<string>{ "{F♯}", "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}" }},
                new keyMatrix{ key = "G♭", preferSharp = false, items = new List<string>{ "{G♭}", "{G}", "{A♭}{G♯}", "{A}", "{B♭}{A♯}", "{C♭}{B}", "{C}", "{D♭}{C♯}", "{D}", "{E♭}{D♯}", "{E}", "{F}" }},
                new keyMatrix{ key = "G", preferSharp = true,  items = new List<string>{ "{G}", "{G♯}{A♭}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}" }},
                new keyMatrix{ key = "G♯", preferSharp = true,  items = new List<string>{ "{G♯}", "{A}", "{A♯}{B♭}", "{B}", "{C}", "{C♯}{D♭}", "{D}", "{D♯}{E♭}", "{E}", "{F}", "{F♯}{G♭}", "{G}" }},
            };

            return output;
        }
    }

    /// <summary>
    /// Loads the static Text class with any saved language from a JSON language file.
    /// The method returns and updated version of the JSON to be saved in the event that
    /// there are new language items that need to be saved.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static void LoadAndUpdateLanguage(Language language)
    {
        // Update any of the static language items with the values from the language object.
        var propertyNames = GetStaticClassPublicStringProperties(typeof(Text));

        foreach (var item in propertyNames) {
            string value = String.Empty;
            var languageProperty = language.GetType().GetProperty(item);
            if (languageProperty != null) {
                var v = languageProperty.GetValue(language);
                if (v != null && !String.IsNullOrWhiteSpace(v.ToString())) {
                    value += v.ToString();
                }
            }

            if (!String.IsNullOrWhiteSpace(value)) {
                var staticProperty = typeof(Text).GetProperty(item);
                if (staticProperty != null) {
                    staticProperty.SetValue(null, value);
                }
            }
        }
    }

    public static async Task<song> LoadLastSong()
    {
        var output = new song();

        var lastSong = await GetOrPost<song>("api/GetLastSong");
        if (lastSong != null) {
            output = lastSong;
        }

        return output;
    }

    public static async Task LoadCachedMessages()
    {
        var messages = await GetOrPost<messages>("api/GetCachedMessages");
        if (messages != null) {
            Model.MessageItems = messages;
        }
    }

    public static async Task LoadCachedSetList()
    {
        var setList = await GetOrPost<setList>("api/GetCachedSetList");
        if (setList != null && setList.items.Count > 0) {
            if (Model.SetList.items.Count == 0) {
                Model.SetList = ReloadSetListItemsFromJson(setList);
            } else if (Model.SetList.activeItem == null) {
                Model.SetList.activeItem = setList.activeItem;
                Model.SetList.activeItemPart = setList.activeItemPart;
            }

            // If the active item is a song, load the song into Model.Song if no song is loaded.
            if (Model.Song.id == Guid.Empty && Model.ActiveItemType == setListItemType.song && Model.ActiveItem != null) {
                var song = Tools.SetListItemAsSong(Model.ActiveItem);
                if (song != null) {
                    Model.Song = song;
                }
            }
        }
    }

    public static async Task LoadSetList(string filename)
    {
        var setList = await GetOrPost<setList>("api/GetSetList", new simplePost { singleItem = filename });
        if (setList != null) {
            Model.SetList = ReloadSetListItemsFromJson(setList);
            Model.Settings.lastSetList = filename;

            await GetOrPost<booleanResponse>("api/SetCachedSetList", Model.SetList);

            await SaveSettings();
        }
    }

    public static async Task<slideshowItem?> LoadSlideshow(string folder)
    {
        var output = await GetOrPost<slideshowItem>("api/GetSlideshow/" + folder);
        return output;
    }

    /// <summary>
    /// Determines if the lyrics contain chords.
    /// </summary>
    /// <param name="lyrics">The lyrics to check.</param>
    /// <returns>True if standard chords are found.</returns>
    public static bool LyricsContainChords(string? lyrics)
    {
        if (!String.IsNullOrWhiteSpace(lyrics)) {
            var chordRoots = new List<string> {
                "[A", "[B", "[C", "[D", "[E", "[F", "[G"
            };

            foreach (var chordRoot in chordRoots) {
                if (lyrics.Contains(chordRoot)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if the lyrics contain chords in the do-re-mi format.
    /// </summary>
    /// <param name="lyrics">The lyrics to check.</param>
    /// <returns>True if do-re-mi chords are found.</returns>
    public static bool LyricsContainDoReMiChords(string? lyrics)
    {
        if (!String.IsNullOrWhiteSpace(lyrics)) {
            var chordRoots = new List<string> {
                "[DO", "[RE", "[RÉ", "[MI", "[FA", "[SO", "[LA", "[TI", "[SI"
            };

            foreach (var chordRoot in chordRoots) {
                if (lyrics.Contains(chordRoot, StringComparison.InvariantCultureIgnoreCase)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if the lyrics contain chords in the Nashville Numbers format.
    /// </summary>
    /// <param name="lyrics">The lyrics to check.</param>
    /// <returns>True if Nashville Number chords are found.</returns>
    public static bool LyricsContainNashvilleNumbers(string? lyrics)
    {
        if (!String.IsNullOrWhiteSpace(lyrics)) {
            var nnRoots = new List<string> {
            "[1", "[2", "[3", "[4", "[5", "[6", "[7"
        };

            foreach (var nnRoot in nnRoots) {
                if (lyrics.Contains(nnRoot)) {
                    return true;
                }
            }
        }

        return false;
    }

    public static string LyricsLinesToDivs(string lyrics)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(lyrics)) {
            // split the text on any newline characters.
            var lines = Tools.SplitTextIntoLines(lyrics); // lyrics.Split(new string[] { Environment.NewLine },StringSplitOptions.None).ToList();

            foreach(var line in lines) {
                if (!String.IsNullOrWhiteSpace(line)) {
                    output += "<div class=\"view-lyrics-line\">" + line.Trim() + "</div>";
                }
            }
        }

        return output;
    }

    public static string LyricsOnly(string? content)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(content)) {
            // Remove anything inside square brackets.
            output = System.Text.RegularExpressions.Regex.Replace(content, @"\[.*?\]", "");
        }

        return output.Trim();
    }

    /// <summary>
    /// Wraps lyrics in td elements.
    /// </summary>
    /// <param name="lyrics">The lyrics to wrap.</param>
    /// <returns>The lyrics wrapped in a td element with the appropriate classes.</returns>
    public static string LyricsToTableDataElements(List<string>? lyrics)
    {
        string output = "";

        if (lyrics != null && lyrics.Any()) {
            foreach (var item in lyrics) {
                string lyric = item.Replace(" ", "&nbsp;");

                if (lyric.Contains("-")) {
                    output += "<td class=\"songformat-lyric hyphenated\">" + HyphenatedLyricsToTable(lyric) + "</td>";
                } else {
                    output += "<td class=\"songformat-lyric\">" + lyric + "</td>";
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Trims a string to a maximum length.
    /// </summary>
    /// <param name="input">The string to trim.</param>
    /// <param name="maxLength">The maximum length for the string.</param>
    /// <param name="addEllipses">Option to add ellipses to trimmed strings to indicate they were trimmed.</param>
    /// <returns>The original string if it's under the max length, or a trimmed string to the maximum length.</returns>
    public static string MaxStringLength(string? input, int maxLength = 100, bool addEllipses = true)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(input)) {
            output = input;

            if (output.Length > maxLength) {
                output = output.Substring(0, maxLength);
                if (addEllipses) {
                    output += "...";
                }
            }
        }

        return output;
    }

    public static string MediaUrl(string folder, string? filename)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(filename)) {
            output = Model.ApplicationUrl + "media/" + folder + "/" + filename;
        }

        return output;
    }

    /// <summary>
    /// Converts an array of strings to a message.
    /// If a single string is in the array only the text is returned.
    /// If multiple items are in the array an <ul> element is returned with <li> elements for each text item.
    /// </summary>
    /// <param name="messages">The messages collection.</param>
    /// <returns>Returns a single string if the array only contained one string, or a <ul> with <li> elements for each string.</returns>
    public static string MessagesToString(List<string>? messages)
    {
        string output = String.Empty;

        if (messages != null && messages.Any()) {
            if (messages.Count() == 1) {
                output += messages.First();
            } else {
                output += "<ul>";

                foreach (var message in messages) {
                    output += "<li>" + message + "</li>";
                }

                output += "</ul>";
            }
        }

        return output;
    }

    public static string MimeType(string? file)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(file)) {
            var ext = Path.GetExtension(file).ToLower();
            switch(ext) {
                case ".mp4":
                case ".m4v":
                    output = "video/mp4";
                    break;
                case ".webm":
                    output = "video/webm";
                    break;
            }
        }

        return output;
    }

    /// <summary>
    /// The style used to indicate a field is missing a value when applying via a style instead of a class.
    /// </summary>
    /// <returns>The style.</returns>
    public static string MissingRequiredFieldStyle {
        get {
            return "background-color: palevioletred; border-color: darkred; color: #fff;";
        }
    }

    /// <summary>
    /// Returns the class that marks a field as missing if no value is provided.
    /// </summary>
    /// <param name="value">A nullable DateTime value.</param>
    /// <param name="defaultClass">An optional default class to append to the output.</param>
    /// <returns>The class value.</returns>
    public static string MissingValue(DateTime? value, string? defaultClass = "")
    {
        if (String.IsNullOrWhiteSpace(defaultClass)) {
            return !value.HasValue ? MissingValueClass : "";
        } else {
            return !value.HasValue ? MissingValueClass + " " + defaultClass : defaultClass;
        }
    }

    /// <summary>
    /// Returns the class that marks a field as missing if no value is provided.
    /// </summary>
    /// <param name="value">A nullable int value.</param>
    /// <param name="defaultClass">An optional default class to append to the output.</param>
    /// <returns>The class value.</returns>
    public static string MissingValue(decimal? value, string? defaultClass = "")
    {
        if (String.IsNullOrWhiteSpace(defaultClass)) {
            return value.HasValue && value.Value > 0 ? "" : MissingValueClass;
        } else {
            return value.HasValue && value.Value > 0 ? defaultClass : MissingValueClass + " " + defaultClass;
        }
    }

    /// <summary>
    /// Returns the class that marks a field as missing if no value is provided.
    /// </summary>
    /// <param name="value">A nullable Guid value.</param>
    /// <param name="defaultClass">An optional default class to append to the output.</param>
    /// <returns>The class value.</returns>
    public static string MissingValue(Guid? value, string? defaultClass = "")
    {
        if (String.IsNullOrWhiteSpace(defaultClass)) {
            return GuidValue(value) == Guid.Empty ? MissingValueClass : "";
        } else {
            return GuidValue(value) == Guid.Empty ? MissingValueClass + " " + defaultClass : defaultClass;
        }

    }

    /// <summary>
    /// Returns the class that marks a field as missing if no value is provided.
    /// </summary>
    /// <param name="value">A nullable int value.</param>
    /// <param name="defaultClass">An optional default class to append to the output.</param>
    /// <returns>The class value.</returns>
    public static string MissingValue(int? value, string? defaultClass = "")
    {
        if (String.IsNullOrWhiteSpace(defaultClass)) {
            return value.HasValue && value.Value > 0 ? "" : MissingValueClass;
        } else {
            return value.HasValue && value.Value > 0 ? defaultClass : MissingValueClass + " " + defaultClass;
        }
    }

    /// <summary>
    /// Returns the class that marks a field as missing if no value is provided.
    /// </summary>
    /// <param name="value">A nullable string value.</param>
    /// <param name="defaultClass">An optional default class to append to the output.</param>
    /// <returns>The class value.</returns>
    public static string MissingValue(string? value, string? defaultClass = "")
    {
        if (String.IsNullOrWhiteSpace(defaultClass)) {
            return String.IsNullOrWhiteSpace(value) ? MissingValueClass : "";
        } else {
            return String.IsNullOrWhiteSpace(value) ? MissingValueClass + " " + defaultClass : defaultClass;
        }
    }

    public static string MissingValueClass {
        get {
            return "m-r";
        }
    }

    /// <summary>
    /// Closes the modal dialog.
    /// </summary>
    public static void ModalClose()
    {
        DialogService.Close();
    }

    /// <summary>
    /// Displays a modal message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="title">An optional title for the dialog.</param>
    /// <param name="DisableClose">Option to disable the close button for the dialog.</param>
    /// <param name="width">Optional width for the dialog (defaults to auto-sized.)</param>
    /// <param name="height">Optional height for the dialog (defaults to auto-sized.)</param>
    public async static Task ModalMessage(string message, string title = "", bool DisableClose = false, string width = "auto", string height = "auto")
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add("Message", message);

        if (width == "auto") {
            width = "";
        }

        if (height == "auto") {
            height = "";
        }

        await DialogService.OpenAsync<BandTogether.Client.Shared.ModalMessage>(title, parameters, new Radzen.DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = !DisableClose,
            ShowClose = !DisableClose,
            Width = width,
            Height = height,
        });
    }

    /// <summary>
    /// Navigates to an application URL.
    /// </summary>
    /// <param name="subUrl">The sub-page to navigate to, otherwise, navigates to the root.</param>
    /// <param name="forceReload">Option to force a full reload on navigate.</param>
    public static void NavigateTo(string subUrl, bool forceReload = false)
    {
        if (subUrl.ToLower().StartsWith("http:") || subUrl.ToLower().StartsWith("https:")) {
            NavManager.NavigateTo(subUrl, forceReload);
        } else {
            NavManager.NavigateTo(BaseUri + subUrl, forceReload);
        }
    }

    /// <summary>
    /// Navigates to the root of the application.
    /// </summary>
    /// <param name="forceReload">Option to force a full reload on navigate.</param>
    public static void NavigateToRoot(bool forceReload = false)
    {
        NavManager.NavigateTo(BaseUri, forceReload);
    }

    /// <summary>
    /// Navigates to a given url using javascript.
    /// </summary>
    /// <param name="url">The url to navigate to.</param>
    public async static Task NavigateToViaJavascript(string url)
    {
        await jsRuntime.InvokeVoidAsync("NavigateTo", url);
    }

    /// <summary>
    /// Converts a number to Roman Numerals. Also handles special cases for Nashville Numbers.
    /// </summary>
    /// <param name="number">A number from 1 to 12.</param>
    /// <returns>Either the Roman Numeral or the NashvilleNumbers text.</returns>
    public static string NumberToRomanNumerals(int number)
    {
        string output = "";

        switch (number) {
            case 1:
                output = "I";
                break;
            case 2:
                output = "II";
                break;
            case 3:
                output = "III";
                break;
            case 4:
                output = "IV";
                break;
            case 5:
                output = "V";
                break;
            case 6:
                output = "VI";
                break;
            case 7:
                output = "VII";
                break;
            case 8:
                output = "VIII";
                break;
            case 9:
                output = "IX";
                break;
            case 10:
                output = "X";
                break;
            case 11:
                output = "XI";
                break;
            case 12:
                output = Text.NashvilleNumbers;
                break;
        }

        return output;
    }

    public async static Task OpenSetting()
    {
        await ReloadSettings();

        var title = Text.Settings;

        await DialogService.OpenAsync<SettingsDialog>(title, null, new Radzen.DialogOptions() {
            AutoFocusFirstElement = false,
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = true,
            ShowClose = true,
            Width = "98%",
            Top = _dialogTop,
        });
    }

    public async static Task PlayAudioFile(string? filename, int volume)
    {
        if (!String.IsNullOrWhiteSpace(filename)) {
            var url = MediaUrl("audio", filename);
            Model.AudioPlayingFile = filename;
            Model.AudioPlaying = true;

            double v = (double)volume / 100;

            await jsRuntime.InvokeVoidAsync("PlayAudioFile", url, v);
        }
    }

    public async static Task PlayAudioFileSetTime(double time)
    {
        await jsRuntime.InvokeVoidAsync("PlayAudioFileSetTime", time);
    }

    public async static Task PlayAudioFileStop()
    {
        await jsRuntime.InvokeVoidAsync("PlayAudioFileStop");
    }

    public async static Task PlayAudioFileTogglePlayPause()
    {
        await jsRuntime.InvokeVoidAsync("PlayAudioFileTogglePlayPause");
    }

    public async static Task PlayVideo(string ElementId, int Volume)
    {
        double v = (double)Volume / 100;
        await jsRuntime.InvokeVoidAsync("PlayVideo", ElementId, v);
    }

    public async static Task PlayVideoSetPlaybackState(string ElementId, string State)
    {
        await jsRuntime.InvokeVoidAsync("PlayVideoSetPlaybackState", "video-player-" + ElementId, State);
    }

    public async static Task PlayVideoUpdatePlaybackTime(string Element, double PlaybackTime)
    {
        await jsRuntime.InvokeVoidAsync("PlayVideoUpdatePlaybackTime", "video-player-" + Element, PlaybackTime);
    }

    public async static Task PlayYouTubeVideo(string ElementId, string VideoId, bool Mute, int Volume)
    {
        await jsRuntime.InvokeVoidAsync("PlayYouTubeVideo", "youtube-player-" + ElementId, VideoId, Mute ? 1 : 0, Volume);
    }

    public async static Task PlayYouTubeVideoSetPlaybackState(string ElementId, string State)
    {
        await jsRuntime.InvokeVoidAsync("PlayYouTubeVideoSetPlaybackState", "youtube-player-" + ElementId, State);
    }

    public async static Task PlayYouTubeVideoUpdatePlayTimeForPlayer(string ElementId, double PlaybackTime)
    {
        await jsRuntime.InvokeVoidAsync("PlayYouTubeVideoUpdatePlayTimeForPlayer", "youtube-player-" + ElementId, PlaybackTime);
    }

    public static string PreviewLabel(string? preview)
    {
        string output = Text.PreviewOff;

        if (!String.IsNullOrWhiteSpace(preview)) {
            switch(preview.ToLower()) {
                case "xs":
                    output = Text.PreviewXSmall;
                    break;

                case "s":
                    output = Text.PreviewSmall;
                    break;

                case "m":
                    output = Text.PreviewMedium;
                    break;

                case "l":
                    output = Text.PreviewLarge;
                    break;

                case "xl":
                    output = Text.PreviewXLarge;
                    break;
            }
        }

        return output;
    }

    public static double PreviewScale(string? preview)
    {
        double output= 1.0;

        if (!String.IsNullOrWhiteSpace(preview)) {
            switch(preview.ToLower()) {
                case "xs":
                    output = 0.15;
                    break;

                case "s":
                    output = 0.3;
                    break;

                case "m":
                    output = 0.4;
                    break;

                case "l":
                    output = 0.5;
                    break;

                case "xl":
                    output = 0.75;
                    break;
            }
        }

        return output;
    }

    public static previewSize PreviewSizeFromString(string size)
    {
        var output = previewSize.off;

        switch(StringLower(size)) {
            case "xs":
                output = previewSize.extraSmall;
                break;

            case "s":
                output = previewSize.small;
                break;

            case "m":
                output = previewSize.medium;
                break;

            case "l":
                output = previewSize.large;
                break;

            case "xl":
                output = previewSize.extraLarge;
                break;
        }

        return output;
    }

    public static string PreviewSizeToString(previewSize size)
    {
        string output = String.Empty;

        switch(size) {
            case previewSize.off:
                break;

            case previewSize.extraSmall:
                output = "xs";
                break;

            case previewSize.small:
                output = "s";
                break;

            case previewSize.medium:
                output = "m";
                break;

            case previewSize.large:
                output = "l";
                break;

            case previewSize.extraLarge:
                output = "xl";
                break;
        }

        return output;
    }
    
    /// <summary>
    /// Recurses an exception and any inner exceptions.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <param name="ShowExceptionType">Option to include the exception type in the results.</param>
    /// <returns>A list of strings containing any exceptions.</returns>
    public static List<string> RecurseException(Exception ex, bool ShowExceptionType = true)
    {
        List<string> output = new List<string>();

        if (ex != null) {
            if (!String.IsNullOrWhiteSpace(ex.Message)) {
                if (ShowExceptionType) {
                    output.Add(ex.GetType().ToString() + ": " + ex.Message);
                } else {
                    output.Add(ex.Message);
                }

            }

            if (ex.InnerException != null) {
                var inner = RecurseException(ex.InnerException, ShowExceptionType);
                if (inner.Any()) {
                    foreach (var message in inner) {
                        output.Add(message);
                    }
                }
            }
        }

        return output;
    }

    public static async Task ReloadAudioFiles()
    {
        var results = await Helpers.GetOrPost<List<string>>("api/GetAudioFiles");
        Model.AudioFiles = results != null && results.Any() ? results : new List<string>();
    }

    public static async Task ReloadBackgrounds()
    {
        var results = await GetOrPost<List<string>>("api/GetBackgrounds");
        Model.Backgrounds = results != null && results.Any() ? results : new List<string>();
    }

    public static async Task ReloadImages()
    {
        var results = await Helpers.GetOrPost<List<string>>("api/GetImages");
        Model.Images = results != null && results.Any() ? results : new List<string>();
    }

    public static async Task ReloadModel()
    {
        var loader = await GetOrPost<dataLoader>("api/GetDataLoader");
        if (loader != null) {
            foreach(var book in loader.songBooks) {
                foreach(var song in book.songs) {
                    song.songBookId = book.id;
                }
            }

            Model.DataLoader = loader;
            Model.Backgrounds = loader.backgrounds;
            Model.Languages = loader.languages;
            Model.Settings = loader.settings;
            Model.SetListFilenames = loader.setListFilenames;
            Model.SongBooks = loader.songBooks;
            Model.Users = loader.users;
            Model.Released = loader.released;
            Model.Version = loader.version;
            Model.Videos = loader.videos;

            // Only load the set list from the loader if it's not already loaded
            if (Model.SetList.items.Count == 0) {
                Model.SetList = ReloadSetListItemsFromJson(loader.setList);

                await GetOrPost<booleanResponse>("api/SetCachedSetList", Model.SetList);
            }

            if (Model.User.id == Guid.Empty) {
                // No user has been loaded, so see if we have a UserId in local storage.
                var lastUserId = await Helpers.GetLocalStorageItem<Guid>("UserId");
                var user = Model.Users.FirstOrDefault(x => x.id == lastUserId);
                if (user != null) {
                    Model.User = user;
                }
            }
        }

        // Set the loaded to true after a small delay to give time for other components to complete loading.
        if (!Model.Loaded) {
            SetTimeout(() => Model.Loaded = true, 400);
        }
    }

    public static async Task ReloadSettings()
    {
        var results = await GetOrPost<appSettings>("api/GetSettings");
        if (results != null) {
            Model.Settings = results;
        }
    }

    public static async Task ReloadSetListFilenames()
    {
        var results = await GetOrPost<List<setListFile>>("api/GetSetListFilenames");
        Model.SetListFilenames = results != null && results.Any() ? results : new List<setListFile>();
    }

    public static setList ReloadSetListItemsFromJson(setList setList)
    {
        // Work on a copy.
        var copy = DuplicateObject<setList>(setList);
        if (copy != null) {
            if (copy.items.Any()) {
                foreach (var item in copy.items) {
                    if (!String.IsNullOrWhiteSpace(item.itemJson)) {
                        switch (item.type) {
                            case setListItemType.audio:
                                var audio = DeserializeObject<audioItem>(item.itemJson);
                                if (audio != null) {
                                    item.item = audio;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.clock:
                                var clock = DeserializeObject<clockItem>(item.itemJson);
                                if (clock != null) {
                                    item.item = clock;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.countdown:
                                var countdown = DeserializeObject<countdownItem>(item.itemJson);
                                if (countdown != null) {
                                    item.item = countdown;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.image:
                                var image = DeserializeObject<imageItem>(item.itemJson);
                                if (image != null) {
                                    item.item = image;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.sheetmusic:
                                var sheetmusic = DeserializeObject<sheetMusicItem>(item.itemJson);
                                if (sheetmusic != null) {
                                    item.item = sheetmusic;
                                }
                                break;

                            case setListItemType.slideshow:
                                var slideshow = DeserializeObject<slideshowItem>(item.itemJson);
                                if (slideshow != null) {
                                    item.item = slideshow;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.song:
                                var song = DeserializeObject<song>(item.itemJson);
                                if (song != null) {
                                    // Songs only store the song id and songbook id, so we need to get the song if we don't already have the parts.
                                    if (song.parts == null || song.parts.Count == 0) {
                                        item.item = GetSong(song.id, song.songBookId);
                                        //item.itemJson = null;
                                    } else {
                                        item.item = song;
                                        //item.itemJson = null;
                                    }
                                }
                                break;

                            case setListItemType.video:
                                var video = DeserializeObject<videoItem>(item.itemJson);
                                if (video != null) {
                                    item.item = video;
                                    //item.itemJson = null;
                                }
                                break;

                            case setListItemType.youTube:
                                var youtube = DeserializeObject<youTubeItem>(item.itemJson);
                                if (youtube != null) {
                                    item.item = youtube;
                                    //item.itemJson = null;
                                }
                                break;

                        }
                    }
                }
            }
        } else {
            copy = new setList();
        }

        return copy;
    }

    public static async Task ReloadSlideshows()
    {
        var results = await GetOrPost<List<string>>("api/GetSlideshows");
        Model.Slideshows = results != null && results.Any() ? results : new List<string>();
    }

    public static async Task ReloadSongBooks()
    {
        var results = await GetOrPost<List<songBook>>("api/GetSongBooks");
        Model.SongBooks = results != null && results.Any() ? results : new List<songBook>();
    }

    public static async Task ReloadUsers()
    {
        var results = await GetOrPost<List<user>>("api/GetUsers");
        Model.Users = results != null && results.Any() ? results : new List<user>();
    }

    public static async Task ReloadVideos()
    {
        var results = await GetOrPost<List<string>>("api/GetVideos");
        Model.Videos = results != null && results.Any() ? results : new List<string>();
    }

    public static async Task RemoveClass(string ElementId, string ClassName)
    {
        await jsRuntime.InvokeVoidAsync("RemoveClass", ElementId, ClassName);
    }

    /// <summary>
    /// Renders a chord in the appropriate format based on song settings and user preferences.
    /// </summary>
    /// <param name="chord">The chord to format.</param>
    /// <param name="originalKey">The original key of the song.</param>
    /// <param name="matrixKey">The selected key to render.</param>
    /// <param name="bassPart">Indicates if this is the bass part of a chord.</param>
    /// <returns></returns>
    public static string RenderChord(string? chord, string? originalKey, string? matrixKey, bool bassPart = false)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(chord)) {
            if (chord == "/" || chord == "|" || chord.StartsWith("!")) {
                return chord;
            }

            if (_UsingNashvilleNumbering) {
                if (_ShowChordsAsNashvilleNumbers) {
                    output += chord;
                } else {
                    output = NashvilleNumbering.FormatChord(StringValue(chord), matrixKey, bassPart);
                }
            } else if (_UsingDoReMi) {
                var nn = DoReMi.ConvertChordToNashvilleNumber(StringValue(chord), bassPart);
                if (_ShowChordsAsNashvilleNumbers) {
                    output = nn;
                } else {
                    output = NashvilleNumbering.FormatChord(nn, matrixKey, bassPart);
                }
            } else if (_ShowChordsAsNashvilleNumbers) {
                output = NashvilleNumbering.ConvertChordToNashvilleNumber(StringValue(chord), matrixKey);
            } else {
                if (!String.IsNullOrWhiteSpace(chord)) {
                    if (originalKey != matrixKey) {
                        chord = TransposeChord(chord, originalKey, matrixKey);
                    }

                    if (chord.Length == 0) {
                        // No chord
                    } else if (chord.Length == 1) {
                        output = chord;
                    } else {
                        // See if there are any minor, sharps, flats, etc.
                        string baseChord = chord.Substring(0, 1);
                        string remainder = chord.Substring(1);

                        output = baseChord;
                        if (remainder.Length == 1) {
                            output += StyleChordCharacter(remainder);
                        } else {
                            // We have more than one remaining character, so check each character for special rules
                            foreach (char c in remainder) {
                                output += StyleChordCharacter(c.ToString());
                            }
                        }
                    }
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Renders a song element.
    /// </summary>
    /// <param name="song">The Song object.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="newKey">The selected key.</param>
    /// <returns>The rendered element.</returns>
    public static string RenderSongElement(song song, songPart part, string? newKey)
    {
        string output = "";

        if (!String.IsNullOrEmpty(_currentSongExport)) {
            _currentSongExport += Environment.NewLine;
        }

        _currentSongExport += part.label + Environment.NewLine;

        if (!String.IsNullOrWhiteSpace(part.content)) {
            string text = part.content;

            text = text
                .Replace("[SVB]", "{{NEWLINE}}")
                .Replace("[NEWLINE]", "{{NEWLINE}}")
                .Replace(Environment.NewLine, "{{NEWLINE}}");

            var lines = text.Split("{{NEWLINE}}");
            foreach (var line in lines) {
                output += RenderSongElementLine(song, line, newKey);
            }
        } else {

        }

        return output;
    }

    /// <summary>
    /// Renders a song element line.
    /// </summary>
    /// <param name="song">The Song object.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="newKey">The selected key.</param>
    /// <returns>The rendered element line.</returns>
    public static string RenderSongElementLine(song song, string? text, string? newKey)
    {
        StringBuilder output = new StringBuilder();

        if (!String.IsNullOrWhiteSpace(text)) {
            output.AppendLine("<table class=\"song-element\">");
            output.AppendLine("  <tbody>");

            var chordsLine = new List<string>();
            var lyricsLine = new List<string>();
            bool inChord = false;
            string currentWord = "";
            string currentChord = "";

            foreach (char c in text) {
                if (c == '[') {
                    // We've just hit the start of a chord, so if we have any previous word or chord push those to their elements
                    inChord = true;
                    if (currentChord != "" || currentWord != "") {
                        chordsLine.Add(currentChord);
                        lyricsLine.Add(currentWord);
                        currentChord = "";
                        currentWord = "";
                    }
                }

                if (c == ']') {
                    inChord = false;
                }

                if (c != '[' && c != ']') {
                    if (inChord) {
                        currentChord += c;
                    } else {
                        currentWord += c;
                    }
                }
            }

            // Now, add whatever is left
            if (currentChord != "" || currentWord != "") {
                chordsLine.Add(currentChord);
                lyricsLine.Add(currentWord);
            }

            // Add hyphens where words break.
            if (chordsLine.Any()) {
                var newChords = new List<string>();
                var newLyrics = new List<string>();

                string previousLyrics = "";

                int elementCount = chordsLine.Count;

                string export = "";

                for (int x = 0; x < elementCount; x++) {
                    string lyrics = lyricsLine[x];
                    string chord = chordsLine[x];

                    if (!String.IsNullOrWhiteSpace(chord)) {
                        export += "[" + RenderChord(chord, song.key, newKey) + "]";
                    } else if (x > 0) {
                        export += "[]";
                    }

                    if (!String.IsNullOrWhiteSpace(lyrics)) {
                        export += lyrics;
                    }

                    string remainingLyrics = String.Empty;
                    if (x < elementCount - 1) {
                        for (int y = x + 1; y < elementCount; y++) {
                            remainingLyrics += lyricsLine[y];
                        }
                    }

                    bool nextCharIsLetter = false;
                    string nextCharacter = String.Empty;

                    if (!String.IsNullOrWhiteSpace(remainingLyrics)) {
                        nextCharacter = remainingLyrics.Substring(0, 1);
                        if (nextCharacter.ToLower() != nextCharacter.ToUpper()) {
                            nextCharIsLetter = true;
                        }
                    }

                    if (previousLyrics != String.Empty && !previousLyrics.EndsWith(" ") && !previousLyrics.EndsWith("-") && nextCharIsLetter) {
                        lyrics = "-" + lyrics;
                    }

                    newChords.Add(chord);
                    newLyrics.Add(lyrics);

                    previousLyrics += lyrics;
                }

                _currentSongExport += export + Environment.NewLine;

                chordsLine = newChords;
                lyricsLine = newLyrics;
            }

            if (!Model.User.preferences.hideChords) {
                output.AppendLine("    <tr class=\"chord-line\">" + ChordsToTableDataElements(chordsLine, song.key, newKey) + "</tr>");
            }
            output.AppendLine("    <tr class=\"lyric-line\">" + LyricsToTableDataElements(lyricsLine) + "</tr>");

            output.AppendLine("  </tbody>");
            output.AppendLine("</table>");
        }

        return output.ToString();
    }

    public static string ReplaceSongTags(string? format, song song)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(format)) {
            output = format
                .Replace("{title}", song.title, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{artist}", song.artist, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{copyright}", song.copyright, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{church_ccli}", Model.Settings.ccliNumber, StringComparison.InvariantCultureIgnoreCase)
                .Replace("{song_ccli}", song.ccliNumber, StringComparison.InvariantCultureIgnoreCase) ;
        }

        return output;
    }

    /// <summary>
    /// Replaces spaces in a string with non-breaking html characters.
    /// </summary>
    /// <param name="input">The string to replace spaces in.</param>
    /// <returns>A string with any spaces replaced with non-breaking html characters.</returns>
    public static string ReplaceSpaces(string? input)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(input)) {
            output = input.Replace(" ", "&nbsp;");
        }

        return output;
    }

    /// <summary>
    /// Resets the padding for the after-song-padding element
    /// </summary>
    /// <returns></returns>
    public static async Task ResetPadding()
    {
        await jsRuntime.InvokeVoidAsync("ResetPadding");
    }

    /// <summary>
    /// Saves the Settings for the app.
    /// </summary>
    public static async Task SaveSettings()
    {
        var saved = await GetOrPost<booleanResponse>("api/SaveSettings", Model.Settings);
    }

    public static async Task<booleanResponse> SaveSongBook(songBook songbook)
    {
        var output = await Helpers.GetOrPost<booleanResponse>("api/SaveSongBook", songbook);
        return output != null ? output : new booleanResponse();
    }

    /// <summary>
    /// Saves the user preferences for the current user.
    /// </summary>
    /// <returns>A BooleanResponse object.</returns>
    public static async Task<booleanResponse> SaveUserPreferences()
    {
        var output = new booleanResponse();

        if (!_savingUserPreferences) {
            _savingUserPreferences = true;
            output = await SaveUserPreferences(Model.User.id, Model.User.preferences);
            _savingUserPreferences = false;
        }

        return output;
    }

    /// <summary>
    /// Saves user preferences for a specific user.
    /// </summary>
    /// <param name="UserId">The unique UserId of the user.</param>
    /// <param name="preferences">The UserPreferences object to save for the user.</param>
    /// <returns>A BooleanResponse object.</returns>
    public static async Task<booleanResponse> SaveUserPreferences(Guid UserId, userPreferences preferences)
    {
        var output = new booleanResponse();

        var saved = await GetOrPost<booleanResponse>("api/SaveUserPreferences/" + UserId.ToString(), preferences);
        if (saved != null) {
            output = saved;
        }

        return output;
    }

    /// <summary>
    /// Scrolls the parent element so that the specified element is centered.
    /// </summary>
    /// <param name="element">the element to focus in the center.</param>
    /// <param name="parent">the parent element containing this element.</param>
    /// <returns></returns>
    public static async Task ScrollElementToCenter(string element, string parent)
    {
        await jsRuntime.InvokeVoidAsync("ScrollElementToCenter", element, parent);
    }

    /// <summary>
    /// Scrolls the browser to a specific element.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static async Task ScrollToElement(string element)
    {
        await jsRuntime.InvokeVoidAsync("ScrollToElement", element);
    }

    /// <summary>
    /// Scrolls the browser back to the top.
    /// </summary>
    public static async Task ScrollToTop()
    {
        await jsRuntime.InvokeVoidAsync("ScrollToTop");
    }

    /// <summary>
    /// Converts seconds to a user-readable friendly time passed string.
    /// </summary>
    /// <param name="seconds">The number of seconds.</param>
    /// <returns>A string in a user-readable format.</returns>
    public static string SecondsToTime(double seconds)
    {
        string output = "";

        int totalSeconds = (int)seconds;

        if (totalSeconds > 0) {
            int minutes = 0;
            int hours = 0;
            int days = 0;

            if (totalSeconds >= 86400) {
                days = (totalSeconds / 86400);
                totalSeconds = totalSeconds - (86400 * days);
            }

            if (totalSeconds >= 3600) {
                hours = (totalSeconds / 3600);
                totalSeconds = totalSeconds - (3600 * hours);
            }

            if (totalSeconds > 60) {
                minutes = (totalSeconds / 60);
                totalSeconds = totalSeconds - (60 * minutes);
            }

            if (days > 0) {
                output += days.ToString() + " " + (days > 1 ? Text.Days : Text.Day);
            }

            if (hours > 0) {
                if (output != "") { output += ", "; }
                output += hours.ToString() + " " + (hours > 1 ? Text.Hours : Text.Hour);
            }

            if (minutes > 0) {
                if (output != "") { output += ", "; }
                output += minutes.ToString() + " " + (minutes > 1 ? Text.Minutes : Text.Minute);
            }

            if (totalSeconds > 0) {
                if (output != "") { output += ", "; }
                output += totalSeconds.ToString() + " " + (totalSeconds > 1 ? Text.Seconds : Text.Second);
            }
        }

        return output;
    }

    /// <summary>
    /// Serializes an object to JSON using the System.Text.Json.JsonSerializer.
    /// </summary>
    /// <param name="Object">The object to serialize.</param>
    /// <param name="formatOutput">Option to format the output with indenting.</param>
    /// <returns>The JSON of the object.</returns>
    public static string SerializeObject(object? Object, bool formatOutput = false)
    {
        string output = String.Empty;

        if (Object != null) {
            if (formatOutput) {
                output += System.Text.Json.JsonSerializer.Serialize(Object, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            } else {
                output += System.Text.Json.JsonSerializer.Serialize(Object);
            }
        }

        return output;
    }

    public static async Task SetActiveItemTypeJSInterop(string type)
    {
        await jsRuntime.InvokeVoidAsync("SetActiveItemType", type);
    }

    public static async Task SetActivePageSection(string section)
    {
        await jsRuntime.InvokeVoidAsync("SetActivePageSection", section);
    }

    public static async Task SetEditMode(bool editMode)
    {
        await jsRuntime.InvokeVoidAsync("SetEditMode", editMode);
    }

    /// <summary>
    /// Uses jsInterop to set the height of an element.
    /// </summary>
    /// <param name="ElementId">The id of the element.</param>
    /// <param name="Height">The height to set.</param>
    /// <returns></returns>
    public static async Task SetElementHeight(string ElementId, string Height)
    {
        await jsRuntime.InvokeVoidAsync("SetElementHeight", ElementId, Height);
    }

    /// <summary>
    /// Sets the value of an element using JSInterop.
    /// </summary>
    /// <param name="ElementId">The id of the html element.</param>
    /// <param name="Value">The value to set.</param>
    public static async Task SetElementValue(string ElementId, string Value)
    {
        await jsRuntime.InvokeVoidAsync("SetElementValue", ElementId, Value);
    }

    /// <summary>
    /// Sets a value for an item in the Local Storage.
    /// </summary>
    /// <param name="key">The key of the item.</param>
    /// <param name="value">The value to store.</param>
    public static async Task SetLocalStorageItem(string key, object value)
    {
        if (LocalStorage != null) {
            await LocalStorage.SetItemAsync(key, value);
        }
    }

    /// <summary>
    /// Executes a function after a specific delay.
    /// </summary>
    /// <param name="methodToInvoke">The delegate to the method to invoke.</param>
    /// <param name="millisecondsDelay">The milliseconds to delay before executing (defaults to 100.)</param>
    public static void SetTimeout(Delegate methodToInvoke, int millisecondsDelay = 100)
    {
        System.Threading.Timer? timer = null;
            timer = new System.Threading.Timer((obj) =>
                {
                    methodToInvoke.DynamicInvoke();
                    timer?.Dispose();
                },
            null, millisecondsDelay, System.Threading.Timeout.Infinite);
    }

    public static async Task SetUserAutoFollow(user user, bool autoFollow)
    {
        user.preferences.autoFollow = autoFollow;
        await SetUserViewPreferences(user);
    }

    public static async Task SetUserCapoPosition(ChangeEventArgs e, user user)
    {
        int capo = 0;

        string value = "";
        if (e != null && e.Value != null) {
            try {
                value += e.Value.ToString();
                capo = Convert.ToInt32(value);
            } catch { }
        }

        var updated = await Helpers.GetOrPost<booleanResponse>("api/SetUserCapoPosition/" + user.id.ToString() + "/" + capo.ToString() + "/" + Model.Song.id.ToString());
    }

    public static async Task SetUserCapoPosition(int capo, user user)
    {
        var updated = await Helpers.GetOrPost<booleanResponse>("api/SetUserCapoPosition/", new songPreferences {
            songId = Model.Song.id,
            songBookId = Helpers.GuidValue(Model.Song.songBookId),
            userId = user.id,
            capo = capo,
            title = StringValue(Model.Song.title),
        });
    }

    public static async Task SetUserHideChords(user user, bool hideChords)
    {
        var updated = await Helpers.GetOrPost<booleanResponse>("api/SetUserHideChords/" + user.id.ToString() + "/" + hideChords.ToString());
    }

    public static async Task SetUserTheme(user user, string theme)
    {
        if (String.IsNullOrWhiteSpace(theme)) {
            theme = "auto";
        }

        var updated = await Helpers.GetOrPost<booleanResponse>("api/SetUserTheme/" + user.id.ToString() + "/" + theme);
    }

    public static async Task SetUserViewPreferences(user user)
    {
        // Sets the UsersPreferences for AutoFollow, AutoFontSize, HideChords, and Zoom
        var updated = await Helpers.GetOrPost<booleanResponse>("api/SetUserViewPreferences", user);
    }

    public static async Task SetUserZoom(int zoom, user user)
    {
        if (zoom > Model.ZoomMin && zoom < Model.ZoomMax) {
            user.preferences.zoom = zoom;
            await SetUserViewPreferences(user);
        }
    }

    /// <summary>
    /// Gets or sets the flag to indicate the user wants the chords converted to Nashville Numbers.
    /// </summary>
    public static bool ShowChordsAsNashvilleNumbers {
        get { return _ShowChordsAsNashvilleNumbers; }
        set {
            _ShowChordsAsNashvilleNumbers = value;
        }
    }

    public static string SlideShowUrl(string folder, string? filename)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(filename)) {
            output = Model.ApplicationUrl + "Slideshow/" + folder + "/" + filename;
        }

        return output;
    }

    public static string SongBookName(Guid? id)
    {
        string output = String.Empty;

        var book = Model.SongBooks.FirstOrDefault(x => x.id == id);

        if (book != null) {
            output = book.name;
        }

        return output;
    }

    public static List<songKey> SongKeys
    {
        get {
            var output = new List<songKey>();

            if (Model.Settings.showNonStandardKeys) {
                output = new List<songKey> {
                    new songKey { label = "A♭", key = "Ab" },
                    new songKey { label = "A", key = "A" },
                    new songKey { label = "A♯", key = "A#" },
                    new songKey { label = "B♭", key = "Bb" },
                    new songKey { label = "B", key = "B" },
                    new songKey { label = "B♯", key = "B#" },
                    new songKey { label = "C♭", key = "Cb" },
                    new songKey { label = "C", key = "C" },
                    new songKey { label = "C♯", key = "C#" },
                    new songKey { label = "D♭", key = "Db" },
                    new songKey { label = "D", key = "D" },
                    new songKey { label = "D♯", key = "D#" },
                    new songKey { label = "E♭", key = "Eb" },
                    new songKey { label = "E", key = "E" },
                    new songKey { label = "E♯", key = "E#" },
                    new songKey { label = "F♭", key = "Fb" },
                    new songKey { label = "F", key = "F" },
                    new songKey { label = "F♯", key = "F#" },
                    new songKey { label = "G♭", key = "Gb" },
                    new songKey { label = "G", key = "G" },
                    new songKey { label = "G♯", key = "G#" },
                };
            } else {
                output = new List<songKey> {
                    new songKey { label = "A♭", key = "Ab" },
                    new songKey { label = "A", key = "A" },
                    new songKey { label = "A♯", key = "A#" },
                    new songKey { label = "B♭", key = "Bb" },
                    new songKey { label = "B", key = "B" },
                    new songKey { label = "C", key = "C" },
                    new songKey { label = "C♯", key = "C#" },
                    new songKey { label = "D♭", key = "Db" },
                    new songKey { label = "D", key = "D" },
                    new songKey { label = "D♯", key = "D#" },
                    new songKey { label = "E♭", key = "Eb" },
                    new songKey { label = "E", key = "E" },
                    new songKey { label = "F", key = "F" },
                    new songKey { label = "F♯", key = "F#" },
                    new songKey { label = "G♭", key = "Gb" },
                    new songKey { label = "G", key = "G" },
                    new songKey { label = "G♯", key = "G#" },
                };
            }

            return output;
        }
    }

    //public static List<songPart> SongParts(song song)
    //{
    //    var output = new List<songPart>();

    //    if (!String.IsNullOrWhiteSpace(song.content)) {
    //            // Split the text into lines.
		  //      var lines = song.content.Trim().Split(new string[] { Environment.NewLine },StringSplitOptions.None).ToList();
		
		  //      var songLines = new List<songLine>();
		  //      foreach (var line in lines.Index()) {
			 //       songLines.Add(new songLine { 
				//        index = line.Index,
				//        text = line.Item,
			 //       });
		  //      }
		
		  //      var part = new songPart();
		  //      var firstLineInSection = true;
		
    //            //int index = -1;
		  //      foreach(var line in songLines) {
			 //       // Get the next non-empty line.
			 //       var nextNonEmptyLine = songLines.FirstOrDefault(x => x.index > line.index && !String.IsNullOrWhiteSpace(x.text));
			 //       if (String.IsNullOrWhiteSpace(line.text)) {
				//        // This is an empty line, which means the start of a new element.
				//        // So, output the current element if it has a Label.
				
				//        // However, if there was just an empty line and the next
				//        // non-empty line contains a chord character ([) then
				//        // just add this empty line and stay in this element.
				//        if (nextNonEmptyLine != null && !String.IsNullOrWhiteSpace(nextNonEmptyLine.text) && nextNonEmptyLine.text.Contains("[")) {
				//	        part.content += Environment.NewLine;
				//        } else {
				//	        if (!String.IsNullOrWhiteSpace(part.label)) {
    //                            //index++;
    //                            //part.Index = index;
    //                            part.index = output.Count;
				//		        part.endLine = line.index;
				//		        output.Add(part);
				//	        }

				//	        part = new songPart();
				//	        firstLineInSection = true;
				//        }
			 //       } else if (firstLineInSection) {
				//        part.startLine = line.index + 1;
				//        part.label = line.text;
				//        firstLineInSection = false;
			 //       } else {
				//        if (!String.IsNullOrWhiteSpace(part.content)) {
				//	        part.content += Environment.NewLine;
				//        }
				//        part.content += line.text;
			 //       }
		  //      }
		
		  //      // Add the final element.
		  //      part.endLine = songLines.Count();
    //            part.index = output.Count;
    //            //index++;
    //            //part.Index = index;
		  //      output.Add(part);
    //        }

    //    return output;
    //}

    /// <summary>
    /// Checks if the song uses the do-re-mi system of chords.
    /// </summary>
    /// <param name="song">The Song object.</param>
    /// <returns>True if this song is in the do-re-mi chord format.</returns>
    public static bool SongUsesDoReMiSystem(song song)
    {
        var parts = Tools.SongParts(song.content);

        if (parts.Any()) {
            string lyrics = String.Empty;

            foreach (var item in parts) {
                if (lyrics != String.Empty) {
                    lyrics += " ";
                }
                lyrics += item.content;
            }

            if (LyricsContainNashvilleNumbers(lyrics)) {
                // This song contains Nashville numbers already, so ignore do-re-mi
                return false;
            } else if (LyricsContainDoReMiChords(lyrics)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Indicates if the song is in the Nashville Number format.
    /// </summary>
    /// <param name="song">The Song object.</param>
    /// <returns>True if the song uses the Nashville Number chord format.</returns>
    public static bool SongUsesNashvilleNumberingSystem(song song)
    {
        var parts = Tools.SongParts(song.content);

        if (parts.Any()) {
            string lyrics = String.Empty;

            foreach (var item in parts) {
                if (lyrics != String.Empty) {
                    lyrics += " ";
                }
                lyrics += item.content;
            }

            if (LyricsContainChords(lyrics)) {
                // If any of the lyrics contain a chord then assume this song uses chords.
                return false;
            } else if (LyricsContainNashvilleNumbers(lyrics)) {
                // There are no chords, but there are numbers, so return true.
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Converts a string to lower case.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string as lower case.</returns>
    public static string StringLower(string? input)
    {
        string output = !String.IsNullOrWhiteSpace(input) ? input.ToLower() : String.Empty;
        return output;
    }

    /// <summary>
    /// Converts a string to upper case.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>The string as upper case.</returns>
    public static string StringUpper(string? input)
    {
        string output = !String.IsNullOrWhiteSpace(input) ? input.ToUpper() : String.Empty;
        return output;
    }

    /// <summary>
    /// Gets the value of a nullable string.
    /// </summary>
    /// <param name="input">A nullable string.</param>
    /// <returns>The original string or an empty string if the input was null.</returns>
    public static string StringValue(string? input)
    {
        string output = !String.IsNullOrWhiteSpace(input) ? input : String.Empty;
        return output;
    }

    /// <summary>
    /// Styles special characters in a chord.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns>The styled version of the character.</returns>
    public static string StyleChordCharacter(string? character)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(character)) {
            switch (character) {
                case "b":
                    output = "♭";
                    break;
                case "#":
                    output = "♯";
                    break;
                case "%":
                    output = "sus";
                    break;
                case "$":
                    output = "maj";
                    break;
                case "^":
                    output = "dim";
                    break;
                default:
                    output = character;
                    break;
            }
        }

        return output;
    }

    /// <summary>
    /// Gets the text from the static Text library.
    /// </summary>
    /// <param name="textProperty">The name of the property on the Text static class.</param>
    /// <returns>Any text found for that property name.</returns>
    public static string TextValue(string textProperty)
    {
        string output = String.Empty;

        try {
            var staticProperty = typeof(Text).GetProperty(textProperty);
            if (staticProperty != null) {
                var value = staticProperty.GetValue(null);

                if (value != null) {
                    output += value.ToString();
                }
            }
        } catch { }

        return output;
    }

    /// <summary>
    /// Calls the ToAbsoluteUri method for the given relativeUri using the NavigationManager.
    /// </summary>
    /// <param name="uri">The relativeUrl.</param>
    /// <returns>An abolute Uri.</returns>
    public static Uri ToAbsoluteUri(string uri)
    {
        return NavManager.ToAbsoluteUri(uri);
    }

    public static string TransitionCSS(viewStyle style, int? transitionSpeed)
    {
        var output = new StringBuilder();

        string transitionSpeedMs = transitionSpeed.HasValue ? transitionSpeed.Value.ToString() : "0";

        output.AppendLine("@keyframes fadeInHeader {");
        output.AppendLine("  from { opacity: 0; }");
        output.AppendLine("  to { opacity: " + (style.headerStyle != null ? style.headerStyle.opacity : 0) + "; }");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine("@keyframes fadeOutHeader {");
        output.AppendLine("  from { opacity: " + (style.headerStyle != null ? style.headerStyle.opacity : 0) + "; }");
        output.AppendLine("  to { opacity: 0; }");
        output.AppendLine("}");
        output.AppendLine(".projection-wrapper .fade-in .header {");
        output.AppendLine("  animation: fadeInHeader ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine(".projection-wrapper .fade-out .header {");
        output.AppendLine("  animation: fadeOutHeader ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");

        output.AppendLine("@keyframes fadeInFooter {");
        output.AppendLine("  from { opacity: 0; }");
        output.AppendLine("  to { opacity: " + (style.footerStyle != null ? style.footerStyle.opacity : 0) + "; }");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine("@keyframes fadeOutFooter {");
        output.AppendLine("  from { opacity: " + (style.footerStyle != null ? style.footerStyle.opacity : 0) + "; }");
        output.AppendLine("  to { opacity: 0; }");
        output.AppendLine("}");
        output.AppendLine(".projection-wrapper .fade-in .footer {");
        output.AppendLine("  animation: fadeInFooter ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine(".projection-wrapper .fade-out .footer {");
        output.AppendLine("  animation: fadeOutFooter ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");

        output.AppendLine("@keyframes fadeInLyrics {");
        output.AppendLine("  from { opacity: 0; }");
        output.AppendLine("  to { opacity: " + (style.lyricsStyle != null ? style.lyricsStyle.opacity : 0) + "; }");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine("@keyframes fadeOutLyrics {");
        output.AppendLine("  from { opacity: " + (style.lyricsStyle != null ? style.lyricsStyle.opacity : 0) + "; }");
        output.AppendLine("  to { opacity: 0; }");
        output.AppendLine("}");

        output.AppendLine(".projection-wrapper .fade-in .projection-lyrics {");
        output.AppendLine("  animation: fadeInLyrics ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine(".projection-wrapper .fade-out .projection-lyrics {");
        output.AppendLine("  animation: fadeOutLyrics ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");

        output.AppendLine(".projection-wrapper .fade-in .embed-video,");
        output.AppendLine(".projection-wrapper .fade-in .youtube-player-wrapper,");
        output.AppendLine(".background-wrapper.fade-in {");
        output.AppendLine("  animation: fadeIn ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");
        output.AppendLine("");
        output.AppendLine(".projection-wrapper .fade-out .embed-video,");
        output.AppendLine(".projection-wrapper .fade-out .youtube-player-wrapper,");
        output.AppendLine(".background-wrapper.fade-out {");
        output.AppendLine("  animation: fadeOut ease " + transitionSpeedMs + "ms;");
        output.AppendLine("  animation-fill-mode: forwards;");
        output.AppendLine("}");

        return output.ToString();
    }

    /// <summary>
    /// Transposes a chord from the original key to the selected key.
    /// </summary>
    /// <param name="Chord">The chord to transpose.</param>
    /// <param name="OriginalKey">The original song key.</param>
    /// <param name="NewKey">The selected key.</param>
    /// <returns>The transposed chord.</returns>
    public static string TransposeChord(string? Chord, string? OriginalKey, string? NewKey)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(Chord)) {
            string originalKey = FormatChordSpecialCharacters(OriginalKey);
            string newKey = FormatChordSpecialCharacters(NewKey);
            string chord = FormatChordSpecialCharacters(Chord);

            if (chord.Contains("/")) {
                var parts = chord.Split("/");
                foreach (var part in parts) {
                    if (output != "") {
                        output += "/";
                    }

                    output += TransposeChord(part, OriginalKey, NewKey);
                }
            } else {
                output = chord;

                string chordRoot = ChordRootMatrix(chord);

                string chordRemainder = chord;
                if (chordRoot.Length > 0) {
                    chordRemainder = chord.Substring(chordRoot.Length);
                }

                if (originalKey != newKey) {
                    // Find the key in the matrix
                    var keyMatrixOriginal = GetKeyMatrix(originalKey);
                    var keyMatrixNew = GetKeyMatrix(newKey);

                    if (keyMatrixOriginal != null && keyMatrixNew != null && keyMatrixOriginal != keyMatrixNew) {
                        int position = -1;

                        for (int x = 0; x < 12; x++) {
                            var items = keyMatrixOriginal.items[x];

                            if (position == -1 && items.Contains("{" + chordRoot + "}")) {
                                position = x;
                            }
                        }

                        if (position > -1) {
                            output = keyMatrixNew.items[position];
                            output = output
                                .Replace("}{", "/")
                                .Replace("{", "")
                                .Replace("}", "");

                            if (output.Contains("/")) {
                                bool preferSharp = keyMatrixNew.preferSharp;

                                var items = output.Split('/');
                                if (items != null && items.Any()) {
                                    foreach (var item in items) {
                                        if (preferSharp && item.Contains("♯")) {
                                            output = item;
                                        } else if (!preferSharp && item.Contains("♭")) {
                                            output = item;
                                        }
                                    }

                                    output += chordRemainder;
                                }
                            } else {
                                output += chordRemainder;
                            }
                        }
                    }
                }
            }
        }

        return output;
    }

    /// <summary>
    /// Transposes a key by the given number of steps.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="steps">The number of steps to transpose, a number between -11 and 11.</param>
    /// <returns>The transposed key.</returns>
    public static string TransposeKey(string? key, int steps)
    {
        string output = "";

        if (!String.IsNullOrWhiteSpace(key)) {
            output = key;

            if (steps > 11 || steps < -11) {
                return output;
            }

            if (steps != 0) {
                List<string> transpose = new List<string>();

                switch (key.ToUpper()) {
                    case "Ab":
                    case "A♭":
                    case "G#":
                    case "G♯":
                        transpose = new List<string> { "A", "Bflat", "B", "C", "Csharp", "D", "Eflat", "E", "F", "Fsharp", "G" };
                        break;

                    case "A":
                        transpose = new List<string> { "Bflat", "B", "C", "Csharp", "D", "Eflat", "E", "F", "Fsharp", "G", "Aflat" };
                        break;

                    case "A#":
                    case "A♯":
                    case "Bb":
                    case "B♭":
                        transpose = new List<string> { "B", "C", "Csharp", "D", "Eflat", "E", "F", "Fsharp", "G", "Aflat", "A" };
                        break;

                    case "B":
                    case "B#":
                    case "B♯":
                    case "Cb":
                    case "C♭":
                        transpose = new List<string> { "C", "Csharp", "D", "Eflat", "E", "F", "Fsharp", "G", "Aflat", "A", "Bflat" };
                        break;

                    case "C":
                        transpose = new List<string> { "Csharp", "D", "Eflat", "E", "F", "Fsharp", "G", "Aflat", "A", "Bflat", "B" };
                        break;

                    case "C#":
                    case "C♯":
                    case "Db":
                    case "D♭":
                        transpose = new List<string> { "D", "Eflat", "E", "F", "Fsharp", "G", "Aflat", "A", "Bflat", "B", "C" };
                        break;

                    case "D":
                        transpose = new List<string> { "Eflat", "E", "F", "Fsharp", "G", "Aflat", "A", "Bflat", "B", "C", "Csharp" };
                        break;

                    case "D#":
                    case "D♯":
                    case "Eb":
                    case "E♭":
                        transpose = new List<string> { "E", "F", "Fsharp", "G", "Aflat", "A", "Bflat", "B", "C", "Csharp", "D" };
                        break;

                    case "E":
                        transpose = new List<string> { "F", "Fsharp", "G", "Aflat", "A", "Bflat", "B", "C", "Csharp", "D", "Eflat" };
                        break;

                    case "E#":
                    case "E♯":
                    case "Fb":
                    case "F♭":
                    case "F":
                        transpose = new List<string> { "Fsharp", "G", "Aflat", "A", "Bflat", "B", "C", "Csharp", "D", "Eflat", "E" };
                        break;

                    case "F#":
                    case "F♯":
                    case "Gb":
                    case "G♭":
                        transpose = new List<string> { "G", "Aflat", "A", "Bflat", "B", "C", "Csharp", "D", "Eflat", "E", "F" };
                        break;

                    case "G":
                        transpose = new List<string> { "Aflat", "A", "Bflat", "B", "C", "Csharp", "D", "Eflat", "E", "F", "Fsharp" };
                        break;
                }

                if (transpose.Any()) {
                    if (steps > 0) {
                        output = transpose[steps - 1];
                    } else {
                        output = transpose[11 + steps];
                    }

                    output = output
                        .Replace("sharp", "♯")
                        .Replace("flat", "♭");
                }
            }
        }

        return output;
    }

    /// <summary>
    /// URL decodes the given input.
    /// </summary>
    /// <param name="url">The input to be decoded.</param>
    /// <returns>The UrlDecoded text.</returns>
    public static string UrlDecode(string? url)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(url)) {
            output = System.Web.HttpUtility.UrlDecode(url);
        }

        return output;
    }

    /// <summary>
    /// URL encodes the given input.
    /// </summary>
    /// <param name="url">The input to be encoded.</param>
    /// <returns>The UrlEncoded text.</returns>
    public static string UrlEncode(string? url)
    {
        string output = String.Empty;

        if (!String.IsNullOrWhiteSpace(url)) {
            output = System.Web.HttpUtility.UrlEncode(url);
        }

        return output;
    }

    public static string Uri {
        get {
            return NavManager.Uri;
        }
    }

    /// <summary>
    /// Gets or sets the flag to indicate if the currently-rendered song uses the Do-Re-Mi system.
    /// </summary>
    public static bool UsingDoReMi {
        get { return _UsingDoReMi; }
        set {
            _UsingDoReMi = value;
        }
    }

    /// <summary>
    /// Gets or sets the flag to indicate if the currently-rendered song uses the Nashville Numbering System.
    /// </summary>
    public static bool UsingNashvilleNumbering {
        get { return _UsingNashvilleNumbering; }
        set {
            _UsingNashvilleNumbering = value;
        }
    }

    /// <summary>
    /// Verifies that the current song and songs in the set list still exist in songbooks.
    /// </summary>
    public static void ValidateCurrentSongAndSetListItems()
    {
        // Make sure the current song still exists in the songbook.
        if (Model.Song.id != Guid.Empty) {
            var songbook = GetSongBook(Model.Song.songBookId);
            if (songbook == null) {
                Model.Song = new song();
                Model.CurrentComponentType = String.Empty;
            }
        }

        // See if there are any set list items that are songs in songbooks that no longer exist.
        var removeSetListItems = new List<Guid>();
        foreach(var item in Model.SetList.items) {
            if (item.type == setListItemType.song && item.item != null) {
                var song = (song)item.item;

                if (song != null) {
                    var songbook = GetSongBook(song.songBookId);
                    if (songbook == null) {
                        removeSetListItems.Add(item.id);
                    } else {
                        var songInBook = songbook.songs.FirstOrDefault(x => x.id == song.id);
                        if (songInBook == null) {
                            removeSetListItems.Add(item.id);
                        }
                    }
                }
            }
        }

        if (removeSetListItems.Any()) {
            Model.SetList.items = Model.SetList.items.Where(x => !removeSetListItems.Contains(x.id)).ToList();
            Model.TriggerUpdate();
        }
    }

    /// <summary>
    /// Adjusts the zoom level using jsInterop.
    /// </summary>
    /// <param name="zoom">The zoom level.</param>
    public static async Task Zoom(int zoom)
    {
        await jsRuntime.InvokeVoidAsync("Zoom", zoom);
    }
}
