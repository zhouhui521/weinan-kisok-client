using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.pay.model;
using KioskApp.pay.service;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ZXing.QrCode;

namespace KioskApp.pay
{
    public partial class FormQrCode : MaterialForm
    {
      

        public PayParam payParam { get; set; }

        public Response<TransactionData> transData { get; set; }

        private readonly MaterialSkinManager materialSkinManager;


        public string qrCodeUrl { get; set; }

        public FormQrCode()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void FormQrCode_Load(object sender, EventArgs e)
        {
            PayService payService = new PayService();
            Response<TransactionData> response = new Response<TransactionData>();
            string profile = AppConfigUtil.get("profile");
            if ("dev".Equals(profile))
            {
                TransactionData transaction = new TransactionData();
                transaction.scanCode = "https://qr.95516.com/01030000/03260171926140000000651011165700127593";
                response.data = transaction;
                response.code = "200";
            }
            else
            {
                response = payService.getQrCode(payParam);
            }
            
           
            if (!"200".Equals(response.code))
            {
                MessageBox.Show(response.message);
                DialogResult = DialogResult.Cancel;
                return;
            }
            transData = response;
            payParam.orderNo = response.data.merTradeNo;
            string qrCode = response.data.scanCode;
            qrCodeUrl = qrCode;
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
            options.DisableECI = true;
            //设置内容编码
            options.CharacterSet = "UTF-8";
            //设置二维码的宽度和高度
            options.Width = 200;
            options.Height = 200;
            //设置二维码的边距,单位不是固定像素
            options.Margin = 1;
            writer.Options = options;

            Bitmap map = writer.Write(qrCode);
            string fileSavePath = AppDomain.CurrentDomain.BaseDirectory + "EWM.jpg";
            this.pictureBox1.Image = map;
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
