using KioskApp.common.model;
using KioskApp.register.model;
using KioskApp.register.service;
using KioskApp.sdk;
using MaterialSkin;
using MaterialSkin.Controls;
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
    public partial class FormMessageIdCard : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public FormMessageIdCard()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        public RegisterParam registerParam { get; set; }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ////发送身份证读取请求
                RegisterService registerService = new RegisterService();
                //Response<RegisterParam> response = registerService.readIdCard();
                //if (response == null)
                //{
                //    MessageBox.Show("未取得返回信息");
                //    this.DialogResult = DialogResult.Cancel;
                //    return;
                //}

                //if (!"200".Equals(response.code))
                //{
                //    MessageBox.Show(response.message);
                //    this.DialogResult = DialogResult.Cancel;
                //    return;
                //}
                registerParam = registerService.readIdCard();
                if (registerParam != null)
                {
                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    MessageBox.Show("未检测到身份证信息");
                    this.DialogResult = DialogResult.Cancel;
                    return;
                }
                
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
              
            }
            //关闭身份证灯光
            HardWareService.getValue("<invoke name=\"DOORLIGHTCLOSE\"><arguments><string id=\"INDEX\">6</string></arguments></invoke>");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var speechSyn = new SpeechSynthesizer())
            {
                speechSyn.Speak("请放置您的身份证");
            }
            //打开身份证灯光
            HardWareService.getValue("<invoke name=\"DOORLIGHTOPEN\"><arguments><string id=\"INDEX\">6</string></arguments></invoke>");
        }

        private void FormMessageIdCard_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
