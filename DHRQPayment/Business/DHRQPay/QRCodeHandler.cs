using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using ThoughtWorks.QRCode.Codec;

namespace DHRQPayment.Business.DHRQPay
{
    /// <summary>
    /// 二维码处理类
    /// </summary>
    public class QRCodeHandler
    {

        public int maxWidth = 640;
        public int maxHeight = 208;

        /// <summary>
        /// 创建二维码
        /// </summary>
        /// <param name="QRString">二维码字符串</param>
        /// <param name="QRCodeEncodeMode">二维码编码(Byte、AlphaNumeric、Numeric)</param>
        /// <param name="QRCodeScale">二维码尺寸(Version为0时，1：26x26，每加1宽和高各加25</param>
        /// <param name="QRCodeVersion">二维码密集度0-40</param>
        /// <param name="QRCodeErrorCorrect">二维码纠错能力(L：7% M：15% Q：25% H：30%)</param>
        /// <param name="filePath">保存路径</param>
        /// <param name="hasLogo">是否有logo(logo尺寸50x50，QRCodeScale>=5，QRCodeErrorCorrect为H级)</param>
        /// <param name="logoFilePath">logo路径</param>
        /// <returns></returns>
        public bool CreateQRCode(string QRString, string QRCodeEncodeMode, short QRCodeScale, int QRCodeVersion, string QRCodeErrorCorrect, string filePath, bool hasLogo, string logoFilePath)
        {
            bool result = true;

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();

            switch (QRCodeEncodeMode)
            {
                case "Byte":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
                case "AlphaNumeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
                    break;
                case "Numeric":
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
                    break;
                default:
                    qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                    break;
            }

            qrCodeEncoder.QRCodeScale = QRCodeScale;
            qrCodeEncoder.QRCodeVersion = QRCodeVersion;

            switch (QRCodeErrorCorrect)
            {
                case "L":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
                    break;
                case "M":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                    break;
                case "Q":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
                    break;
                case "H":
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
                default:
                    qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;
                    break;
            }

            try
            {
                Image image = qrCodeEncoder.Encode(QRString, System.Text.Encoding.UTF8);
                image.Save(logoFilePath, ImageFormat.Bmp);
                maxHeight = image.Height;
                Bitmap imgbitmap = getbmp(image, maxWidth, maxHeight);

                imgbitmap.Save(filePath, ImageFormat.Bmp);
                image.Dispose();

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }


        private Bitmap getbmp(Image inputimg, int maxWidth, int maxHeight)
        {
            Bitmap bmp = new Bitmap(inputimg);
            Bitmap bgbmp = new Bitmap(maxWidth, maxHeight);
            int posWidth = (maxWidth - bmp.Width) / 2;
            //int posHeight = (maxHeight - bmp.Height) / 2;
            int midrgb = Color.FromArgb(0x80, 0x80, 0x80).ToArgb();
            for (int j = 0; j < bgbmp.Height; j++)
            {
                //if (j >= posHeight && j < posHeight + bmp.Height)
                {
                    for (int i = 0; i < bgbmp.Width; i++)
                    {
                        if (i >= posWidth && i < posWidth + bmp.Width)
                        {
                            if (bmp.GetPixel(i - posWidth, j).ToArgb() > midrgb)
                            {
                                bgbmp.SetPixel(i, j, Color.White);
                            }
                            else
                            {
                                bgbmp.SetPixel(i, j, Color.Black);
                            }
                        }
                        else
                        {
                            bgbmp.SetPixel(i, j, Color.White);
                        }
                    }


                }
                //else
                //{
                //    for (int i = 0; i < bgbmp.Width; i++)
                //    {
                //        bgbmp.SetPixel(i, j, Color.White);
                //    }

                //}
            }
            inputimg.Dispose();
            return ConvertTo24bppTo1bpp(bgbmp);
        }

        //public Bitmap ConvertTo24bppTo1bpp(Bitmap SrcImg)
        //{
        //    unsafe
        //    {
        //        byte* SrcPointer, DestPointer;
        //        int Width, Height, SrcStride, DestStride;
        //        int X, Y, Index, Sum; ;
        //        Bitmap DestImg = new Bitmap(SrcImg.Width, SrcImg.Height, PixelFormat.Format1bppIndexed);
        //        BitmapData SrcData = new BitmapData();
        //        SrcImg.LockBits(new Rectangle(0, 0, SrcImg.Width, SrcImg.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb, SrcData);
        //        BitmapData DestData = new BitmapData();
        //        DestImg.LockBits(new Rectangle(0, 0, SrcImg.Width, SrcImg.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed, DestData);
        //        Width = SrcImg.Width; Height = SrcImg.Height; SrcStride = SrcData.Stride; DestStride = DestData.Stride;
        //        for (Y = 0; Y < Height; Y++)
        //        {
        //            SrcPointer = (byte*)SrcData.Scan0 + Y * SrcStride;
        //            DestPointer = (byte*)DestData.Scan0 + Y * DestStride;
        //            Index = 7; Sum = 0;
        //            for (X = 0; X < Width; X++)
        //            {
        //                if (*SrcPointer + (*(SrcPointer + 1) << 1) + *(SrcPointer + 2) >= 512) Sum += (1 << Index);
        //                if (Index == 0)
        //                {
        //                    *DestPointer = (byte)Sum;
        //                    Sum = 0;
        //                    Index = 7;
        //                    DestPointer++;
        //                }
        //                else
        //                    Index--;
        //                SrcPointer += 3;
        //            }
        //            if (Index != 7) *DestPointer = (byte)Sum;
        //        }
        //        SrcImg.UnlockBits(SrcData);
        //        DestImg.UnlockBits(DestData);
        //        return DestImg;
        //    }
        //}
    }
}
