using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay.model
{
    public class TransactionData
    {
        /**
     * 00 表示成功，其它表示失败
     * 响应码
     */
        public string respCode { get; set; }

        /**
         * 返回码解释信息
         */
        public string respMsg { get; set; }

        /**
         * 设备终端编号
         * 唯一，可以不填写
         */
        public string posNo { get; set; }

        /**
         * 交易类型
         * C：表示被扫消费
         * R：表示退款
         * F: 为主扫下单
         * S: 为关闭订单
         * G：表示订单支付结果查询
         * J：表示退货结果查询
         */
        public string tranType { get; set; }

        /**
         * 以分为单位的交易金额
         */
        public string txnAmt { get; set; }

        /**
         * 商户系统订单号
         * 商户系统订单号，消费交易商户生成唯一订单号（如果不
         * 能生成，可以向扫码平台申请商户系统订单号）；支付结
         * 果查询、消费撤销、退货交易需要传入原消费交易商户系
         * 统订单号
         */
        public string merTradeNo { get; set; }

        /**
         * 商户号
         */
        public string mid { get; set; }

        /**
         * 商户名称
         */
        public string merName { get; set; }

        /**
         * 终端号
         */
        public string tid { get; set; }

        /**
         * 终端流水号
         * 终端号系统跟踪号，同请求报文原值返回，客户端收到应
         * 答报文需要验证 traceNo 字段值，需与请求报文值一致，
         * 如果不一致则丢包交易失败
         */
        public string traceNo { get; set; }

        /**
         * 支付方式
         * ZFBA-支付宝
         * WEIX-微信
         * UPAY-银联二维码
         * DZZF-电子支付
         */
        public string payType { get; set; }

        /**
         * 交易时间
         */
        public string txnTime { get; set; }

        /**
         * 支付订单号
         * 银行返回系统订单号，需要保存该支付交易订单号
         */
        public string tradeNo { get; set; }

        /**
         * 第三方支付订单号
         */
        public string transNo { get; set; }


        /**
         * 退款单号
         * 商户系统退货单号，同请求一致
         */
        public string vfTradeNo { get; set; }

        /**
         * 银行优惠金额
         */
        public string discountAmt { get; set; }

        /**
         * 有效时间
         * 二维码本身的有效时间，是相对时间，单位为秒，以接收
         * 方收到报文时间为起始点计时。不同类型的订单以及不同
         * 的订单状况会对应不同的默认有效时间和最大有效时间
         * （可以为空）
         */
        public string qrValidTime { get; set; }

        /**
         * 二维码信息
         * 主扫支付二维码，以二维码形式显示，手机 APP 扫二维码
         * 码消费
         */
        public string scanCode { get; set; }

        /**
         * 订单数据
         * 当 tranType 为 F 时，payType 值为 ZFBA 或 WEIX 时
         * 支付宝返回的 tradeNo
         * 或者微信返回的 prepayId
         */
        public string payData { get; set; }
    }
}
