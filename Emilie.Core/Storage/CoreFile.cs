using System;
using System.IO;
using System.Threading.Tasks;

namespace Emilie.Core.Storage
{
    /// <summary>
    /// <see cref="IFile"/> implementation utilising <see cref="System.IO.FileInfo"/> API's.
    /// </summary>
    public class CoreFile : IFile
    {
        FileInfo _fileInfo { get; }

        public DateTime DateCreatedUtc  => _fileInfo.CreationTimeUtc;
        public string Path              => _fileInfo.FullName;
        public string Extension         => _fileInfo.Extension;
        public string Name              => _fileInfo.Name;

        public CoreFile(string path) : this(new FileInfo(path)) { }

        public CoreFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        /// <summary>
        /// Opens a file in Read/Write mode
        /// </summary>
        /// <returns></returns>
        public Task<Stream> OpenReadWriteAsync()
        {
            try
            {
                return Task.FromResult<Stream>(_fileInfo.Open(FileMode.OpenOrCreate));
            }
            catch (Exception ex)
            {
                return Task.FromException<Stream>(ex);
            }
        }

        public Task<Stream> OpenReadAsync()
        {
            try
            {
                return Task.FromResult<Stream>(_fileInfo.OpenRead());
            }
            catch (Exception ex)
            {
                return Task.FromException<Stream>(ex);
            }
        }

        public Task<Stream> OpenWriteAsync()
        {
            try
            {
                return Task.FromResult<Stream>(_fileInfo.OpenWrite());
            }
            catch (Exception ex)
            {
                return Task.FromException<Stream>(ex);
            }
        }

        public Task<string> ReadTextAsync()
        {
            return Task.Run(() => File.ReadAllText(_fileInfo.FullName));
        }

        public Task WriteTextAsync(string text)
        {
            return Task.Run(() => File.WriteAllText(_fileInfo.FullName, text));
        }

        public Task DeleteAsync()
        {
            return Task.Run(() => _fileInfo.Delete());
        }

        /// <summary>
        /// Update the last modified time of the file to the current time
        /// </summary>
        /// <returns></returns>
        public Task UpdateLastModifiedAsync()
        {
            return Task.Run(() => _fileInfo.LastWriteTimeUtc = DateTime.UtcNow);
        }
    }
}