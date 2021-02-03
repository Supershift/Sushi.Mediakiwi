using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;

namespace Wim.Utilities
{
	/// <summary>
	/// Summary description for Thumbnailer.
	/// </summary>
	public class Thumbnailer : IDisposable
	{
        string m_Filepath;
        /// <summary>
        /// The physical path of the image
        /// </summary>
        public string Filepath
        {
            get { return m_Filepath; }
            set { m_Filepath = value; }
        }

        #region Dispose
        private bool disposed = false;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    //adoSp.Dispose();
                }
            }
            disposed = true;         
        }

        /// <summary>
        /// 
        /// </summary>
        ~Thumbnailer()      
        {
            Dispose(false);
        }
        #endregion

        #region Global variables
        //private HttpContext Context = HttpContext.Current;
        private int _thumbnailWidth;
        private int _thumbnailHeight;
        private string _imagePath;
        private ImageFormat _contentType;
        private bool _isAlwaysSpecifiedSize;
        private bool _isEnlargedWhenSmallerThenSpecified;
        private Color _backGroundColor;
        private long _quality;
        private string _imgName;
        private bool _rotated = false;
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        public bool Rotated
        {
            get { return _rotated; }
            set { _rotated = value; }
        }        
        
        /// <summary>
        /// Width of the to be made Thumbnail
        /// </summary>
        public int ThumbNailWidth
        {
            get { return _thumbnailWidth; }
            set { _thumbnailWidth = value; }
        }

        /// <summary>
        /// Quality of the to be made Thumbnail
        /// </summary>
        public long ThumbnailQuality
        {
            get { return _quality; }
            set { _quality = value; }
        }

        /// <summary>
        /// Height of the to be made thumbnail
        /// </summary>
        public int ThumbNailHeight
        {
            get { return _thumbnailHeight; }
            set { _thumbnailHeight = value; }
        }
     
        /// <summary>
        /// Path of the image
        /// </summary>
        private string ImageInput
        {
            get { return _imgName; }
            set { _imgName = value; }
        }
        
        /// <summary>
        /// Path of the image
        /// </summary>
        public string ImageVirtualPath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        /// <summary>
        /// The image is not enlarged when it is smaller then specified
        /// </summary>
        public bool IsEnlargedWhenSmallerThenSpecified
        {
            get { return _isEnlargedWhenSmallerThenSpecified; }
            set { _isEnlargedWhenSmallerThenSpecified = value; }
        }

        /// <summary>
        /// Format of the to be made thumbnail
        /// </summary>
        public ImageFormat ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        /// <summary>
        /// If the to be created thumbnail shows colored background/borders.
        /// </summary>
        public bool IsAlwaysSpecifiedSize
        {
            get { return _isAlwaysSpecifiedSize; }
            set { _isAlwaysSpecifiedSize = value; }
        }

        /// <summary>
        /// Background/Border color
        /// </summary>
        public Color BackGroundColor
        {
            get { return _backGroundColor; }
            set { _backGroundColor = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public Thumbnailer()
        {
            //ApplicationPath = Context.Request.ApplicationPath;
            ContentType = ImageFormat.Jpeg;
            IsAlwaysSpecifiedSize = true;
            BackGroundColor = Color.White;
            ThumbnailQuality = 80;
            IsEnlargedWhenSmallerThenSpecified = false;
        }
        #endregion

        #region Thumbnail creation
        /// <summary>
        /// Create the thumbnail
        /// </summary>
        /// <param name="imgPhoto">The img photo.</param>
        /// <param name="Width">The width.</param>
        /// <param name="Height">The height.</param>
        /// <param name="cropWidth">Width of the crop.</param>
        /// <param name="cropHeight">Height of the crop.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public System.Drawing.Image FixedSize(System.Drawing.Image imgPhoto, int Width, int Height, int cropWidth, int cropHeight, Sushi.Mediakiwi.Data.ImagePosition position)
        {
            if (IsAlwaysSpecifiedSize)
            {
                if (cropWidth > Width)
                    Width = cropWidth;

                if (cropHeight > Height)
                    Height = cropHeight;
            }

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0; 
            bool imgIsSame = false;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width/(float)sourceWidth);
            nPercentH = ((float)Height/(float)sourceHeight);
            if (!IsEnlargedWhenSmallerThenSpecified && Width >=  sourceWidth && Height >= sourceHeight)
            {
                imgIsSame = true;
                nPercentH = 1;
                nPercentW = 1;
            }

            if(nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent))/2);
            }
            else if (nPercentH > nPercentW)
            {
                nPercent = nPercentW;
                
                if (position == Data.ImagePosition.Center) 
                    destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
                
                if (imgIsSame)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
                }
            }
            else
            {
                nPercent = nPercentW;

                //if (position == Data.ImagePosition.Center)
                //    destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);

                if (imgIsSame)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
                }

                if (position == Data.ImagePosition.Center || position == Data.ImagePosition.TopCenter)
                {
                    if (Width > sourceWidth)
                        destX = System.Convert.ToInt32((Width - sourceWidth) / 2);
                }
                if (position == Data.ImagePosition.Center)
                {
                    if (Height > sourceHeight)
                        destY = System.Convert.ToInt32((Height - sourceHeight) / 2);
                }
            }

            decimal tmpWidth = Decimal.Multiply(Convert.ToDecimal(sourceWidth), Convert.ToDecimal(nPercent));
            int destWidth = Convert.ToInt32(tmpWidth);

            decimal tmpHeight = Decimal.Multiply(Convert.ToDecimal(sourceHeight), Convert.ToDecimal(nPercent));
            int destHeight = Convert.ToInt32(tmpHeight);

            Bitmap bmPhoto = null;
            if (IsAlwaysSpecifiedSize)
            {
                if (this.Rotated)
                    bmPhoto = new Bitmap(Height, Width, PixelFormat.Format24bppRgb );
                else
                    bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb );
            }
            else 
            {
                // Rotating
                if (this.Rotated)
                    bmPhoto = new Bitmap(destHeight, destWidth, PixelFormat.Format24bppRgb );
                else
                    bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format24bppRgb );
                destX = 0;
                destY = 0;
            }


            Graphics grPhoto = Graphics.FromImage(bmPhoto);

            if (this.Rotated)
                grPhoto.RotateTransform(90F, MatrixOrder.Prepend);            
            
            grPhoto.Clear(BackGroundColor);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBilinear;

            //destY = 0;
                
            if (imgIsSame)
            {

                grPhoto.DrawImage(imgPhoto, 
                    new Rectangle(destX,destY,sourceWidth,sourceHeight),
                    new Rectangle(sourceX,sourceY,sourceWidth,sourceHeight),
                    GraphicsUnit.Pixel);
            }
            else
            {
                if (this.Rotated)
                {
                    grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX - 1, destY - destHeight - 1, destWidth + 1, destHeight + 1),
                        new Rectangle(sourceX, sourceY, sourceWidth,sourceHeight),
                        GraphicsUnit.Pixel);
                }
                else
                {
                    //  [20070705:MM] Added a deduct/add 1 pixel to avoid borderforming
                    grPhoto.DrawImage(imgPhoto,
                        new Rectangle(destX - 1, destY - 1, destWidth + 1, destHeight + 1),
                        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel);

                }
            }

            // MarkR changed this on 21-09-2012, to fix an OutOfMemory Exception
            /// was : 
            if (cropWidth > 0 && cropHeight > 0)
            //if ((cropWidth > 0 && cropHeight > 0) && (cropWidth < Width && cropHeight < Height))
            {
                Rectangle cropArea = new Rectangle(0, 0, cropWidth, cropHeight);
                bmPhoto = bmPhoto.Clone(cropArea, bmPhoto.PixelFormat);
            }

            grPhoto.Dispose();
            return bmPhoto;
        }

        #endregion

        #region Save thumbnail
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public bool SaveThumbnail(System.Drawing.Image img)
        {
            
            try
            {
                //Build image encoder details
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, ThumbnailQuality);

                //Get jpg format ID
                foreach (ImageCodecInfo encoder in encoders)
                {
                    if (encoder.MimeType.IndexOf(ContentType.ToString().ToLower(), 0) > 1)
                    {
                        // Save
                        try
                        {
                            if (Sushi.Mediakiwi.Data.Asset.HasCloudSetting)
                            {
                                MemoryStream stream = new MemoryStream();
                                img.Save(stream, encoder, encoderParameters);

                                var p1 = this.Filepath.Split(new string[] { "/repository/generated/" }, StringSplitOptions.RemoveEmptyEntries)[1];
                                var p2 = p1.LastIndexOf('/');

                                int pos = p1.LastIndexOf('/');
                                var part1 = p1.Substring(0, pos);
                                pos++;
                                var part2 = p1.Substring(pos, p1.Length - pos);

                                Sushi.Mediakiwi.Data.Asset.m_CloundInstance.UploadGeneratedContent(stream, part2, "jpg", part1);
                            }
                            else
                                img.Save(this.Filepath, encoder, encoderParameters);

                            img.Dispose();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Cannot write to the thumbnail folder", ex);
                        }

                    }
                }
                throw new Exception("Suitable JPEG encoder format not found");
            }
            catch (Exception)
            {
                if (img != null)
                    img.Dispose();
                throw;
            }
            finally
            {
                if(img != null)
                    img.Dispose();
            }
        }
        #endregion
    }
}
