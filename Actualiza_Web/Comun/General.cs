using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Actualiza_Web.Comun
{
    public class General

    {
        public static bool gblnGrabacion_OK;

        datos_app_Config datos = datos_app_Config.Instance();
        encrypasocv.Encryption e = new encrypasocv.Encryption();

        public static bool hola;
    
        public void Iniciar_Datos()
        {
            //parametros para FTP
            datos.FTP_NAME = ConfigurationManager.AppSettings["FTP_NAME"].ToString();
            datos.FTP_USER = e.Decrypt(ConfigurationManager.AppSettings["FTP_USER"].ToString());
            datos.FTP_PASS = e.Decrypt(ConfigurationManager.AppSettings["FTP_PASS"].ToString());
            datos.FTP_BASE_FOLDER = ConfigurationManager.AppSettings["FTP_BASE_FOLDER"].ToString();
        }
    }
}
