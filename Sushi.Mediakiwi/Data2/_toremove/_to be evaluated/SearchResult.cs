using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    //public enum SearchArea
    //{
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Page = 1,
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Product = 2,
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    Structure = 3
    //}

    /// <summary>
    /// 
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// All words in the query
        /// </summary>
        All_Word = 1,
        /// <summary>
        /// All words in the query with the addition that the supplied word can also be part of a found result word
        /// </summary>
        All_Word_And_Partial = 2,
        /// <summary>
        /// Any of the words in the query
        /// </summary>
        Any_Word = 3,
        /// <summary>
        /// Any of the words in the query with the addition that the supplied word can also be part of a found result word
        /// </summary>
        Any_Word_And_Partial = 4,
        /// <summary>
        /// The exact phrase
        /// </summary>
        Exact_Phrase = 5
    }

    /// <summary>
    /// 
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Converts to int.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns></returns>
        public static int[] ConvertToInt(SearchResult[] results)
        {
            if (results == null) return null;

            List<int> list = new List<int>();
            foreach (SearchResult result in results)
                list.Add(result.Id);
            return list.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="referenceId">The reference id.</param>
        /// <param name="ranking">The ranking.</param>
        public SearchResult(short type, int referenceId, int ranking)
        {
            Type = type;
            m_Id = referenceId;
            Ranking = ranking;
        }

        int m_Id;
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }

        int m_Type;
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public int Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        int m_Ranking;
        /// <summary>
        /// Search result ranking
        /// </summary>
        public int Ranking
        {
            get { return m_Ranking; }
            set { m_Ranking = value; }
        }

        string m_LinkText;
        /// <summary>
        /// Linktext of the page that contains this content.
        /// </summary>
        public string LinkText
        {
            get { return m_LinkText; }
            set { m_LinkText = value; }
        }

        string m_Description;
        /// <summary>
        /// Description of the search result page.
        /// </summary>
        public string Description
        {
            get { return m_Description; }
            set { m_Description = value; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <param name="maxLength">Length of the text.</param>
        /// <returns></returns>
        public string GetDescription(int maxLength)
        {
            if (string.IsNullOrEmpty(Description))
                return null;

            if (Description.Length <= maxLength)
                return Description;

            return Wim.Utility.ConvertToFixedLengthText(Description, maxLength, "...");
        }

        DateTime? m_Published;
        /// <summary>
        /// Publication date of the found content.
        /// </summary>
        public DateTime? Published
        {
            get { return m_Published; }
            set { m_Published = value; }
        }
    }
}

