using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverChat
{
    public static class ChatController
    {
        // Максимально кол-во сообщений
        private const int MAXMESSAGE = 100;
        // Список всех сообщений
        public static List<message> Chat = new List<message>();

        /// <summary>
        /// Структура сообщения
        /// </summary>
        public struct message
        {
            public string userName;
            public string data;
            // Получение имени клиента и сообщения
            public message(string name, string msg)
            {
                userName = name;
                data = msg;
            }
        }

        /// <summary>
        /// Добавление сообщения в список сообщений и обновление чата у всех клиентов
        /// </summary>
        /// <param name="userName"></param>
        /// <param msg="msg"></param>
        public static void AddMessage(string userName,string msg)
        {
            try
            {
                // Если сообщение пустое ничего не делаем
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(msg)) return;
                int countMessages = Chat.Count;
                // Если кол-во сообщения в списке больше допустимого значения
                // удаляем самое первое сообщение
                if (countMessages > MAXMESSAGE) ClearChat();
                message newMessage = new message(userName, msg);
                Chat.Add(newMessage);
                Console.WriteLine("Новое сообщение от {0}.",userName);
                Server.UpdateAllChats();
            }
            catch (Exception exp) { Console.WriteLine("Ошибка добавления сообщения: {0}.", exp.Message); }
        }

        /// <summary>
        /// Функция  удаления первого сообщения из списка
        /// </summary>
        public static void ClearChat()
        {
            Chat.RemoveAt(0);
        }

        /// <summary>
        /// Функция получения всех сообщений из списка для отправки их клиенту
        /// </summary>
        /// <returns></returns>
        public static string GetChat()
        {
            try
            {
                // Создаём строку с коммандой #updatechat&
                string data = "#updatechat&";
                int countMessages = Chat.Count;
                // Если в списке 0 сообщений, возвращаем пустую строку
                if (countMessages <= 0) return string.Empty;
                // Иначе записываем в строку все сообщения из списка
                for (int i = 0; i < countMessages; i++)
                {
                    data += String.Format("{0}~{1}|", Chat[i].userName, Chat[i].data);
                }
                return data;
            }
            catch (Exception exp) { Console.WriteLine("Ошибка получения сообщений: {0}", exp.Message); return string.Empty; }
        }
    }
}
