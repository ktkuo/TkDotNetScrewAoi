using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.Windows.Forms;

namespace TkDotNetScrewAoi.cameras
{
    public class CameraOptDisplay
    {
        public HalconDotNet.HWindowControl hWindowRoi ; // 1~4 ROI
        public HalconDotNet.HWindowControl hWindowBall; // 1~4 BALL
        public StatusStrip statusStrip;//狀態列
        public Button buttonLed;//辨識結果
    }
}
