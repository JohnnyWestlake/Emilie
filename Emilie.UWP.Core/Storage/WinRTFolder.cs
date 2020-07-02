using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emilie.Core.Storage;
using Emilie.UWP.Extensions;
using Windows.Storage;

namespace Emilie.UWP.Storage
{
    public class WinRTFolder : IFolder
    {
        private StorageFolder _folder { get; }

        public string Path => _folder.Path;

        public string Name => _folder.Name;


        public WinRTFolder(StorageFolder folder)
        {
            _folder = folder;
        }

        public async Task<bool> ContainsFileAsync(string fileName)
        {
            return await _folder.TryGetFileAsync(fileName).ConfigureAwait(false) != null;
        }

        public async Task<IFolder> CreateOrOpenFolderAsync(string folderName)
        {
            StorageFolder folder = await _folder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists).ConfigureAwait();
            return folder != null ? new WinRTFolder(folder) : null;
        }

        public async Task<IFile> TryGetFileAsync(string fileName)
        {
            IStorageFile file = await _folder.TryGetFileAsync(fileName).ConfigureAwait(false);
            return file != null ? new WinRTFile(file) : null;
        }

        public async Task<IFile> CreateOrOpenFileAsync(string fileName)
        {
            StorageFile file = await _folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists).ConfigureAwait();
            return file != null ? new WinRTFile(file) : null;
        }

        public async Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            return (from file in await _folder.GetFilesAsync().ConfigureAwait(false)
                    select new WinRTFile(file)).ToList();
        }
    }
}
