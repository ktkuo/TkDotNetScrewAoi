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
using TkDotNetScrewAoi.module;

namespace TkDotNetScrewAoi.view
{
    public partial class FormImagGet : Form
    {

        public CameraOptHalcon cameraOptFirst;
        public CameraOptDisplay CameraOptDisplayFirst;

        public Inspection inspectionScrewType;

        public FormImagGet()
        {
            InitializeComponent();
            this.inspectionScrewType = new Inspection();//創建檢測流程
            this.cameraOptFirst = new CameraOptHalcon();
            this.CameraOptDisplayFirst = new CameraOptDisplay();
            this.CameraOptDisplayFirst.hWindowRoi_1 = hWControl_Roi1;
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
            else if(this.cameraOptFirst.grabState != enumGrabState.RUN)
            {                
                this.cameraOptFirst.Open();
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

        private void btn_ImageSave_Click(object sender, EventArgs e)
        {

        }

        private void btn_MechaismTest_Click(object sender, EventArgs e)
        {
        }
    }
}
