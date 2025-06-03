using Vivantio.Samples.DTO.TicketManagement;

namespace Vivantio.Samples.MvcSamplesCore.ViewModels.TaskBoard
{
    public class TaskBoardViewModel(IEnumerable<TicketType> ticketTypes)
    {
        public IEnumerable<TicketType> TicketTypes { get; private set; } = ticketTypes;
    }
}