using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
	public class Order
	{
        [Key]
        public Guid id { get; set; }

        [ForeignKey("UserId")]
        public virtual EShopApplicationUser User { get; set; }
        public virtual List<TicketInOrder> Tickets { get; set; }
    }
}
