using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using HalconDotNet;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;

namespace TkDotNetScrewAoi.controls
{
    /*
     *  TCP 父類
     *  參考資料:https://dotblogs.com.tw/kevintan1983/2010/10/15/18348
     *  基本功能描述
     *  1. 傳送參數 與 接收回聲
     *  2. 監聽
     *  3. 確認連線狀態
     *
     */
    public enum SendStatus //本地為Client
    {
        CONNECT,       //發送通道 連線
        DISCONNECT,    //發聳通道 斷開        
    }

    public enum ListenStatus //本地為Server
    {
        CONNECT,       //發送通道 連線
        DISCONNECT,    //發聳通道 斷開   
    }
    /// <summary>
    /// 本地為Client 接收事件
    /// </summary>
    public class SendReciveArgs : EventArgs
    {
        public byte[] SendReciveArray { get; set; }

        public SendReciveArgs(byte[] _ReciveArray)
        {
            this.SendReciveArray = _ReciveArray;
        }
    }

    /// <summary>
    /// 本地為Server 接收事件
    /// </summary>
    public class ServerReciveArgs : EventArgs
    {
        public byte[] ServerReciveeArray { get; set; }

        public ServerReciveArgs(byte[] ReciveArray_)
        {
            this.ServerReciveeArray = ReciveArray_;
        }
    }

    public class MessageReciveArgs : EventArgs
    {
        public byte[] MessageReciveArray { get; set; }
        public MessageReciveArgs(byte[] MessageReciveArgs_)
        {
            this.MessageReciveArray = MessageReciveArgs_;
        }
    }

    /// <summary>
    ///  圖像 接收事件
    /// </summary>
    public class ReciveImageArgs : EventArgs
    {
        public Bitmap bitmap { get; set; }

        public ReciveImageArgs(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }
    }
    public class Sockets
    {
        public event EventHandler<ReciveImageArgs> OnReceiveImg;//傳圖

        public event EventHandler<ServerReciveArgs> OnServerRecive;

        public event EventHandler<MessageReciveArgs> OnReciveMsg;

        public Stopwatch stopWatchSockets { get; set; }

        public TcpClient tcpSendClient { get; set; }//本地端 為 客戶端

        public TcpListener tcpListener { get; set; }//本地端 為 發送端

        private SendStatus sendStatus_ = SendStatus.DISCONNECT;
        public SendStatus sendStatus //本地端 為 客戶端  連線狀態
        {
            get { return sendStatus_; }
            set { sendStatus_ = value; }
        }

        private ListenStatus listenStatus_ = ListenStatus.DISCONNECT;
        public ListenStatus listenStatus  //本地端 為 伺服器端  連線狀態
        {
            get { return listenStatus_; }
            set { listenStatus_ = value; }
        }

        private int serverPort_ = 0;//本地端 伺服器 Port
        public int serverPort //本地端 伺服器 Port
        { get { return serverPort_; } set { serverPort_ = value; } }

        private int clientPort_ = 0;//本地端 客戶端 Port
        public int clientPort //本地端 客戶端 Port
        { get { return clientPort_; } set { clientPort_ = value; } }

        private string ipLocal;
        private IPEndPoint iPClientEndPoint_; //本地端IP
        public IPEndPoint iPClientEndPoint //本地端IP
        {
            get { return iPClientEndPoint_; }
            set { iPClientEndPoint_ = value; }
        }

        public bool isClientReceive = false;//本地客戶端是否接收訊息
        public bool isServerReceive = false;//本地伺服器端收訊息
        public bool isImageClientReceive = false;//本地客戶端是否接收影像
        public bool isImageServerReceive = false;//本地伺服器端是否接收影像
        public bool isKeepConnectClinet = false;//本地客戶端是否常連接 異地
        public bool isImageSend = false;
        public int sendInterval = 50;//發送週期

        /// <summary>
        /// 類創建
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="serverPort"></param>
        public Sockets(string ip, int port, int serverPort)
        {
            this.ipLocal = ip;
            this.clientPort = port;//異地
            this.iPClientEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);//
            this.serverPort = serverPort;//本機
        }


        /// <summary>
        /// 監聽方法初始化
        /// </summary>
        public virtual void ServerListenerInit()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse(ipLocal), this.serverPort);
            this.isServerReceive = true;
            this.isImageClientReceive = true;
            Task.Run(new Action(() =>
            {
                ServerListenerLoop();
            }));
        }

        /// <summary>
        /// 循環監聽等待連線
        /// </summary>
        public virtual void ServerListenerLoop()
        {
            this.tcpListener.Start();
            //Console.WriteLine("等待連線"+this.isServerReceive.ToString());
            while (this.isServerReceive)
            {
                if (tcpListener.Pending())
                {
                    //Console.WriteLine("連線進來了");
                    TcpClient tcpClientTemp = tcpListener.AcceptTcpClient();//接收連線
                    Task.Run(new Action(() =>
                    {
                        ReciveImage(tcpClientTemp);
                    }));
                }
            }
        }

        /// <summary>
        /// 判斷本地端連線狀態
        /// </summary>
        /// <returns></returns>
        public bool IsConnect()
        {

            if (tcpSendClient == null)//若是tcpSendClient 為null 則表示無連線
            {
                //OnStatus?.Invoke(this, new SocketSendStatusArgs(this.tcpSendClient.Connected));
                return false;
            }
            //OnStatus?.Invoke(this, new SocketSendStatusArgs(false));
            return tcpSendClient.Connected;
        }

        /// <summary>
        /// 本地客戶端連線到異地
        /// 點對點 長連接
        /// </summary>
        public virtual bool Connect()
        {
            try
            {
                try
                {
                    if (tcpSendClient != null && tcpSendClient.Connected)//連線關閉與釋放
                    {
                        tcpSendClient.Close();
                        tcpSendClient.Dispose();
                        tcpSendClient = null;
                        //OnStatus?.Invoke(this, new SocketSendStatusArgs(this.tcpSendClient.Connected));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("tcpSendClient 關閉與釋放異常", ex.Message);
                    //LogHelper.error("tcpSendClient 關閉與釋放異常", ex);
                }

                // 創建新連線
                tcpSendClient = new TcpClient();
                isKeepConnectClinet = true;//執行長連接               
                isClientReceive = true;//持續接收                
                Task.Run(new Action(() =>
                {
                    keepConnect();//持續連線  直到 isKeepConnectClinet=false
                }));

            }
            catch (Exception ex)
            {
                //LogHelper.error("連接失敗", ex);
                Console.WriteLine("連接失敗", ex);
                return false;
            }
            return true;
        }

        public virtual void Close()
        {
            isKeepConnectClinet = false;//執行長連接               
            isClientReceive = false;//持續接收  
            tcpSendClient.Close();
            tcpSendClient.Dispose();
            tcpSendClient = null;
        }

        public virtual async void keepConnect()
        {
            while (isKeepConnectClinet)//長連接
            {
                try
                {
                    if (!IsConnect())
                    {
                        Console.WriteLine("IsConnect()" + IsConnect().ToString());
                        if (tcpSendClient != null)//如果發送通道不為空 連線一次
                        {
                            tcpSendClient.Close();
                            tcpSendClient.Dispose();
                            tcpSendClient = null;
                        }
                        tcpSendClient = new TcpClient();
                        await tcpSendClient.ConnectAsync(iPClientEndPoint.Address, iPClientEndPoint.Port);//本地端 IP PORT
                        Console.WriteLine("連線上了");
                        ReceiveMsg();
                    }
                    //isClientReceive = true;//啟用回傳值接收任務
                    //Task.Run(ReceiveUntilDisConnect);
                    //isKeepConnectClinet = false;//連線完成
                }
                catch (Exception ex)
                {
                    Console.WriteLine("連線失敗" + ex.ToString());
                    //LogHelper.error("Socket 連線失敗 ", ex);//關閉回傳值接收任務
                    tcpSendClient.Close();
                    tcpSendClient.Dispose();
                    tcpSendClient = null;
                }
                finally
                {
                    await Task.Delay(3000);//每3秒連線一次
                }
            }
        }

        /// <summary>
        /// 本地端 持續接收訊息
        /// </summary>
        private void ReceiveMsg()
        {
            try
            {
                NetworkStream networkStream = tcpSendClient.GetStream();//持續讀取回傳值
                byte[] bytes = new byte[tcpSendClient.ReceiveBufferSize];
                while (isClientReceive)
                {
                    int length = networkStream.Read(bytes, 0, bytes.Length);//持續讀取回傳值
                    if (length > 0)
                    {
                        
                        byte[] bytesRead = new byte[length];
                        Array.Copy(bytes, bytesRead, length);
                        //Console.WriteLine("收完值了"+ BitConverter.ToString(CmdScrewSelf.Instance.modeHandle));
                        OnReciveMsg?.Invoke(this, new MessageReciveArgs(bytesRead));//將訊息送出
                    }
                }
                networkStream.Close(); networkStream.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReceiveMsg()", ex.ToString());//斷線
                //isKeepConnectClinet = true;
                //Task.Run(new Action(() => 
                //{
                //    keepConnect();
                //}));
                //LogHelper.error("客戶端回傳異常", ex);//斷線
            }
        }

        /// <summary>
        /// 傳送命令字串
        /// </summary>
        /// <param name="_Cmd"></param>
        /// <exception cref="SocketException"></exception>
        public void Send(byte[] cmd)
        {
            if (cmd != null)
            {
                try
                {
                    if (tcpSendClient != null && tcpSendClient.Connected)
                    {
                        NetworkStream networkStream = tcpSendClient.GetStream();//networkStream.ReadTimeout = 100;                        
                        networkStream.Write(cmd, 0, cmd.Length);
                        networkStream.Flush();
                    }
                    //networkStream.Read(vs, 0, 1024);
                    //_adressRecive = BitConverter.ToString(vs,5);
                    // _adressRecive = BitConverter.ToString(vs.Skip(5).Take(1).ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //utils.LogHelper.error("totalQueue傳送命令失敗", ex);
                    throw new SocketException();
                }
                Thread.Sleep(sendInterval);
            }
        }

        /// <summary>
        /// 傳送圖像
        /// </summary>
        /// <param name="bitmap"></param>
        public void Send(Bitmap bitmap)
        {
            try
            {
                if (tcpSendClient != null && tcpSendClient.Connected)
                {
                    try
                    {
                        NetworkStream networkStream = tcpSendClient.GetStream();
                        byte[] bytes = Bitmap2Array(bitmap);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Position = 0;
                            using (BinaryWriter bw = new BinaryWriter(ms))
                            {
                                bw.Write(bytes.Length);
                                bw.Write(bytes);
                                byte[] targetBuffer = new byte[ms.Length];
                                Buffer.BlockCopy(ms.GetBuffer(), 0, targetBuffer, 0, (int)ms.Length);
                                networkStream.Write(targetBuffer, 0, targetBuffer.Length);
                            }
                            ms.Close();
                        }
                        //Console.WriteLine("發送端 MemoryStream: ms" + ms.Length.ToString(), ",發送端 bytes.Length:" + bytes.Length.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //utils.LogHelper.error("totalQueue傳送命令失敗", ex);
            }
            Thread.Sleep(100);
        }

        public void Send(byte[] headflag1, byte[] data)
        {
            byte[] bytes_; //自定義協定
            try
            {
                if (tcpSendClient != null && tcpSendClient.Connected)
                {
                    using (MemoryStream memoryStream = new MemoryStream()) //創建內存流
                    {
                        using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))//以二进制写入器往这个流里写内容
                        {
                            binaryWriter.Write(headflag1); //写入协议一级标志，占1个字节
                            binaryWriter.Write(BitConverter.GetBytes(data.Length)); //写入实际消息长度，占4个字节
                            if (data.Length > 0)
                            {
                                binaryWriter.Write(data); //写入实际消息内容
                            }
                            bytes_ = memoryStream.ToArray(); //将流内容写入自定义字节数组
                            binaryWriter.Close(); //关闭写入器释放资源
                        }
                    }
                    NetworkStream networkStream = tcpSendClient.GetStream();//networkStream.ReadTimeout = 100;
                    networkStream.Write(bytes_, 0, bytes_.Length);
                    networkStream.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());//utils.LogHelper.error("totalQueue傳送命令失敗", ex);
                throw new SocketException();
            }
        }

        /// <summary>
        /// 接收圖像
        /// 玉銘參考
        /// </summary>
        /// <param name="tcpImageRevClient"></param>
        public void ReciveImage(TcpClient tcpImageRevClient)
        {
            //參考資料
            //https://blog.csdn.net/lvbian/article/details/14000359
            NetworkStream networkStream = tcpImageRevClient.GetStream();
            while (this.isImageClientReceive)
            {
                try
                {
                    tcpImageRevClient.ReceiveBufferSize = 32;//單一封包上限
                    byte[] firstBytes = new byte[tcpImageRevClient.ReceiveBufferSize];//封包陣列
                    int packetLength = networkStream.Read(firstBytes, 0, firstBytes.Length);//接收封包
                    int indexFlag = BitConverter.ToString(firstBytes).IndexOf("F1");//搜尋封包頭
                    if (indexFlag > -1)//表示取得表頭 F1
                    {
                        int flagPosition = (indexFlag / 3) + 1; //換算表頭 位址值

                        byte[] lastBytes = firstBytes.Take(flagPosition - 1).ToArray(); //前包數據
                        byte[] thisBytes = firstBytes.Skip(flagPosition + 4).Take(firstBytes.Length - lastBytes.Length).ToArray();//本包數據
                        byte[] byteslebgth = firstBytes.Skip(flagPosition).Take(4).ToArray();// 擷取數據長度 資訊  =  4 byte
                        Array.Reverse(byteslebgth);
                        int dataLength = int.Parse(BitConverter.ToString(byteslebgth).Replace("-", ""), System.Globalization.NumberStyles.HexNumber);//實際資料長度
                        //Console.WriteLine("前次數據 :" + BitConverter.ToString(lastBytes) +" ; "+ lastBytes.Length.ToString());
                        //Console.WriteLine("數據本體 : "+BitConverter.ToString(thisBytes) + "\r\n接收數據 : " + BitConverter.ToString(firstBytes));
                        //Console.WriteLine("實際資料長度 : "+ dataLength.ToString());
                        //Console.WriteLine("thisBytes: "+ thisBytes.Length.ToString()+"\r\n"+ "dataLength: "+ dataLength.ToString());
                        bool end = false;
                        int residual = dataLength;//殘餘資料長度
                        while (!end)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                byte[] bytesRead;
                                if (ms.CanRead)
                                {
                                    if (residual > thisBytes.Length)//殘餘值>首次數據 
                                    {
                                        bytesRead = new byte[thisBytes.Length];
                                        Array.Copy(thisBytes, 0, bytesRead, 0, bytesRead.Length); //取得首段資料
                                        ms.Write(bytesRead, 0, bytesRead.Length);//將接收值持續寫入記憶體
                                        residual = residual - thisBytes.Length;//拿完首包
                                        int packetNum = (int)(residual / tcpImageRevClient.ReceiveBufferSize);//完整包數
                                        int packetFinal = (int)(residual % tcpImageRevClient.ReceiveBufferSize);//餘數
                                        Console.WriteLine("次數" + packetNum.ToString() + "; 餘數" + packetFinal.ToString());
                                        for (int i = 0; i < packetNum; i++)//次數
                                        {
                                            bytesRead = new byte[tcpImageRevClient.ReceiveBufferSize];
                                            packetLength = networkStream.Read(bytesRead, 0, bytesRead.Length);//接收封包
                                            ms.Write(bytesRead, 0, bytesRead.Length);//將接收值持續寫入記憶體
                                        }
                                        //尾段數據
                                        bytesRead = new byte[packetFinal];
                                        packetLength = networkStream.Read(bytesRead, 0, bytesRead.Length);//取得尾段資料
                                        ms.Write(bytesRead, 0, bytesRead.Length);//將接收值持續寫入記憶體
                                        end = true;//收值完成
                                    }
                                    else if (thisBytes.Length > residual)//首次封包 即完全收到
                                    {
                                        bytesRead = new byte[dataLength];
                                        Array.Copy(thisBytes, 0, bytesRead, 0, bytesRead.Length); //取得首段資料                                    
                                        ms.Write(bytesRead, 0, bytesRead.Length);//將接收值持續寫入記憶體                                    
                                        end = true;//收值完成
                                    }
                                    Console.WriteLine("ms.Length:" + " : " + BitConverter.ToString(ms.ToArray()));
                                    if (ms.Length > 0)//https://blog.csdn.net/yuhijk2055/article/details/87935783
                                    {
                                        Image returnImage = Image.FromStream(ms); //exception occurs here
                                        Bitmap bitmap = new Bitmap(returnImage);
                                        OnReceiveImg?.Invoke(this, new ReciveImageArgs(bitmap));//只要相機開著就持續取像 送出影像
                                    }
                                }
                            }
                        }

                    }
                    if (!tcpImageRevClient.Connected)
                    {
                        isImageClientReceive = false;
                        Console.WriteLine("斷線");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString() + "ReciveImageERROR");
                }
                Thread.Sleep(1);
            }
            networkStream.Close(); networkStream.Dispose();
        }

        public virtual byte[] Bitmap2Array(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
        public virtual int Search(byte[] src, byte[] pattern)
        {
            int maxFirstCharSlot = src.Length - pattern.Length + 1;
            for (int i = 0; i < maxFirstCharSlot; i++)
            {
                if (src[i] != pattern[0]) // compare only first byte
                    continue;

                // found a match on first byte, now try to match rest of the pattern
                for (int j = pattern.Length - 1; j >= 1; j--)
                {
                    if (src[i + j] != pattern[j]) break;
                    if (j == 1) return i;
                }
            }
            return -1;
        }

    }
}
