namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FreezeDownTimer.Models.Interfaces;

    [Table("Role")]
    public partial class Role : iModficationHistory
    {
        public Role()
        {
            Permissions = new HashSet<Permission>();
            Users = new HashSet<User>();
        }

        [Key]
        public int RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsSysAdmin { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
