using System;
using System.Collections.Generic;
using System.Text;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Configuring search
    /// </summary>
    public class SearchConfiguration
    {
        /// <summary>
        /// Get all results matching the query using the catalogfilter and return the first 50 results for page 1
        /// having no maximum of pages and only return published items.
        /// </summary>
        /// <param name="type">The type.</param>
        public SearchConfiguration(Sushi.Mediakiwi.Data.SearchType type)
            : this(type, 1, 50, 0, true) { }

        /// <summary>
        /// Get all results matching the query and return only published items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="maxResultSize">Size of the max result.</param>
        public SearchConfiguration(Sushi.Mediakiwi.Data.SearchType type, int currentPage, int maxResultSize)
            : this(type, currentPage, maxResultSize, 0, true) { }

        /// <summary>
        /// Get all results matching the query.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="maxResultSize">Size of the max result.</param>
        /// <param name="maxPageSize">Size of the max page.</param>
        /// <param name="onlyReturnPublished">if set to <c>true</c> [only return published].</param>
        public SearchConfiguration(Sushi.Mediakiwi.Data.SearchType type, int currentPage, int maxResultSize, int maxPageSize, bool onlyReturnPublished)
        {
            SearchType = type;
            CurrentPage = currentPage;
            MaxResultSize = maxResultSize;
            MaxPageSize = maxPageSize;
            OnlyReturnPublished = onlyReturnPublished;
        }

        /// <summary>
        /// Sets the cache key.
        /// </summary>
        /// <param name="searchParameter">The search parameter.</param>
        internal void SetCacheKey(object searchParameter)
        {
            m_CacheKey = string.Format("SearchResult.{0}",
                Wim.Utility.HashStringBySHA1(string.Concat(
                    searchParameter.ToString(),
                    MaxResultSize,
                    MaxPageSize,
                    OnlyReturnPublished)));
        }

        /// <summary>
        /// The site to search in
        /// </summary>
        public int Site;
        /// <summary>
        /// The page to return
        /// </summary>
        public int CurrentPage;
        /// <summary>
        /// The size of each page
        /// </summary>
        public int MaxResultSize;
        /// <summary>
        /// The maximum amount of pages stored
        /// </summary>
        public int MaxPageSize;

        internal int m_Elements;
        /// <summary>
        /// Gets the number of result elements 
        /// </summary>
        public int Elements
        {
            get { return m_Elements; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Sushi.Mediakiwi.Data.SearchType SearchType;

        internal int m_Pages;
        /// <summary>
        /// Gets the number of pages that can be assigned
        /// </summary>
        public int Pages
        {
            get { return m_Pages; }
        }

        /// <summary>
        /// Only return published items
        /// </summary>
        public bool OnlyReturnPublished;
        
        string m_CacheKey;
        /// <summary>
        /// Gets the key used for caching the search result.
        /// </summary>
        public string CacheKey
        {
            get { return m_CacheKey; }
        }

        DateTime m_CacheExpiration;
        /// <summary>
        /// Gets or sets the expiration date for the cache result to be clear. If not applied the default setting is 5 minutes.
        /// </summary>
        public DateTime CacheExpiration
        {
            set { m_CacheExpiration = value; }
            get {
                if (m_CacheExpiration == DateTime.MinValue) m_CacheExpiration = DateTime.Now.AddMinutes(5);
                return m_CacheExpiration; 
            }
        }
    }
}
