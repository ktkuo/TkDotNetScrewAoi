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
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace TkDotNetScrewAoi.module
{
    public enum MemoryStreamStats
    {
        INIT,
        WRITE,
        OPEN,
        CLOSE,
        DISPOSE,
    }
    public class ImageMemoryStream
    {
        private string fileName_ = "1_1";
        public string FileName
        {
            get { return fileName_; }
            set { fileName_ = value; }
        }

        public MemoryStream[] memoryStreams;//相片傳輸通道數量,一次24張相片

        /// <summary>
        /// 創建 ImageMemoryStream
        /// </summary>
        /// <param name="x"></param>
        public ImageMemoryStream(int x)
        {
            memoryStreams = new MemoryStream[x];
            MemoryStreamInit();
        }
        public MemoryStream New()
        {
            MemoryStream memoryStream = new MemoryStream();
            return memoryStream;
        }

        public async Task<string> Write(Bitmap bitmap_, int xfileName_)
        {
            string fileName_ = "F" + (xfileName_ + 1).ToString()+ "_" ;
            bitmap_.Save(memoryStreams[xfileName_], System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = memoryStreams[xfileName_].GetBuffer();  //byte[]   bytes=   ms.ToArray(); 
            memoryStreams[xfileName_].Close();
            var mmf = MemoryMappedFile.CreateOrOpen(fileName_, bytes.Length, MemoryMappedFileAccess.ReadWrite);
            var viewAccessor = mmf.CreateViewAccessor(0, bytes.Length);
            viewAccessor.Write(0, bytes.Length); ;
            viewAccessor.WriteArray<byte>(0, bytes, 0, bytes.Length);
            string s= fileName_ + ":"+ bytes.Length.ToString();
            return bytes.Length.ToString();
            //HttpGet("http://127.0.0.1:8000/read_image/test1/" + bytes.Length.ToString()); 
            //TODO  POST "test1" LENGTH             
        }
        public MemoryStream MemoryDispose(int xfileName_)
        {
            memoryStreams[xfileName_].Close();
            memoryStreams[xfileName_].Dispose();
            memoryStreams[xfileName_] = null;
            return memoryStreams[xfileName_];
        }

        public void MemoryStreamInit()
        {            
            for (int i = 0; i < memoryStreams.GetLength(0); i++)
            {
                memoryStreams[i] = New();
            }
        }

        public void MemoryStreamRestAll()
        {
            for (int i = 0; i < memoryStreams.GetLength(0); i++)
            {    
                memoryStreams[i] = MemoryDispose(i);
            }
        }

        public async Task<string> HttpGet(string targetUrl)
        {
            string result = "";
            try
            {
                //HttpClient client = new HttpClient();

                //var fooResult = await client.GetStringAsync(targetUrl);                
                HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 30000;
                // 取得回應資料
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GET/POST ERROR" + ex.Message);
            }
            return result;
        }

        ~ImageMemoryStream()
        {
            //dispose
        }
    }
}
