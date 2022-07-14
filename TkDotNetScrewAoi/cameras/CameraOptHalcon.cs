using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HalconDotNet;
using TkDotNetScrewAoi.cameras;

namespace TkDotNetScrewAoi.cameras
{
    /*
         *  狀態 
         *  相機 存圖 取像  
         */

    public enum enumCameraState
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

    public enum enumImageSaveMode
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

    public enum enumGrabState
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

    /// <summary>
    /// 接收圖片事件
    /// </summary>
    public class ImageReceiveArgs : EventArgs
    {
        public HObject img_;
        public ImageReceiveArgs(HObject img_)
        {
            this.img_ = img_;
        }
    }

    /// <summary>
    /// 相機狀態
    /// </summary>
    public class CameraStatusArgs : EventArgs
    {
        public enumCameraState cameraState;
        public CameraStatusArgs(enumCameraState cameraState_)
        {
            this.cameraState = cameraState_;
        }
    }

    /// <summary>
    /// 擷取狀態
    /// </summary>
    public class GrabStatusArgs : EventArgs
    {
        public enumGrabState grabState;
        public GrabStatusArgs(enumGrabState grabState_)
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

        //初始化
        public enumGrabState grabState = enumGrabState.INIT;
        public enumCameraState cameraState = enumCameraState.INIT;
        public enumImageSaveMode imageSaveMode=enumImageSaveMode.INIT;
        public HTuple hv_AcqHandle;//相機裝置
        public HObject imageGet=null;
        public int IntervalGrab=20; //取像週期
        public CameraOptDisplay ccdDisplay {get; set;}

        private string nameCcd_= "MachineVision:7K0108EPAK00004";
        public string NameCcd { get { return nameCcd_; } set { nameCcd_ = value; } }

        private bool isTrigger_= false;
        public bool isTrigger { get { return isTrigger_; } set { isTrigger_ = value; } }

        private double exposureTime_ = 1800;
        public double exposureTime { get { return exposureTime_; } set { exposureTime_ = value; } }


        public ConcurrentQueue<HObject> queueImageTrans = new ConcurrentQueue<HObject>();//圖片傳輸
        public ConcurrentQueue<HObject> queueImageSave = new ConcurrentQueue<HObject>();//圖片儲存

        public CameraOptHalcon()
        {

        }

        /// <summary>
        /// 相機連線確認
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOpen()
        {
            if (this.cameraState == enumCameraState.OPEN_TRIGGER || this.cameraState == enumCameraState.OPEN_NOTRIGGER)
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
                this.cameraState = enumCameraState.ERROR;
            }            
        }

        /// <summary>
        /// 相機資訊初始化
        /// </summary>
        public void Init()
        {
            try
            {
                if (this.hv_AcqHandle == null || this.cameraState == enumCameraState.INIT)//程式第一次連線
                {
                }
                else if (this.cameraState == enumCameraState.CLOSE)
                {
                }
                else if (this.cameraState == enumCameraState.OPEN_TRIGGER || this.cameraState == enumCameraState.OPEN_NOTRIGGER)
                {
                    //TODO 關閉TRIGGER 在關閉                
                }
                else if (this.cameraState == enumCameraState.ERROR)
                {
                    //TODO 異常後重設
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("相機初始化異常"+ex.ToString());
                this.cameraState=enumCameraState.ERROR;
            }            
        }

        /// <summary>
        /// 相機連線
        /// </summary>
        public void Connect()
        {
            try
            {             
                if (isTrigger)
                {
                    HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", NameCcd, 0, -1, out this.hv_AcqHandle);
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "TriggerMode", "On");
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "TriggerSource", "Line1");
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "AcquisitionMode", "Continuous");
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "TriggerSelector", "FrameStart");
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "TriggerActivation", "RisingEdge");
                    this.cameraState = enumCameraState.OPEN_TRIGGER;
                }
                else
                {
                    HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive", -1, "default", -1, "false", "default", NameCcd, 0, -1, out this.hv_AcqHandle);
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                    this.cameraState = enumCameraState.OPEN_NOTRIGGER;
                }
                HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "ExposureMode", "Timed");
                HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "ExposureTime", (HTuple)this.exposureTime);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", 5000);
                HOperatorSet.GrabImageStart(this.hv_AcqHandle, -1);
                this.grabState = enumGrabState.RUN;
            }
            catch(Exception ex)
            {
                Console.WriteLine("開啟相機異常" + ex.ToString());
            }            
        }

        /// <summary>
        /// 相機斷線
        /// </summary>
        public void DisConnect()
        {
            try
            {
                if (this.cameraState == enumCameraState.OPEN_TRIGGER || this.cameraState == enumCameraState.OPEN_NOTRIGGER || this.cameraState == enumCameraState.ERROR)
                {
                    HOperatorSet.SetFramegrabberParam(this.hv_AcqHandle, "TriggerMode", "Off");
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                    HOperatorSet.CloseFramegrabber(this.hv_AcqHandle);
                    this.cameraState = enumCameraState.CLOSE;
                    this.grabState = enumGrabState.STOP;
                }
                else if (this.cameraState == enumCameraState.CLOSE)
                {
                    Console.WriteLine("相機已經關閉");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("關閉相機異常"+ex.ToString());
                this.cameraState=enumCameraState.ERROR;
            }
            
        }

        /// <summary>
        /// 開啟並擷取顯示
        /// </summary>
        public void Grab()
        {
            
            HObject hoImage_;
            while (this.grabState == enumGrabState.RUN)
            {
                try
                {
                    if (this.cameraState == enumCameraState.OPEN_TRIGGER || this.cameraState ==enumCameraState.OPEN_NOTRIGGER)
                    {
                        Console.WriteLine("取向");
                        //HOperatorSet.GrabImage(out hoImage_ , this.hv_AcqHandle);
                        HOperatorSet.GrabImageAsync(out hoImage_, this.hv_AcqHandle, -1);
                        //OnReceiveImg?.Invoke(this, new ImageReceiveArgs(hoImage));//只要相機開著就持續取像 送出影像
                        HOperatorSet.DispObj(hoImage_, this.ccdDisplay.hWindowRoi.HalconWindow);
                        Thread.Sleep(IntervalGrab);//取樣週期
                    }
                }
                catch (Exception ex)
                {
                    //TODO 通常是TimeOut
                    Console.WriteLine("Grab Delay : " + ex.ToString());
                }
                Thread.Sleep(1);//釋放CPU
            }//結束取像 關相機
            try
            {                    
                this.DisConnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Close CCD ERROR : " + ex.ToString());
            }
        }

        public void Run()
        {
            Task.Run(new Action(() => 
            {
                if(!IsOpen())
                    Connect();
                if(this.grabState==enumGrabState.RUN)
                    Grab();
            }));
        }
    }
}
