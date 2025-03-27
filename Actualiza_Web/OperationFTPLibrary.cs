using FTPLibrary.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Data;
//using Microsoft.Office.Interop.Excel;

namespace FTPLibrary
{
    public static class OperationFTPLibrary
    {

        public static FtpResponse UploadFile(
            string ftp_name,
            string user_ftp,
            string pass_ftp,
            string folder_base,
            //string archivo_local,
            string nombre_carpeta,
            DataTable dt_Archivos
           )
        {

            if (!isValidConnection(ftp_name, user_ftp, pass_ftp)) return new FtpResponse { code = FtpResponseCode.ERROR_CONEXION_SERVIDOR, ruta = String.Empty };
            //if (!File.Exists(archivo_local)) return new FtpResponse { code = FtpResponseCode.ARCHIVO_LOCAL_INEXISTENTE, ruta = String.Empty };
            if (!isValidConnection(ftp_name + $"/{folder_base}/" + nombre_carpeta, user_ftp, pass_ftp))
            {
                bool ftpMakeDirectory = FtpMakeDirectory(ftp_name + $"/{folder_base}/" + nombre_carpeta, user_ftp, pass_ftp);

                //connectionSSH.ChangeDirectory(rutaCarpetaServer);

                if (!ftpMakeDirectory) return new FtpResponse { code = FtpResponseCode.ERROR_CREAR_CARPETA_SERVIDOR, ruta = String.Empty };
            }


            //datatable
            string archivo_local;
            string nombre_Archivo;
            foreach (DataRow row in dt_Archivos.Rows)
            {

                archivo_local = row["RutaComp"].ToString();
                nombre_Archivo = row["NombreArchivo"].ToString();

                if (!File.Exists(archivo_local))
                {
                    //return new FtpResponse { code = FtpResponseCode.ARCHIVO_LOCAL_INEXISTENTE, ruta = String.Empty };
                }
                else
                {
                    Upload(
                        ftp_name,
                        user_ftp,
                        pass_ftp,
                        archivo_local,
                        folder_base,
                        nombre_carpeta,
                        nombre_Archivo);
                }

            }
            return new FtpResponse { code = FtpResponseCode.OK, ruta = "" };
        }

        public static FtpResponse DownloadFile(
           string ftp_name,
           string user_ftp,
           string pass_ftp,
           string ruta_en_server_ftp,
           string rutal_local_guardar,
           string nombre_archivo
          )
        {


            if (!isValidConnection(ftp_name, user_ftp, pass_ftp)) return new FtpResponse { code = FtpResponseCode.ERROR_CONEXION_SERVIDOR, ruta = String.Empty };
            if (!isValidConnection(ftp_name + $"/{ruta_en_server_ftp}", user_ftp, pass_ftp)) return new FtpResponse { code = FtpResponseCode.ERROR_CONEXION_SERVIDOR, ruta = String.Empty };
            bool folderExists = Directory.Exists(rutal_local_guardar);
            if (!folderExists)
            {
                var infoDirectory = Directory.CreateDirectory(rutal_local_guardar);
                if (!infoDirectory.Exists) return new FtpResponse { code = FtpResponseCode.ERROR_CREAR_CARPETA_LOCAL, ruta = String.Empty };
            }

            return Download(
                    ftp_name,
                     user_ftp,
                     pass_ftp,
                     ruta_en_server_ftp,
                     rutal_local_guardar,
                     nombre_archivo);
        }

        /// 
        ///		Creo la carpeta en el servidor ftp.
        /// 
        private static bool FtpMakeDirectory(string ftp_name, string user_ftp, string pass_ftp)
        {

            try
            {
                WebRequest request = WebRequest.Create($"ftp://{ftp_name}");
                ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(user_ftp, pass_ftp);

                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    if (resp.StatusCode != FtpStatusCode.PathnameCreated) return false;

                    request.Abort();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// 
        ///		Valido si hay conexion con el servidor y si los directorios existen.
        /// 
        private static bool isValidConnection(string ftp_name, string user_ftp, string pass_ftp)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{ftp_name}");
                ServicePointManager.ServerCertificateValidationCallback = (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(user_ftp, pass_ftp);
                request.GetResponse();
                request.Abort();

            }
            catch (WebException ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message.ToString());
                return false;
            }
            return true;
        }

        /// 
        ///		Envía un archivo por FTP
        /// 
        private static FtpResponse Upload(string ftp_name, string user_ftp, string pass_ftp,
                           string archivo_local, string folder_base,string nombre_carpeta, string nombre_archivo)
        {
            FtpWebRequest ftpRequest;


            // Obtenesmos la extension del archivo a enviar
            var extension = Path.GetExtension(archivo_local);

            // Crea la ruta completa donde se guardara el archivo
            var pathFtp = string.Format("ftp://{0}/{1}/{2}/{3}", ftp_name, folder_base, nombre_carpeta, nombre_archivo.Replace(".pdf","") + extension);

            // Crea el objeto de conexión del servidor FTP
            ftpRequest = (FtpWebRequest)WebRequest.Create(pathFtp);
            // Asigna las credenciales
            ftpRequest.Credentials = new NetworkCredential(user_ftp, pass_ftp);
            // Asigna las propiedades
            ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
            //ftpRequest.UsePassive = true;
            //ftpRequest.UseBinary = true;
            //ftpRequest.KeepAlive = true;
            //  ftpRequest.Timeout = 3000;

            // Lee el archivo y lo envía
            using (FileStream stmFile = File.OpenRead(archivo_local))
            { // Obtiene el stream sobre la comunicación FTP

                int cnstIntLengthBuffer = Convert.ToInt32(stmFile.Length);

                try
                {

                    using (Stream stmFTP = ftpRequest.GetRequestStream())
                    {
                        byte[] arrBytBuffer = new byte[cnstIntLengthBuffer];
                        int intRead;

                        // Lee y escribe el archivo en el stream de comunicaciones
                        while ((intRead = stmFile.Read(arrBytBuffer, 0, cnstIntLengthBuffer)) != 0)
                            stmFTP.Write(arrBytBuffer, 0, intRead);
                        // Cierra el stream FTP
                        stmFTP.Close();
                    }
                    // Cierra el stream del archivo
                    stmFile.Close();

                    return new FtpResponse { code = FtpResponseCode.OK, ruta = pathFtp };


                }
                catch (Exception)
                {
                    return new FtpResponse { code = FtpResponseCode.ERROR_ARCHIVO_SUBIR, ruta = string.Empty };

                }

            }
        }


        private static FtpResponse Download(string ftp_name,
           string user_ftp,
           string pass_ftp,
           string ruta_en_server_ftp,
           string rutal_local_guardar,
           string nombre_archivo)
        {
            FtpWebRequest ftpRequest;

            var pathLocal = rutal_local_guardar + $"\\{Path.GetFileName(ruta_en_server_ftp)}";

            // Crea el objeto de conexión del servidor FTP
            ftpRequest = (FtpWebRequest)WebRequest.Create(string.Format("ftp://{0}/{1}", ftp_name,
                                                                         ruta_en_server_ftp));
            // Asigna las credenciales
            ftpRequest.Credentials = new NetworkCredential(user_ftp, pass_ftp);
            // Asigna las propiedades
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            //ftpRequest.UsePassive = true;
            //ftpRequest.UseBinary = true;
            //ftpRequest.KeepAlive = false;
            // Descarga el archivo y lo graba
            using (FileStream stmFile = File.OpenWrite(pathLocal))
            { // Obtiene el stream sobre la comunicación FTP

                try
                {
                    using (Stream responseStream = ((FtpWebResponse)ftpRequest.GetResponse()).GetResponseStream())
                    {
                        ;
                        byte[] arrBytBuffer = new byte[2085];
                        int intRead;

                        // Lee los datos del stream y los graba en el archivo
                        while ((intRead = responseStream.Read(arrBytBuffer, 0, Convert.ToInt32(arrBytBuffer.Length))) != 0)
                            stmFile.Write(arrBytBuffer, 0, intRead);
                        // Cierra el stream FTP	
                        responseStream.Close();
                    }
                    // Cierra el archivo de salida
                    stmFile.Flush();
                    stmFile.Close();

                    return new FtpResponse { code = FtpResponseCode.OK, ruta = pathLocal };

                }
                catch (Exception)
                {
                    return new FtpResponse { code = FtpResponseCode.ERROR_ARCHIVO_DESCARGAR, ruta = string.Empty };

                }

            }
        }

    }
}
