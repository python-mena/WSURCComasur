//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSURCComasur.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ROLES
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ROLES()
        {
            this.PERMISSIONS = new HashSet<PERMISSIONS>();
            this.USERS = new HashSet<USERS>();
        }
    
        public int Role_Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public Nullable<bool> IsSysAdmin { get; set; }
        public System.DateTime LastModified { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERMISSIONS> PERMISSIONS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USERS> USERS { get; set; }
    }
}
