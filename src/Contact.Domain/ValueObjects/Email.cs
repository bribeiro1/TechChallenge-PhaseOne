using System.Text.RegularExpressions;

namespace Contacts.Domain.ValueObjects;
public class Email
{
    public string Address { get; private set; } = string.Empty;

    public Email() { }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("O endereço de e-mail não pode ser nulo ou vazio.");

        if (!ValidateEmail(address))
            throw new ArgumentException("O endereço de e-mail é inválido.");

        Address = address;
    }

    private bool ValidateEmail(string email)
    {
        var regex = new Regex(@"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$");
        return regex.IsMatch(email);
    }

    public override string ToString() => Address;

    public override bool Equals(object obj)
    {
        if (obj is Email email)
            return Address == email.Address;

        return false;
    }

    public override int GetHashCode() => Address.GetHashCode();
}
