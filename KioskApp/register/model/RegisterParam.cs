using KioskApp.common.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.register.model
{
    public class RegisterParam : BaseParam
    {
        public string name { get; set; }

        public string idCard { get; set; }

        public string sex { get; set; }

        public string birthday { get; set; }

        public string nation { get; set; }

        public string address { get; set; }

        public string phone { get; set; }

    }
}
