# BandTogether

A C# Blazor web application designed for musicians to keep all their music in one place and to be able to play from tablets, which has grown into much more.

- Keep all your charts in the chordpro format with an easy-to-use song editor (a sample Public Domain song is included).
- Put together set lists.
- Use a tablet on stage to view your charts in real time with options to personalize your view, including adding capos for guitar players and removing chords for vocalists.
- Charts can be stored in chord notation or using the Nashville Numbering System.
- Songs can be transposed as needed.
- Set lists can contains Songs, Audio Files, Images, Countdown Timers, Slideshows, Videos, and YouTube Videos.
- A projection view can be used to show your set information on the screen,
  including fully customizing the background, text, etc. for how songs are viewed.
- The screen view app can be loaded into any browser window, so it can either be on the same computer
  used to run the set list and dragged to a second window that goes to your screens, or you can simple
  use other systems connected to TVs pointing to the correct page.
- The entire app is a self-contained .NET 9 Blazor app that uses SignalR to keep all connected
  tablets and screens in sync with your set list.
- The app supports multiple languages and can easily be customized by editing/copy the en-US.json language file.

## Getting Started with BandTogether

### Install

Download and unzip the files to a location on a computer that will be used to run the app.
Open the BandTogether.exe file, which launches a self-contained web server. The URL to the app
will be the local IP address of the computer and port 5000 (eg: http://192.168.0.10:5000).
You can then open the /Tablet and /Screen views on tablets on stage and displays.
In most cases the app will be able to detect the IP address and will display
that information in the console with a link that can be opened by pressing
CTRL+click.

## Contact

Contact Brad Wickett at [brad@wickett.net](mailto:brad@wickett.net) for assistance.

You can also download the source code for this project from GitHub at 
[https://github.com/wicketbr/BandTogether](https://github.com/wicketbr/BandTogether).
