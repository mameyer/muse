using System.Threading.Tasks;

namespace Muse.Controllers.API
{
    public class DeviceApiController : BaseApiController
    {
        public DeviceApiController()
            : base()
        {
        }

        public async Task<object[]> Get()
        {
            // var message = await fluentSpotifyClient.Me.Player.Devices.GetAsync();
            // if (message == null)
            // {
            //     return new FluentSpotifyApi.Model.Device[] {};
            // }

            // var devices = message.Items;

            // // for (int i = 0; i < devices.Length; i++)
            // // {
            // //     await fluentSpotifyClient.Me.Player.Playback(devices[i].Id).;
            // // }

            // return devices;

            return null;
        }
    }
}