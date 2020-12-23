using KioskApp.common;
using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.patient;
using KioskApp.pay;
using KioskApp.pay.model;
using KioskApp.prestore;
using KioskApp.prestore.model;
using KioskApp.register;
using KioskApp.register.model;
using KioskApp.register.service;
using KioskApp.sdk;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Win32;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskApp
{
    public partial class Form2 : Form
    {
        

        /// <summary>
        /// 小票内容
        /// </summary>
        public string printContent { get; set; }

        public Form2()
        {
          
            InitializeComponent();
            
        }

        /// <summary>
        /// 办卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            FormMessage formMessage = new FormMessage();
            RegisterService registerService = new RegisterService();

            //确认读取身份证信息
            FormMessageIdCard formMessageIdCard = new FormMessageIdCard();
            DialogResult dialogResult = formMessageIdCard.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                
                return;

            }

            //显示身份证信息、输入电话
            RegisterParam registerParam = formMessageIdCard.registerParam;
            FormMessageIdCardInfo messageIdCardInfo = new FormMessageIdCardInfo();
            messageIdCardInfo.registerParam = registerParam;
            dialogResult = messageIdCardInfo.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
               
                return;

            }

            #region 注册患者
            string profile = AppConfigUtil.get("profile");
            string cardNo = "";
            if ("dev".Equals(profile))
            {
                cardNo = "2c7d3d09650804006263646566676869";
            }
            else
            {
             
                bool move = HardWareService.sendMoveCard();
                if (!move)
                {
                    formMessage.Text = "发卡器异常";
                    formMessage.content = "移动卡到射频位失败";
                    formMessage.ShowDialog();
                 
                    return;
                }



                cardNo = HardWareService.sendReadCard();
                if (string.IsNullOrEmpty(cardNo))
                {
                    formMessage.Text = "发卡器读卡失败";
                    formMessage.content = "未读到卡号";
                    formMessage.ShowDialog();
                   
                    HardWareService.recycle();
                    return;
                }
            }


            registerParam.cardNo = cardNo;

            Response<RegisterParam> response = registerService.makeCard(registerParam);

            if (response == null)
            {
                HardWareService.recycle();
                formMessage.Text = "注册失败";
                formMessage.content = "HIS注册接口未返回信息";
                formMessage.ShowDialog();
              
                return;
            }

            if (!"200".Equals(response.code))
            {
                HardWareService.recycle();

                formMessage.Text = "注册失败";
                formMessage.content = response.message;
                formMessage.ShowDialog();
               
               
               
                return;
            }
            registerParam = response.data;
            #endregion

            //registerParam = messageIdCardInfo.registerParam;

            //选择支付方式
            FormPayChoose formPayChoose = new FormPayChoose();

            PayParam payParam = new PayParam();
            payParam.cardNo = registerParam.cardNo;
            formPayChoose.payParam = payParam;
            dialogResult = formPayChoose.ShowDialog();
            if (dialogResult!=DialogResult.OK)
            {
                if (!"dev".Equals(profile))
                {
                    HardWareService.recycle();
                }
                    
                registerService.deleteCard(payParam.cardNo);
                return;
            }
            payParam = formPayChoose.payParam;


            if ("现金".Equals(payParam.payType))
            {
                string result1 = HardWareService.getValue("<invoke name=\"BILLACCEPTOROPENPORT\"><arguments><string id=\"ID\">123456</string></arguments></invoke>");
                if (result1.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("打开钞箱端口失败");

                    return;
                }

                result1 = HardWareService.getValue("<invoke name=\"BILLACCEPTORALLOWCASHIN\"><arguments></arguments></invoke>");
                if (result1.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("允许进入钞失败");

                    return;
                }
                Patient patient = new Patient();
                //办卡现金充值

                patient.name = registerParam.name;
                patient.cardNo = registerParam.cardNo;

                cashSave(patient);
                HardWareService.getValue("<invoke name =\"BILLACCEPTORCLOSEPORT\"><arguments></arguments></invoke>");
                HardWareService.sendOutCard();
                return;
            }


            //输入金额
            FormInputMoney formInputMoney = new FormInputMoney();
            formInputMoney.payParam = payParam;
            dialogResult = formInputMoney.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                if (!"dev".Equals(profile))
                {
                    HardWareService.recycle();
                }
                registerService.deleteCard(payParam.cardNo);
                return;
            }
            if (!string.IsNullOrEmpty(payParam.money))
            {
                int money = Convert.ToInt32(payParam.money);

                if (money<10)
                {
                    MessageBox.Show("办卡充值金额需大于10元");
                    return;
                }
            }

            if ("银联".Equals(payParam.payType))
            {
                bankSave(payParam.money, registerParam.cardNo,registerParam.name);
                HardWareService.sendOutCard();
                return;
            }





            payParam = formInputMoney.payParam;

            FormQrCode formQrCode = new FormQrCode();
            formQrCode.payParam = payParam;
            
            dialogResult = formQrCode.ShowDialog();
            string qrCode = formQrCode.qrCodeUrl;
            if (dialogResult != DialogResult.OK)
            {
                if (!"dev".Equals(profile))
                {
                    HardWareService.recycle();
                }
                registerService.deleteCard(payParam.cardNo);
                formMessage.Text = "提示";
                formMessage.content = "交易失败";
                formMessage.ShowDialog();
                return;
            }
            payParam = formQrCode.payParam;
            FormQueryResult formQueryResult = new FormQueryResult();
            formQueryResult.qrCode = qrCode;
            formQueryResult.payParam = payParam;
            dialogResult = formQueryResult.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                if (!"dev".Equals(profile))
                {
                    HardWareService.recycle();
                }
                registerService.deleteCard(payParam.cardNo);
                formMessage.Text = "提示";
                formMessage.content = "交易失败";
                formMessage.ShowDialog();
                return;
            }


            //打印小票
            printContent = "渭南市第一医院自助机凭条\n\n";
            printContent += "卡号:" + payParam.cardNo + "\n\n";
            printContent += "姓名:" + registerParam.name + "\n\n";
            printContent += "充值金额:" + payParam.money + "\n\n";
            printContent += "充值流水号:" + formQueryResult.transData.data.merTradeNo + "\n";
           
            printContent += "1、此凭据不能用作报销，\n\n仅做预交结算使用\n\n";
            printContent += "2、卡片押金5元，退卡时退还\n\n";
            printDocument1.Print();

            string result = HardWareService.getValue("<invoke name=\"CARDSENDEROUTCARD\"><arguments></arguments></invoke>");
            formMessage.Text = "提示";
            formMessage.content = "交易成功,请取走您的就诊卡";
            formMessage.ShowDialog();


        }


        /// <summary>
        /// 门诊充值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox3_Click(object sender, EventArgs e)
        {

            string profile = AppConfigUtil.get("profile");
            HardWareService.getValue("<invoke name=\"READCARDALLOWCARDIN\"><arguments></arguments></invoke>");



            FormInputCard formInputCard = new FormInputCard();
            DialogResult dialogResult = formInputCard.ShowDialog();
            if (dialogResult!=DialogResult.OK)
            {
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }
          

            Patient patient = new Patient();
            patient = formInputCard.patient;
            if (patient==null)
            {
                MessageBox.Show("未获取到患者信息");
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }
            //主界面显示姓名和余额
            label1.Text = "姓名：" + patient.name;
            label2.Text = "余额：" + patient.balance;

            //选择支付方式
            FormPayChoose formPayChoose = new FormPayChoose();
            PayParam payParam = new PayParam();
            payParam.cardNo = patient.cardNo;
            formPayChoose.payParam = payParam;
            dialogResult = formPayChoose.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }
            payParam = formPayChoose.payParam;


            if ("现金".Equals(payParam.payType))
            {
                string result = HardWareService.getValue("<invoke name=\"BILLACCEPTOROPENPORT\"><arguments><string id=\"ID\">1</string></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("打开钞箱端口失败");
                    HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                    return;
                }

                result = HardWareService.getValue("<invoke name=\"BILLACCEPTORALLOWCASHIN\"><arguments></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    HardWareService.outCard();
                    MessageBox.Show("允许进入钞失败");
                    HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                    return;
                }

                cashSave(patient);

                //关闭纸币器端口
                HardWareService.getValue("<invoke name =\"BILLACCEPTORCLOSEPORT\"><arguments></arguments></invoke>");

                using (var speechSyn = new SpeechSynthesizer())
                {
                    speechSyn.Speak("请在发卡口取走就诊卡");
                }

                //关闭读卡口灯
                HardWareService.getValue("<invoke name=\"DOORLIGHTCLOSE\"><arguments><string id=\"INDEX\">1</string></arguments></invoke>");
                HardWareService.outCard();
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }


            

            //输入金额
            FormInputMoney formInputMoney = new FormInputMoney();
            formInputMoney.payParam = payParam;
            dialogResult = formInputMoney.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }

            if (Convert.ToInt32(payParam.money)<10)
            {
                MessageBox.Show("最低充值10元");
                return;
            }

            payParam = formInputMoney.payParam;



            if ("银联".Equals(payParam.payType))
            {
               
                bankSave(payParam.money, patient.cardNo, patient.name);
                
                using (var speechSyn = new SpeechSynthesizer())
                {
                    speechSyn.Speak("请在读卡器口取走您的就诊卡");
                }
                HardWareService.outCard();
                //关闭读卡口灯
                HardWareService.getValue("<invoke name=\"DOORLIGHTCLOSE\"><arguments><string id=\"INDEX\">1</string></arguments></invoke>");
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;

            }



            //获取支付二维码
            FormQrCode formQrCode = new FormQrCode();
            formQrCode.payParam = payParam;
            dialogResult = formQrCode.ShowDialog();
            string qrCode = formQrCode.qrCodeUrl;
            if (dialogResult != DialogResult.OK)
            {
                MessageBox.Show("交易失败");
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }
            payParam = formQrCode.payParam;
            Response<TransactionData> transData = new Response<TransactionData>();
            transData = formQrCode.transData;
            //if (transData==null|| transData.data==null)
            //{
            //    MessageBox.Show("未获取到交易结果信息");
            //    return;
            //}
            
            //查询交易结果
            FormQueryResult formQueryResult = new FormQueryResult();
            formQueryResult.payParam = payParam;
            formQueryResult.transData = transData;
            formQueryResult.qrCode = qrCode;
            dialogResult = formQueryResult.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                MessageBox.Show("交易失败");
                HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");
                return;
            }
            //打印小票
            printContent = "渭南市第一医院自助机凭条\n\n";
            printContent += "卡号:"+ payParam.cardNo+"\n\n";
            printContent += "姓名:" + patient.name + "\n\n";
            printContent += "充值金额:" + payParam.money + "\n\n";
            printContent += "充值流水号:" + formQueryResult.transData.data.merTradeNo + "\n";
            printDocument1.Print();

            
            MessageBox.Show("交易成功，请取走您的就诊卡");
            HardWareService.outCard();
            label1.Text = "姓名：";
            label2.Text = "余额：";
            printContent = "";
            HardWareService.getValue("<invoke name=\"READCARDNOTALLOWCARDIN\"><arguments></arguments></invoke>");

        }


        /// <summary>
        /// 银行交易
        /// </summary>
        /// <param name="inMoney"></param>
        /// <param name="cardNo"></param>
        private void bankSave(string inMoney,string cardNo,string name)
        {
            string url = AppConfigUtil.get("url");
            int intMoney = Convert.ToInt32(inMoney);

            string machineId = "01001";
            string operatorId = "123456";
            string transType = "C";
            string money = (intMoney * 100) + "";
            string payType = "001";

            string strInput = machineId.PadRight(10);
            strInput += operatorId.PadRight(10);
            strInput += transType;
            strInput += money.PadLeft(12, '0');
            strInput += payType.PadRight(6);

            StringBuilder sbInput = new StringBuilder(strInput);
            StringBuilder sbOutPut = new StringBuilder(150);


            int a = BankDll.CreditTrans(sbInput, sbOutPut);
            string result = sbOutPut.ToString();

            BankResult bankResult = new BankResult();
            bankResult.responseCode = result.Substring(0, 2);
            bankResult.bankCardNo = result.Substring(2, 19);
            bankResult.transType = result.Substring(20, 1);
            bankResult.money = result.Substring(21, 12);
            bankResult.bankCode = result.Substring(34, 4);
            bankResult.saveMoney = inMoney;
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
            string jsonData = JsonConvert.SerializeObject(bankResult);
            string serverResult = HttpRestTemplate.postForString(jsonData, url + "api/pay/bankSave");
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
            MessageBox.Show("充值成功");
            printContent = "渭南市第一医院自助机凭条\n\n";
            printContent += "卡号:" + cardNo + "\n\n";
            printContent += "姓名:" + name + "\n\n";
            printContent += "充值金额:" + inMoney + "\n\n";
            printContent += "充值流水号:" + response.data.hisSerialNumber + "\n\n";
            printContent += "余额:" + response.data.balance + "\n\n";
          
            printContent += "1、此凭据不能用作报销，\n\n仅做预交结算使用\n\n";
            printContent += "2、卡片押金5元，退卡时退还\n\n";
            printDocument1.Print();

            
            label1.Text = "姓名：";
            label2.Text = "余额：";
            printContent = "";

        }

        /// <summary>
        /// 现金充值
        /// </summary>
        private void cashSave(Patient patient)
        {
            //打开现金充值界面
            FormCashSave formCashSave = new FormCashSave();
            formCashSave.patient = patient;
            DialogResult dialogResult = formCashSave.ShowDialog();
            if (dialogResult!=DialogResult.OK)
            {
                HardWareService.getValue("<invoke name =\"BILLACCEPTORCLOSEPORT\"><arguments></arguments></invoke>");
                return;
            }
            //获取张数和金额
            int totalCount = formCashSave.totalCount;
            int money = formCashSave.allMoney;
            //调用his充值接口
            string url = AppConfigUtil.get("url");
            CashDTO cashDTO = new CashDTO();
            cashDTO.cardNo = patient.cardNo;
            cashDTO.money = money + "";
            cashDTO.moneyCount = totalCount + "";
            if(money<10){
                MessageBox.Show("办卡充值金额需大于10元");
                return;
            }
            string jsonData = JsonConvert.SerializeObject(cashDTO);
            string result = HttpRestTemplate.postForString(jsonData, url + "api/pay/cashSave");
            Response<CashVO> response = JsonConvert.DeserializeObject<Response<CashVO>>(result);//反序列化
            if (response==null)
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
         
            printContent += "1、此凭据不能用作报销，\n\n仅做预交结算使用\n\n";
            printContent += "2、卡片押金5元，退卡时退还\n\n";
            printDocument1.Print();

            
            MessageBox.Show("充值成功");
            label1.Text = "姓名：";
            label2.Text = "余额：";
            printContent = "";

        }

        /// <summary>
        /// 现金住院充值
        /// </summary>
        /// <param name="patient"></param>
        private void cashSaveHos(Patient patient)
        {
            //打开现金充值界面
            FormCashSave formCashSave = new FormCashSave();
            formCashSave.patient = patient;
            DialogResult dialogResult = formCashSave.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            //获取张数和金额
            int totalCount = formCashSave.totalCount;
            int money = formCashSave.allMoney;
            //调用his充值接口
            string url = AppConfigUtil.get("url");
            CashDTO cashDTO = new CashDTO();
            cashDTO.cardNo = patient.cardNo;
            cashDTO.money = money + "";
            cashDTO.moneyCount = totalCount + "";
            string jsonData = JsonConvert.SerializeObject(cashDTO);
            string result = HttpRestTemplate.postForString(jsonData, url + "api/pay/cashSaveHos");
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
            printContent += "住院号:" + patient.cardNo + "\n\n";
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





        /// <summary>
        /// 退卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton1_Click(object sender, EventArgs e)
        {
            label1.Text = "姓名：";
            label2.Text = "余额：";
            printContent = "";
            HardWareService.outCard();
        }

        /// <summary>
        /// 住院预交金
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton2_Click(object sender, EventArgs e)
        {
            //读卡
            FormInputInHosId formInputCard = new FormInputInHosId();
            DialogResult dialogResult = formInputCard.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            string hosId = formInputCard.hosId;
            if (string.IsNullOrEmpty(hosId))
            {
                MessageBox.Show("请输入住院号");
                return;
            }
            PatientService patientService = new PatientService();

            string result = patientService.getHosPatient(hosId);

            if (string.IsNullOrEmpty(result))
            {
                MessageBox.Show("未获取到住院信息【"+ hosId + "】");
                return;
            }
            Response<PatientVO> response = JsonConvert.DeserializeObject<Response<PatientVO>>(result);//反序列化
            if (response==null)
            {
                MessageBox.Show("未获取到住院信息【" + hosId + "】");
                return;
            }

            if (!"200".Equals(response.code))
            {
                MessageBox.Show(response.message);
                return;
            }
            
            //确认患者信息
            FormConfirmPatient formConfirmPatient = new FormConfirmPatient();
            formConfirmPatient.name = response.data.name;

            formConfirmPatient.sex = response.data.sex;

            formConfirmPatient.age = response.data.age;

            formConfirmPatient.hosId = hosId;

            dialogResult = formConfirmPatient.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            //调用充值类型

            //选择支付方式
            FormNewPayChoose formPayChoose = new FormNewPayChoose();
            dialogResult = formPayChoose.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            //支付方式
            string payType = formPayChoose.payType;

            if ("现金".Equals(payType))
            {
                HardWareService.getValue("<invoke name=\"BILLACCEPTOROPENPORT\"><arguments><string id=\"ID\">123456</string></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    
                    MessageBox.Show("打开钞箱端口失败");

                    return;
                }

                result = HardWareService.getValue("<invoke name=\"BILLACCEPTORALLOWCASHIN\"><arguments></arguments></invoke>");
                if (result.Contains("DEVERROR"))
                {
                    
                    MessageBox.Show("允许进入钞失败");

                    return;
                }

                Patient patient = new Patient();
                patient.cardNo = hosId;
                patient.name = response.data.name; 
                cashSaveHos(patient);
                return;
            }





            //输入金额
            FormNewInputMoney formInputMoney = new FormNewInputMoney();
            dialogResult = formInputMoney.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }
            //充值金额
            string money = formInputMoney.money;

            if (Convert.ToInt32(money) < 100)
            {
                MessageBox.Show("最低充值100元");
                return;
            }


            if ("银联".Equals(payType))
            {
                bankSaveHos(money, hosId, response.data.name);
                return;
            }


            PayParam payParam = new PayParam();
            payParam.money = money;
            payParam.payType = payType;
            payParam.cardNo = hosId;
            //获取支付二维码
            FormQrCode formQrCode = new FormQrCode();
            formQrCode.payParam = payParam;
            dialogResult = formQrCode.ShowDialog();
            string qrCode = formQrCode.qrCodeUrl;
            if (dialogResult != DialogResult.OK)
            {
                MessageBox.Show("交易失败");
                return;
            }
            payParam = formQrCode.payParam;
            Response<TransactionData> transData = new Response<TransactionData>();
            //获取交易结果
            transData = formQrCode.transData;


            payParam.bizType = "住院预交金";
            payParam.hosId = hosId;
            FormQueryResult formQueryResult = new FormQueryResult();
            formQueryResult.payParam = payParam;
            formQueryResult.transData = transData;
            formQueryResult.qrCode = qrCode;
            dialogResult = formQueryResult.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                MessageBox.Show("交易失败");
                return;
            }

            MessageBox.Show("交易成功，请取走您的就诊卡");
            HardWareService.outCard();
        }

       
        /// <summary>
        /// 信息检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton3_Click(object sender, EventArgs e)
        {

            string url = AppConfigUtil.get("url")+"front";
            //try
            //{
            //    // 64位注册表路径
            //    var openKey = @"SOFTWARE\Wow6432Node\Google\Chrome";
            //    if (IntPtr.Size == 4)
            //    {
            //        // 32位注册表路径
            //        openKey = @"SOFTWARE\Google\Chrome";
            //    }
            //    RegistryKey appPath = Registry.LocalMachine.OpenSubKey(openKey);
            //    // 谷歌浏览器就用谷歌打开，没找到就用系统默认的浏览器
            //    // 谷歌卸载了，注册表还没有清空，程序会返回一个"系统找不到指定的文件。"的bug
            //    if (appPath != null)
            //    {
            //        var result = Process.Start("chrome.exe", " -kiosk " + url);
            //        if (result == null)
            //        {

            //        }
            //    }
            //    else
            //    {
            //        var result = Process.Start("chrome.exe", url);
            //        if (result == null)
            //        {

            //        }
            //    }
            //}
            //catch
            //{
            //    //出错调用IE

            //}
            Process.Start("iexplore.exe", " -k "+url);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //string machineId = "01001";
            //string operatorId = "123456";
            //string transType = "N";
            //string money = "1";
            //string payType = "001";

            //string strInput = machineId.PadRight(10);
            //strInput += operatorId.PadRight(10);
            //strInput += transType;
            //strInput += money.PadLeft(12, '0');
            //strInput += payType.PadRight(6);

            //StringBuilder sbInput = new StringBuilder(strInput);
            //StringBuilder sbOutPut = new StringBuilder(150);
           
            //int result = BankDll.CreditTrans(sbInput, sbOutPut);
           
        }

        //public static Hardware com = new Hardware();

        private void materialButton1_Click_1(object sender, EventArgs e)
        {
           


        }

        private void materialButton2_Click_1(object sender, EventArgs e)
        {
           
        }

        private void materialButton1_Click_2(object sender, EventArgs e)
        {
            string result = HardWareService.sendMoveCard1();
            MessageBox.Show(result);
        }

        private void materialButton2_Click_2(object sender, EventArgs e)
        {
            string result = HardWareService.sendOutCard1();
           
        }



        /// <summary>
        /// 打印小票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        private void bankSaveHos(string inMoney, string cardNo, string name)
        {
            string url = AppConfigUtil.get("url");
            int intMoney = Convert.ToInt32(inMoney);

            string machineId = "01001";
            string operatorId = "123456";
            string transType = "C";
            string money = (intMoney * 100) + "";
            string payType = "001";

            string strInput = machineId.PadRight(10);
            strInput += operatorId.PadRight(10);
            strInput += transType;
            strInput += money.PadLeft(12, '0');
            strInput += payType.PadRight(6);

            StringBuilder sbInput = new StringBuilder(strInput);
            StringBuilder sbOutPut = new StringBuilder(150);


            int a = BankDll.CreditTrans(sbInput, sbOutPut);
            string result = sbOutPut.ToString();

            BankResult bankResult = new BankResult();
            bankResult.responseCode = result.Substring(0, 2);
            bankResult.bankCardNo = result.Substring(2, 19);
            bankResult.transType = result.Substring(20, 1);
            bankResult.money = result.Substring(21, 12);
            bankResult.bankCode = result.Substring(34, 4);
            bankResult.saveMoney = inMoney;
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
            string jsonData = JsonConvert.SerializeObject(bankResult);
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
            printContent += "姓名:" + name + "\n\n";
            printContent += "充值金额:" + inMoney + "\n\n";
            printContent += "充值流水号:" + response.data.hisSerialNumber + "\n\n";
            printContent += "余额:" + response.data.balance + "\n\n";
            printDocument1.Print();

            HardWareService.outCard();
            MessageBox.Show("充值成功");
            label1.Text = "姓名：";
            label2.Text = "余额：";
            printContent = "";

        }

        private void materialButton1_Click_3(object sender, EventArgs e)
        {
           string result =  HardWareService.getValue("<invoke name=\"CARDSENDEROUTCARD\"><arguments></arguments></invoke>");

            MessageBox.Show(result);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            FormManager formManager = new FormManager();
            formManager.ShowDialog();
        }
    }
}
