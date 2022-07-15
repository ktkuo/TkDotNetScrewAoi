using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TkDotNetScrewAoi.cameras;

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
        public CameraOptDisplay display;
        public int numberImageCcd1=0, numberImageCcd2=0;
        public bool isStream = false;

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

        public Inspection()
        {
            Ccd1=new CameraOptHalcon();
            Ccd2=new CameraOptHalcon();
            display=new CameraOptDisplay();
            this.Ccd1.OnReceiveImg+= new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd1);
            this.Ccd2.OnReceiveImg+=new EventHandler<ImageReceiveArgs>(OnReceiveImgCcd2);
            isStream=true;
        }

        public void Run()
        {

        }

        public void Stop()
        {

        }

        public void IsRun()
        {

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
