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
using TkDotNetScrewAoi.control;

namespace TkDotNetScrewAoi.view
{
    public partial class FormImagGet : Form
    {        
        public Inspection inspectionScrewType;
        public CameraOptDisplay displayInspect;


        public FormImagGet()
        {
            InitializeComponent();
            displayInspect = new CameraOptDisplay();
            this.displayInspect.hWindowRoi_1 = hWControl_Roi1;
            this.displayInspect.hWindowRoi_2 = hWControl_Roi2;
            this.displayInspect.hWindowRoi_3 = hWControl_Roi3;
            this.displayInspect.hWindowRoi_4 = hWControl_Roi4;
            this.displayInspect.hWindowBall_1 = hWControl_Ball1;
            this.displayInspect.hWindowBall_2 = hWControl_Ball2;
            this.displayInspect.hWindowBall_3 = hWControl_Ball3;
            this.displayInspect.hWindowBall_4 = hWControl_Ball4;
            this.inspectionScrewType = new Inspection(displayInspect);//創建檢測流程
            //this.cameraOptFirst = new CameraOptHalcon();
            //this.CameraOptDisplayFirst = new CameraOptDisplay();
            //this.CameraOptDisplayFirst.hWindowRoi_1 = hWControl_Roi1;
            //this.CameraOptDisplayFirst.hWindowRoi_2 = hWControl_Roi2;
            //this.cameraOptFirst.ccdDisplay = CameraOptDisplayFirst;
        }

        private void FormImagGet_Load(object sender, EventArgs e)
        {

        }

        private void btn_CcdConnect_Click(object sender, EventArgs e)
        {
            if (!inspectionScrewType.isInspectionRun)
                inspectionScrewType.Run();
            else
                inspectionScrewType.Stop();
            inspectionScrewType.isInspectionRun = !inspectionScrewType.isInspectionRun;
        }

        private void radioButton_Software_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_Software.Checked)
            {
                inspectionScrewType.Ccd1.isTrigger = false;
                inspectionScrewType.Ccd2.isTrigger = false;
            }
            else if (radioButton_External.Checked)
            {
                inspectionScrewType.Ccd1.isTrigger = true;
                inspectionScrewType.Ccd2.isTrigger = true;
            }
        }

        private void btn_ImageSave_Click(object sender, EventArgs e)
        {

        }
        bool ll = true;
        public SerialPorts serialPorts;
        private void btn_MechaismTest_Click(object sender, EventArgs e)
        {
            if (serialPorts == null)
                serialPorts = new SerialPorts();
            if(ll)
                serialPorts.Send("COM6", 38400, "sw 0\r");
            
            else
                serialPorts.Send("COM6", 38400, "sw 1\r");

            ll = !ll;
        }
    }
}
