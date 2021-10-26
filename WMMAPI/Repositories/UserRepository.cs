using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database;
using WMMAPI.Database.Models;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User associated to provided Id</returns>
        public User Get(Guid id)
        {
            return Context.Users
                .Where(u => u.UserId == id)
                .SingleOrDefault();
        }

        /// <summary>
        /// Compares passed email string to emails in the db.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns bool stating if the past email already exists in the db.</returns>
        public bool EmailExists(string email)
        {
            return Context.Users
                .Any(u => u.EmailAddress.ToLower() == email.ToLower());
        }
    }
}
