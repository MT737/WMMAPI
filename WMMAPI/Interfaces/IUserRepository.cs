using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User Authenticate(string email, string password);
        User Create(User user, string password);
        void Modify(User user, string password);
        User GetById(Guid id);
        bool EmailExists(string email);
    }
}