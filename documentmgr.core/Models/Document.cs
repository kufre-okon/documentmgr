using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace documentmgr.core.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User USER { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string FilePath { get; set; }

        [StringLength(10)]
        public string Extension { get; set; }

        [StringLength(200)]
        public string ContentType { get; set; }

        /// <summary>
        /// Size in bytes
        /// </summary>
        public long Size { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    }
}
