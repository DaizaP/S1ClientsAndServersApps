using System.Net;
using System.Net.Sockets;

namespace S1ClientsAndServersApps.Server
{
    public class ChatServer
    {
        TcpListener tcpListener = new TcpListener(IPAddress.Any, 55555); // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения
        protected internal void RemoveConnection(string id)
        {
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null) clients.Remove(client);
            client?.Close();
        }
        // прослушивание входящих подключений
        protected internal async Task ListenAsync(CancellationToken token)
        {
            Task disconnectTask = new Task(() => Disconnect(token));
            disconnectTask.Start();
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");
                while (true)
                {
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                    ClientObject clientObject = new Server.ClientObject(tcpClient, this);
                    clients.Add(clientObject);
                    Task t = new Task(() => clientObject.ProcessAsync()); // добавляем фоновый поток
                    t.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect(token);
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal async Task BroadcastMessageAsync(string message, string id)
        {
            foreach (var client in clients)
            {
                if (client.Id != id)
                {
                    await client.Writer.WriteLineAsync(message); //передача данных
                    await client.Writer.FlushAsync();
                }
                if (client.Id == id)
                {
                    if (!client.isClosed)
                    {
                        await client.Writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.succesCode} Message sent"); //передача данных
                        await client.Writer.FlushAsync();
                    }
                }
            }
        }
        // отключение всех клиентов
        protected internal async Task Disconnect(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    foreach (var client in clients)
                    {
                        await client.Writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.shutdownServerCode} Server shutdown"); //передача данных
                        await client.Writer.FlushAsync();
                        client.Close(); //отключение клиента
                    }
                    tcpListener.Stop(); //остановка сервера
                    Console.WriteLine("Работа сервера остановлена");
                    break;
                }
            }
        }
    }
}
