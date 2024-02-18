
namespace S1ClientsAndServersApps.Server
{
    internal class ServerProgram
    {
        static async Task Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.ListenAsync();
            Console.WriteLine("Q — Выключение сервера");
            string message = Console.ReadLine();
            if (message == "Q" || message == "q" || message == "Й" || message == "й") 
            {
                server.Disconnect();
            }

        }
    }
}
