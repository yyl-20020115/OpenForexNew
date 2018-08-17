using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace CommonSupport
{
    public static class GraphicsHelper
    {

        /// <summary>
        /// Static constructor.
        /// </summary>
        static GraphicsHelper()
        {
        }

        /// <summary>
        /// Will draw the image, replacing oldColor with the foreColor.
        /// And also replacing the color at [0,0] to be the default transparent color.
        /// Usefull for operations with system BMP images.
        /// </summary>
        static public void DrawImageColorMapped(Graphics g, Bitmap image, Rectangle rectangle, Color oldColor, Color foreColor)
        {
            ColorMap[] colorMap = new ColorMap[2];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = oldColor;
            colorMap[0].NewColor = foreColor;
            
            colorMap[1] = new ColorMap();
            colorMap[1].OldColor = image.GetPixel(0, 0);
            colorMap[1].NewColor = Color.Transparent;

            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetRemapTable(colorMap);

                g.DrawImage(
                   image,
                   //new Rectangle(0, 0, image.Width, image.Height),
                   rectangle,
                   0, 0,
                   image.Width,
                   image.Height,
                   GraphicsUnit.Pixel,
                   imageAttributes);
            }
        
        }

    }
}
