using System.Collections.Generic;
using System.Threading.Tasks;
using FluentSpotifyApi;
using Microsoft.AspNetCore.Mvc;

namespace Muse.Controllers.API
{
    public class DeviceApiController : BaseApiController
    {
        public DeviceApiController(IFluentSpotifyClient fluentSpotifyClient)
            : base(fluentSpotifyClient)
        {
        }

        public async Task<FluentSpotifyApi.Model.Device[]> Get()
        {
            var message = await fluentSpotifyClient.Me.Player.Devices.GetAsync();
            if (message == null)
            {
                return new FluentSpotifyApi.Model.Device[] {};
            }

            var devices = message.Items;

            // for (int i = 0; i < devices.Length; i++)
            // {
            //     await fluentSpotifyClient.Me.Player.Playback(devices[i].Id).;
            // }

            return devices;
        }
    }
}