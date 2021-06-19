using Colourful;

namespace BBG_Pro
{
    //提供广泛的自定类型，以及一些方法
    public static class CommonStructure
    {

        //二维模型
        public struct BlockData
        {
            public readonly string name;//形如 namespace:blockname 的方块唯一id
            public readonly byte metadata;//附加值
            public readonly LabColor LabColor;//Lab颜色
            public readonly RGBColor RGBColor;//RGB颜色
            public readonly string[] image;//方块图片
            public readonly byte classid;//方块图片
            public readonly byte height;
            public bool enabled;

            public BlockData(string namae, byte meta, LabColor lab, RGBColor rGB, string[] img, byte cls, byte h, bool b = false)
            {
                name = namae;
                metadata = meta;
                LabColor = lab;
                RGBColor = rGB;
                image = img;
                classid = cls;
                height = h;
                enabled = b;
            }
        }

        //三维模型
        /// <summary>
        /// 三维的blockdata。增加了
        /// </summary>
        public struct BlockData3D
        {
            public readonly string name;//形如 namespace:blockname 的方块唯一id
            public readonly byte metadata;//附加值
            public readonly LabColor LabColor;//Lab颜色
            public readonly RGBColor RGBColor;//RGB颜色
            public readonly string image;//方块图片
            public byte height;//方块高度

        }

        /// <summary>
        /// 方块结果数据，含有方块数目统计
        /// </summary>
        public struct BlockResult
        {
            public readonly string name;//形如 namespace:blockname 的方块唯一id
            public readonly byte metadata;//附加值
            public readonly LabColor LabColor;//Lab颜色
            public readonly RGBColor RGBColor;//RGB颜色
            public readonly string image;//方块图片
            public byte height;//方块高度
            public int count;//计数

        }

        public static void CopyBlockData(this BlockData blockData, BlockResult block)
        {

        }
    }
}
