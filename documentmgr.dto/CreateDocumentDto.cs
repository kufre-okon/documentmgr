using System;

namespace documentmgr.dto
{
    public class CreateDocumentDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }
        public string Title { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public string LocalFilePath { get; set; }
        public string ContentType { get; set; }
    }
}
