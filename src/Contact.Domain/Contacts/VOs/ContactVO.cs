using Contacts.Domain.Contacts.Models;

namespace Contacts.Domain.Contacts.VOs;

public class ContactVO
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string PhoneDDD { get; set; }
    public string EmailAddress { get; set; }

    public static ContactVO Cast(Contact entity)
    {
        var vo = new ContactVO()
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            Name = entity.Name,
            PhoneDDD = entity.Phone.DDD,
            PhoneNumber = entity.Phone.Number,
            EmailAddress = entity.Email.Address,
        };

        return vo;
    }

    public static Contact Cast(ContactVO vo, Contact entity = null)
    {
        if (vo.Id == Guid.Empty)
            entity = new Contact();

        if (entity == null)
            throw new ArgumentException("Entry not found!");

        if (vo.Id != Guid.Empty)
        {
            var hasChanged = false;
            hasChanged |= entity.Name != vo.Name;
            hasChanged |= entity.Phone.DDD != vo.PhoneDDD;
            hasChanged |= entity.Phone.Number != vo.PhoneNumber;
            hasChanged |= entity.Email.Address != vo.EmailAddress;

            if (!hasChanged)
                throw new ArgumentException("Nothing to update!");

        }

        entity.Name = vo.Name;
        entity.Phone = new ContactPhone
        {
            DDD = vo.PhoneDDD,
            Number = vo.PhoneNumber,
        };
        entity.Email = new ContactEmail
        {
            Address = vo.EmailAddress,
        };

        return entity;
    }
}