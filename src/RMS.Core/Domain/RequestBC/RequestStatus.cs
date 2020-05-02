using System;
using System.Collections.Generic;
using RMS.Core.Extensions;

namespace RMS.Core.Domain.RequestBC
{
    public class RequestStatus : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public virtual ICollection<Request> Requests { get; set; }

        public static Result<RequestStatus> Create(Guid id, string name, string description)
        {
            if (id == Guid.Empty)
            {
                return Result.Fail<RequestStatus>(ErrorMessages.InvalidId);
            }
            if (name.IsNullOrEmpty())
            {
                return Result.Fail<RequestStatus>(ErrorMessages.NameShouldNotBeEmpty);
            }
            var request = new RequestStatus
            {
                Id = id,
                Name = name,
                Description = description
            };
            return Result.Ok(request);
        }

        public Result UpdateDetails(string name, string description)
        {
            if (name.IsNullOrEmpty())
            {
                return Result.Fail(ErrorMessages.NameShouldNotBeEmpty);
            }
            Name = name;
            Description = description;

            return Result.Ok();
        }
    }
}
