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
    
    public partial class Booking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Booking()
        {
            this.Payment1 = new HashSet<Payment>();
        }
    
        public int BookingID { get; set; }
        public int ScheduleID { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ParishionerID { get; set; }
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        public string Booking_Status { get; set; }
        public Nullable<decimal> Payment { get; set; }
    
        public virtual Schedule Schedule { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payment1 { get; set; }
        public virtual User_Account User_Account { get; set; }
    }
}