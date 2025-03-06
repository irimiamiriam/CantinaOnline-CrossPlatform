using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CantinaOnline.Models;

public class EventModel
{
    public string Name { get; set; } // Event name/title
    public string Description { get; set; } // Event description/details
    public DateTime StartTime { get; set; } // Optional: Event start time
    public DateTime EndTime { get; set; } // Optional: Event end time
}

