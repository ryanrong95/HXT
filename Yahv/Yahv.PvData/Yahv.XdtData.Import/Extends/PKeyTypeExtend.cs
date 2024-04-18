using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Extends
{
    public static class PKeyTypeExtend
    {
        public static string[] Pick(this Enums.PkeyType keyType, int length)
        {
            string[] serirs;
            if (length == 1)
                serirs = new string[1] { Layers.Data.PKeySigner.Pick(keyType) };
            else
                serirs = Layers.Data.PKeySigner.Series(keyType, length);

            return serirs;
        }

        public static string[] Pick(this Underly.PKeyType keyType, int length)
        {
            string[] serirs;
            if (length == 1)
                serirs = new string[1] { Layers.Data.PKeySigner.Pick(keyType) };
            else
                serirs = Layers.Data.PKeySigner.Series(keyType, length);

            return serirs;
        }
    }
}
