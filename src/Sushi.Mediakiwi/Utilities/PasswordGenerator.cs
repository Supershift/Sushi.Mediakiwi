using System;
using System.Text;

namespace Sushi.Mediakiwi.Utilities
{
    /// <summary>
    /// Generate a password based on a possible character string
    /// </summary>
    public class PasswordGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGenerator"/> class.
        /// </summary>
        public PasswordGenerator()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordGenerator"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        public PasswordGenerator(int length)
        {
            this.Length = length;
        }

        string m_PossibleCharacters = "ABCDEFGHKLMNPQRSTUVWXYZ23456789";
        /// <summary>
        /// Gets or sets the possible characters.
        /// </summary>
        /// <value>The possible characters.</value>
        public string PossibleCharacters
        {
            get { return m_PossibleCharacters; }
            set { m_PossibleCharacters = value; }
        }

        int m_Length = 8;
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        /// <summary>
        /// Generates this instance.
        /// </summary>
        /// <returns></returns>
        public string Generate()
        {
            return Generate(Length, PossibleCharacters);
        }

        /// <summary>
        /// Generates the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public string Generate(int length)
        {
            return Generate(length, PossibleCharacters);
        }

        static Random rnd = new Random(Environment.TickCount);

        /// <summary>
        /// Generates the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="possibleCharacters">The possible characters.</param>
        /// <returns></returns>
        public string Generate(int length, string possibleCharacters)
        {
            if (length < 1) return null;
            StringBuilder candidate = new StringBuilder();

            //if (rnd == null)
            //    rnd = new Random(DateTime.Now.Millisecond);// System.Environment.TickCount);

            while (length > 0)
            {
                candidate.Append(possibleCharacters[rnd.Next(0, possibleCharacters.Length - 1)]);
                length--;
            }
            return candidate.ToString();
        }
    }
}
