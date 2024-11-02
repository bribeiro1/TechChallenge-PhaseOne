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
        //CHECK IF REQUIRED DATA IS FILLED

        var nameIsEmpty = string.IsNullOrWhiteSpace(vo.Name);
        if (nameIsEmpty)
            throw new ArgumentException("Name shouldn't be empty!");

        var phoneDDDIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneDDD);
        if (phoneDDDIsEmpty)
            throw new ArgumentException("Phone DDD shouldn't be empty!");

        var phoneNumberIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneNumber);
        if (phoneNumberIsEmpty)
            throw new ArgumentException("Phone Number shouldn't be empty!");

        var phoneNumberContainsOnlyNumbers = vo.PhoneNumber.All(r => char.IsNumber(r));
        if (!phoneNumberContainsOnlyNumbers)
            throw new ArgumentException("Phone Number should have only numbers!");

        var emailIsEmpty = string.IsNullOrWhiteSpace(vo.EmailAddress);
        if (emailIsEmpty)
            throw new ArgumentException("Email Address shouldn't be empty!");

        //CHECK IF DATA IS VALID

        var phoneDDDIsInvalid = !StringUtils.ValidatePhoneDDD(vo.PhoneDDD);
        if (phoneDDDIsInvalid)
            throw new ArgumentException("Phone DDD is invalid!");

        var phoneNumberIsInvalid = !StringUtils.ValidatePhoneNumber(vo.PhoneNumber);
        if (phoneNumberIsInvalid)
            throw new ArgumentException("Phone Number is invalid!");

        var emailIsInvalid = !StringUtils.ValidateEmailAddress(vo.EmailAddress);
        if (emailIsInvalid)
            throw new ArgumentException("Email Address is invalid!");

        //CHECK IF DATA IS ALREADY IN USE

        var nameAlreadyInUse = Repository.ContactNameAlreadyExists(vo.Name, vo.Id);
        if (nameAlreadyInUse)
            throw new ArgumentException("Name already in use!");

        var phoneAlreadyInUse = Repository.ContactPhoneAlreadyExists(vo.PhoneDDD, vo.PhoneNumber, vo.Id);
        if (phoneAlreadyInUse)
            throw new ArgumentException("Phone already in use!");

        var emailAlreadyInUse = Repository.ContactEmailAlreadyExists(vo.EmailAddress, vo.Id);
        if (emailAlreadyInUse)
            throw new ArgumentException("Email already in use!");
    }
}