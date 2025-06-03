using System;

namespace VivantioApi.Model
{
    public class TicketAction
    {
        public int Id { get; set; }
        public string ActionUserName { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }
        public string ActionUserEmail { get; set; }
    }
}
