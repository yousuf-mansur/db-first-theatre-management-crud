using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace TheatreManagement.Models.ViewModels
{
    public class ArtistViewModel
    {
        public ArtistViewModel()
        {
            this.RoleList = new List<int>();
        }
        public int ArtistId { get; set; }
        [Required, Display(Name = "Artist Name"), StringLength(100)]
        public string ArtistName { get; set; }
        [Required, Display(Name = "Birth Date"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Picture { get; set; }
        [Display(Name = "Picture")]
        public HttpPostedFileBase PictureFile { get; set; }
        public bool Status { get; set; }
        public List<int> RoleList { get; set; }
    }
}