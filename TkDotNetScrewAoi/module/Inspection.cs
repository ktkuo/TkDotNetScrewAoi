using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkDotNetScrewAoi.cameras;
using TkDotNetScrewAoi.view;
using System.Windows.Forms;
using HalconDotNet;

namespace TkDotNetScrewAoi.module
{
    public enum enumStreamStatus 
    {
        INIT,
        EMPTY,
        FULL,
        WRITE,
        COMPELET
    }

    public enum enumInspectionStatus
    {
        INIT,
        RUN,
        STOP,
        ERROR
    }

    public class Inspection
    {
        public CameraOptHalcon Ccd1;
        public CameraOptHalcon Ccd2;
        public CameraOptDisplay displayInspect { get; set; }
        public int numberImageCcd1=0, numberImageCcd2=0;
        public bool isStream = false;
        public bool isInspectionRun = false;
        private void OnReceiveImgCcd1(object sender, ImageReceiveArgs e)
        {
            if (this.Ccd1.queueImageTrans.Count > 23)
                this.Ccd1.queueImageTrans.Enqueue(e.image.Clone());
            else
                this.Ccd1.isGrabIdle = true;
        }

        private void OnReceiveImgCcd2(object sender, ImageReceiveArgs e)
        {
            if (this.Ccd2.queueImageTrans.Count > 23)
                this.Ccd2.queueImageTrans.Enqueue(e.image.Clone());
            else
                this.Ccd2.isGrabIdle = true;
        }

        public Inspection(CameraOptDisplay cameraOptDisplay)
        {            
            this.displayInspect = cameraOptDisplay;
            this.Ccd1 =new CameraOptHalcon(displayInspect);
            this.Ccd2 =new CameraOptHalcon(displayInspect);
            //this.CameraOptDisplayFirst = new CameraOptDisplay();
            //this.CameraOptDisplayFirst.hWindowRoi_1 = hWControl_Roi1;
            //this.CameraOptDisplayFirst.hWindowRoi_2 = hWControl_Roi2;
            //this.cameraOptFirst.ccdDisplay = CameraOptDisplayFirst;
            this.Ccd1.OnReceiveImg+= new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd1);
            this.Ccd2.OnReceiveImg+=new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd2);
            isStream=true;
        }

        public void Run()
        {
            //TODO CCD2 OPEN()
            if (Ccd2.grabState != enumGrabState.RUN)
            {
                this.Ccd1.Open();  
            }                                   
            else                   
            {                    
                Console.WriteLine("檢測流程開啟異常");              
            }
        }

        public void Stop()
        {
            if (Ccd1.grabState == enumGrabState.RUN)
            {
                this.Ccd1.grabState = enumGrabState.STOP;
            }
            else
            {
                Console.WriteLine("檢測流程關閉異常");
            }
        }
       
        public void ImageTotalQueueBuffer()
        {
            while (isStream)
            {
                if (!this.Ccd1.queueImageTrans.IsEmpty)
                {

                }
            }            
        }
    }
}
