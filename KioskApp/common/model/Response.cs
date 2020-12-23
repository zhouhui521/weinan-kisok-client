using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.common.model
{
    public class Response<T>
    {
        public string code { get; set; }

        public T data { get; set; }

        public string message { get; set; }

    }
}
