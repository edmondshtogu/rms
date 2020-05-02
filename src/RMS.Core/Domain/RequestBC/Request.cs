using System;
using RMS.Core.Extensions;

namespace RMS.Core.Domain.RequestBC
{
    public class Request : BaseEntity
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public Guid StatusId { get; private set; }

        public DateTime RaisedDate { get; private set; }

        public DateTime DueDate { get; private set; }

        public virtual RequestStatus Status { get; private set; }

        public string Attachments { get; private set; }

        public DateTime CreatedOnUtc { get; private set; }
        
        public DateTime UpdatedOnUtc { get; private set; }

        public static Result<Request> Create(Guid id, string name, string description, DateTime raisedDate, DateTime dueDate, Guid statusId, string attachments)
        {
            if (id == Guid.Empty)
            {
                return Result.Fail<Request>(ErrorMessages.InvalidId);
            }
            if (name.IsNullOrEmpty())
            {
                return Result.Fail<Request>(ErrorMessages.NameShouldNotBeEmpty);
            }
            if (statusId == Guid.Empty)
            {
                return Result.Fail<Request>(ErrorMessages.InvalidId);
            }

            var request = new Request
            {
                Id = id,
                Name = name,
                Description = description,
                StatusId = statusId,
                RaisedDate = raisedDate,
                DueDate = dueDate,
                Attachments = attachments,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            return Result.Ok(request);
        }

        public Result UpdateDetails(string name, string description, DateTime raisedDate, DateTime dueDate, Guid statusId, string attachments)
        {
            if (name.IsNullOrEmpty())
            {
                return Result.Fail(ErrorMessages.NameShouldNotBeEmpty);
            }
            if (statusId == Guid.Empty)
            {
                return Result.Fail<Request>(ErrorMessages.InvalidId);
            }
            Name = name;
            Description = description;
            StatusId = statusId;
            RaisedDate = raisedDate;
            DueDate = dueDate;
            Attachments = attachments;
            UpdatedOnUtc = DateTime.UtcNow;

            return Result.Ok();
        }
    }
}
