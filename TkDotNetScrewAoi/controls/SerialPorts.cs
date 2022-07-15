using System;
using System.ComponentModel;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace TkDotNetScrewAoi.control
{
    public enum enumSerialPortStatus
    {
        INIT,
        OPEN,
        WRITE,
        CLOSE,
        ERROR
    }

    public class SerialPorts
    {
        private SerialPort comlPort;
        public enumSerialPortStatus SerialPortStatus=enumSerialPortStatus.INIT;

        private bool isPortOpen_;
        private bool isPortRead_;
        public bool isPortOpen
        {
            get { return isPortOpen_; }
            set { isPortOpen_ = value; }
        }

        public SerialPorts()
        {
            
        }

        public void Init()
        {
            try
            {
                if (comlPort == null)
                {
                    comlPort = new SerialPort();
                }
                else if (SerialPortStatus == enumSerialPortStatus.INIT)
                {
                    comlPort.Dispose();
                    comlPort = null;
                    comlPort = new SerialPort();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("串列阜初始化異常" + ex.Message);
            }  
        }

        public void Open(string COM, Int32 baud)
        {
            try
            {
                Init();//
                comlPort.PortName = COM;
                comlPort.BaudRate = baud;
                comlPort.DataBits = 8;
                comlPort.Parity = Parity.None;
                comlPort.StopBits = StopBits.One;
                comlPort.Open();
                SerialPortStatus = enumSerialPortStatus.OPEN;
                isPortRead_=true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("串列阜開啟異常" + ex.Message);
            }
        }

        public void Close()
        {
            isPortRead_=false;
            comlPort.Close();
            comlPort.Dispose();
            comlPort = null;
        }

        public bool IsComPortOpen()
        {
            if (comlPort == null)
                return isPortOpen = false;

            if (comlPort.IsOpen)
            {
                return isPortOpen=true;
            }
            else
                return isPortOpen = false;
        }

        public void Write(string command)
        {
            comlPort.Write(command);
        }

        public void Read()
        {
            while (isPortRead_)
            {
                try
                {
                    Console.WriteLine("讀取");
                    string indata = comlPort.ReadLine();
                    Console.WriteLine("讀取2");
                    Console.WriteLine("讀取內容:"+indata);
                    //serialPort.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void Send(string COM, Int32 baud,string command)
        {
            if (!IsComPortOpen())
            {            
                Open(COM, baud);
            }            
            Write(command);
            
            Close();
        }
    }
}
