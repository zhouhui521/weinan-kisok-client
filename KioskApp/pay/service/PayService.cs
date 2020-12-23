using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.pay.model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.pay.service
{
    public class PayService
    {
        public Response<TransactionData> getQrCode(PayParam payParam)
        {
            string jsonData = JsonConvert.SerializeObject(payParam);
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(jsonData, url + "api/pay/index");
            if(string.IsNullOrEmpty(result)){
                return null;
            }
            Response<TransactionData> response = JsonConvert.DeserializeObject<Response<TransactionData>>(result);//反序列化
            return response;
        }


        public Response<TransactionData> getQrCode(PayParam payParam,bool flag)
        {
            string jsonData = JsonConvert.SerializeObject(payParam);
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(jsonData, url + "api/pay/index");
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            Response<TransactionData> response = JsonConvert.DeserializeObject<Response<TransactionData>>(result);//反序列化
            return response;
        }

        public Response<TransactionData> queryResult(string qrCode)
        {
            //string jsonData = JsonConvert.SerializeObject(transData);
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(qrCode, url + "api/pay/query");
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            Response<TransactionData> response = JsonConvert.DeserializeObject<Response<TransactionData>>(result);//反序列化
            return response;
        }

        public Response<string> save(string tradeNo)
        {
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(tradeNo, url + "api/pay/save");
            Response<string> response = JsonConvert.DeserializeObject<Response<string>>(result);//反序列化
            return response;
        }
        
        /// <summary>
        /// 住院预交金
        /// </summary>
        /// <param name="tradeNo"></param>
        /// <returns></returns>
        public Response<PayResultVO> saveHos(string tradeNo)
        {
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(tradeNo, url + "api/pay/saveHos");
            Response<PayResultVO> response = JsonConvert.DeserializeObject<Response<PayResultVO>>(result);//反序列化
            return response;
        }

        public Response<string> save(string tradeNo,string bizType)
        {
            string url = AppConfigUtil.get("url");
            string result = HttpRestTemplate.postForString(tradeNo, url + "api/pay/hospitalized");
            Response<string> response = JsonConvert.DeserializeObject<Response<string>>(result);//反序列化
            return response;
        }

    }
}
