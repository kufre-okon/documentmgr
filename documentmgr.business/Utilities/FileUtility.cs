
using documentmgr.dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace documentmgr.business.Utilities
{
    public class FileUtility : IFileUtility
    {
        private readonly IHostingEnvironment _env;

        public FileUtility(IHostingEnvironment env)
        {
            _env = env;
        }

        public bool IsImage(IFormFile file)
        {
            return ((file != null) && Regex.IsMatch(file.ContentType, "image/\\S+") && (file.Length > 0));
        }

        public bool IsImage(string contentType, long size)
        {
            return ((contentType != null) && Regex.IsMatch(contentType, "image/\\S+") && (size > 0));
        }

        public bool IsOfSize(IFormFile file, double size, bool isSizeInKb = true)
        {
            long iFileSize = file.Length;
            return (iFileSize <= size * (isSizeInKb ? 1024 : 1048576));
        }

        public FileUploadResult UploadFile(IFormFile file, string dirPath, string newFileName = null)
        {
            FileUploadResult fileUploadResult = new FileUploadResult();
            if (file.FileName.Length > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                string filename = newFileName;
                string mapDirPath = MapPath(dirPath);

                if (!CreateDirIfNotExits(mapDirPath))
                    return fileUploadResult;

                if (filename.IsEmpty())
                    filename = Path.GetFileNameWithoutExtension(file.FileName);
                // replace spaces with underscore 
                filename = filename.Replace(" ", "_");
                string filePath = GetUniqueFilePath(mapDirPath, filename, extension);
                SaveFile(file, filePath);
                //strip off root paths 
                filePath = filePath.Replace("\\", "/");
                var regex = new Regex($@"({dirPath}(?:[^\s]+))", RegexOptions.IgnoreCase);
                if (regex.IsMatch(filePath))
                    filePath = regex.Match(filePath).Captures[0].Value;

                fileUploadResult = new FileUploadResult
                {
                    LocalFilePath = filePath,
                    FileName = filename,
                    FileLength = file.Length,
                    Extension = extension,
                    ContentType = file.ContentType
                };
            }
            return fileUploadResult;
        }

        public void DeleteFile(string filepath, bool isAbsolutePath = true)
        {
            if (!isAbsolutePath)
                filepath = MapPath(filepath);
            if (File.Exists(filepath))
                File.Delete(filepath);
        }

        public bool HasExt(IFormFile file, string ext)
        {
            if (file == null || string.IsNullOrWhiteSpace(ext))
                return false;
            var fileExt = Path.GetExtension(file.FileName);
            var splitExts = ext.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

            return splitExts.Any(s => s == fileExt);
        }

        public string MapPath(string path)
        {
            var webRoot = _env.WebRootPath;
            var fullPath = Path.GetFullPath(webRoot + path);
            return fullPath;
        }

        #region private       

        private void SaveFile(IFormFile source, string fileName)
        {
            using (FileStream output = File.Create(fileName))
                source.CopyTo(output);
        }

        private bool CreateDirIfNotExits(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);
                return true;
            }
            catch { return false; }
        }

        private string GetUniqueFilePath(string dirPath, string fileName, string extension)
        {
            string filename = Path.GetFileNameWithoutExtension(fileName);

            string filePath = Path.Combine(dirPath, fileName + extension);
            for (int i = 1; File.Exists(filePath); i++)
            {
                string temp = string.Format("{0}({1}){2}", filename, i, extension);
                filePath = Path.Combine(dirPath, temp);
            }
            return filePath;
        }
        #endregion


    }

    public interface IFileUtility
    {
        void DeleteFile(string filepath, bool isPhysicalPath = true);
        bool IsImage(IFormFile file);
        bool IsImage(string contentType, long size);
        bool IsOfSize(IFormFile file, double size, bool isSizeInKb = true);
        FileUploadResult UploadFile(IFormFile file, string folder, string newFileName = null);
        bool HasExt(IFormFile uploadedFile, string ext);
        string MapPath(string path);
    }

}