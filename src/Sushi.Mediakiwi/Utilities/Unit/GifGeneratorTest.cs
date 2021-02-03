#if DEBUG
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text;
//using NUnit.Framework;

//namespace Wim.Utilities.Unit
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    [TestFixture]
//    public class GifGeneratorTest
//    {
//        /// <summary>
//        /// Creates the GIF.
//        /// </summary>
//        [Test()]
//        public void CreateGif()
//        {
//            GifGenerator gifgen = new GifGenerator();
//            gifgen.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
//            gifgen.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
//            gifgen.Kerning = 1.0F;

//            System.Drawing.Color textColor = System.Drawing.Color.White;

//            string file = "INTBD___.ttf";

//            Assert.IsTrue(File.Exists(file));

//            System.Drawing.Color bgcolor = System.Drawing.Color.Black;
//            System.Drawing.Font font = GifGenerator.GetFontByLocalfile(file, 15.0F, System.Drawing.FontStyle.Regular);

//            System.Drawing.Image img = gifgen.Generate("c:\\test.gif", 300, 0, "Nieuwe waaromwoord?\n ineengroot woordverhaal in een 2 regelinge notedop, jammer hoor.  >>>", font, textColor, bgcolor, false);
//            Console.Out.WriteLine(img.Width.ToString());
//            Console.Out.WriteLine(img.VerticalResolution.ToString());
//            img.Dispose();
//            img = null;
//        }

//        /// <summary>
//        /// Creates the gif2.
//        /// </summary>
//        [Test()]
//        public void CreateGif2()
//        {
//            GifGenerator gifgen = new GifGenerator();
//            gifgen.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
//            gifgen.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
//            gifgen.Kerning = 1.0F;

//            System.Drawing.Color textColor = System.Drawing.Color.White;

//            string file = "INTR____.ttf";

//            Assert.IsTrue(File.Exists(file));

//            System.Drawing.Color bgcolor = System.Drawing.Color.Black;
//            System.Drawing.Font font = GifGenerator.GetFontByLocalfile(file, 13.0F, System.Drawing.FontStyle.Regular);

//            System.Drawing.Image img = gifgen.Generate("c:\\test2.gif", 120, 0, "Klanten", font, textColor, bgcolor, true);
//            Console.Out.WriteLine(img.Width.ToString());
//            Console.Out.WriteLine(img.VerticalResolution.ToString());
//            img.Dispose();
//            img = null;
//        }


//        /// <summary>
//        /// Creates the gif3.
//        /// </summary>
//        [Test()]
//        public void CreateGif3()
//        {
//            GifGenerator gifgen = new GifGenerator();
//            gifgen.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
//            gifgen.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
//            gifgen.Kerning = 1.0F;

//            System.Drawing.Color textColor = System.Drawing.Color.Black;

//            string file = "INTR____.ttf";

//            Assert.IsTrue(File.Exists(file));

//            System.Drawing.Color bgcolor = System.Drawing.Color.White;
//            System.Drawing.Font font = GifGenerator.GetFontByLocalfile(file, 13.0F, System.Drawing.FontStyle.Regular);

//            System.Drawing.Image img = gifgen.Generate("c:\\test3.gif", 0, 0, "Terms & Conditions", font, textColor, bgcolor, true);
//            //Console.Out.WriteLine(img.Width.ToString());
//            //Console.Out.WriteLine(img.VerticalResolution.ToString());
//            img.Dispose();
//            img = null;
//        }
//    }
//}
#endif
