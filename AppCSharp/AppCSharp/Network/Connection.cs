using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppCSharp
{
    public class Connection
    {
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private TcpClient client;//объект клиента
        private NetworkStream stream;//объект потока этого клиента
        private string message;//сообщение
        public bool isConnect = false;//флаг состояния, который указывает было ли установлено соединение

        GameForm gameForm;//объект формы

        //конструктор, который инициализирует соединение
        public Connection(GameForm obj)
        {
            gameForm = obj;
            StartConnection();
        }

        //установка соединения
        public void StartConnection()
        {
            if (!isConnect)
            {
                client = new TcpClient();
                try
                {
                    client.Connect(host, port); //подключение клиента
                    stream = client.GetStream(); // получаем поток

                    // запускаем новый поток для получения данных
                    isConnect = true;
                    gameForm.SetConnectionStatus("Активно");
                    gameForm.SetGameStatus("Ожидание игрока");
                    Task.Run((Action)ReceiveMessage);//в отдельном потоке запускаем процедуру получения сообщения от сервера
                }
                catch//в случае проблем - соединение не установлено
                {
                    gameForm.SetConnectionStatus("Отсутствует");
                    gameForm.SetGameStatus("-");
                    gameForm.SetNumber("-");
                }
            }
        }

        //процедура отправки сообщения
        public void SendMessage(string msg)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);//преобразовываем сообщение в массив байтов
            stream.Write(data, 0, data.Length);//записываем эти байты  в поток
        }

        //процедура получения сообщения от сервера
        private void ReceiveMessage()
        {
            StringBuilder builder = new StringBuilder();

            //в бесконечном цикле смотрим, если в потоке данные, если есть, то достаем их и обрабатываем
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; //буфер для получаемых данных
                    builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    message = builder.ToString();

                    gameForm.game.CheckMessage(message);//отправляем полученные данные на обработку
                    Thread.Sleep(10);
                }
                catch
                {
                    Disconnect();//в случае проблем отключение
                }
            }
        }

        //процедура отключения соединения
        public void Disconnect()
        {
            if (isConnect)
            {
                stream?.Close(); //отключение потока
                client?.Close(); //отключение клиента
                isConnect = false;
            }
        }
    }
}

