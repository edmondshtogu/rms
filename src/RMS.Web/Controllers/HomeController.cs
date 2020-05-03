using RequestsManagementSystem.Models;
using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using RMS.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> DataHandler(DTParameters requestModel)
        {
            try
            {
                var columnSearch = new List<string>();

                foreach (var col in requestModel.Columns)
                {
                    columnSearch.Add(col.Search.Value);
                }

                var requests = await _bus.QueryAsync(new GetRequests()
                {
                    OrderByString = requestModel.SortOrder,
                    SearchString = requestModel.Search.Value,
                    ColumnFilters = columnSearch.ToArray(),
                    PageIndex = requestModel.Start / requestModel.Length,
                    PageSize = requestModel.Length
                });

                return Json(new DTResult<GetRequestResult>
                {
                    draw = requestModel.Draw,
                    data = requests.Data.ToList(),
                    recordsFiltered = (int)requests.FilteredCount,
                    recordsTotal = (int)requests.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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

        public async Task<ActionResult> Create()
        {
            return View(new CreateRequestViewModel()
            {
                RequestStatuses = await _bus.QueryAsync(new GetRequestStatuses())
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _bus.ExecuteAsync(model.AddRequestCommand);
                return RedirectToAction("Index");
            }
            return View(model);
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

            var statuses = await _bus.QueryAsync(new GetRequestStatuses());

            var updateCommand = new UpdateRequest
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                RaisedDate = request.RaisedDate,
                DueDate = request.DueDate,
                StatusId = request.StatusId,
                StatusName = request.StatusName,
                StatusDescription = request.StatusDescription,
                Attachments = request.Attachments
            };

            var selectedStatus = statuses.Select((s, i) => new { s, i })
                .FirstOrDefault(x => x.s.Id.ToString().Equals(request.StatusId.ToString()))?.i + 1 ?? 1;

            return View(new UpdateRequestViewModel()
            {
                UpdateRequestCommand = updateCommand,
                SelectedRequestStatus = selectedStatus,
                RequestStatuses = statuses
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _bus.ExecuteAsync(model.UpdateRequestCommand);
                return RedirectToAction("Index");
            }
            return View(model);
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