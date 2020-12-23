using KioskApp.common.http;
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

namespace KioskApp.pay
{
    public partial class FormPayChoose : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;
        public FormPayChoose()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        public PayParam payParam;

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            payParam.payType = "微信";

            this.DialogResult = DialogResult.OK;
            

        }

        /// <summary>
        /// 现金充值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            payParam.payType = "现金";
            this.DialogResult = DialogResult.OK;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            payParam.payType = "银联";
            this.DialogResult = DialogResult.OK;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            payParam.payType = "支付宝";

            this.DialogResult = DialogResult.OK;
        }
    }
}
