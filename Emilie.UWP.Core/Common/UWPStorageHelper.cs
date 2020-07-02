using System.Threading.Tasks;
using Emilie.Core.Storage;
using Emilie.UWP.Storage;
using Windows.Storage;

namespace Emilie.UWP.Common
{
    public class UWPStorageHelper : IStorageHelper
    {
        public string GetAppCacheFolderPath()
        {
            return ApplicationData.Current.LocalCacheFolder.Path;
        }

        public string GetAppStorageFolderPath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        public string GetAppTemporaryFolderPath()
        {
            return ApplicationData.Current.TemporaryFolder.Path;
        }

        public async Task<IFile> GetFileFromPathAsync(string path)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(path).ConfigureAwait(false);
            return file != null ? new WinRTFile(file) : null;
        }

        public async Task<IFolder> GetFolderFromPathAsync(string path)
        {
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path).ConfigureAwait();
            return folder != null ? new WinRTFolder(folder) : null;
        }
    }
}
