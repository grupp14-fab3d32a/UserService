using Business.Contracts.Requests;
using Data.Entities;

namespace Business.Factories;
public class AuthFactory
{
  public static UserEntity ToEntity(SignUpRequest form) =>
    new()
    {
      FirstName = form.FirstName,
      LastName = form.LastName,
      Email = form.Email,
      UserName = form.Email
    };
}
