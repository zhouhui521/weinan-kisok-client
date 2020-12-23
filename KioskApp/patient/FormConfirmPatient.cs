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

namespace KioskApp.patient
{
    public partial class FormConfirmPatient : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public string name { get; set; }

        public string sex { get; set; }

        public string age { get; set; }

        public string hosId { get; set; }
        public FormConfirmPatient()
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
            DialogResult = DialogResult.OK;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void FormConfirmPatient_Load(object sender, EventArgs e)
        {
            label1.Text = "姓名：" + name;

            label2.Text = "性别：" + sex;

            label3.Text = "年龄：" + age;

            label4.Text = "住院号：" + hosId;

        }
    }
}
