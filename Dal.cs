using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace toto_monitoring
{
    public class Dal
    {
        private string _connectionString;
        private List<UserModel> tempList;
        public Dal(IConfiguration iconfiguration)
        {
            _connectionString = iconfiguration.GetConnectionString("Default");
            tempList = new();
        }
        public List<UserModel> GetList()
        {
            var listUserExpired = new List<UserModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT USRID, NM, STTDT, EXPDT, CREATEDT from T_USR WHERE EXPDT < GETDATE()", con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        listUserExpired.Add(new UserModel
                        {
                            UsrId = rdr[0].ToString(),
                            Name = rdr[1].ToString(),
                            StDate = rdr[2].ToString(),
                            ExpDate = rdr[3].ToString(),
                            CreateDate = rdr[4].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (tempList.Count < listUserExpired.Count)
            {
                tempList = listUserExpired;
                return tempList;
            }
            else{
                return null;
            }

        }
    }
}
