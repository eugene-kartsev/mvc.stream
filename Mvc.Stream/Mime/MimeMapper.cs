using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mvc.Stream.Mime
{
    /// <summary>
    /// Default MIME mapper which allows to get standard content type based on file extension.
    /// </summary>
    public class MimeMapper : IMimeMapper
    {
        /// <summary>
        /// Default Mime - it'll be used if no any other mapping is found
        /// </summary>
        private const string DefaultMime = "application/octet-stream";

        /// <summary>
        /// Default pattern of searching file extensions in file path
        /// </summary>
        private readonly Regex _pathExtensionPattern = new Regex("\\.(\\w*)$");

        /// <summary>
        /// Default dictionary of extensions and Mime types (Content types)
        /// </summary>
        private static Dictionary<string, string> _items;

        public MimeMapper() : this(null) {}
        public MimeMapper(params MimeMappingItem[] extensions)
        {
            _items = new Dictionary<string, string>();
            foreach (var mapping in DefaultMimeItems.Items)
            {
                _items.Add(mapping.Extension, mapping.MimeType);
            }
            Extend(extensions);
        }

        /// <summary>
        /// The method allows to extend standard list of mime mapping rules.
        /// Extension has higher priority - this means that standard mime will be overriden if extension has the same extension as standard item
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public IMimeMapper Extend(params MimeMappingItem[] extensions)
        {
            if(extensions != null)
            {
                foreach (var mapping in extensions)
                {
                    if (_items.ContainsKey(mapping.Extension))
                    {
                        _items[mapping.Extension] = mapping.MimeType;
                    }
                    else
                    {
                        _items.Add(mapping.Extension, mapping.MimeType);
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// The method returns content type for certain file extension or Default one if no any correspondence is found
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public string GetMimeFromExtension(string fileExtension)
        {
            fileExtension = (fileExtension ?? string.Empty).ToLower();
            fileExtension = fileExtension.Trim().StartsWith(".")
                            ? fileExtension.Replace(".", "")
                            : fileExtension;

            return _items.ContainsKey(fileExtension)
                       ? _items[fileExtension]
                       : DefaultMime;
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetMimeFromPath(string path)
        {
            var extension = GetExtension(path);
            return GetMimeFromExtension(extension);
        }

        protected string GetExtension(string path)
        {
            var match = _pathExtensionPattern.Match(path ?? string.Empty);
            if (match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }
            return null;
        }
    }
}