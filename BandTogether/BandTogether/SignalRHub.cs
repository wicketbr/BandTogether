using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace BandTogether;

public class signalRhub : Hub
{
    public async Task Message(message msg)
    {
        if (msg.target == "screen") {
            GlobalSettings.CachedMessages.screenMessage = msg;
        } else if (msg.target == "tablet") {
            GlobalSettings.CachedMessages.tabletMessage = msg;
        }

        await Clients.All.SendAsync("message", msg);
    }

    public async Task SetList(setList setlist)
    {
        GlobalSettings.CachedSetList = setlist;

        await Clients.All.SendAsync("setlist", setlist);
    }

    public async Task Update(signalRUpdate update)
    {
        await Clients.All.SendAsync("update", update);
    }
}