//
//  File generated by HDevelop for HALCON/DOTNET (C#) Version 11.0
//

using HalconDotNet;

public partial class HDevelopExport
{
#if !NO_EXPORT_APP_MAIN
  public HDevelopExport()
  {
    // Default settings used in HDevelop 
    HOperatorSet.SetSystem("width", 512);
    HOperatorSet.SetSystem("height", 512);
    action();
  }
#endif

  // Procedures 
  // Chapter: File
  // Short Description: Parse a filename into directory, base filename, and extension 
  public void parse_filename (HTuple hv_FileName, out HTuple hv_BaseName, out HTuple hv_Extension, 
      out HTuple hv_Directory)
  {


    // Local control variables 

    HTuple hv_DirectoryTmp = null, hv_Substring = null;

    // Initialize local and output iconic variables 

    //This procedure gets a filename (with full path) as input
    //and returns the directory path, the base filename and the extension
    //in three different strings.
    //
    //In the output path the path separators will be replaced
    //by '/' in all cases.
    //
    //The procedure shows the possibilities of regular expressions in HALCON.
    //
    //Input parameters:
    //FileName: The input filename
    //
    //Output parameters:
    //BaseName: The filename without directory description and file extension
    //Extension: The file extension
    //Directory: The directory path
    //
    //Example:
    //basename('C:/images/part_01.png',...) returns
    //BaseName = 'part_01'
    //Extension = 'png'
    //Directory = 'C:\\images\\' (on Windows systems)
    //
    //Explanation of the regular expressions:
    //
    //'([^\\\\/]*?)(?:\\.[^.]*)?$':
    //To start at the end, the '$' matches the end of the string,
    //so it is best to read the expression from right to left.
    //The part in brackets (?:\\.[^.}*) denotes a non-capturing group.
    //That means, that this part is matched, but not captured
    //in contrast to the first bracketed group ([^\\\\/], see below.)
    //\\.[^.]* matches a dot '.' followed by as many non-dots as possible.
    //So (?:\\.[^.]*)? matches the file extension, if any.
    //The '?' at the end assures, that even if no extension exists,
    //a correct match is returned.
    //The first part in brackets ([^\\\\/]*?) is a capture group,
    //which means, that if a match is found, only the part in
    //brackets is returned as a result.
    //Because both HDevelop strings and regular expressions need a '\\'
    //to describe a backslash, inside regular expressions within HDevelop
    //a backslash has to be written as '\\\\'.
    //[^\\\\/] matches any character but a slash or backslash ('\\' in HDevelop)
    //[^\\\\/]*? matches a string od 0..n characters (except '/' or '\\')
    //where the '?' after the '*' switches the greediness off,
    //that means, that the shortest possible match is returned.
    //This option is necessary to cut off the extension
    //but only if (?:\\.[^.]*)? is able to match one.
    //To summarize, the regular expression matches that part of
    //the input string, that follows after the last '/' or '\\' and
    //cuts off the extension (if any) after the last '.'.
    //
    //'\\.([^.]*)$':
    //This matches everything after the last '.' of the input string.
    //Because ([^.]) is a capturing group,
    //only the part after the dot is returned.
    //
    //'.*[\\\\/]':
    //This matches the longest substring with a '/' or a '\\' at the end.
    //
    HOperatorSet.TupleRegexpMatch(hv_FileName, ".*[\\\\/]", out hv_DirectoryTmp);
    HOperatorSet.TupleSubstr(hv_FileName, hv_DirectoryTmp.TupleStrlen(), (hv_FileName.TupleStrlen()
        )-1, out hv_Substring);
    HOperatorSet.TupleRegexpMatch(hv_Substring, "([^\\\\/]*?)(?:\\.[^.]*)?$", out hv_BaseName);
    HOperatorSet.TupleRegexpMatch(hv_Substring, "\\.([^.]*)$", out hv_Extension);
    //
    //Finally all found backslashes ('\\') are converted
    //to a slash to get consistent paths
    HOperatorSet.TupleRegexpReplace(hv_DirectoryTmp, (new HTuple("\\\\")).TupleConcat(
        "replace_all"), "/", out hv_Directory);

    return;
  }

  // Main procedure 
  private void action()
  {

    // Local iconic variables 

    HObject ho_imageRaw=null, ho_edgeAmplitude1=null;
    HObject ho_imageEmphasize=null, ho_imageMean=null, ho_region=null;
    HObject ho_regionClosing1=null, ho_connectedRegions=null;
    HObject ho_selectedRegions=null, ho_rectangle1=null, ho_imageReduced=null;
    HObject ho_imagePart=null;


    // Local control variables 

    HTuple hv_classFiles = null, hv_saveDirectory = null;
    HTuple hv_fileName = null, hv_size = null, hv_grayMin = null;
    HTuple hv_grayMax = null, hv_indexClass = null, hv_path = new HTuple();
    HTuple hv_imageLoadList = new HTuple(), hv_indexImage = new HTuple();
    HTuple hv_baseName = new HTuple(), hv_extension = new HTuple();
    HTuple hv_directory = new HTuple(), hv_fileExists = new HTuple();
    HTuple hv_area = new HTuple(), hv_row = new HTuple(), hv_column = new HTuple();
    HTuple hv_width = new HTuple(), hv_height = new HTuple();
    HTuple hv_saveName = new HTuple();

    // Initialize local and output iconic variables 
    HOperatorSet.GenEmptyObj(out ho_imageRaw);
    HOperatorSet.GenEmptyObj(out ho_edgeAmplitude1);
    HOperatorSet.GenEmptyObj(out ho_imageEmphasize);
    HOperatorSet.GenEmptyObj(out ho_imageMean);
    HOperatorSet.GenEmptyObj(out ho_region);
    HOperatorSet.GenEmptyObj(out ho_regionClosing1);
    HOperatorSet.GenEmptyObj(out ho_connectedRegions);
    HOperatorSet.GenEmptyObj(out ho_selectedRegions);
    HOperatorSet.GenEmptyObj(out ho_rectangle1);
    HOperatorSet.GenEmptyObj(out ho_imageReduced);
    HOperatorSet.GenEmptyObj(out ho_imagePart);

    //*20220716 ?????? ?????? ????????????
    //*????????????
    HOperatorSet.ListFiles("D:/00_ProgramRepository/04TkDotNetAoiScrewType/TkDotNetScrewAoi/TkDotNetScrewAoi/imagesTunes", 
        "directories", out hv_classFiles);
    hv_saveDirectory = "D:/00_ProgramRepository/04TkDotNetAoiScrewType/TkDotNetScrewAoi/TkDotNetScrewAoi/imageComplete";
    hv_fileName = "01_dataclassifiy";
    hv_size = 500;
    hv_grayMin = 70;
    hv_grayMax = 90;
    //*??????????????????
    for (hv_indexClass=0; (int)hv_indexClass<=(int)((new HTuple(hv_classFiles.TupleLength()
        ))-1); hv_indexClass = (int)hv_indexClass + 1)
    {

      //*???????????????
      hv_path = hv_classFiles.TupleSelect(hv_indexClass);
      //*???????????????
      HOperatorSet.ListFiles(hv_path, (new HTuple("files")).TupleConcat("follow_links"), 
          out hv_imageLoadList);
      if ((int)(new HTuple((new HTuple(hv_imageLoadList.TupleLength())).TupleGreater(
          0))) != 0)
      {
        for (hv_indexImage=0; (int)hv_indexImage<=(int)((new HTuple(hv_imageLoadList.TupleLength()
            ))-1); hv_indexImage = (int)hv_indexImage + 1)
        {
          //*????????????
          parse_filename(hv_imageLoadList.TupleSelect(hv_indexImage), out hv_baseName, 
              out hv_extension, out hv_directory);

          //*????????????????????????
          HOperatorSet.FileExists((hv_saveDirectory+"\\")+hv_fileName, out hv_fileExists);

          //*?????????21
          if ((int)(new HTuple(hv_fileExists.TupleLess(1))) != 0)
          {
            //*???????????????
            HOperatorSet.MakeDir((hv_saveDirectory+"\\")+hv_fileName);
          }

          //*??????
          ho_imageRaw.Dispose();
          HOperatorSet.ReadImage(out ho_imageRaw, hv_imageLoadList.TupleSelect(hv_indexImage));

          //*?????????  ????????????
          //*http://www.ihalcon.com/read-2139.html
          ho_edgeAmplitude1.Dispose();
          HOperatorSet.SobelAmp(ho_imageRaw, out ho_edgeAmplitude1, "sum_abs", 9);

          //*gray_range_rect (imageRaw, ImageResult, 11, 11)

          //*??????  ???????????? ??? ????????????????????? ?????? ????????? ?????????
          //*res := round((orig - mean) * Factor) + orig
          ho_imageEmphasize.Dispose();
          HOperatorSet.Emphasize(ho_edgeAmplitude1, out ho_imageEmphasize, 12, 12, 
              4);

          //*?????????????????????????????????
          ho_imageMean.Dispose();
          HOperatorSet.MeanImage(ho_edgeAmplitude1, out ho_imageMean, 4, 4);
          //*?????????
          ho_region.Dispose();
          HOperatorSet.Threshold(ho_imageMean, out ho_region, 12, 44);

          //*??????  ?????????????????? =>??????????????????
          ho_regionClosing1.Dispose();
          HOperatorSet.ClosingCircle(ho_region, out ho_regionClosing1, 100);

          //*region ??????
          ho_connectedRegions.Dispose();
          HOperatorSet.Connection(ho_regionClosing1, out ho_connectedRegions);

          //*???????????????(?????????)
          ho_selectedRegions.Dispose();
          HOperatorSet.SelectShape(ho_connectedRegions, out ho_selectedRegions, "area", 
              "and", 10000, 999999);

          //*????????????????????????????????????
          HOperatorSet.AreaCenter(ho_selectedRegions, out hv_area, out hv_row, out hv_column);

          //*????????????
          ho_rectangle1.Dispose();
          HOperatorSet.GenRectangle1(out ho_rectangle1, hv_row-(hv_size*0.5), hv_column-(hv_size*0.5), 
              hv_row+(hv_size*0.5), hv_column+(hv_size*0.5));

          //*??????????????????
          ho_imageReduced.Dispose();
          HOperatorSet.ReduceDomain(ho_imageRaw, ho_rectangle1, out ho_imageReduced
              );

          //*?????????????????????
          ho_imagePart.Dispose();
          HOperatorSet.CropDomain(ho_imageReduced, out ho_imagePart);
          if (HDevWindowStack.IsOpen())
          {
            HOperatorSet.ClearWindow(HDevWindowStack.GetActive());
          }
          if (HDevWindowStack.IsOpen())
          {
            HOperatorSet.DispObj(ho_imagePart, HDevWindowStack.GetActive());
          }
          HOperatorSet.GetImageSize(ho_imagePart, out hv_width, out hv_height);
          hv_saveName = ((((((hv_saveDirectory+"\\")+hv_fileName)+"\\")+hv_indexClass)+"_")+hv_indexImage)+".jpg";
          HOperatorSet.WriteImage(ho_imagePart, "jpeg", 0, ((((((hv_saveDirectory+"\\")+hv_fileName)+"\\")+hv_indexClass)+"_")+hv_indexImage)+".jpg");

          //*stop ()
          //*area_center (RegionClosing1, Area, Row, Column)

          //*dev_display (imageRaw)
          //*gen_circle (Circle, Row, Column, 100.5)
          //* gen_circle_contour_xld (ContCircle, Row, Column, 100, 0, 6.28318, 'positive', 1)


          //*??????
          //*rgb1_to_gray (imageRaw, imageGray)
          //*?????????
          //*threshold (imageGray,imageThread,grayMin,grayMax)
          //*??????
          //*fill_up (imageThread, RegionFillUp)
          //*de
          //*closing (RegionFillUp, RegionFillUp, RegionClosing)
        }
      }
    }

    ho_imageRaw.Dispose();
    ho_edgeAmplitude1.Dispose();
    ho_imageEmphasize.Dispose();
    ho_imageMean.Dispose();
    ho_region.Dispose();
    ho_regionClosing1.Dispose();
    ho_connectedRegions.Dispose();
    ho_selectedRegions.Dispose();
    ho_rectangle1.Dispose();
    ho_imageReduced.Dispose();
    ho_imagePart.Dispose();

  }


}
