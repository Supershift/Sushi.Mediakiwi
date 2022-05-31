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

        /// <summary>
        /// Contains a raw JSON object
        /// </summary>
        JSON = 40,
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

        public object PathOrSource { get; set; }

        public bool AppendPath { get; set; }

        public bool LoadAsync { get; set; }

        public string Title { get; set; }

        public bool Equals([AllowNull] AdditionalResourceItem other)
        {
            if (other == null)
            {
                return false;
            }

            return (other.Location.Equals(Location)
                && other.ResourceType.Equals(ResourceType)
                && other.PathOrSource.Equals(PathOrSource)
                && other.AppendPath.Equals(AppendPath)
                && other.LoadAsync.Equals(LoadAsync)
                && other.Title.Equals(Title, StringComparison.InvariantCultureIgnoreCase)
            );
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
        /// <param name="pathOrSource">The relative path to the file, the sourceCode when using HTML, or the JSON object</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string pathOrSource)
        {
            await AddAsync(location, type, pathOrSource, false);
        }

        /// <summary>
        /// Adds a resource file to the page
        /// </summary>
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="pathOrSource">The relative path to the file, the sourceCode when using HTML, or the JSON object</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default: false, only works for JS & CSS)</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string pathOrSource, bool appendApplicationPath)
        {
            await AddAsync(location, type, pathOrSource, appendApplicationPath, false);
        }


        /// <summary>
        /// Adds a resource file to the page
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="pathOrSource">The relative path to the file, the sourceCode when using HTML, or the JSON object</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default false, only works for JS & CSS)</param>
        /// <param name="loadAsync">Should this file be loaded Asynchronous ? (default false, only works for JS & CSS)</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string pathOrSource, bool appendApplicationPath, bool loadAsync)
        {
            await AddAsync(location, type, pathOrSource, appendApplicationPath, loadAsync, false);
        }

        /// <summary>
        /// Adds a resource file to the page
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="pathOrSource">The relative path to the file, the sourceCode when using HTML, or the JSON object</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default false, only works for JS & CSS)</param>
        /// <param name="loadAsync">Should this file be loaded Asynchronous ? (default false, only works for JS & CSS)</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ? this is the case when completely overwriting the page.</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, string pathOrSource, bool appendApplicationPath, bool loadAsync, bool clearBaseTemplateBody)
        {
            await AddAsync(location, type, pathOrSource, appendApplicationPath, loadAsync, clearBaseTemplateBody, "");
        }

        /// <summary>
        /// Adds a resource file to the page
        /// <param name="location">Where must the file be added</param>
        /// <param name="type">The type of file being added</param>
        /// <param name="pathOrSource">The relative path to the file, the sourceCode when using HTML, or the JSON object</param>
        /// <param name="appendApplicationPath">Do we need to append the application path ? (default false, only works for JS & CSS)</param>
        /// <param name="loadAsync">Should this file be loaded Asynchronous ? (default false, only works for JS & CSS)</param>
        /// <param name="clearBaseTemplateBody">Should the existing body HTML be cleared ? this is the case when completely overwriting the page.</param>
        /// <param name="title">The title for the resource</param>
        public async Task AddAsync(ResourceLocation location, ResourceType type, object pathOrSource, bool appendApplicationPath, bool loadAsync, bool clearBaseTemplateBody, string title)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                var newResourceItem = new AdditionalResourceItem()
                {
                    AppendPath = appendApplicationPath,
                    LoadAsync = loadAsync,
                    PathOrSource = pathOrSource,
                    Location = location,
                    ResourceType = type,
                    Title = title
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
                    totalPath = (appendApplicationPath) ? _root.AddApplicationPath(pathOrSource.ToString()) : pathOrSource.ToString();
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
                            totalHtml = pathOrSource.ToString();
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
                    case ResourceType.JSON: 
                        {
                            totalHtml = $"<script type=\"text/javascript\">var {title} = {System.Text.Json.JsonSerializer.Serialize(pathOrSource)};</script>";
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
