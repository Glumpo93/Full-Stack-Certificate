namespace TaskManagerApp.Models
{
    public class EventModel
    {
        public int Id { get; set; } // Add this property
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}