using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.CategoryModels;
using static WMMAPI.Helpers.Globals;

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
        /// <returns>A Category model for the requested category.</returns>
        public CategoryModel Get(Guid id, Guid userId)
        {
            var category = Context.Categories
                .Where(c => c.Id == id && c.UserId == userId)
                .SingleOrDefault();

            if (category == null)
                throw new AppException("Category not found.");

            return new CategoryModel(category);
        }

        /// <summary>
        /// Returns an IList of categories that exist in the database.
        /// </summary>
        /// <param name="userId">Guid: Id of the user that owns the categories</param>
        /// <returns>An IList of category models</returns>
        public IList<CategoryModel> GetList(Guid userId)
        {
            var categories = Context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .Select(c => new CategoryModel(c));

            if (!categories.Any())
                throw new AppException("No categories not found.");

            return categories.ToList();
        }

        /// <summary>
        /// Adds category to the db.
        /// </summary>
        /// <param name="category">Category to add to the database. Throws AppException if validation fails.</param>
        public void AddCategory(Category category)
        {
            // Validate category. Validation errors result in thrown exceptions.
            ValidateCategory(category);

            // If still here, validation passed. Add category.
            Add(category);
        }

        /// <summary>
        /// Validates changes and modifies the passed category.
        /// </summary>
        /// <param name="category">Modifies passed account. Throws AppException if validation fails.</param>
        public void ModifyCategory(Category category)
        {

            Category currentCategory = Context.Categories
                .FirstOrDefault(c => c.Id == category.Id && c.UserId == category.UserId);

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

        /// <summary>
        /// Adjusts all transactions using this category to another cateogry and removes category record from the database.
        /// </summary>
        /// <param name="absorbedId">CategoryId of the category to be removed from the database.</param>
        /// <param name="absorbingId">CategoryId of the category to absorb the transaction of the to be deleted category.</param>
        /// <param name="userId">UserId of the owner of the categories.</param>
        public void DeleteCategory(Guid absorbedId, Guid absorbingId, Guid userId)
        {
            // Confirm categories exist and are owned by user
            var absorbedCatExists = Context.Categories.FirstOrDefault(c => c.Id == absorbedId && c.UserId == userId);
            if (absorbedCatExists == null)
                throw new AppException("Category selected for deletion not found.");
            if (absorbedCatExists.IsDefault)
                throw new AppException($"{absorbedCatExists.Name} is a default category and cannot be deleted.");
            
            var absorbingCatExists = Context.Categories.Any(c => c.Id == absorbingId && c.UserId == userId);
            if (!absorbingCatExists)
                throw new AppException("Category selected to absorbed deleted category not found.");

            //TODO: Problem. What if the db fails to delete post absorption? Worst case, category continues to exist
            //but all transactions have been modified. However, would prefer to update the database at one time...
            // Call absorption process
            Absorption(absorbedId, absorbingId, userId);
            Delete(absorbedId);
        }

        /// <summary>
        /// Generates default categories for the user if they do not exist in the user's DB profile.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for which to generate default categories.</param>
        public void CreateDefaults(Guid userId)
        {
            if (!DefaultsExist(userId)) //Preventing duplication of defaults.
            {
                string[] categories = DefaultCategories.GetAllDefaultCategories();
                string[] notDisplayed = DefaultCategories.GetAllNotDisplayedDefaultCategories();

                foreach (string category in categories)
                {
                    Category cat = new Category
                    {
                        UserId = userId,
                        Name = category,
                        IsDefault = true,
                        IsDisplayed = !notDisplayed.Contains(category) // TODO refactor to remove double neg?
                    };

                    Add(cat, false);
                }

                // Save categories to the db
                SaveChanges();
            }
        }


        #region Private Helpers
        /// <summary>
        /// Indicates the existence of a the category.
        /// </summary>
        /// <param name="desiredCategoryName">String: Desired category name.</param>
        /// <param name="categoryId">Category Id of which the name existence is desired</param>
        /// <param name="userId">Guid: UserID of the account.</param>
        /// <returns>Bool: Indication of the category name's current existence in the user's DB profile.</returns>
        private bool NameExists(Category category)
        {
            return Context.Categories
                .Where(c => c.UserId == category.UserId
                    && c.Name.ToLower() == category.Name.ToLower()
                    && c.Id != category.Id)
                .Any();
        }

        /// <summary>
        /// Converts the absorbed category Id field in all transactions to the absorbing category Id.
        /// </summary>
        /// <param name="absorbedId">Guid: category Id that is being absorbed.</param>
        /// <param name="absorbingId">Guid: category Id that is absorbing.</param>
        /// <param name="userId">Guid: User Id of the owner of the categories being adjusted.</param>
        private void Absorption(Guid absorbedId, Guid absorbingId, Guid userId)
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
        /// Indicates the existing of default categories in the user's DB profile.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for which to look for default categories.</param>
        /// <returns>Bool: Indication of the existence of default categories in the user's DB profile.</returns>
        private bool DefaultsExist(Guid userId)
        {
            return Context.Categories.Where(c => c.UserId == userId && c.IsDefault == true).Any();
        }

        /// <summary>
        /// Validates the passed category.
        /// </summary>
        /// <param name="category">Category to be validated</param>
        /// <exception cref="AppException">Throws AppException if validation fails</exception>
        private void ValidateCategory(Category category)
        {
            if (String.IsNullOrWhiteSpace(category.Name))
                throw new AppException("Category name cannot be empty or whitespace only string.");

            if (NameExists(category))
                throw new AppException($"{category.Name} already exists.");
        }
        #endregion
    }
}
