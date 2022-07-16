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

namespace TkDotNetScrewAoi.stream
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


        MemoryStreamStats memoryStreamStats = MemoryStreamStats.INIT;
        MemoryStream memoryStream;
        public ImageMemoryStream()
        {
            New();
        }
        public void New()
        {
            if (memoryStreamStats == MemoryStreamStats.INIT)
            {
                //
                memoryStream = new MemoryStream();
                memoryStreamStats = MemoryStreamStats.OPEN;

                Console.WriteLine("memoryStream New already");
            }
            else
            {
                Console.WriteLine("memoryStream is already");
            }
        }

        public void Write(Bitmap bitmap_,string fileName)
        {
            bitmap_.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            byte[] bytes = memoryStream.GetBuffer();  //byte[]   bytes=   ms.ToArray(); 
            memoryStream.Close();
            memoryStreamStats = MemoryStreamStats.CLOSE;
            var mmf = MemoryMappedFile.CreateOrOpen(fileName, bytes.Length, MemoryMappedFileAccess.ReadWrite);
            var viewAccessor = mmf.CreateViewAccessor(0, bytes.Length);
            memoryStreamStats = MemoryStreamStats.WRITE;
            viewAccessor.Write(0, bytes.Length); ;
            viewAccessor.WriteArray<byte>(0, bytes, 0, bytes.Length);
            HttpGet("http://127.0.0.1:8000/read_image/test1/" + bytes.Length.ToString());
            //TODO  POST "test1" LENGTH             
        }
        public void Close()
        {
            memoryStream.Close();
            memoryStream.Dispose();
            memoryStreamStats = MemoryStreamStats.DISPOSE;
        }

        public void HttpGet(string targetUrl)
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(targetUrl) as HttpWebRequest;
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 30000;

                string result = "";
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

        }

        ~ImageMemoryStream()
        {
            //dispose
        }
    }
}
