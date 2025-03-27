using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actualiza_Web
{
    public class datos_app_Config
    {
        private static datos_app_Config CONFIG_APP;
        private datos_app_Config() { }
        public static datos_app_Config Instance()
        {
            if (CONFIG_APP == null)
            {
                CONFIG_APP = new datos_app_Config();
            }
            return CONFIG_APP;
        }

        public string C_MYSQL_PATH { get; set; }
        public string C_ACCESS_PATH { get; set; }
        public int C_TIMER_INTERVAL { get; set; }//informar en minutos
        public int C_TIMER_INTERVAL_SLEEP { get; set; }//informar en minutos
        public string C_PATH_LOG { get; set; }
        public string C_PATH_LICENCIA { get; set; }
        public string C_ID_CLIENTE { get; set; }//cliente
        public string C_NOMBRE_CLIENTE { get; set; }//cliente
        //DATOS PARA GRABAR EN EL LOG
        public string LECTURA_SIN_DATOS { get; set; }
        public string LECTURA_CON_DATOS { get; set; }
        public string CONFIRMA_ENVIA_EMAIL { get; set; }
        public string ERROR_CONEXION { get; set; }
        public string ERROR_FALLO_ENVIO_EMAIL { get; set; }
        public string ERROR_FALLO_GRABAR_DATOS_WEB { get; set; }
        public string ERROR_FALLO_GRABAR_DATOS_ACCESS { get; set; }

        //DATOS PARA ENVIAR EMAIL.
        public string PASS { get; set; }
        public string SMTP { get; set; }
        public string FROM_ADDRESS { get; set; }
        public string TO_ADDRESS { get; set; }
        public string SUBJECT { get; set; }
        public string BODY { get; set; }
        //FTP DATOS
        public string FTP_NAME { get; set; }
        public string FTP_USER { get; set; }
        public string FTP_PASS { get; set; }
        public string FTP_BASE_FOLDER { get; set; }
        //SSH DATOS BASE ACCESO
        public string VAR_SERVER { get; set; }
        public string VAR_sshUserName { get; set; }
        public string VAR_sshPassword { get; set; }
        public string VAR_DB_UserName { get; set; }
        public string VAR_DB_Password { get; set; }
        public string MySQL_LOCAL_SERVER { get; set; }
        public string MySQL_LOCAL_DB { get; set; }

    }
}
