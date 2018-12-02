using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class ProfileImage
    {
        public byte[] Data { get; private set; }

        public void ImageData(byte[] data)
        {
            Data = data;
        }

        public static void ImageToDB(byte[] ImageData)
        {
            dataBase.connect.Open();
            dataBase.command = new MySql.Data.MySqlClient.MySqlCommand("UPDATE `P2_15_Pervoi`.`users` SET image  = @ImageData where login = '" + Login.loginStat + "'", dataBase.connect);
            dataBase.command.Parameters.Add("@ImageData", MySql.Data.MySqlClient.MySqlDbType.Blob, 1000000000);
            dataBase.command.Parameters["@ImageData"].Value = ImageData;


            dataBase.command.ExecuteNonQuery();
            dataBase.connect.Close();
        }

        
    }


}
