using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using RMS.Messages;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RequestsManagementSystem.Controllers
{
    public class HomeController : AppController
    {
        public HomeController(IBus bus) : base(bus)
        {
        }

        public async Task<ActionResult> Index()
        {
            var requests = await _bus.QueryAsync(new GetRequests()
            {
                PageIndex = 0,
                PageSize = 20
            });

            return View(requests);
        }

        public async Task<ActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var request = await _bus.QueryAsync(new GetRequest()
            {
                Id = id
            });

            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Todos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AddRequest command)
        {
            if (ModelState.IsValid)
            {
                await _bus.ExecuteAsync(command);
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<ActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = await _bus.QueryAsync(new GetRequest { Id = id });
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateRequest command)
        {
            if (ModelState.IsValid)
            {
                await _bus.ExecuteAsync(command);
                return RedirectToAction("Index");
            }
            return View(command);
        }

        public async Task<ActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var request = await _bus.QueryAsync(new GetRequest { Id = id });
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await _bus.ExecuteAsync(new DeleteRequest { Id = id });
            return RedirectToAction("Index");
        }
    }
}