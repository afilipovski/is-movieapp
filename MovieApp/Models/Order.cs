using System.ComponentModel.DataAnnotations;

namespace MovieApp.Models
{
	public class Order
	{
        [Key]
        public Guid id { get; set; }
        public virtual EShopApplicationUser User { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
    }
}
