using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMMAPI.Database;
using WMMAPI.Database.Models;
using WMMAPI.Interfaces;

namespace WMMAPI.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(WMMContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns the category for the specified Id.
        /// </summary>
        /// <param name="id">Guid: Id of the category to return.</param>
        /// <param name="userId">Guid: Id of the user that owns the category.</param>
        /// <returns>A Category entity.</returns>
        public Category Get(Guid id, Guid userId)
        {
            return Context.Categories
                .Where(c => c.CategoryId == id && c.UserId == userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Returns an IList of categories.
        /// </summary>
        /// <param name="userId">Guid: Id of the user that owns the categories</param>
        /// <returns>An IList of category entities</returns>
        public List<Category> GetList(Guid userId)
        {
            return Context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToList();
        }

        /// <summary>
        /// Returns a count of Categories owned by the user.
        /// </summary>
        /// <param name="userId">Guid: Id of the user that owns the categories.</param>
        /// <returns>Int: value representing the number categories owned by the user.</returns>
        public int GetCount(Guid userId)
        {
            return Context.Categories
                .Where(c => c.UserId == userId)
                .Count();
        }

        /// <summary>
        /// Indicates the existence of a the category.
        /// </summary>
        /// <param name="desiredCategoryName">String: Desired category name.</param>
        /// <param name="categoryId">Category Id of which the name existence is desired</param>
        /// <param name="userId">Guid: UserID of the account.</param>
        /// <returns>Bool: Indication of the category name's current existence in the user's DB profile.</returns>
        public bool NameExists(string desiredCategoryName, Guid categoryId, Guid userId)
        {
            return Context.Categories
                .Where(c => c.UserId == userId
                    && c.Name.ToLower() == desiredCategoryName.ToLower()
                    && c.CategoryId == categoryId)
                .Any();
        }

        /// <summary>
        /// Returns total spending in the specified category for the specified user.
        /// </summary>
        /// <param name="categoryId">Guid: Category Id for which to get total spending.</param>
        /// <param name="userId">Guid: User Id for which to get category total spending.</param>
        /// <returns>Decimal: total user specific spending for the category.</returns>
        public decimal GetCategorySpending(Guid categoryId, Guid userId)
        {
            return Context.Transactions
                .Where(t => t.CategoryId == categoryId && t.UserId == userId)
                .ToList().Sum(t => t.Amount);
        }

        /// <summary>
        /// Indicates if the user owns the specified category.
        /// </summary>
        /// <param name="categoryId">Guid: Id of the specified category.</param>
        /// <param name="userId">Guid: User's Id.</param>
        /// <returns>Bool: Indication of the user's ownership of the category.</returns>
        public bool UserOwnsCategory(Guid categoryId, Guid userId)
        {
            return Context.Categories
                .Where(c => c.CategoryId == categoryId && c.UserId == userId)
                .Any();
        }

        /// <summary>
        /// Returns the Id for the category specified by name.
        /// </summary>
        /// <param name="name">String: Name of the category for which to determine the Id.</param>
        /// <param name="userId">Guid: Id of the user that owns the category.</param>
        /// <returns>Guid: Id for the specified category.</returns>
        public Guid GetId(string name, Guid userId)
        {
            return Context.Categories
                .Where(c => c.Name == name && c.UserId == userId)
                .SingleOrDefault().CategoryId;
        }

        /// <summary>
        /// Converts the absorbed category Id field in all transactions to the absorbing category Id.
        /// </summary>
        /// <param name="absorbedId">Guid: category Id that is being absorbed.</param>
        /// <param name="absorbingId">Guid: category Id that is absorbing.</param>
        /// <param name="userId">Guid: User Id of the owner of the categories being adjusted.</param>
        public void Absorption(Guid absorbedId, Guid absorbingId, Guid userId)
        {
            // TODO: This works for a small database, but for large scale, this should be set to bulk update.
            IQueryable<Transaction> transactionCategoriesToUpdate = Context.Transactions
                .Where(c => c.CategoryId == absorbedId && c.UserId == userId);

            foreach (Transaction transaction in transactionCategoriesToUpdate)
            {
                transaction.CategoryId = absorbingId;
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// Generates default categories for the user if they do not exist in the user's DB profile.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for which to generate default categories.</param>
        public void CreateDefaults(Guid userId)
        {
            if (!DefaultsExist(userId)) //Preventing duplication of defaults.
            {
                // TODO: Replace these magic strings with a global constants.
                string[] categories = new string[] {"Account Transfer", "Account Correction", "New Account", "ATM Withdrawal", "Eating Out",
                "Entertainment", "Gas", "Groceries/Sundries", "Income", "Shopping", "Returns/Deposits", "Other"};

                foreach (string category in categories)
                {
                    Category cat = new Category
                    {
                        UserId = userId,
                        Name = category,
                        IsDefault = true,
                    };

                    // TODO: Replace these magic strings with global constants.
                    if (category == "Account Transfer" || category == "Account Correction" || category == "New Account" || category == "Income")
                    {
                        cat.IsDisplayed = false;
                    }
                    else
                    {
                        cat.IsDisplayed = true;
                    }

                    Add(cat);
                }
            }
        }

        /// <summary>
        /// Indicates the existing of default categories in the user's DB profile.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for which to look for default categories.</param>
        /// <returns>Bool: Indication of the existence of default categories in the user's DB profile.</returns>
        public bool DefaultsExist(Guid userId)
        {
            return Context.Categories.Where(c => c.UserId == userId && c.IsDefault == true).Any();
        }


        /// <summary>
        /// Inidcates the default status of the category.
        /// </summary>
        /// <param name="entityID">Guid: the id of the category for which to check default status.</param>
        /// <param name="userID">Guid: the user's Id to confirm ownership of the category.</param>
        /// <returns></returns>
        public bool IsDefault(Guid entityId, Guid userId)
        {
            return Context.Categories
                .Where(c => c.CategoryId == entityId && c.UserId == userId && c.IsDefault == true)
                .Any();
        }
    }
}
