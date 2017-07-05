using System;

namespace Kasbah.Web.ContentManagement.ViewModels
{
    public class UpdateUserRequest : CreateUserRequest
    {
        public Guid Id { get; set; }
    }
}
