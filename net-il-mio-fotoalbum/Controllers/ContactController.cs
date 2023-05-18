using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Models;


namespace net_il_mio_fotoalbum.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly PhotoContext _dbContext;

        public ContactController(PhotoContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Post([FromBody] Message message)
        {
            if (ModelState.IsValid)
            {
                using (_dbContext)
                {
                    _dbContext.Messages.Add(message);
                    _dbContext.SaveChanges();
                }
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
