using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TkDotNetScrewAoi.controls
{

    public class BaseCmd
    {
        /*自製螺桿機*/
        public byte[] Idle;// 待機    M42
        public byte[] HANDLE;// 手動  M43
        public byte[] Auto;// 自動    M44        
        public byte[] motorFeeder;// 入料
        public byte[] stepMotorArrange;//單顆整列
        public byte[] sidePlateOn;// D3104 Y3 開啟側轉
        public byte[] sidePlateOff;// D3104 Y3 關閉側轉 
        public byte[] lightON;// D3105 Y4 開啟光源
        public byte[] lightOFF;// D3105 Y4 關閉光源
        public byte[] sortHand;// D3109 Y5  手動分料
        public byte[] sortNG;// D3105 Y5  自動分料
        public byte[] sortOK;// D3105 Y5  自動分料        
        public byte[] servoMotorPowerOn;// 馬達送電
        public byte[] servoMotorPowerOff;// 馬達斷電
        public byte[] CcdTriggerOne;// 相機單張取相
        public byte[] CcdTriggerTen;// 連續取相多張
        public byte[] FeegBack;// 請求回傳值

        public byte[] servoMotorStop;// 馬達停止    
        public byte[] servoMotorContinuous;// 馬達連動
        public byte[] servopMotorStep;// 馬達單動
        public byte[] servoMotorPosition;// 馬達定位
        public byte[] servoMotorAcc;// 馬達Reset  加速度值
        public byte[] servoMotorSpeed;// 馬達Reset  速度值

        /// <summary>
        /// D3106 Y16 綠燈
        /// </summary>
        public byte[] LR_GREEN = HexToByte("000000000009" + "0110" + "1C1F" + "0001" + "02" + "0001");
        /// <summary>
        /// D3108 Y17 紅燈
        /// </summary>
        public byte[] LR_RED = HexToByte("000000000009" + "0110" + "1C1F" + "0001" + "02" + "0001");
        /// <summary>
        /// D3122 D3123  馬達轉速設定
        /// </summary>
        public byte[] LR_RPM = HexToByte("000000000009" + "0110" + "1C32" + "0001" + "02" + "0040");
        /// <summary>
        /// D3100 D3110
        /// </summary>
        public byte[] ModeReceive;//模式接收
        /// <summary>
        /// D3
        /// </summary>
        public byte[] Mode2nd;

        public byte[] StatusReceive = BaseCmd.HexToByte("000000000006" + "0103" + "9195" + "0002");       // D4000 讀取



        public string EmergencyState;//緊急停止
        public string EmergencyFree;//緊急松
        public string MotorFB;//馬達到位

        public static byte[] HexToByte(string hexString)                            //運算後的位元組長度:16進位數字字串長/2
        {
            byte[] byteOUT = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i = i + 2)
            {
                byteOUT[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);     //每2位16進位數字轉換為一個10進位整數
            }
            return byteOUT;
        }
    }

    public class ScrewSelfCmd : BaseCmd
    {
        public static ScrewSelfCmd Instance { get; } = new ScrewSelfCmd();

        private ScrewSelfCmd()
        {
            //D3100   "總長度"+"站位與功能碼"+"頭碼"+"位址數量"+"ByteLength"+"值"
            this.Idle = BaseCmd.HexToByte   ("000000000009" + "0110" + "1C1C" + "0001" + "02" + "0000");//D3100
            this.HANDLE = BaseCmd.HexToByte ("000000000009" + "0110" + "1C1C" + "0001" + "02" + "0001");//D3100   
            this.Auto = BaseCmd.HexToByte   ("000000000009" + "0110" + "1C1C" + "0001" + "02" + "0002");//D3100  


            //D3102
            this.servoMotorPowerOn = BaseCmd.HexToByte  ("000000000009" + "0110" + "1C1E" + "0001" + "02" + "0001");//D3102=1 馬達送電
            this.CcdTriggerOne = BaseCmd.HexToByte      ("000000000009" + "0110" + "1C1E" + "0001" + "02" + "0003");//D3102=3 觸發一循環

            //D499~D508
            this.servoMotorStop = BaseCmd.HexToByte         ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0002" + "0000" + "0000" + "0000" + "0000" + "0000" + "00C8" + "00CB");
            this.servoMotorContinuous = BaseCmd.HexToByte   ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0003" + "0000" + "0000" + "0000" + "0000" + "0000" + "0000" + "0004");
            this.servoMotorPosition = BaseCmd.HexToByte     ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0004" + "0001" + "0000" + "0000" + "0000" + "00C8" + "0000" + "00CE");
            this.servopMotorStep = BaseCmd.HexToByte        ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0004" + "0001" + "0000" + "00FF" + "00FF" + "0038" + "0000" + "003C");
            this.servoMotorAcc = BaseCmd.HexToByte          ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0003" + "0000" + "0000" + "0000" + "0000" + "0000" + "0000" + "0004");
            this.servoMotorSpeed = BaseCmd.HexToByte        ("000000000019" + "0110" + "11F3" + "0009" + "12" + "0001" + "0001" + "0003" + "0000" + "0000" + "0000" + "0000" + "0000" + "0000" + "0004");

            //
            this.servoMotorPowerOff = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0007");// D3124 右側馬達送電7
            this.servoMotorPowerOn = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0008");// D3124  右側馬達斷電8;

            this.motorFeeder = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0003");// D3124  右側馬達單動3  
            this.sortHand = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0004");// D3124  右側馬達定位4
            this.sortNG = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0005");// D3124  右側馬達加速度5
            this.sortOK = BaseCmd.HexToByte("000000000009" + "0110" + "1C34" + "0001" + "02" + "0006");// D3124  右側馬達定位速度6
            
        }
    }
}
