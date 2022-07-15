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
        public HalconDotNet.HWindowControl hWindowRoi_1 ; // 1~4 ROI
        public HalconDotNet.HWindowControl hWindowRoi_2;
        public HalconDotNet.HWindowControl hWindowRoi_3;
        public HalconDotNet.HWindowControl hWindowRoi_4;

        public HalconDotNet.HWindowControl hWindowBall_1; // 1~4 BALL
        public HalconDotNet.HWindowControl hWindowBall_2;
        public HalconDotNet.HWindowControl hWindowBall_3;
        public HalconDotNet.HWindowControl hWindowBall_4;

        public StatusStrip statusStrip;//狀態列
        public Button buttonLed;//辨識結果
    }
}
