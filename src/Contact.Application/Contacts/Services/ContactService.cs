using Contacts.Application.Utils;
using Contacts.Domain.Contacts.Models;
using Contacts.Domain.Contacts.Repositories;
using Contacts.Domain.Contacts.Services;
using Contacts.Domain.Contacts.VOs;
using Contacts.Infrastructure.Services;

namespace Contacts.Application.Contacts.Services;

public class ContactService
    : ServiceBase<Contact, IContactRepository>, IContactService
{
    public ContactService(IContactRepository repository)
        : base(repository)
    {
    }

    public IList<ContactVO> List(string ddd)
    {
        return Repository.Query(tracking: false)
            .Where(r => string.IsNullOrEmpty(ddd) || r.Phone.DDD == ddd)
            .Select(ContactVO.Cast).ToList();
    }

    public void Create(ContactVO vo)
    {
        EnsureValidation(vo);

        var entity = ContactVO.Cast(vo);
        Repository.Create(entity);
    }

    public void Update(ContactVO vo)
    {
        EnsureValidation(vo);

        var entity = Repository.GetById(vo.Id);
        var updatedEntity = ContactVO.Cast(vo, entity);

        Repository.Update(updatedEntity);
    }

    public void Delete(Guid id)
    {
        Repository.Delete(id);
    }

    private void EnsureValidation(ContactVO vo)
    {
        string mensagemErro = string.Empty;

        //CHECK IF REQUIRED DATA IS FILLED

        var nameIsEmpty = string.IsNullOrWhiteSpace(vo.Name);
        if (nameIsEmpty)
            mensagemErro = "Name shouldn't be empty! \n";

        var phoneDDDIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneDDD);
        if (phoneDDDIsEmpty)
            mensagemErro += "Phone DDD shouldn't be empty! \n";

        var phoneNumberIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneNumber);
        if (phoneNumberIsEmpty)
            mensagemErro += "Phone Number shouldn't be empty! \n";

        var phoneNumberContainsOnlyNumbers = vo.PhoneNumber.All(r => char.IsNumber(r));
        if (!phoneNumberContainsOnlyNumbers)
            mensagemErro += "Phone Number should have only numbers! \n";

        var emailIsEmpty = string.IsNullOrWhiteSpace(vo.EmailAddress);
        if (emailIsEmpty)
            mensagemErro += "Email Address shouldn't be empty! \n";

        //CHECK IF DATA IS VALID

        var phoneDDDIsInvalid = !StringUtils.ValidatePhoneDDD(vo.PhoneDDD);
        if (phoneDDDIsInvalid)
            mensagemErro += "Phone DDD is invalid! \n";

        var phoneNumberIsInvalid = !StringUtils.ValidatePhoneNumber(vo.PhoneNumber);
        if (phoneNumberIsInvalid)
            mensagemErro += "Phone Number is invalid! \n";

        var emailIsInvalid = !StringUtils.ValidateEmailAddress(vo.EmailAddress);
        if (emailIsInvalid)
            mensagemErro += "Email Address is invalid! \n";

        //CHECK IF DATA IS ALREADY IN USE

        var nameAlreadyInUse = Repository.ContactNameAlreadyExists(vo.Name, vo.Id);
        if (nameAlreadyInUse)
            mensagemErro += "Name already in use! \n";

        var phoneAlreadyInUse = Repository.ContactPhoneAlreadyExists(vo.PhoneDDD, vo.PhoneNumber, vo.Id);
        if (phoneAlreadyInUse)
            mensagemErro += "Phone already in use! \n";

        var emailAlreadyInUse = Repository.ContactEmailAlreadyExists(vo.EmailAddress, vo.Id);
        if (emailAlreadyInUse)
            mensagemErro += "Email already in use! \n";

        if (!string.IsNullOrEmpty(mensagemErro))
            throw new ArgumentException(mensagemErro);
    }
}