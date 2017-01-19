using System;
using System.IO;
using System.Net.Http;

namespace RapidAPISDK
{
    public class FileParameter : Parameter
    {

        #region  C'tor

        public FileParameter()
        {
        }

        public FileParameter(string key, Stream stream, string fileName) : base(key)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));

            Stream = stream;
            FileName = fileName;
        }

        public FileParameter(string key, string filePath, string fileName = null) : base(key)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not exist or can't be read.");
            Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            FileName = fileName ?? filePath;
        }

        #endregion

        #region Public Properties

        public Stream Stream { get; set; }

        public string FileName { get; set; }

        #endregion

        #region Overrides of Parameter

        public override void AddToContent(MultipartFormDataContent content)
        {
            StreamContent streamContent = new StreamContent(Stream);
            content.Add(streamContent, Key, FileName);
        }

        #endregion
    }
}