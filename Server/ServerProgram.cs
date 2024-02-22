
namespace S1ClientsAndServersApps.Server
{
    internal class ServerProgram
    {
        static async Task Main(string[] args)
        {
            ChatServer server = new ChatServer();
            Thread t = new Thread(() =>
            {
                Console.WriteLine("Q — Выключение сервера");
                while (true)
                {
                    string message = Console.ReadLine();
                    if (message == "Q" || message == "q" || message == "Й" || message == "й")
                    {
                        server.Disconnect();
                        break;
                    }
                }
            });
            t.Start();
            server.ListenAsync();

        }
    }
}
