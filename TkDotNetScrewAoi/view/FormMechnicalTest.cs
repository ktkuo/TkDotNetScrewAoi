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
using TkDotNetScrewAoi.controls;
using System.Threading;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

namespace TkDotNetScrewAoi.view
{
    public partial class FormMechnicalTest : Form
    {
        private Sockets plc { get; set; }
        public HObject ImageCcdTest { get;}

        private InspectionCcd2 inspectionCcd2 { get; set; }

        public FormMechnicalTest(Sockets sockets_, InspectionCcd2 inspectionCcd2_)
        {
            InitializeComponent();
            this.plc= sockets_;
            this.inspectionCcd2 = inspectionCcd2_;
        }

        private void FormMechnicalTest_Load(object sender, EventArgs e)
        {
            plc.Send(CmdScrewSelf.Instance.modeHandle);
        }

        private void btn_modeChange_Click(object sender, EventArgs e)
        {
            plc.Send(CmdScrewSelf.Instance.modeHandle);
        }

        private void btn_ModeIdle_Click(object sender, EventArgs e)
        {
            plc.Send(CmdScrewSelf.Instance.modeIdle);
        }

        bool isLight = false;
        private void btn_LightOnOff_Click(object sender, EventArgs e)
        {
            if (isLight)
            {
                plc.Send(CmdScrewSelf.Instance.KeepLight);
            }
            else
            {
                plc.Send(CmdScrewSelf.Instance.KeepLightOff);
            }
            isLight = !isLight;
        }

        bool isSort = false;
        private void btn_SortHandle_Click(object sender, EventArgs e)
        {
            if (isSort)
            {
                plc.Send(CmdScrewSelf.Instance.sortHandOK);
            }
            else
            {
                plc.Send(CmdScrewSelf.Instance.sortHandNG);
            }
            isSort = !isSort;
        }

        private void btn_ServoPicthR_Click(object sender, EventArgs e)
        {
            plc.Send(CmdScrewSelf.Instance.servopMotorStepR);
        }

        private void btn_ServoPicth_Click(object sender, EventArgs e)
        {
            plc.Send(CmdScrewSelf.Instance.servopMotorStep);
        }

        
        private void btn_CcdTrigger_Click(object sender, EventArgs e)
        {
            
            
            if (isCcdTest)
            {
                
                plc.Send(CmdScrewSelf.Instance.servoMotorPitchGrab);
            }
            else
            {
                
                plc.Send(CmdScrewSelf.Instance.servoMotorPitchGrabR);
            }
            isCcdTest = !isCcdTest;
        }
        bool isCcdTest = true;
        private void btn_CcdSwitch_Click(object sender, EventArgs e)
        {
            if (isCcdTest)
            {
                inspectionCcd2.Ccd1.isTrigger = true;
                inspectionCcd2.Ccd1.Open();
            }
            else
            {
                inspectionCcd2.Ccd1.enumGrabState = TkDotNetScrewAoi.cameras.ENUM_GrabState.STOP;
            }
            
        }
    }
}
