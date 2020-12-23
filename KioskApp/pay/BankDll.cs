using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay
{
    public class BankDll
    {
        [DllImport(@"D:\\NH\\Release\\BankPos.dll")]
        public static extern int CreditTrans(StringBuilder strin, StringBuilder strout);
    }
}
