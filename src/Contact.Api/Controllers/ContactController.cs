using Contacts.Application.Services.Contacts;
using Contacts.Domain.Contacts.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            this._contactService = contactService;
        }

        [HttpGet]
        public ActionResult<IList<ContactDto>> List([FromQuery] string ddd = null)
        {
            try
            {
                var result = _contactService.List(ddd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost()]
        public IActionResult Create(ContactDto contact)
        {
            try
            {
                _contactService.Create(contact);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut()]
        public IActionResult Update(ContactDto contact)
        {
            try
            {
                _contactService.Update(contact);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _contactService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
