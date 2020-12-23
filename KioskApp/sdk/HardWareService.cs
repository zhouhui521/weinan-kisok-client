using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.register.model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.sdk
{
    public class HardWareService
    {
        [DllImport(@"GGXmlTcp.dll")]
        public static extern int XmlTcp(StringBuilder xmlbuf, int timeout);

   

        public static string getValue(string param)
        {
            StringBuilder xmlBuff = new StringBuilder(1000);
            xmlBuff.Append(param);
            int ret = XmlTcp(xmlBuff, 10000);
            return xmlBuff.ToString();
        }

        public static bool checkCard()
        {
            string param = "<invoke name=\"READCARDTESTINSERTCARD\"><arguments></arguments></invoke>";
            string result = getValue(param);
            if (string.IsNullOrEmpty(result))
            {
                return false;
            }
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='CARDSTACKSTATE']");
            if (xnd==null)
            {
                return false;
            }
            string CardStackState = xnd.InnerText;
            if (!"1".Equals(CardStackState))
            {
                return false;
            }

            return true;
        }

        public static bool moveCard()
        {
            

            return true;

        }

        /// <summary>
        /// 读取身份证
        /// </summary>
        /// <returns></returns>
        public static RegisterParam readIdCard()
        {
            RegisterParam register = new RegisterParam();
            string param = "<invoke name=\"SHENFENZHENG\"><arguments></arguments></invoke>";
            string hardWareResult = "";
           
            hardWareResult = getValue(param);
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
            register.name = xnd.InnerText;

            xnd = doc.SelectSingleNode("/return/arguments/string[@id='IDCARDNO']");
            if (xnd == null)
            {

                return null;
            }
            register.idCard = xnd.InnerText;

            xnd = doc.SelectSingleNode("/return/arguments/string[@id='SEX']");
            if (xnd == null)
            {

                return null;
            }
            register.sex = xnd.InnerText;

            xnd = doc.SelectSingleNode("/return/arguments/string[@id='BORN']");
            if (xnd == null)
            {

                return null;
            }
            register.birthday = xnd.InnerText;

            xnd = doc.SelectSingleNode("/return/arguments/string[@id='ADDRESS']");
            if (xnd == null)
            {

                return null;
            }
            register.address = xnd.InnerText;

            return register;
        }

        public static string readCard()
        {
            
           
          
            string result = getValue("<invoke name=\"READCARDREADRFCARD\"><arguments>" + $"<string id=\"SECTORNO\">0</string>" + $"<string id=\"BLOCKNO\">0</string>" + $"<string id=\"PASSWORD\">FFFFFFFFFFFF</string>" + "</arguments></invoke>");
            if (string.IsNullOrEmpty(result))
            {

                return null;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='ERROR']");
            if (xnd == null)
            {

                return null;
            }
            string card = xnd.InnerText;

            if (!"SUCCESS".Equals(card))
            {

                return null;
            }
            xnd = doc.SelectSingleNode("/return/arguments/string[@id='CARDNO']");
            if (xnd == null)
            {

                return null;
            }
           
            
            return xnd.InnerText;

        }

        public static void allowCard()
        {
            string param = "<invoke name=\"READCARDALLOWCARDIN\"><arguments></arguments></invoke>";
            string result = getValue(param);
           
        }


        public static string sendOutCard()
        {
            return getValue("<invoke name=\"CARDSENDEROUTCARD\"><arguments></arguments></invoke>");

        }

        public static string sendOutCard1()
        {
            return getValue("<invoke name=\"CARDSENDEROUTCARD\"><arguments></arguments></invoke>");

        }

        public static void outCard()
        {
           
            string result = getValue("<invoke name=\"READCARDOUTCARD\"><arguments></arguments></invoke>");
        }

        public static string sendMoveCard1()
        {
            string result = getValue("<invoke name=\"CARDSENDERMOVECARDTORF\"><arguments></arguments></invoke>");
            return result;
        }
        /// <summary>
        /// 发卡器移动卡到射频卡位
        /// </summary>
        public static bool sendMoveCard()
        {
            
            string result = getValue("<invoke name=\"CARDSENDERMOVECARDTORF\"><arguments></arguments></invoke>");
            if (string.IsNullOrEmpty(result))
            {

                return false;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='ERROR']");
            if (xnd == null)
            {

                return false;
            }
            string card = xnd.InnerText;

            if (!"SUCCESS".Equals(card))
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// 发卡器读取射频卡
        /// </summary>
        public static string sendReadCard()
        {
            string result = getValue("<invoke name=\"CARDSENDERREADRFCARD\"><arguments>" + $"<string id=\"BLOCKADR\">0</string>" + $"<string id=\"NADR\">0</string>" + $"<string id=\"PASSWORD\">FFFFFFFFFFFF</string>" + "</arguments></invoke>");
            if (string.IsNullOrEmpty(result))
            {

                return null;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='ERROR']");
            if (xnd == null)
            {

                return null;
            }
            string card = xnd.InnerText;

            if (!"SUCCESS".Equals(card))
            {

                return null;
            }
            xnd = doc.SelectSingleNode("/return/arguments/string[@id='CARDNO']");
            if (xnd == null)
            {

                return null;
            }


            return xnd.InnerText;
        }

        public static string recycle()
        {
           
               
            return getValue("<invoke name=\"CARDSENDERRECYCLE\"><arguments></arguments></invoke>");
        }
       
    }
}
