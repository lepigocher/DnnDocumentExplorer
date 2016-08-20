using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using ICSharpCode.SharpZipLib.Zip;

using TidyModules.DocumentExplorer.Common;
using TidyModules.DocumentExplorer.DataTransfertObjects;

namespace TidyModules.DocumentExplorer.Components
{
    public sealed class ServicesController : DnnApiController
    {
        #region  Private Properties

        private bool IsAdmin
        {
            get { return UserInfo.IsSuperUser || UserInfo.IsInRole(PortalSettings.AdministratorRoleName); }
        }

        #endregion

        #region Web Service Methods

        /// <summary>
        /// Get module options.
        /// </summary>
        /// <returns>Options</returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Options()
        {
            try
            {
                DocumentSettings settings = new DocumentSettings(ActiveModule);
                Options options = new Options
                {
                    Resources = GetResources(),
                    Rows = settings.Rows,
                    ShowIcon = settings.ShowIcon,
                    ResetFilters = settings.ResetFilters,
                    Columns = GetColumns(settings),
                    SynchronizeFolder = IsAdmin,
                    FileManagement = settings.FileManagement,
                    OpenOnDblclick = settings.OpenOnDblclick,
                    ImagePreview = settings.ImagePreview
                };

                return Request.CreateResponse(HttpStatusCode.OK, options, GetFormatter());
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Get folders from the specified path.
        /// </summary>
        /// <param name="path">Path of the parent folder</param>
        /// <returns>Folder list or error message</returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Folders([FromUri] ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                IFolderInfo parent = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

                if (parent != null)
                {
                    bool isRoot = string.IsNullOrEmpty(folderPath);
                    IEnumerable<FolderDTO> children = GetChildrenFolder(parent, isRoot);

                    return Request.CreateResponse(HttpStatusCode.OK, children, GetFormatter());
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, folderPath);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Get files from the specified item path.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>File list or error message</returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Files([FromUri] ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                IFolderInfo parent = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

                if (parent != null)
                {
                    IEnumerable<FileDTO> files = FolderManager.Instance.GetFiles(parent).Select(f => new FileDTO
                    {
                        Icon = itemCommand.Flag ? GetFileIcon(f) : null,
                        Name = f.FileName,
                        Extension = f.Extension,
                        Modified = f.LastModifiedOnDate,
                        Size = f.Size
                    });

                    return Request.CreateResponse(HttpStatusCode.OK, files, GetFormatter());
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, folderPath);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Generate the download url of a file.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>Download URL or error</returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage DownloadURL([FromUri] ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                string filePath = folderPath + itemCommand.Files[0];
                IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);
                bool forceDownload = itemCommand.Flag;

                if (file != null)
                {
                    string url = Globals.LinkClick(string.Format("FileID={0}", file.FileId), -1, -1, false, forceDownload);

                    return Request.CreateResponse(HttpStatusCode.OK, url);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, filePath);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Get image thumbnail.
        /// </summary>
        /// <returns>Options</returns>
        [HttpGet]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Thumbnail([FromUri] ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                string filePath = folderPath + itemCommand.Files[0];
                IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);

                if (file != null)
                {
                    DocumentSettings settings = new DocumentSettings(ActiveModule);
                    int height = settings.ThumbnailHeight;
                    int width = settings.ThumbnailWidth;
                    string extension = "." + file.Extension;

                    using (Stream content = FileManager.Instance.GetFileContent(file))
                    {
                        using (Stream thumbnail = ImageUtils.CreateImage(content, height, width, extension))
                        {
                            string img64 = string.Format("data:{0};base64,{1}", file.ContentType, ReadFullyAsBase64(thumbnail));

                            return Request.CreateResponse(HttpStatusCode.OK, img64);
                        }
                    }
                }

                return Request.CreateResponse(HttpStatusCode.NotFound, filePath);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Past the selected files to the destination folder.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage PasteFiles(ItemCommand itemCommand)
        {
            try
            {
                string toPath = PathUtils.Instance.FormatFolderPath(itemCommand.ToPath);
                IFolderInfo destination = FolderManager.Instance.GetFolder(PortalSettings.PortalId, toPath);

                if (destination == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, toPath);

                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                bool moveFiles = itemCommand.Flag;

                foreach (string fileName in itemCommand.Files)
                {
                    string filePath = folderPath + fileName;
                    IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);

                    if (file != null)
                    {
                        if (moveFiles)
                            FileManager.Instance.MoveFile(file, destination);
                        else
                            FileManager.Instance.CopyFile(file, destination);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound, fileName);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Rename the selected item.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Rename(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);

                // Rename folder
                if (itemCommand.Files == null)
                {
                    IFolderInfo folder = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

                    if (folder != null)
                        FolderManager.Instance.RenameFolder(folder, itemCommand.ToPath);
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound, folderPath);
                }
                else // Rename file
                {
                    string fileName = itemCommand.Files[0];
                    string filePath = Path.Combine(folderPath, fileName);
                    IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);

                    if (file != null)
                        FileManager.Instance.RenameFile(file, itemCommand.ToPath);
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound, fileName);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Delete the selected item(s).
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Delete(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);

                if (itemCommand.Files == null)
                {
                    IFolderInfo folder = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

                    if (folder != null)
                    {
                        int fileCounter = FolderManager.Instance.GetFiles(folder).ToList().Count;

                        if (folder.HasChildren || fileCounter > 0)
                            return Request.CreateResponse(HttpStatusCode.Conflict, LocalizeString("FolderNotEmpty"));

                        FolderManager.Instance.DeleteFolder(folder);
                    }
                    else
                        return Request.CreateResponse(HttpStatusCode.NotFound, folderPath);
                }
                else
                {
                    foreach (string fileName in itemCommand.Files)
                    {
                        string filePath = folderPath + fileName;
                        IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);

                        if (file != null)
                            FileManager.Instance.DeleteFile(file);
                        else
                            return Request.CreateResponse(HttpStatusCode.NotFound, fileName);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Pack the selected item(s).
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Pack(ItemCommand itemCommand)
        {
            string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
            IFolderInfo folder = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

            if (folder != null)
            {
                string fileName = itemCommand.ToPath;
                List<string> files = itemCommand.Files;

                if (files.Count > 0 && !string.IsNullOrEmpty(fileName))
                {
                    if (!fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        fileName += ".zip";

                    string error = ZipFiles(files, folder, fileName);

                    if (string.IsNullOrEmpty(error))
                        return Request.CreateResponse(HttpStatusCode.OK);

                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
                }
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Unpack the selected item.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Unpack(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                string filePath = folderPath + itemCommand.Files[0];
                IFileInfo file = FileManager.Instance.GetFile(PortalSettings.PortalId, filePath);

                if (file != null)
                {
                    FileManager.Instance.UnzipFile(file);

                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Create the specified folder.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage CreateFolder(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                string newFolder = folderPath + itemCommand.ToPath;

                FolderManager.Instance.AddFolder(PortalSettings.PortalId, newFolder);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Move the specified folder.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage MoveFolder(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                string toPath = PathUtils.Instance.FormatFolderPath(itemCommand.ToPath);

                IFolderInfo folder = FolderManager.Instance.GetFolder(PortalSettings.PortalId, folderPath);

                if (folder == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, folderPath);

                IFolderInfo destination = FolderManager.Instance.GetFolder(PortalSettings.PortalId, toPath);

                if (destination == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, toPath);

                FolderManager.Instance.MoveFolder(folder, destination);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        /// <summary>
        /// Synchronize the file system starting to the selected item.
        /// </summary>
        /// <param name="itemCommand">ItemCommand object</param>
        /// <returns>OK on success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SupportedModules("TidyModules.DocumentExplorer")]
        [DnnAuthorize]
        public HttpResponseMessage Synchronize(ItemCommand itemCommand)
        {
            try
            {
                string folderPath = PathUtils.Instance.FormatFolderPath(itemCommand.Path);
                bool recursive = itemCommand.Flag;

                FolderManager.Instance.Synchronize(PortalSettings.PortalId, folderPath, recursive, true);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, LocalizeString("UnattentedError"));
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Localize a string specified by his key.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string</returns>
        private string LocalizeString(string key)
        {
            return Localization.GetString(key, Constants.ModuleSharedResources);
        }

        /// <summary>
        /// Create a JSON formatter with default parameters.
        /// </summary>
        /// <returns>JSON Media type formatter</returns>
        private JsonMediaTypeFormatter GetFormatter()
        {
            JsonMediaTypeFormatter formatter = new JsonMediaTypeFormatter();

            formatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return formatter;
        }

        /// <summary>
        /// Create a dictionary of localized resources.
        /// </summary>
        /// <returns>Resources</returns>
        private Dictionary<string, string> GetResources()
        {
            Dictionary<string, string> resources = new Dictionary<string, string>();

            resources.Add("errorTitle", LocalizeString("ErrorTitle"));
            resources.Add("confirmTitle", LocalizeString("ConfirmTitle"));
            resources.Add("createFolderTitle", LocalizeString("CreateFolderTitle"));
            resources.Add("renameFolderTitle", LocalizeString("RenameFolderTitle"));
            resources.Add("renameFileTitle", LocalizeString("RenameFileTitle"));
            resources.Add("packNameTitle", LocalizeString("PackNameTitle"));
            resources.Add("imagePreviewTitle", LocalizeString("ImagePreviewTitle"));
            resources.Add("dialogOk", LocalizeString("DialogOk"));
            resources.Add("dialogCancel", LocalizeString("DialogCancel"));
            resources.Add("dialogClose", LocalizeString("DialogClose"));
            resources.Add("deleteFolderMessage", LocalizeString("DeleteFolderMessage"));
            resources.Add("deleteFilesMessage", LocalizeString("DeleteFilesMessage"));
            resources.Add("itemSelectAll", LocalizeString("ItemSelectAll"));
            resources.Add("itemName", LocalizeString("ItemName"));
            resources.Add("itemCut", LocalizeString("ItemCut"));
            resources.Add("itemCopy", LocalizeString("ItemCopy"));
            resources.Add("itemPaste", LocalizeString("ItemPaste"));
            resources.Add("cantPaste", LocalizeString("CantPaste"));
            resources.Add("itemRename", LocalizeString("ItemRename"));
            resources.Add("itemDelete", LocalizeString("ItemDelete"));
            resources.Add("imagePreview", LocalizeString("ImagePreview"));
            resources.Add("fileOpen", LocalizeString("FileOpen"));
            resources.Add("fileDownload", LocalizeString("FileDownload"));
            resources.Add("fileClipboard", LocalizeString("FileClipboard"));
            resources.Add("clipboardFailed", LocalizeString("ClipboardFailed"));
            resources.Add("filePack", LocalizeString("FilePack"));
            resources.Add("fileUnpack", LocalizeString("FileUnpack"));
            resources.Add("folderCreate", LocalizeString("FolderCreate"));
            resources.Add("folderSynchronize", LocalizeString("FolderSynchronize"));
            resources.Add("folderSynchronizeRecurse", LocalizeString("FolderSynchronizeRecurse"));
            resources.Add("zeroByte", LocalizeString("ZeroByte"));
            resources.Add("sizes", LocalizeString("Sizes"));
            resources.Add("noFiles", LocalizeString("NoFiles"));
            resources.Add("loading", LocalizeString("Loading"));

            return resources;
        }

        /// <summary>
        /// Create the column list from module settings.
        /// </summary>
        /// <param name="settings">Module settings</param>
        /// <returns>Column list</returns>
        private List<Column> GetColumns(DocumentSettings settings)
        {
            List<Column> columns = new List<Column>(3);

            Column name = new Column
            {
                Field = "name",
                HeaderText = LocalizeString("HeaderName"),
                HeaderClass = settings.NameHeaderClass,
                BodyClass = settings.NameBodyClass,
                Filter = settings.NameFilter,
                FilterMatchMode = settings.NameFilterMatchMode,
                Sortable = settings.NameSortable,
                Editor = settings.FileManagement ? "input" : null
            };

            columns.Add(name);

            if (settings.ShowSize)
            {
                Column size = new Column
                {
                    Field = "size",
                    HeaderText = LocalizeString("HeaderSize"),
                    HeaderClass = settings.SizeHeaderClass,
                    BodyClass = settings.SizeBodyClass,
                    Filter = settings.SizeFilter,
                    FilterMatchMode = settings.SizeFilterMatchMode,
                    Sortable = settings.SizeSortable
                };

                columns.Add(size);
            }

            if (settings.ShowDate)
            {
                Column modified = new Column
                {
                    Field = "modified",
                    HeaderText = LocalizeString("HeaderDate"),
                    HeaderClass = settings.DateHeaderClass,
                    BodyClass = settings.DateBodyClass,
                    Filter = settings.DateFilter,
                    FilterMatchMode = settings.DateFilterMatchMode,
                    Sortable = settings.DateSortable
                };

                columns.Add(modified);
            }

            return columns;
        }

        /// <summary>
        /// Get Font Awesome icon from file extension.
        /// </summary>
        /// <param name="file">Current file</param>
        /// <returns>HTLM span with icon class and title</returns>
        private string GetFileIcon(IFileInfo file)
        {
            string fileExtension = file.Extension.ToLower();
            string toggleIcon = "fa-file-o";
            string iconTitle = string.Format(LocalizeString("MiscFile"), fileExtension);

            switch (fileExtension)
            {
                case "bmp":
                case "gif":
                case "jpg":
                case "jpeg":
                case "png":
                case "tif":
                case "tiff":
                case "ico":
                    toggleIcon = "fa-file-image-o";
                    iconTitle = string.Format(LocalizeString("ImageFile"), fileExtension, file.Width, file.Height);
                    break;
                case "avi":
                case "wmv":
                case "mov":
                case "flv":
                case "mpg":
                case "mp4":
                case "vob":
                case "mkv":
                    toggleIcon = "fa-file-video-o";
                    iconTitle = string.Format(LocalizeString("VideoFile"), fileExtension);
                    break;
                case "wav":
                case "wma":
                case "mp3":
                case "m4a":
                case "flac":
                case "aac":
                case "ogg":
                    toggleIcon = "fa-file-audioFile";
                    iconTitle = string.Format(LocalizeString("AudioFile"), fileExtension);
                    break;
                case "doc":
                case "docx":
                    toggleIcon = "fa-file-word-o";
                    iconTitle = string.Format(LocalizeString("WordFile"), fileExtension);
                    break;
                case "xls":
                case "xlsx":
                    toggleIcon = "fa-file-excel-o";
                    iconTitle = string.Format(LocalizeString("ExcelFile"), fileExtension);
                    break;
                case "ppt":
                case "pptx":
                    toggleIcon = "fa-file-powerpoint-o";
                    iconTitle = string.Format(LocalizeString("PowerPointFile"), fileExtension);
                    break;
                case "pdf":
                    toggleIcon = "fa-file-pdf-o";
                    iconTitle = string.Format(LocalizeString("PDFFile"), fileExtension);
                    break;
                case "zip":
                case "gzip":
                case "rar":
                case "tar":
                    toggleIcon = "fa-file-archive-o";
                    iconTitle = string.Format(LocalizeString("ArchiveFile"), fileExtension);
                    break;
                case "txt":
                    toggleIcon = "fa-file-text-o";
                    iconTitle = string.Format(LocalizeString("TextFile"), fileExtension);
                    break;
                default:
                    break;
            }

            return string.Format("<span class='fa fa-lg {0}' title='{1}'></span>", toggleIcon, iconTitle);
        }

        /// <summary>
         /// Get children folders of the specified parent.
         /// </summary>
         /// <param name="parent">Parent folder</param>
         /// <param name="isRoot">True if it's the root folder</param>
         /// <returns>List of folder</returns>
        private IEnumerable<FolderDTO> GetChildrenFolder(IFolderInfo parent, bool isRoot)
        {
            List<FolderDTO> children = null;

            if (parent.HasChildren)
            {
                children = new List<FolderDTO>();
                IEnumerable<IFolderInfo> folders = FolderManager.Instance.GetFolders(parent);

                foreach (IFolderInfo folder in folders)
                {
                    FolderInfo current = folder as FolderInfo;

                    if (FolderPermissionController.CanBrowseFolder(current))
                    {
                        if (current.IsProtected || current.FolderPath == "Cache/")
                            continue;

                        if (current.FolderPath == "Users/" && !IsAdmin)
                            continue;

                        FolderDTO child = new FolderDTO
                        {
                            Label = current.FolderName,
                            Data = new FolderDataDTO { Path = current.FolderPath, CanManage = FolderPermissionController.CanManageFolder(current) },
                            Leaf = !current.HasChildren,
                            Children = current.HasChildren ? new List<FolderDTO>() : null
                        };

                        children.Add(child);
                    }
                }

                if (isRoot)
                {
                    // Add personal folder at the end for not admin users
                    if (!IsAdmin)
                    {
                        DocumentSettings settings = new DocumentSettings(ActiveModule);

                        if (settings.UserFolder)
                        {
                            FolderInfo userFolder = FolderManager.Instance.GetUserFolder(UserInfo) as FolderInfo;

                            FolderDTO userChild = new FolderDTO
                            {
                                Label = LocalizeString("UserFolder"),
                                Data = new FolderDataDTO { Path = userFolder.FolderPath, CanManage = FolderPermissionController.CanManageFolder(userFolder) },
                                Leaf = !userFolder.HasChildren,
                                Children = userFolder.HasChildren ? new List<FolderDTO>() : null
                            };

                            children.Add(userChild);
                        }
                    }

                    List<FolderDTO> rootFolders = new List<FolderDTO>();

                    FolderDTO root = new FolderDTO
                    {
                        Label = LocalizeString("RootFolder"),
                        Data = new FolderDataDTO { Path = "", CanManage = FolderPermissionController.CanManageFolder(parent as FolderInfo) },
                        Leaf = children.Count == 0,
                        Expanded = true,
                        Children = children
                    };

                    rootFolders.Add(root);

                    return rootFolders;
                }
            }

            return children;
        }

        /// <summary>
        /// Zip a list of files.
        /// </summary>
        /// <param name="zipFileList">List of files names/param>
        /// <param name="folder">Folder path</param>
        /// <param name="filename">Zip file name</param>
        /// <returns>Empty string or error</returns>
        private string ZipFiles(IEnumerable<string> zipFileList, IFolderInfo folder, string zipFileName)
        {
            string error = string.Empty;
            byte[] buffer = new byte[4096];
            string folderPhysicalPath = folder.PhysicalPath;

            using (MemoryStream ms = new MemoryStream(4096))
            {
                try
                {
                    using (ZipOutputStream zipOutputStream = new ZipOutputStream(ms))
                    {
                        zipOutputStream.SetLevel(7); //0-9, 9 being the highest level of compression

                        foreach (string fileName in zipFileList)
                        {
                            string filePath = Path.Combine(folderPhysicalPath, fileName);

                            using (Stream fs = File.OpenRead(filePath))
                            {
                                ZipEntry entry = new ZipEntry(ZipEntry.CleanName(fileName, true));
                                entry.Size = fs.Length;
                                zipOutputStream.PutNextEntry(entry);

                                int count = fs.Read(buffer, 0, buffer.Length);
                                while (count > 0)
                                {
                                    zipOutputStream.Write(buffer, 0, count);
                                    count = fs.Read(buffer, 0, buffer.Length);
                                    ms.Flush();
                                }
                                fs.Close();
                            }
                        }

                        zipOutputStream.Finish();
                        FileManager.Instance.AddFile(folder, zipFileName, ms);
                        zipOutputStream.Close();
                    }
                }
                catch (Exception exc)
                {
                    error = exc.Message;
                }
                finally
                {
                    ms.Close();
                }
            }

            return error;
        }

        /// <summary>
        /// Convert content from a stream to a base 64 string.
        /// </summary>
        /// <param name="input">Stream</param>
        /// <returns>Base 64 string</returns>
        private string ReadFullyAsBase64(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.Position = 0;
                input.CopyTo(ms);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        #endregion
    }
}