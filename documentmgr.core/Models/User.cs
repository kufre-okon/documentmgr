using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace documentmgr.core.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string MiddleName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        public DateTime DateCreated { get; private set; } = DateTime.UtcNow;

        [StringLength(64)]
        public string TransactionNumber { get; set; }

        public virtual ICollection<Document> DOCUMENTS { get; set; } = new List<Document>();


    }
}
