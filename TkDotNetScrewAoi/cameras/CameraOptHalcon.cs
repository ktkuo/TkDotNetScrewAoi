using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TkDotNetScrewAoi.cameras;
using TkDotNetScrewAoi.controls;
using System.IO.Ports;
using TkDotNetScrewAoi.view;
using System.Windows.Forms;
using HalconDotNet;

namespace TkDotNetScrewAoi.cameras
{
    /*
         *  狀態 
         *  相機 存圖 取像  
         */

    public enum ENUM_CameraState
    {
        /*
         * 初始,觸發啟用,非觸發啟用,關閉相機,相機異常
         */
        INIT,
        OPEN_TRIGGER,
        OPEN_NOTRIGGER,
        CLOSE,
        ERROR,
    }

    public enum ENUM_ImageSaveMode
    {
        /*
         * 初始 全部儲存 瑕疵儲存 手動儲存 不儲存
         */
        INIT,
        SAVEALL,
        SAVEDEFECT,
        SAVEHANDLE,
        NONSAVE,
    }

    public enum ENUM_GrabState
    {
        /*
         * 初始 擷取 超時 停止 異常
         */
        INIT,
        RUN,
        TIMEOUT,
        STOP,
        ERROR,
    }

    public enum ENUM_CcdNumber
    {
        FIRST=0,
        SECOND=2,
    }

    /// <summary>
    /// 接收圖片事件
    /// </summary>
    public class ImageReceiveArgs : EventArgs
    {
        public HObject image;
        public ImageReceiveArgs(HObject img_)
        {
            this.image = img_;
        }
    }

    /// <summary>
    /// 相機狀態
    /// </summary>
    public class CameraStatusArgs : EventArgs
    {
        public ENUM_CameraState cameraState;
        public CameraStatusArgs(ENUM_CameraState cameraState_)
        {
            this.cameraState = cameraState_;
        }
    }

    /// <summary>
    /// 擷取狀態
    /// </summary>
    public class GrabStatusArgs : EventArgs
    {
        public ENUM_GrabState grabState;
        public GrabStatusArgs(ENUM_GrabState grabState_)
        {
            this.grabState = grabState_;
        }
    }


    //相機類
    public class CameraOptHalcon
    {
        public event EventHandler<ImageReceiveArgs>  OnReceiveImg;// 接收圖片主事件  OnReceiveImg
        public event EventHandler<CameraStatusArgs>  OnCameraSate;// 相機狀態主事件  OnCameraSate
        public event EventHandler<GrabStatusArgs>    OnGrabStatus;//   擷取狀態主事件  OnGrabStatus
        public HTuple hv_AcqHandle;//相機裝置
        //初始化
        public ENUM_GrabState enumGrabState = ENUM_GrabState.INIT;
        public ENUM_CameraState enumCameraState = ENUM_CameraState.INIT;
        public ENUM_ImageSaveMode enumImageSaveMode = ENUM_ImageSaveMode.INIT;        
        public HObject imageGet=null;
        public int IntervalGrab=20; //取像週期
        private string nameCcd_= "MachineVision:7K0108EPAK00005";
        public string NameCcd { get { return nameCcd_; } set { nameCcd_ = value; } }
        private bool isTrigger_= false;//硬體觸發
        private bool isGrabWaitForNextGroup_ = false;//6張相片
        private bool isTesting_ = false;
        public bool isTesting { get { return isTesting_; } set { isTesting_ = value; } }
        public bool isTrigger { get { return isTrigger_; } set { isTrigger_ = value; } }
        public bool isGrabWaitForNextGroup { get { return isGrabWaitForNextGroup_; } set { isGrabWaitForNextGroup_ = value; } }
        private double exposureTime_ = 1800;
        public double exposureTime { get { return exposureTime_; } set { exposureTime_ = value; } }
        public ConcurrentQueue<HObject> queueImageTrans = new ConcurrentQueue<HObject>();//圖片傳輸
        public ConcurrentQueue<HObject> queueImageSave = new ConcurrentQueue<HObject>();//圖片儲存
        HalconDotNet.HWindowControl[] hWindowControlsRois, hWindowControlsBalls;
        private ENUM_CcdNumber enumCcdNumber;
        public bool isWait { get; set; }
        public CameraOptHalcon(HalconDotNet.HWindowControl[] hWindowControlsRois_, HalconDotNet.HWindowControl[] hWindowControlsBalls_, ENUM_CcdNumber enumCcdNumber_)
        {
            this.hWindowControlsRois = hWindowControlsRois_;
            this.hWindowControlsBalls = hWindowControlsBalls_;
            this.enumCcdNumber = enumCcdNumber_;
        }

        /// <summary>
        /// 相機連線確認
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOpen()
        {
            if (this.enumCameraState == ENUM_CameraState.OPEN_TRIGGER || this.enumCameraState == ENUM_CameraState.OPEN_NOTRIGGER)
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        /// 取得資訊
        /// </summary>
        public void GetInfo()
        {
            try
            {
                HTuple hv_Information = null, hv_ValueList = null;
                HOperatorSet.InfoFramegrabber("USB3Vision", "info_boards", out hv_Information, out hv_ValueList);
                Console.WriteLine((string)hv_ValueList);
            }
            catch(Exception ex)
            {
                Console.WriteLine("取得資訊異常"+ex.ToString());
                this.enumCameraState = ENUM_CameraState.ERROR;
            }            
        }

        /// <summary>
        /// 相機資訊初始化
        /// </summary>
        public void Init()
        {
            
        }

        public void Open()
        {
            try
            {
                if (!IsOpen())
                {
                    if (isTrigger)
                    {
                        HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", NameCcd, 0, -1, out hv_AcqHandle);
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "On");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Line1");
                        //HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerActivation", "RisingEdge");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "Continuous");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSelector", "FrameStart");
                        this.enumCameraState = ENUM_CameraState.OPEN_TRIGGER;
                    }
                    else
                    {
                        HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", NameCcd, 0, -1, out hv_AcqHandle);
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                        this.enumCameraState = ENUM_CameraState.OPEN_NOTRIGGER;
                    }
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", (HTuple)this.exposureTime);
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", -1);
                    HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
                    this.enumGrabState = ENUM_GrabState.RUN;
                    Task.Run(new Action(() =>
                    {
                        Run();
                    }));
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Close()
        {
            try
            {      
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");                    
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");                    
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                this.enumCameraState = ENUM_CameraState.CLOSE;//關閉CCD成功
            }
            catch (Exception ex)
            {
                Console.WriteLine("CLOSE CCD ERROR" +ex.Message);
            }
        }

        public void Run() 
        {
            Task.Run(new Action(() => 
            {
                HObject hoImage_;
                while (this.enumGrabState == ENUM_GrabState.RUN)
                {
                    try
                    {                            
                        if (this.enumCameraState == ENUM_CameraState.OPEN_TRIGGER || this.enumCameraState == ENUM_CameraState.OPEN_NOTRIGGER)
                        {
                            isWait = true;
                            HOperatorSet.GrabImageAsync(out hoImage_, hv_AcqHandle, -1);
                            //HOperatorSet.ReadImage(out hoImage_, @"D:\\02當前程式\\01TkAoiScrew\\TkDotNetScrewAoi\\TkDotNetScrewAoi\\imagesTunes\\2022-07-20\\1.bmp"); //*讀圖
                            //Console.WriteLine("相片來了");
                            OnReceiveImg?.Invoke(this, new ImageReceiveArgs(hoImage_));//只要相機開著就持續取像 送出影像        
                            //HOperatorSet.DispObj(hoImage_, this.hWindowControlsRois[(int)enumCcdNumber].HalconWindow);         
                            //HOperatorSet.DispObj(hoImage_, this.hWindowControlsRois[(int)enumCcdNumber +1].HalconWindow);
                            Thread.Sleep(1);
                            //Thread.Sleep(IntervalGrab);//取樣週期
                            //HTuple ww, hh; HOperatorSet.GetImageSize(hoImage_,out ww ,out hh);Console.WriteLine(ww.D.ToString()+","+hh.D.ToString());
                        }                        
                    }
                        
                    catch (Exception ex)                        
                    {                            
                        Thread.Sleep(1);                            
                        //TODO 通常是TimeOut                            
                        Console.WriteLine("取像延遲 : " + ex.ToString());                        
                    }
                }   

                Console.WriteLine("離開取向迴圈");
                //關閉相機
                while (this.enumCameraState != ENUM_CameraState.CLOSE)
                {
                    try
                    {
                        this.Close();
                        Console.WriteLine("相機關閉完成");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("相機關閉失敗"+ex.ToString());
                    }
                }
            }));
        }
    }
}
