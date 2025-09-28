using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApp.Models;

namespace TaskManagerApp.Services
{
    public class EventService
    {
        private readonly List<EventModel> events = new()
        {
            new EventModel { Id = 1, Name = "Blazor Workshop", Date = DateTime.Now.AddDays(1), Location = "Online" },
            new EventModel { Id = 2, Name = "C# Conference", Date = DateTime.Now.AddDays(10), Location = "New York" }
        };

        public Task<List<EventModel>> GetEventsAsync()
        {
            // Simulate asynchronous data fetching
            return Task.FromResult(events);
        }

        public Task<EventModel?> GetEventByIdAsync(int id)
        {
            var eventItem = events.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(eventItem);
        }
    }
}