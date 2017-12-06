using System;

namespace Kasbah.Web.Management.ViewModels
{
    public class UpdateUserRequest : CreateUserRequest
    {
        public Guid Id { get; set; }
    }
}
