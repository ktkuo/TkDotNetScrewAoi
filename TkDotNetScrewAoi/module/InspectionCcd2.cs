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
using TkDotNetScrewAoi.controls;

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

    public class InspectionCcd2
    {
        private DisplayInspection displayInspect { get; set; }//綁定前台
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
        int scepterflag = 0;//權杖 /*權杖由0開始*/
        public ImageMemoryStream imageMemoryStreams = new ImageMemoryStream(24);//存圖 陣列 24圖
        public int[,] scepterflagMatrix = new int[22, 22];
        public int[] memoryLength = new int[24];//長度
        private bool isCcd1Over = false,isCcd2Over=false
            , isImageComplete=false;//相片是否完整

        private string pathSvae_ = @"//image";
        public string pathSvae { get; set; }
        private Sockets plc { get; set; }

        public HObject hObjectBuf=null;
        private void OnReceiveImgCcd1(object sender, ImageReceiveArgs e)
        {
            if (numberImageCcd1< numberImagePerCcd)
            {
                queueImageCcd1.Enqueue(e.image.Clone());//CCD 照片
                numberImageCcd1++;
            }       
        }
        private void OnReceiveImgCcd2(object sender, ImageReceiveArgs e)
        {
            if (numberImageCcd2 < numberImagePerCcd)
            {
                queueImageCcd2.Enqueue(e.image.Clone());//CCD 照片
                numberImageCcd2++;
            }
        }
        

        public InspectionCcd2(DisplayInspection displayInspect_, Sockets sockets_)
        {
            this.plc = sockets_;
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
            this.Ccd1 =new CameraOptHalcon(displayInspect_.hWindowRois, displayInspect_.hWindowBalls,ENUM_CcdNumber.FIRST);
            //this.Ccd2 =new CameraOptHalcon(displayInspect_.hWindowRois, displayInspect_.hWindowBalls, ENUM_CcdNumber.SECOND);
            this.Ccd1.OnReceiveImg+= new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd1);
            //this.Ccd2.OnReceiveImg+=new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd2);
            isStream=true;//使用串流
        }

        ~InspectionCcd2()
        {
             InspectionDispose();
        }

        public void Start()
        {
            this.Ccd1.Open();
            numberImageCcd1 = 0; numberImageCcd2 = 0;
            InspectionDispose();
            //this.Ccd2.Open();
            bool isfirst_ = true;
            bool isRever =false;
            if (Ccd1.enumGrabState == ENUM_GrabState.RUN)
            {
                /*
                 * 1.將所有的相片提取出queue
                 * 2.統計是否到達目標張數
                 */
                Task.Run(async () =>
                {
                    HObject hObject1_, hObject2_;
                    List<HObject> hImages_ = new List<HObject>(new HObject[24]);//張相片
                    int numberImage = 0;
                    while (isInspectionRun)//是否檢測流程
                    {
                        
                        //Console.WriteLine("是否檢測流程"+ isInspectionRun.ToString());                        
                        while (true && isfirst_ == true)
                        {
                            Console.WriteLine("等待觸發1");
                            if (Ccd1.isWait && isfirst_ == true)
                            {
                                this.plc.Send(CmdScrewSelf.Instance.servoMotorPitchGrab);
                                Ccd1.isWait = false;
                                isfirst_ = false;                                
                                break;
                            }
                        }
                        if (numberImageCcd1 == numberImagePerCcd )//單隻相機集滿6張queue
                        {
                            stopwatch1.Restart();
                            stopwatch2.Restart();
                            //numberImageCcd1 == numberImagePerCcd && numberImageCcd2==numberImagePerCcd //單隻相機集滿12張queue
                            /*啟動前處理流程*/
                            for (int i = 0; i < 12; i++)//12張相片
                            {
                                if (i<6)
                                {
                                    if (queueImageCcd1.TryDequeue(out hObject1_))
                                    {
                                        //(X,Y,SIZE)
                                        stopwatch4.Restart();
                                        hImages_[i]   = await Algorithm.RoiRegion(displayInspect.hWindowRois[0], hObject1_, 983.636, 7.26321, 1526.78, 856.041);//將相機照片切為2個ROI-1
                                        stopwatch4.Stop(); Console.WriteLine("RoiRegion時間 : " + stopwatch4.ElapsedMilliseconds.ToString()); stopwatch4.Restart();
                                        hImages_[i+6] = await Algorithm.RoiRegion(displayInspect.hWindowRois[1],hObject1_, 983.636, 7.26321, 1526.78, 856.041);//將相機照片切為2個ROI-2
                                        stopwatch4.Stop(); Console.WriteLine("RoiRegion時間 : " + stopwatch4.ElapsedMilliseconds.ToString());
                                        numberImage++;
                                    }
                                }
                                else
                                {
                                    //if (queueImageCcd2.TryDequeue(out hObject2_))
                                    //{
                                    //    //(X,Y,SIZE)
                                    //    hImages_[i+6]  = await Algorithm.RoiRegion(hObject2_, 400, 800, 500);//將相機照片切為2個ROI-3
                                    //    hImages_[i+12] = await Algorithm.RoiRegion(hObject2_, 400, 800, 500);//將相機照片切為2個ROI-4
                                    //    numberImage++;
                                    //}
                                }
                                
                            }
                            
                            if (numberImage==6)//所有相片不為空  實際為12張
                            {
                                isImageComplete = true;//Console.WriteLine("相片集滿");
                                stopwatch1.Stop(); Console.WriteLine("圖片提取時間 : " + stopwatch1.ElapsedMilliseconds.ToString());
                            }
                            else
                            {
                                isImageComplete = false;
                            }

                            if (isImageComplete)
                            {
                                stopwatch1.Restart();
                                try
                                {
                                    stopwatch1.Restart();
                                    numberImageCcd1 = 0; numberImageCcd2 = 0;
                                    var task1 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 0);
                                    var task2 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 1);
                                    var task3 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 2);
                                    var task4 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 3);
                                    var task5 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 4);
                                    var task6 = GetBalltoStream(displayInspect.hWindowBalls[0], hImages_[0], 5);
                                    var task7 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 6);
                                    var task8 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 7);
                                    var task9 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 8);
                                    var task10 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 9);
                                    var task11 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 10);
                                    var task12 = GetBalltoStream(displayInspect.hWindowBalls[1], hImages_[1], 11);
                                    var task13 = GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 12);
                                    var task14 = GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 13);
                                    var task15 = GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 14);
                                    var task16= GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 15);
                                    var task17 = GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 16);
                                    var task18 = GetBalltoStream(displayInspect.hWindowBalls[2], hImages_[2], 17);
                                    var task19= GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 18);
                                    var task20 = GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 19);
                                    var task21 = GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 20);
                                    var task22 = GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 21);
                                    var task23 = GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 22);
                                    var task24 = GetBalltoStream(displayInspect.hWindowBalls[3], hImages_[3], 23);
                                    await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10
                                        , task11, task12, task13, task14, task15, task16, task17, task18, task19, task20
                                        , task21, task22, task23, task24);
                                    stopwatch1.Stop(); Console.WriteLine("異步完成時間ms : " + stopwatch1.ElapsedMilliseconds.ToString());

                                    //HOperatorSet.DispObj(hImages_[0], displayInspect.hWindowRois[0].HalconWindow);
                                    /*
                                    var task1 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[0], 0, 0, false, pathSvae);
                                    var task2 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[1], 0, 0, false, pathSvae);
                                    var task3 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[2], 0, 0, false, pathSvae);
                                    var task4 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[3], 0, 0, false, pathSvae);
                                    var task5 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[4], 0, 0, false, pathSvae);
                                    var task6 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[0], hImages_[5], 0, 0, false, pathSvae);
                                    var task7 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[6], 0, 0, false, pathSvae);
                                    var task8 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[7], 0, 0, false, pathSvae);
                                    var task9 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[8], 0, 0, false, pathSvae);
                                    var task10 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[9], 0, 0, false, pathSvae);
                                    var task11 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[10], 0, 0, false, pathSvae);
                                    var task12 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[1], hImages_[11], 0, 0, false, pathSvae);
                                    var task13 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[0], 0, 0, false, pathSvae);
                                    var task14 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[1], 0, 0, false, pathSvae);
                                    var task15 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[2], 0, 0, false, pathSvae);
                                    var task16 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[3], 0, 0, false, pathSvae);
                                    var task17 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[4], 0, 0, false, pathSvae);
                                    var task18 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[2], hImages_[5], 0, 0, false, pathSvae);
                                    var task19 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[6], 0, 0, false, pathSvae);
                                    var task20 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[7], 0, 0, false, pathSvae);
                                    var task21 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[8], 0, 0, false, pathSvae);
                                    var task22 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[9], 0, 0, false, pathSvae);
                                    var task23 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[10], 0, 0, false, pathSvae);
                                    var task24 = Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage, displayInspect.hWindowBalls[3], hImages_[11], 0, 0, false, pathSvae);
                                    await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10
                                        ,task11,task12,task13,task14,task15,task16,task17,task18,task19,task20
                                        ,task21,task22,task23,task24);

                                    var taskStream1 = imageMemoryStreams.WriteAsync(task1.Result,        0);
                                    var taskStream2 = imageMemoryStreams.WriteAsync(task2.Result,        1);
                                    var taskStream3 = imageMemoryStreams.WriteAsync(task3.Result,        2);
                                    var taskStream4 = imageMemoryStreams.WriteAsync(task4.Result,        3);
                                    var taskStream5 = imageMemoryStreams.WriteAsync(task5.Result,        4);
                                    var taskStream6 = imageMemoryStreams.WriteAsync(task6.Result,        5);
                                    var taskStream7 = imageMemoryStreams.WriteAsync(task7.Result,        6);
                                    var taskStream8 = imageMemoryStreams.WriteAsync(task8.Result,        7);
                                    var taskStream9 = imageMemoryStreams.WriteAsync(task9.Result,        8);
                                    var taskStream10 = imageMemoryStreams.WriteAsync(task10.Result,      9);
                                    var taskStream11 = imageMemoryStreams.WriteAsync(task11.Result,      10);
                                    var taskStream12 = imageMemoryStreams.WriteAsync(task12.Result,      11);
                                    var taskStream13 = imageMemoryStreams.WriteAsync(task13.Result    ,  12);
                                    var taskStream14 = imageMemoryStreams.WriteAsync(task14.Result    ,  13);
                                    var taskStream15 = imageMemoryStreams.WriteAsync(task15.Result    ,  14);
                                    var taskStream16 = imageMemoryStreams.WriteAsync(task16.Result    ,  15);
                                    var taskStream17 = imageMemoryStreams.WriteAsync(task17.Result    ,  16);
                                    var taskStream18 = imageMemoryStreams.WriteAsync(task18.Result    ,  17);
                                    var taskStream19 = imageMemoryStreams.WriteAsync(task19.Result    ,  18);
                                    var taskStream20 = imageMemoryStreams.WriteAsync(task20.Result    ,  19);
                                    var taskStream21= imageMemoryStreams.WriteAsync( task21.Result    ,  20);
                                    var taskStream22= imageMemoryStreams.WriteAsync( task22.Result    ,  21);
                                    var taskStream23= imageMemoryStreams.WriteAsync( task23.Result    ,  22);
                                    var taskStream24= imageMemoryStreams.WriteAsync( task24.Result    ,  23);
                                    await Task.WhenAll(taskStream1, taskStream2, taskStream3, taskStream4, taskStream5, taskStream6, taskStream7,
                                        taskStream8, taskStream9, taskStream10, taskStream11, taskStream12, taskStream13, taskStream14, taskStream15, taskStream16
                                        , taskStream17, taskStream18, taskStream19, taskStream20, taskStream21, taskStream22, taskStream23, taskStream24);
                                    
                                    string[] requestLength = new string[] { 
                                        taskStream1.Result, taskStream2.Result, taskStream3.Result, taskStream4.Result, taskStream5.Result,
                                        taskStream6.Result, taskStream7.Result, taskStream8.Result, taskStream9.Result,
                                        taskStream10.Result, taskStream11.Result, taskStream12.Result, taskStream13.Result,
                                        taskStream14.Result,taskStream15.Result,taskStream16.Result,taskStream17.Result,taskStream18.Result,taskStream19.Result,taskStream20.Result,
                                        taskStream21.Result, taskStream22.Result, taskStream23.Result, taskStream24.Result};
                                    */

                                    //string result = await imageMemoryStreams.HttpGet("http:/127.0.0.1:8000/makePredictionBatch/" +
                                    //    taskStream1.Result + "/" +
                                    //    taskStream2.Result + "/" +
                                    //    taskStream3.Result + "/" +
                                    //    taskStream4.Result + "/" +
                                    //    taskStream5.Result + "/" +
                                    //    taskStream6.Result + "/" +
                                    //    taskStream7.Result + "/" +
                                    //    taskStream8.Result + "/" +
                                    //    taskStream9.Result + "/" +
                                    //    taskStream10.Result + "/" +
                                    //    taskStream11.Result + "/" +
                                    //    taskStream12.Result + "/"
                                    //    taskStream13.Result + "/" +
                                    //    taskStream14.Result + "/" +
                                    //    taskStream15.Result + "/" +
                                    //    taskStream16.Result + "/" +
                                    //    taskStream17.Result + "/" +
                                    //    taskStream18.Result + "/" +
                                    //    taskStream19.Result + "/" +
                                    //    taskStream20.Result + "/" +
                                    //    taskStream21.Result + "/" +
                                    //    taskStream22.Result + "/" +
                                    //    taskStream23.Result + "/" +
                                    //    taskStream24.Result + "/"
                                    //    );

                                    bool sort_ = await InspectionMatrix(scepterflag, "@1@2@3@4@");                                    
                                    scepterflag++;
                                    if (scepterflag == 22)
                                    {
                                        scepterflag = 0;
                                    }
                                    imageMemoryStreams.MemoryStreamInit();
                                    while (true && isfirst_ == false)
                                    {
                                        //Console.WriteLine("等待觸發2");
                                        if (Ccd1.isWait == true && isfirst_ == false)
                                        {
                                            if (!isRever)
                                            {
                                                this.plc.Send(CmdScrewSelf.Instance.servoMotorPitchGrabR);
                                                isRever = !isRever;

                                            }
                                            else
                                            {
                                                this.plc.Send(CmdScrewSelf.Instance.servoMotorPitchGrab);
                                                isRever = !isRever;
                                            }
                                            isfirst_ = false;
                                            Ccd1.isWait = false;
                                            break;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.ToString());
                                }           
                            }
                            stopwatch1.Stop(); Console.WriteLine("前處理時間 : " + stopwatch1.ElapsedMilliseconds.ToString());
                            stopwatch2.Stop(); Console.WriteLine("總時間 : " + stopwatch2.ElapsedMilliseconds.ToString());
                            numberImage = 0;
                            GC.Collect();
                        }
                        else
                        {
                            //Console.WriteLine("等待取像");
                        }
                    }
                    Console.WriteLine("離開檢測流程");
                });
                
            }                                   
            else                   
            {                    
                Console.WriteLine("檢測流程開啟異常");              
            }
        }

        public void Stop()
        {
            if (Ccd1.enumGrabState == ENUM_GrabState.RUN)//關閉相機流程
            {
                this.Ccd1.enumGrabState = ENUM_GrabState.STOP;                
                InspectionDispose();
            }
            else
            {
                Console.WriteLine("檢測流程關閉異常");
            }
        }

        public async Task<bool> GetBalltoStream(HalconDotNet.HWindowControl hWindowControl_,HObject hImages_,int imageMemoryStreamsNumber_)
        {
            //GetBall(Object lock_, HalconDotNet.HWindowControl hWindowControl, HObject ho_imageRaw,bool isSave_, string dirSave)
            stopwatch3.Restart();
            Bitmap bitmap = Algorithm.GetBall(lockSaveImage, hWindowControl_, hImages_, false, pathSvae);
            stopwatch3.Stop(); Console.WriteLine("GetBall完成時間ms : " + stopwatch3.ElapsedMilliseconds.ToString()); stopwatch3.Restart();
            string fileInfomation_ = imageMemoryStreams.Write(bitmap, imageMemoryStreamsNumber_);//string s = fileName_ + ":" + bytes.Length.ToString();
            stopwatch3.Stop(); Console.WriteLine("imageMemoryStreams完成時間ms : " + stopwatch3.ElapsedMilliseconds.ToString());
            return true;
        }

        /// <summary>
        /// 分料矩陣
        /// </summary>
        /// <param name="scepterflag_"></param>
        /// <param name="figureArray_"></param>
        /// <returns></returns>
        public async Task<bool> InspectionMatrix(int scepterflag_, string figureArray_)
        {
            await Task.Delay(10);
            int[] ints = new int[4] { 21, 20, 1, 0 };
            /*權杖由1開始*/
            string[] strings_ = figureArray_.Split('@');
            for (int i = 0; i < 4; i++)//將辨識結果儲存於矩陣中
            {
                if (i < 2)
                {
                    scepterflagMatrix[scepterflag_, i] = Int16.Parse(strings_[i + 1]);//將辨識結果儲存於矩陣中
                }
                else
                {
                    scepterflagMatrix[scepterflag_, i + 18] = Int16.Parse(strings_[i + 1]);//將辨識結果儲存於矩陣中
                }
            }

            //取出結果
            for (int i = 0; i < 4; i++)

            {
                if (scepterflagMatrix[scepterflag_, ints[i]] != 0)
                {
                    //NG PLC
                    return false;
                }
                scepterflag_ = scepterflag_ - 1;
                if (scepterflag_ == -1)
                {
                    scepterflag_ = 21;
                }
            }
            //OK PLC
            return true;
        }

        /// <summary>
        /// 將所有的Queue清空
        /// </summary>
        public void InspectionDispose()
        {
            HObject hObject_;
            Bitmap bitmap_;
            while (true)
            {
                queueImageTune1.TryDequeue(out hObject_);                  //InspectionDispose
                queueImageTune2.TryDequeue(out hObject_);                  //InspectionDispose
                queueImageTune3.TryDequeue(out hObject_);                  //InspectionDispose
                queueImageTune4.TryDequeue(out hObject_);                  //InspectionDispose
                queueImageCcd1.TryDequeue(out hObject_);                   //InspectionDispose
                queueImageCcd2.TryDequeue(out hObject_);                   //InspectionDispose
                queueImageRoi1.TryDequeue(out bitmap_);                    //InspectionDispose
                queueImageRoi2.TryDequeue(out bitmap_);                    //InspectionDispose
                queueImageRoi3.TryDequeue(out bitmap_);                    //InspectionDispose
                queueImageRoi4.TryDequeue(out bitmap_);                    //InspectionDispose
                if ((
                    queueImageRoi1.Count + queueImageRoi2.Count + queueImageRoi3.Count + queueImageRoi4.Count +   //InspectionDispose
                    queueImageTune1.Count + queueImageTune2.Count + queueImageTune3.Count + queueImageTune4.Count +   //InspectionDispose
                    queueImageCcd1.Count + queueImageCcd2.Count) == 0 //InspectionDispose
                    )//InspectionDispose
                {
                    break;
                }
                Thread.Sleep(1);
            }
            GC.Collect();
        }

        /// <summary>
        /// 開發者模式
        /// </summary>
        public void Dev()
        {
            HTuple hv_fileName = "/01_dataclassifiy";//圖像標籤
            HTuple hv_saveDirectory = "\\192.168.3.190\\TK_Quality\\03_天工圖庫(僅供冠廷編輯與操作)\\04_20220704_1p2mm\\01Data\\" + hv_fileName;//存圖路徑
            Task.Run(new Action(() => { ImageDip(stopwatch1,1, queueImageTune1, displayInspect.hWindowRois[0], displayInspect.hWindowBalls[0],hv_saveDirectory); }));
            Task.Run(new Action(() => { ImageDip(stopwatch2,2, queueImageTune2, displayInspect.hWindowRois[1], displayInspect.hWindowBalls[1], hv_saveDirectory ); }));
            Task.Run(new Action(() => { ImageDip(stopwatch3,3, queueImageTune3, displayInspect.hWindowRois[2], displayInspect.hWindowBalls[2], hv_saveDirectory ); }));
            Task.Run(new Action(() => { ImageDip(stopwatch4,4, queueImageTune4, displayInspect.hWindowRois[3], displayInspect.hWindowBalls[3], hv_saveDirectory ); }));
            if(enumInspectionMode==ENUM_InspectionMode.DEV)
                Task.Run(new Action(() => { ImageLoad(); }));
        }

        /// <summary>
        /// 開發者 讀圖
        /// </summary>
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
                hv_path= "\\192.168.3.190\\TK_Quality\\03_天工圖庫(僅供冠廷編輯與操作)\\04_20220704_1p2mm\\01Data\\1.2mm OK珠\\";
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

        /// <summary>
        /// 開發者模式 調機用
        /// </summary>
        /// <param name="stopwatch"></param>
        /// <param name="sq"></param>
        /// <param name="queues"></param>
        /// <param name="hWindowControlRoi_"></param>
        /// <param name="hWindowControlBall_"></param>
        /// <param name="path_"></param>
        public async void ImageDip(Stopwatch stopwatch,int sq,ConcurrentQueue<HObject> queues,HWindowControl hWindowControlRoi_, HWindowControl hWindowControlBall_,string path_)
        {
            HObject image_;
            while (true)
            {
                if (!queues.IsEmpty)
                {
                    queues.TryDequeue(out image_);
                    await Algorithm.GetBallAsync(enumInspectionMode, lockSaveImage,hWindowControlBall_, image_,0,0,isSave, path_);
                    //HOperatorSet.DispObj(image_, hWindowControlRoi_.HalconWindow);
                }
                Thread.Sleep(1);
            }
        }
          
    }
}
