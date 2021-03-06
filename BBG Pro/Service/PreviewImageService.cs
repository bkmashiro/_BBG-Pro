using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BBG_Pro.Service
{
    /// <summary>
    /// 提供生成预览图片的服务
    /// </summary>
    class PreviewImageService
    {
        BlockInfoService BlockInfoService;
        int width = 1;
        int height = 1;
        byte[,] result;
        public void Init(BlockInfoService b, int w, int h, byte[,] byteResult)
        {
            BlockInfoService = b;
            result = byteResult;
            width = w;
            height = h;
            //生成交叉数组
            blockCssByte = new byte[BlockInfoService.classEnabled.Length][];

            for (int j = 0; j < BlockInfoService.classEnabled.Length; j++)
            {
                if (BlockInfoService.classEnabled[j])
                {
                    blockCssByte[j] = new byte[16 * 16 * 3];
                    getBGRArrayForBlockBmp(new Bitmap(BlockInfoService.demoBlock[j] ?? System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata\" + BlockInfoService.blockDatas_higher[j].image[0]), 0, 0, 16, 16, blockCssByte[j], 0, 16);
                }
            }
            Console.WriteLine();
        }
        //存储方块css的交叉数组
        byte[][] blockCssByte;

        public Bitmap GetBitamp()
        {
            if (BlockInfoService == null)
            {
                return null;
            }
            //创建一维图像缓存，按照BGR存入
            byte[] buffer = new byte[width * height * 16 * 16 * 3];
            byte blockIndex = 0;
            int width2 = result.GetLength(0) * 16 * 3;
            //生成buffer
            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int z = 0; z < result.GetLength(1); z++)
                {
                    blockIndex = result[x, z];
                    for (int yy = 0; yy < 16; yy++)
                    {
                        Buffer.BlockCopy(blockCssByte[blockIndex], 48 * yy, buffer, (yy + z * 16) * width2 + x * 48, 48);
                    }
                }
            }


            return MySaveBMP(buffer, width * 16, height * 16);
        }



        public static Bitmap MySaveBMP(byte[] buffer, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            // add back dummy bytes between lines, make each line be a multiple of 4 bytes
            int skipByte = bmpData.Stride - width * 3;
            byte[] newBuff = new byte[buffer.Length + skipByte * height];
            for (int j = 0; j < height; j++)
            {

                Buffer.BlockCopy(buffer, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);

                //Console.WriteLine($"From{j * width * 3}to{j * (width * 3 + skipByte)}");
            }

            // fill in rgbValues
            Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
            b.UnlockBits(bmpData);
            // b.Save(@"transformed.bmp", ImageFormat.Bmp);
            return b;
        }

        public void getBGRArrayForBlockBmp(Bitmap image, int startX, int startY, int w, int h, byte[] bgrArray, int offset, int scansize)
        {
            //const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            if (image == null) throw new ArgumentNullException("image");
            if (bgrArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (bgrArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                int pointer = 0;
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), bgrArray, pointer, data.Stride);
                    pointer += data.Stride;
                }

            }
            finally
            {
                image.UnlockBits(data);
            }
        }

    }
}
