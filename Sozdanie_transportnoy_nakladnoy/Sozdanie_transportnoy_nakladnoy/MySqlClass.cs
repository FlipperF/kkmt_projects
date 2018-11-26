using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace Sozdanie_transportnoy_nakladnoy
{
    class MySqlClass
    {
        private static string sConnString = (@"server=den1.mysql4.gear.host;
                                                                port=3306;
                                                                user=gruzoperevozki;
                                                                password=*87654321;
                                                                database=gruzoperevozki"); //Строка подключения к серверу
        private static MySqlConnection mySQLConn = new MySqlConnection(sConnString); // Объект подключения к БД
        /// <summary>
        /// Метод открытия соединения с БД
        /// </summary>
        private static void openConnect()
        {
            try
            {
                mySQLConn.Open(); //Открытие соединения
            }
            catch
            {
                MessageBox.Show("Соединение с сервером отсутствует"); // Информационное окно в случае возникновения ошибки
            }
        }
        /// <summary>
        /// Формирования объекта команды БД для работы с БД через хранимую процедуру
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <returns>Объект команды БД</returns>
        private static MySqlCommand getCommandProcedure(string nameProcedure)
        {
            MySqlCommand cmd = new MySqlCommand
            {
                Connection = mySQLConn, // Установления ассоциации для команды соединения
                CommandType = CommandType.StoredProcedure, // Обаначения команды как тип хранимая процедура
                CommandText = nameProcedure //Передача наименования хранимой процедуры
            }; //Создание новго объекта в виде команды MySQL
            return cmd;
        }
        /// <summary>
        /// Формирования объекта команды БД для работы с БД через хранимую процедуру с входными параметрами
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <param name="arrParam">Массив с наименованием параметров</param>
        /// <param name="arrParamData">Массив с данными</param>
        /// <returns>Объект команды БД</returns>
        private static MySqlCommand getCommandProcedureInParam(string nameProcedure, string[] arrParam, string[] arrParamData)
        {
            MySqlCommand cmd = getCommandProcedure(nameProcedure);
            for (int i = 0; i < arrParam.Length; i++)
            {
                cmd.Parameters.Add(new MySqlParameter(arrParam[i], MySqlDbType.VarChar, 1000)); //Добавления наименования параметров в объект команды
                cmd.Parameters[arrParam[i]].Value = arrParamData[i];  // Добавление значения в параметр
            }
            return cmd;
        }
        /// <summary>
        /// Получения таблицы с помощью запроса
        /// </summary>
        /// <param name="Command">Строка запроса</param>
        /// <returns>Вывод объекта в формате DateView (работа с таблицами)</returns>
        public static DataView getTableQuery(string Command)
        {
            openConnect();
            DataTable DT = new DataTable(); // Создания специального объекта для работы с таблицами
            (new MySqlDataAdapter(new MySqlCommand(Command, mySQLConn))).Fill(DT); // Получения таблицы путе создания новой команы в новом адаптаре и транслирования её в объект DT
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
            return DT.AsDataView();
        }
        /// <summary>
        /// Выполнения запроса в которых не требуется вывода таблицы (delete, insert, update)
        /// </summary>
        /// <param name="Command">Строка запроса</param>
        public static void executeQuery(string Command)
        {
            openConnect();
            (new MySqlCommand(Command, mySQLConn)).ExecuteNonQuery(); // Выполенние команды через создания новой команды 
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
        }
        /// <summary>
        /// Метод выполения процедуры в которой есть только входные параметры
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <param name="arrParam">Массив с наименованием параметров</param>
        /// <param name="arrParamData">Массив с данными</param>
        public static void executeProcedureInItem(string nameProcedure, string[] arrParam, string[] arrParamData)
        {
            openConnect();
            getCommandProcedureInParam(nameProcedure, arrParam, arrParamData).ExecuteNonQuery(); //Выполнение команды
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
        }
        /// <summary>
        /// Метод получения выходных параметров через процедру и входные параметры
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <param name="arrParam">Массив с наименованием параметров</param>
        /// <param name="arrParamData">Массив с данными</param>
        /// <returns>Выходные параметры</returns>
        public static string[] executeProcedureInOutItem(string nameProcedure, string[] arrParam, string[] arrParamData)
        {
            openConnect();
            MySqlCommand cmd = getCommandProcedure(nameProcedure);
            string[] arrParamOut = new string[arrParam.Length - arrParamData.Length]; //Массив выходных параметров
            for (int i = 0; i < arrParam.Length; i++)
            {
                cmd.Parameters.Add(new MySqlParameter(arrParam[i], MySqlDbType.VarChar, 1000)); //Добавления всех параметров в объект команды
                if (i < arrParamData.Length)
                {
                    cmd.Parameters[arrParam[i]].Value = arrParamData[i]; //Занесения значений параметров в объект команды
                }
                else
                {
                    cmd.Parameters[arrParam[i]].Direction = ParameterDirection.Output; // Обозначения параметра как выходной
                }
            }
            cmd.ExecuteNonQuery(); //Выполнение команды
            for (int i = 0; i < arrParamOut.Length; i++)
            {
                arrParamOut[i] = Convert.ToString(cmd.Parameters[arrParam[arrParamData.Length + i]].Value); //Присвоение массиву выходных параметров полученных данных из процедуры
            }
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
            return arrParamOut;
        }
        /// <summary>
        /// Метод получения таблицы без входных параметров
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <returns>Вывод объекта в формате DateView (работа с таблицами)</returns>
        public static DataView getTableProcedure(string nameProcedure)
        {
            openConnect();
            DataTable DT = new DataTable(); // Создания специального объекта для работы с таблицами
            (new MySqlDataAdapter(getCommandProcedure(nameProcedure))).Fill(DT); // Получения таблицы путе создания нового адаптера с командой cmd и транслирования её в объект DT
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
            return DT.AsDataView();
        }
        /// <summary>
        /// Метод получения таблицы через хранимую процедуру с входными параметрами
        /// </summary>
        /// <param name="nameProcedure">Наименование процедуры</param>
        /// <param name="arrParam">Массив с наименованием параметров</param>
        /// <param name="arrParamData">Массив с данными</param>
        /// <returns>Вывод объекта в формате DateView (работа с таблицами)</returns>
        public static DataView getTableProcedureInItem(string nameProcedure, string[] arrParam, string[] arrParamData)
        {
            openConnect();
            DataTable DT = new DataTable(); // Создания специального объекта для работы с таблицами
            (new MySqlDataAdapter(getCommandProcedureInParam(nameProcedure, arrParam, arrParamData))).Fill(DT); // Получения таблицы путе создания нового адаптера с командой cmd и транслирования её в объект DT
            mySQLConn.Close(); // Закрытие соединения (Перемещается в пул)
            return DT.AsDataView();
        }
    }
}
