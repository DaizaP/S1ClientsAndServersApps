using System.Net.Sockets;

namespace S1ClientsAndServersApps.Server
{
    internal class ClientObject
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }
        protected internal bool isClosed { get; private set; } = false;

        TcpClient client;
        ChatServer server; // объект сервера

        public ClientObject(TcpClient tcpClient, ChatServer serverObject)
        {
            client = tcpClient;
            server = serverObject;
            // получаем NetworkStream для взаимодействия с сервером
            var stream = client.GetStream();
            // создаем StreamReader для чтения данных
            Reader = new StreamReader(stream);
            // создаем StreamWriter для отправки данных
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                // получаем имя пользователя
                string? userName = await Reader.ReadLineAsync();
                string? message = $"{userName} вошел в чат";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessageAsync(message, Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        message = await Reader.ReadLineAsync();
                        if (message == null) continue;
                        if (message.Contains(Code.succesCode))
                        {
                            Console.WriteLine(message);
                            continue;
                        }
                        message = $"{userName}: {message}";
                        if(message == $"{userName}: Exit")
                        {
                            throw new Exception("User exit");
                        }
                        server.BroadcastMessageAsync(message, Id);
                    }
                    catch (Exception ex)
                    {
                        isClosed = true;
                        message = $"{userName} покинул чат";
                        Console.WriteLine(message);
                        server.BroadcastMessageAsync(message, Id);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(Id);
            }
        }
        // закрытие подключения
        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            client.Close();
        }
    }
}
