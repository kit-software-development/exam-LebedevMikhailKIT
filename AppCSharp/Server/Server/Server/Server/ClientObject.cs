using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    //коды сообщений
    //Системные:
    //4 - Старт игры
    //5 - Конец игры
    //6 - Противник отключился
    //01 - установка номера игрока в 1
    //Игровые:
    //[столбец][столбец][0/1 - нолик\крести] - какую клетку открыть и как

    public class ClientObject
    {
        public string Id;//уникальный идентификатор клиента
        public NetworkStream Stream;//поток клиента
        public string Room; //номер комнаты

        private int room_number = 1;//номер комнаты

        TcpClient client;// объект клиента
        ServerObject server; // объект сервера

        //конструктор, который инициализирует клиента
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            Stream = client.GetStream();

            int counter = 0;//счетчик кол-ва соединения с одинаковой комнатой
                            //если соединений два, то комната заполнена, переходим к следующей
            
            //цикл установки комнаты
            while (true)
            {
                //проходим по всем комнатам и считаем сколько клиентов с этой комнатой
                //проверка комнат начинается с 1
                for (int i = 0; i < server.clients.Count; i++)
                {
                    if (server.clients[i].Room == room_number.ToString())
                        counter++;
                }

                //если соединений меньше, чем 2
                if (counter < 2)
                {
                    if (counter > 0)//если в комнате был один игрок, следовательно теперь их два, значит старт игры
                    {
                        Room = room_number.ToString();
                        Console.WriteLine("Подключился 2 игрок к комнате {0}", Room);
                        serverObject.GlobalRoomMessage("4", this.Room);//старт игры
                        Console.WriteLine("Старт игры");
                        break;
                    }
                    else//иначе, просто ожидаем подключение
                    {
                        Room = room_number.ToString();
                        Console.WriteLine("Подключился 1 игрок к комнате {0}", Room);
                        serverObject.RoomAnswerMessage("01", Room, Id);
                        break;
                    }
                }
                else// если подключения два, значит комната занята, увеличиваем номер комнаты
                {
                    room_number++;
                    counter = 0;
                }
            }
        }

        //процедура прослушивания потока на наличие данных в нем - получение сообщения от клиента
        public void Process()
        {
            try
            {
                // получаем имя пользователя
                string message = String.Empty;

                try
                {
                    // в бесконечном цикле получаем сообщения от клиента
                    while (true)
                    {
                        Stream = client.GetStream();
                        message = GetMessage();
                        Thread.Sleep(10); //нужна задержка, чтобы не склеивались несколько сообщений
                        if (message == "5")//если игра окончена, нужно уведомить и других игроков в комнате
                        {
                            Console.WriteLine("Игра в комнате {0} завершена", Room);
                            server.RoomOpponentMessage("5", Room, Id);
                        }
                        else//если нет, то просто пересылаем сообщение
                        {
                            server.RoomOpponentMessage(message, this.Room, this.Id);
                        }
                    }
                }
                catch//в случае каких-либо проблем, указываем игрокам в комнате, что у одного из игроков потеряено соединение, т.е. игра остановится
                {
                    server.RemoveConnection(this.Id);
                    server.RoomOpponentMessage("6", Room, Id);
                    Close();
                }
            }
            catch
            {
                server.RemoveConnection(this.Id);
                server.RoomOpponentMessage("6", Room, Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        public string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        public void Close()
        {
            server.GlobalRoomMessage("6", Room);
            Console.WriteLine("Игра в комнате {0} завершена", Room);
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}
