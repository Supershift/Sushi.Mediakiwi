using System;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    public interface IPageMapping
    {
        /// <summary>
        /// When this is a pagemap to a file, this contains the Asset ID
        /// </summary>
        int AssetID { get; set; }

        /// <summary>
        /// DateTime when this pagemap was created
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// The identifier for this pagemap
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Is this pagemap active 
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// To which page is this pagemap connected
        /// </summary>
        Page Page { get; }

        /// <summary>
        /// The page ID to which this pagemap is connected
        /// </summary>
        int PageID { get; set; }

        /// <summary>
        /// The Path (URL) to act on
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The Query to add to the redirect output URL
        /// </summary>
        string Query { get; set; }

        /// <summary>
        /// The Expression coupled to this pagemap
        /// </summary>
        string Expression { get; set; }

        /// <summary>
        /// The target type for this pagemap
        /// </summary>
        PageMappingTargetType TargetType { get; set; }

        /// <summary>
        /// The target type ID for this pagemap
        /// </summary>
        int TargetTypeID { get; set; }

        /// <summary>
        /// The Browser Title for this pagemap
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// The redirect type for this pagemap
        /// </summary>
        PageMappingType MappingType { get; set; }

        /// <summary>
        /// The redirect type ID for this pagemap
        /// </summary>
        int MappingTypeID { get; set; }

        /// <summary>
        /// The Target URL for this pagemap
        /// </summary>
        string TargetURL { get; }

        void Delete();

        Task DeleteAsync();

        bool Save();

        Task<bool> SaveAsync();
    }
}