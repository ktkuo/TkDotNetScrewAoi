using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using TkDotNetScrewAoi.cameras;

namespace TkDotNetScrewAoi.view
{
    public partial class FormImagGet : Form
    {

        public CameraOptHalcon cameraOptFirst;
        public CameraOptDisplay CameraOptDisplayFirst;

        public FormImagGet()
        {
            InitializeComponent();
            this.cameraOptFirst = new CameraOptHalcon();
            this.CameraOptDisplayFirst = new CameraOptDisplay();
            this.CameraOptDisplayFirst.hWindowRoi = hWControl_Roi1;
            this.cameraOptFirst.ccdDisplay = CameraOptDisplayFirst;
        }

        private void FormImagGet_Load(object sender, EventArgs e)
        {

        }

        private void btn_CcdConnect_Click(object sender, EventArgs e)
        {
            if (this.cameraOptFirst.grabState == enumGrabState.RUN)
            {
                this.cameraOptFirst.grabState = enumGrabState.STOP;
            }
            else if(this.cameraOptFirst.grabState != enumGrabState.STOP)
            {
                this.cameraOptFirst.Run();
            }
            else if (cameraOptFirst.grabState == enumGrabState.ERROR)
            {
                Console.WriteLine("取像流程異常");
            }
        }

        private void radioButton_Software_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_Software.Checked)
            {
                this.cameraOptFirst.isTrigger = false;
            }
            else if (radioButton_External.Checked)
            {
                this.cameraOptFirst.isTrigger = true;
            }
        }

        bool  ff=false;
        private void btn_ImageSave_Click(object sender, EventArgs e)
        {
            
            Task.Run(new Action(() => 
            {
                ff = true;
                HTuple hv_AcqHandle;
                HObject ho_Image;
                HOperatorSet.OpenFramegrabber("HMV3rdParty", 0, 0, 0, 0, 0, 0, "progressive",-1, "default", -1, "false", "default", "MachineVision:7K0108EPAK00004", 0, -1, out hv_AcqHandle); HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerMode", "Off");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "TriggerSource", "Software");
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "ExposureTime", 1800.0);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "grab_timeout", 5000);
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
                while (ff)
                {
                    //ho_Image.Dispose();
                    HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, 500);
                    HOperatorSet.DispObj(ho_Image,hWControl_Roi1.HalconWindow);
                    //Image Acquisition 01: Do something
                }
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
            }));
            

        }

        private void btn_MechaismTest_Click(object sender, EventArgs e)
        {
            ff = false;
        }
    }
}
