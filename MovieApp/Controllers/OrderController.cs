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
                .Include(e => e.MyTickets)
                .FirstOrDefaultAsync(e => e.Id == userId);

            var model = new OrderDTO
            {
                AllTickets = loggedInUser.Order.Tickets,
                TotalPrice = loggedInUser.Order.Tickets.Sum(t => t.Ticket.Price)
            };

            return View(model);
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] TicketInOrder ticketInOrder)
        {
            if (ModelState.IsValid)
            {
                ticketInOrder.Id = Guid.NewGuid();
                _context.Add(ticketInOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticketInOrder);
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
