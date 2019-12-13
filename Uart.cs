using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Common
{
    public class Uart
    {
        /// <summary>
        /// 解析数据包类
        /// </summary>
        public class ParseData
        {
            public enum ParseDataResult
            {
                Result_None,
                Result_OK,
                Result_Error,
            }
            public ParseDataResult Result { set; get; }
            public int Data1 { set; get; }
            public int Data2 { set; get; }
            public int Data3 { set; get; }
            public int Data4 { set; get; }
            public int Data5 { set; get; }
            public int Data6 { set; get; }
            public List<byte> Pack { set; get; }
            public string PackHexString { set; get; }

            public ParseData()
            {
                Result = ParseDataResult.Result_None;
            }

            /// <summary>
            /// 解析数据包 请在子类重写
            /// </summary>
            /// <param name="item">解析一个字节</param>
            public virtual void ParseDataByByte(byte item)
            {
            }

        }
        /// <summary>
        /// 设置解析对象
        /// </summary>
        public ParseData ParseDataObj {set;get;}
        public event Action<ParseData> ReceivedDataPack;   //声明收到数包事件
        public event Action<byte[]>    ReceivedBytes;
        public event Action<byte[]>    SendBytesEvent;
        private System.Timers.Timer timer_1s = new System.Timers.Timer(1000);
        private System.Timers.Timer timerf_30ms = new System.Timers.Timer();
        private SerialPort serialPort1 = new System.IO.Ports.SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);

        // 接收、发送计数
        public int TxCount { set; get; }
        public int RxCount { set; get; }
        // 错误、好的数据包计数,接收包频率
        public int ErrorPackCount { set; get; }
        public int GoodPackCount { set; get; }
        //获取所有的串口名字
        public string[] FindSerialPortName
        {
            get
            {
                if(SerialPort.GetPortNames().Length > 0)
                {
                    return SerialPort.GetPortNames();
                }
                else
                {
                    return new string[] { "port not found" };
                }
            }
        }   
        public bool SerialPortIsOpen { get { return serialPort1.IsOpen; } }          //串口是否打开

        // 接收包频率,接收速率
        public int PackFrequency { set; get; }
        public int ReceivingRate { set; get; }
        private int packFreqCount = 0;
        private int recRate = 0;

        private UartConfig config;
        private Form OwnerForm;
        private Thread thread1;

        public Uart()
        {
            ParseDataObj = null;
            OwnerForm = null;
            timer_1s.Interval = 1000;
            timer_1s.Elapsed += Timer_1s_Elapsed;
            timer_1s.AutoReset = true;
            timer_1s.Enabled = true;

            //timerf_30ms.Interval = 30;
            //timerf_30ms.Elapsed += Timerf_30ms_Tick;
            //timerf_30ms.AutoReset = true;
            //timerf_30ms.Enabled = true;
        }
        public Uart(Form OwnerForm)
        {
            ParseDataObj = null;
            this.OwnerForm = OwnerForm;
            timer_1s.Interval = 1000;
            timer_1s.Elapsed += Timer_1s_Elapsed;
            timer_1s.AutoReset = true;
            timer_1s.Enabled = true;

            //timerf_30ms.Interval = 30;
            //timerf_30ms.Elapsed += Timerf_30ms_Tick;
            //timerf_30ms.AutoReset = true;
            //timerf_30ms.Enabled = true;
        }
        /// <summary>
        /// Uart config form
        /// </summary>
        public void UartConfigByGUI()
        {
            if(config == null || config.IsDisposed == true)
            {
                config = new UartConfig(serialPort1);
                if(OwnerForm != null)
                {
                    config.Owner = this.OwnerForm;
                }
            }

            config.ShowDialog();
        }
        /// <summary>
        /// Uart config
        /// </summary>
        public void UartConfig(string portName = "COM1",Int32 baudRate = 9600,Int32 dataBits = 8,Parity parity = Parity.None,StopBits stopBits = StopBits.One)
        {
            serialPort1.BaudRate = baudRate;
            serialPort1.PortName = portName;
            serialPort1.DataBits = dataBits;
            serialPort1.Parity = parity;
            serialPort1.StopBits = stopBits;
        }
        /// <summary>
        /// 打开或者关闭串口
        /// </summary>
        /// <param name="portName">COM 号 如果打开出错返回出错消息</param>
        /// <returns>true ok false fail</returns>
        public bool OpenOrClosePort(ref string portName)
        {
            if(serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                return false;
            }
            else
            {
                if(portName != "") { serialPort1.PortName = portName; }
                try {
                    serialPort1.Open();
                    SttartReceivedThread();
                }
                catch(Exception ex)
                {
                    portName = ex.Message;
                    StopReceivedThread();
                    return false;//	throw;
                }
            }
            return true;
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="portName">COM 号</param>
        /// <returns></returns>
        public bool OpenPort(ref string portName)
        {
            if(serialPort1.IsOpen == true) return true;

            serialPort1.PortName = portName;
            try {
                serialPort1.Open();
                SttartReceivedThread();
            }
            catch(Exception ex)
            {
                portName = ex.Message;
                StopReceivedThread();
                return false;//	throw;
            }
            return true;
        }
        public void SendBytes(byte[] data)
        {
            if(SerialPortIsOpen)
            {
                if(data.Length > 0)
                {
                    serialPort1.Write(data, 0, data.Length);
                    TxCount += data.Length;
                    if(SendBytesEvent != null)
                    {
                        if(this.OwnerForm != null)
                        {
                            this.OwnerForm.BeginInvoke(SendBytesEvent,data);
                        }
                        else
                        {
                            SendBytesEvent(data);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Dispose()
        {
            if(serialPort1.IsOpen == true)
            {
                serialPort1.Close();
                StopReceivedThread();
            }
        }
        private void SttartReceivedThread()
        {
            thread1 = new Thread(new ThreadStart(UsartDataReceivedThead));
            thread1.IsBackground = true;
            thread1.Name = "UsartDataReceivedThead";
            thread1.Start();
        }
        private void StopReceivedThread()
        {
        }
        /**/
        private void Timerf_30ms_Tick(object sender, EventArgs e)
        {
            // 使用Windows.Forms.Timer 定时器来定时接收数据,更新UI时更流畅
            if(serialPort1.IsOpen == true)
                SerialPort1DataReceived();
        }

        private void UsartDataReceivedThead()
        {
            while(serialPort1.IsOpen)
            {
                //System.Threading.Thread.Sleep(30);
                // 用不着此线程
                SerialPort1DataReceived();
            }
        }
        private void Timer_1s_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(serialPort1.IsOpen == true)
            {
                PackFrequency = packFreqCount;
                packFreqCount = 0;
                ReceivingRate = recRate;
                recRate = 0;
            }
        }



        private void ReceivingPackets(byte[] buffer)
        {
            /* 判断是否有解析对象 */
            if(ParseDataObj != null)
            {
                foreach(byte item in buffer)
                {
                    ParseDataObj.ParseDataByByte(item);
                    if(ParseDataObj.Result != ParseData.ParseDataResult.Result_None)
                    {
                        packFreqCount++;
                        if(ParseDataObj.Result == ParseData.ParseDataResult.Result_OK)          GoodPackCount++;     /* 数据包校验正确 */
                        else if(ParseDataObj.Result == ParseData.ParseDataResult.Result_Error)  ErrorPackCount++;    /* 数据包校验错误 */

                        if(ReceivedDataPack != null)
                        {
                            if(this.OwnerForm != null)
                            {
                                this.OwnerForm.BeginInvoke(ReceivedDataPack, ParseDataObj);
                            }
                            else
                            {
                                ReceivedDataPack(ParseDataObj);
                            }
                        }
                    }
                }
            }
            if(ReceivedBytes != null)
            {
                if(this.OwnerForm != null)
                {
                    this.OwnerForm.BeginInvoke(ReceivedBytes, buffer);
                }
                else
                {
                    ReceivedBytes(buffer);
                }
            }
        }
        private void SerialPort1DataReceived()
        {
            SerialPort sp = serialPort1;
            int n = sp.BytesToRead;             //读取缓存里的字节数
            if(n <= 0) return;                  //没有数据退出 //收到0x1A时 还有数据  但n=0 不知怎么回事
            byte[] re_buf = new byte[n];
            sp.Read(re_buf, 0, n);              //读取缓冲数据
            
            RxCount += n;
            recRate += n;
            ReceivingPackets(re_buf);   // 接收解析数据包
        }
    }
}
