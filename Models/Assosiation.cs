using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventManagerWeb.Models
{
        public class Association
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            [Key]
            public string Id { get; set; }
            public string UserId { get; set; }
            public string EventId { get; set; }
            public User User { get; set; }
            public Event Event { get; set; }
        }
}
