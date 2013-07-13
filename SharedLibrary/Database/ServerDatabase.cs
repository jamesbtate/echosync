using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace SharedLibrary.Database
{
    public static class ServerDatabase
    {
        static String connString = "server=128.82.4.29;uid=echosync;pwd=yjPLAjAqu13lVO4l1dw7G74D;database=echosync;";
        MySqlConnection connection;
        
        void Init()
        {
            try
            {
                connection = new MySqlConnection(connString);
                connection.Open();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
