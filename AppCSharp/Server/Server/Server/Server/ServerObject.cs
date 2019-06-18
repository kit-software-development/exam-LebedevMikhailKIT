using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения
        public List<string> rooms = new List<string>();

        public void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        public void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Я запустился! Слушаю соединения...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // отправка сообщения опоненту в комнату
        public void RoomOpponentMessage(string message, string room_name, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Room == room_name && clients[i].Id != id) // если id клиента не равно id отправляющего, а комнаты совпадают
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        //отправка ответного сообщения тому же клиенту, который запросил
        public void RoomAnswerMessage(string message, string room_name, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Room == room_name && clients[i].Id == id) // если id клиента не равно id отправляющего и комнаты совпадают
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        //отправка сообщения всем в комнате
        public void GlobalRoomMessage(string message, string room_name)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Room == room_name)//если комната совпадает с запрошенной
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        // отключение всех клиентов
        public void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
