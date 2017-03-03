using System;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;

using System.Diagnostics;

using LcmsNetDataClasses.Devices;

namespace ASIpump
{
    public class SerialDevice
    {
        public delegate void MessageReplyDelegate(string sentStr, string replyStr);

        public delegate void MessageSentDelegate(string rcvStr);
        public event MessageSentDelegate MessageSent;


        public delegate void MessageStreamDelegate(string rcvStr);
        public event MessageStreamDelegate MessageStreamed;

        protected string mConcatStr = "";
        protected string mReplyStr = "";

        protected SerialPort mPort = new SerialPort();
        public SerialPort Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        public SerialDevice()
        {
            mPort.DataReceived += new SerialDataReceivedEventHandler(mPort_DataReceived);

            Timeout = 1;
        }


        private string mReplyDelimeter = "\r\n";
        public string ReplyDelimeter
        {
            get { return mReplyDelimeter; }
            set { mReplyDelimeter = value; }
        }

        private string mSendDelimeter = "\r\n";
        public string SendDelimeter
        {
            get { return mSendDelimeter; }
            set { mSendDelimeter = value; }
        }

        [Description("Timeout")]
        [Category("Serial")]
        [DisplayName("Timeout")]
        [classPersistenceAttribute("Timeout")]
        public double Timeout { get; set; }

        [Description("Port Name")]
        [Category("Serial")]
        [DisplayName("Port Name")]
        [classPersistenceAttribute("PortName")]
        public string PortName
        {
            get { return mPort.PortName; }
            set
            {
                if (value != null)
                {
                    var wasOpen = mPort.IsOpen;

                    if (mPort.IsOpen)
                    {
                        mPort.Close();
                    }
                    mPort.PortName = value;

                    if (wasOpen)
                    {
                        try
                        {
                            mPort.Open();
                        }
                        catch { }
                    }
                }
            }
        }

        [Description("Handshake")]
        [Category("Serial")]
        [DisplayName("Handshake")]
        [classPersistenceAttribute("")]
        public Handshake Handshake
        {
            get { return mPort.Handshake; }
            set { mPort.Handshake = value; }
        }

        [Description("DTR Enabled")]
        [Category("Serial")]
        [DisplayName("DTR Enambled")]
        [classPersistenceAttribute("")]
        public bool DtrEnable
        {
            get { return mPort.DtrEnable; }
            set { mPort.DtrEnable = value; }
        }

        [Description("RTS Enabled")]
        [Category("Serial")]
        [DisplayName("RTS Enabled")]
        [classPersistenceAttribute("")]
        public bool RtsEnable
        {
            get { return mPort.RtsEnable; }
            set { mPort.RtsEnable = value; }
        }

        [Description("Baud Rate")]
        [Category("Serial")]
        [DisplayName("Baud Rate")]
        [classPersistenceAttribute("BaudRate")]
        public int BaudRate
        {
            get { return mPort.BaudRate; }
            set
            {
                if (value == 0)
                    value = 9600;
                mPort.BaudRate = value;
            }
        }

        [Description("Data Bits")]
        [Category("Serial")]
        [DisplayName("Data Bits")]
        [classPersistenceAttribute("DataBits")]
        public int DataBits
        {
            get { return mPort.DataBits; }
            set
            {
                if (value == 0)
                    value = 8;
                mPort.DataBits = value;
            }
        }

        [Description("Stop Bits")]
        [Category("Serial")]
        [DisplayName("Stop Bits")]
        [classPersistenceAttribute("StopBits")]
        public StopBits StopBits
        {
            get { return mPort.StopBits; }
            set
            {
                if (value == StopBits.None)
                    value = StopBits.One;
                mPort.StopBits = value;
            }
        }

        [Description("Parity")]
        [Category("Serial")]
        [DisplayName("Parity")]
        [classPersistenceAttribute("Parity")]
        public Parity Parity
        {
            get { return mPort.Parity; }
            set { mPort.Parity = value; }
        }

        public override string ToString()
        {
            return (PortName.ToString() +
                        " baud=" + BaudRate.ToString() +
                        " parity=" + Parity.ToString() +
                        " data=" + DataBits.ToString() +
                        " stop=" + StopBits.ToString());
        }

        public string[] Split(string line, string delimeter)
        {
            var delim = new string[1];
            delim[0] = delimeter;
            return (line.Split(delim, StringSplitOptions.None));
        }

        public void Send(string sendStr)
        {
            try
            {
                if (mPort.IsOpen)
                {
                    // clear these so unneeded replies don't build up
                    mReplyStr = null;
                    mConcatStr = "";
                    var inputStr = mPort.ReadExisting();

                    mPort.Write(sendStr + mSendDelimeter);

                    MessageSent?.Invoke(sendStr);
                }
            }
            catch { }
        }

        // failure to get a reply returns a null
        public string Query(string queryString)
        {
            if (mPort.IsOpen == false)
                return null;

            mReplyStr = null;
            mConcatStr = "";

            // clear stuff unread on the port
            var inputStr = mPort.ReadExisting();

            Send(queryString);

            return ReceiveData();
        }

        private readonly Stopwatch timeoutWatch = new Stopwatch();
        // pull data from the receive queue
        public string ReceiveData()
        {
            mReplyStr = null;

            //see if it has already arrived
            ParseConcatStr();

            timeoutWatch.Reset();
            timeoutWatch.Start();
            while (mReplyStr == null && timeoutWatch.Elapsed.Seconds < Timeout)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }

            return mReplyStr;
        }

        public virtual void ParseConcatStr()
        {
            lock (mConcatStr)
            {

                while (mConcatStr.Contains(mReplyDelimeter))
                {
                    var pos = mConcatStr.IndexOf(mReplyDelimeter, StringComparison.Ordinal);
                    mReplyStr += mConcatStr.Substring(0, pos);
                    mConcatStr = mConcatStr.Substring(pos + mReplyDelimeter.Length);
                }


                if (MessageStreamed != null && !String.IsNullOrEmpty(mReplyStr))
                {
                    MessageStreamed(mReplyStr);
                    mReplyStr = "";
                }
            }
        }

        private void mPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var inputStr = mPort.ReadExisting();

            mConcatStr += inputStr;

            ParseConcatStr();
        }

        private void mPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }
    }
}
