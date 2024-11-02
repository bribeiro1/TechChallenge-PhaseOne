using Contacts.Domain.Contacts.VOs;

namespace Contacts.Domain.Contacts.Services
{
    public interface IContactService
    {
        IList<ContactVO> List(string ddd);
        void Create(ContactVO vo);
        void Update(ContactVO vo);
        void Delete(Guid id);
    }
}
