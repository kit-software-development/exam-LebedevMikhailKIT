using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace Server
{
    namespace ChatServer
    {
        class Program
        {
            static ServerObject server; // сервер
            static Thread listenThread; // потока для прослушивания
            static void Main(string[] args)
            {
                try
                {
                    server = new ServerObject();//инициализация сервера
                    listenThread = new Thread(new ThreadStart(server.Listen));//запуск в отдельном потоке прослушивателя соединения
                    listenThread.Start(); //старт потока
                }
                catch (Exception ex)
                {
                    server.Disconnect();//в случае проблем останавливаем соединения
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}