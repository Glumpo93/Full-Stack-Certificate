using Microsoft.AspNetCore.Components;

namespace TaskManagerApp.Components
{
    public partial class EventCard
    {
        [Parameter] public string EventName { get; set; } = "Unknown Event";
        [Parameter] public DateTime EventDate { get; set; } = DateTime.MinValue;
        [Parameter] public string EventLocation { get; set; } = "Unknown Location";

        protected override void OnParametersSet()
        {
            // Ensure EventName and EventLocation are not null or empty
            EventName = string.IsNullOrWhiteSpace(EventName) ? "Unknown Event" : EventName;
            EventLocation = string.IsNullOrWhiteSpace(EventLocation) ? "Unknown Location" : EventLocation;

            // Ensure EventDate is valid
            if (EventDate == DateTime.MinValue)
            {
                EventDate = DateTime.Now; // Default to current date
            }
        }
    }
}