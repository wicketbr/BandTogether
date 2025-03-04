# BandTogether

![screenshot](https://github.com/wicketbr/Misc/blob/main/images/BandTogether/MainScreen.jpg?raw=true)

## History

This program started as an application to store my music charts in one place
with the ability to send those charts to one or more tablets on stage with
each musician having control over their own settings and ability to add a
capo to play in a more instrument-friendly key, etc.
As I was working on the application I ended up adding most of the features found
in other projection applications (like ProPresenter, MediaShout, VideoPsalm, etc.)
As an open source software contributor I have decided to release this for others
to use as they see fit.

## About

BandTogether is a C# Blazor WebAssembly application designed for musicians 
to keep all their music in one place and to be able to play from tablets.
This can be used by bands, churches, organizations that want to run media for a meeting,
schools, etc. The major features are:

- Keep all your charts in the chordpro format with an easy-to-use song editor
  (a sample Public Domain song is included).
- Charts can be stored in chord notation or using the Nashville Numbering System.
- Charts can be imported from chordpro files and the editor has the option to 
  convert lyrics with chords above the words (as found on many websites online) to
  the chordpro format.
- Put together set lists.
- Use a tablet on stage to view your charts in real time with options to personalize your view,
  including adding capos for guitar players and removing chords for vocalists.
- Songs can be transposed as needed in the set list on a per-item basis that doesn't
  affect the song stored in the song book.
- Set lists can contains Songs, Audio Files, Images, Countdown Timers (both in seconds and
  to countdown to a specific time), Sheet Music, Slideshows, Videos, and YouTube Videos.
- A projection view can be used to show your media items and songs on the screen,
  including fully customizing the background, text, etc., for how songs are viewed.
- The screen view app can be loaded into any browser window, so it can either be on the same computer
  used to run the set list and dragged to a second window that goes to your screens, or you can simply
  use other systems connected to TVs pointing to the correct web page. Most browsers support
  a full-screen mode, usually by pressing F11 to toggle between widowed and full-screen mode.
- The entire app is a self-contained .NET 9 Blazor app that uses SignalR to keep all connected
  tablets and screens in sync with your set list.
- Send messages from the person controlling the set list to the screens or tablets.
- The app supports multiple languages and can easily be customized by editing/copying
  the en-US.json language file.

## Getting Started with BandTogether

### Install - Windows

Download and unzip the files to a location on a computer that will be used to run the app.
Open the BandTogether.exe file, which launches a self-contained web server. The URL to the app
will be the local IP address of the computer and port 5000 (eg: http://192.168.0.10:5000).
You can then open the /Tablet and /Screen views on tablets on stage and displays.
In most cases the app will be able to detect the IP address and will display
that information in the console with a link that can be opened by pressing
CTRL+click.

NOTE: The embeded YouTube player may not work when using the local IP address if you
are using a non-routable IP address (eg: 192.168.x.x). In those cases you should use the
hostname of the computer running the app instead of the IP address.
If the computer is not discoverable by name on your network you may need to add
the name and ip address to the hosts file on the system. That process varies by operating system.

You can point to an alternate location for your data by modifying the shortcut used to
start the application executable and adding -f PATH to the end of the command line.
For example, if you have all your data in C:\MyBandData you would add -f C:\MyBandData.

### Install - Mac

The Mac version of the software requires that you already have the .NET 9 Framework installed,
which can be downloaded from Microsoft at:

https://dotnet.microsoft.com/en-us/download/dotnet/9.0

The Mac version of the software is distributed as a zip file that contains the application files,
which should be placed in a folder name "BandTogether" in the Applications folder.

To start the application open a console window and issue the following command:

dotnet /Applications/BandTogether/BandTogether.dll

Or, if you wish to point to a different location on Mac for the "Data" folder:

dotnet /Applications/BandTogether/BandTogether.dll -f /path/to/data/folder

Mac OS may initially block the application from running due to security settings.
When this happens you need to open the System Settings, go to Privacy & Security, scroll to the bottom
and click Open Anyway.

### Install - Other Platforms

The application can run on any platform that supports .NET 9.
In addition to the Windows and Mac binaries included in the releases on GitHub,
the application can be compiled for:
win-x86, win-arm, win-arm64, osx-x64, linux-x64, linux-arm, and linux-arm64.
You can do this using Visual Studio, Visual Studio Code, or the .NET CLI.

For any OS that supports .NET, as long as you have the .NET SDK installed you
can simply download the source code for this project, open a command prompt,
change to the directory where the BandTogether.csproj file is located
(/BandTogether/BandTogether/) and issue the following command:

dotnet run

That will compile the application and run it locally. The first compile will
take a minute to run, but subsequent runs will be faster as the application
will have already been compiled.

### Install - General

When the application is first started it will create a Data folder in the same location
as the application (unless you have specified another location using the -f command line parameter).
In the Data folder folders will be created for storing items for
Audio, Backgrounds, Images, Languages, SetLists, SheetMusic, Slideshows, SongBooks, Users, and Videos.
All application data is stored in JSON files, which are plain text files that can
easily be edited with any text editor.

If the folders don't exist and are being created then
sample media items will be created in the Backgrounds, Images, SheetMusic, and Slideshows folders
and a sample set list will be created and will include samples of most set list item types.

If you wish to force the URL that the app will run as you can modify the LaunchUrl
setting in the appsettings.json file. If this is left empty the app will attempt to run
on http://0.0.0.0:5000 for windows and http://localhost:5000 for Mac.

### Language

All application language is stored in the Languages folder. The default language is en-US.json.
If you wish to make a translation for another language, simply make a copy of the en-US.json file
and name it according to the language code you wish to use (eg: fr-FR.json for French).
Or, you can update any text in the en-US.json file if you wish to change the wording.

## The Interface

The interface is divided into the main sections: 

- The Menu Bar at the Top of the Application
- The Set List and Song Books items in a resizable split-pane on the left.
- The main Item View area on the right, with an optional Preview area shown at the bottom.

### The Menu Bar

![toolbar](https://github.com/wicketbr/Misc/blob/main/images/BandTogether/Toolbar.jpg?raw=true)

The first menu item is the Edit Mode toggle.
Depending on whether or not you have Edit Mode toggled on in the top menu you will see
different options in the interface. For example, the context menus that appear when
right-clicking on the set list, a song book, or songs in a song book will differ
depending on whether or not you are in Edit Mode. Also, in Edit Mode you can edit
the song in the main view area, as shown below.

![screenshot](https://github.com/wicketbr/Misc/blob/main/images/BandTogether/MainScreen-EditMode.jpg?raw=true)

Next to the Edit Mode menu option is an option to show or hide the messages area.
When shown, the messages interface will appear at the top of the item view area just
below the toolbar menu. This feature can be used to send messages to the main projection
screen view or to any connected tablets using the tablet view.

If an audio file is 
playing from the set list the audio player controls will appear just above the messaging area.
When an audio file is playing you can move on to other items in the set list and the
audio will continue to play. Once the audio has completed the audio interface will disappear.
You can close the audio player during playback by clicking the X in the upper right corner.

The next two icons relate to blanking the screen and hiding text. The Toggle Blank Screen menu
option will blank the screen to black. The Toggle Hide Text menu option will hide all text
on a slide but leave the background in place. This works for Clocks, Countdowns, and Songs.

The Screen View menu option will open a new browser window with the screen view interface.
This can be dragged onto a second monitor and then be run in full screen (usually by pressing F11.)
You can open multiple screen views on different monitors, or even on completely different computers.
For example, if your main projection computer running the software is connected to a projector
or to multiple TVs mirroring a second screen, you could show the screen view on those screens from the
projection computer. If you have TVs in another area that have a computer connected that can
access the URL of the software, those TVs could also show the screen view interface.

The Tablet View menu option is used for opening the interface meant for musicians on stage.
This interface allows each musician to choose their own settings for light or dark mode,
font size for the song chart, whether to show or hide chords, whether to view in the original
key or to view in the Nashville Numbering System, and to add a capo to the song chart.
When Auto-Follow is on then the chart will automatically scroll to stay in line with the
current element of the song that is selected in the system controlling the set list.

The Settings menu item brings up the interface for configuring the software. Here you can
select whether or not to show non-standard keys for songs and transposing, enable or disable
the projection features, and enable a Church Mode that will show CCLI information for use
when showing songs in a church environment. You can also use the Projection tab (if that
feature is enabled) to configure the style for fonts used for lyrics, headers, footers, as
well as backgrounds used for song presentation on screen. The Messaging tab allows
you to configure the font settings for both the screen and tablet messages.
The Fonts tab allows you to select additional fonts installed on your system that you
want to use in the software. By default, all standard web-safe fonts are included, as well
as a few Google fonts stored as woff2 files in the wwwroot/fonts folder. You can add
additional woff2 fonts to this folder and they will be loaded during the application startup
and will then be available for use in the various style interfaces.

The Show/Hide Chords menu option toggles chords on and off in the non-Edit Mode view when
viewing a song in the main item view area.

The next menu option turns the preview window on and off and allows you to select the 
size for the preview window.

The user menu option allows you to switch to a different user account, and to add additional users.

The Theme menu option lets you switch between auto, light, or dark mode. In auto mode the
software will use the system setting for light or dark mode.

### Set List

You can use the up and down arrows to move between items. Pressing Enter with an item
selected will present that item and switch the focus to the view area.
Pressing Delete will delete the currently-selected set list item.
You can use the standard cut, copy, and paste shortcuts with set list items.
Pressing the Tab key will switch the focus to the view area.

You can drag songs from a song book to the set list area, or you can right-click on a song
and select the Add to Set List option. Items in the set list can also be rearranged by dragging.
By default, the previously-opened set list will be opened again the next time you open the software.

Use the Plus icon above the set list to add a new item to the set list. Supported item types are:

- Audio Files
- A Clock
- A Countdown Timer (both in seconds or to a specific time)
- Images
- Sheet Music
- Slideshows
- Videos
- YouTube Videos

If you have PowerPoint installed on the computer running the software you can choose the
option when creating a new Slideshow to create the slideshow images from a PowerPoint presentation.
Due to the nature of how this software works, PowerPoint slideshows cannot be played directly,
as the software is not doing any sort of video rendering on the screens. Instead, only images
can be used at this point.

### View Area

You can use the arrow keys or the page up/down keys to move between items in the view area.
When in edit mode and editing a song you can switch parts by double-clicking
on the part in the editor, or by pressing the CTRL key or OPTION key plus arrow up and down.

## Tech Notes

In my testing I have found that the background videos do not perform well in all browsers.
The videos are played using the native HTML5 video player, so the performance is dependent
on the browser and the computer running the browser.
A better alternative to looping background videos is to use animated looping GIFs instead
(a sample animated gif is included with the sample media.)

Unlike other projection applications that use graphics engines to render everything on the
screen, BandTogether uses native HTML elements to render the screen view and draw text.
This means that videos are played on the main computer running the show, and the
screen view is kept in sync by checking the current video time on the main presentation
computer and messages are sent to all clients using SignalR. This means that the
video on the main presentation computer and the video on the other screens may be
slightly out of sync by a few milliseconds, but in most cases this is not noticeable.
The desire was to make this application completely web-based without requiring any
advanced graphics rendering or video processing to mirror that video on other screens.

For the Sheet Music option the files must be in the PDF format, and your tablets need
to have the ability to show a PDF file. Each song should have its own folder inside the
SheetMusic folder, with individual folders for each instrument part. When users select
a part for a song that will be remembered as part of their user preferences.

## Contact

Contact Brad Wickett at [brad@wickett.net](mailto:brad@wickett.net) for assistance.

If you think of something that has been missed that should be added to the software
please let me know, or feel free to submit a pull request if you are a developer.

You can also download or clone the source code for this project from GitHub at 
[https://github.com/wicketbr/BandTogether](https://github.com/wicketbr/BandTogether).