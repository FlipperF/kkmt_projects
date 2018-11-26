using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class dataBase
    {
        public static MySqlConnection connect = new MySqlConnection(@"server=81.200.119.82;
                                                                port=3306;
                                                                user=student;
                                                                password=Student!@#;
                                                                database=P2_15_Pervoi");
        public static MySqlDataReader reader;
        public static MySqlCommand command;
    }
}
