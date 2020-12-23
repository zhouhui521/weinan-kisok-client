using KioskApp.register.model;
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

namespace KioskApp.register
{
    public partial class FormInputPhone : Form
    {
        public RegisterParam registerParam { get; set; }

        public string phone { get; set; }
        public FormInputPhone()
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

        private void FormInputPhone_Load(object sender, EventArgs e)
        {
            labIdCard.Text = registerParam.idCard;
            labName.Text = registerParam.name;
            labSex.Text = registerParam.sex;
            labNation.Text = registerParam.nation;
            labBrith.Text = registerParam.birthday;
            labAddress.Text = registerParam.address;
        }

        private void materialButton13_Click(object sender, EventArgs e)
        {
            phone = materialTextBoxPhone.Text;
        }
    }
}
