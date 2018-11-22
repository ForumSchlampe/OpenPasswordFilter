using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using OPFService.Properties;

namespace OPFService
{
    class PwnedLocalDB
    {
        MySqlConnection mysqlConn;
        SqlConnection mssqlConn;

        public PwnedLocalDB()
        {
            if (Settings.Default.PwnedLocalMySQLDB)
            {
                mysqlConn = new MySqlConnection(Settings.Default.PwnedLocalMySQLDBConnString);
                try
                {
                    mysqlConn.Open();
                }
                catch (MySqlException mex)
                {
                    LogFunc.writeLog(mex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    LogFunc.writeLog(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            if (Settings.Default.PwnedLocalMSSQLDB)
            {
                mssqlConn = new SqlConnection(Settings.Default.PwnedLocalMSSQLDBConnString);
                try
                {
                    mssqlConn.Open();
                }
                catch (SqlException sex)
                {
                    LogFunc.writeLog(sex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    LogFunc.writeLog(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                }
            }
        }

        public bool checkPassword(string Password)
        {
            if (Settings.Default.PwnedLocalMySQLDB)
            {
                string SQL = "SELECT * FROM Passwordlist WHERE Passwords='" + Password + "'";//TODO MySQL-Statement, erst mal ohne Hash
                try
                {
                    MySqlCommand cmd = new MySqlCommand(SQL, mysqlConn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmd.Dispose();
                            return true;
                        }
                        else
                        {
                            cmd.Dispose();
                        }
                    }
                }
                catch (MySqlException mex)
                {
                    LogFunc.writeLog(mex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return true;
                }
                catch (Exception ex)
                {
                    LogFunc.writeLog(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return true;
                }
            }

            if (Settings.Default.PwnedLocalMSSQLDB)
            {
                string SQL = "SELECT * FROM Passwordlist WHERE Passwords='" + Password + "'";//TODO MSSQL-Statement, erst mal ohne Hash
                try
                {
                    SqlCommand cmd = new SqlCommand(SQL, mssqlConn);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cmd.Dispose();
                            return true;
                        }
                        else
                        {
                            cmd.Dispose();
                        }
                    }

                }
                catch (SqlException sex)
                {
                    LogFunc.writeLog(sex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return true;
                }
                catch (Exception ex)
                {
                    LogFunc.writeLog(ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return true;
                }
            }

            return false;
        }

        ~PwnedLocalDB()
        {
            if (mysqlConn != null)
            {
                mysqlConn.Close();
            }
            if (mssqlConn != null)
            {
                mssqlConn.Close();
            }
        }
    }
}
