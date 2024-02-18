using System.Net;
using System.Net.Sockets;

namespace S1ClientsAndServersApps.Server
{
    internal class ServerProgram
    {
        static async Task Main(string[] args)
        {
            ChatServer server = new ChatServer();
            await server.ListenAsync();
        }
    }
}
