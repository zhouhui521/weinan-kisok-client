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
    public partial class FormPayNewChoose : Form
    {
        public string payType { get; set; }
        public FormPayNewChoose()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            payType = "微信";
            DialogResult = DialogResult.OK;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            payType = "支付宝";
            DialogResult = DialogResult.OK;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            payType = "银联";
            DialogResult = DialogResult.OK;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            payType = "现金";
            DialogResult = DialogResult.OK;
        }
    }
}
