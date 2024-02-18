using System.Net.Sockets;

namespace S1ClientsAndServersApps.Client
{
    internal class ClassClient
    {
        public async static Task Run()
        {
            string host = "127.0.0.1";
            int port = 55555;
            using TcpClient client = new TcpClient();
            Console.Write("Введите свое имя: ");
            string? userName = Console.ReadLine();
            Console.WriteLine($"Добро пожаловать, {userName}");
            StreamReader? Reader = null;
            StreamWriter? Writer = null;

            try
            {
                client.Connect(host, port); //подключение клиента
                Reader = new StreamReader(client.GetStream());
                Writer = new StreamWriter(client.GetStream());
                if (Writer is null || Reader is null) return;
                // запускаем новый поток для получения данных
                Task.Run(() => ReceiveMessageAsync(Reader, Writer));
                // запускаем ввод сообщений
                await SendMessageAsync(Writer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Writer?.Close();
            Reader?.Close();

            // отправка сообщений
            async Task SendMessageAsync(StreamWriter writer)
            {
                // сначала отправляем имя
                await writer.WriteLineAsync(userName);
                await writer.FlushAsync();
                Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
                while (true)
                {
                    try
                    {
                        if (!client.Connected)
                        {
                            Console.WriteLine("Нет соединения с сервером");
                            break;
                        }
                        string? message = Console.ReadLine();
                        if (message == "Exit")
                        {
                            Console.WriteLine("Вы вышли из чата");
                            await writer.WriteLineAsync(message);
                            await writer.FlushAsync();
                            break;
                        }
                        await writer.WriteLineAsync(message);
                        await writer.FlushAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(" | Failed");
                    }
                    finally
                    {
                        await writer.FlushAsync();
                    }
                }
            }
            // получение сообщений
            async Task ReceiveMessageAsync(StreamReader reader, StreamWriter writer)
            {
                while (true)
                {
                    try
                    {
                        // считываем ответ в виде строки
                        string? message = await reader.ReadLineAsync();
                        // если пустой ответ, ничего не выводим на консоль
                        if (string.IsNullOrEmpty(message))
                        {
                            continue;
                        }
                        if (message.Contains(Code.succesCode))
                        {
                            Console.WriteLine(" | OK");
                            continue;
                        }
                        if (message.Contains(Code.shutdownServerCode))
                        {
                            Console.WriteLine(" | Сервер был выключен");
                            break;
                        }
                        else
                        {
                            Print(message);//вывод сообщения
                        }
                        // Отправляем подтверждение отправки
                        await writer.WriteLineAsync($"{DateTime.Now}. Code:{Code.succesCode} The message received by user {userName}"); //передача данных
                        await writer.FlushAsync();
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            // чтобы полученное сообщение не накладывалось на ввод нового сообщения
            void Print(string message)
            {
                if (OperatingSystem.IsWindows())    // если ОС Windows
                {
                    var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
                    int left = position.Left;   // смещение в символах относительно левого края
                    int top = position.Top;     // смещение в строках относительно верха
                                                // копируем ранее введенные символы в строке на следующую строку
                    Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
                    // устанавливаем курсор в начало текущей строки
                    Console.SetCursorPosition(0, top);
                    // в текущей строке выводит полученное сообщение
                    Console.WriteLine(message);
                    // переносим курсор на следующую строку
                    // и пользователь продолжает ввод уже на следующей строке
                    Console.SetCursorPosition(left, top + 1);
                }
                else Console.WriteLine(message);
            }
        }
    }
}
