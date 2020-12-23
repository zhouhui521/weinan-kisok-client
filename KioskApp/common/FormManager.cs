using KioskApp.common.utils;
using KioskApp.pay;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioskApp.common
{
    public partial class FormManager : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public FormManager()
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
            if (AppConfigUtil.get("password").Equals(materialTextBoxPhone.Text))
            {
                materialButton1.Enabled = true;

               
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            HardWareService.getValue("<invoke name=\"LOCKOPEN\"><arguments><string id=\"INDEX\">2</string></arguments></invoke>");
        }

        /// <summary>
        /// 签到
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void materialButton14_Click(object sender, EventArgs e)
        {
            string machineId = "01001";
            string operatorId = "123456";
            string transType = "N";
            string money = "1";
            string payType = "001";

            string strInput = machineId.PadRight(10);
            strInput += operatorId.PadRight(10);
            strInput += transType;
            strInput += money.PadLeft(12, '0');
            strInput += payType.PadRight(6);

            StringBuilder sbInput = new StringBuilder(strInput);
            StringBuilder sbOutPut = new StringBuilder(150);

            int result = BankDll.CreditTrans(sbInput, sbOutPut);
        }
    }
}
