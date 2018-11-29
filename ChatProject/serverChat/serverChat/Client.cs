using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace serverChat
{
    public class Client
    {
        private string userName;
        private Socket handler;
        private Thread userThread;

        /// <summary>
        /// Функция создания клиента
        /// </summary>
        /// <param Socket="socket"></param>
        public Client(Socket socket)
        {
            // Присваиваем переменной полученный сокет
            handler = socket;
            // Создаём и запускаем фоновый поток клиента
            userThread = new Thread(listner);
            userThread.IsBackground = true;
            userThread.Start();
        }

        /// <summary>
        /// Функия получающая имя пользователя
        /// </summary>
        public string UserName
        {
            get { return userName; }
        }

        /// <summary>
        /// Функция "слушающая" клиентов
        /// </summary>
        private void listner()
        {
            while (true)
            {
                try
                {
                    // Получение сообщения, его декодировка и
                    // отправка комманды
                    byte[] buffer = new byte[1048576];
                    int bytesRec = handler.Receive(buffer);
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                    handleCommand(data);
                }
                catch { Server.EndClient(this); return; }
            }
        }

        /// <summary>
        /// Функия отключения пользователя
        /// </summary>
        public void End()
        {
            try
            {
                // Закрываем получение сообщений
                handler.Close();
                try
                {
                    // Закрываем поток
                    userThread.Abort();
                }
                catch { } // г
            }
            catch (Exception exp) { Console.WriteLine("Ошибка отключения: {0}.",exp.Message); }
        }

        /// <summary>
        /// Функция получения комманд для сервера
        /// </summary>
        /// <param command="data"></param>
        private void handleCommand(string data)
        {
            // Если в комманде содержится строка "#setname"
            // значит клиент ввёл своё имя и отправил его на сервер.
            string command = "#setname";
            int count = 0;
            if (data.Contains(command))
            {
                for(int i = 0; i < 8; i++)
                {
                    if(data[i] == command[i])
                    {
                        count++;
                    }
                    if (count == 8)
                    {
                        userName = data.Split('&')[1];
                        // Обновляем чат (добавляем все сообщения хранящиеся на сервере)
                        UpdateChat();
                    }
                }
                count = 0;
                return;
            }
            // Если в комманде содержится строка "#newmsg"
            // значит это сообщение от пользователя.
            // Пример строки - data = "#newmsg&Всем привет!"
            if (data.Contains("#newmsg"))
            {
                string message = data.Split('&')[1];
                // Добавляем сообщение в список.
                ChatController.AddMessage(userName,message);
                return;
            }
        }

        /// <summary>
        /// Функция обновления чата(добавление клиенту всех сообщений хранящихся на сервере)
        /// </summary>
        public void UpdateChat()
        {
            Send(ChatController.GetChat());
        }

        /// <summary>
        /// Функция отправки списка сообщений клиенту
        /// </summary>
        /// <param name="command"></param>
        public void Send(string command)
        {
            try
            {
                int bytesSent = handler.Send(Encoding.UTF8.GetBytes(command));
                if (bytesSent > 0) Console.WriteLine("OK");
            }
            catch (Exception exp) { Console.WriteLine("ERROR: {0}.", exp.Message); Server.EndClient(this); }
        }
    }
}
