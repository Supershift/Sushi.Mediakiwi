using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sushi.Mediakiwi.RichRext
{
    public class CharacterCleaner : BaseCleaner
    {
        public string ApplyCharacterClean(string input)
        {
            char[] invalidChars = System.Text.Encoding.ASCII.GetChars(new byte[] { 149 });
            char invalidChar = (char)8226;// '•';            
            string invalidStr = invalidChar.ToString();
            return input.Replace(invalidStr, string.Empty);
        }
    }
}
