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
        public DisplayInspection display;
        public FormImagGet()
        {
            InitializeComponent();
            display = new DisplayInspection();
            this.display.hWindowRois=new HWindowControl[] { hWControl_Roi1,hWControl_Roi2,hWControl_Roi3,hWControl_Roi4 } ;
            this.display.hWindowBalls = new HWindowControl[] { hWControl_Ball1,hWControl_Ball2,hWControl_Ball3,hWControl_Ball4};
            this.display.labelImageNumber = label_numberImage;
            this.display.buttonRun = btn_CcdConnect;
            this.inspectionScrewType = new Inspection(display);//創建檢測流程
        }

        private void FormImagGet_Load(object sender, EventArgs e)
        {

        }

        private void btn_CcdConnect_Click(object sender, EventArgs e)
        {
            if (radioButton_modeInspect.Checked==true)
            {
                inspectionScrewType.enumInspectionMode = module.ENUM_InspectionMode.INSPECT;
                if (!inspectionScrewType.isInspectionRun)
                    inspectionScrewType.Run();
                else
                    inspectionScrewType.Stop();
                inspectionScrewType.isInspectionRun = !inspectionScrewType.isInspectionRun;
            }
            else if (radioButton_modeDev.Checked == true)
            {
                Console.WriteLine("開發者模式");
                inspectionScrewType.enumInspectionMode = module.ENUM_InspectionMode.DEV;
                btn_CcdConnect.Enabled = false;
                Task.Run(new Action(() => 
                {
                    inspectionScrewType.isSave = true;//校機存圖
                    inspectionScrewType.Dev();
                }));
                
            }
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
            if(inspectionScrewType.enumInspectionMode != module.ENUM_InspectionMode.DEV)
            {
                inspectionScrewType.isSave = !inspectionScrewType.isSave;//是否存圖控件
                if (inspectionScrewType.isSave == true)
                    btn_ImageSave.BeginInvoke(new Action(() => { btn_ImageSave.BackColor = Color.Green; }));
                else
                    btn_ImageSave.BeginInvoke(new Action(() => { btn_ImageSave.BackColor = SystemColors.Control; }));
            }
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

        private void radioButton_modeTune_CheckedChanged(object sender, EventArgs e)
        {
            inspectionScrewType.enumInspectionMode = module.ENUM_InspectionMode.TUNE;
            btn_CcdConnect.Text = "調機模式開始";
            btn_NextStep.Enabled = true;btn_NextStep.Visible = true;
            btn_PreStep.Enabled = true; btn_PreStep.Visible = true; btn_ImageChange.Visible = true;
        }

        private void radioButton_modeInspect_CheckedChanged(object sender, EventArgs e)
        {
            inspectionScrewType.enumInspectionMode = module.ENUM_InspectionMode.INSPECT;
            btn_CcdConnect.Text = "檢測運行開始";
            btn_NextStep.Enabled = false; btn_PreStep.Enabled = false; btn_NextStep.Visible = false; btn_PreStep.Visible = false;btn_ImageChange.Visible = false;

        }

        private void radioButton_modeDev_CheckedChanged(object sender, EventArgs e)
        {
            
            btn_CcdConnect.Text = "開發者模式開始";
            btn_NextStep.Enabled = false; btn_PreStep.Enabled = false; btn_NextStep.Visible = false; btn_PreStep.Visible = false; btn_ImageChange.Visible = false;
        }                                                             

        private void btn_NextStep_Click(object sender, EventArgs e)
        {
            TkDotNetScrewAoi.module.Algorithm.enumBreak = TkDotNetScrewAoi.module.ENUM_BREAK.DOWN;
        }

        private void btn_PreStep_Click(object sender, EventArgs e)
        {
            TkDotNetScrewAoi.module.Algorithm.enumBreak = TkDotNetScrewAoi.module.ENUM_BREAK.UP;
        }

        private void radioButton_External_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btn_ImageChange_Click(object sender, EventArgs e)
        {

        }
    }
}
