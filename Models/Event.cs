using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace EventManagerWeb.Models
{
    public class Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        [Display(Name = "Title")]
        public string EventTitle { get; set; }

        [Required(ErrorMessage = "Address is required!")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Time is required!")]
        [DataType(DataType.Time)]
        public DateTime EventTime { get; set; }

        [Required(ErrorMessage = "Date is required!")]
        [Display(Name = "Date")]
        [DateInTheFuture]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Duration is required!")]
        public int Duration { get; set; }
        [Required]

        [Display(Name = " ")]
        public string TimeType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public string? UserId { get; set; }
        public string? CreatorName { get; set; }

        //nav
        public List<Association>? Guests { get; set; }
    }
}
