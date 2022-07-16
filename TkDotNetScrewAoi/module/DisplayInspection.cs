using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace TkDotNetScrewAoi.module
{
    public class DisplayInspection
    {
        public HWindowControl[] hWindowRois = new HWindowControl[] { }; // 1~4 ROI
        public HWindowControl[] hWindowBalls = new HWindowControl[] { }; // 1~4 BALL
        public StatusStrip statusStrip;//狀態列
        public Button buttonLed;//辨識結果
        public Label labelImageNumber;
        public Button buttonRun;
    }
}
