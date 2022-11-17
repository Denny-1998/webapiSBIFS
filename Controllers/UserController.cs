using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapiSBIFS.Model;

namespace webapiSBIFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        
    }
}
