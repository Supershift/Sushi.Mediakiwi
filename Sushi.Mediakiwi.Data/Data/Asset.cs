using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.MircoORM;

namespace Sushi.Mediakiwi.Data
{
    [DataMap(typeof(AssetMap))]
    public class Asset
    {
        public Asset()
        {
            this.IsNewStyle = true;
            this.IsActive = true;
        }

        public class AssetMap : DataMap<Asset>
        {
            public AssetMap() : this(false) { }            

            public AssetMap(bool isSave) 
            {
                /* TODO MJ 2019-01-03: Add this sort order, when calling data, like SelectAll()
                Order = "Asset_SortOrder asc, Asset_Title asc")
                */

                if (isSave)
                    Table("wim_Assets");
                else
                    Table("wim_Assets left join wim_GalleryView on Gallery_Key = Asset_Gallery_Key left join wim_AssetTypes on AssetType_Key = Asset_AssetType_Key");
                Id(x => x.ID, "Asset_Key").Identity().SqlType(SqlDbType.Int);
                Map(x => x.GUID, "Asset_GUID").SqlType(SqlDbType.UniqueIdentifier);
                Map(x => x.GalleryID, "Asset_Gallery_Key").SqlType(SqlDbType.Int);
                Map(x => x.Title, "Asset_Title").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.FileName, "Asset_Filename").SqlType(SqlDbType.NVarChar).Length(255);
                Map(x => x.BaseGalleryID, "Gallery_Base_Key").SqlType(SqlDbType.Int).ReadOnly();
                Map(x => x.CompletePath, "Gallery_CompletePath").SqlType(SqlDbType.NVarChar).Length(1000).ReadOnly();
                Map(x => x.Extention, "Asset_Extention").SqlType(SqlDbType.VarChar).Length(20);
                Map(x => x.Size, "Asset_Size").SqlType(SqlDbType.Int);
                Map(x => x.Type, "Asset_Type").SqlType(SqlDbType.VarChar).Length(150);
                Map(x => x.AssetTypeID, "Asset_AssetType_Key").SqlType(SqlDbType.Int);
                Map(x => x.ParentID, "Asset_Asset_Key").SqlType(SqlDbType.Int);
                Map(x => x.Description, "Asset_Description").SqlType(SqlDbType.NText);

                Map(x => x.Created, "Asset_Created").SqlType(SqlDbType.DateTime);
                Map(x => x.IsOldStyle, "Asset_OldStyle").SqlType(SqlDbType.Bit);
                Map(x => x.IsNewStyle, "Asset_NewStyle").SqlType(SqlDbType.Bit);
                Map(x => x.Height, "Asset_Height").SqlType(SqlDbType.Int);
                Map(x => x.SortOrder, "Asset_SortOrder").SqlType(SqlDbType.Int);
                Map(x => x.Width, "Asset_Width").SqlType(SqlDbType.Int);
                Map(x => x.DataString, "Asset_Data").SqlType(SqlDbType.Xml);
                Map(x => x.IsArchived, "Asset_IsArchived");
                Map(x => x.IsImage, "Asset_IsImage").SqlType(SqlDbType.Bit);
                Map(x => x.IsActive, "Asset_IsActive").SqlType(SqlDbType.Bit);
                Map(x => x.Updated, "Asset_Updated").SqlType(SqlDbType.DateTime);


                Map(x => x.RemoteLocation, "Asset_RemoteLocation").Length(512);
                Map(x => x.RemoteLocation_Thumb, "Asset_RemoteLocation_Thumb").Length(512);
                Map(x => x.RemoteDownload, "Asset_RemoteDownload");

                Map(x => x.AssetType, "AssetType_Name").ReadOnly();
                Map(x => x.AssetTypeTag, "AssetType_Tag").ReadOnly();
            }
        }

        #region properties

        public string RemoteLocation { get; set; }
        
        public string RemoteLocation_Thumb { get; set; }

        public bool RemoteDownload { get; set; }

        

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        public Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty)
                    this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Gets or sets the gallery ID.
        /// </summary>
        /// <value>The gallery ID.</value>
        public int GalleryID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        private string m_FileName;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string FileName
        {
            get { return m_FileName; }
            set
            {
                if (this.ID == 0)
                {
                    m_FileName = value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_FileName = m_FileName
                            .Replace("'", string.Empty)
                            .Replace("?", string.Empty);
                        return;
                    }
                }
                m_FileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the base gallery ID.
        /// </summary>
        /// <value>The base gallery ID.</value>
        public int BaseGalleryID { get; set; }

        /// <summary>
        /// Gets or sets the complete path.
        /// </summary>
        /// <value>The complete path.</value>
        public string CompletePath { get; set; }

        /// <summary>
        /// Gets or sets the extention.
        /// </summary>
        /// <value>The extention.</value>
        public string Extention { get; set; }

        public string ExtentionClassName
        {
            get
            {
                if (!string.IsNullOrEmpty(Extention))
                {
                    switch (Extention.ToLower())
                    {
                        case "docx":
                        case "doc": return "doc";
                        case "pdf": return "pdf";
                        case "jpeg":
                        case "jpg":
                        case "png":
                        case "bmp":
                        case "gif": return "image";
                        case "xls":
                        case "xlsx": return "xls";
                        case "ppt":
                        case "pptx": return "ppt";
                        case "zip":
                        case "rar": return "zip";
                        case "vsd": return "vsd";
                        case "eml":
                        case "msg": return "msg";
                        case "txt": return "txt";
                        case "nof": return "nof";
                        default: return "unk";
                    }
                }
                else
                    return "unk";
            }
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the asset type ID.
        /// </summary>
        /// <value>The asset type ID.</value>
        public int? AssetTypeID { get; set; }

        public int? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }        

        /// <summary>
        /// Gets the name of the AssetType.
        /// </summary>
        public string AssetType { get; set; }     

        /// <summary>
        /// Gets the Tag of the AssetType.
        /// </summary>
        public string AssetTypeTag { get; set; }     

        private DateTime m_Created;

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        public DateTime Created
        {
            get
            {
                if (this.m_Created == DateTime.MinValue)
                    this.m_Created = Common.DatabaseDateTime;
                return m_Created;
            }
            set { m_Created = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        public bool IsOldStyle { get; set; }

        /// <summary>
        /// Is this asset archived ?
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [new style].
        /// </summary>
        /// <value><c>true</c> if [new style]; otherwise, <c>false</c>.</value>
        public bool IsNewStyle { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height { get; set; }

        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width { get; set; }

        /// <summary>
        /// Represents the serialized XML value of the CustomData object
        /// </summary>
        public string DataString { get; set; }

        private CustomData m_Data;

        /// <summary>
        /// Holds all customData properties
        /// </summary>
        public CustomData Data
        {
            get
            {
                if (m_Data == null)
                    m_Data = new CustomData(DataString);

                return m_Data;
            }
            set
            {
                m_Data = value;
                DataString = m_Data.Serialized;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is old style.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is old style; otherwise, <c>false</c>.
        /// </value>
        public bool IsImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        Image m_Image;
        /// <summary>
        /// Gets the image instance.
        /// </summary>
        /// <value>The image instance.</value>
        public Image ImageInstance
        {
            get
            {
                if (m_Image == null && this.IsImage)
                {
                    m_Image = new Image();
                    Sushi.Mediakiwi.Data.Utility.ReflectProperty(this, m_Image);
                }
                return m_Image;
            }
        }


        Document m_Document;
        /// <summary>
        /// Gets the document instance.
        /// </summary>
        /// <value>The document instance.</value>
        public Document DocumentInstance
        {
            get
            {
                if (m_Document == null)
                {
                    m_Document = new Document();
                    Sushi.Mediakiwi.Data.Utility.ReflectProperty(this, m_Document);
                }
                return m_Document;
            }
        }

        #region iExportable Members

        public DateTime? Updated { get; set; }

        #endregion iExportable Members

        #endregion properties

        public static List<Asset> SelectAll_Local()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RemoteLocation, null);
            filter.Add(x => x.ParentID, null);

            return connector.FetchAll(filter);
        }

        public static async Task<List<Asset>> SelectAll_LocalAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.RemoteLocation, null);
            filter.Add(x => x.ParentID, null);

            return await connector.FetchAllAsync(filter);
        }

        public static Asset SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            return connector.FetchSingle(ID);
        }

        public static async Task<Asset> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            return await connector.FetchSingleAsync(ID);
        }

        public static Asset SelectOne(int galleryID, int assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.AssetTypeID, assetTypeID);
            filter.Add(x => x.GalleryID, galleryID);
            
            return connector.FetchSingle(filter);
        }

        public static async Task<Asset> SelectOneAsync(int galleryID, int assetTypeID)
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.AssetTypeID, assetTypeID);
            filter.Add(x => x.GalleryID, galleryID);

            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Deletes the inactive.
        /// </summary>
        internal static bool DeleteInactive()
        {
            var connector = ConnectorFactory.CreateConnector<Asset>();
            var filter = connector.CreateDataFilter();
            filter.Add(x => x.IsActive, false);
            connector.Delete(filter);
            return true;
        }

        public bool Save()
        {
            if (string.IsNullOrEmpty(this.Type))
                GuessType();

            Updated = Common.DatabaseDateTime;

            var connector = ConnectorFactory.CreateConnector<Asset>(new AssetMap(true));


            connector.Save(this);
            bool shouldSetSortorder = (this.ID == 0);
            if (!this.SortOrder.HasValue)
            {
                this.SortOrder = this.ID;

                connector.Save(this);
            }

            return true;
        }


        public async Task<bool> SaveAsync()
        {
            if (string.IsNullOrEmpty(this.Type))
                GuessType();

            Updated = Common.DatabaseDateTime;

            var connector = ConnectorFactory.CreateConnector<Asset>(new AssetMap(true));

            await connector.SaveAsync(this);
            bool shouldSetSortorder = (this.ID == 0);
            if (!this.SortOrder.HasValue)
            {
                this.SortOrder = this.ID;

                await connector.SaveAsync(this);
            }

            return true;
        }

        void GuessType()
        {
            this.Type = GuessType(this.Extention);
        }

        /// <summary>
        /// Guesses the type.
        /// </summary>
        /// <param name="extention">The extention.</param>
        /// <returns></returns>
        public static string GuessType(string extention)
        {
            string candidate = null;
            if (MIMETypesDictionary.TryGetValue(extention.ToLower(), out candidate))
                return candidate;
            return "application/octet-stream";
        }

        string ConvertUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                return url;

            //  Remove HTTP: and make it //
            return url.Substring(5, url.Length - 5);
        }


        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>()
  {
    {"ai", "application/postscript"},
    {"aif", "audio/x-aiff"},
    {"aifc", "audio/x-aiff"},
    {"aiff", "audio/x-aiff"},
    {"asc", "text/plain"},
    {"atom", "application/atom+xml"},
    {"au", "audio/basic"},
    {"avi", "video/x-msvideo"},
    {"bcpio", "application/x-bcpio"},
    {"bin", "application/octet-stream"},
    {"bmp", "image/bmp"},
    {"cdf", "application/x-netcdf"},
    {"cgm", "image/cgm"},
    {"class", "application/octet-stream"},
    {"cpio", "application/x-cpio"},
    {"cpt", "application/mac-compactpro"},
    {"csh", "application/x-csh"},
    {"css", "text/css"},
    {"dcr", "application/x-director"},
    {"dif", "video/x-dv"},
    {"dir", "application/x-director"},
    {"djv", "image/vnd.djvu"},
    {"djvu", "image/vnd.djvu"},
    {"dll", "application/octet-stream"},
    {"dmg", "application/octet-stream"},
    {"dms", "application/octet-stream"},
    {"doc", "application/msword"},
    {"docx","application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
    {"dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
    {"docm","application/vnd.ms-word.document.macroEnabled.12"},
    {"dotm","application/vnd.ms-word.template.macroEnabled.12"},
    {"dtd", "application/xml-dtd"},
    {"dv", "video/x-dv"},
    {"dvi", "application/x-dvi"},
    {"dxr", "application/x-director"},
    {"eps", "application/postscript"},
    {"etx", "text/x-setext"},
    {"exe", "application/octet-stream"},
    {"ez", "application/andrew-inset"},
    {"gif", "image/gif"},
    {"gram", "application/srgs"},
    {"grxml", "application/srgs+xml"},
    {"gtar", "application/x-gtar"},
    {"hdf", "application/x-hdf"},
    {"hqx", "application/mac-binhex40"},
    {"htm", "text/html"},
    {"html", "text/html"},
    {"ice", "x-conference/x-cooltalk"},
    {"ico", "image/x-icon"},
    {"ics", "text/calendar"},
    {"ief", "image/ief"},
    {"ifb", "text/calendar"},
    {"iges", "model/iges"},
    {"igs", "model/iges"},
    {"jnlp", "application/x-java-jnlp-file"},
    {"jp2", "image/jp2"},
    {"jpe", "image/jpeg"},
    {"jpeg", "image/jpeg"},
    {"jpg", "image/jpeg"},
    {"js", "application/x-javascript"},
    {"kar", "audio/midi"},
    {"latex", "application/x-latex"},
    {"lha", "application/octet-stream"},
    {"lzh", "application/octet-stream"},
    {"m3u", "audio/x-mpegurl"},
    {"m4a", "audio/mp4a-latm"},
    {"m4b", "audio/mp4a-latm"},
    {"m4p", "audio/mp4a-latm"},
    {"m4u", "video/vnd.mpegurl"},
    {"m4v", "video/x-m4v"},
    {"mac", "image/x-macpaint"},
    {"man", "application/x-troff-man"},
    {"mathml", "application/mathml+xml"},
    {"me", "application/x-troff-me"},
    {"mesh", "model/mesh"},
    {"mid", "audio/midi"},
    {"midi", "audio/midi"},
    {"mif", "application/vnd.mif"},
    {"mov", "video/quicktime"},
    {"movie", "video/x-sgi-movie"},
    {"mp2", "audio/mpeg"},
    {"mp3", "audio/mpeg"},
    {"mp4", "video/mp4"},
    {"mpe", "video/mpeg"},
    {"mpeg", "video/mpeg"},
    {"mpg", "video/mpeg"},
    {"mpga", "audio/mpeg"},
    {"ms", "application/x-troff-ms"},
    {"msh", "model/mesh"},
    {"mxu", "video/vnd.mpegurl"},
    {"nc", "application/x-netcdf"},
    {"oda", "application/oda"},
    {"ogg", "application/ogg"},
    {"pbm", "image/x-portable-bitmap"},
    {"pct", "image/pict"},
    {"pdb", "chemical/x-pdb"},
    {"pdf", "application/pdf"},
    {"pgm", "image/x-portable-graymap"},
    {"pgn", "application/x-chess-pgn"},
    {"pic", "image/pict"},
    {"pict", "image/pict"},
    {"png", "image/png"},
    {"pnm", "image/x-portable-anymap"},
    {"pnt", "image/x-macpaint"},
    {"pntg", "image/x-macpaint"},
    {"ppm", "image/x-portable-pixmap"},
    {"ppt", "application/vnd.ms-powerpoint"},
    {"pptx","application/vnd.openxmlformats-officedocument.presentationml.presentation"},
    {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
    {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
    {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
    {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
    {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
    {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
    {"ps", "application/postscript"},
    {"qt", "video/quicktime"},
    {"qti", "image/x-quicktime"},
    {"qtif", "image/x-quicktime"},
    {"ra", "audio/x-pn-realaudio"},
    {"ram", "audio/x-pn-realaudio"},
    {"ras", "image/x-cmu-raster"},
    {"rdf", "application/rdf+xml"},
    {"rgb", "image/x-rgb"},
    {"rm", "application/vnd.rn-realmedia"},
    {"roff", "application/x-troff"},
    {"rtf", "text/rtf"},
    {"rtx", "text/richtext"},
    {"sgm", "text/sgml"},
    {"sgml", "text/sgml"},
    {"sh", "application/x-sh"},
    {"shar", "application/x-shar"},
    {"silo", "model/mesh"},
    {"sit", "application/x-stuffit"},
    {"skd", "application/x-koan"},
    {"skm", "application/x-koan"},
    {"skp", "application/x-koan"},
    {"skt", "application/x-koan"},
    {"smi", "application/smil"},
    {"smil", "application/smil"},
    {"snd", "audio/basic"},
    {"so", "application/octet-stream"},
    {"spl", "application/x-futuresplash"},
    {"src", "application/x-wais-source"},
    {"sv4cpio", "application/x-sv4cpio"},
    {"sv4crc", "application/x-sv4crc"},
    {"svg", "image/svg+xml"},
    {"swf", "application/x-shockwave-flash"},
    {"t", "application/x-troff"},
    {"tar", "application/x-tar"},
    {"tcl", "application/x-tcl"},
    {"tex", "application/x-tex"},
    {"texi", "application/x-texinfo"},
    {"texinfo", "application/x-texinfo"},
    {"tif", "image/tiff"},
    {"tiff", "image/tiff"},
    {"tr", "application/x-troff"},
    {"tsv", "text/tab-separated-values"},
    {"txt", "text/plain"},
    {"ustar", "application/x-ustar"},
    {"vcd", "application/x-cdlink"},
    {"vrml", "model/vrml"},
    {"vxml", "application/voicexml+xml"},
    {"wav", "audio/x-wav"},
    {"wbmp", "image/vnd.wap.wbmp"},
    {"wbmxl", "application/vnd.wap.wbxml"},
    {"wml", "text/vnd.wap.wml"},
    {"wmlc", "application/vnd.wap.wmlc"},
    {"wmls", "text/vnd.wap.wmlscript"},
    {"wmlsc", "application/vnd.wap.wmlscriptc"},
    {"wrl", "model/vrml"},
    {"xbm", "image/x-xbitmap"},
    {"xht", "application/xhtml+xml"},
    {"xhtml", "application/xhtml+xml"},
    {"xls", "application/vnd.ms-excel"},
    {"xml", "application/xml"},
    {"xpm", "image/x-xpixmap"},
    {"xsl", "application/xml"},
    {"xlsx","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
    {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
    {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
    {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
    {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
    {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
    {"xslt", "application/xslt+xml"},
    {"xul", "application/vnd.mozilla.xul+xml"},
    {"xwd", "image/x-xwindowdump"},
    {"xyz", "chemical/x-xyz"},
    {"zip", "application/zip"}
  };

    }
}