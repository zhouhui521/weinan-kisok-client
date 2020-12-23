using KioskApp.common.http;
using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.pay.model;
using KioskApp.pay.service;
using KioskApp.sdk;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskApp.pay
{
    public partial class FormQueryResult : MaterialForm
    {

        private readonly MaterialSkinManager materialSkinManager;

        public Response<TransactionData> transData { get; set; }
        public PayParam payParam { get; set; }


        public string qrCode { get; set; }

      

        public FormQueryResult()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void FormQueryResult_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("获取到的qrcode：" + qrCode);
            PayService payService = new PayService();
            string url = AppConfigUtil.get("url");
            //HttpRestTemplate http = new HttpRestTemplate();
            //string result = http.doPost(qrCode, url + "api/pay/query");
            //MessageBox.Show("请求结果："+result);
            Response<TransactionData> response = payService.queryResult(qrCode);

            if (response==null)
            {
                MessageBox.Show("未查询到支付结果");
                DialogResult = DialogResult.Cancel;
                return;
            }

            if (!"200".Equals(response.code))
            {
                MessageBox.Show(response.message);
                DialogResult = DialogResult.Cancel;
                return;
            }
            transData = response;
            //充值
            label1.Text = "正在充值……";

            int a = 1;
          

            //调用充值接口
            //读取卡号
            if ("住院预交金".Equals(payParam.bizType))
            {
                
                Response<PayResultVO> saveResponse = payService.saveHos(response.data.merTradeNo);
                if (!"200".Equals(saveResponse.code))
                {
                    MessageBox.Show(saveResponse.message);
                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            else
            {
                Response<string> saveResponse = payService.save(response.data.merTradeNo);
                if (!"200".Equals(saveResponse.code))
                {
                    MessageBox.Show(saveResponse.message);
                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
            
            DialogResult = DialogResult.OK;
        }
    }
}
