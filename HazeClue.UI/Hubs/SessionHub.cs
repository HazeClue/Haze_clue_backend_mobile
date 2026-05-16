using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HazeClue.UI.Hubs
{
    [Authorize]
    public class SessionHub : Hub
    {
        // Clients can call this method to stream their live concentration data to the server
        public async Task StreamConcentrationData(string sessionId, int concentrationValue)
        {
            // For now, we can just log it or broadcast it to an admin dashboard
            // If the user has multiple devices, we could send it to their other devices:
            // await Clients.User(Context.UserIdentifier).SendAsync("ReceiveConcentrationData", sessionId, concentrationValue);

            // Here we could also persist real-time data points to a fast datastore like Redis or Time-Series DB if needed.
            // For now, acting as an active stream endpoint is sufficient to establish the architecture.
            
            // Send back acknowledgment or trigger alerts if concentration drops
            if (concentrationValue < 30)
            {
                await Clients.Caller.SendAsync("ReceiveAlert", "Your concentration is dropping, take a deep breath!");
            }
        }
    }
}
