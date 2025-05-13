namespace GyanGanga.Web.Models.Classes
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; }
    }
}