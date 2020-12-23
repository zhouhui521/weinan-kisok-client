using KioskApp.common.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay.model
{
    public class PayParam : BaseParam
    {
        public string payType;

        public string money { get; set; }

        public string orderNo { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string bizType { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string hosId { get; set; }
    }
}
