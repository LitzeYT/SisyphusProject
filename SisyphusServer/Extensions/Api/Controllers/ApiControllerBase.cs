using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SisyphusServer.Extensions.Api.Controllers {
    public class ApiControllerBase : ControllerBase {
        private ISender? _Sender = null;
        protected ISender Sender => _Sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    }
}
