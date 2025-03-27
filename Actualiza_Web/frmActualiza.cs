using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MySql.Data.MySqlClient;

using System.Data.Common;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;
using FTPLibrary;
using System.Windows.Forms.VisualStyles;

namespace Actualiza_Web
{
    public partial class frmActualiza : Form
    {

        int intResto = 0;
        int Count = 0;
        public frmActualiza()
        {
            InitializeComponent();

            Inicia();
            //BloqueActualizacionAccess(0);
        }

        private void Inicia()
        {

            lblActualizando.Visible = false;

            //System.Timers.Timer aTimer = new System.Timers.Timer();
            //aTimer.Elapsed += new System.Timers.ElapsedEventHandler(Actualiza);

            //aTimer.Interval = 10000;
            //aTimer.Enabled = true;

            int intIntervalo_Actualizacion = 0;

            intIntervalo_Actualizacion = int.Parse(ConfigurationManager.AppSettings["Intervalo_Actualizacion"]);

            timer.Interval = 1000;

            intResto = (timer.Interval * intIntervalo_Actualizacion)/1000;

            this.lblRestan.Text = "Restan " + intResto.ToString() + " segundos...";
            this.Refresh();

            timer.Start();

            this.cmdIniciar.Enabled = false;
            this.Refresh();

            int intActivo = Activo_Actualizacion_WEB();
            if (intActivo==0)
            {
                Actualiza(1);//la primera vez que inicia.
            }

            

        }
        private Boolean BloqueActualizacionAccess(int intNuevoValor,int intViejoValor)
        {
            //bloque de 0 a 1
            //tengo que validar si el otro sistema esta leyendo
            //si bloqueo esta en 0, cambio a 1 y luego a 0, 
            //pero si el otro sistema lee, debe pasar de 0 a 1 y luego a 0
            //solo actualizo si bloqueo estan en 0
            //Microsoft.Office.Interop.Access.Application appAccess = new Microsoft.Office.Interop.Access.Application();
            DataTable dt;
            List<OleDbParameter> Parametros = new List<OleDbParameter>();
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];
            int Resultado;
            //Cargao Nombre COnsula
            string strSQL_Access = "Transferecia_ActualizoTestigo";
            Parametros.Add(new OleDbParameter("intNew", OleDbType.Integer) { Value = intNuevoValor });
            Parametros.Add(new OleDbParameter("intOld", OleDbType.Integer ) { Value = intViejoValor });


            Entidades.ExecuteNonQuery_Access(strSQL_Access, Parametros);

            strSQL_Access = "Transferencia_Testigo";

            dt=Entidades.GetDataTable_Access(strSQL_Access,CommandType.StoredProcedure);
            if (dt.Rows.Count > 0)
            {
                Resultado = Convert.ToInt32(dt.Rows[0][0].ToString());
                if (Resultado == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        private int Activo_Actualizacion_WEB()
        {
            int intActivo;
            intActivo = 0;
            
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            DataSet dsProveedores = Entidades.GetDataSet_MySql("call SP_Configuracion(" + Codigo_Empresa_MySql + ")");

            foreach (DataTable dtbDatos in dsProveedores.Tables)
            {
                foreach (DataRow row in dtbDatos.Rows)
                {
                    intActivo = Convert.ToInt32(row["Actualizacion_Web_Activa"].ToString());
                    return intActivo;
                }
            }
            return intActivo;
        }

        private void Actualiza(int intPrimera)
        {
            try
            {


            //Guarda la hora justo antes del proceso a cronometrar
            DateTime tiempo1 = DateTime.Now;

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Visible = true;
            this.Refresh();

            string Proceso_Generales = ConfigurationManager.AppSettings["Corre_Proceso_Generales"];
            string Proceso_IVA = ConfigurationManager.AppSettings["Corre_Proceso_IVA"];
            timer.Enabled = false;
            Count++;
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];
            if (BloqueActualizacionAccess(1,0)==false)
            {
                return;
            }

            Corre_Proceso_Contenedores();
            if (Proceso_Generales == "1")
            {
                Procesa_Clientes();

                Procesa_Proveedores_Desde_MySql();

                Procesa_Remitos();

                Procesa_Remitos_Especiales();

                Procesa_Anticipos();
                
                Procesa_Informe_Combustible();

                Procesa_Proveedores_desde_Access();

                //Procesa_Proveedores_PDF();

                    
            }
            if (Count == 4 || intPrimera==1)
            {

                Count = 0;
                if (Proceso_IVA == "1")
                {
                    Procesa_IVA_Ventas();

                    Procesa_IVA_Compras();

                    
                }
            }

            BloqueActualizacionAccess(0, 1);
            timer.Enabled = true;

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Visible = false;
            this.Refresh();

            //Guarda la hora al finalizar
            DateTime tiempo2 = DateTime.Now;

            //Crea un "intervalo temporal"
            TimeSpan total = new TimeSpan(tiempo2.Ticks - tiempo1.Ticks);
            }
            catch (Exception ex)
            {

                MessageBox.Show ("Error: " + ex.Message.ToString());
                return ;
            }
        }
        private void Procesa_Informe_Combustible()
        {
            List<OleDbParameter> Parametros = new List<OleDbParameter>();
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            DateTime fechatemp;
            DateTime datFecha_Desde;
            DateTime datFecha_Hasta;


            fechatemp = DateTime.Today;
            datFecha_Desde = new DateTime(fechatemp.Year, fechatemp.Month, 1).AddDays(-5);

            if ((fechatemp.Month + 1)==13)
            {
                datFecha_Hasta = new DateTime(fechatemp.Year+1, 1, 1).AddDays(-1);
            }
            else
            {
                datFecha_Hasta = new DateTime(fechatemp.Year, fechatemp.Month+1, 1).AddDays(-1);
            }
            

            //Cargao Nombre COnsula
            string strSQL_Access = "MySQL_Uso_Combustible";
            //Armo los parametros
            // dtGrilla = ocls.GetDataTable_Access(SQL, String.Empty, Parametros)
            Parametros.Add(new OleDbParameter("intEmpresa", OleDbType.Integer) { Value = Codigo_Empresa_MySql });
            //Cargo el DataTable
            DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access, Parametros);

            string q;
            string intEmpresa;
            string datFecha;
            string decValor;
            string decLitros_Entregados;
            string decImporte_Pagados;
            string decEquivalencia_Litros;
            string decDiferencia_Litros;

            q = string.Empty;
            foreach (DataRow drow in dtbDatos.Rows)
            {
                intEmpresa = Codigo_Empresa_MySql;
                datFecha = Convert.ToDateTime(drow["Fecha"]).ToString("yyyy-MM-dd");
                decValor = drow["Valor"].ToString().Replace(",", ".");
                decLitros_Entregados = drow["Litros_Entregados"].ToString().Replace(",", ".");
                decImporte_Pagados = drow["Importe_Pagados"].ToString().Replace(",", ".");
                decEquivalencia_Litros = drow["Equivalencia_Litros"].ToString().Replace(",", ".");
                decDiferencia_Litros = drow["Diferencia_Litros"].ToString().Replace(",", ".");

                q = string.Concat(q, "CALL SP_Combustible_Carga(", intEmpresa, ",'", datFecha, "',", decValor, ",", decLitros_Entregados, ",", decImporte_Pagados, ",", decEquivalencia_Litros, ",", decDiferencia_Litros, ");", Environment.NewLine);
                Entidades.EjecutaNonQuery_MySql(q);
            }
            
        }
        private void Procesa_IVA_Ventas()
        {
            List<OleDbParameter> Parametros = new List<OleDbParameter>();
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];
            int intCantidadDias= Convert.ToInt32(ConfigurationManager.AppSettings["Cant_Dias_Atras"]);

            DateTime fechatemp;
            DateTime datFecha_Desde;
            DateTime datFecha_Hasta;


            fechatemp = DateTime.Today;
            datFecha_Desde = new DateTime(fechatemp.Year, fechatemp.Month, 1).AddDays(intCantidadDias);
            if ((fechatemp.Month + 1) == 13)
            {
                datFecha_Hasta = new DateTime(fechatemp.Year + 1, 1, 1).AddDays(-1);
            }
            else
            {
                datFecha_Hasta = new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1);
            }

            //Cargao Nombre COnsula
            string strSQL_Access = "MySQL_SubDiarioIVA_Ventas";
            //Armo los parametros
            // dtGrilla = ocls.GetDataTable_Access(SQL, String.Empty, Parametros)
            Parametros.Add(new OleDbParameter("intEmpresa", OleDbType.Integer) { Value = Codigo_Empresa_MySql});
            Parametros.Add(new OleDbParameter("datFechaDesde", OleDbType.Date) { Value = datFecha_Desde });
            Parametros.Add(new OleDbParameter("datFechaHasta", OleDbType.Date) { Value = datFecha_Hasta });
            //Cargo el DataTable
            DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access, Parametros);
            string q;
            string intEmpresa;
            string strTipoComp;
            string strTipoFactura;
            string strNroFactura;
            string intCliente;
            string strEmpresa;
            string strCliente;
            string strCUIT;
            string datFechaEmision;
            string decNeto;
            string decIva;
            string decExento;
            string decTotal;

            q = string.Empty;
            foreach (DataRow drow in dtbDatos.Rows)
            {
                
                //qDet = string.Concat(qDet, "CALL ", strSP_Items, "(", tEmpresa ,",",tItem ,",",tCont,",",tGen ,",",tEstado, ");",Environment.NewLine);
                intEmpresa = drow["Empresa"].ToString();
                strTipoComp = drow["TipoComp"].ToString();
                strTipoFactura = drow["TipoFactura"].ToString();
                strNroFactura = drow["NroFactura"].ToString();
                intCliente = drow["Cliente"].ToString();
                strEmpresa = drow["Razon Social"].ToString();
                strCliente = drow["NombreCompania"].ToString();
                strCUIT = drow["Cuit"].ToString();
                datFechaEmision = Convert.ToDateTime(drow["Fecha de Emision"]).ToString("yyyy-MM-dd");
                decNeto = drow["Neto"].ToString().Replace(",", ".");
                decIva = drow["IVA"].ToString().Replace(",", ".");
                decExento = drow["Exento"].ToString().Replace(",", ".");
                decTotal = drow["Total"].ToString().Replace(",", ".");

                q = string.Concat(q, "CALL SP_Grabar_SubDiarioIVA_Ventas (", intEmpresa, ",'", strTipoComp, "','", strTipoFactura, "','", strNroFactura, "',", intCliente, ",'", strEmpresa, "','", strCliente, "','", strCUIT, "','", datFechaEmision, "',", decNeto, ",", decIva, ",", decExento, ",", decTotal, ");", Environment.NewLine);

            }
            Entidades.EjecutaNonQuery_MySql(q);
        }

        private void Procesa_IVA_Compras()
        {
            List<OleDbParameter> Parametros = new List<OleDbParameter>();
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];
            int intCantidadDias = Convert.ToInt32(ConfigurationManager.AppSettings["Cant_Dias_Atras"]);

            DateTime fechatemp;
            DateTime datFecha_Desde;
            DateTime datFecha_Hasta;


            fechatemp = DateTime.Today;
            datFecha_Desde = new DateTime(fechatemp.Year, fechatemp.Month, 1).AddDays(intCantidadDias);
            
            if ((fechatemp.Month + 1) == 13)
            {
                datFecha_Hasta = new DateTime(fechatemp.Year + 1, 1, 1).AddDays(-1);
            }
            else
            {
                datFecha_Hasta = new DateTime(fechatemp.Year, fechatemp.Month + 1, 1).AddDays(-1);
            }

            //Cargao Nombre COnsula
            string strSQL_Access = "MySQL_SubDiarioIVA_Compras";
            //Armo los parametros
            // dtGrilla = ocls.GetDataTable_Access(SQL, String.Empty, Parametros)
            Parametros.Add(new OleDbParameter("intEmpresa", OleDbType.Integer) { Value = Codigo_Empresa_MySql });
            Parametros.Add(new OleDbParameter("datFechaDesde", OleDbType.Date) { Value = datFecha_Desde });
            Parametros.Add(new OleDbParameter("datFechaHasta", OleDbType.Date) { Value = datFecha_Hasta });
            //Cargo el DataTable
            DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access, Parametros);
            string q;
            string strNombreEmpresa;
            string strNombreProveedor;
            string intEmpresa;
            string intProveedor;
            string strTipoComp;
            string strNroComp;
            string datFechaComp;
            string datFechaReg;
            string strCUIT;
            string decTotalIvas;
            string decTotal;
            string decImpoBruto;
            string decRetGanancia;
            string decIva21;
            string decIva105;
            string decIva27;
            string decPercIIBB_BS;
            string decPercIIBB_Cap;
            string decTotalIVA;
            string decPercIVA;
            string decPercGanancias;
            string decImpoNoGravado;

            q = string.Empty;
            foreach (DataRow drow in dtbDatos.Rows)
            {

                //qDet = string.Concat(qDet, "CALL ", strSP_Items, "(", tEmpresa ,",",tItem ,",",tCont,",",tGen ,",",tEstado, ");",Environment.NewLine);

                strNombreEmpresa= drow["Empresa"].ToString();
                strNombreProveedor=drow["NombreProveedor"].ToString();
                intEmpresa = drow["Cod_Empresa"].ToString();
                intProveedor= drow["IdProveedor"].ToString();
                strTipoComp = drow["Tipo Comprobante"].ToString();
                strNroComp = drow["Numero Comprobante"].ToString();

                datFechaComp = Convert.ToDateTime(drow["Fecha"]).ToString("yyyy-MM-dd");
                datFechaReg = Convert.ToDateTime(drow["Fecregistrac"]).ToString("yyyy-MM-dd");
                strCUIT = drow["Cuit"].ToString();
                
                
                decTotalIvas = drow["TotalIvas"].ToString().Replace(",", ".");
                decTotal = drow["Total"].ToString().Replace(",", ".");
                decImpoBruto = drow["ImporteBruto"].ToString().Replace(",", ".");
                decTotal = drow["Total"].ToString().Replace(",", ".");
                decRetGanancia = drow["RetGanancia"].ToString().Replace(",", ".");
                decIva21 = drow["Iva21"].ToString().Replace(",", ".");
                decIva105 = drow["Iva105"].ToString().Replace(",", ".");
                decIva27 = drow["Iva27"].ToString().Replace(",", ".");
                decPercIIBB_BS = drow["PercepcionIIBB"].ToString().Replace(",", ".");
                decPercIIBB_Cap = drow["PercepcionIIBB1"].ToString().Replace(",", ".");
                decTotalIVA = drow["TotalIva"].ToString().Replace(",", ".");
                decPercIVA = drow["CondicionIVA"].ToString().Replace(",", ".");
                decPercGanancias = drow["CondicionGanancia"].ToString().Replace(",", ".");
                decImpoNoGravado = drow["impoNoGravado"].ToString().Replace(",", ".");


                q = string.Concat(q, "CALL SP_Grabar_SubDiarioIVA_Compras ('", strNombreEmpresa, "','", strNombreProveedor, "',", intEmpresa, ",", intProveedor, ",'", strTipoComp, "','", strNroComp, "','", datFechaComp, "','", datFechaReg, "','", strCUIT, "',", decTotalIvas, ",", decTotal, ",", decImpoBruto, ",", decRetGanancia, ",", decIva21, ",", decIva105, ",", decIva27, ",", decPercIIBB_BS, ",", decPercIIBB_Cap, ",", decTotalIVA, ",", decPercIVA, ",", decPercGanancias, ",", decImpoNoGravado, ");", Environment.NewLine);

            }
            Entidades.EjecutaNonQuery_MySql(q);
        }
        private void Procesa_Anticipos()
        {
            string Proceso_Tag=string.Empty;
            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Anticipos...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intID;
            int intOT;
            int intItem;
            int intEmpresa;
            string strLeyenda;

            string strSQL_Server = string.Empty;

            //DataTable dt;
            string strInsert_MySQL = string.Empty;
            string strSQL_Access;

            int intCantidad_Registros = 0;
            DataSet dsRemitos = Entidades.GetDataSet_MySql("call SP_Transferencia_Para_Anticipos(" + Codigo_Empresa_MySql + ")");
            foreach (DataTable dtbDatos in dsRemitos.Tables)
            {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {
                    this.prgBar.Value += 1;


                    try
                    {
                        //Primero verifico si existe el proveedor

                        //strSQL_Access = string.Empty;
                        //strSQL_Access = "SELECT 1 FROM PROVEEDORES WHERE idProveedor=" + intID;
                        //dt = Entidades.GetDataTable_Access(strSQL_Access, CommandType.Text);

                        strLeyenda = string.Empty;
                        strLeyenda = row["Observaciones"].ToString();
                        //strLeyenda = strLeyenda.Replace("XXX", "' + chr$(13) + chr$(10) + '");
                        Proceso_Tag = "Insert en MySQL_Anticipos";
                        strSQL_Access = string.Empty;
                        strSQL_Access = string.Concat(strSQL_Access, "INSERT INTO MySQL_Anticipos(Empresa, OT, Item_OT, Item_Anticipo, Fecha, Importe, Litros, ");
                        strSQL_Access = string.Concat(strSQL_Access, "Precio_Litro, Observaciones, Contenedor, IdProveedor,Anticipo_Asignado,Forma_Pago, opMoro,Fecha_Cheque,Nro_Cheque) ");
                        strSQL_Access = string.Concat(strSQL_Access, "VALUES (", row["Empresa"].ToString(), ",", row["OT"].ToString(), ",", row["Item_OT"].ToString(), ",", row["Item_Anticipo"].ToString(), ",'");
                        strSQL_Access = string.Concat(strSQL_Access, Convert.ToDateTime(row["Fecha"]).ToString("yyyy-MM-dd"), "',", row["Importe"].ToString().Replace(",", "."), ",", row["Litros"].ToString().Replace(",", "."));
                        strSQL_Access = string.Concat(strSQL_Access, ",", row["Precio_Litro"].ToString().Replace(",", "."), ",'");
                        strSQL_Access = string.Concat(strSQL_Access, strLeyenda, "','");
                        strSQL_Access = string.Concat(strSQL_Access, row["Contenedor"].ToString(), "',", row["IdProveedor"].ToString().Replace(",", "."), ",'", row["Nro_AP"].ToString(), "',");
                        strSQL_Access = string.Concat(strSQL_Access, row["Forma_Pago"].ToString(), ",", row["opMoro"].ToString() , ",'", Convert.ToDateTime(row["Fecha_Cheque"]).ToString("yyyy-MM-dd"), "','", row["Nro_Cheque"].ToString(), "')");


                        Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);

                        intID = Int32.Parse(row["ID"].ToString());
                        intOT= Int32.Parse(row["OT"].ToString());
                        intItem = Int32.Parse(row["Item_OT"].ToString());
                        intEmpresa= Int32.Parse(row["Empresa"].ToString());



                        string strSQL_Update = "";
                        strSQL_Update = "Update rel_OT_Item_Anticipos Set Actualizacion_Web = Now() Where ID = " + intID ;

                        MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        conn.Open();

                        MySqlCommand mySqlcmd = new MySqlCommand(strSQL_Update, conn);

                        MySqlDataReader MyReader;

                        MyReader = mySqlcmd.ExecuteReader();

                        //
                        //string strSQL_Update = "";
                        strSQL_Update = "Update itemot Set TieneAnticipo = 1 Where IdEmpresa = " + intEmpresa + " AND IdOT=" + intOT + " AND Item=" + intItem;

                        MySqlConnection conn1 = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        conn1.Open();

                        MySqlCommand mySqlcmd1 = new MySqlCommand(strSQL_Update, conn1);

                        MySqlDataReader MyReader1;

                        MyReader1 = mySqlcmd1.ExecuteReader();

                        //MyReader.Close();
                        conn.Close();
                        conn1.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "(" + Proceso_Tag +")","Proceso Anticipos");
                        //throw;
                    }
                    finally
                    {

                        //connection.Close();
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }
            }
        }
        private void Procesa_Remitos()
        {
            string Proceso_Tag = string.Empty;
            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Remitos...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intEmpresa;
            int intOT;
            int intItem;
            string strLeyenda;

            string strSQL_Server = string.Empty;

            //DataTable dt;
            string strInsert_MySQL = string.Empty;
            string strSQL_Access;

            int intCantidad_Registros = 0;
            DataSet dsRemitos = Entidades.GetDataSet_MySql("call SP_Transferencia_Para_Remitos("+ Codigo_Empresa_MySql  +")");

            foreach (DataTable dtbDatos in dsRemitos.Tables)
            {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {

                    this.prgBar.Value += 1;

                    
                    try
                    {
                        //Primero verifico si existe el proveedor

                        //strSQL_Access = string.Empty;
                        //strSQL_Access = "SELECT 1 FROM PROVEEDORES WHERE idProveedor=" + intID;
                        //dt = Entidades.GetDataTable_Access(strSQL_Access, CommandType.Text);
                        Proceso_Tag = "Inserto n MySQL_Remitos";
                        strLeyenda = string.Empty;
                        strLeyenda = row["Leyenda"].ToString();
                        strLeyenda = strLeyenda.Replace("XXX", "' + chr$(13) + chr$(10) + '");
                            strSQL_Access = string.Empty;
                            strSQL_Access = string.Concat(strSQL_Access, "INSERT INTO MySQL_Remitos(IdEmpresa, IdOT, Item, IdCliente, Razon_Social, Buque, Booking, ");
                            strSQL_Access = string.Concat(strSQL_Access, "Servicio, RT, Ruta, Venta, Prov, NroContenedor, Leyenda,IdProveedor,FechaFacturacion,Cod_Referencia,IdEstado,Compra) ");
                            strSQL_Access = string.Concat(strSQL_Access, "VALUES (", row["IdEmpresa"].ToString(), ",", row["IdOT"].ToString(), ",", row["Item"].ToString(), ",", row["IdCliente"].ToString(),",'");
                            strSQL_Access = string.Concat(strSQL_Access, row["Razon_Social"].ToString(), "','", row["Buque"].ToString(), "','", row["Booking"].ToString(),"','",row["Servicio"].ToString(),"','",row["Modalidad"].ToString(),"','");
                            strSQL_Access = string.Concat(strSQL_Access, row["Ruta"].ToString(), "',", row["Venta"].ToString().Replace(",","."), ",'", row["Prov"].ToString(), "','", row["NroContenedor"].ToString(), "','", strLeyenda, "'," , row["IdProveedor"].ToString() , ",'");
                            strSQL_Access = string.Concat(strSQL_Access, Convert.ToDateTime(row["FechaFacturacion"]).ToString("yyyy-MM-dd"),"','", row["Cod_Referencia"].ToString(), "',", row["Estado"].ToString(),",", row["Costo"].ToString().Replace(",", "."),")");


                            Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);
                        
                            intEmpresa = Int32.Parse(row["IdEmpresa"].ToString());
                            intOT = Int32.Parse(row["IdOT"].ToString());
                            intItem = Int32.Parse(row["Item"].ToString());


                        string strSQL_Update = "";
                        strSQL_Update = "Update itemot Set TieneFactura = 0 Where IdEmpresa = " + intEmpresa + " AND IdOT="+intOT + " AND Item="+intItem ;
                        Entidades.EjecutaNonQuery_MySql(strSQL_Update);
                        //MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        //conn.Open();

                        //MySqlCommand mySqlcmd = new MySqlCommand(strSQL_Update, conn);

                        //MySqlDataReader MyReader;

                        //MyReader = mySqlcmd.ExecuteReader();

                        ////MyReader.Close();
                        //conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "(" + Proceso_Tag + ")", "Proceso Remitos");
                        //throw;
                    }
                    finally
                    {

                        //connection.Close();
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }
            }
        }
        private void Procesa_Remitos_Especiales()
        {
            string Proceso_Tag = string.Empty;
            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Remitos...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intEmpresa;
            int intOT;
            int intItem;
            string strLeyenda;

            string strSQL_Server = string.Empty;

            //DataTable dt;
            string strInsert_MySQL = string.Empty;
            string strSQL_Access;

            int intCantidad_Registros = 0;
            DataSet dsRemitos = Entidades.GetDataSet_MySql("call SP_Transferencia_Para_Remitos_Especiales(" + Codigo_Empresa_MySql + ")");

            foreach (DataTable dtbDatos in dsRemitos.Tables)
            {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {

                    this.prgBar.Value += 1;


                    try
                    {
                        //Primero verifico si existe el proveedor

                        //strSQL_Access = string.Empty;
                        //strSQL_Access = "SELECT 1 FROM PROVEEDORES WHERE idProveedor=" + intID;
                        //dt = Entidades.GetDataTable_Access(strSQL_Access, CommandType.Text);
                        Proceso_Tag = "Inserto n MySQL_Remitos_Especiales";
                        strLeyenda = string.Empty;
                        strLeyenda = row["Leyenda"].ToString();
                        strLeyenda = strLeyenda.Replace("XXX", "' + chr$(13) + chr$(10) + '");
                        strSQL_Access = string.Empty;
                        strSQL_Access = string.Concat(strSQL_Access, "INSERT INTO MySQL_Remitos_Especiales(IdEmpresa, IdOT, Item, IdCliente, Razon_Social, Buque, Booking, ");
                        strSQL_Access = string.Concat(strSQL_Access, "Servicio, RT, Ruta, Venta, Prov, NroContenedor, Leyenda,IdProveedor,FechaFacturacion,Cod_Referencia) ");
                        strSQL_Access = string.Concat(strSQL_Access, "VALUES (", row["IdEmpresa"].ToString(), ",", row["IdOT"].ToString(), ",", row["Item"].ToString(), ",", row["IdCliente"].ToString(), ",'");
                        strSQL_Access = string.Concat(strSQL_Access, row["Razon_Social"].ToString(), "','", row["Buque"].ToString(), "','", row["Booking"].ToString(), "','", row["Servicio"].ToString(), "','", row["Modalidad"].ToString(), "','");
                        strSQL_Access = string.Concat(strSQL_Access, row["Ruta"].ToString(), "',", row["Venta"].ToString().Replace(",", "."), ",'", row["Prov"].ToString(), "','", row["NroContenedor"].ToString(), "','", strLeyenda, "',", row["IdProveedor"].ToString(), ",'");
                        strSQL_Access = string.Concat(strSQL_Access, Convert.ToDateTime(row["FechaFacturacion"]).ToString("yyyy-MM-dd"), "','", row["Cod_Referencia"].ToString(), "')");


                        Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);

                        intEmpresa = Int32.Parse(row["IdEmpresa"].ToString());
                        intOT = Int32.Parse(row["IdOT"].ToString());
                        intItem = Int32.Parse(row["Item"].ToString());


                        string strSQL_Update = "";
                        strSQL_Update = "Update itemot Set opMoro_Estado = 1 Where IdEmpresa = " + intEmpresa + " AND IdOT=" + intOT + " AND Item=" + intItem;
                        Entidades.EjecutaNonQuery_MySql(strSQL_Update);
                        //MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        //conn.Open();

                        //MySqlCommand mySqlcmd = new MySqlCommand(strSQL_Update, conn);

                        //MySqlDataReader MyReader;

                        //MyReader = mySqlcmd.ExecuteReader();

                        ////MyReader.Close();
                        //conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "(" + Proceso_Tag + ")", "Proceso Remitos");
                        //throw;
                    }
                    finally
                    {

                        //connection.Close();
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }
            }
        }

        private void Corre_Proceso_Contenedores()
        {
            string Proceso_Tag = string.Empty;
            string Proceso_Contenedor = ConfigurationManager.AppSettings["Corre_Proceso_Contenedores"];
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];
            
            if (Proceso_Contenedor=="0")
            {
                return;
            }
            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Contenedores...";

            this.prgBar.Maximum = 0;

            this.Refresh();


            string strSQL_Server = string.Empty;

            //DataTable dt;
            string strInsert_MySQL = string.Empty;
            string strSQL_Access;

            int intCantidad_Registros = 0;
            DataSet dsRemitos = Entidades.GetDataSet_MySql("call SP_ZZZ_CONTENEDORES()");

            foreach (DataTable dtbDatos in dsRemitos.Tables)
            {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {

                    this.prgBar.Value += 1;


                    try
                    {
                        //Primero verifico si existe el proveedor

                        //strSQL_Access = string.Empty;
                        //strSQL_Access = "SELECT 1 FROM PROVEEDORES WHERE idProveedor=" + intID;
                        //dt = Entidades.GetDataTable_Access(strSQL_Access, CommandType.Text);
                        Proceso_Tag = "Inserto n MySQL_Remitos";
                        strSQL_Access = string.Empty;
                        strSQL_Access= string.Concat(strSQL_Access, "UPDATE MySQL_Remitos SET MySQL_Remitos.IdProveedor = ", row["IdProveedor"].ToString() ,",FechaFacturacion='", Convert.ToDateTime(row["RetiroFecha"]).ToString("yyyy-MM-dd"), "' ");
                        strSQL_Access = string.Concat(strSQL_Access, "WHERE MySQL_Remitos.IdEmpresa = ", row["IdEmpresa"].ToString(), " AND ");
                        strSQL_Access = string.Concat(strSQL_Access, "MySQL_Remitos.IdOT = ", row["IdOT"].ToString(), " AND ");
                        strSQL_Access = string.Concat(strSQL_Access, "MySQL_Remitos.Item = ", row["Item"].ToString(), "  AND ");
                        strSQL_Access = string.Concat(strSQL_Access, "MySQL_Remitos.NroContenedor= '", row["NroContenedor"].ToString(), "'  ");
                        
                        Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);

                        //intEmpresa = Int32.Parse(row["IdEmpresa"].ToString());
                        //intOT = Int32.Parse(row["IdOT"].ToString());
                        //intItem = Int32.Parse(row["Item"].ToString());


                        //string strSQL_Update = "";
                        //strSQL_Update = "Update itemot Set TieneFactura = 0 Where IdEmpresa = " + intEmpresa + " AND IdOT=" + intOT + " AND Item=" + intItem;

                        //MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        //conn.Open();

                        //MySqlCommand mySqlcmd = new MySqlCommand(strSQL_Update, conn);

                        //MySqlDataReader MyReader;

                        //MyReader = mySqlcmd.ExecuteReader();

                        ////MyReader.Close();
                        //conn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "(" + Proceso_Tag + ")", "Proceso Remitos");
                        //throw;
                    }
                    finally
                    {

                        //connection.Close();
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }
            }
        }

        private void Procesa_Clientes()
        {

            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Clientes...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intID;
            decimal decDescuento;
            string Codigo_Empresa_MySql;

            string strSQL_Access = string.Empty;

            strSQL_Access = "Traer_Clientes_para_MySQL";


            string strInsert_MySQL = string.Empty;

            int intCantidad_Registros = 0;
            //int intContador = 0;

            string MyConnectionString = string.Empty;

            MyConnectionString = Entidades.CadenaConexion_MySql; //"Server=192.185.83.243;Database=fasanico_BBDD;Uid=fasanico_User;Pwd=palomar2912;";
            MySqlConnection connection = new MySqlConnection(MyConnectionString);

            //MySqlCommand cmd = new MySqlCommand();
            connection.Open();

            DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access,CommandType.StoredProcedure);

           // foreach (DataTable dtbDatos in dsClientes.Tables)
          //  {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {

                    this.prgBar.Value += 1;

                    Codigo_Empresa_MySql = row["Empresa"].ToString();
                    intID = Convert.ToInt32 (row["ID"].ToString());
                    decDescuento = Convert.ToDecimal(row["Descuento"].ToString());

                    strInsert_MySQL = string.Empty;

                    strInsert_MySQL = "Delete From " + strNombre_BBDD_MySql + ".clientes Where IdEmpresa=" + Codigo_Empresa_MySql  + " AND ID = " + intID + "; " + Environment.NewLine;

                    strInsert_MySQL += "Insert Into " + strNombre_BBDD_MySql + ".clientes (IdEmpresa,ID, Razon_Social, Registro_Fiscal, Domicilio, Localidad, Provincia, Pais, Codigo_Postal, " + Environment.NewLine;
                    strInsert_MySQL += "Telefonos, EMail, Condicion_IVA, Descuento,Inicio_Actividades " + Environment.NewLine;
                    strInsert_MySQL += " ) Values (" + Environment.NewLine;

                DateTime datInicio_Actividad;
                datInicio_Actividad = Convert.ToDateTime(row["Inicio_Actividades"].ToString());

                    strInsert_MySQL += Codigo_Empresa_MySql + ", ";
                    strInsert_MySQL += intID + ", ";
                    strInsert_MySQL += "'" + row["Razon_Social"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["Registro_Fiscal"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["Domicilio"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["Localidad"].ToString() + "', ";
                    strInsert_MySQL += row["Provincia"].ToString() + ", ";
                    strInsert_MySQL += row["Pais"].ToString() + ", ";
                    strInsert_MySQL += "'" + row["Codigo_Postal"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["Telefonos"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["E_Mail"].ToString() + "', ";
                    strInsert_MySQL += row["Condicion_IVA"].ToString() + ", ";
                    strInsert_MySQL += row["Descuento"].ToString() + ", '";
                    strInsert_MySQL += datInicio_Actividad.ToString("yyyy-MM-dd") +"' ";
                    strInsert_MySQL += "); " + Environment.NewLine;

                    //strInsert_MySQL += decDescuento.ToString() + ", ";

                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(strInsert_MySQL, connection);
                        MySqlDataReader MyReader;

                        MyReader = cmd.ExecuteReader();

                        MyReader.Close();

                        string strSQL_Update = "";

                        strSQL_Update = "UPDATE clientes SET Novedad=False Where Empresa=" + Codigo_Empresa_MySql + " AND IdCliente = " + intID;

                        int intResultado = Entidades.ExecuteNonQuery_Access(strSQL_Update,CommandType.Text);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //throw;
                    }
                    finally
                    {
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }

           // }

        }

        private void Procesa_Proveedores_PDF()
        {
            datos_app_Config datos = datos_app_Config.Instance();
            Comun.General g = new Comun.General();
            g.Iniciar_Datos();
            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];

            string strInsert_MySQL;
            string strSQL_Access;
            int intID;
            int intProveedor;

            strSQL_Access = "Traer_Proveedores_PDF";

            string strMiRuta_FTP;
            string MyConnectionString;

            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            MyConnectionString = Entidades.CadenaConexion_MySql; //"Server=192.185.83.243;Database=fasanico_BBDD;Uid=fasanico_User;Pwd=palomar2912;";
            MySqlConnection connection = new MySqlConnection(MyConnectionString);

            connection.Open();

            ///Access
            DataTable dtbDatosC = Entidades.GetDataTable_Access(strSQL_Access, CommandType.StoredProcedure);
            this.prgBar.Minimum = 0;
            this.prgBar.Maximum = dtbDatosC.Rows.Count ;

            foreach (DataRow rowC in dtbDatosC.Rows)
            {

                this.prgBar.Value += 1;

                
                intProveedor = Convert.ToInt32(rowC[1].ToString());
                //decDescuento = Convert.ToDecimal(row["Descuento"].ToString());

                strSQL_Access = "SELECT * FROM Traer_Proveedores_PDF_Detalle WHERE IdEmpresa="+ Codigo_Empresa_MySql + " AND Cod_Proveedor=" + intProveedor;
                DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access, CommandType.Text );
                //foreach (DataRow row in dtbDatos.Rows)
                //{

                    //intID = Convert.ToInt32(row[0].ToString());


                    strInsert_MySQL = string.Empty;



                //primero subo archivo ftp
                //var responseUpload = OperationFTPLibrary.UploadFile(datos.FTP_NAME, datos.FTP_USER, datos.FTP_PASS, datos.FTP_BASE_FOLDER, row["RutaComp"].ToString(), rowC["CUIT"].ToString(), row["NombreArchivo"].ToString());
                var responseUpload = OperationFTPLibrary.UploadFile(datos.FTP_NAME, datos.FTP_USER, datos.FTP_PASS, datos.FTP_BASE_FOLDER, rowC["CUIT"].ToString(), dtbDatos);
                //}
                foreach (DataRow row in dtbDatos.Rows)
                {
                    intID = Convert.ToInt32(row[0].ToString());
                    strMiRuta_FTP = "/" + datos.FTP_BASE_FOLDER + "/" + row["CUIT"].ToString() + "/" + row["NombreArchivo"].ToString();
                    strInsert_MySQL = string.Empty;
                    strInsert_MySQL += "Insert Into " + strNombre_BBDD_MySql + ".web_prov_detalle (IdEmpresa,cod_Proveedor,fecha,detalle,nombreArchivo,rutaArchivo,CUIT " + Environment.NewLine;
                    strInsert_MySQL += " ) Values (" + Environment.NewLine;


                    strInsert_MySQL += Codigo_Empresa_MySql + ", ";
                    strInsert_MySQL += "" + intProveedor + ", ";
                    strInsert_MySQL += "'" + Convert.ToDateTime(row["FechaComp"]).ToString("yyyy-MM-dd") + "', ";
                    strInsert_MySQL += "'" + row["DetalleComp"].ToString() + "', ";
                    strInsert_MySQL += "'" + row["NombreArchivo"].ToString() + "', ";
                    strInsert_MySQL += "'" + strMiRuta_FTP + "', ";
                    strInsert_MySQL += "'" + row["CUIT"].ToString() + "' ";
                    strInsert_MySQL += "); " + Environment.NewLine;

                        //strInsert_MySQL += decDescuento.ToString() + ", ";

                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(strInsert_MySQL, connection);
                            MySqlDataReader MyReader;

                            MyReader = cmd.ExecuteReader();

                            MyReader.Close();

                            string strSQL_Update = "";

                            strSQL_Update = "UPDATE Proveedores_Detalle_Web SET Procesado=False Where Id = " + intID;

                            int intResultado = Entidades.ExecuteNonQuery_Access(strSQL_Update, CommandType.Text);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            //throw;
                        }
                        finally
                        {
                            /*
                            if (gblnGrabacion_OK == true)
                            {
                                MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            */
                        }
                    
                }
            }

                
        }
        private void Procesa_Proveedores_desde_Access()
        {

            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"];

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Proveedores...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intID;
            //decimal decDescuento;

            string strSQL_Access;

            strSQL_Access = "Traer_Proveedores_para_MySQL";


            string strInsert_MySQL;

            int intCantidad_Registros;
            //int intContador = 0;

            string MyConnectionString;
            string Codigo_Empresa_MySql = ConfigurationManager.AppSettings["Codigo_Empresa_Registrada"];

            MyConnectionString = Entidades.CadenaConexion_MySql; //"Server=192.185.83.243;Database=fasanico_BBDD;Uid=fasanico_User;Pwd=palomar2912;";
            MySqlConnection connection = new MySqlConnection(MyConnectionString);

            //MySqlCommand cmd = new MySqlCommand();
            connection.Open();

            DataTable dtbDatos = Entidades.GetDataTable_Access(strSQL_Access, CommandType.StoredProcedure);

            // foreach (DataTable dtbDatos in dsClientes.Tables)
            //  {
            intCantidad_Registros = dtbDatos.Rows.Count;

            this.prgBar.Maximum = intCantidad_Registros;
            this.prgBar.Minimum = 0;

            foreach (DataRow row in dtbDatos.Rows)
            {

                this.prgBar.Value += 1;

                intID = Convert.ToInt32(row[1].ToString());
                //decDescuento = Convert.ToDecimal(row["Descuento"].ToString());

                strInsert_MySQL = string.Empty;

                
                //strInsert_MySQL += "Insert Into " + strNombre_BBDD_MySql + ".web_prov (IdEmpresa, cod_Proveedor,Razon_Social, cuit " + Environment.NewLine;
                //strInsert_MySQL += " ) Values (" + Environment.NewLine;

                
                //strInsert_MySQL += Codigo_Empresa_MySql + ", ";
                //strInsert_MySQL += "" + intID + ", ";
                //strInsert_MySQL += "'" + row[2].ToString() + "', ";
                //strInsert_MySQL += "'" + row[3].ToString() + "' ";
                //strInsert_MySQL += "); " + Environment.NewLine;

                strInsert_MySQL = string.Concat("CALL SP_Web_Alta_Proveedores (", Codigo_Empresa_MySql, ",", intID , ",'", row[2].ToString(), "','", row[3].ToString(),"')");

                //strInsert_MySQL += decDescuento.ToString() + ", ";

                try
                {
                    //MySqlCommand cmd = new MySqlCommand(strInsert_MySQL, connection);
                    //MySqlDataReader MyReader;

                    //MyReader = cmd.ExecuteReader();

                    //MyReader.Close();
                    Entidades.EjecutaNonQuery_MySql(strInsert_MySQL);

                    string strSQL_Update = "";

                    strSQL_Update = "UPDATE proveedores SET Subir_Web=False Where Empresa=" + Codigo_Empresa_MySql + " AND  IdProveedor = " + intID;

                    int intResultado = Entidades.ExecuteNonQuery_Access(strSQL_Update, CommandType.Text);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    //throw;
                }
                finally
                {
                    /*
                    if (gblnGrabacion_OK == true)
                    {
                        MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    */
                }

            }

            // }

        }

        private void Procesa_Proveedores_Desde_MySql()
        {
            string Codigo_Empresa_MySql;
            string strNombre_BBDD_MySql = ConfigurationManager.AppSettings["Nombre_BBDD_MySql"].ToString();

            this.lblActualizando.Text = string.Empty;
            this.lblActualizando.Text = "Actualizando Proveedores...";

            this.prgBar.Maximum = 0;

            this.Refresh();

            int intID;

            string strSQL_Server = string.Empty;

            //strSQL_Server = "Select ID, Razon_Social, Registro_Fiscal, Domicilio, Localidad, Provincia, Isnull(Codigo_Postal, '') Codigo_Postal,  " + Environment.NewLine;
            //strSQL_Server += "Isnull(Telefonos, '') Telefonos, Isnull(E_Mail,'') E_Mail, Condicion_IVA, 1 Pais " + Environment.NewLine;
            //strSQL_Server += "From proveedores " + Environment.NewLine;
            //strSQL_Server += "Where Actualizacion_Web is Null ";
            DataTable dt;
            string strInsert_MySQL = string.Empty;
            string strSQL_Access;

            int intCantidad_Registros = 0;
            //string MyConnectionString = string.Empty;


            DataSet dsProveedores = Entidades.GetDataSet_MySql("call SP_GET_Proveedores_All(null, null, 1)");

            foreach (DataTable dtbDatos in dsProveedores.Tables)
            {
                intCantidad_Registros = dtbDatos.Rows.Count;

                this.prgBar.Maximum = intCantidad_Registros;
                this.prgBar.Minimum = 0;

                foreach (DataRow row in dtbDatos.Rows)
                {

                    this.prgBar.Value += 1;

                    intID = Convert.ToInt32(row["ID"].ToString());
                    Codigo_Empresa_MySql = row["idEmpresa"].ToString();


                    try
                    {
                        //Primero verifico si existe el proveedor
                        
                        strSQL_Access = string.Empty;
                        strSQL_Access = "SELECT 1 FROM PROVEEDORES WHERE Empresa=" + Codigo_Empresa_MySql + " AND idProveedor=" + intID;
                        dt = Entidades.GetDataTable_Access(strSQL_Access,CommandType.Text);

                        if(dt.Rows.Count != 0)//si hay registros
                        {
                            strSQL_Access = string.Empty;
                            strSQL_Access = string.Concat(strSQL_Access, "UPDATE Proveedores SET ");
                            strSQL_Access = string.Concat(strSQL_Access, "Dirección='", row["Domicilio"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "Ciudad='", row["Localidad"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "IdProvincia=", row["Provincia"].ToString(), ",");
                            strSQL_Access = string.Concat(strSQL_Access, "CódPostal='", row["Codigo_Postal"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "IdPais=", row["Pais"].ToString(), ",");
                            strSQL_Access = string.Concat(strSQL_Access, "NúmTeléfono='", row["Telefonos"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "DirCorreoElectrónico='", row["EMail"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "cuit='", row["Registro_Fiscal"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "CondicionIVA=", row["Condicion_IVA"].ToString(), " ");
                            strSQL_Access = string.Concat(strSQL_Access, "WHERE IdProveedor =", intID, " AND Empresa=", Codigo_Empresa_MySql);

                            Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);
                        }
                        else
                        {
                            strSQL_Access = string.Empty;
                            strSQL_Access = string.Concat(strSQL_Access, "INSERT INTO Proveedores ([Empresa], [IdProveedor], [NombreCompañía], [Dirección], [Ciudad], [IdProvincia], [CódPostal], [IdPais], [NúmTeléfono], [DirCorreoElectrónico], ");
                            strSQL_Access = string.Concat(strSQL_Access, "[cuit], [CondicionIVA],[CondicionGanancia],[CondicionIIBB],[TipoProveedor],Subir_Web) ");
                            strSQL_Access = string.Concat(strSQL_Access, "VALUES (", Codigo_Empresa_MySql, ",", intID,",'", row["Razon_Social"].ToString(), "','", row["Domicilio"].ToString(), "',");
                            strSQL_Access = string.Concat(strSQL_Access, "',",row["Localidad"].ToString(), "',", row["Provincia"].ToString(),",'", row["Codigo_Postal"].ToString(),"',", row["Pais"].ToString(),",");
                            strSQL_Access = string.Concat(strSQL_Access, "',", row["Telefonos"].ToString(), "','", row["EMail"].ToString(), "','", row["Registro_Fiscal"].ToString(), "',", row["Condicion_IVA"].ToString(), ",0,0,1,True)");
                            Entidades.ExecuteNonQuery_Access(strSQL_Access, CommandType.Text);
                        }



                        string strSQL_Update = "";
                        strSQL_Update = "Update proveedores Set Novedad = 0 Where IdEmpresa="  + Codigo_Empresa_MySql + " AND id = " + intID;

                        MySqlConnection conn = new MySqlConnection(Entidades.CadenaConexion_MySql);

                        conn.Open();

                        MySqlCommand mySqlcmd = new MySqlCommand(strSQL_Update, conn);

                        MySqlDataReader MyReader;

                        MyReader = mySqlcmd.ExecuteReader();

                        //MyReader.Close();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //throw;
                    }
                    finally
                    {

                        //connection.Close();
                        /*
                        if (gblnGrabacion_OK == true)
                        {
                            MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        */
                    }

                }

            }

        }


        //private void Procesa_Articulos()
        //{
        //    this.lblActualizando.Text = "";
        //    this.lblActualizando.Text = "Actualizando Artículos...";
        //    this.Refresh();

        //    string strSQL_Server;

        //    string strCodigo_Articulo;
        //    decimal decPrecio_Lista;
        //    decimal decPorcentaje_Impuesto;

        //    strSQL_Server = "exec uni_Carga_Articulos_Actualizacion_Web @strAgente = 'FASANI-A', @intDescargaStock = 1, @intActualizacion_Web = 1";

        //    string strInsert_MySQL = "";

        //    int intCantidad_Registros = 0;

        //    string MyConnectionString = "";
        //    MyConnectionString = Entidades.CadenaConexion_MySql; 
        //    MySqlConnection connection = new MySqlConnection(MyConnectionString);
        //    connection.Open();

        //    DataSet dsClientes = Entidades.GetDataSet(strSQL_Server);

        //    foreach (DataTable dtbDatos in dsClientes.Tables)
        //    {
        //        intCantidad_Registros = dtbDatos.Rows.Count;
        //        this.prgBar.Maximum = intCantidad_Registros;
        //        this.prgBar.Minimum = 0;

        //        foreach (DataRow row in dtbDatos.Rows)
        //        {

        //            prgBar.Value += +1;

        //            strCodigo_Articulo = row["Articulo"].ToString();
        //            decPrecio_Lista = Convert.ToDecimal(row["Neto_Venta"].ToString());
        //            decPorcentaje_Impuesto = Convert.ToDecimal(row["Porcentaje_Impuestos"].ToString());

        //            strInsert_MySQL = "SET SQL_SAFE_UPDATES = 0;";
        //            strInsert_MySQL = "Update fasanico_BBDD.uni_Articulos Set Precio_Lista_Anterior = Lista Where Articulo = '" + strCodigo_Articulo + "'; ";

        //            strInsert_MySQL += "Update fasanico_BBDD.uni_Articulos ";
        //            strInsert_MySQL += "Set Descripcion = '" + row["Descripcion"].ToString() + "', ";
        //            strInsert_MySQL += "Marca = '" + row["Desc_Marca"].ToString() + "', ";
        //            strInsert_MySQL += "Lista = " + decPrecio_Lista.ToString() + ", ";
        //            strInsert_MySQL += "Porcentaje_Impuesto = " + decPorcentaje_Impuesto.ToString() + ", ";
        //            strInsert_MySQL += "Actualizacion_Web = CURDATE();";
        //            strInsert_MySQL += "Where Articulo = '" + row["Articulo"].ToString() + "', ";

        //            try
        //            {

        //                int intResultado = Entidades.EjecutaNonQuery(strInsert_MySQL);

        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //                throw;
        //            }
        //            finally
        //            {
        //                //connection.Close();
        //                /*
        //                if (gblnGrabacion_OK == true)
        //                {
        //                    MessageBox.Show("Registro Insertado", "Sr. Usuario", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        //                }
        //                */
        //            }

        //        }

        //    }

       // }


        private void btnConectar_Click(object sender, EventArgs e)
        {
            //Actualiza();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

            int intIntervalo_Actualizacion = 0;

            intIntervalo_Actualizacion = int.Parse(ConfigurationManager.AppSettings["Intervalo_Actualizacion"]);


            if (intResto > 0)
            {
                intResto = intResto - 1;
            }
            else
            {
                int intActivo = Activo_Actualizacion_WEB();
                if (intActivo == 0)
                {
                    Actualiza(0);
                }
                intResto = (timer.Interval * intIntervalo_Actualizacion)/ 1000;
            }

            this.lblRestan.Text = "Restan " + intResto.ToString() + " segundos...";
            this.Refresh();

        }

        private void cmdSalir_Click(object sender, EventArgs e)
        {
            this.timer.Enabled = false;
            Application.Exit();
        }

        private void cmdDetener_Click(object sender, EventArgs e)
        {
            this.timer.Enabled = false;

            this.cmdDetener.Enabled = false;
            this.cmdIniciar.Enabled = true;
            this.Refresh();
        }

        private void cmdIniciar_Click(object sender, EventArgs e)
        {
            this.timer.Enabled = true;

            this.cmdDetener.Enabled = true;
            this.cmdIniciar.Enabled = false;
            this.Refresh();

        }

        private void frmActualiza_Load(object sender, EventArgs e)
        {
            Notif.ShowBalloonTip(1000);
            Notif.Visible = true;
            this.Visible = false;
        }

        private void Notif_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Notif.Visible = false;
            this.Visible = true;
        }

        private void cmdMinimizar_Click(object sender, EventArgs e)
        {
            Notif.Visible = true;
            this.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void cmdForzar_Click(object sender, EventArgs e)
        {
            Actualiza(1);
        }
    }
}
