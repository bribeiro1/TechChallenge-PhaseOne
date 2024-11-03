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
        string errorMessage = string.Empty;

        var nameIsEmpty = string.IsNullOrWhiteSpace(vo.Name);
        if (nameIsEmpty)
            errorMessage = "Name shouldn't be empty! \n";
        else
        {
            var nameAlreadyInUse = Repository.ContactNameAlreadyExists(vo.Name, vo.Id);
            if (nameAlreadyInUse)
                errorMessage += "Name already in use! \n";
        }


        var phoneDDDIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneDDD);
        if (phoneDDDIsEmpty)
            errorMessage += "Phone DDD shouldn't be empty! \n";
        else
        {
            var phoneDDDIsInvalid = !StringUtils.ValidatePhoneDDD(vo.PhoneDDD);
            if (phoneDDDIsInvalid)
                errorMessage += "Phone DDD is invalid! \n";
        }


        var phoneNumberIsEmpty = string.IsNullOrWhiteSpace(vo.PhoneNumber);
        if (phoneNumberIsEmpty)
            errorMessage += "Phone Number shouldn't be empty! \n";
        else
        {
            var phoneNumberContainsOnlyNumbers = vo.PhoneNumber.All(r => char.IsNumber(r));
            if (!phoneNumberContainsOnlyNumbers)
                errorMessage += "Phone Number should have only numbers! \n";
            else
            {
                var phoneNumberIsInvalid = !StringUtils.ValidatePhoneNumber(vo.PhoneNumber);
                if (phoneNumberIsInvalid)
                    errorMessage += "Phone Number is invalid! \n";
                else
                {
                    var phoneAlreadyInUse = Repository.ContactPhoneAlreadyExists(vo.PhoneDDD, vo.PhoneNumber, vo.Id);
                    if (phoneAlreadyInUse)
                        errorMessage += "Phone already in use! \n";
                }
            }
        }


        var emailIsEmpty = string.IsNullOrWhiteSpace(vo.EmailAddress);
        if (emailIsEmpty)
            errorMessage += "Email Address shouldn't be empty! \n";
        else
        {
            var emailIsInvalid = !StringUtils.ValidateEmailAddress(vo.EmailAddress);
            if (emailIsInvalid)
                errorMessage += "Email Address is invalid! \n";
            else
            {
                var emailAlreadyInUse = Repository.ContactEmailAlreadyExists(vo.EmailAddress, vo.Id);
                if (emailAlreadyInUse)
                    errorMessage += "Email already in use! \n";
            }
        }


        if (!string.IsNullOrEmpty(errorMessage))
            throw new ArgumentException(errorMessage);
    }
}