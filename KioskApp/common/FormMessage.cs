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

namespace KioskApp.common
{
    public partial class FormMessage : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public string content { get; set; }

        public FormMessage()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = content;
            if (content.Equals("交易成功,请取走您的就诊卡"))
            {
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (var speechSyn = new SpeechSynthesizer())
            {
                speechSyn.Speak("请在发卡口取走您的就诊卡");
            }
        }
    }
}
