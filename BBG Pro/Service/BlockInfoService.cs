using Colourful;
using Colourful.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static BBG_Pro.CommonStructure;

namespace BBG_Pro.Service
{
    //提供方块信息寄存的服务
    public class BlockInfoService
    {
        public List<BlockData> choosen_blockDatas = new List<BlockData>();//已选择的方块信息
        public List<BlockData> result_blockDatas = new List<BlockData>();//输出方块信息

        public bool[] classEnabled;//启用颜色
        public string[] demoBlock;//用于显示的方块的URI

        List<BlockData> blockDatas_higher = new List<BlockData>();//更高的方块信息
        List<BlockData> blockDatas_flat = new List<BlockData>();//齐平的方块信息
        List<BlockData> blockDatas_lower = new List<BlockData>();//更低的方块信息

        List<List<string>> block_image = new List<List<string>>();//存储方块图片Path

        float multiplier_flat = 0.86274f;

        float multiplier_lower = 0.70588f;

        public bool ThreeDEnabled = false;
        /// <summary>
        /// 初始化h,f,l
        /// </summary>
        public void Init()
        {
            LoadAllBlocks();

            ColourfulConverter converter = new ColourfulConverter { WhitePoint = Illuminants.D65 };
            foreach (var item in blockDatas_higher)
            {
                RGBColor RGB = RgbMulitply(multiplier_flat, item.RGBColor);
                LabColor lab = converter.ToLab(RGB);
                BlockData blockData = new BlockData(item.name, item.metadata, lab, RGB, item.image, item.classid, 1);
                blockDatas_flat.Add(blockData);
                RGBColor RGB2 = RgbMulitply(multiplier_lower, item.RGBColor);
                LabColor lab2 = converter.ToLab(RGB);
                BlockData blockData2 = new BlockData(item.name, item.metadata, lab, RGB, item.image, item.classid, 0);
                blockDatas_lower.Add(blockData);
            }
        }

        /// <summary>
        /// 载入方块信息
        /// </summary>
        /// <returns></returns>
        public string LoadAllBlocks()
        {
            int color_cnt = 0;
            int image_cnt = 0;
            List<BlockData> tmp = new List<BlockData>();

            try
            {
                StreamReader F = new StreamReader("BlockData.txt");
                string data = F.ReadToEnd();
                ColourfulConverter converter = new ColourfulConverter { WhitePoint = Illuminants.D65 };


                foreach (var blockclass in data.Trim().Split('\n'))
                {
                    if (blockclass == string.Empty)
                    {
                        continue;
                    }
                    ++color_cnt;
                    string[] dts = blockclass.Trim().Split(',');
                    if (dts.Length == 7)
                    {
                        RGBColor RGB = new RGBColor(System.Drawing.Color.FromArgb(255, int.Parse(dts[2]), int.Parse(dts[3]), int.Parse(dts[4])));
                        LabColor lab = converter.ToLab(RGB);
                        BlockData blockData = new BlockData(dts[5], 0, lab, RGB, dts[6].TrimEnd(';').Split(';'), byte.Parse(dts[0]), 2);
                        tmp.Add(blockData);
                    }
                }
                classEnabled = new bool[color_cnt];
                demoBlock = new string[color_cnt];
                blockDatas_higher = tmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ERR-READ-001 读取方块信息时出现异常：" + ex.Message);
                throw;
            }
            for (int i = 0; i < classEnabled.Length; i++)
            {
                classEnabled[i] = true;
            }
            return $"{color_cnt},{image_cnt}";
        }
        public void EnableAllHeight()
        {
            choosen_blockDatas.Clear();
            choosen_blockDatas.AddRange(blockDatas_higher);
            choosen_blockDatas.AddRange(blockDatas_flat);
            choosen_blockDatas.AddRange(blockDatas_lower);
            IsHigherEnabled = true;
            IsFlatEnabled = true;
            IsLowerEnabled = true;
            result_blockDatas = choosen_blockDatas;
            ThreeDEnabled = true;
        }
        public void DisableHeight()
        {
            choosen_blockDatas.Clear();
            choosen_blockDatas.AddRange(blockDatas_flat);
        }
        public BlockData[] GetBlockDatas() => blockDatas_flat.ToArray();
        public BlockData[] GetChoosenBlocks()
        {
            if (result_blockDatas.Count != 0)
            {
                return result_blockDatas.ToArray();
            }
            else
            {
                return choosen_blockDatas.ToArray();
            }
        }
        private RGBColor RgbMulitply(double rate, RGBColor rgb)
        {
            return new RGBColor(rgb.R * rate, rgb.G * rate, rgb.B * rate);
        }

        public enum HeightState
        {
            higher,
            flat,
            lower,
        }

        bool IsHigherEnabled = false;
        bool IsFlatEnabled = false;
        bool IsLowerEnabled = false;

        public void EnableHeight(HeightState heightState)
        {
            switch (heightState)
            {
                case HeightState.higher:
                    if (!IsHigherEnabled)
                    {
                        IsHigherEnabled = true;
                        choosen_blockDatas.AddRange(blockDatas_higher);
                    }
                    break;
                case HeightState.flat:
                    if (!IsFlatEnabled)
                    {
                        IsFlatEnabled = true;
                        choosen_blockDatas.AddRange(blockDatas_flat);
                    }
                    break;
                case HeightState.lower:
                    if (!IsLowerEnabled)
                    {
                        IsLowerEnabled = true;
                        choosen_blockDatas.AddRange(blockDatas_lower);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearHeight()
        {
            IsHigherEnabled = false;
            IsFlatEnabled = false;
            IsLowerEnabled = false;
            choosen_blockDatas.Clear();
        }
    }
}
