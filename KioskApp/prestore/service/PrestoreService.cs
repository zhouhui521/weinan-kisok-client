using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.prestore.model;
using KioskApp.sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.prestore.service
{
    public class PrestoreService
    {
        public Response<string> readCard()
        {
            string cardNo = "";
            Response<string> response = new Response<string>();
            string profile = AppConfigUtil.get("profile");
            if ("dev".Equals(profile))
            {
                cardNo = "acc4459248406263646566676869";
            }
            else
            {
                cardNo = HardWareService.readCard();
            }

            

            if (string.IsNullOrEmpty(cardNo))
            {
                
                response.code = "500";
                response.message = "未读取到卡号";
                return response;
            }


            
            response.code = "200";
            response.message = cardNo;
            response.data = cardNo;
            return response;

        }


        public Response<Patient> getPatient(string cardNo)
        {
            string url = AppConfigUtil.get("url");
 
            if (cardNo.Length>32)
            {
                cardNo = cardNo.Substring(0, 32);
            }
            string result = HttpRestTemplate.postForString(cardNo, url + "api/patient/index");

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            Response<Patient> response = JsonConvert.DeserializeObject<Response<Patient>>(result);//反序列化

            return response;
        }
    }
}
