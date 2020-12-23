using KioskApp.common.model;
using KioskApp.common.utils;
using KioskApp.prestore.model;
using KioskApp.prestore.service;
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

namespace KioskApp.prestore
{
    public partial class FormInputCard : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public Patient patient { get; set; }

        public FormInputCard()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            PrestoreService prestoreService = new PrestoreService();
            materialButton1.Enabled = false;


            Response<Patient> patientResponse = new Response<Patient>();


            Response<string> response = prestoreService.readCard();

            if (response==null)
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            if ("500".Equals(response.code))
            {
                MessageBox.Show(response.message);
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            patientResponse = prestoreService.getPatient(response.data);
            if (patientResponse!=null&&"200".Equals(patientResponse.code))
            {
                this.DialogResult = DialogResult.OK;
                patient = patientResponse.data;
                return;
            }
            else if(patientResponse!=null)
            {
                MessageBox.Show(patientResponse.message);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            else
            {
                MessageBox.Show("未获取到患者信息");
                this.DialogResult = DialogResult.Cancel;
                return;
            }
            
            
        }

        private void FormInputCard_Load(object sender, EventArgs e)
        {
            label1.Text = "请将就诊卡放入插卡口并点击确认";
            backgroundWorker1.RunWorkerAsync();
            //打开读卡器允许进卡
            string profile = AppConfigUtil.get("profile");
            if (!"dev".Equals(profile))
            {
                HardWareService.allowCard();
            }
            

        }

        /// <summary>
        /// 语音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var speechSyn = new SpeechSynthesizer())
            {
                speechSyn.Speak("请将就诊卡插入读卡器并点击确定");
            }
            HardWareService.getValue("<invoke name=\"DOORLIGHTOPEN\"><arguments><string id=\"INDEX\">1</string></arguments></invoke>");
        }
    }
}
