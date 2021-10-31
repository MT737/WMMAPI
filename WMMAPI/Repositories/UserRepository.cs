using System;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserService
    {
        public UserRepository(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Authenticates the provided email and password against stored DB values. Returns associated user.
        /// </summary>
        /// <param name="email">Email input provided by client.</param>
        /// <param name="password">Password input provided by client.</param>
        /// <returns>Null if failure to authenticate. User model if authentication succeeds.</returns>
        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = Context.Users
                .SingleOrDefault(u => u.EmailAddress == email);

            // Check if email exists
            if (user == null)
                return null;

            // Check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // Authentication successful
            return user;
        }

        //TODO: Add Summary
        public User Create(User user, string password)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (EmailExists(user.EmailAddress))
                throw new AppException($"Email address {user.EmailAddress} is already registered to an account.");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            Add(user);

            return user;
        }

        public void Modify(User user, string password = null)
        {
            var currentUser = Context.Users
                .FirstOrDefault(u => u.UserId == user.UserId);

            if (currentUser == null)
                throw new AppException("User not found");

            // Confirm email is open or current email for user
            if (!String.IsNullOrWhiteSpace(user.EmailAddress))
            {
                if (Context.Users.Any(u => u.EmailAddress == user.EmailAddress && u.UserId != user.UserId))
                    throw new AppException("Email is alread registered to an account");
                
                currentUser.EmailAddress = user.EmailAddress;
            }

            // Update remaining properties
            if (!string.IsNullOrWhiteSpace(user.FirstName))
                currentUser.FirstName = user.FirstName;

            if (!string.IsNullOrWhiteSpace(user.LastName))
                currentUser.LastName = user.LastName;

            if (user.DOB != DateTime.MinValue)
                currentUser.DOB = user.DOB;

            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                currentUser.PasswordHash = passwordHash;
                currentUser.PasswordSalt = passwordSalt;
            }

            Update(currentUser);            
        }

        /// <summary>
        /// Gets user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User associated to provided Id</returns>
        public User GetById(Guid id)
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


        // Private helper methods
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace, only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordSalt");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        public void RemoveUser(Guid userId)
        {
            //TODO: Additional validation?
            Delete(userId);
        }
    }
}
