using System;

namespace documentmgr.dto
{
    public class FileUploadResult
    {
        public string LocalFilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
    }
}
