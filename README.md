# BandTogether

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
  to countdown to a specific time), Slideshows, Videos, and YouTube Videos.
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
the name and ip address to the hosts file on the system. That process various by operating system.

You can point to an alertnate location for your data by modifying the shortcut used to
start the application executable and adding -f PATH to the end of the command line.
For example, if you have all your data in C:\MyBandData you would add -f C:\MyBandData.

### Install - Mac

For Mac security you should place the files in a BandTogether folder inside the
Applications folder. Start the application by issuing the following command from
a console in the application folder. 

dotnet BandTogether.dll

Or, if you wish to point to a different location on Mac for the "Data" folder:

dotnet BandTogether.dll -f /path/to/data/folder

### Install - Other Platforms

The application can run on any platform that supports .NET 9.
In addition to the Windows and Mac binaries included in the releases on GitHub,
the application can be compiled for:
win-x86, win-arm, win-arm64, osx-x64, linux-x64, linux-arm, and linux-arm64.
You can do this using Visual Studio, Visual Studio Code, or the .NET CLI.

### Install - General

When the application is first started it will create a Data folder in the same location
as the application (unless you have specified another location using the -f command line parameter).
In the Data folder folders will be created for storing items for
Audio, Backgrounds, Images, Languages, SetLists, Slideshows, SongBooks, Users, and Videos.
All application data is stored in JSON files, which are plain text files that can
easily be edited with any text editor.

If the folders don't exist and are being created then
sample media items will be created in the Backgrounds, Images, and Slideshows folders
and a sample set list will be created and will include samples of most set list item types.

If you wish to force the URL that the app will run as you can modify the LaunchUrl
setting in the appsettings.json file. If this is left empty the app will attempt to run
on http://0.0.0.0:5000 for windows and http://localhost:5000 for Mac.

### Language

All application language is stored in the Languages folder. The default language is en-US.json.
If you wish to make a translation for another language, simply make a copy of the en-US.json file
and name it according to the language code you wish to use (eg: fr-FR.json for French).
Or, you can update any text in the en-US.json file if you wish to change the wording.

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

## Contact

Contact Brad Wickett at [brad@wickett.net](mailto:brad@wickett.net) for assistance.

If you think of something that has been missed that should be added to the software
please let me know, or feel free to submit a pull request if you are a developer.

You can also download or clone the source code for this project from GitHub at 
[https://github.com/wicketbr/BandTogether](https://github.com/wicketbr/BandTogether).