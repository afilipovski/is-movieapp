using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;
using MovieApp.Models;
using MovieApp.Models.DTO;

namespace MovieApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return View();
            }

            var loggedInUser = await _context.Users
                .OfType<EShopApplicationUser>()
                .Include(e => e.Order)
                .ThenInclude(o => o.Tickets)
                .FirstOrDefaultAsync(e => e.Id == userId);

            double totalPrice = 0.0;

            foreach(TicketInOrder tio in loggedInUser.Order.Tickets)
            {
                var price = _context.TicketOrders
                    .Include(to => to.Ticket)
                    .First(to => to.Id == tio.Id)
                    .Ticket.Price;
                totalPrice += price;
            }

            List<TicketInOrder> ticketInOrders = loggedInUser.Order.Tickets;

            foreach(TicketInOrder tio in ticketInOrders)
            {
                tio.Ticket.Movie = _context.Tickets.Include(t => t.Movie).First(t => t.Id == tio.Ticket.Id).Movie;
            }



            var model = new OrderDTO
            {
                AllTickets = ticketInOrders,
                TotalPrice = totalPrice
            };

            return View(model);
        }

        public async Task<IActionResult> Order()
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null)
			{
				return View();
			}

            var loggedInUser = await _context.Users
                .OfType<EShopApplicationUser>().FirstAsync();

            loggedInUser.Order = new Order();

            return RedirectToAction(nameof(Index));
		}

        // GET: Order/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInOrder = await _context.TicketOrders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketInOrder == null)
            {
                return NotFound();
            }

            return View(ticketInOrder);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create()
        {
			//find all tickets
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId == null)
			{
				return View();
			}

            var loggedInUser = await _context.Users
                .OfType<EShopApplicationUser>()
                .Include(e => e.MyTickets)
                .Include(e => e.Order)
                .Include("MyTickets.Movie")
                .FirstAsync(e => e.Id == userId);

            var withNames = from t in loggedInUser.MyTickets
                            select new
                            {
                                Id = t.Id,
                                Name = t.Movie.MovieName + " " + t.Price
                            };

            ViewBag.TicketId = new SelectList(withNames, "Id", "Name");
            ViewData["OrderId"] = loggedInUser.Order.id;
            return View();

        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketInOrderDTO ticketInOrderDto)
        {
            TicketInOrder tio = new TicketInOrder();
            tio.Id = new Guid();
            tio.Ticket = await _context.Tickets.FirstAsync(t => t.Id == ticketInOrderDto.TicketId);
            tio.Order = await _context.Orders.FirstAsync(o => o.id == ticketInOrderDto.OrderId);
            await _context.TicketOrders.AddAsync(tio);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInOrder = await _context.TicketOrders.FindAsync(id);
            if (ticketInOrder == null)
            {
                return NotFound();
            }
            return View(ticketInOrder);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id")] TicketInOrder ticketInOrder)
        {
            if (id != ticketInOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketInOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketInOrderExists(ticketInOrder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticketInOrder);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketInOrder = await _context.TicketOrders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketInOrder == null)
            {
                return NotFound();
            }

            return View(ticketInOrder);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var ticketInOrder = await _context.TicketOrders.FindAsync(id);
            if (ticketInOrder != null)
            {
                _context.TicketOrders.Remove(ticketInOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketInOrderExists(Guid id)
        {
            return _context.TicketOrders.Any(e => e.Id == id);
        }
    }
}
