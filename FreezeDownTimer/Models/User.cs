namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FreezeDownTimer.Models.Interfaces;

    [Table("User")]
    public partial class User : iModficationHistory
    {
        public User()
        {
            Roles = new HashSet<Role>();
        }

        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required]
        [StringLength(30)]
        public string Password { get; set; }

        public bool? Inactive { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        //[StringLength(30)]
        //public string Title { get; set; }

        //[StringLength(3)]
        //public string Initial { get; set; }

        [StringLength(100)]
        public string EMail { get; set; }

        public DateTime InsertDate { get; set; }



        public DateTime? LastUpdate { get; set; }

        public int? LastUpdateBy { get; set; }

        public bool IsDirty { get; set; }


        public virtual ICollection<Role> Roles { get; set; }
    }
}
