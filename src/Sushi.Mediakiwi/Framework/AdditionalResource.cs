using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Framework
{
    public enum ResourceType
    {
        /// <summary>
        /// Unknown type, does not render anything
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Adds a javascript file
        /// </summary>
        JAVASCRIPT = 10,

        /// <summary>
        /// Adds a CSS stylesheet file
        /// </summary>
        STYLESHEET = 20,

        /// <summary>
        /// Adds a piece of sourceCode
        /// </summary>
        HTML = 30,
    }

    public enum ResourceLocation
    {
        /// <summary>
        /// Unknown type, does not render anything
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Adds the code to the Header of the page
        /// </summary>
        HEADER = 10,

        /// <summary>
        /// Adds the code below the Body of the page
        /// </summary>
        BODY_BELOW = 20,

        /// <summary>
        /// Adds the code to the Body of the page
        /// </summary>
        BODY_NESTED = 30,
    }

    public class AdditionalResourceItem : IEquatable<AdditionalResourceItem>
    {
        public ResourceLocation Location { get; set; }
        public ResourceType ResourceType { get; set; }

        public string Path { get; set; }

        public bool AppendPath { get; set; }

        public bool LoadAsync { get; set; }

        public bool Equals([AllowNull] AdditionalResourceItem other)
        {
            if (other == null)
            {
                return false;
            }

            return (other.Location == Location && other.ResourceType == ResourceType && other.Path == Path && other.AppendPath == AppendPath && other.LoadAsync == LoadAsync);
        }
    }

    public class AdditionalResource
    {
        private readonly System.Threading.SemaphoreSlim _semaphoreSlim;

        private readonly WimComponentListRoot _root;

        public AdditionalResource(WimComponentListRoot root)
        {
            _root = root;
            _semaphoreSlim = new System.Threading.SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// The collection of added resources
        /// </summary>
        public ICollection<AdditionalResourceItem> Items { get; } = new List<AdditionalResourceItem>();

        /// <summary>
        /// Adds a resource file to the page
        /// </summary>
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="source">The relative path to the file, or the sourceCode when using HTML as type</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string source)
        {
            await AddAsync(location, type, source, false);
        }

        /// <summary>
        /// Adds a resource file to the page
        /// </summary>
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="source">The relative path to the file, or the sourceCode when using HTML as type</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default: false, only works for JS & CSS)</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string source, bool appendApplicationPath)
        {
            await AddAsync(location, type, source, appendApplicationPath, false);
        }


        /// <summary>
        /// Adds a resource file to the page
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="source">The relative path to the file, or the sourceCode when using HTML as type</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default false, only works for JS & CSS)</param>
        /// <param name="loadAsync">Should this file be loaded Asynchronous ? (default false, only works for JS & CSS)</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string source, bool appendApplicationPath, bool loadAsync)
        {
            await AddAsync(location, type, source, appendApplicationPath, loadAsync, false);
        }

        /// <summary>
        /// Adds a resource file to the page
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="source">The relative path to the file, or the sourceCode when using HTML as type</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default false, only works for JS & CSS)</param>
        /// <param name="loadAsync">Should this file be loaded Asynchronous ? (default false, only works for JS & CSS)</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ? this is the case when completely overwriting the page.</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string source, bool appendApplicationPath, bool loadAsync, bool clearBaseTemplateBody)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {

                var newResourceItem = new AdditionalResourceItem()
                {
                    AppendPath = appendApplicationPath,
                    LoadAsync = loadAsync,
                    Path = source,
                    Location = location,
                    ResourceType = type
                };

                // Check if we already have this item, if so, return
                if (Items.Contains(newResourceItem))
                {
                    return;
                }

                Items.Add(newResourceItem);

                // Sets the path to the file
                var totalPath = "";
                if (type == ResourceType.JAVASCRIPT || type == ResourceType.STYLESHEET)
                {
                    totalPath = (appendApplicationPath) ? _root.AddApplicationPath(source) : source;
                    if (string.IsNullOrWhiteSpace(CommonConfiguration.FILE_VERSION) == false)
                    {
                        totalPath += $"?v={CommonConfiguration.FILE_VERSION}";
                    }
                }

                // Sets the HTML output
                var totalHtml = "";
                switch (type)
                {
                    default:
                    case ResourceType.HTML:
                        {
                            totalHtml = source;
                        }
                        break;
                    case ResourceType.JAVASCRIPT:
                        {
                            totalHtml = $"<script type=\"text/javascript\" src=\"{totalPath}\"{(loadAsync ? " async" : "")}></script>";
                        }
                        break;
                    case ResourceType.STYLESHEET:
                        {
                            totalHtml = $"<link rel=\"stylesheet\" href=\"{totalPath}\" type=\"text/css\" media=\"all\"{(loadAsync ? " async" : "")}/>";
                        }
                        break;
                }

                switch (location)
                {
                    default:
                    case ResourceLocation.BODY_NESTED:
                        {
                            _root.Page.Body.AddResource(totalHtml, clearBaseTemplateBody, Body.BodyTarget.Nested);
                        }
                        break;
                    case ResourceLocation.HEADER:
                        {
                            _root.Page.Head.AddResource(totalHtml);
                        }
                        break;
                    case ResourceLocation.BODY_BELOW:
                        {
                            _root.Page.Body.AddResource(totalHtml, clearBaseTemplateBody, Body.BodyTarget.Below);
                        }
                        break;
                }

            }
            finally
            {
                _semaphoreSlim.Release();

            }
        }
    }
}
