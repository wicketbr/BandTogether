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

        CacheStore.SetCacheItem("messages", GlobalSettings.CachedMessages);

        await Clients.All.SendAsync("message", msg);
    }

    public async Task SetList(setList setlist)
    {
        //var json = System.Text.Json.JsonSerializer.Serialize(setlist);

        // Only cache the item if this didn't come from VideoPsalm.
        if (Helpers.StringLower(setlist.fileName) != "videopsalm_api_call") {
            GlobalSettings.CachedSetList = setlist;
            CacheStore.SetCacheItem("setlist", setlist);
        }
            
        await Clients.All.SendAsync("setlist", setlist);
    }

    public async Task Update(signalRUpdate update)
    {
        await Clients.All.SendAsync("update", update);
    }
}