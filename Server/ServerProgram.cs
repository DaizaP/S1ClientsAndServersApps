namespace S1ClientsAndServersApps.Server
{
    internal class ServerProgram
    {
        static async Task Main(string[] args)
        {
            ChatServer server = new ChatServer();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Q — Выключение сервера");
                while (true)
                {
                    string message = Console.ReadLine();
                    if (message == "Q" || message == "q" || message == "Й" || message == "й")
                    {
                        cancelTokenSource.Cancel();
                        break;
                    }
                }
            });

            t.Start();
            server.ListenAsync(token);
            t.Join();
            cancelTokenSource.Dispose();
            Console.ReadKey();
        }
    }
}
