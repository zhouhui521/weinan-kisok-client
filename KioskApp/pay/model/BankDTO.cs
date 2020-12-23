using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay.model
{
    public class BankDTO
    {
        /// <summary>
        /// 款台号
        /// </summary>
        public string machineId { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        public string operatorId { get; set; }

        /// <summary>
        /// 交易类型标志
        /// </summary>
        public string transType { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string money { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string payType { get; set; }
    }
}
