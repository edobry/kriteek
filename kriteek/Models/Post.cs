//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace kriteek.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Post
    {
        public Post()
        {
            this.Ratings = new HashSet<Rating>();
            this.Topics = new HashSet<Topic>();
            this.VisibleTo = new HashSet<Friendtype>();
        }
    
        public int ID { get; set; }
        public int Type { get; set; }
        public string Text { get; set; }
        public int PosterID { get; set; }
        public System.DateTime Time { get; set; }
    
        public virtual Person Poster { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Friendtype> VisibleTo { get; set; }
    }
}
