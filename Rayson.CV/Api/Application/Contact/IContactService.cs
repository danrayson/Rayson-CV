using Application.Core;

namespace Application.Contact;

public interface IContactService
{
    Task<ServiceResponse> SendContactEmailAsync(ContactRequest request);
}
