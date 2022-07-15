using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkDotNetScrewAoi.control
{
    public class CommandLight
    {
        public string lightCenterOn ;
        public string lightCenterOff;
        public string lightPlate1On;
        public string lightPlate1Off;
        public string lightPlate2On;
        public string lightPlate2Off;
        public static CommandLight getInstance { get; } = new CommandLight();
        private CommandLight()
        {         
            lightCenterOn = "ra 0 2\r";
            lightCenterOff = "ra 0 0\r";

            lightPlate1On = "ra 1 2\r";
            lightPlate1Off = "ra 1 1\r";        
            
            lightPlate2On = "ra 2 2\r";
            lightPlate2Off= "ra 2 0\r";
        }
    }
}
