using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TkDotNetScrewAoi.cameras;
using TkDotNetScrewAoi.view;
using System.Windows.Forms;
using System.IO;
using HalconDotNet;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace TkDotNetScrewAoi.module
{
    public enum ENUM_StreamStatus 
    {
        INIT,
        EMPTY,
        FULL,
        WRITE,
        COMPELET
    }

    public enum ENUM_InspectionStatus
    {
        INIT,
        RUN,
        STOP,
        ERROR
    }

    public enum ENUM_InspectionMode
    {
        INSPECT,
        TUNE,
        DEV,
    }

    public class Inspection
    {
        public DisplayInspection displayInspect { get; set; }//綁定前台
        private ENUM_InspectionMode enumInspectionMode_ = ENUM_InspectionMode.INSPECT;
        public ENUM_InspectionMode enumInspectionMode { get; set; }

        private int numberImageLoadTune_=0;//總照片數
        private int currentImageTune_ = 0;//當前照片
        public int currentImageTune
        {
            get { return currentImageTune_; }
            set 
            {
                currentImageTune_ = value;
                displayInspect.labelImageNumber.BeginInvoke(new Action(() => 
                {
                    displayInspect.labelImageNumber.Text = "共 "+ currentImageTune_.ToString() + "/"+numberImageLoadTune_.ToString()+" 張相片";
                }));
            }
        }

        public CameraOptHalcon Ccd1;
        public CameraOptHalcon Ccd2;
        public bool isStream = false;//串流是否開啟
        public bool isInspectionRun = false;//檢測是否啟用
        public bool isSave = false;//是否存圖
        public bool isDev=false;//是否開發者

        private string imagesDirDefect = string.Empty;// 瑕疵影像路徑
        private string imagesDirTune = string.Empty;// 調機影像路徑
        private Stopwatch stopwatch1 = new Stopwatch();
        private Stopwatch stopwatch2 = new Stopwatch();
        private Stopwatch stopwatch3 = new Stopwatch();
        private Stopwatch stopwatch4 = new Stopwatch();
        private Stopwatch stopwatch5 = new Stopwatch();
        public ConcurrentQueue<HObject> queueImageTuneTotal = new ConcurrentQueue<HObject>();//TuneTotal
        public ConcurrentQueue<HObject> queueImageTune1 = new ConcurrentQueue<HObject>();//Tune1
        public ConcurrentQueue<HObject> queueImageTune2 = new ConcurrentQueue<HObject>();//Tune2
        public ConcurrentQueue<HObject> queueImageTune3 = new ConcurrentQueue<HObject>();//Tune3
        public ConcurrentQueue<HObject> queueImageTune4 = new ConcurrentQueue<HObject>();//Tune4

        public ConcurrentQueue<HObject> queueImageCcd1= new ConcurrentQueue<HObject>();//Ccd1
        public ConcurrentQueue<HObject> queueImageCcd2 = new ConcurrentQueue<HObject>();//Ccd2
        public ConcurrentQueue<Bitmap> queueImageRoi1 = new ConcurrentQueue<Bitmap>();//Roi1
        public ConcurrentQueue<Bitmap> queueImageRoi2 = new ConcurrentQueue<Bitmap>();//Roi2
        public ConcurrentQueue<Bitmap> queueImageRoi3 = new ConcurrentQueue<Bitmap>();//Roi3
        public ConcurrentQueue<Bitmap> queueImageRoi4 = new ConcurrentQueue<Bitmap>();//Roi4

        Object lockSaveImage =new object();
        Object lockImage2Bit =new object();

        int numberImagePerCcd = 6; //單次拍攝照片數
        int numberImageCcd1 = 0;//拍到第幾張
        int numberImageCcd2 = 0;//拍到第幾張        
        int scepterflag = 0;//權杖
        public ImageMemoryStream imageMemoryStreams = new ImageMemoryStream(5,5);//存圖矩陣
        public int[,] memoryLength = new int[5, 5];//長度資訊 

        private void OnReceiveImgCcd1(object sender, ImageReceiveArgs e)
        {
            if (numberImageCcd1< numberImagePerCcd)
            {
                stopwatch1.Restart();
                queueImageCcd1.Enqueue(e.image.Clone());
                numberImageCcd1++;
            }
        }
        private void OnReceiveImgCcd2(object sender, ImageReceiveArgs e)
        {
            if (numberImageCcd2 < numberImagePerCcd)
            {                
                queueImageCcd2.Enqueue(e.image.Clone());
                numberImageCcd2++;
                Console.WriteLine(numberImageCcd2.ToString());
            }
        }

        public Inspection(DisplayInspection displayInspect_)
        {
            //https://dotblogs.com.tw/supershowwei/2017/01/22/004746
            imagesDirDefect = System.IO.Directory.GetCurrentDirectory() + @"..\..\..\imagesDefect\\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            if (!Directory.Exists(imagesDirDefect))
            {
                Directory.CreateDirectory(imagesDirDefect);
            }
            imagesDirTune = System.IO.Directory.GetCurrentDirectory() + @"..\..\..\imagesTunes\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(imagesDirTune))
            {
                Directory.CreateDirectory(imagesDirTune);
            }
            for (int i=0;i<4;i++)
            {
                HOperatorSet.SetColor(displayInspect_.hWindowRois[i].HalconWindow, "red");                
                HOperatorSet.SetColor(displayInspect_.hWindowBalls[i].HalconWindow, "red");
                HOperatorSet.SetPart(displayInspect_.hWindowBalls[i].HalconWindow, 0,0,500,500);
            }
            
            this.displayInspect = displayInspect_;
            this.Ccd1 =new CameraOptHalcon(displayInspect_.hWindowRois, displayInspect_.hWindowBalls,ENUM_CcdNumber.SECOND);
            this.Ccd2 =new CameraOptHalcon(displayInspect_.hWindowRois, displayInspect_.hWindowBalls, ENUM_CcdNumber.SECOND);
            this.Ccd1.OnReceiveImg+= new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd1);
            this.Ccd2.OnReceiveImg+=new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd2);
            isStream=true;
        }

        ~Inspection()
        {
            InspectionDispose();
        }

        public void Run()
        {
            //TODO CCD2 OPEN()
            if (Ccd1.enumGrabState != ENUM_GrabState.RUN)
            {
                Task.Run(new Action(() =>
                {
                    ImageCcd1QueueBuffer();
                    ImageCcd2QueueBuffer();
                }));
                this.Ccd1.Open();  
                //this.Ccd2.Open();
            }                                   
            else                   
            {                    
                Console.WriteLine("檢測流程開啟異常");              
            }
        }

        public void Stop()
        {
            if (Ccd1.enumGrabState == ENUM_GrabState.RUN)
            {
                this.Ccd1.enumGrabState = ENUM_GrabState.STOP;
                while (true)
                {
                    if (this.Ccd1.enumCameraState == ENUM_CameraState.CLOSE)
                        break;
                }
                InspectionDispose();
            }
            else
            {
                Console.WriteLine("檢測流程關閉異常");
            }
        }
       
        public void Dev()
        {
            HTuple hv_fileName = "/01_dataclassifiy";//圖像標籤
            HTuple hv_saveDirectory = "D:/00_ProgramRepository/04TkDotNetAoiScrewType/TkDotNetScrewAoi/TkDotNetScrewAoi/imageTuneComplete"+ hv_fileName;//存圖路徑
            Task.Run(new Action(() => { ImageDip(stopwatch1,1, queueImageTune1, displayInspect.hWindowRois[0], displayInspect.hWindowBalls[0],hv_saveDirectory); }));
            Task.Run(new Action(() => { ImageDip(stopwatch2,2, queueImageTune2, displayInspect.hWindowRois[1], displayInspect.hWindowBalls[1], hv_saveDirectory ); }));
            Task.Run(new Action(() => { ImageDip(stopwatch3,3, queueImageTune3, displayInspect.hWindowRois[2], displayInspect.hWindowBalls[2], hv_saveDirectory ); }));
            Task.Run(new Action(() => { ImageDip(stopwatch4,4, queueImageTune4, displayInspect.hWindowRois[3], displayInspect.hWindowBalls[3], hv_saveDirectory ); }));
            if(enumInspectionMode==ENUM_InspectionMode.DEV)
                Task.Run(new Action(() => { ImageLoad(); }));
        }

        public void ImageLoad()
        {
            HTuple hv_classFiles, hv_imageLoadList, hv_indexClass, hv_path, hv_indexImage;
            HObject ho_imageRaw;
            string sPath = Path.GetFullPath(imagesDirTune);
            HOperatorSet.ListFiles(sPath, "directories", out hv_classFiles);//搜尋子目錄
            for (hv_indexClass = 0; (int)hv_indexClass <= (int)((new HTuple(hv_classFiles.TupleLength())) - 1);
                hv_indexClass = (int)hv_indexClass + 1)
            {
                hv_path = hv_classFiles.TupleSelect(hv_indexClass);
                HOperatorSet.ListFiles(hv_path, (new HTuple("files")).TupleConcat("follow_links"), out hv_imageLoadList);
                numberImageLoadTune_=hv_imageLoadList.TupleLength();
                if ((int)(new HTuple((new HTuple(hv_imageLoadList.TupleLength())).TupleGreater(0))) != 0)
                {
                    for (hv_indexImage = 0; hv_indexImage.I <= ((new HTuple(hv_imageLoadList.TupleLength())).I - 1);
                        hv_indexImage = hv_indexImage.I + 1)
                    {
                        HOperatorSet.ReadImage(out ho_imageRaw, hv_imageLoadList.TupleSelect(hv_indexImage)); //*讀圖
                        currentImageTune = hv_indexImage.I+1;
                        switch (hv_indexImage.I % 4)
                        {
                            case 0:
                                queueImageTune1.Enqueue(ho_imageRaw); break;
                            case 1:
                                queueImageTune2.Enqueue(ho_imageRaw); break;
                            case 2:
                                queueImageTune3.Enqueue(ho_imageRaw); break;
                            case 3:
                                queueImageTune4.Enqueue(ho_imageRaw); break;
                        }
                        GC.Collect();
                        Thread.Sleep(1);
                    }
                }
            }
            while (queueImageTune1.Count>0 || queueImageTune2.Count > 0
                    || queueImageTune3.Count > 0 || queueImageTune4.Count > 0 )
            {  Thread.Sleep(1);   }
            isDev = false;
            enumInspectionMode = ENUM_InspectionMode.INSPECT;
            displayInspect.buttonRun.BeginInvoke(new Action(() => {displayInspect.buttonRun.Enabled=true; Console.WriteLine("調機完成"); }));
        }

        public void ImageDip(Stopwatch stopwatch,int sq,ConcurrentQueue<HObject> queues,HWindowControl hWindowControlRoi_, HWindowControl hWindowControlBall_,string path_)
        {
            HObject image_;
            while (true)
            {
                if (!queues.IsEmpty)
                {
                    stopwatch.Restart();
                    queues.TryDequeue(out image_);
                    Algorithm.GetBall(enumInspectionMode, lockSaveImage,hWindowControlBall_, image_,0,0,isSave, path_);
                    //HOperatorSet.DispObj(image_, hWindowControlRoi_.HalconWindow);
                    stopwatch.Stop();
                }
                Thread.Sleep(1);
            }
        }

        public void ImageCcd1QueueBuffer()
        {
            while (isStream)
            {
                if (!queueImageCcd1.IsEmpty)
                {
                    HObject image_;
                    if (queueImageCcd1.TryDequeue(out image_))
                    {
                        HObject hObjectRoi1 = Algorithm.RoiRegion(image_, 400, 800, 500);                            
                        HObject hObjectRoi2 = Algorithm.RoiRegion(image_, 800, 400, 500);        
                        queueImageRoi1.Enqueue(Algorithm.Hobjet2Bitmap32(hObjectRoi1));
                        queueImageRoi2.Enqueue(Algorithm.Hobjet2Bitmap32(hObjectRoi2));
                        //HObject hObjectRoi1 = Algorithm.RoiRegion(image_, 400, 800, 500);
                        //HObject hObjectRoi2 = Algorithm.RoiRegion(image_, 800, 400, 500);
                        //HOperatorSet.DispObj(image_, displayInspect.hWindowRois[2].HalconWindow);
                        //HOperatorSet.DispObj(image_, displayInspect.hWindowRois[3].HalconWindow);
                        //HOperatorSet.DispObj(hObjectRoi1, displayInspect.hWindowBalls[2].HalconWindow);
                        //HOperatorSet.DispObj(hObjectRoi2, displayInspect.hWindowBalls[3].HalconWindow);
                        stopwatch1.Stop();
                        
                        Thread.Sleep(1);
                    }
                    else
                    {
                        if (numberImageCcd1 == numberImagePerCcd)
                        {
                            numberImageCcd1 = 0;
                        }
                    }
                }
                else
                {
                    stopwatch2.Stop();
                    Console.WriteLine("圖數量: " + numberImageCcd1.ToString() + ", stopwatch1 ms :" + stopwatch1.ElapsedMilliseconds.ToString());
                    if (numberImageCcd1 == numberImagePerCcd)
                    {
                        stopwatch2.Restart();
                        numberImageCcd1 = 0;
                    }
                }
            }            
        }

        public void ImageCcd2QueueBuffer()
        {

            while (isStream)
            {
                if (!queueImageCcd2.IsEmpty)
                {
                    HObject image_;
                    if (queueImageCcd2.TryDequeue(out image_))
                    {
                        HObject hObjectRoi1 = Algorithm.RoiRegion(image_, 400, 800, 500);
                        HObject hObjectRoi2 = Algorithm.RoiRegion(image_, 800, 400, 500);
                        queueImageRoi3.Enqueue(Algorithm.Hobjet2Bitmap32(hObjectRoi1));
                        queueImageRoi4.Enqueue(Algorithm.Hobjet2Bitmap32(hObjectRoi2));
                        //HObject hObjectRoi1 = Algorithm.RoiRegion(image_, 400, 800, 500);
                        //HObject hObjectRoi2 = Algorithm.RoiRegion(image_, 800, 400, 500);
                        //HOperatorSet.DispObj(image_, displayInspect.hWindowRois[2].HalconWindow);
                        //HOperatorSet.DispObj(image_, displayInspect.hWindowRois[3].HalconWindow);
                        //HOperatorSet.DispObj(hObjectRoi1, displayInspect.hWindowBalls[2].HalconWindow);
                        //HOperatorSet.DispObj(hObjectRoi2, displayInspect.hWindowBalls[3].HalconWindow);
                        stopwatch2.Stop();
                        Thread.Sleep(1);
                        Console.WriteLine("stopwatch2 ms :" + stopwatch2.ElapsedMilliseconds.ToString());
                    }
                    else
                    {
                        if (numberImageCcd2 == numberImagePerCcd)
                        {
                            numberImageCcd2= 0;
                        }
                    }
                }
            }
        }

        public void ImageBall2Stream()
        {
            //TODO 權杖
            scepterflag = 0;
            Bitmap bitmap1,bitmap2, bitmap3, bitmap4;
            while (true)
            {
                if(queueImageRoi1.TryDequeue(out bitmap1)) 
                {
                    
                }                   
                if (queueImageRoi2.TryDequeue(out bitmap2))
                {
                }
                if(queueImageRoi3.TryDequeue(out bitmap3)) 
                {
                }
                if(queueImageRoi4.TryDequeue(out bitmap4)) 
                {
                }
                if(scepterflag < 5)
                    scepterflag++;
                else
                    scepterflag = 0;
            }
        }        

        public void MemoryWrite()
        {

        }

        public void InspectionDispose()
        {
            HObject hObject_;
            Bitmap bitmap_;
            while (true)
            {
                Console.WriteLine("移除所有儲列");
                queueImageTune1.TryDequeue(out hObject_);
                queueImageTune2.TryDequeue(out hObject_);
                queueImageTune3.TryDequeue(out hObject_);
                queueImageTune4.TryDequeue(out hObject_);
                queueImageCcd1.TryDequeue(out hObject_);
                queueImageCcd2.TryDequeue(out hObject_);
                queueImageRoi1.TryDequeue(out bitmap_);
                queueImageRoi2.TryDequeue(out bitmap_);
                queueImageRoi3.TryDequeue(out bitmap_);
                queueImageRoi4.TryDequeue(out bitmap_);
                if ((
                    queueImageRoi1.Count + queueImageRoi2.Count  + queueImageRoi3.Count  + queueImageRoi4.Count +
                    queueImageTune1.Count+ queueImageTune2.Count + queueImageTune3.Count + queueImageTune4.Count+
                    queueImageCcd1.Count + queueImageCcd2.Count)==0
                    )
                {
                        break;
                }
                Thread.Sleep(1);
            }
            GC.Collect();
            Console.WriteLine("移除所有儲列完成");
            Console.WriteLine(
                queueImageTune1.Count.ToString()+","+
                    queueImageTune2.Count.ToString() + "," +
                    queueImageTune3.Count.ToString() + "," +
                    queueImageTune4.Count.ToString() + "," +
                    queueImageCcd1.Count.ToString() + "," +
                    queueImageCcd2.Count.ToString() + "," 
                    );
        }
    }
}
