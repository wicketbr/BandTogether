using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CodeAnalysis;
using System.Data;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;

namespace BandTogether;

#region Enumerations and Data Objects used by the CRM
public enum MessageType
{
    Primary,
    Secondary,
    Success,
    Danger,
    Warning,
    Info,
    Light,
    Dark,
}

public class Message
{
    public string Id { get; set; } = "";
    public bool AutoHide { get; set; }
    public DateTime Shown { get; set; } = DateTime.UtcNow;
    public string Text { get; set; } = "";
    public string TimeLabel { get; set; } = "";
    public MessageType MessageType { get; set; } = MessageType.Dark;
    public bool ReplaceLineBreaks { get; set; }
}

public class NewMessage
{
    public string Text { get; set; } = "";
    public MessageType MessageType { get; set; } = MessageType.Dark;
}

public enum TextCase
{
    Normal,
    Lowercase,
    Uppercase,
    Sentence,
    Title,
}
#endregion

public class BlazorDataModel
{
    private List<string> _AudioFiles = new List<string>();
    private bool _AudioPlaying = false;
    private string _AudioPlayingFile = String.Empty;
    private audioItemCurrentTime _AudioPlayingTime = new audioItemCurrentTime();
    private bool _AudioPaused = false;
    private List<string> _Backgrounds = new List<string>();
    private bool _BlankScreen = false;
    private string _CultureCode = "en-US";
    private List<string> _CultureCodes = new List<string>();
    private string _CurrentComponentType = String.Empty;
    private dataLoader _DataLoader = new dataLoader();
    private bool _DialogOpen = false;
    private List<string> _DotNetHelperMessages = new List<string>();
    private bool _HideText = false;
    private List<string> _Images = new List<string>();
    private List<Language> _Languages = new List<Language>();
    private bool _Loaded = false;
    private DateTime _LoadedAt = DateTime.UtcNow;
    private string _MatrixKey = String.Empty;
    private int _MaxTransitionDuration = 3000;
    private messages _MessageItems = new messages();
    private List<Message> _Messages = new List<Message>();
    private DateTime _ModelUpdated = DateTime.UtcNow;
    private string? _NavigationId = String.Empty;
    private string _OriginalMatrixKey = String.Empty;
    private DateOnly? _Released;
    private appSettings _Settings = new appSettings();
    private setList _SetList = new setList();
    private List<setListFile> _SetListFilenames = new List<setListFile>();
    private HubConnection _signalrHub = null!;
    private List<string> _Slideshows = new List<string>();
    private song _Song = new song();
    private List<songBook> _SongBooks = new List<songBook>();
    private List<string> _Subscribers_OnAudioPlaybackEnded = new List<string>();
    private List<string> _Subscribers_OnChange = new List<string>();
    private List<string> _Subscribers_OnDotNetHelperHandler = new List<string>();
    private List<string> _Subscribers_OnKeyboardEvent = new List<string>();
    private List<string> _Subscribers_OnSignalRMessage = new List<string>();
    private List<string> _Subscribers_OnSignalRSetList = new List<string>();
    private List<string> _Subscribers_OnSignalRUpdate = new List<string>();
    private List<string> _Subscribers_OnSignalRUser = new List<string>();
    private user _User = new user();
    private List<user> _Users = new List<user>();
    private string _Version = String.Empty;
    private List<string> _Vidoes = new List<string>();
    private string _View = String.Empty;
    private int _YouTubePlayingStatus = -1;
    protected int _zoomMax = 200;
    protected int _zoomMin = 24;

    /// <summary>
    /// Contains the active item in the item view pane.
    /// </summary>
    public setListItem? ActiveItem {
        get {
            setListItem? output = null;

            if (_SetList.items.Any() && _SetList.activeItem.HasValue) {
                output = _SetList.items.FirstOrDefault(x => x.id == _SetList.activeItem.Value);
            }

            return output;
        }
    }

    public int ActiveItemPartCount {
        get {
            int output = 0;

            var item = _SetList.items.FirstOrDefault(x => x.id == _SetList.activeItem);

            if (item != null ) {
                switch (item.type) {
                    case setListItemType.audio:
                    case setListItemType.clock:
                    case setListItemType.countdown:
                    case setListItemType.image:
                    case setListItemType.video:
                    case setListItemType.youTube:
                        output = 0;
                        break;

                    case setListItemType.slideshow:
                        var slideshow = Tools.SetListItemAsSlideshow(item);
                        if (slideshow != null && slideshow.images != null) {
                            output = slideshow.images.Count;
                        }
                        break;

                    case setListItemType.song:
                        var song = Tools.SetListItemAsSong(item);
                        if (song != null && song.parts != null) {
                            output = song.parts.Count;
                        }
                        break;
                }
            }

            return output;
        }
    }

    public string ActiveItemType {
        get {
            string output = String.Empty;

            if (ActiveItem != null) {
                output = ActiveItem.type;
            }

            return output;
        }
    }

    public int ActiveSetListIndex {
        get {
            int output = -1;

            if (_SetList.selectedItem != null) {
                output = _SetList.items.FindIndex(x => x.id == _SetList.selectedItem);
            }

            return output;
        }
    }

    public song? ActiveSong {
        get {
            song? output = null;

            if (ActiveItem != null) {
                output = Tools.SetListItemAsSong(ActiveItem);
            }

            return output;
        }
    }

    /// <summary>
    /// Adds a Toast message to the user interface.
    /// </summary>
    /// <param name="message">A message object to be added.</param>
    /// <param name="AutoHide">If true the message will automatically be hidden after 5 seconds.</param>
    /// <param name="RemovePreviousMessages">If true any previous messages will be removed and only this new message will be shown.</param>
    /// <param name="ReplaceLineBreaks">Option to replace any line breaks in the text with HTML an &lt;br /&gt; element.</param>
    public void AddMessage(NewMessage message, bool AutoHide = true, bool RemovePreviousMessages = false, bool ReplaceLineBreaks = false)
    {
        if (RemovePreviousMessages) {
            _Messages = new List<Message> {
                new Message {
                    Id = Guid.NewGuid().ToString(),
                    AutoHide = AutoHide,
                    MessageType = message.MessageType,
                    Shown = DateTime.UtcNow,
                    Text = message.Text,
                    TimeLabel = "",
                    ReplaceLineBreaks = ReplaceLineBreaks,
                }
            };
        } else {
            _Messages.Add(new Message {
                Id = Guid.NewGuid().ToString(),
                AutoHide = AutoHide,
                MessageType = message.MessageType,
                Shown = DateTime.UtcNow,
                Text = message.Text,
                TimeLabel = "",
                ReplaceLineBreaks = ReplaceLineBreaks,
            });
        }
        _ModelUpdated = DateTime.UtcNow;
        NotifyDataChanged();
    }

    /// <summary>
    /// Adds a Toast message to the user interface.
    /// </summary>
    /// <param name="message">The message to add. And text inside double curly brackets (eg: {{Tag}}) will be replaced with the language tag for that item.</param>
    /// <param name="messageType">The message type, related to a Bootstrap type.</param>
    /// <param name="AutoHide">If true the message will automatically be hidden after 5 seconds.</param>
    /// <param name="RemovePreviousMessages">If true any previous messages will be removed and only this new message will be shown.</param>
    /// <param name="ReplaceLineBreaks">Option to replace any line breaks in the text with HTML an &lt;br /&gt; element.</param>
    public void AddMessage(string message, MessageType messageType = MessageType.Primary, bool AutoHide = true, bool RemovePreviousMessages = false, bool ReplaceLineBreaks = false)
    {
        AddMessage(new NewMessage {
            Text = message,
            MessageType = messageType,
        }, AutoHide, RemovePreviousMessages, ReplaceLineBreaks);
    }

    /// <summary>
    /// Gets or sets the app url.
    /// </summary>
    public string ApplicationUrl {
        get {
            string output = Helpers.BaseUri;
            if (!output.EndsWith("/")) {
                output += "/";
            }
            return output;
        }
    }

    public string AspectRatio {
        get {
            return Helpers.AspectRatioToCssClass(_Settings.projectionModeAspectRatio);
        }
    }

    public string AspectRatioBootstrapResponsive {
        get {
            return Helpers.AspectRatioToBootstrapResponsive(_Settings.projectionModeAspectRatio);
        }
    }

    /// <summary>
    /// Gets or sets the list of available audio files.
    /// </summary>
    public List<string> AudioFiles {
        get { return _AudioFiles; }
        set {
            if (!ObjectsAreEqual(_AudioFiles, value)) {
                _AudioFiles = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Indicates if an audio file is playing.
    /// </summary>
    public bool AudioPlaying {
        get { return _AudioPlaying; }
        set {
            if (_AudioPlaying != value) {
                _AudioPlaying = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The current audio file playing.
    /// </summary>
    public string AudioPlayingFile {
        get { return _AudioPlayingFile; }
        set {
            if (_AudioPlayingFile != value) {
                _AudioPlayingFile = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public audioItemCurrentTime AudioPlayingTime {
        get { return _AudioPlayingTime; } 
        set {
            if (!ObjectsAreEqual(_AudioPlayingTime, value)) {
                _AudioPlayingTime = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> AvailableFonts {
        get {
            var output = _Settings.availableFonts;

            foreach(var font in _DataLoader.fontWoffFiles) {
                if (!output.Contains(font)) {
                    output.Add(font);
                }
            }

            if (output.Count > 0) {
                output = output.OrderBy(x => x).ToList();
            }

            

            return output;
        }
    }

    public bool AudioPaused {
        get { return _AudioPaused; } 
        set {
            if (_AudioPaused != value) {
                _AudioPaused = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> BackgroundImages {
        get {
            var output = new List<string>();

            var backgrounds = _Backgrounds;
            if (backgrounds.Any()) {
                foreach(var item in backgrounds) {
                    switch (Helpers.StringLower(System.IO.Path.GetExtension(item))) {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                            output.Add(item);
                            break;
                    }
                }
            }

            return output;
        }
    }

    /// <summary>
    /// Gets or sets the available backgrounds.
    /// </summary>
    public List<string> Backgrounds {
        get { return _Backgrounds; }
        set {
            if (!ObjectsAreEqual(_Backgrounds, value)) {
                _Backgrounds = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> BackgroundVideos {
        get {
            var output = new List<string>();

            var backgrounds = _Backgrounds;
            if (backgrounds.Any()) {
                foreach(var item in backgrounds) {
                    switch (Helpers.StringLower(System.IO.Path.GetExtension(item))) {
                        case ".mp4":
                        case ".m4v":
                        case ".webm":
                            output.Add(item);
                            break;
                    }
                }
            }

            return output;
        }
    }

    public bool BlankScreen {
        get { return _BlankScreen; }
        set {
            if (_BlankScreen != value) {
                _BlankScreen = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> ChordsInSong {
        get {
            var output = new List<string>();

            if (!String.IsNullOrEmpty(_Song.content)) {
                var matches = System.Text.RegularExpressions.Regex.Matches(_Song.content, @"\[([^\]]*)\]");
		        
                output = matches
			        .Select(x => x.Value)
			        .Distinct()
			        .Where(x => !x.StartsWith("[!") && x != "[]")
			        .Select(x => x.Substring(1, x.Length - 2))
			        .OrderBy(x => x)
			        .ToList();
            }

            return output;
        }
    }

    /// <summary>
    /// Clears any Toast messages in the user interface.
    /// </summary>
    public void ClearMessages()
    {
        _Messages = new List<Message>();
        _ModelUpdated = DateTime.UtcNow;
        NotifyDataChanged();
    }

    /// <summary>
    /// The current culture code (defaults to 'en-US').
    /// </summary>
    public string CultureCode {
        get { return _CultureCode; }
        set {
            if (_CultureCode != value) {
                _CultureCode = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// A collection of all available culture codes.
    /// </summary>
    public List<string> CultureCodes {
        get { return _CultureCodes; }
        set {
            if (!ObjectsAreEqual(_CultureCodes, value)) {
                _CultureCodes = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public string CurrentComponentType {
        get { return _CurrentComponentType; }
        set {
            if (_CurrentComponentType != value) {
                _CurrentComponentType = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public int CurrentSetListIndex {
        get {
            if (_SetList.items.Any()) {
                foreach(var item in _SetList.items.Index()) {
                    if (item.Item.id == _SetList.activeItem) {
                        return item.Index;
                    }
                }
            }

            return 0;
        }
    }

    public dataLoader DataLoader {
        get { return _DataLoader; }
        set {
            if (!ObjectsAreEqual(_DataLoader, value)) {
                _DataLoader = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public bool DialogOpen {
        get { return _DialogOpen; }
        set {
            if (_DialogOpen != value) {
                _DialogOpen = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public bool EmptySetList {
        get {
            bool output = _SetList.items.Count == 0;

            if (!output && _SetList.items.Count(x => x.id != Guid.Empty) == 0) {
                output = true;
            }

            return output;
        }
    }

    /// <summary>
    /// Shows a Toast with an error message.
    /// </summary>
    /// <param name="message">A message object to be added.</param>
    /// <param name="AutoHide">If true the message will automatically be hidden after 5 seconds.</param>
    /// <param name="RemovePreviousMessages">If true any previous messages will be removed and only this new message will be shown.</param>
    /// <param name="ReplaceLineBreaks">Option to replace any line breaks in the text with HTML an &lt;br /&gt; element.</param>
    public void ErrorMessage(string message, bool AutoHide = false, bool RemovePreviousMessages = true, bool ReplaceLineBreaks = false)
    {
        if (!String.IsNullOrWhiteSpace(message)) {
            AddMessage(new NewMessage {
                Text = message,
                MessageType = MessageType.Danger,
            }, AutoHide, RemovePreviousMessages, ReplaceLineBreaks);
        }
    }

    /// <summary>
    /// Shows a Toast with one or more error messages.
    /// </summary>
    /// <param name="messages">A Collection of Messages</param>
    /// <param name="AutoHide">If true the message will automatically be hidden after 5 seconds.</param>
    /// <param name="RemovePreviousMessages">If true any previous messages will be removed and only this new message will be shown.</param>
    /// <param name="ReplaceLineBreaks">Option to replace any line breaks in the text with HTML an &lt;br /&gt; element.</param>
    public void ErrorMessages(List<string> messages, bool AutoHide = false, bool RemovePreviousMessages = true, bool ReplaceLineBreaks = false)
    {
        if (messages.Any()) {
            string message = "";

            if (messages.Count() == 1) {
                message += "<div class=\"mt-2\">" + messages[0] + "</div>";
            } else {
                message += "<ul class=\"mt-2\">";
                foreach (var msg in messages) {
                    message += "<li>" + msg + "</li>";
                }
                message += "</ul>";
            }

            AddMessage(new NewMessage {
                Text = message,
                MessageType = MessageType.Danger,
            }, AutoHide, RemovePreviousMessages, ReplaceLineBreaks);
        }
    }

    public double FontBaseMultiplier {
        get {
            return .095;
        }
    }

    public bool HideText {
        get { return _HideText; }
        set {
            if (_HideText != value) {
                _HideText = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Indicate if a song is currently loaded.
    /// </summary>
    public bool InSong {
        get {
            return _Song.id != Guid.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the list of available images.
    /// </summary>
    public List<string> Images {
        get { return _Images; }
        set {
            if (!ObjectsAreEqual(_Images, value)) {
                _Images = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the collection of Language objects.
    /// </summary>
    public List<Language> Languages {
        get { return _Languages; }
        set {
            if (!ObjectsAreEqual(_Languages, value)) {
                _Languages = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Indicates if the Model has been loaded.
    /// </summary>
    public bool Loaded {
        get { return _Loaded; }
        set {
            if (_Loaded != value) {
                _Loaded = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Indicates the time this DataModel was loaded.
    /// </summary>
    public DateTime LoadedAt {
        get { return _LoadedAt; }
        set {
            if (_LoadedAt != value) {
                _LoadedAt = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Indicates if a user is selected.
    /// </summary>
    public bool LoggedIn {
        get {
            return _User != null && _User.id != Guid.Empty;
        }
    }

    /// <summary>
    /// Gets or sets the current Matrix key.
    /// </summary>
    public string MatrixKey {
        get { return _MatrixKey; }
        set {
            if (_MatrixKey != value) {
                _MatrixKey = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public int MaxTransitionDuration {
        get {  return _MaxTransitionDuration; }
        set {
            if (_MaxTransitionDuration != value) {
                _MaxTransitionDuration = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the message items for screens and tables.
    /// </summary>
    public messages MessageItems {
        get { return _MessageItems; }
        set {
            if (!ObjectsAreEqual(_MessageItems, value)) {
                _MessageItems = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Shows a standard "Deleting" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Deleting(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='me-2 toast-large-icon'><i class='fas fa-trash'></i></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.DeletingWait + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Danger, false);
    }

    /// <summary>
    /// Shows a standard "Loading" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Loading(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='spinner-grow me-2' role='status'></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.LoadingWait + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Success, false);
    }

    /// <summary>
    /// Shows a standard "Processing" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Processing(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='spinner-grow me-2' role='status'></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.ProcessingWait + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Success, false);
    }

    /// <summary>
    /// Shows a standard "Saved" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Saved(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='me-2 toast-large-icon'><i class='fa fa-save'></i></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.SavedAt + " " + DateTime.Now.ToShortTimeString() + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Success, true, true);
    }

    /// <summary>
    /// Shows a standard "Saving" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Saving(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='me-2 toast-large-icon'><i class='fa fa-save'></i></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.SavingWait + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Success, false);
    }

    /// <summary>
    /// Shows a standard "Success" message.
    /// </summary>
    /// <param name="message">Optional message to override the default message.</param>
    public void Message_Success(string message = "")
    {
        if (String.IsNullOrWhiteSpace(message)) {
            message =
                "<div class='d-flex align-items-center'>\n" +
                "  <div class='me-2 toast-large-icon'><i class='fa fa-save'></i></div>\n" +
                "  <div class='me-auto toast-large'>" + Text.Success + "</div>\n" +
                "</div>\n";
        }

        AddMessage(message, MessageType.Success, true);
    }

    /// <summary>
    /// Contains the current Toast messages for the user interface.
    /// </summary>
    public List<Message> Messages {
        get { return _Messages; }
        set {
            if (!ObjectsAreEqual(_Messages, value)) {
                _Messages = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The DateTime value of when the model was last updated.
    /// </summary>
    public DateTime ModelUpdated {
        get { return _ModelUpdated; }
    }

    /// <summary>
    /// Gets or sets the optional ID in the navigation.
    /// </summary>
    public string? NavigationId {
        get { return _NavigationId; }
        set {
            if (_NavigationId != value) {
                _NavigationId = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Used to compare if two objects are equal.
    /// </summary>
    /// <param name="obj1">The first object.</param>
    /// <param name="obj2">The second object.</param>
    /// <returns>True if the objects serialize the same.</returns>
    public bool ObjectsAreEqual(object? obj1, object? obj2)
    {
        if (obj1 == null && obj2 != null) {
            return false;
        } else if (obj2 == null && obj1 != null) {
            return false;
        } else {
            return System.Text.Json.JsonSerializer.Serialize(obj1) == System.Text.Json.JsonSerializer.Serialize(obj2);
        }
    }

    public void OnAudioPlaybackHasEnded()
    {
        OnAudioPlaybackEnded?.Invoke();
    }

    /// <summary>
    /// The handler that receives keyboard events from javascript and alerts any subcribers of the OnKeyboardEvent event in the model.
    /// </summary>
    /// <param name="keyboard"></param>
    public void OnKeyboardEventProcessor(keyboardEvent keyboard)
    {
        OnKeyboardEvent?.Invoke(keyboard);
    }

    /// <summary>
    /// Gets or sets the Orignal Matrix Key.
    /// </summary>
    public string OriginalMatrixKey {
        get { return _OriginalMatrixKey; }
        set {
            if (_OriginalMatrixKey != value) {
                _OriginalMatrixKey = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the Release date.
    /// </summary>
    public DateOnly? Released {
        get { return _Released; }
        set {
            if (_Released != value) {
                _Released = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Contains the selected item in the item view pane.
    /// </summary>
    public object? SelectedItem {
        get {
            object? output = null;

            if (_SetList.items.Any() && _SetList.selectedItem.HasValue && _SetList.selectedItem.Value != Guid.Empty) {
                output = _SetList.items.FirstOrDefault(x => x.id == _SetList.selectedItem.Value);
            } else if (_Song.id != Guid.Empty) {
                output = _Song;
            }

            return output;
        }
    }

    /// <summary>
    /// Sends a message to all clients.
    /// </summary>
    /// <param name="msg">The message object.</param>
    public async Task SendSignalRMessage(message msg)
    {
        await _signalrHub.SendAsync("message", msg);
    }

    /// <summary>
    /// Sends a signalR message to all clients.
    /// </summary>
    /// <param name="update">The SignalRUpdate object</param>
    public async Task SendSignalRUpdate(signalRUpdate update)
    {
        await _signalrHub.SendAsync("update", update);
    }

    /// <summary>
    /// Sends the updated SetList to all client.
    /// </summary>
    /// <param name="setList">The SetList Object.</param>
    public async Task SendSignalRSetListUpdate(setList setList)
    {
        var dup = Helpers.DuplicateObject<setList>(setList);
        if (dup != null) {
            foreach(var item in dup.items) {
                if (item.item != null) {
                    item.itemJson = Helpers.SerializeObject(item.item);
                    item.item = null;
                }
            }

            await _signalrHub.SendAsync("setlist", dup);
        }
    }

    /// <summary>
    /// Gets or sets the app settings.
    /// </summary>
    public appSettings Settings {
        get { return _Settings; }
        set {
            if (!ObjectsAreEqual(_Settings, value)) {
                _Settings = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the SetList.
    /// </summary>
    public setList SetList {
        get {return _SetList;}
        set {
            if (!ObjectsAreEqual(_SetList, value)) {
                _SetList = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the list of set list filenames.
    /// </summary>
    public List<setListFile> SetListFilenames {
        get { return _SetListFilenames; }
        set {
            if (!ObjectsAreEqual(_SetListFilenames, value)) {
                _SetListFilenames = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The app signalR hub connection
    /// </summary>
    public HubConnection signalrRHub {
        get { return _signalrHub; }
        set {
            if (_signalrHub != value) {
                _signalrHub = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public void SignalRMessage(message msg)
    {
        NotifySignalRMessage(msg);
    }

    /// <summary>
    /// The method that notifies other pages of SignalR Set List updates.
    /// </summary>
    /// <param name="setList">The SetList object.</param>
    public void SignalRSetList(setList setlist)
    {
        NotifySignalRSetList(setlist);
    }

    /// <summary>
    /// The method that notifies other pages of SignalR updates.
    /// </summary>
    /// <param name="update">The SignalRUpdate object.</param>
    public void SignalRUpdate(signalRUpdate update)
    {
        NotifySignalRUpdate(update);
    }

    /// <summary>
    /// The method that notifies other pages of SignalR user updates.
    /// </summary>
    /// <param name="user"></param>
    public void SignalRUser(user user)
    {
        NotifySignalRUser(user);
    }

    /// <summary>
    /// Gets or sets the list of slideshow folders.
    /// </summary>
    public List<string> Slideshows {
        get { return _Slideshows; }
        set {
            if (!ObjectsAreEqual(_Slideshows, value)) {
                _Slideshows = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the current song.
    /// </summary>
    public song Song {
        get { return _Song; }
        set {
            if (!ObjectsAreEqual(_Song, value)) {
                _Song = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the collection of SongBook objects.
    /// </summary>
    public List<songBook> SongBooks {
        get { return _SongBooks; }
        set {
            if (!ObjectsAreEqual(_SongBooks, value)) {
                _SongBooks = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets the key of the current song.
    /// </summary>
    public string SongKey {
        get {
            return _Song.key;
        }
    }

    /// <summary>
    /// Gets the key label of the current song.
    /// </summary>
    public string SongKeyLabel {
        get {
            if (SongKeyMinor) {
                return _MatrixKey + "m";
            } else {
                return _MatrixKey;
            }
        }
    }

    /// <summary>
    /// Indicates if the current song is in a minor key.
    /// </summary>
    public bool SongKeyMinor {
        get {
            bool output = false;

            if (!String.IsNullOrWhiteSpace(_Song.key)) {
                if (_Song.key.Contains("m") || _Song.key.Contains("minor")) {
                    output = true;
                }
            }

            return output;
        }
    }

    /// <summary>
    /// Gets the key type of the current song ("minor" or "major".)
    /// </summary>
    public string SongKeyType {
        get {
            string output = SongKeyMinor ? "minor" : "major";
            return output;
        }
    }

    public List<songPart> SongParts {
        get {
            var output = Tools.SongParts(_Song);
            return output;
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnAudioPlaybackEnded event.
    /// </summary>
    public List<string> Subscribers_OnAudioPlaybackEnded {
        get { return _Subscribers_OnAudioPlaybackEnded; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnAudioPlaybackEnded, value)) {
                _Subscribers_OnAudioPlaybackEnded = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnChange event.
    /// </summary>
    public List<string> Subscribers_OnChange {
        get { return _Subscribers_OnChange; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnChange, value)) {
                _Subscribers_OnChange = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnDotNetHelperHandler event.
    /// </summary>
    public List<string> Subscribers_OnDotNetHelperHandler {
        get { return _Subscribers_OnDotNetHelperHandler; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnDotNetHelperHandler, value)) {
                _Subscribers_OnDotNetHelperHandler = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> Subscribers_OnKeyboardEvent {
        get { return _Subscribers_OnKeyboardEvent; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnKeyboardEvent, value)) {
                _Subscribers_OnKeyboardEvent = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public List<string> Subscribers_OnSignalRMessage {
        get { return _Subscribers_OnSignalRMessage; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnSignalRMessage, value)) {
                _Subscribers_OnSignalRMessage = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnSignalRSetList event.
    /// </summary>
    public List<string> Subscribers_OnSignalRSetList {
        get { return _Subscribers_OnSignalRSetList;}
        set {
            if (!ObjectsAreEqual(_Subscribers_OnSignalRSetList, value)) {
                _Subscribers_OnSignalRSetList = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnSignalRUpdate event.
    /// </summary>
    public List<string> Subscribers_OnSignalRUpdate {
        get { return _Subscribers_OnSignalRUpdate; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnSignalRUpdate, value)) {
                _Subscribers_OnSignalRUpdate = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of subscribers to the OnSignalRUser event.
    /// </summary>
    public List<string> Subscribers_OnSignalRUser {
        get { return _Subscribers_OnSignalRUser; }
        set {
            if (!ObjectsAreEqual(_Subscribers_OnSignalRUser, value)) {
                _Subscribers_OnSignalRUser = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The method used to notify pages of data model updates.
    /// </summary>
    public void TriggerUpdate()
    {
        _ModelUpdated = DateTime.UtcNow;
        NotifyDataChanged();
    }

    /// <summary>
    /// Shows a Toast message stating that an unknown error has occurred.
    /// </summary>
    /// <param name="errorMessage">An optional message to show. If not set then the ErrorUnknown language tag is used.</param>
    /// <param name="AutoHide">If true the message will automatically be hidden after 5 seconds.</param>
    /// <param name="RemovePreviousMessages">If true any previous messages will be removed and only this new message will be shown.</param>
    /// <param name="ReplaceLineBreaks">Option to replace any line breaks in the text with HTML an &lt;br /&gt; element.</param>
    public void UnknownError(string errorMessage = "", bool AutoHide = false, bool RemovePreviousMessages = true, bool ReplaceLineBreaks = false)
    {
        if (String.IsNullOrWhiteSpace(errorMessage)) {
            errorMessage = Text.UnknownError;
        }

        ErrorMessage(errorMessage, AutoHide, RemovePreviousMessages, ReplaceLineBreaks);
    }

    /// <summary>
    /// The User object for the current user, or an empty User object if no user is logged in.
    /// </summary>
    public user User {
        get { return _User; }
        set {
            if (!ObjectsAreEqual(_User, value)) {
                _User = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The collection of all User objects for the current user (as a user may have accounts in more than one tenant).
    /// </summary>
    public List<user> Users {
        get { return _Users; }
        set {
            if (!ObjectsAreEqual(_Users, value)) {
                _Users = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the app version.
    /// </summary>
    public string Version {
        get { return _Version; }
        set {
            if (_Version != value) {
                _Version = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the list of available videos.
    /// </summary>
    public List<string> Videos {
        get { return _Vidoes; }
        set {
            if (!ObjectsAreEqual(_Vidoes, value)) {
                _Vidoes = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// The current view of the application.
    /// </summary>
    public string View {
        get { return _View; }
        set {
            if (_View != value) {
                _View = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    public int YouTubePlayingStatus {
        get { return _YouTubePlayingStatus; }
        set {
            if (_YouTubePlayingStatus != value) {
                _YouTubePlayingStatus = value;
                _ModelUpdated = DateTime.UtcNow;
                NotifyDataChanged();
            }
        }
    }

    /// <summary>
    /// Returns the max zoom level.
    /// </summary>
    public int ZoomMax {
        get { return _zoomMax; }
    }

    /// <summary>
    /// Returns the min zoom level.
    /// </summary>
    public int ZoomMin {
        get { return _zoomMin; }
    }

    /// <summary>
    /// The OnAudioPlaybackEnded event that can be subscribed to in a view or component to be notified when audio playback ends.
    /// </summary>
    public event Action? OnAudioPlaybackEnded;

    /// <summary>
    /// The OnChange event that can be subscribed to in a view or component to be notified when this model changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// An event that can be subscribed to for updates from the javascript DotNetHelper.
    /// </summary>
    public event Action<List<string>>? OnDotNetHelperHandler;

    /// <summary>
    /// An event that can be subscribed to for keyboard events.
    /// </summary>
    public event Action<keyboardEvent>? OnKeyboardEvent;

    /// <summary>
    /// An event that can be subscribe to for SignalR message updates.
    /// </summary>
    public event Action<message>? OnSignalRMessage;

    /// <summary>
    /// An event that can be subscribe to for SignalR Set List updates.
    /// </summary>
    public event Action<setList>? OnSignalRSetList;

    /// <summary>
    /// An event that can be subscribed to for SignalR updates.
    /// </summary>
    public event Action<signalRUpdate>? OnSignalRUpdate;

    /// <summary>
    /// An event that can be subscribed to for SignalR user updates.
    /// </summary>
    public event Action<user>? OnSignalRUser;

    private void NotifyAudioPlaybackEnded() => OnAudioPlaybackEnded?.Invoke();

    //private void NotifyDataChanged() => OnChange?.Invoke();
    private void NotifyDataChanged()
    {
        if (OnChange != null) {
            OnChange.Invoke();
        }
    }

    private void NotifyDotNetHelperHandler() => OnDotNetHelperHandler?.Invoke(_DotNetHelperMessages);

    private void NotifySignalRMessage(message msg)
    {
        if (OnSignalRMessage != null) {
            OnSignalRMessage.Invoke(msg);
        }
    }

    private void NotifySignalRSetList(setList setlist)
    {
        if (OnSignalRSetList != null) {
            OnSignalRSetList.Invoke(setlist);
        }
    }

    private void NotifySignalRUpdate(signalRUpdate update)
    {
        if (OnSignalRUpdate != null) {
            OnSignalRUpdate.Invoke(update);
        }
    }

    private void NotifySignalRUser(user user)
    {
        if (OnSignalRUser != null) {
            OnSignalRUser.Invoke(user);
        }
    }
}