using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
namespace 串口调试
{
    public partial class Form1 : Form
    {
        SerialPort sp = null;//声明一个串口类
        bool isOpen = false;//打开串口标志位
        bool isSetProperty = false;//属性设置标志位
        bool isHex = false;//十六进制显示标志位
        public Form1()
        {
            InitializeComponent();//窗口初始化，net 自动生成
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            for (int i = 0; i < 10; i++)//最大支持到串口 10，可根据自己需求增加
            {
                cbxCOMPort.Items.Add("COM" + (i + 1).ToString());
            }
            cbxCOMPort.SelectedIndex = 0;
            //列出常用的波特率
            cbxBaudRate.Items.Add("1200");
            cbxBaudRate.Items.Add("2400");
            cbxBaudRate.Items.Add("4800");
            cbxBaudRate.Items.Add("9600");
            cbxBaudRate.Items.Add("19200");
            cbxBaudRate.Items.Add("38400");
            cbxBaudRate.Items.Add("43000");
            cbxBaudRate.Items.Add("56000");
            cbxBaudRate.Items.Add("57600");
            cbxBaudRate.Items.Add("115200");
            cbxBaudRate.SelectedIndex = 5;
            //列出停止位
            cbxStopBits.Items.Add("0");
            cbxStopBits.Items.Add("1");
            cbxStopBits.Items.Add("1.5");
            cbxStopBits.Items.Add("2");
            cbxStopBits.SelectedIndex = 1;
            //列出数据位
            cbxDataBits.Items.Add("8");
            cbxDataBits.Items.Add("7");
            cbxDataBits.Items.Add("6");
            cbxDataBits.Items.Add("5");
            cbxDataBits.SelectedIndex = 0;
            //列出奇偶校验位
            cbxParity.Items.Add("无");
            cbxParity.Items.Add("奇校验");
            cbxParity.Items.Add("偶校验");
            cbxParity.SelectedIndex = 0;
            //默认为 Char 显示
            rbnChar.Checked = true;
        }
        private void btnCheckCOM_Click(object sender, EventArgs e)//检测哪些串口可用
        {
            bool comExistence = false;//有可用串口标志位
            cbxCOMPort.Items.Clear(); //清除当前串口号中的所有串口名称
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    SerialPort sp = new SerialPort("COM" + (i + 1).ToString());
                    sp.Open();
                    System.Threading.Thread.Sleep(100);
                    sp.Close();
                    System.Threading.Thread.Sleep(100);
                    cbxCOMPort.Items.Add("COM" + (i + 1).ToString());
                    comExistence = true;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (comExistence)
            {
                cbxCOMPort.SelectedIndex = 0;//使 ListBox 显示第 1 个添加的索引
            }
            else
            {
                MessageBox.Show("没有找到可用串口！", "错误提示");
            }
        }
        private bool CheckPortSetting()//检查串口是否设置
        {
            if (cbxCOMPort.Text.Trim() == "") return false;
            if (cbxBaudRate.Text.Trim() == "") return false;
            if (cbxDataBits.Text.Trim() == "") return false;
            if (cbxParity.Text.Trim() == "") return false;
            if (cbxStopBits.Text.Trim() == "") return false;
            return true;
        }
        private bool CheckSendData()
        {
            if (tbxSendData.Text.Trim() == "") return false;
            return true;
        }
        private void SetPortProperty()//设置串口的属性
        {
            sp = new SerialPort();
            sp.PortName = cbxCOMPort.Text.Trim();//设置串口名
            sp.BaudRate = Convert.ToInt32(cbxBaudRate.Text.Trim());//设置串口的波特率
            float f = Convert.ToSingle(cbxStopBits.Text.Trim());//设置停止位
            if (f == 0)
            {
                sp.StopBits = StopBits.None;
            }
            else if (f == 1.5)
            {
                sp.StopBits = StopBits.OnePointFive;
            }
            else if (f == 1)
            {
                sp.StopBits = StopBits.One;
            }
            else if (f == 2)
            {
                sp.StopBits = StopBits.Two;
            }
            else
            {
                sp.StopBits = StopBits.One;
            }
            sp.DataBits = Convert.ToInt16(cbxDataBits.Text.Trim());//设置数据位
            string s = cbxParity.Text.Trim(); //设置奇偶校验位
            if (s.CompareTo("无") == 0)
            {
                sp.Parity = Parity.None;
            }
            else if (s.CompareTo("奇校验") == 0)
            {
                sp.Parity = Parity.Odd;
            }
            else if (s.CompareTo("偶校验") == 0)
            {
                sp.Parity = Parity.Even;
            }
            else
            {
                sp.Parity = Parity.None;
            }
            sp.ReadTimeout = -1;//设置超时读取时间
            sp.RtsEnable = true;
            //定义 DataReceived 事件，当串口收到数据后触发事件
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            if (rbnHex.Checked)
            {
               isHex = true;
            }
            else
            {
                isHex = false;
            }
        }
        private void btnSend_Click(object sender, EventArgs e)//发送串口数据
        {
            if (isOpen)//写串口数据
            {
                if (!CheckSendData())//检测要发送的数据
                {
                    MessageBox.Show("请输入要发送的数据！", "错误提示");
                    return;
                }
                try
                {
                    sp.WriteLine(tbxSendData.Text);
                    tbxRecvData.Text = "";
                    tbxSendData.Text = "";
                }
                catch (Exception)
                {
                    MessageBox.Show("发送数据时发生错误！", "错误提示");
                    return;
                }
            }
            else
            {
                MessageBox.Show("串口未打开！", "错误提示");
                return;
            }
            
        }
        private void btnOpenCom_Click(object sender, EventArgs e)
        {
            if (isOpen == false)
            {
                if (!CheckPortSetting())//检测串口设置
                {
                    MessageBox.Show("串口未设置！", "错误提示");
                    return;
                }
                if (!isSetProperty)//串口未设置则设置串口
                {
                    SetPortProperty();
                    isSetProperty = true;
                }
                try//打开串口
                {
                    sp.Open();
                    isOpen = true;
                    btnOpenCom.Text = "关闭串口";
                    //串口打开后则相关的串口设置按钮便不可再用
                    cbxCOMPort.Enabled = false;
                    cbxBaudRate.Enabled = false;
                    cbxDataBits.Enabled = false;
                    cbxParity.Enabled = false;
                    cbxStopBits.Enabled = false;
                    rbnChar.Enabled = false;
                    rbnHex.Enabled = false;
                }
                catch (Exception)
                {
                    //打开串口失败后，相应标志位取消
                    isSetProperty = false;
                    isOpen = false;
                    MessageBox.Show("串口无效或已被占用！", "错误提示");
                }
            }
            else
            {
                try//打开串口
                {
                    sp.Close();
                    isOpen = false;
                    isSetProperty = false;
                    btnOpenCom.Text = "打开串口";
                    //关闭串口后，串口设置选项便可以继续使用
                    cbxCOMPort.Enabled = true;
                    cbxBaudRate.Enabled = true;
                    cbxDataBits.Enabled = true;
                    cbxParity.Enabled = true;
                    cbxStopBits.Enabled = true;
                    rbnChar.Enabled = true;
                    rbnHex.Enabled = true;
                }
                catch (Exception)
                {
                    lblStatus.Text = "关闭串口时发生错误";
                }
            }
        }
        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(100);//延时 100ms 等待接收完数据
                                               //this.Invoke 就是跨线程访问 ui 的方法，也是本文的范例
            this.Invoke((EventHandler)(delegate
            {
                if (isHex == false)
                {
                    tbxRecvData.Text += sp.ReadLine();
                }
                else
                {
                    Byte[] ReceivedData = new Byte[sp.BytesToRead]; //创建接收字节数组
                    sp.Read(ReceivedData, 0, ReceivedData.Length); //读取所接收到的数据
                    String RecvDataText = null;
                    for (int i = 0; i < ReceivedData.Length - 1; i++)
                    {
                        RecvDataText += ("0x" + ReceivedData[i].ToString("X2") + " ");// X 十六进制 2每次两位数
                    }
                    tbxRecvData.Text += RecvDataText;
                }
                sp.DiscardInBuffer();//丢弃接收缓冲区数据
            }));
        }
        private void btnCleanData_Click(object sender, EventArgs e)
        {
            tbxRecvData.Text = "";
            tbxSendData.Text = "";
        }
    }

}