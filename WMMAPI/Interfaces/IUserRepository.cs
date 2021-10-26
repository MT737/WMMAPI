using System;
using WMMAPI.Database.Models;
using WMMAPI.Interfaces;

namespace WMMAPI.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        bool EmailExists(string email);
        User Get(Guid id);
    }
}