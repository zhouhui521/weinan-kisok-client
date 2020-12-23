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
    public partial class FormNewInputMoney : Form
    {

        public string money { get; set; }

      
        public FormNewInputMoney()
        {
            InitializeComponent();
         
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
            money = materialTextBoxPhone.Text;
            DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
