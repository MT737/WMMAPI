using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;

namespace WMMAPI.Services
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        public CategoryService(WMMContext context) : base(context)
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

        public void AddCategory(Category category)
        {
            // Validate category. Validation errors result in thrown exceptions.
            ValidateCategory(category);

            // If still here, validation passed. Add category.
            Add(category);
        }

        public void ModifyCategory(Category category)
        {
            Category currentCategory = Context.Categories
                .FirstOrDefault(c => c.CategoryId == category.CategoryId && c.UserId == category.UserId);

            if (currentCategory == null)
                throw new AppException("Category not found.");

            if (currentCategory.IsDefault)
                throw new AppException("Default categories cannot be modified.");

            // Validate category modification. Validation errors result in thrown exceptions.
            ValidateCategory(category);

            // If still here, validation passed. Update properties and call update.
            currentCategory.Name = category.Name;
            currentCategory.IsDefault = category.IsDisplayed;
            Update(currentCategory);
        }

        public void DeleteCategory(Guid absorbedId, Guid absorbingId, Guid userId)
        {
            // Confirm categories exist and are owned by user
            var absorbedCatExists = Context.Categories.FirstOrDefault(c => c.CategoryId == absorbedId && c.UserId == userId);
            if (absorbedCatExists == null)
                throw new AppException("Category selected for deletion not found.");
            if (absorbedCatExists.IsDefault)
                throw new AppException($"{absorbedCatExists.Name} is a default category and cannot be deleted.");
            
            var absorbingCatExists = Context.Categories.Any(c => c.CategoryId == absorbingId && c.UserId == userId);
            if (!absorbingCatExists)
                throw new AppException("Category selected to absorbed deleted category not found.");

            //TODO: Problem. What if the db fails to delete post absorption? Worst case, category continues to exist
            //but all transactions have been modified. However, would prefer to update the database at one time...
            // Call absorption process
            Absorption(absorbedId, absorbingId, userId);
            Delete(absorbedId);
        }

        /// <summary>
        /// Indicates the existence of a the category.
        /// </summary>
        /// <param name="desiredCategoryName">String: Desired category name.</param>
        /// <param name="categoryId">Category Id of which the name existence is desired</param>
        /// <param name="userId">Guid: UserID of the account.</param>
        /// <returns>Bool: Indication of the category name's current existence in the user's DB profile.</returns>
        public bool NameExists(Category category)
        {
            return Context.Categories
                .Where(c => c.UserId == category.UserId
                    && c.Name.ToLower() == category.Name.ToLower()
                    && c.CategoryId == category.CategoryId)
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

        //TODO: Deprecated??
        /// <summary>
        /// Inidcates the default status of the category.
        /// </summary>
        /// <param name="entityID">Guid: the id of the category for which to check default status.</param>
        /// <param name="userID">Guid: the user's Id to confirm ownership of the category.</param>
        /// <returns></returns>
        //public bool IsDefault(Guid entityId, Guid userId)
        //{
        //    return Context.Categories
        //        .Where(c => c.CategoryId == entityId && c.UserId == userId && c.IsDefault == true)
        //        .Any();
        //}

        // Private helper methods
        private void ValidateCategory(Category category)
        {
            if (NameExists(category))
                throw new AppException($"{category.Name} already exists.");

            if (String.IsNullOrWhiteSpace(category.Name))
                throw new AppException("Category name cannot be empty or whitespace only string.");
        }
    }
}
