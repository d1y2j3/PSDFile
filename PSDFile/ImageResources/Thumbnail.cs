using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSDFile
{
    public class Thumbnail : RawImageResource
    {
        public Bitmap Image { get; private set; }

        public Thumbnail(ResourceID id, string name) : base(id, name)
        {
        }

        public Thumbnail(PsdBinaryReader psdReader, ResourceID id, string name, int numBytes)
            : base(psdReader, "8BIM", id, name, numBytes)
        {
            using (var memoryStream = new MemoryStream(Data))
            {
                using (var reader = new PsdBinaryReader(memoryStream, psdReader))
                {
                    const int HEADER_LENGTH = 28;
                    var format = reader.ReadUInt32();
                    var width = reader.ReadUInt32();
                    var height = reader.ReadUInt32();
                    var widthBytes = reader.ReadUInt32();
                    var size = reader.ReadUInt32();
                    var compressedSize = reader.ReadUInt32();
                    var bitPerPixel = reader.ReadUInt16();
                    var planes = reader.ReadUInt16();

                    Bitmap bitmap = null;
                    if (format == 0) // Raw RGB bitmap
                    {
                        // 根据读取的数据创建 Bitmap
                        // 注意：这里需要更多的代码来处理实际的像素数据
                        // 以下代码仅为示例
                        bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format24bppRgb);
                    }
                    else if (format == 1) // JPEG bitmap
                    {
                        var imgData = reader.ReadBytes(numBytes - HEADER_LENGTH);
                        using (MemoryStream stream = new MemoryStream(imgData, false))
                        {
                            bitmap = new Bitmap(stream);
                        }


                    }
                    else
                    {
                        throw new PsdInvalidException("Unknown thumbnail format.");
                    }

                    // 将读取的 Bitmap 赋值给 Image 属性
                    Image = bitmap; // 注意：这里不再克隆 Bitmap
                }   
            }  
        }
    }
}
