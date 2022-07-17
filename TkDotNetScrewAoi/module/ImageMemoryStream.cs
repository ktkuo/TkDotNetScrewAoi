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

        public MemoryStream[,] memoryStreams;

        public ImageMemoryStream(int x, int y)
        {
            memoryStreams = new MemoryStream[x, y];
            MemoryStreamInit();
        }
        public MemoryStream New()
        {
            MemoryStream memoryStream = new MemoryStream();
            return memoryStream;
        }

        public string Write(Bitmap bitmap_, int xfileName,int yfileName)
        {
            string fileName_ = xfileName.ToString() + "_" + yfileName.ToString();
            bitmap_.Save(memoryStreams[xfileName,yfileName], System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = memoryStreams[xfileName, yfileName].GetBuffer();  //byte[]   bytes=   ms.ToArray(); 
            memoryStreams[xfileName, yfileName].Close();
            var mmf = MemoryMappedFile.CreateOrOpen(fileName_, bytes.Length, MemoryMappedFileAccess.ReadWrite);
            var viewAccessor = mmf.CreateViewAccessor(0, bytes.Length);
            viewAccessor.Write(0, bytes.Length); ;
            viewAccessor.WriteArray<byte>(0, bytes, 0, bytes.Length);
            string s= fileName_ + ":"+ bytes.Length.ToString();
            return s;
            //HttpGet("http://127.0.0.1:8000/read_image/test1/" + bytes.Length.ToString()); 
            //TODO  POST "test1" LENGTH             
        }
        public MemoryStream MemoryDispose(MemoryStream memoryStream_)
        {
            memoryStream_.Close();
            memoryStream_.Dispose();
            memoryStream_ = null;
            return memoryStream_;
        }

        public void MemoryStreamInit()
        {            
            for (int i = 0; i < memoryStreams.GetLength(0); i++)
            {
                for (int j = 0; j < memoryStreams.GetLength(1); j++)
                {
                    memoryStreams[i, j] = New();
                    //Console.WriteLine("(" + i.ToString() + "," + j.ToString() + ")");
                }
            }
        }

        public void MemoryStreamRest()
        {
            for (int i = 0; i < memoryStreams.GetLength(0); i++)
            {
                for (int j = 0; j < memoryStreams.GetLength(1); j++)
                {
                    if(memoryStreams[i, j] != null)
                    {
                        memoryStreams[i, j] = MemoryDispose(memoryStreams[i, j]);
                    }                    
                }
            }
            if (memoryStreams[3, 3] == null)
            {
                Console.WriteLine("Yes");
            }
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
