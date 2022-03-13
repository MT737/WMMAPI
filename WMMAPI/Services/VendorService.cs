﻿using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using static WMMAPI.Helpers.Globals;

namespace WMMAPI.Services
{
    public class VendorService : BaseService<Vendor>, IVendorService
    {
        //TODO: Update all saved summaries (repos have had large modifications)

        public VendorService(WMMContext context) : base(context)
        {
        }

        #region ServiceMethods
        /// <summary>
        /// Returns the Vendor entity associated to the passed vendor Id.
        /// </summary>
        /// <param name="id">Guid: VendorId associated to the desired vendor.</param>
        /// <param name="userId">Guid: Id of the user that owns the vendor.</param>
        /// <returns>Vendor entity with the passed VendorId.</returns>
        public Vendor Get(Guid id, Guid userId)
        {
            return Context.Vendors
                .Where(v => v.Id == id && v.UserId == userId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Returns an IList of vendor entities.
        /// </summary>
        /// <param name="userID">Guid: UserId for which to get a list of vendors.</param>
        /// <returns>IList of vendor entities.</returns>
        public IList<Vendor> GetList(Guid userId)
        {
            return Context.Vendors
                .Where(v => v.UserId == userId)
                .OrderBy(v => v.Name)
                .ToList();
        }

        public void AddVendor(Vendor vendor)
        {
            // Validate vendor. Validation errors result in thrown exceptions.
            ValidateVendor(vendor);

            // If still here, validation passed. Call add
            Add(vendor);
        }

        public void ModifyVendor(Vendor vendor)
        {
            var currentVendor = Context.Vendors
                .FirstOrDefault(v => v.Id == vendor.Id && v.UserId == vendor.UserId);

            if (currentVendor == null)
                throw new AppException("Vendor not found.");

            if (currentVendor.IsDefault)
                throw new AppException("Default vendors cannot be modified.");

            // Validate vendor modification. Validation errors result in thrown exceptions.
            ValidateVendor(vendor);

            // If still here, validation passed. Update properties and call update
            currentVendor.Name = vendor.Name;
            currentVendor.IsDisplayed = vendor.IsDisplayed;

            Update(vendor);

        }

        //TODO: Much of this logic is shared with Category. Look into consolidation
        public void DeleteVendor(Guid absorbedId, Guid absorbingId, Guid userId)
        {
            // Confirm vendors exist and are owned by the user
            var absorbedVendorExists = Context.Vendors.FirstOrDefault(v => v.Id == absorbedId && v.UserId == userId);
            if (absorbedVendorExists == null)
                throw new AppException("Vendor selected for deletion not found.");
            if (absorbedVendorExists.IsDefault)
                throw new AppException($"{absorbedVendorExists.Name} is a default vendor and cannot be deleted.");

            var absorbingVendorExists = Context.Vendors.Any(v => v.Id == absorbingId && v.UserId == userId);
            if (!absorbingVendorExists)
                throw new AppException("Vendor selected to absorb deleted vendor not found.");

            //TODO: Problem. What if the db fails to delete post absorption? Worst case, vendor continues to exist
            Absorption(absorbedId, absorbingId, userId);
            Delete(absorbedId);
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Calculates the amount of user spending at a given vendor.
        /// </summary>
        /// <param name="vendorID">Guid: VendorId for which to determine total spending.</param>
        /// <param name="userID">Guid: UserId for which to determine total spending.</param>
        /// <returns>Decimal: amount of user spending with the given vendor.</returns>
        public decimal GetVendorSpending(Guid vendorID, Guid userID)
        {
            return Context.Transactions
                .Where(t => t.VendorId == vendorID && t.UserId == userID)
                .ToList().Sum(t => t.Amount);
        }

        /// <summary>
        /// Determines if the vendor name currently exits in the DB.
        /// </summary>
        /// <returns>Bool: True if the vendor name already exists in the DB. False otherwise.</returns>
        public bool NameExists(Vendor vendor)
        {
            return Context.Vendors
                .Where(v => v.UserId == vendor.UserId
                    && v.Name.ToLower() == vendor.Name.ToLower()
                    && v.Id != vendor.Id)
                .Any();
        }

        /// <summary>
        /// Replaces all Transactions table instances of the aborbedId with the absorbingId.
        /// </summary>
        /// <param name="absorbedId">Guid: the vendorId of the vendor being absorbed(deleted).</param>
        /// <param name="absorbingId">Guid: the vendorId of the vendor absorbing the absorbed VendorId.</param>
        /// <param name="userId">Guid: UserId of the owner of the vendors being adjusted.</param>
        public void Absorption(Guid absorbedId, Guid absorbingId, Guid userId)
        {
            //TODO: this works for a small database, but for large scale, this method should be updated to perform a bulk update.
            //TODO: Could use dapper to just manually generate the transaction script
            IQueryable<Transaction> vendorsToUpdate = Context.Transactions
                .Where(v => v.VendorId == absorbedId && v.UserId == userId);

            foreach (Transaction transaction in vendorsToUpdate)
            {
                transaction.VendorId = absorbingId;
            }
            Context.SaveChanges();
        }

        /// <summary>
        /// Creates default vendor data for the user.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for which to create defaults.</param>
        public void CreateDefaults(Guid userId)
        {
            if (!DefaultsExist(userId)) //Preventing duplication of defaults.
            {
                string[] vendors = DefaultVendors.GetAllDevaultVendors();
                string[] notDisplayed = DefaultVendors.GetAllNotDisplayedDefaultVendors();

                foreach (string vendor in vendors)
                {
                    Vendor vend = new Vendor
                    {
                        UserId = userId,
                        Name = vendor,
                        IsDefault = true,
                        IsDisplayed = !notDisplayed.Contains(vendor) // TODO refactor to remove double neg?
                    };

                    Add(vend, false);
                }

                // Save vendors to the db
                SaveChanges();
            }
        }

        /// <summary>
        /// Indicates the existence of default vendors in the user's DB profile.
        /// </summary>
        /// <param name="userId">Guid: Id of the user for whom to check for default vendors.</param>
        /// <returns>Bool: True if the user's DB profile contains default vendors. Otherwise false.</returns>
        public bool DefaultsExist(Guid userId)
        {
            return Context.Vendors.Where(v => v.UserId == userId && v.IsDefault == true).Any();
        }

        // Private helper methods
        public void ValidateVendor(Vendor vendor)
        {
            if (NameExists(vendor))
                throw new AppException($"Vendor {vendor.Name} already exists.");

            if (String.IsNullOrWhiteSpace(vendor.Name))
                throw new AppException("Vendor name cannot be empty or whitespace only string.");
        }
        #endregion
    }
}
