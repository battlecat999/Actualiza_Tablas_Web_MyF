using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

using System.Windows.Forms;

using Actualiza_Web.Comun;

using MySql.Data.MySqlClient;
using System.Data.OleDb;

namespace Actualiza_Web
{
    public class Entidades
    {

        public static string CadenaConexion_Access
        {

            get
            {
                string Cadena;
                string en_Cadena;

                encrypasocv.Encryption e = new encrypasocv.Encryption();
                Cadena = ConfigurationManager.ConnectionStrings["Conexion_Access"].ConnectionString;

                en_Cadena = e.Decrypt(Cadena);
                return en_Cadena;
            }
        }

        public static string CadenaConexion_MySql
        {

            get
            {
                string Cadena;
                string en_Cadena;

                encrypasocv.Encryption e = new encrypasocv.Encryption();
                Cadena = ConfigurationManager.ConnectionStrings["Conexion_MySql"].ConnectionString;

                en_Cadena = e.Decrypt(Cadena);
                return en_Cadena;
            }
        }

        public static string Provider_Access
        {
            get { return ConfigurationManager.ConnectionStrings["Conexion_Access"].ProviderName; }
        }

        public static string CadenaConexionParametros
        {
            get { return ConfigurationManager.ConnectionStrings["Reportes"].ConnectionString; }
        }

        public static DbProviderFactory DbPF
        {
            get
            {
                return DbProviderFactories.GetFactory(Provider_Access);
            }
        }

        public static OleDbDataReader GetDataReader_Access(string strConsulta)
        {

            OleDbConnection conConexion_Access;   // create connection
            OleDbCommand cmdComando_Consulta_Access;  // create command
            OleDbDataReader drData_Reader;  //Dataread for read data from database

            string strConnection_Access;
 
            strConnection_Access = CadenaConexion_Access;
 


            conConexion_Access = new OleDbConnection(strConnection_Access);

            cmdComando_Consulta_Access = new OleDbCommand(strConsulta, conConexion_Access);

            conConexion_Access.Open();

            drData_Reader = cmdComando_Consulta_Access.ExecuteReader();

            //conConexion_Access.Close();

            return drData_Reader;

        }


        public static int ExecuteNonQuery_Access(string strStored_Procedure, OleDbCommand cmdComando_Consulta_Access)
        {

            // OleDbConnection conConexion_Access;   // create connection
            //OleDbCommand cmdComando_Consulta_Access;  // create command

            //string strConnection_Access = CadenaConexion;
            //string strConnection_Access = "";

            //if (TipoConexion == 1)
            //{
            //    strConnection_Access = CadenaConexionLocal;
            //}
            //else
            //{
            //    strConnection_Access = CadenaConexionRemota;
            //}

            //conConexion_Access = new OleDbConnection(strConnection_Access);

            //cmdComando_Consulta_Access = new OleDbCommand(strStored_Procedure, conConexion_Access);

            cmdComando_Consulta_Access.CommandText = strStored_Procedure;

            cmdComando_Consulta_Access.Parameters.Clear();

            //foreach (OleDbParameter Param in Parametros)
            //    cmdComando_Consulta_Access.Parameters.Add(Param);

            //conConexion_Access.Open();
            int intResultado = cmdComando_Consulta_Access.ExecuteNonQuery();

            //conConexion_Access.Close();

            return intResultado;
        }

        public static int ExecuteNonQuery_Access(string strStored_Procedure,CommandType Tipo_Query)
        {

            OleDbConnection conConexion_Access;   // create connection
            OleDbCommand cmdComando_Consulta_Access;  // create command

            //string strConnection_Access = CadenaConexion;
            string strConnection_Access = "";

                strConnection_Access = CadenaConexion_Access;
           

            conConexion_Access = new OleDbConnection(strConnection_Access);

            cmdComando_Consulta_Access = new OleDbCommand(strStored_Procedure, conConexion_Access);

            cmdComando_Consulta_Access.CommandType = Tipo_Query;// CommandType.StoredProcedure;

            cmdComando_Consulta_Access.Parameters.Clear();

            //foreach (OleDbParameter Param in Parametros)
            //    cmdComando_Consulta_Access.Parameters.Add(Param);

            conConexion_Access.Open();
            int intResultado = cmdComando_Consulta_Access.ExecuteNonQuery();

            conConexion_Access.Close();

            return intResultado;
        }

        public static int ExecuteNonQuery_Access(string strStored_Procedure, List<OleDbParameter> Parametros)
        {

            OleDbConnection conConexion_Access;   // create connection
            OleDbCommand cmdComando_Consulta_Access;  // create command

            //string strConnection_Access = CadenaConexion;
            string strConnection_Access = "";


                strConnection_Access = CadenaConexion_Access;


            conConexion_Access = new OleDbConnection(strConnection_Access);

            cmdComando_Consulta_Access = new OleDbCommand(strStored_Procedure, conConexion_Access);
            cmdComando_Consulta_Access.CommandTimeout = 600;
            cmdComando_Consulta_Access.CommandType = CommandType.StoredProcedure;




            foreach (OleDbParameter Param in Parametros)
                cmdComando_Consulta_Access.Parameters.Add(Param);

            conConexion_Access.Open();
            int intResultado = cmdComando_Consulta_Access.ExecuteNonQuery();

            cmdComando_Consulta_Access.Parameters.Clear();
            conConexion_Access.Close();

            return intResultado;
        }



        public static DataTable GetDataTable_Access(string strConsulta,CommandType TipoComando)
        {

            OleDbConnection conConexion_Access;   // create connection
            OleDbCommand cmdComando_Consulta_Access;  // create command
                                                      //OleDbDataReader drData_Reader;  //Dataread for read data from database

            DataTable dtData_Table = new DataTable();

            string strConnection_Access = "";

            strConnection_Access = CadenaConexion_Access;

            


            conConexion_Access = new OleDbConnection(strConnection_Access);

            cmdComando_Consulta_Access = new OleDbCommand(strConsulta, conConexion_Access);
            cmdComando_Consulta_Access.CommandType = TipoComando;

            conConexion_Access.Open();


            OleDbDataAdapter dtData_Adapter = new OleDbDataAdapter(cmdComando_Consulta_Access);

            dtData_Adapter.Fill(dtData_Table);
            //  }

            conConexion_Access.Close();

            return dtData_Table;

        }

        public static DataTable GetDataTable_Access(String strConsulta, List<OleDbParameter> Parametros)
        {

            OleDbConnection conConexion_Access;   // create connection
            OleDbCommand cmdComando_Consulta_Access;  // create command
                                                      //OleDbDataReader drData_Reader;  //Dataread for read data from database

            DataTable dtData_Table = new DataTable();

            string strConnection_Access = "";

            
                     strConnection_Access = CadenaConexion_Access;

            conConexion_Access = new OleDbConnection(strConnection_Access);

            cmdComando_Consulta_Access = new OleDbCommand(strConsulta, conConexion_Access);
            cmdComando_Consulta_Access.CommandTimeout = 600;
            cmdComando_Consulta_Access.CommandType = CommandType.StoredProcedure;

            foreach (OleDbParameter Param in Parametros)
                cmdComando_Consulta_Access.Parameters.Add(Param);

            conConexion_Access.Open();

            OleDbDataAdapter dtData_Adapter = new OleDbDataAdapter(cmdComando_Consulta_Access);
            dtData_Adapter.Fill(dtData_Table);

            cmdComando_Consulta_Access.Parameters.Clear();
            conConexion_Access.Close();

            return dtData_Table;

        }

        //public static int EjecutaNonQuery(string strStoredProcedures, List<DbParameter> Parametros)
        //{
        //    int ID = 0;

        //    General.gblnGrabacion_OK = true;

        //    try
        //    {
        //        using (DbConnection Con = DbPF.CreateConnection())
        //        {
        //            Con.ConnectionString = CadenaConexion_SQL_Server;
        //            using (DbCommand cmd = DbPF.CreateCommand())
        //            {
        //                cmd.Connection = Con;
        //                cmd.CommandText = strStoredProcedures;
        //                cmd.CommandType = CommandType.StoredProcedure;

        //                foreach (DbParameter Param in Parametros)
        //                    cmd.Parameters.Add(Param);

        //                Con.Open();
        //                ID = cmd.ExecuteNonQuery();

        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        General.gblnGrabacion_OK = false;
        //        //throw;
        //    }

        //    return ID;

        //}

        //public static int EjecutaNonQuery(string strSQL)
        //{
        //    int ID = 0;

        //    General.gblnGrabacion_OK = true;

        //    try
        //    {
        //        using (DbConnection Con = DbPF.CreateConnection())
        //        {
        //            Con.ConnectionString = CadenaConexion_SQL_Server;
        //            using (DbCommand cmd = DbPF.CreateCommand())
        //            {
        //                cmd.Connection = Con;
        //                cmd.CommandText = strSQL;
        //                cmd.CommandType = CommandType.Text;

        //                //foreach (DbParameter Param in Parametros)
        //                //    cmd.Parameters.Add(Param);

        //                Con.Open();
        //                ID = cmd.ExecuteNonQuery();

        //            }
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        General.gblnGrabacion_OK = false;
        //        //throw;
        //    }

        //    return ID;

        //}

        public static int EjecutaNonQuery_MySql(string strStoredProcedures, List<DbParameter> Parametros)
        {
            int ID = 0;

            General.gblnGrabacion_OK = true;

            try
            {
                using (DbConnection Con = DbPF.CreateConnection())
                {
                    Con.ConnectionString = CadenaConexion_MySql;
                    using (DbCommand cmd = DbPF.CreateCommand())
                    {
                        cmd.Connection = Con;
                        cmd.CommandText = strStoredProcedures;
                        cmd.CommandType = CommandType.StoredProcedure;

                        foreach (DbParameter Param in Parametros)
                            cmd.Parameters.Add(Param);

                        Con.Open();
                        ID = cmd.ExecuteNonQuery();

                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                General.gblnGrabacion_OK = false;
                //throw;
            }

            return ID;

        }

        public static int EjecutaNonQuery_MySql(string strSql)
        {
            int ID = 0;

            General.gblnGrabacion_OK = true;

            try
            {
                MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);
                conn.Open();

                MySqlCommand mySqlcmd = new MySqlCommand(strSql, conn);

                MySqlDataReader MyReader;

                MyReader = mySqlcmd.ExecuteReader();

                //MyReader.Close();
                conn.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sr. Usuario "  + strSql, MessageBoxButtons.OK, MessageBoxIcon.Error);
                General.gblnGrabacion_OK = false;
                //throw;
            }

            return ID;

        }

        public static DataSet GetDataSet_MySql(String strConsulta)
        {

            string MyConnectionString = string.Empty;

            MyConnectionString = Entidades.CadenaConexion_MySql;

            MySqlConnection connection = new MySqlConnection(MyConnectionString);

            //MySqlCommand cmd = new MySqlCommand(strConsulta, connection);
            //cmd.CommandText = strConsulta;

            MySqlDataAdapter mySql_DataAdapter = new MySqlDataAdapter(strConsulta, connection);
            mySql_DataAdapter.SelectCommand.CommandType = CommandType.Text;

            DataTable mySql_DataTable = new DataTable();
            mySql_DataAdapter.Fill(mySql_DataTable);

            DataSet mySql_DataSet = new DataSet();
            mySql_DataSet.Tables.Add(mySql_DataTable);

            return mySql_DataSet;

        }




        public static DataSet GetDataSet(String strSqlConsulta)
        {

            DbDataAdapter dbDataAdapter = new SqlDataAdapter();
            SqlCommand Commando = new SqlCommand(strSqlConsulta);
            DataTable dataTable = new DataTable();

            using (DbConnection con = DbPF.CreateConnection())
            {
                con.ConnectionString = CadenaConexion_Access;

                using (DbCommand commando = DbPF.CreateCommand())
                {
                    commando.Connection = con;
                    con.Open();

                    DataSet dataSet = new DataSet();    
                    dbDataAdapter.SelectCommand = commando;
                    dbDataAdapter.SelectCommand.CommandText = strSqlConsulta;
                    dbDataAdapter.SelectCommand.Connection = con;

                    dbDataAdapter.Fill(dataSet);
                    
                    dataTable.EndLoadData();
                    dataSet.EnforceConstraints = false;
                    dataSet.Tables.Add(dataTable);

                    return dataSet;

                }
            }
        }

        //public static DataTable GetDataTable(String strConsulta)
        //{
        //    DbDataAdapter dbDataAdapter = new SqlDataAdapter();
        //    DataTable dataTable = new DataTable();

        //    using (DbConnection con = DbPF.CreateConnection())
        //    {
        //        con.ConnectionString = CadenaConexion_SQL_Server;

        //        using (DbCommand commando = DbPF.CreateCommand())
        //        {
        //            commando.Connection = con;

        //            con.Open();

        //            DataSet dataSet = new DataSet();

        //            dbDataAdapter.SelectCommand = commando;
        //            dbDataAdapter.SelectCommand.CommandType = CommandType.Text;
        //            dbDataAdapter.SelectCommand.CommandTimeout = 0;
        //            dbDataAdapter.SelectCommand.CommandText = strConsulta;
        //            dbDataAdapter.SelectCommand.Connection = con;

        //            dbDataAdapter.Fill(dataSet);

        //            dataTable.EndLoadData();
        //            dataSet.EnforceConstraints = false;
        //            dataSet.Tables.Add(dataTable);

        //            return dataSet.Tables[0]; 

        //        }
        //    }
        //}

        public static DataTable GetDataTable(String strConsulta, string strConnectionString)
        {
            DbDataAdapter dbDataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();

            using (DbConnection con = DbPF.CreateConnection())
            {
                //con.ConnectionString = CadenaConexion;
                con.ConnectionString = strConnectionString;

                using (DbCommand commando = DbPF.CreateCommand())
                {
                    commando.Connection = con;
                    commando.CommandTimeout = 0;

                    con.Open();

                    DataSet dataSet = new DataSet();

                    dbDataAdapter.SelectCommand = commando;
                    dbDataAdapter.SelectCommand.CommandType = CommandType.Text;
                    dbDataAdapter.SelectCommand.CommandText = strConsulta;
                    dbDataAdapter.SelectCommand.Connection = con;

                    dbDataAdapter.Fill(dataSet);

                    dataTable.EndLoadData();
                    dataSet.EnforceConstraints = false;
                    dataSet.Tables.Add(dataTable);

                    return dataSet.Tables[0];

                }
            }
        }


        //public static DataTable GetDataTable_New(String strConsulta)
        //{
        //    DbDataAdapter dbDataAdapter = new SqlDataAdapter();
        //    DataTable dataTable = new DataTable();

        //    //string strStoredProcedure = "up_carga_OTs";

        //    using (DbConnection con = DbPF.CreateConnection())
        //    {
        //        //con.ConnectionString = CadenaConexion_AutoPack;
        //        con.ConnectionString = CadenaConexion_SQL_Server;

        //        using (DbCommand commando = DbPF.CreateCommand())
        //        {
        //            commando.Connection = con;
        //            //commando.CommandText = strStoredProcedure;
        //            //commando.CommandType = CommandType.StoredProcedure;

        //            con.Open();

        //            DataSet dataSet = new DataSet();

        //            dbDataAdapter.SelectCommand = commando;
        //            dbDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
        //            dbDataAdapter.SelectCommand.CommandText = strConsulta;
        //            dbDataAdapter.SelectCommand.Connection = con;

        //            dbDataAdapter.Fill(dataSet);

        //            dataTable.EndLoadData();
        //            dataSet.EnforceConstraints = false;
        //            dataSet.Tables.Add(dataTable);

        //            return dataSet.Tables[0];

        //        }
        //    }
        //}


    }
}