using System.Threading.Tasks;
using Emilie.Core.Storage;

namespace Emilie.Core.Storage
{
    public interface IStorageHelper
    {
        /// <summary>
        /// Returns a path to a storage folder for an application.
        /// This folder may be backed up to the cloud.
        /// </summary>
        /// <returns></returns>
        string GetAppStorageFolderPath();

        /// <summary>
        /// Returns a path to a storage folder for an application.
        /// This folder is not backed up or synced to online storage.
        /// </summary>
        /// <returns></returns>
        string GetAppCacheFolderPath();

        /// <summary>
        /// Returns a folder path to a temporary folder.
        /// This folder is may be deleted or cleaned by the system at any point.
        /// </summary>
        /// <returns></returns>
        string GetAppTemporaryFolderPath();

        Task<IFolder> GetFolderFromPathAsync(string path);

        Task<IFile> GetFileFromPathAsync(string path);
    }
}
