using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace serverChat
{
    public static class Server
    {
        // Список всех клиентов
        public static List<Client> Clients = new List<Client>();

        /// <summary>
        /// Функция добавления нового клиента
        /// </summary>
        /// <param socket="handle"></param>
        public static void NewClient(Socket handle)
        {
            try
            {
                // Добавляем нового клиента через класс Client
                Client newClient = new Client(handle);
                Clients.Add(newClient);
                Console.WriteLine("Новый пользователь подключён: {0}", handle.RemoteEndPoint);
            }
            catch (Exception exp) { Console.WriteLine("Ошибка подключения пользователя: {0}.",exp.Message); }
        }

        /// <summary>
        /// Функция отключения клиента
        /// </summary>
        /// <param Client="client"></param>
        public static void EndClient(Client client)
        {
            try
            {
                // Отключение клиентов от сервера и удаление его из списка
                client.End();
                Clients.Remove(client);
                Console.WriteLine("Пользователь {0} отключился.", client.UserName);
            }
            catch (Exception exp) { Console.WriteLine("Ошибка отключения пользователя: {0}.",exp.Message); }
        }

        /// <summary>
        /// Функция обновлениия чата у всех клиентов
        /// </summary>
        public static void UpdateAllChats()
        {
            try
            {
                int countUsers = Clients.Count;
                // Проходим по всем клиентам в списке и вызываем функцию
                // UpdateChat из класса Client
                for (int i = 0; i < countUsers; i++)
                {
                    Clients[i].UpdateChat();
                }
            }
            catch (Exception exp) { Console.WriteLine("Ошибка обновления всех чатов: {0}.",exp.Message); }
        }
        
    }
}
