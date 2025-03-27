using FTPLibrary.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPLibrary
{
    public class FtpResponse
    {
        public FtpResponseCode code { get; set; }
        public string ruta { get; set; }
    }
}
