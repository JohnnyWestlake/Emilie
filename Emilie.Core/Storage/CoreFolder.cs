using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emilie.Core.Storage
{
    public class CoreFolder : IFolder
    {
        private DirectoryInfo _directory { get; }

        public string Path => _directory.FullName;

        public string Name => _directory.Name;


        public CoreFolder(DirectoryInfo directory)
        {
            _directory = directory;
        }

        public Task<bool> ContainsFileAsync(string fileName)
        {
            return Task.Run(() => File.Exists(System.IO.Path.Combine(Path, fileName)));
        }

        public Task<IFile> CreateOrOpenFileAsync(string fileName)
        {
            return Task.Run<IFile>(() =>
            {
                string path = System.IO.Path.Combine(Path, fileName);
                if (!File.Exists(path))
                {
                    Stream s = File.Open(path, FileMode.OpenOrCreate);
                    s.Dispose();
                }

                return new CoreFile(path);
            });
        }

        public Task<IFolder> CreateOrOpenFolderAsync(string folderName)
        {
            return Task.Run<IFolder>(() =>
            {
                string path = System.IO.Path.Combine(Path, folderName);
                if (!Directory.Exists(path))
                {
                    return new CoreFolder(Directory.CreateDirectory(path));
                }

                return new CoreFolder(new DirectoryInfo(path));
            });
        }

        public Task<IReadOnlyList<IFile>> GetFilesAsync()
        {
            return Task.Run<IReadOnlyList<IFile>>(() =>
            {
                return (from file in _directory.GetFiles()
                        select new CoreFile(file)).ToList();
            });
        }

        public Task<IFile> TryGetFileAsync(string fileName)
        {
            return Task.Run<IFile>(() =>
            {
                string path = System.IO.Path.Combine(Path, fileName);
                if (File.Exists(path))
                    return new CoreFile(path);

                return null;
            });
        }
    }
}
