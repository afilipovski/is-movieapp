using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
	public class TicketInOrder
	{
        [Key]
        public Guid Id { get; set; }
        public Ticket Ticket { get; set; }

        public Order Order { get; set; }
    }
}
