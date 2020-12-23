using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.pay;
using KioskApp.pay.model;
using KioskApp.prestore;
using KioskApp.prestore.model;
using KioskApp.register;
using KioskApp.register.model;
using KioskApp.sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskApp
{
    public partial class Form3 : Form
    {

        public string printContent { get; set; }
        public Form3()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 门诊充值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Patient patient = new Patient();
            HardWareService.getValue("<invoke name=\"READCARDALLOWCARDIN\"><arguments></arguments></invoke>");
            string url = AppConfigUtil.get("url");
            using (FormNewInputCard formNewInputCard = new FormNewInputCard())
            {
                DialogResult dialogResult = formNewInputCard.ShowDialog();
                if (dialogResult!=DialogResult.OK)
                {
                    return;
                }
            }


            string result = HardWareService.getValue("<invoke name=\"READCARDREADRFCARD\"><arguments>" + $"<string id=\"SECTORNO\">0</string>" + $"<string id=\"BLOCKNO\">0</string>" + $"<string id=\"PASSWORD\">FFFFFFFFFFFF</string>" + "</arguments></invoke>");
            if (string.IsNullOrEmpty(result))
            {

                MessageBox.Show("未读取到就诊卡");
                return;
            }

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='ERROR']");
            if (xnd == null)
            {

                MessageBox.Show("未读取到就诊卡:"+ result);
                return;
            }
            string card = xnd.InnerText;

            if (!"SUCCESS".Equals(card))
            {

                MessageBox.Show("未读取到就诊卡:" + result);
                return;
            }
            xnd = doc.SelectSingleNode("/return/arguments/string[@id='CARDNO']");
            if (xnd == null)
            {

                MessageBox.Show("未读取到就诊卡:" + result);
                return;
            }


            string cardNo = xnd.InnerText;

            if (cardNo.Length > 32)
            {
                cardNo = cardNo.Substring(0, 32);
            }
            result = HttpRestTemplate.postForString(cardNo, url + "api/patient/index");

            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("未查询到患者信息");
                return;
            }

            Response<Patient> responsePatient = JsonConvert.DeserializeObject<Response<Patient>>(result);//反序列化
            
            patient = responsePatient.data;
            //主界面显示姓名和余额
            label1.Text = "姓名：" + responsePatient.data.name;
            label2.Text = "余额：" + responsePatient.data.balance;

            string payType = "";
            using (FormNewPayChoose formNewPayChoose = new FormNewPayChoose())
            {
                DialogResult dialogResult = formNewPayChoose.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    return;
                }
                payType = formNewPayChoose.payType;
            }

            if ("现金".Equals(payType))
            {
                result = HardWareService.getValue("<invoke name=\"BILLACCEPTOROPENPORT\"><arguments><string id=\"ID\">123456</string></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("打开钞箱端口失败");
                    return;
                }
                result = HardWareService.getValue("<invoke name=\"BILLACCEPTORALLOWCASHIN\"><arguments></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("允许进入钞失败");
                    return;
                }
                using (FormNewCashSave formNewCashSave = new FormNewCashSave())
                {
                    DialogResult dialogResult = formNewCashSave.ShowDialog();
                    if (dialogResult != DialogResult.OK)
                    {
                        return;
                    }
                }

                #region 获取存钞金额
                result = HardWareService.getValue("<invoke name=\"BILLACCEPTORSTACKMONEYDETAIL\"><arguments></arguments></invoke>");

                doc = new System.Xml.XmlDocument();
                doc.LoadXml(result);
                xnd = doc.SelectSingleNode("/return/arguments/string[@id='STACKMONEY']");
                if (xnd == null)
                {
                    MessageBox.Show("没有获取到存钞票明细");
                    return;
                }
                string cashResult = xnd.InnerText;
                int totalMoney = 0;
                int totalCount = 0;
                if (cashResult.Contains("|"))
                {
                    string[] cashList = cashResult.Split('|');

                    

                    for (int i = 0; i < cashList.Length; i++)
                    {
                        //获取每张明细
                        string cashDetail = cashList[i];

                        string money = cashDetail.Split(',')[0];
                        int intMoney = Convert.ToInt32(money);
                        totalMoney += intMoney;

                    }
                    totalCount = cashList.Length;
                    label3.Text = "已存放张数：" + cashList.Length+ ",已存放金额：" + totalMoney;

                }
                else
                {
                    totalCount = 1;
                    label3.Text = "已存放张数：1,已存放金额：" + totalMoney;
                }
                #endregion           
                CashDTO cashDTO = new CashDTO();
                cashDTO.cardNo = patient.cardNo;
                cashDTO.money = totalMoney + "";
                cashDTO.moneyCount = totalCount + "";
                string jsonData = JsonConvert.SerializeObject(cashDTO);
                result = HttpRestTemplate.postForString(jsonData, url + "api/pay/cashSave");
                Response<CashVO> response = JsonConvert.DeserializeObject<Response<CashVO>>(result);//反序列化
                if (response == null)
                {
                    MessageBox.Show("为获取到平台现金接口返回值");
                    return;
                }

                if (!"200".Equals(response.code))
                {
                    MessageBox.Show(response.message);
                    return;
                }
                printContent = "渭南市第一医院自助机凭条\n\n";
                printContent += "卡号:" + patient.cardNo + "\n\n";
                printContent += "姓名:" + patient.name + "\n\n";
                printContent += "充值金额:" + cashDTO.money + "\n\n";
                printContent += "充值流水号:" + response.data.hisSerialNumber + "\n\n";
                printContent += "余额:" + response.data.balance + "\n\n";
                printDocument1.Print();
                HardWareService.outCard();
                MessageBox.Show("充值成功");
                label1.Text = "姓名：";
                label2.Text = "余额：";
                printContent = "";
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            printDocument1.DefaultPageSettings.Margins.Top = 0;
            printDocument1.DefaultPageSettings.Margins.Bottom = 0;
            printDocument1.DefaultPageSettings.Margins.Left = 0;
            printDocument1.DefaultPageSettings.Margins.Right = 0;
            //下面这个也设成高质量
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //下面这个设成High
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.HasMorePages = false;

            string str = "";
            //下面这个也设成高质量
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //下面这个设成High
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.HasMorePages = false;
            Font titleFont = new Font("黑体", 16, System.Drawing.FontStyle.Bold);//标题字体           
            Font titleFont2 = new Font("黑体", 14, System.Drawing.FontStyle.Bold);//标题字体           
            Font fntTxt = new Font("黑体", 12, System.Drawing.FontStyle.Regular);//正文文字         
            Font fntTxt1 = new Font("黑体", 10, System.Drawing.FontStyle.Regular);//正文文字           
            System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.Black);//画刷           
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);//线条颜色         

            try
            {

                //printDocument5.DefaultPageSettings.PaperSize.Width = 299;//请注意，这里是单位：（百分之一英寸）
                //printDocument5.DefaultPageSettings.PaperSize.Height = 100;//请注意，这里是单位：（百分之一英寸）
                //e.PageSettings.PaperSize.Width = 299;//请注意，这里是单位：（百分之一英寸）
                //e.PageSettings.PaperSize.Height = 30;//请注意，这里是单位：（百分之一英寸）
                e.Graphics.DrawString($"{printContent}", fntTxt, brush, new System.Drawing.Point(26, 58));

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }


        /// <summary>
        /// 门诊办卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            string url = AppConfigUtil.get("url");
            RegisterParam registerParam = new RegisterParam();
            string profile = AppConfigUtil.get("profile");
            using (FormPutIdCard formPutIdCard = new FormPutIdCard())
            {
                DialogResult dialogResult = formPutIdCard.ShowDialog();
                if (dialogResult!=DialogResult.OK)
                {
                    return;
                }
            }
            #region 读身份证
            registerParam = HardWareService.readIdCard();
            MessageBox.Show(registerParam.name);
            #endregion
            using (FormInputPhone formInputPhone = new FormInputPhone())
            {
                formInputPhone.registerParam = registerParam;
                DialogResult dialogResult = formInputPhone.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    return;
                }
                registerParam.phone = formInputPhone.phone;
            }
            bool move = HardWareService.sendMoveCard();
            if (!move)
            {
                MessageBox.Show("移动卡到射频位失败");

                return;
            }
            string cardNo = HardWareService.sendReadCard();
            if (string.IsNullOrEmpty(cardNo))
            {
                MessageBox.Show("未读到卡号");
           
                HardWareService.recycle();
                return;
            }

            registerParam.cardNo = cardNo;

            # region 注册
            //检测是否办过卡
            string jsonData = JsonConvert.SerializeObject(registerParam);
            string result = HttpRestTemplate.postForString(jsonData, url + "api/register");
            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("HIS平台没有返回值");
                return;
            }
            Response<RegisterParam> responseRegisterParam = JsonConvert.DeserializeObject<Response<RegisterParam>>(result);//反序列化
            if (responseRegisterParam==null)
            {
                MessageBox.Show("HIS平台没有返回值");
                return;
            }
            registerParam = responseRegisterParam.data;
            #endregion
            string payType = "";
            using (FormPayNewChoose formPayNewChoose = new FormPayNewChoose())
            {
                DialogResult dialogResult = formPayNewChoose.ShowDialog();
                if (dialogResult!=DialogResult.OK)
                {
                    HardWareService.recycle();
                    HttpRestTemplate.postForString(cardNo, url + "api/patient/delete");
                }
                payType = formPayNewChoose.payType;
            }
            PayParam payParam = new PayParam();
            payParam.payType = payType;
            payParam.cardNo = registerParam.cardNo;
            string money = "";
            string saveMoney = "";
            using (FormNewInputMoney formNewInputMoney = new FormNewInputMoney())
            {
                DialogResult dialogResult = formNewInputMoney.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    HardWareService.recycle();
                    HttpRestTemplate.postForString(cardNo, url + "api/patient/delete");
                }
                money = formNewInputMoney.money;
                saveMoney = formNewInputMoney.money;
            }

            if ("银联".Equals(payType))
            {
                int intMoney = Convert.ToInt32(money);

                string machineId = "01001";
                string operatorId = "123456";
                string transType = "C";
                money = (intMoney * 100) + "";
                payType = "001";

                string strInput = machineId.PadRight(10);
                strInput += operatorId.PadRight(10);
                strInput += transType;
                strInput += money.PadLeft(12, '0');
                strInput += payType.PadRight(6);

                StringBuilder sbInput = new StringBuilder(strInput);
                StringBuilder sbOutPut = new StringBuilder(150);


                int a = BankDll.CreditTrans(sbInput, sbOutPut);
                result = sbOutPut.ToString();

                BankResult bankResult = new BankResult();
                bankResult.responseCode = result.Substring(0, 2);
                bankResult.bankCardNo = result.Substring(2, 19);
                bankResult.transType = result.Substring(20, 1);
                bankResult.money = result.Substring(21, 12);
                bankResult.bankCode = result.Substring(34, 4);
                bankResult.saveMoney = money;
                bankResult.cardNo = cardNo;

                if (!"00".Equals(bankResult.responseCode))
                {
                    MessageBox.Show("银行卡交易失败");
                    HardWareService.outCard();
                    label1.Text = "姓名：";
                    label2.Text = "余额：";
                    printContent = "";
                    return;
                }
                jsonData = JsonConvert.SerializeObject(bankResult);
                string serverResult = HttpRestTemplate.postForString(jsonData, url + "api/pay/bankSaveHos");
                Response<BankVO> response = JsonConvert.DeserializeObject<Response<BankVO>>(serverResult);//反序列化
                if (response == null)
                {
                    MessageBox.Show("为获取到平台现金接口返回值");
                    HardWareService.outCard();
                    label1.Text = "姓名：";
                    label2.Text = "余额：";
                    printContent = "";
                    return;
                }

                if (!"200".Equals(response.code))
                {
                    MessageBox.Show(response.message);
                    return;
                }

                printContent = "渭南市第一医院自助机凭条\n\n";
                printContent += "卡号:" + cardNo + "\n\n";
                printContent += "姓名:" + registerParam.name + "\n\n";
                printContent += "充值金额:" + saveMoney + "\n\n";
                printContent += "充值流水号:" + response.data.hisSerialNumber + "\n\n";
                printContent += "余额:" + response.data.balance + "\n\n";
                printDocument1.Print();

                HardWareService.outCard();
                MessageBox.Show("充值成功");
                label1.Text = "姓名：";
                label2.Text = "余额：";
                printContent = "";
                return;
            }
        }
    }
}
