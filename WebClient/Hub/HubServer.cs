namespace WebClient.Hub
{
    using Microsoft.AspNetCore.SignalR;

    public class HubServer: Hub
    {
        public void HasNewData()
        {
            this.Clients.All.SendAsync("Reload");
        }
    }
}
