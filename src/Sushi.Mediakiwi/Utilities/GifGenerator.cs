using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Data;

//namespace Wim.Utilities
//{
//    /// <summary>
//    /// Generate a textual GIF image.
//    /// </summary>
//    public class GifGenerator
//    {
//        System.Drawing.Image m_gifImage;
//        ColorPalette m_palette;
//        int m_CurrentEntry;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="GifGenerator"/> class.
//        /// </summary>
//        public GifGenerator()
//            : this(new PointF(0, 0), 0)
//        { }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="GifGenerator"/> class.
//        /// </summary>
//        /// <param name="kerning">The kerning.</param>
//        public GifGenerator(float kerning)
//            : this(new PointF(0, 0), kerning)
//        {
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="GifGenerator"/> class.
//        /// </summary>
//        /// <param name="textPostion">The text postion.</param>
//        /// <param name="kerning">The kerning.</param>
//        public GifGenerator(PointF textPostion, float kerning)
//        {
//            m_OriginalTextPosition = textPostion;
//            m_TextPosition = textPostion;
//            m_Kerning = kerning;
//        }

        

//        PointF m_OriginalTextPosition;
//        PointF m_TextPosition;
//        //PointF m_TextPositionTmp;
//        /// <summary>
//        /// Sets the text position.
//        /// </summary>
//        /// <value>The text position.</value>
//        public PointF TextPosition
//        {
//            set { m_TextPosition = value; }
//        }

//        float m_Kerning;
//        /// <summary>
//        /// Sets the kerning.
//        /// </summary>
//        /// <value>The kerning.</value>
//        public float Kerning
//        {
//            set { m_Kerning = value; }
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        public SmoothingMode SmoothingMode = SmoothingMode.Default;
        
//        InterpolationMode InterpolationMode = InterpolationMode.Default;
//        PixelOffsetMode PixelOffsetMode = PixelOffsetMode.HighQuality;
//        CompositingQuality CompositingQuality = CompositingQuality.HighQuality;


//        /// <summary>
//        /// 
//        /// </summary>
//        public TextRenderingHint TextRenderingHint = TextRenderingHint.AntiAlias;

//        /// <summary>
//        /// Gets the font by localfile.
//        /// </summary>
//        /// <param name="file">The file.</param>
//        /// <param name="size">The size.</param>
//        /// <returns></returns>
//        public static Font GetFontByLocalfile(string file, float size)
//        {
//            return GetFontByLocalfile(file, size, FontStyle.Regular);
//        }

//        /// <summary>
//        /// Gets the font by localfile.
//        /// </summary>
//        /// <param name="file">The file.</param>
//        /// <param name="size">The size.</param>
//        /// <param name="style">The style.</param>
//        /// <returns></returns>
//        public static Font GetFontByLocalfile(string file, float size, FontStyle style)
//        {

//            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
//            try
//            {
//                privateFontCollection.AddFontFile(file);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(string.Format("Could not locate font '{0}': {1}", file, ex.Message));
//            }

//            FontFamily fontFamily = privateFontCollection.Families[0];
//            Font font = new Font(fontFamily, size, style, GraphicsUnit.Pixel);
//            return font;
//        }

//        /// <summary>
//        /// Generates the specified file.
//        /// </summary>
//        /// <param name="file">The file.</param>
//        /// <param name="width">The width.</param>
//        /// <param name="height">The height.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="font">The font.</param>
//        /// <returns></returns>
//        public System.Drawing.Image Generate(string file, int width, int height, string title, Font font)
//        {
//            return Generate(file, width, height, title, font, Color.Black);
//        }

//        /// <summary>
//        /// Generates the specified file.
//        /// </summary>
//        /// <param name="file">The file.</param>
//        /// <param name="width">The width.</param>
//        /// <param name="height">The height.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="font">The font.</param>
//        /// <param name="textColor">Color of the text.</param>
//        /// <returns></returns>
//        public System.Drawing.Image Generate(string file, int width, int height, string title, Font font, Color textColor)
//        {
//            return Generate(file, width, height, title, font, textColor, Color.White);
//        }

//        /// <summary>
//        /// Generates the specified file.
//        /// </summary>
//        /// <param name="file">The file.</param>
//        /// <param name="width">The width.</param>
//        /// <param name="height">The height.</param>
//        /// <param name="title">The title.</param>
//        /// <param name="font">The font.</param>
//        /// <param name="textColor">Color of the text.</param>
//        /// <param name="backgroundColor">Color of the background.</param>
//        /// <returns></returns>
//        public System.Drawing.Image Generate(string file, int width, int height, string title, Font font, Color textColor, Color backgroundColor)
//        {
//            return Generate(file, width, height, title, font, textColor, backgroundColor, false);
//        }

//        float GetWordWidth(string title, Font font)
//        {
//            Graphics g = Graphics.FromImage(new Bitmap(100, 100));
//            float info = 0;
//            for (int index = 0; index < title.Length; index++)
//            {
//                if (title[index] == ' ' || title[index] == '\n')
//                {
//                    if (index == 0) return 0;
//                    return g.MeasureString(title.Substring(0, index), font).Width;
//                }
//            }
//            g.Dispose();
//            return info;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="file"></param>
//        /// <param name="width"></param>
//        /// <param name="height">if height = 0 then it will set it.</param>
//        /// <param name="title"></param>
//        /// <param name="font"></param>
//        /// <param name="textColor"></param>
//        /// <param name="backgroundColor"></param>
//        /// <param name="isTransparant"></param>
//        /// <returns></returns>
//        public System.Drawing.Image Generate(string file, int width, int height, string title, Font font, Color textColor, Color backgroundColor, bool isTransparant)
//        {
//            System.Drawing.Image generatedImage = GenerateImage(file, width, height, title, font, textColor, backgroundColor, isTransparant, height == 0);
            

//            if (width == 0)
//            {
//                width = Convert.ToInt32(m_maxwidth) + Convert.ToInt32(title.Length * m_Kerning /2);
//                m_TextPosition = m_OriginalTextPosition;
//                generatedImage = GenerateImage(file, width, height, title, font, textColor, backgroundColor, isTransparant, false);
//            }
//            if (height == 0)
//            {
//                height = Convert.ToInt32(m_lines * m_maxheight);
//                m_TextPosition = m_OriginalTextPosition;
//                generatedImage = GenerateImage(file, width, height, title, font, textColor, backgroundColor, isTransparant, false);
//            }

//            return generatedImage;

//        }

//        System.Drawing.Image GenerateImage(string file, int width, int height, string title, Font font, Color textColor, Color backgroundColor, bool isTransparant, bool skipSave)
//        {
//            if (width == 0) width = 10000;
//            if (height == 0) height = 1;
//            m_lines = 1;
//            m_maxwidth = 0;
//            m_maxheight = 0;
            

//            if (backgroundColor == Color.Transparent)
//            {
//                //  Transparancy not supported by default. Replacing this with the white color which is replace in a later stage.
//                backgroundColor = Color.White;
//                isTransparant = true;
//            }

            
//            m_gifImage = new Bitmap(width, height);

//            Graphics g = Graphics.FromImage(m_gifImage);
//            g.Clear(backgroundColor);

//            g.InterpolationMode = InterpolationMode;
//            g.SmoothingMode = SmoothingMode;
//            g.PixelOffsetMode = PixelOffsetMode;
//            g.CompositingQuality = CompositingQuality;
//            g.TextRenderingHint = TextRenderingHint;

//            SolidBrush fgBrush = new SolidBrush(textColor);

//            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
//            format.Alignment = StringAlignment.Near;
//            format.Trimming = StringTrimming.Character;

//            SizeF stringSize;

//            if (m_Kerning != 0)
//            {
//                float kerningHalve = m_Kerning == 0 ? 0 : m_Kerning / 2;
//                float calculatedLetterSpace;
//                float letterspace;

//                int index = 0;


//                foreach (char c in title.ToCharArray())
//                {
//                    index++;

//                    string remainder = title.Substring(index, title.Length - index);
//                    float remainderNextWordBreak = 0;

//                    if (c == ' ')
//                    {
//                        stringSize = g.MeasureString("A", font, new PointF(0, 0), format);
//                        letterspace = GetLetterInfo(stringSize, 'A', font) / 2;
//                    }
//                    else
//                    {
//                        remainderNextWordBreak = GetWordWidth(remainder, font);

//                        stringSize = g.MeasureString(c.ToString(), font, new PointF(0, 0), format);
//                        letterspace = GetLetterInfo(stringSize, c, font);
//                    }

//                    calculatedLetterSpace = (letterspace + kerningHalve);

//                    if (letterspace != stringSize.Width)
//                        m_TextPosition.X -= ((stringSize.Width - letterspace) / 2) - kerningHalve;

//                    float rightBorder = m_TextPosition.X + RoundFloatUp(stringSize.Width) + remainderNextWordBreak;

//                    m_maxwidth = rightBorder;
//                    //Console.Out.WriteLine(string.Format("{0} --> x: {1} - w: {2} RightBorder: {3} - remainerwordbreak: {4}", c, m_TextPosition.X, stringSize.Width, rightBorder, remainderNextWordBreak));

//                    if (c == '\n' || rightBorder > width)
//                    {
//                        m_TextPosition.X = 0;
//                        m_TextPosition.Y += m_maxheight;
//                        m_lines++;
//                    }

//                    float roundedWidth = RoundFloat(stringSize.Width);
//                    if (roundedWidth < calculatedLetterSpace)
//                    {
//                        roundedWidth = calculatedLetterSpace;
//                    }
                    
//                    RectangleF rectF = new RectangleF(RoundFloat(m_TextPosition.X), RoundFloat(m_TextPosition.Y), RoundFloatUp(stringSize.Width), RoundFloat(stringSize.Height));

//                    if (m_maxheight < stringSize.Height)
//                        m_maxheight = stringSize.Height;


//                    //if (c == ' ')
//                    //    g.DrawString("_", font, fgBrush, rectF, format);
//                    //else
//                        g.DrawString(c.ToString(), font, fgBrush, rectF, format);
//                    m_TextPosition.X += (letterspace + kerningHalve);
//                    if (c == ' ')
//                        calculatedLetterSpace = 3;

//                }
//            }
//            else
//            {
//                RectangleF rectF = new RectangleF(m_TextPosition.X, m_TextPosition.Y, width, height);
//                g.DrawString(title, font, fgBrush, rectF, format);
//            }

//            if (skipSave) return null;


//            //  For GIF quality (hotfix in GDI+)
//            Supporting.OctreeQuantizer q = new Supporting.OctreeQuantizer(255, 8);
//            m_gifImage = q.Quantize(m_gifImage);

//            if (isTransparant)
//            {
//                m_palette = m_gifImage.Palette;
//                SetTransparentImage(backgroundColor);
//            }

//            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, 90L);
//            ImageCodecInfo codec = GetEncoderInfo("image/gif");

//            EncoderParameters encoderParams = new EncoderParameters(1);
//            encoderParams.Param[0] = qualityParam;

//            m_gifImage.Save(file, codec, encoderParams);

//            Console.Out.WriteLine(">" + m_lines.ToString());

//            Console.Out.Flush();

//            return m_gifImage;
//        }

//        int m_lines = 0;
//        float m_maxheight = 0;
//        float m_maxwidth = 0;

//        float RoundFloat(float candidate)
//        {
//            return float.Parse(Decimal.Round(Convert.ToDecimal(candidate), 1).ToString());
//        }

//        float RoundFloatUp(float candidate)
//        {
//            return float.Parse(Decimal.Ceiling(Convert.ToDecimal(candidate)).ToString());
//        }

//        float GetLetterInfo(SizeF stringSize, char letter, Font font)
//        {
//            if (m_Kerning == 0) return stringSize.Width;

//            StringFormat format = new StringFormat();
//            format.Alignment = StringAlignment.Near;
//            format.Trimming = StringTrimming.Character;

//            System.Drawing.Image image = new Bitmap(50, 50);
//            Graphics g = Graphics.FromImage(image);
//            g.Clear(Color.White);
//            RectangleF rectF = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
//            g.DrawString(letter.ToString(), font, new SolidBrush(Color.Black), rectF, format);

//            float freespace = GetLastPixel(image, rectF, Color.Black);
//            return freespace;
//        }

//        float GetLastPixel(System.Drawing.Image image, RectangleF rectF, Color brushcolor)
//        {
//            int top = Convert.ToInt32(rectF.Top);
//            int right = Convert.ToInt32(rectF.Right);
//            bool found = false;
//            while (right > Convert.ToInt32(rectF.Left))
//            {
//                top = Convert.ToInt32(rectF.Top);
//                while (top < Convert.ToInt32(rectF.Bottom))
//                {
//                    Color X = ((Bitmap)image).GetPixel(right, top);
//                    if (X.ToArgb() == brushcolor.ToArgb())
//                    {
//                        found = true;
//                        break;
//                    }
//                    top++;
//                }
//                if (found) return (right - Convert.ToInt32(rectF.Left));
//                right--;
//            }
//            return 0;
//        }


//        /// <summary> 
//        /// Returns the image codec with the given mime type 
//        /// </summary> 
//        ImageCodecInfo GetEncoderInfo(string mimeType)
//        {
//            // Get image codecs for all image formats 
//            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

//            // Find the correct image codec 
//            for (int i = 0; i < codecs.Length; i++)
//                if (codecs[i].MimeType == mimeType)
//                    return codecs[i];
//            return null;
//        }

//        void SetTransparentImage(Color backgroundcolor)
//        {
//            //Creates a new GIF image with a modified colour palette 
//            if (m_palette != null)
//            {
//                //Create a new 8 bit per pixel image 
//                Bitmap bm = new Bitmap(m_gifImage.Width, m_gifImage.Height, PixelFormat.Format8bppIndexed);
//                //get it's palette 

//                ColorPalette ncp = bm.Palette;
//                //copy all the entries from the old palette removing any transparency 

//                int n = 0;

//                foreach (Color c in m_palette.Entries)

//                    ncp.Entries[n++] = Color.FromArgb(255, c);

//                int i = 0;
//                foreach (Color c in m_palette.Entries)
//                {
//                    if (c.ToArgb() == backgroundcolor.ToArgb())
//                    {
//                        m_CurrentEntry = i;
//                        break;
//                    }
//                    i++;
//                }

//                //Set the newly selected transparency 
//                ncp.Entries[m_CurrentEntry] = Color.FromArgb(0, m_palette.Entries[m_CurrentEntry]);

//                //re-insert the palette 

//                bm.Palette = ncp;

//                //now to copy the actual bitmap data 
//                //lock the source and destination bits 
//                BitmapData src = ((Bitmap)m_gifImage).LockBits(new Rectangle(0, 0, m_gifImage.Width, m_gifImage.Height), ImageLockMode.ReadOnly, m_gifImage.PixelFormat);
//                BitmapData dst = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.WriteOnly, bm.PixelFormat);

//                //uses pointers so we need unsafe code. 
//                //the project is also compiled with /unsafe 
//                unsafe
//                {
//                    //steps through each pixel 
//                    for (int y = 0; y < m_gifImage.Height; y++)
//                        for (int x = 0; x < m_gifImage.Width; x++)
//                        {
//                            //transferring the bytes 
//                            ((byte*)dst.Scan0.ToPointer())[(dst.Stride * y) + x] = ((byte*)src.Scan0.ToPointer())[(src.Stride * y) + x];
//                        }
//                }

//                //all done, unlock the bitmaps 
//                ((Bitmap)m_gifImage).UnlockBits(src);
//                bm.UnlockBits(dst);

//                m_gifImage.Dispose();
//                //set the new image in place 
//                m_gifImage = bm;
//                m_palette = m_gifImage.Palette;
//            }
//        }
//    }
//}
