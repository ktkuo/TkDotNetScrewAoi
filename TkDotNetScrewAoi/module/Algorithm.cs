using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using HalconDotNet;
using System.Drawing;
using System.Drawing.Imaging;

namespace TkDotNetScrewAoi.module
{
    public enum ENUM_BREAK
    {
        INIT,
        UP,
        DOWN,
        BREAK,
    }
    public enum ENUM_LABLE { }

    public class Algorithm
    {
        private static ENUM_BREAK enumBreak_= ENUM_BREAK.INIT;
        public static ENUM_BREAK enumBreak { get; set; }
        private static void DevWait(ENUM_InspectionMode enumInspectionMode_,HObject hObject , HalconDotNet.HWindowControl hWindowControl_)
        {
            if(enumInspectionMode_ != ENUM_InspectionMode.INSPECT)
            {
                Console.WriteLine("開發或調機模式");
                while (true)
                {
                    //TODO 暫時都先跳下一步
                    if (enumBreak != ENUM_BREAK.INIT)
                    {
                        enumBreak_ = ENUM_BREAK.INIT;
                        Console.WriteLine("下一步");
                        break;
                    }
                    else
                    {
                        try
                        {
                            HOperatorSet.DispObj(hObject, hWindowControl_.HalconWindow);
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("開發或調機模式Error: "+ex.Message);
                        }                        
                    }
                    Thread.Sleep(1);
                }
            }            
        }

        public static Random random = new Random();//隨機函數

        public static async Task<HObject> RoiRegion(HalconDotNet.HWindowControl hWindowControl_,HObject ho_imageRaw, HTuple roiX1, HTuple roiY1, HTuple roiX2, HTuple roiY2)
        {
            HObject ho_rectangle1, ho_imageReduced, ho_imagePart=null;
            try
            {                
                HOperatorSet.GenRectangle1(out ho_rectangle1, roiX1, roiY1, roiX2, roiY2);//*圈出矩形
                HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced);//*與原圖取交集
                HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);//*輸出新的裁切圖
                HOperatorSet.DispObj(ho_imagePart, hWindowControl_.HalconWindow);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return ho_imagePart;
        }

        public static async Task<Bitmap> GetBallAsync(ENUM_InspectionMode enumInspectionMode_, Object lock_,HalconDotNet.HWindowControl hWindowControl, HObject ho_imageRaw, HTuple roiX,HTuple roiY,bool isSave_,string dirSave)
        {
            int ballNumber = 0; Bitmap bitmap = null;
            try 
            {                
                HObject ho_edgeAmplitude1, ho_imageEmphasize, ho_imageMean, ho_region,//二質化
                    ho_regionClosing1, ho_connectedRegions, ho_selectedRegions,//球面績
                    ho_rectangle1, ho_imageReduced, ho_imagePart;

                HTuple sizeSobelAmp = 9;
                HOperatorSet.SobelAmp(ho_imageRaw, out ho_edgeAmplitude1, "sum_abs", sizeSobelAmp);/*取導數  可找邊緣 http://www.ihalcon.com/read-2139.html*/

                HTuple maskWidth = 12, maskHeight = 12, factor = 4;//res := round((orig - mean) * Factor) + orig
                HOperatorSet.Emphasize(ho_edgeAmplitude1, out ho_imageEmphasize, maskWidth, maskHeight, factor);/* 強化  單點像素 與 全圖像素之平均 加大 黑更黑 白更白*/

                HTuple meanWidth = 4, meanHeight = 4;
                HOperatorSet.MeanImage(ho_edgeAmplitude1, out ho_imageMean, meanWidth, meanHeight);/*強化後，需要去躁點*/

                HTuple minGray = 12, maxGray = 44;
                HOperatorSet.Threshold(ho_imageMean, out ho_region, minGray, maxGray);/*二質化*/

                HTuple radiusClosingCircle = 100;
                HOperatorSet.ClosingCircle(ho_region, out ho_regionClosing1, radiusClosingCircle);/*閉合  先膨脹再侵蝕=>盡量完善圓周*/
                HOperatorSet.Connection(ho_regionClosing1, out ho_connectedRegions);/*region 切開*/

                HTuple minArea = 10000, maxArea = 999999;
                HOperatorSet.SelectShape(ho_connectedRegions, out ho_selectedRegions, "area", "and", 10000, 999999);//*找尋球面積(上下限)
                                                                                                                    //DevWait(ho_selectedRegions, hWindowControl);

                HOperatorSet.DispObj(ho_connectedRegions, hWindowControl.HalconWindow);
                HTuple hv_area = null, hv_row=null, hv_column = null;
                HOperatorSet.AreaCenter(ho_selectedRegions, out hv_area, out hv_row, out hv_column);//*找出區域的中心座標與面積
                HTuple hv_size = 500, ww, hh,LL;
                HOperatorSet.TupleLength(hv_row, out LL);
                if (ho_selectedRegions != null && LL.D>0)
                {                    
                    HOperatorSet.GetImageSize(ho_imageRaw,out ww,out hh);
                    HOperatorSet.GenRectangle1(out ho_rectangle1, hv_row - (hv_size * 0.5), hv_column - (hv_size * 0.5), hv_row + (hv_size * 0.5), hv_column + (hv_size * 0.5));                
                    HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced);//*與原圖取交集
                    HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);//*輸出新的裁切圖
                    if (isSave_)
                    {
                        lock (lock_)
                        {
                            HOperatorSet.WriteImage(ho_imagePart, "jpeg", 0, dirSave + random.Next() + ".jpg");
                        }
                    }
                    HOperatorSet.DispObj(ho_imagePart, hWindowControl.HalconWindow);
                }
                else
                {
                    HOperatorSet.GenRectangle1(out ho_rectangle1, 0, 0,//*圈出矩形
                        hv_size, hv_size);
                    HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced);//*與原圖取交集
                    HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);//*輸出新的裁切圖
                    HOperatorSet.DispObj(ho_imagePart, hWindowControl.HalconWindow);
                }
                bitmap = Hobjet2Bitmap32(ho_imagePart);
            }
            catch(Exception ex)
            {
                Console.WriteLine("切圖演算法異常"+ex.ToString());
            }
            
            return bitmap;
        }

        public static Bitmap GetBall(Object lock_, HalconDotNet.HWindowControl hWindowControl, HObject ho_imageRaw,bool isSave_, string dirSave)
        {
            int ballNumber = 0; Bitmap bitmap = null;
            HTuple sizeSobelAmp = 9;
            HTuple maskWidth = 12, maskHeight = 12, factor = 4;//res := round((orig - mean) * Factor) + orig
            HTuple meanWidth = 4, meanHeight = 4;
            HTuple minGray = 12, maxGray = 44;
            HTuple radiusClosingCircle = 100;
            HTuple minArea = 10000, maxArea = 999999;
            HTuple hv_size = 500, ww, hh, LL;
            try
            {
                HObject ho_edgeAmplitude1, ho_imageEmphasize, ho_imageMean, ho_region,//二質化
                    ho_regionClosing1, ho_connectedRegions, ho_selectedRegions,//球面績
                    ho_rectangle1, ho_imageReduced, ho_imagePart;
                HOperatorSet.SobelAmp(ho_imageRaw, out ho_edgeAmplitude1, "sum_abs", sizeSobelAmp);/*取導數  可找邊緣 http://www.ihalcon.com/read-2139.html*/
                HOperatorSet.Emphasize(ho_edgeAmplitude1, out ho_imageEmphasize, maskWidth, maskHeight, factor);/* 強化  單點像素 與 全圖像素之平均 加大 黑更黑 白更白*/
                HOperatorSet.MeanImage(ho_edgeAmplitude1, out ho_imageMean, meanWidth, meanHeight);/*強化後，需要去躁點*/                
                HOperatorSet.Threshold(ho_imageMean, out ho_region, minGray, maxGray);/*二質化*/                
                HOperatorSet.ClosingCircle(ho_region, out ho_regionClosing1, radiusClosingCircle);/*閉合  先膨脹再侵蝕=>盡量完善圓周*/
                HOperatorSet.Connection(ho_regionClosing1, out ho_connectedRegions);/*region 切開*/                
                HOperatorSet.SelectShape(ho_connectedRegions, out ho_selectedRegions, "area", "and", minArea, maxArea);//*找尋球面積(上下限)
                                                                                                                    //DevWait(ho_selectedRegions, hWindowControl);
                HOperatorSet.DispObj(ho_connectedRegions, hWindowControl.HalconWindow);
                HTuple hv_area = null, hv_row = null, hv_column = null;
                HOperatorSet.AreaCenter(ho_selectedRegions, out hv_area, out hv_row, out hv_column);//*找出區域的中心座標與面積
                HOperatorSet.TupleLength(hv_row, out LL);
                if (ho_selectedRegions != null && LL.D > 0)
                {
                    HOperatorSet.GetImageSize(ho_imageRaw, out ww, out hh);
                    HOperatorSet.GenRectangle1(out ho_rectangle1, hv_row - (hv_size * 0.5), hv_column - (hv_size * 0.5), hv_row + (hv_size * 0.5), hv_column + (hv_size * 0.5));
                    HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced);//*與原圖取交集
                    HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);//*輸出新的裁切圖
                    if (isSave_)
                    {  
                        HOperatorSet.WriteImage(ho_imagePart, "jpeg", 0, dirSave + random.Next() + ".jpg");
                    }
                    HOperatorSet.DispObj(ho_imagePart, hWindowControl.HalconWindow);
                }
                else
                {
                    HOperatorSet.GenRectangle1(out ho_rectangle1, 0, 0,//*圈出矩形
                        hv_size, hv_size);
                    HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced);//*與原圖取交集
                    HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);//*輸出新的裁切圖
                    HOperatorSet.DispObj(ho_imagePart, hWindowControl.HalconWindow);
                }
                bitmap = Hobjet2Bitmap32(ho_imagePart);
            }
            catch (Exception ex)
            {
                Console.WriteLine("切圖演算法異常" + ex.ToString());
            }

            return bitmap;
        }

        private static int[] Morphology()
        {
            int[] Ballfigure = new int[] { };
            return Ballfigure;
        }

        public static Bitmap Hobjet2Bitmap32(HObject image)
        {
            Bitmap bitmap2 =null;
            try
            {
                HTuple hred, hgreen, hblue, type, width, height;
                HOperatorSet.GetImagePointer3(image, out hred, out hgreen, out hblue, out type, out width, out height);
                bitmap2 = new Bitmap(width.I, height.I, PixelFormat.Format32bppRgb);
                Rectangle rect2 = new Rectangle(0, 0, width.I, height.I); 
                BitmapData bitmapData2 = bitmap2.LockBits(rect2, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                unsafe
                {
                    byte* bptr = (byte*)bitmapData2.Scan0;
                    byte* r = ((byte*)hred.L);
                    byte* g = ((byte*)hgreen.L);
                    byte* b = ((byte*)hblue.L);
                    int lengh = width.I * height.I;
                    for (int i = 0; i < lengh; i++)
                    {
                        bptr[i * 4]     = (b)[i];
                        bptr[i * 4 + 1] = (g)[i];
                        bptr[i * 4 + 2] = (r)[i];
                        bptr[i * 4 + 3] = 255;
                    }
                }
                bitmap2.UnlockBits(bitmapData2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return bitmap2;
        }

        public static HObject Bitmap2Hobget32(Bitmap bitmap)
        {
            HObject hObject=null;
            return hObject;
        }
    }
}
