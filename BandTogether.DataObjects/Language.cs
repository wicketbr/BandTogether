﻿using System.Text.Json.Serialization;

namespace BandTogether;

public class Language
{
    public string? cultureCode { get; set; }
    public string? cultureName { get; set; }

    public string activeUsersOnly { get; set; } = "";
    public string about { get; set; } = "";
    public string add { get; set; } = "";
    public string addAudioFile { get; set; } = "";
    public string addBlankSlide { get; set; } = "";
    public string addClock { get; set; } = "";
    public string addCountdown { get; set; } = "";
    public string addCountdownInfo { get; set; } = "";
    public string addImage { get; set; } = "";
    public string addItem { get; set; } = "";
    public string addSetListItem { get; set; } = "";
    public string addSheetMusic { get; set; } = "";
    public string addSlideshow { get; set; } = "";
    public string addSlideshowInfo { get; set; } = "";
    public string addSong { get; set; } = "";
    public string addToSetList { get; set; } = "";
    public string addVideo { get; set; } = "";
    public string addYouTube { get; set; } = "";
    public string ago { get; set; } = "";
    public string alignment { get; set; } = "";
    public string alignmentBottom { get; set; } = "";
    public string alignmentCenter { get; set; } = "";
    public string alignmentLeft { get; set; } = "";
    public string alignmentMiddle { get; set; } = "";
    public string alignmentRight { get; set; } = "";
    public string alignmentTop { get; set; } = "";
    public string appName { get; set; } = "";
    public string artist { get; set; } = "";
    public string audioFile { get; set; } = "";
    public string auto { get; set; } = "";
    public string autoFollowOff { get; set; } = "";
    public string autoFollowOn { get; set; } = "";
    public string autoFontSize { get; set; } = "";
    public string back { get; set; } = "";
    public string background { get; set; } = "";
    public string backgroundType { get; set; } = "";
    public string backgroundTypeColor { get; set; } = "";
    public string backgroundTypeImage { get; set; } = "";
    public string backgroundTypeVideo { get; set; } = "";
    public string blankSlide { get; set; } = "";
    public string bold { get; set; } = "";
    public string cancel { get; set; } = "";
    public string capo { get; set; } = "";
    public string ccliNumber { get; set; } = "";
    public string chords { get; set; } = "";
    public string chordsInSong { get; set; } = "";
    public string churchMode { get; set; } = "";
    public string churchModeInfo { get; set; } = "";
    public string clear { get; set; } = "";
    public string clock { get; set; } = "";
    public string close { get; set; } = "";
    public string closeSetList { get; set; } = "";
    public string confirm { get; set; } = "";
    public string confirmDeleteSong { get; set; } = "";
    public string confirmDeleteSongInfo { get; set; } = "";
    public string confirmDeleteSongbook { get; set; } = "";
    public string confirmDeleteSongbookInfo { get; set; } = "";
    public string convertChordsOverTextToChordpro { get; set; } = "";
    public string convertChordsOverTextToChordproInfo { get; set; } = "";
    public string copySongToSongbook { get; set; } = "";
    public string copyToClipboard { get; set; } = "";
    public string copyToSongbook { get; set; } = "";
    public string copyright { get; set; } = "";
    public string countdown { get; set; } = "";
    public string countdownTo { get; set; } = "";
    public string countdownToTime { get; set; } = "";
    public string countdownType { get; set; } = "";
    public string currentItem { get; set; } = "";
    public string currentSetList { get; set; } = "";
    public string day { get; set; } = "";
    public string days { get; set; } = "";
    public string decreaseFont { get; set; } = "";
    public string defaults { get; set; } = "";
    public string delete { get; set; } = "";
    public string deleteSong { get; set; } = "";
    public string deletingWait { get; set; } = "";
    public string displayOnAllSlides { get; set; } = "";
    public string displayOnFirstAndLastSlides { get; set; } = "";
    public string displayOnFirstSlide { get; set; } = "";
    public string displayOnLastSlide { get; set; } = "";
    public string documentation { get; set; } = "";
    public string duplicate { get; set; } = "";
    public string duplicateUsername { get; set; } = "";
    public string edit { get; set; } = "";
    public string editMode { get; set; } = "";
    public string editSetListItem { get; set; } = "";
    public string editStyle { get; set; } = "";
    public string emptySetList { get; set; } = "";
    public string enabled { get; set; } = "";
    public string enableProjectionMode { get; set; } = "";
    public string enterFileName { get; set; } = "";
    public string enterFileNameInfo { get; set; } = "";
    public string error { get; set; } = "";
    public string errors { get; set; } = "";
    public string export { get; set; } = "";
    public string exportTitle { get; set; } = "";
    public string fontColor { get; set; } = "";
    public string fontFamily { get; set; } = "";
    public string fontLineHeight { get; set; } = "";
    public string fontOutline { get; set; } = "";
    public string fontOutlineColor { get; set; } = "";
    public string fonts { get; set; } = "";
    public string sontsInstructions { get; set; } = "";
    public string fontShadow { get; set; } = "";
    public string fontShadowBlur { get; set; } = "";
    public string fontShadowColor { get; set; } = "";
    public string fontShadowOffsetX { get; set; } = "";
    public string opacity { get; set; } = "";
    public string opacityStep { get; set; } = "";
    public string opacityStepInfo { get; set; } = "";
    public string fontShadowOffsetY { get; set; } = "";
    public string fontSize { get; set; } = "";
    public string fontStyle { get; set; } = "";
    public string footer { get; set; } = "";
    public string footerDisplay { get; set; } = "";
    public string footerFormat { get; set; } = "";
    public string header { get; set; } = "";
    public string headerDisplay { get; set; } = "";
    public string headerFormat { get; set; } = "";
    public string hide { get; set; } = "";
    public string hideChords { get; set; } = "";
    public string hideUserList { get; set; } = "";
    public string home { get; set; } = "";
    public string hour { get; set; } = "";
    public string hours { get; set; } = "";
    public string increaseFont { get; set; } = "";
    public string info { get; set; } = "";
    public string image { get; set; } = "";
    public string import { get; set; } = "";
    public string importPowerPoint { get; set; } = "";
    public string importPowerPointInfo { get; set; } = "";
    public string importSong { get; set; } = "";
    public string importSongInfo { get; set; } = "";
    public string italic { get; set; } = "";
    public string label { get; set; } = "";
    public string lang { get; set; } = "";
    public string layout { get; set; } = "";
    public string loading { get; set; } = "";
    public string loadingWait { get; set; } = "";
    public string loopItem { get; set; } = "";
    public string lyrics { get; set; } = "";
    public string messaging { get; set; } = "";
    public string messagingThemeBlack { get; set; } = "";
    public string messagingThemeBlue { get; set; } = "";
    public string messagingThemeGreen { get; set; } = "";
    public string messagingThemeRed { get; set; } = "";
    public string minute { get; set; } = "";
    public string minutes { get; set; } = "";
    public string moveSongToSongbook { get; set; } = "";
    public string moveToSongbook { get; set; } = "";
    public string muteInMainWindow { get; set; } = "";
    public string muteOnScreens { get; set; } = "";
    public string name { get; set; } = "";
    public string nashvilleNumbers { get; set; } = "";
    public string newSetList { get; set; } = "";
    public string newSongBook { get; set; } = "";
    public string newSongBookName { get; set; } = "";
    public string newSongBookNameInfo { get; set; } = "";
    public string newSongName { get; set; } = "";
    public string newUser { get; set; } = "";
    public string nns { get; set; } = "";
    public string noAudioFiles { get; set; } = "";
    public string noBackgroundImages { get; set; } = "";
    public string noBackgroundVideos { get; set; } = "";
    public string noCapo { get; set; } = "";
    public string none { get; set; } = "";
    public string noSavedSetlistFiles { get; set; } = "";
    public string ok { get; set; } = "OK";
    public string offset { get; set; } = "";
    public string open { get; set; } = "";
    public string openSetList { get; set; } = "";
    public string optionalFormatFields { get; set; } = "";
    public string pause { get; set; } = "";
    public string play { get; set; } = "";
    public string playingYouTubeVideo { get; set; } = "";
    public string playingVideo { get; set; } = "";
    public string present { get; set; } = "";
    public string preview { get; set; } = "";
    public string previewLarge { get; set; } = "";
    public string previewMedium { get; set; } = "";
    public string previewOff { get; set; } = "";
    public string previewSize { get; set; } = "";
    public string previewSmall { get; set; } = "";
    public string previewXLarge { get; set; } = "";
    public string previewXSmall { get; set; } = "";
    public string processingWait { get; set; } = "";
    public string projectionModeAspectRatio { get; set; } = "";
    public string rename { get; set; } = "";
    public string renameSongbook { get; set; } = "";
    public string renameSongbookInfo { get; set; } = "";
    public string resetFontSize { get; set; } = "";
    public string runningSince { get; set; } = "";
    public string sampleMessage { get; set; } = "";
    public string save { get; set; } = "";
    public string saved { get; set; } = "";
    public string savedAt { get; set; } = "";
    public string saveSetList { get; set; } = "";
    public string saveSetListAs { get; set; } = "";
    public string saveSong { get; set; } = "";
    public string saveSongbook { get; set; } = "";
    public string savingWait { get; set; } = "";
    public string screenMessage { get; set; } = "";
    public string screenMessages { get; set; } = "";
    public string screenView { get; set; } = "";
    public string second { get; set; } = "";
    public string seconds { get; set; } = "";
    public string select { get; set; } = "";
    public string selectSongBook { get; set; } = "";
    public string selectUser { get; set; } = "";
    public string send { get; set; } = "";
    public string settings { get; set; } = "";
    public string settingsProjection { get; set; } = "";
    public string sheetMusic { get; set; } = "";
    public string sheetMusicSelectPart { get; set; } = "";
    public string sheetMusicViewInfo { get; set; } = "";
    public string settingsGeneral { get; set; } = "";
    public string setList { get; set; } = "";
    public string showChords { get; set; } = "";
    public string showExport { get; set; } = "";
    public string showFontPreviews { get; set; } = "";
    public string showNonStandardKeys { get; set; } = "";
    public string showNonStandardKeysInfo { get; set; } = "";
    public string showOrHideChords { get; set; } = "";
    public string showPreviousLyrics { get; set; } = "";
    public string showUpcomingLyrics { get; set; } = "";
    public string showSeconds { get; set; } = "";
    public string showUserList { get; set; } = "";
    public string skip { get; set; } = "";
    public string slideshow { get; set; } = "";
    public string song { get; set; } = "";
    public string songBook { get; set; } = "";
    public string songBookExists { get; set; } = "";
    public string songBooks { get; set; } = "";
    public string songCapoPreferences { get; set; } = "";
    public string songFormat { get; set; } = "";
    public string songKey { get; set; } = "";
    public string songs { get; set; } = "";
    public string sortLabel { get; set; } = "";
    public string stop { get; set; } = "";
    public string stopAndClosePlayer { get; set; } = "";
    public string success { get; set; } = "";
    public string supportedFileTypes { get; set; } = "";
    public string switchUser { get; set; } = "";
    public string tabletMessage { get; set; } = "";
    public string tabletMessages { get; set; } = "";
    public string tabletView { get; set; } = "";
    public string tempo { get; set; } = "";
    public string theme { get; set; } = "";
    public string themeAuto { get; set; } = "";
    public string themeDark { get; set; } = "";
    public string themeLight { get; set; } = "";
    public string thumbnailColumns { get; set; } = "";
    public string time { get; set; } = "";
    public string timeSignature { get; set; } = "";
    public string title { get; set; } = "";
    public string toggleBlankScreen { get; set; } = "";
    public string toggleHideText { get; set; } = "";
    public string toggleEditMode { get; set; } = "";
    public string transitionSpeed { get; set; } = "";
    public string transparency { get; set; } = "";
    public string transpose { get; set; } = "";
    public string updateStageView { get; set; } = "";
    public string unknownError { get; set; } = "";
    public string unsaved { get; set; } = "";
    public string uploading { get; set; } = "";
    public string url { get; set; } = "";
    public string video { get; set; } = "";
    public string view { get; set; } = "";
    public string viewMode { get; set; } = "";
    public string volume { get; set; } = "";
    public string waitingForSetlist { get; set; } = "";
    public string yesDeleteSong { get; set; } = "";
    public string yesDeleteSongbook { get; set; } = "";
    public string youTube { get; set; } = "";
    public string youTubeVideo { get; set; } = "";
    public string youTubeVideoId { get; set; } = "";
    public string youTubeVideoIdInfo { get; set; } = "";
    public string zoom { get; set; } = "";
}