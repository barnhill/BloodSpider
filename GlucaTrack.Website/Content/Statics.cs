using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlucaTrack.Website.Content
{
    public class Statics
    {
        public static int LowThreshold = 70;
        public static int HighThreshold = 110;

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }
    }
}