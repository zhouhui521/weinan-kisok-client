using KioskApp.prestore.model;
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

namespace KioskApp.pay
{
    public partial class FormCashSave : MaterialForm
    {
        private readonly MaterialSkinManager materialSkinManager;

        public Patient patient { get; set; }

        public int totalCount { get; set; }

        public int allMoney { get; set; }

        public FormCashSave()
        {
            InitializeComponent();
            materialSkinManager = MaterialSkinManager.Instance;

            materialSkinManager.EnforceBackcolorOnAllComponents = true;

            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100, Accent.Pink200, TextShade.WHITE);
        }

        private void FormCashSave_Load(object sender, EventArgs e)
        {
            materialLabel1.Text = "卡号："+patient.cardNo;
            materialLabel2.Text = "姓名：" + patient.name;
            //打开纸币器

            materialButton2.Enabled = false;

        }


        /// <summary>
        /// 结束存钞
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void materialButton1_Click(object sender, EventArgs e)
        {
            string result = HardWareService.getValue("<invoke name=\"BILLACCEPTORSTACKMONEYDETAIL\"><arguments></arguments></invoke>");

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(result);
            System.Xml.XmlNode xnd = doc.SelectSingleNode("/return/arguments/string[@id='STACKMONEY']");
            if (xnd == null)
            {
                MessageBox.Show("没有获取到存钞票明细");
                DialogResult = DialogResult.Cancel;
            }
            string cashResult = xnd.InnerText;
            int totalMoney = 0;
            if (!string.IsNullOrEmpty(cashResult))
            {
                if (cashResult.Contains("|"))
                {
                    string[] cashList = cashResult.Split('|');

                    label6.Text = "已存放张数：" + cashList.Length;
                    totalCount = cashList.Length;




                    for (int i = 0; i < cashList.Length; i++)
                    {
                        //获取每张明细
                        string cashDetail = cashList[i];

                        string money = cashDetail.Split(',')[0];
                        int intMoney = Convert.ToInt32(money);
                        totalMoney += intMoney;

                    }
                    allMoney = totalMoney;
                    label5.Text = "已存放金额：" + totalMoney;
                    materialButton1.Enabled = false;
                    materialButton2.Enabled = true;
                }
                else
                {
                    totalCount = 1;
                    string money = cashResult.Split(',')[0];
                    int intMoney = Convert.ToInt32(money);
                    totalMoney += intMoney;
                    allMoney = totalMoney;
                    label5.Text = "已存放金额：" + totalMoney;
                    materialButton1.Enabled = false;
                    materialButton2.Enabled = true;
                }
            }
            
           

           
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
