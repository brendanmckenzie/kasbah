using System;

namespace Kasbah.Web.ViewModels.Management
{
    public class UpdateUserRequest : CreateUserRequest
    {
        public Guid Id { get; set; }
    }
}
