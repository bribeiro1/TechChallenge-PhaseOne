using Contacts.Api.Controllers;
using Contacts.Application.Contexts;
using Contacts.Domain.Contacts.Services;
using Contacts.Domain.Contacts.VOs;
using Contacts.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Test.Tests
{
    public class ContactControllerTest : IDisposable
    {
        private WebApplicationBuilder builder;
        private WebApplication app;

        private readonly ContactController controller;

        #region SampleValues

        private const string Name_Empty = "";
        private const string Name_Valid1 = "Name1";
        private const string Name_Valid2 = "Name2";

        private const string PhoneDDD_Empty = "";
        private const string PhoneDDD_Valid1 = "11";
        private const string PhoneDDD_Valid2 = "21";
        private const string PhoneDDD_Invalid = "01";

        private const string PhoneNumber_Empty = "";
        private const string PhoneNumber_Valid8Digits1 = "30222222";
        private const string PhoneNumber_Valid8Digits2 = "30111111";
        private const string PhoneNumber_Valid9Digits1 = "992222222";
        private const string PhoneNumber_Valid9Digits2 = "991111111";
        private const string PhoneNumber_Invalid8Digits = "92222222";
        private const string PhoneNumber_Invalid9Digits = "122222222";
        private const string PhoneNumber_InvalidWithSimbols = "9222-2222";

        private const string EmailAddress_Empty = "";
        private const string EmailAddress_Valid1 = "mail1@domain.com";
        private const string EmailAddress_Valid2 = "mail2@domain.com";
        private const string EmailAddress_Invalid = "mail@domain@com";

        #endregion

        public ContactControllerTest()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "");

            builder = WebApplication.CreateBuilder();
            builder.Services.AddSingleton<DbInitializer>();
            builder.Services.AddDbContext<AppDbContext>(options =>
                 options.UseSqlite("Filename=:memory:"));
            builder.Services.AddAutoRegister<AppDbContext>();
            builder.Services.AddControllers();

            app = builder.Build();

            var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
            dbInitializer.Initialize();

            var dbContext = app.Services.GetRequiredService<AppDbContext>();
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();

            var service = app.Services.GetRequiredService<IContactService>();
            controller = new ContactController(service);
        }

        public void Dispose()
        {
            app.DisposeAsync().AsTask().Wait();
            app = null;
            builder = null;
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void List_ShouldReturnAllContacts_WhenExists()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            // Act
            var result = controller.List();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IList<ContactVO>>(okResult.Value);

            Assert.Single(contacts);
            Assert.Equal(Name_Valid1, contacts[0].Name);
            Assert.Equal(PhoneDDD_Valid1, contacts[0].PhoneDDD);
            Assert.Equal(PhoneNumber_Valid8Digits1, contacts[0].PhoneNumber);
            Assert.Equal(EmailAddress_Valid1, contacts[0].EmailAddress);
        }

        [Fact]
        public void List_ShouldReturnAnEmptyList_WhenDoesntExists()
        {
            // Arrange

            // Act
            var result = controller.List();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IList<ContactVO>>(okResult.Value);

            Assert.Empty(contacts);
        }

        [Fact]
        public void List_ShouldThrowAnException_WhenAnyErrorOccurs()
        {
            // Arrange
            var newController = new ContactController(null);

            // Act
            var result = newController.List();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void ListByDDD_ShouldReturnCorrespondingContacts_WhenExists()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid9Digits1, EmailAddress = EmailAddress_Valid2 };
            controller.Create(contact);

            // Act
            var result = controller.List(PhoneDDD_Valid2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IList<ContactVO>>(okResult.Value);

            Assert.Single(contacts);
            Assert.Equal(Name_Valid2, contacts[0].Name);
            Assert.Equal(PhoneDDD_Valid2, contacts[0].PhoneDDD);
            Assert.Equal(PhoneNumber_Valid9Digits1, contacts[0].PhoneNumber);
            Assert.Equal(EmailAddress_Valid2, contacts[0].EmailAddress);
        }

        [Fact]
        public void ListByDDD_ShouldReturnAnEmptyList_WhenDoesntExists()
        {
            // Arrange

            // Act
            var result = controller.List(PhoneDDD_Valid2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var contacts = Assert.IsAssignableFrom<IList<ContactVO>>(okResult.Value);
            Assert.Empty(contacts);
        }

        [Fact]
        public void ListByDDD_ShouldThrowAnException_WhenAnyErrorOccurs()
        {
            // Arrange
            var newController = new ContactController(null);

            // Act
            var result = newController.List(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Create_ShouldCreateAContact_WhenValid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenIdIsSet()
        {
            // Arrange
            var contact = new ContactVO { Id = Guid.NewGuid(), Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Entry not found!", badRequestResult.Value);
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenNameIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Empty, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Name shouldn't be empty!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneDDDIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Empty, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone DDD is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Empty, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenEmailAddressIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Empty };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email Address is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneDDDIsInvalid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Invalid, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone DDD is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneNumberContainsAnyNonAlphanumericCharacters()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_InvalidWithSimbols, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number should have only numbers!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneNumberIsInvalid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Invalid8Digits, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenEmailAddressIsInvalid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Invalid };

            // Act
            var result = controller.Create(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email Address is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenNameAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var newcontact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid8Digits2, EmailAddress = EmailAddress_Valid2 };

            // HERE


            // Act
            var result = controller.Create(newcontact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Name already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenPhoneAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var newcontact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid2 };

            // Act
            var result = controller.Create(newcontact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Create_ShouldThrowAnException_WhenEmailAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var newcontact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid8Digits2, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Create(newcontact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldUpdateAContact_WhenValid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.Name = Name_Valid2;

            // Act
            var result = controller.Update(existingContact);

            //// Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenNothingChanged()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];

            // Act
            var result = controller.Update(existingContact);

            //// Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nothing to update!", badRequestResult.Value);
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenIdDoesntExists()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };

            // Act
            var result = controller.Update(contact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Id shouldn't be empty!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenNameIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.Name = Name_Empty;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Name shouldn't be empty!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneDDDIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.PhoneDDD = PhoneDDD_Empty;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone DDD is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneNumberIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.PhoneNumber = PhoneNumber_Empty;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenEmailAddressIsEmpty()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.EmailAddress = EmailAddress_Empty;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email Address is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneDDDIsInvalid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.PhoneDDD = PhoneDDD_Invalid;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone DDD is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneNumberContainsAnyNonAlphanumericCharacters()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.PhoneNumber = PhoneNumber_InvalidWithSimbols;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number should have only numbers!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneNumberIsInvalid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];
            existingContact.PhoneNumber = PhoneNumber_Invalid9Digits;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone Number is invalid!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenNameAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];

            var anotherContact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid8Digits2, EmailAddress = EmailAddress_Valid2 };
            controller.Create(anotherContact);

            existingContact.Name = Name_Valid2;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Name already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenPhoneAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];

            var anotherContact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid8Digits2, EmailAddress = EmailAddress_Valid2 };
            controller.Create(anotherContact);

            existingContact.PhoneDDD = PhoneDDD_Valid2;
            existingContact.PhoneNumber = PhoneNumber_Valid8Digits2;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Phone already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Update_ShouldThrowAnException_WhenEmailAlreadyInUse()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];

            var anotherContact = new ContactVO { Name = Name_Valid2, PhoneDDD = PhoneDDD_Valid2, PhoneNumber = PhoneNumber_Valid8Digits2, EmailAddress = EmailAddress_Valid2 };
            controller.Create(anotherContact);

            existingContact.EmailAddress = EmailAddress_Valid2;

            // Act
            var result = controller.Update(existingContact);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email already in use!", badRequestResult.Value.ToString());
        }

        [Fact]
        public void Delete_ShouldDeleteAContact_WhenValid()
        {
            // Arrange
            var contact = new ContactVO { Name = Name_Valid1, PhoneDDD = PhoneDDD_Valid1, PhoneNumber = PhoneNumber_Valid8Digits1, EmailAddress = EmailAddress_Valid1 };
            controller.Create(contact);

            var existingResult = controller.List();
            var existingOkResult = Assert.IsType<OkObjectResult>(existingResult.Result);
            var existingContacts = Assert.IsAssignableFrom<IList<ContactVO>>(existingOkResult.Value);

            var existingContact = existingContacts[0];

            var existingContactId = existingContact.Id;

            // Act
            var result = controller.Delete(existingContactId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_ShouldThrowAnException_WhenIdDoesntExists()
        {
            // Arrange
            var contactId = Guid.NewGuid();

            // Act
            var result = controller.Delete(contactId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id not found!", badRequestResult.Value);
        }
    }
}
