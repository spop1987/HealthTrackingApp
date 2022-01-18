using Microsoft.AspNetCore.Mvc;
using MyHealthNotebook.DataService.IConfiguration;

namespace MyHealthNotebook.Api.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class BaseController : ControllerBase
    {
        public readonly IUnitOfWork _unitOfWork;
        public BaseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}