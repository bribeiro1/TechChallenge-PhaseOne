using Contacts.Domain.Contacts.Dto;

namespace Contacts.Application.Services.Contacts
{
    public interface IContactService
    {
        IList<ContactDto> List(string ddd);
        void Create(ContactDto vo);
        void Update(ContactDto vo);
        void Delete(Guid id);
    }
}