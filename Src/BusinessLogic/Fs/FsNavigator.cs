using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BusinessLogic.Fs
{
    public class FsNavigator
    {
        private string _currentFolder;
        private readonly string _rootFolder;
        private string _name;

        public FsNavigator(string rootFolder)
        {
            if (Directory.Exists(rootFolder) == false)
                throw new DirectoryNotFoundException();
            _rootFolder = rootFolder;
            Name = _rootFolder;
            CurrentFolder = _rootFolder;            
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                if (Segments != null && Segments.Count > 0)
                    Segments[0].Name = _name;
            }
        }

        public string RootFolder
        {
            get { return _rootFolder; }
        }

        public string CurrentFolder
        {
            get { return _currentFolder; }
            set
            {
                if (value == null || value.StartsWith(_rootFolder) == false)
                    throw new ArgumentException();
                if (value == _currentFolder)
                    return;
                _currentFolder = value;
                UpdateSegments();
            }
        }

        /// <summary>
        /// Current folder path parts
        /// </summary>
        public IList<FsItem> Segments { get; private set; }

        protected virtual FsItem GetNewSegment()
        {
            return new FsItem();
        }

        /// <summary>
        /// Splits current folder path in segments. Each segment can be used to navigate to folder
        /// </summary>
        public virtual void UpdateSegments()
        {
            // root segment 
            var root = GetNewSegment();
            root.Name = Name.Trim(Path.DirectorySeparatorChar);
            root.FullPath = RootFolder;

            var s = new List<FsItem> { root };

            // get folders names from path
            var dirs = CurrentFolder.Replace(RootFolder, "").Split(new char[] {Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

            // create segment for each folder
            string path = RootFolder;
            foreach (string dir in dirs)
            {
                path = Path.Combine(path, dir);
                
                var item = GetNewSegment();
                item.Name = dir;
                item.FullPath = path;                
                s.Add(item);
            }
            Segments = s;
        }

        /// <summary>
        /// Gets all files from the current folder
        /// </summary>        
        public virtual IList<FsItem> GetFiles()
        {
            if (Directory.Exists(CurrentFolder))
                return new DirectoryInfo(CurrentFolder).EnumerateFiles()
                    .Where(f => false == f.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(f => new FsItem { Name = f.Name, Length = f.Length, FullPath = f.FullName, IsDirectory = false })
                    .ToList();
            return new List<FsItem>();
        }

        /// <summary>
        /// Gets all subfolders from the current folder
        /// </summary>
        public virtual IList<FsItem> GetFolders()
        {
            if (Directory.Exists(CurrentFolder))
                return new DirectoryInfo(CurrentFolder).EnumerateDirectories()
                    .Where(d => false == d.Attributes.HasFlag(FileAttributes.Hidden))
                    .Select(d => new FsItem { Name = d.Name, FullPath = d.FullName + Path.DirectorySeparatorChar })
                    .ToList();
            return new List<FsItem>();
        }        

        public bool CanNavigateToParentFolder
        {
            get { return _currentFolder != _rootFolder; }
        }

        public string GetParentFolder()
        {
            return CanNavigateToParentFolder
                ? Path.GetDirectoryName(_currentFolder.TrimEnd(Path.DirectorySeparatorChar))
                : _currentFolder;
        }

        /// <summary>
        /// Performs operation with an item (file or folder)
        /// </summary>
        /// <param name="item"></param>        
        public virtual bool PickItem(FsItem item)
        {
            if (item == null)
                return false;
            
            if (item.IsDirectory)
            {
                string path = null;
                if (item.Name == FsItem.GotoParent.Name)
                {
                    // return to parent
                    if (CanNavigateToParentFolder)
                        path = GetParentFolder();
                }
                else                
                    path = item.FullPath;
                
                // open folder
                if (path != null)
                {
                    CurrentFolder = path;
                    return true;
                }
            }
            else
            {
                // open file
                Process.Start(item.FullPath);
            }
            return false;
        }
    }
}
