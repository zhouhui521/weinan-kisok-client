using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.register.model;
using KioskApp.register.service;
using KioskApp.sdk;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskApp.register
{
    public partial class FormMessageIdCardInfo : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public FormMessageIdCardInfo()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        public RegisterParam registerParam;

        private void FormMessageIdCardInfo_Load(object sender, EventArgs e)
        {
            labIdCard.Text = registerParam.idCard;
            labName.Text = registerParam.name;
            labSex.Text = registerParam.sex;
            labNation.Text = registerParam.nation;
            labBrith.Text = registerParam.birthday;
            labAddress.Text = registerParam.address;
            backgroundWorker1.RunWorkerAsync();
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            MaterialButton materialButton = (MaterialButton)sender;
            string text = materialButton.Text;
            materialTextBoxPhone.Text += text;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            materialTextBoxPhone.Text = "";
        }


   
        private void materialButton13_Click(object sender, EventArgs e)
        {
            registerParam.phone = materialTextBoxPhone.Text;
            //string profile = AppConfigUtil.get("profile");
            //string cardNo = "";
            //if ("dev".Equals(profile))
            //{
            //    cardNo = "2c7d3d09650804006263646566676869";
            //}
            //else
            //{
            //    Hardware com = new Hardware();
            //    bool move = HardWareService.sendMoveCard();
            //    if (!move)
            //    {
            //        MessageBox.Show("移动卡到射频位失败");
            //        this.DialogResult = DialogResult.Cancel;
            //        return;
            //    }



            //    cardNo = HardWareService.sendReadCard();
            //    if (string.IsNullOrEmpty(cardNo))
            //    {
            //        MessageBox.Show("发卡器未读到卡号");
            //        this.DialogResult = DialogResult.Cancel;
            //        HardWareService.recycle();
            //        return;
            //    }
            //}


            //registerParam.cardNo = cardNo;

            //RegisterService registerService = new RegisterService();

            //Response<RegisterParam>  response = registerService.makeCard(registerParam);

            //if (response == null)
            //{
            //    MessageBox.Show("HIS注册接口未返回信息");
            //    this.DialogResult = DialogResult.Cancel;
            //    HardWareService.recycle();
            //    return;
            //}

            //if (!"200".Equals(response.code))
            //{
            //    MessageBox.Show(response.message);
            //    this.DialogResult = DialogResult.Cancel;
            //    HardWareService.recycle();
            //    return;
            //}
            //registerParam = response.data;
            this.DialogResult = DialogResult.OK;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var speechSyn = new SpeechSynthesizer())
            {
                speechSyn.Speak("请输入您的手机号码");
            }
            
        }
    }
}
