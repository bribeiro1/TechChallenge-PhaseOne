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
        var errorList = new List<string>();

        var nameIsEmpty = string.IsNullOrWhiteSpace(vo.Name);
        if (nameIsEmpty)
            errorList.Add("Name shouldn't be empty!");

        var phoneDDDIsInvalid = !StringUtils.ValidatePhoneDDD(vo.PhoneDDD);
        if (phoneDDDIsInvalid)
            errorList.Add("Phone DDD is invalid!");

        var phoneNumberIsInvalid = !vo.PhoneNumber.All(char.IsNumber);
        if (phoneNumberIsInvalid)
            errorList.Add("Phone Number should have only numbers!");

        phoneNumberIsInvalid = !phoneNumberIsInvalid && !StringUtils.ValidatePhoneNumber(vo.PhoneNumber);
        if (phoneNumberIsInvalid)
            errorList.Add("Phone Number is invalid!");

        var emailIsInvalid = !StringUtils.ValidateEmailAddress(vo.EmailAddress);
        if (emailIsInvalid)
            errorList.Add("Email Address is invalid!");

        var nameAlreadyInUse = !nameIsEmpty && Repository.ContactNameAlreadyExists(vo.Name, vo.Id);
        if (nameAlreadyInUse)
            errorList.Add("Name already in use!");

        var phoneAlreadyInUse = !phoneDDDIsInvalid && !phoneNumberIsInvalid && Repository.ContactPhoneAlreadyExists(vo.PhoneDDD, vo.PhoneNumber, vo.Id);
        if (phoneAlreadyInUse)
            errorList.Add("Phone already in use!");

        var emailAlreadyInUse = !emailIsInvalid && Repository.ContactEmailAlreadyExists(vo.EmailAddress, vo.Id);
        if (emailAlreadyInUse)
            errorList.Add("Email already in use!");

        if (errorList.Count > 0)
            throw new ArgumentException(string.Join("\n", errorList));
    }
}
