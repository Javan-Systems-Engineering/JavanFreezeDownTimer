namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using FreezeDownTimer.Models.Interfaces;

    [Table("Permission")]
    public partial class Permission : iModficationHistory
    {
        public Permission()
        {
            Roles = new HashSet<Role>();
        }

        [Key]
        public int PermissionID { get; set; }

        [Required]
        [StringLength(50)]
        public string PermissionName { get; set; }

        [StringLength(50)]
        public string PermissionDescription { get; set; }

        public DateTime InsertDate { get; set; }


        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
