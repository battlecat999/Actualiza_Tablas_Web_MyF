using EnumsNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPLibrary.Enum
{
    public static class FtpResponseExtension
    {

        public static string ToDescription(this FtpResponseCode ftpResponseCode)
        {
            return ftpResponseCode.AsString(EnumFormat.Description);
        }


        public static string ToCodeStr(this FtpResponseCode ftpResponseCode)
        {
            EnumFormat codeFormat = Enums.RegisterCustomEnumFormat(m => m.Attributes.Get<CodeAttribute>()?.Name);
            return ftpResponseCode.AsString(codeFormat);
        }

    }
}
