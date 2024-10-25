namespace Contacts.Domain.ValueObjects;
public class Phone
{
    public string Number { get; private set; } = string.Empty;
    public string DDD { get; private set; } = string.Empty;

    public Phone() { }

    public Phone(string number, string ddd)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length != 9)
            throw new ArgumentException("O número do telefone deve conter 9 dígitos.");

        if (string.IsNullOrWhiteSpace(ddd) || ddd.Length != 2)
            throw new ArgumentException("O DDD deve conter 2 dígitos.");

        Number = number;
        DDD = ddd;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Phone phone) return false;
        return Number == phone.Number && DDD == phone.DDD;
    }

    public override int GetHashCode() => HashCode.Combine(Number, DDD);

    public override string ToString() => $"({DDD}) {Number}";
}
