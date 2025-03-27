using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPLibrary.Enum
{
    public enum FtpResponseCode
    {
        OK = 0,
        [Code("ECS01"), Description("La no hubo conexcion al servidor FTP")]
        ERROR_CONEXION_SERVIDOR = 9999,
        [Code("ALI01"), Description("El archivo local que se requiere para subir no existe")]
        ARCHIVO_LOCAL_INEXISTENTE = 9998,
        [Code("ECC01"), Description("Hubo un error al crear la carpeta en el servidor FTP")]
        ERROR_CREAR_CARPETA_SERVIDOR = 9997,
        [Code("EAS1"), Description("Hubo un error al momento de subir el archivo")]
        ERROR_ARCHIVO_SUBIR = 9996,
        [Code("ERD1"), Description("La ruta o archivo no existen en el servidor FTP")]
        ERROR_RUTA_DESCARGA = 9995,
        [Code("ECC02"), Description("Hubo un error al crear la capeta local")]
        ERROR_CREAR_CARPETA_LOCAL = 9994,
        [Code("EAD01"), Description("La cbu existe en una sesion abierta")]
        ERROR_ARCHIVO_DESCARGAR = 9993,
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class CodeAttribute : Attribute
    {
        public string Name { get; set; }

        public CodeAttribute(string name)
        {
            Name = name;
        }
    }

}

