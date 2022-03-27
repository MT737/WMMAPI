using System;
using WMMAPI.Database.Entities;

namespace WMMAPI.Interfaces
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        User Create(User user, string password);
        void Modify(User user, string password);
        User GetById(Guid id);
        void RemoveUser(Guid userId);
    }
}