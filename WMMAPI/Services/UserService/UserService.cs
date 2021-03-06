using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using static WMMAPI.Helpers.Globals;

namespace WMMAPI.Services.UserService
{
    public class UserService : BaseService<User>, IUserService
    {
        public UserService(WMMContext context) : base(context)
        {
        }

        #region ServiceMethods
        /// <summary>
        /// Authenticates the provided email and password against stored DB values. Returns associated user.
        /// </summary>
        /// <param name="email">Email input provided by client.</param>
        /// <param name="password">Password input provided by client.</param>
        /// <returns>Null if failure to authenticate. User model if authentication succeeds.</returns>
        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = Context.Users
                .SingleOrDefault(u => u.EmailAddress == email && u.IsDeleted == false);

            // Check if user exists
            if (user == null)
                return null;

            // Check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // Authentication successful
            return user;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user">User object with completed required properties</param>
        /// <param name="password">User password</param>
        /// <returns>User model for newly created user</returns>
        /// <exception cref="AppException">Throws AppException if validation fails</exception>
        public User Create(User user, string password)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (string.IsNullOrWhiteSpace(user.EmailAddress))
                throw new AppException("Email is required");

            if (EmailExists(user.EmailAddress))
                throw new AppException($"Email address {user.EmailAddress} is already registered to an account.");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.Categories = CreateDefaults<Category>(
                user.Id, DefaultCategories.GetAllDefaultCategories(), DefaultCategories.GetAllNotDisplayedDefaultCategories());
            user.Vendors = CreateDefaults<Vendor>(
                user.Id, DefaultVendors.GetAllDevaultVendors(), DefaultVendors.GetAllNotDisplayedDefaultVendors());

            Add(user);

            return user;
        }

        /// <summary>
        /// Modifies a user using the passed-in user values
        /// </summary>
        /// <param name="user">User to be modified</param>
        /// <param name="password">New password if changed</param>
        /// <exception cref="AppException">Throws AppException if user not found or email already in use</exception>
        public void Modify(User user, string password)
        {
            var currentUser = Context.Users
                .FirstOrDefault(u => u.Id == user.Id);

            if (currentUser == null)
                throw new AppException("User not found");

            // Confirm email is open or current email for user
            if (!String.IsNullOrWhiteSpace(user.EmailAddress))
            {
                if (Context.Users.Any(u => u.EmailAddress == user.EmailAddress && u.Id != user.Id))
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

            currentUser.IsDeleted = false;

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
                .Where(u => u.Id == id)
                .SingleOrDefault();
        }

        /// <summary>
        /// Removes user from the DB.
        /// </summary>
        /// <param name="userId">User ID of the user to be removed from the db</param>
        public void RemoveUser(Guid userId)
        {
            var currentUser = Context.Users
                .FirstOrDefault(u => u.Id == userId);

            if (currentUser == null)
                throw new AppException("User not found");

            currentUser.IsDeleted = true;

            Update(currentUser);
        }
        #endregion


        // TODO Add email/account recovery for users that forget credentials and for previously deleted accounts.

        #region Private Helpers
        /// <summary>
        /// Compares passed email string to emails in the db.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Returns bool stating if the past email already exists in the db.</returns>
        private bool EmailExists(string email)
        {
            return Context.Users
                .Any(u => u.EmailAddress.ToLower() == email.ToLower());
        }

        /// <summary>
        /// Creates a password hash based on the passed password.
        /// </summary>
        /// <param name="password">New password to be hashed</param>
        /// <param name="passwordHash">Output hash</param>
        /// <param name="passwordSalt">Ouput salt</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Verifies that the password passed in, once hashed, matches the stored hash.
        /// </summary>
        /// <param name="password">Password provided by the user attempting authorization</param>
        /// <param name="storedHash">The stored password hash for the requested user</param>
        /// <param name="storedSalt">The stored salt used to generate the original password hash</param>
        /// <returns>Bool: true if the password hash is varified</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Creates property defaults based on the passed type
        /// </summary>
        /// <typeparam name="T">Type must inherit from BaseVendCat</typeparam>
        /// <param name="userId">User's Id</param>
        /// <param name="defaults">String array containing the entry names for which to create defaults.</param>
        /// <param name="notDisplayed">String array containing the entry names for which to not display.</param>
        /// <returns></returns>
        private ICollection<T> CreateDefaults<T>(Guid userId, string[] defaults, string[] notDisplayed) where T : BaseVendCat
        {
            ICollection<T> defaultsList = new List<T>();
            foreach (var item in defaults)
            {
                var entry = (T)Activator.CreateInstance(typeof(T));
                entry.Id = Guid.NewGuid();
                entry.UserId = userId;
                entry.Name = item;
                entry.IsDefault = !notDisplayed.Contains(item);
                entry.IsDisplayed = true;
                
                defaultsList.Add(entry);
            }
            return defaultsList;
        }
        #endregion
    }
}
