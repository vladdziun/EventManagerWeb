namespace EventManagerWeb.Models
{
    public class FileUpload
    {
        public IFormFile FileToUpload { get; set; }

        public Event Event { get; set; }
    }
}
