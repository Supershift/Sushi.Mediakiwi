using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Sushi.Mediakiwi.Framework
{
    /// <summary>
    /// 
    /// </summary>
    public class TestFilter : System.IO.Stream
    {
        private Regex m_formAction = new Regex(@"action="".*?""", RegexOptions.IgnoreCase);

        private System.IO.MemoryStream moMemoryStream = new System.IO.MemoryStream();
        private System.IO.Stream moStream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public TestFilter(System.IO.Stream stream)
        {
            moStream = stream;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Length
        {
            get
            {
                return moMemoryStream.Length;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override long Position
        {
            get
            {
                return moMemoryStream.Position;
            }
            set
            {
                moMemoryStream.Position = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return moMemoryStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public override long Seek(long offset, System.IO.SeekOrigin direction)
        {
            return moMemoryStream.Seek(offset, direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        public override void SetLength(long length)
        {
            moMemoryStream.SetLength(length);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            moStream.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Flush()
        {
            moStream.Flush();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            System.Text.UTF8Encoding utf8 = new UTF8Encoding();
            
            //string candidate = m_formAction.Replace(utf8.GetString(buffer), m_formActionReplacement);
            string candidate = utf8.GetString(buffer);
            
            
            buffer = utf8.GetBytes(candidate);
            count = buffer.Length;

            moStream.Write(buffer, offset, count);
            moMemoryStream.Write(buffer, offset, count);
        }
    }
}
