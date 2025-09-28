using Microsoft.AspNetCore.Components;

namespace TaskManagerApp.Components
{
    public partial class EventCard
    {
        [Parameter] public string EventName { get; set; } = string.Empty;
        [Parameter] public DateTime EventDate { get; set; }
        [Parameter] public string EventLocation { get; set; } = string.Empty;
    }
}