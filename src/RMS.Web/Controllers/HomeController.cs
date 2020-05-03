using RequestsManagementSystem.Internal;
using RequestsManagementSystem.Models;
using RMS.Application.Commands.RequestBC;
using RMS.Application.Queries.RequestBC;
using RMS.Core;
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
        private readonly ITypeAdapter _typeAdapter;
        private readonly IUploadDownloadService _uploadDownloadService;
        public HomeController(IBus bus, ITypeAdapter typeAdapter, IUploadDownloadService uploadDownloadService) : base(bus)
        {
            _typeAdapter = typeAdapter;
            _uploadDownloadService = uploadDownloadService;

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
                var command = _typeAdapter.Adapt<UpdateRequest>(model.AddRequestCommand);
                command.Id = await _bus.ExecuteAsync(model.AddRequestCommand);
                command.Attachments += "; " + await _uploadDownloadService.UploadNewAttachmentAsync(command.Id, model.AttachmentFile);
                command.Attachments = command.Attachments.TrimStart(new[] { ';', ' ' });
                await _bus.ExecuteAsync(command);
                return RedirectToAction("Index");
            }
            model.RequestStatuses = await _bus.QueryAsync(new GetRequestStatuses());
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

            var updateCommand = _typeAdapter.Adapt<UpdateRequest>(request);

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
                var command = model.UpdateRequestCommand;
                command.Attachments += "; " + await _uploadDownloadService.UploadNewAttachmentAsync(command.Id, model.AttachmentFile);
                command.Attachments = command.Attachments.TrimStart(new[] { ';', ' ' });
                await _bus.ExecuteAsync(command);
                return RedirectToAction("Index");
            }
            model.RequestStatuses = await _bus.QueryAsync(new GetRequestStatuses());
            model.SelectedRequestStatus = model.RequestStatuses.Select((s, i) => new { s, i })
                .FirstOrDefault(x => x.s.Id.ToString().Equals(model.UpdateRequestCommand.StatusId.ToString()))?.i + 1 ?? 1;
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

        public async Task<ActionResult> Download(Guid id)
        {
            var request = await _bus.QueryAsync(new GetRequest { Id = id });
            if (request == null)
            {
                return HttpNotFound();
            }

            var (fileContents, contentType, fileName) = await _uploadDownloadService
                .DownloadAttachmentsOwnedByAsync(id);
            return File(fileContents, contentType, fileName);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(Guid id)
        {
            await _bus.ExecuteAsync(new DeleteRequest { Id = id });
            await _uploadDownloadService.CleanAttachmentsOwnedByAsync(id);
            return RedirectToAction("Index");
        }
    }
}