using System.ComponentModel.DataAnnotations;

namespace EventManagerWeb.Models
{
        public class Association
        {
            [Key]
            public string Id { get; set; }
            public string UserId { get; set; }
            public string EventId { get; set; }
            public User User { get; set; }
            public Event Event { get; set; }
        }
}
