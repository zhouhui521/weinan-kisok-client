using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay.model
{
    public class BankResult
    {
        public string responseCode { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string bankCardNo { get; set; }

        /// <summary>
        /// 交易类型标志
        /// </summary>
        public string transType { get; set; }


        /// <summary>
        /// 金额
        /// </summary>
        public string money { get; set; }

        /// <summary>
        /// 发卡行代码
        /// </summary>
        public string bankCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string payType { get; set; }

        /// <summary>
        /// 票据号
        /// </summary>
        public string billNo { get; set; }

        /// <summary>
        /// 授权号
        /// </summary>
        public string authorNo { get; set; }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string voucherNo { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string batchNo { get; set; }

        /// <summary>
        /// 交易时间日期
        /// </summary>
        public string transDate { get; set; }

        /// <summary>
        /// 参考号
        /// </summary>
        public string referenceNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string merchantNo { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        public string terminalNo { get; set; }

        public string cardNo { get; set; }

        public string saveMoney { get; set; }
    }

}
