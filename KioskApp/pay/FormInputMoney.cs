using KioskApp.pay.model;
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

namespace KioskApp.pay
{
    public partial class FormInputMoney : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public PayParam payParam { get; set; }

        public FormInputMoney()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void materialButton3_Click(object sender, EventArgs e)
        {
            MaterialButton materialButton = (MaterialButton)sender;
            string text = materialButton.Text;
            materialTextBoxPhone.Text += text;
        }

        private void materialButton13_Click(object sender, EventArgs e)
        {

            payParam.money = materialTextBoxPhone.Text;
            DialogResult = DialogResult.OK;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            materialTextBoxPhone.Text = "";
        }
    }
}
