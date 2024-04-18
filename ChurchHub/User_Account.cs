//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChurchHub
{
    using System;
    using System.Collections.Generic;
    
    public partial class User_Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User_Account()
        {
            this.Booking = new HashSet<Booking>();
            this.Parishioner = new HashSet<Parishioner>();
            this.User_Account1 = new HashSet<User_Account>();
        }
    
        public int id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Nullable<int> AccountStatus { get; set; }
        public string Email { get; set; }
        public Nullable<int> Role { get; set; }
        public string VerCode { get; set; }
        public Nullable<System.DateTime> Date_created { get; set; }
        public Nullable<System.DateTime> Date_modified { get; set; }
        public Nullable<int> CreatedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Booking { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Parishioner> Parishioner { get; set; }
        public virtual Role Role1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User_Account> User_Account1 { get; set; }
        public virtual User_Account User_Account2 { get; set; }
    }
}
