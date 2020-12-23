using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.register.model;
using KioskApp.sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.register.service
{
    public class RegisterService
    {
        public RegisterParam readIdCard()
        {
           


            RegisterParam registerParam = new RegisterParam();
            string profile = AppConfigUtil.get("profile");

            string hardWareResult = string.Empty;

            if ("dev".Equals(profile))
            {
                hardWareResult = "<return name=\"SHENFENZHENG\"><arguments><string id=\"ERROR\">SUCCESS</string><string id=\"IDNAME\">黄孟伟</string><string id=\"SEX\">男</string><string id=\"BORN\">19930320</string><string id=\"ADDRESS\">陕西省西咸新区秦汉新城窑店街道黄家沟515号</string><string id=\"IDCARDNO\">6104041993113123213</string><string id=\"NATION\">汉</string><string id=\"NATIONCODE\">01</string></arguments></return>";
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(hardWareResult);
                System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='ERROR']");
                if (xnd == null)
                {

                    return null;
                }


                xnd = doc.SelectSingleNode("/return/arguments/string[@id='IDNAME']");
                if (xnd == null)
                {

                    return null;
                }
                registerParam.name = xnd.InnerText;

                xnd = doc.SelectSingleNode("/return/arguments/string[@id='IDCARDNO']");
                if (xnd == null)
                {

                    return null;
                }
                registerParam.idCard = xnd.InnerText;

                xnd = doc.SelectSingleNode("/return/arguments/string[@id='SEX']");
                if (xnd == null)
                {

                    return null;
                }
                registerParam.sex = xnd.InnerText;

                xnd = doc.SelectSingleNode("/return/arguments/string[@id='BORN']");
                if (xnd == null)
                {

                    return null;
                }
                registerParam.birthday = xnd.InnerText;

                xnd = doc.SelectSingleNode("/return/arguments/string[@id='ADDRESS']");
                if (xnd == null)
                {

                    return null;
                }
                registerParam.address = xnd.InnerText;
            }
            else
            {
                registerParam = HardWareService.readIdCard();
            }
            
         

            return registerParam;


        }

        public Response<RegisterParam> makeCard(RegisterParam registerParam)
        {
            string url = AppConfigUtil.get("url");
            //检测是否办过卡
            string jsonData = JsonConvert.SerializeObject(registerParam);
            string result = HttpRestTemplate.postForString(jsonData, url + "api/register");
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            Response<RegisterParam> response = JsonConvert.DeserializeObject<Response<RegisterParam>>(result);//反序列化

            return response;
         
        }

        public void deleteCard(string cardNo)
        {
            string url = AppConfigUtil.get("url");
            //检测是否办过卡
            //string jsonData = JsonConvert.SerializeObject(registerParam);
            string result = HttpRestTemplate.postForString(cardNo, url + "api/patient/delete");
            //if (string.IsNullOrEmpty(result))
            //{
            //    return null;
            //}
            //Response<RegisterParam> response = JsonConvert.DeserializeObject<Response<RegisterParam>>(result);//反序列化

            //return response;
        }
    }
}
