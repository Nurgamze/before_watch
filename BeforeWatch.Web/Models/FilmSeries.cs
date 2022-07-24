//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BeforeWatch.Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FilmSeries
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FilmSeries()
        {
            this.Comment = new HashSet<Comment>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string FilmOrSeries { get; set; }
        public int TypeID { get; set; }
        public string Director { get; set; }
        public string Cast { get; set; }
        public string Country { get; set; }
        public string ReleaseYear { get; set; }
        public string Duration { get; set; }
        public string Detail { get; set; }
        public string YoutubeFragman { get; set; }
        public string PosterName { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual Type Type { get; set; }
    }
}