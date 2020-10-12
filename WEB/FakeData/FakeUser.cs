#if SEEDING
using System.Collections.Generic;

using Bogus;
using Models;

namespace WEB.FakeData
{
    public static class FakeUser
    {
        public static List<WebUser> Generate(int number)
        {
            var userFaker = new Faker<WebUser>("ru")
                .RuleFor(b => b.Email, f => f.Person.Email.ToLower())
                .RuleFor(b => b.PhoneNumber, f => f.Phone.PhoneNumber())
                .RuleFor(b => b.UserName, f => f.Person.UserName);//f, b) => b.Email)
            return userFaker.Generate(number);
        }
    }
}
#endif
