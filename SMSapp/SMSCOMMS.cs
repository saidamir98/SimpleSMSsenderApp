using System;
using System.Threading;
using System.ComponentModel;
using System.IO.Ports;

namespace SMSapp
{
    public class SMSCOMMS
    {
        private SerialPort SMSPort;
        private Thread SMSThread;
        private Thread ReadThread;
        public static bool _Continue = false;
        public static bool _ContSMS = false;
        private bool _Wait = false;
        public static bool _ReadPort = false;
        public delegate void SendingEventHandler(bool Done);
        public event SendingEventHandler Sending;
        public delegate void DataReceivedEventHandler(string Message);
        public event DataReceivedEventHandler DataReceived;

        public SMSCOMMS(ref string COMMPORT)
        {
            SMSPort = new SerialPort();
            SMSPort.PortName = COMMPORT;
            SMSPort.BaudRate = 921600;
            SMSPort.Parity = Parity.None;
            SMSPort.DataBits = 8;
            SMSPort.StopBits = StopBits.One;
            SMSPort.Handshake = Handshake.RequestToSend;
            SMSPort.DtrEnable = true;
            SMSPort.RtsEnable = true;
            SMSPort.NewLine = System.Environment.NewLine;
            ReadThread = new Thread(
                new System.Threading.ThreadStart(ReadPort));
        }

        public bool SendSMS(string CellNumber, string SMSMessage)
        {
            string MyMessage = null;
            //Check if Message Length <= 160
            if (SMSMessage.Length <= 160)
                MyMessage = SMSMessage;
            else
                MyMessage = SMSMessage.Substring(0, 160);
            if (SMSPort.IsOpen == true)
            {
                SMSPort.WriteLine(@"AT" + (char)(13));
                Thread.Sleep(200);
                SMSPort.WriteLine("AT+CMGF=1" + (char)(13));
                Thread.Sleep(200);
                SMSPort.WriteLine(@"AT+CMGS=""" + CellNumber + @"""" + (char)(13));
                Thread.Sleep(200);
                _ContSMS = false;
                SMSPort.WriteLine(SMSMessage + (char)(26));
                _Continue = false;
                return true;
                
            }
            return false;
        }

        private void ReadPort()
        {
            string SerialIn = null;
            byte[] RXBuffer = new byte[SMSPort.ReadBufferSize + 1];
            string SMSMessage = null;
            int Strpos = 0;
            string TmpStr = null;
            while (SMSPort.IsOpen == true)
            {
                if ((SMSPort.BytesToRead != 0) & (SMSPort.IsOpen == true))
                {
                    while (SMSPort.BytesToRead != 0)
                    {
                        SMSPort.Read(RXBuffer, 0, SMSPort.ReadBufferSize);
                        SerialIn =
                            SerialIn + System.Text.Encoding.ASCII.GetString(
                            RXBuffer);
                        if (SerialIn.Contains(">") == true)
                        {
                            _ContSMS = true;
                        }
                        if (SerialIn.Contains("+CMGS:") == true)
                        {
                            _Continue = true;
                            if (Sending != null)
                                Sending(true);
                            _Wait = false;
                            SerialIn = string.Empty;
                            RXBuffer = new byte[SMSPort.ReadBufferSize + 1];
                        }
                    }
                    if (DataReceived != null)
                        DataReceived(SerialIn);
                    SerialIn = string.Empty;
                    RXBuffer = new byte[SMSPort.ReadBufferSize + 1];
                }
            }
        }

        public void Open()
        {
            if (SMSPort.IsOpen == false)
            {
                SMSPort.Open();
                ReadThread.Start();
            }
        }

        public void Close()
        {
            if (SMSPort.IsOpen == true)
            {
                SMSPort.Close();
            }
        }
    }
}
