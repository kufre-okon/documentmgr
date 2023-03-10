using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace documentmgr.dto
{
    public class FileUploadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
    }

    public class FileSearchDto : FileUploadDto
    {
        public bool IsImage { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
    }
}
