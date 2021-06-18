using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using System;
using static BBG_Pro.CommonStructure;

namespace BBG_Pro.Service
{
    //提供地图信息生成的服务
    class MapGenerator
    {
        delegate double errCalc<T, U>(T orginal, U sample);
        errCalc<RGBColor, BlockData> GetErr;
        byte[,,] Mapping;//缓存
        public byte[,] result;
        public byte[,] height;

        public void Generate(BlockData[] blockDatas,byte[,,] rgbArray) 
        {
            init();
            Mapping = new byte[256,256,256];
            int imgwidth = rgbArray.GetLength(0);
            int imgheight = rgbArray.GetLength(1);
            result = new byte[imgwidth, imgheight];
            byte cache = 0;
            double deltae = 0;
            double tmp_deltae = 0;
            byte tmp_id = 0;
            int hitcache = 0;

            RGBColor tmpRGB;
            for (int x = 0; x < rgbArray.GetLength(0); x++)
            {
                for (int y = 0; y < rgbArray.GetLength(1); y++)
                {
                    cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                    if (cache != 0)
                    {//命中缓存
                        result[x, y] = cache;
                        hitcache++;
                        continue;
                    }
                    else
                    {//未命中，计算最适合的方块 
                        deltae = double.MaxValue;
                        tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));

                        for (byte _id = 0; _id < blockDatas.Length; _id++)
                        {
                            tmp_deltae = GetErr(tmpRGB, blockDatas[_id]);
                            if (tmp_deltae < deltae)
                            {
                                deltae = tmp_deltae;
                                tmp_id = _id;
                            }
                        }
                        Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(blockDatas[tmp_id].classid - 1);
                        result[x, y] = (byte)(blockDatas[tmp_id].classid - 1);
                    }
                }
            }
            Console.WriteLine($"done!hit:{hitcache},total{imgwidth * imgheight},{(double)hitcache / imgwidth / imgheight*100}%");
        }
        public void Generate3D(BlockData[] blockDatas, byte[,,] rgbArray) 
        {
            init();
            Mapping = new byte[256, 256, 256];
            int imgwidth = rgbArray.GetLength(0);
            int imgheight = rgbArray.GetLength(1);
            height = new byte[imgheight, imgheight];
            result = new byte[imgwidth, imgheight];
            byte cache = 0;
            double deltae = 0;
            double tmp_deltae = 0;
            byte tmp_id = 0;
            int hitcache = 0;

            RGBColor tmpRGB;
            for (int x = 0; x < rgbArray.GetLength(0); x++)
            {
                for (int y = 0; y < rgbArray.GetLength(1); y++)
                {
                    cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                    if (cache != 0)
                    {//命中缓存
                        result[x, y] = cache;
                        height[x, y] = (blockDatas[cache].height);

                        hitcache++;
                        continue;
                    }
                    else
                    {//未命中，计算最适合的方块 
                        deltae = double.MaxValue;
                        tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));

                        for (byte _id = 0; _id < blockDatas.Length; _id++)
                        {
                            tmp_deltae = GetErr(tmpRGB, blockDatas[_id]);
                            if (tmp_deltae < deltae)
                            {
                                deltae = tmp_deltae;
                                 tmp_id = _id;
                            }
                        }
                        Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(blockDatas[tmp_id].classid - 1);
                        result[x, y] = (byte)(blockDatas[tmp_id].classid - 1);
                        height[x,y]= (blockDatas[tmp_id].height);
                    }
                }
            }
        }

        bool useLabcolor = true;
        private void init()
        {
            if (useLabcolor)
            {
                GetErr = LABDifference;

            }
            else
            {
                GetErr = RGBDifference;
            }
        }

        private double RGBDifference(RGBColor color0, BlockData color1)
        {
            return Math.Pow(color0.R-color1.RGBColor.R,2)+ Math.Pow(color0.G - color1.RGBColor.G, 2)+ Math.Pow(color0.B - color1.RGBColor.B, 2);
        }

        public static ColourfulConverter converter = new ColourfulConverter { WhitePoint = Illuminants.D65 };
        CIEDE2000ColorDifference cIEDE2000ColorDifference = new CIEDE2000ColorDifference();
        private double LABDifference(RGBColor color0, BlockData color1)
        {
            return cIEDE2000ColorDifference.ComputeDifference(converter.ToLab(color0), color1.LabColor);
        }
    }
}
