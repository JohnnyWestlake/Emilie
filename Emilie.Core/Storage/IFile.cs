using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Emilie.Core.Storage
{
    public interface IFile
    {
        /// <summary>
        /// The full path to the file
        /// </summary>
        string Path { get; }

        /// <summary>
        /// File extension of file
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Name of the file including extension if there is one
        /// </summary>
        string Name { get; }

        DateTime DateCreatedUtc { get; }

        Task<Stream> OpenReadAsync();
        Task<Stream> OpenWriteAsync();

        /// <summary>
        /// Open the File's stream in Read/Write mode.
        /// </summary>
        /// <returns></returns>
        Task<Stream> OpenReadWriteAsync();

        Task<string> ReadTextAsync();

        Task WriteTextAsync(string text);

        Task DeleteAsync();

        Task UpdateLastModifiedAsync();
    }

    public interface IFolder
    {
        /// <summary>
        /// The full path to the Folder, it if has one
        /// </summary>
        string Path { get; }

        /// <summary>
        /// The name of the Folder, if it has one
        /// </summary>
        string Name { get; }

        Task<bool> ContainsFileAsync(string fileName);
        Task<IFolder> CreateOrOpenFolderAsync(string folderName);
        Task<IFile> CreateOrOpenFileAsync(string fileName);
        Task<IFile> TryGetFileAsync(string fileName);
        Task<IReadOnlyList<IFile>> GetFilesAsync();
    }
}
