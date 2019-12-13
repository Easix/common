using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Common
{
    /// <summary>
    /// uart form config
    /// </summary>
    public partial class UartConfig: Form
    {
        private SerialPort sp;
        public UartConfig(SerialPort sp)
        {
            this.sp = sp;
            InitializeComponent();
            label_Msg.Text = "";
            UpdatePortList();
            cobBox_BaudRate.Text = sp.BaudRate.ToString();
            cobBox_ParityBit.SelectedIndex = (int)sp.Parity;
            cobBox_DataBit.SelectedIndex = sp.DataBits - 5; /* 5-8 */
            if(sp.StopBits == StopBits.None)
                 cobBox_StopBit.SelectedIndex = (int)sp.StopBits;
            else cobBox_StopBit.SelectedIndex = (int)sp.StopBits - 1;

            if(sp.IsOpen == true)   { groupBox1.Enabled = false; btn_Open.Text = "Close Port"; }
            else                    { groupBox1.Enabled = true; btn_Open.Text = "Open Port"; }
        }
        private void UpdatePortList()
        {
            string[] portList = SerialPort.GetPortNames(); //获取所有的串口名字
            cobBox_Name.Items.Clear();//清空链表内容
            if(portList.Length > 0)
            {
                cobBox_Name.Items.AddRange(portList);
                int i = 0;
                foreach(string str in portList)
                {
                    if(sp.PortName == str) { cobBox_Name.SelectedIndex = i; return; }
                    i++;
                }
                cobBox_Name.SelectedIndex = 0;
            }
            else
            {
                cobBox_Name.Items.Add("port not found");
                cobBox_Name.SelectedIndex = 0;
            }
        }
        private void bnt_OK_Click(object sender, EventArgs e)
        {
            if(sp.IsOpen == false)
            {
                if(cobBox_Name.Text != null)
                    if(cobBox_Name.Text != "")
                    sp.PortName = cobBox_Name.Text;
                sp.BaudRate = int.Parse(cobBox_BaudRate.Text);
                sp.Parity = (Parity)cobBox_ParityBit.SelectedIndex;
                sp.DataBits = cobBox_DataBit.SelectedIndex + 5;
                sp.StopBits = (StopBits)cobBox_StopBit.SelectedIndex + 1;
            }
            this.Close();
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            if(sp.IsOpen == true)
            {
                sp.Close();
            }
            else
            {
                try {
                    sp.PortName = cobBox_Name.Text;
                    sp.BaudRate = int.Parse(cobBox_BaudRate.Text);
                    sp.Parity = (Parity)cobBox_ParityBit.SelectedIndex;
                    sp.DataBits = cobBox_DataBit.SelectedIndex + 5;
                    sp.StopBits = (StopBits)cobBox_StopBit.SelectedIndex + 1;
                    sp.Open();
                    label_Msg.Text = "";
                }
                catch(Exception ex)
                {
                    label_Msg.Text = ex.Message;
                    label_Msg.ForeColor = Color.Red;
                }
            }
            if(sp.IsOpen == true) { groupBox1.Enabled = false; btn_Open.Text = "Close Port"; }
            else { groupBox1.Enabled = true; btn_Open.Text = "Open Port"; }
        }

        private void cobBox_Name_DropDown(object sender, EventArgs e)
        {
            UpdatePortList();
        }
    }
}
