using System;
using System.IO;
using System.Threading.Tasks;
using Emilie.Core.Storage;
using Emilie.UWP.Extensions;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Emilie.UWP.Storage
{
    public class WinRTFile : IFile
    {
        private IStorageFile _file { get; }

        public DateTime DateCreatedUtc { get; }

        public string Path => _file.Path;

        public string Extension => _file.FileType;

        public string Name => _file.Name;

        public WinRTFile(IStorageFile file)
        {
            _file = file;
            DateCreatedUtc = file.DateCreated.ToUniversalTime().DateTime;
        }

        public async Task<Stream> OpenReadWriteAsync()
        {
            var s = await _file.OpenAsync(FileAccessMode.ReadWrite).ConfigureAwait(false);
            return s.AsStream();
        }

        public Task<Stream> OpenReadAsync()
        {
            return _file.OpenStreamForReadAsync();
        }

        public Task<Stream> OpenWriteAsync()
        {
            return _file.OpenStreamForWriteAsync();
        }

        public Task<string> ReadTextAsync()
        {
            return FileIO.ReadTextAsync(_file).AsTask();
        }

        public Task WriteTextAsync(string text)
        {
            return FileIO.WriteTextAsync(_file, text).AsTask();
        }

        public Task DeleteAsync()
        {
            return _file.DeleteAsync(StorageDeleteOption.PermanentDelete).AsTask();
        }

        public async Task UpdateLastModifiedAsync()
        {
            // We actually have to "pretend" to write data to the file to get windows
            // to co-operate and actually let us 
            using (Stream stream = await OpenWriteAsync().ConfigureAwait(false))
            {
                stream.Flush();
            }
        }
    }
}