namespace Contacts.Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await Application.GetWebApplication(args).RunAsync();
    }
}
