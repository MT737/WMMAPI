using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Services;
using WMMAPITests.DataHelpers;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class VendorServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;

        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();
            _tdc = new TestDataContext(_testData);
            _tdc.WMMContext.Setup(m => m.Set<Vendor>()).Returns(_tdc.VendorSet.Object);
        }

        #region ServiceHelpers     
        [DataTestMethod]
        [DataRow(1126.80)]
        public void GetAmountSucceeds(double spending)
        {
            // Fabricate test transactions for the given vendor
            Vendor vend = _testData.Vendors.Skip(3).First(); // Skip in order to pick a non-default vendor
            List<Account> accounts = _testData.Accounts.Where(a => a.UserId == vend.UserId).ToList();
            List<Category> categories = _testData.Categories.Where(c => c.UserId == vend.UserId).ToList();

            List<Transaction> trans = new();
            for (int i = 0; i < 4; i++)
            {
                Account account = accounts.ElementAt(TestData._random.Next(accounts.Count()));
                Guid categoryId = categories.ElementAt(TestData._random.Next(categories.Count())).Id;

                trans.Add(_testData.CreateTestTransaction(account, true, (decimal)spending / 4, categoryId, vend.Id));
            }
            _testData.Transactions = _testData.Transactions.Concat(trans);

            // Initialize service and call method
            _tdc = new(_testData);
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            var result = service.GetVendorSpending(vend.Id, vend.UserId);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            Assert.AreEqual((decimal)spending, result);
        }

        [TestMethod]
        public void GetAmountWhenNoTransactions()
        {
            // Fabricate test transactions for the given vendor
            Guid vendorId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            var result = service.GetVendorSpending(vendorId, userId);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            Assert.AreEqual(0.00M, result);
        }

        [TestMethod]
        public void TestNameExistsTrue()
        {
            // Fabricate test data
            Vendor vend = _testData.Vendors.Skip(3).First();
            Vendor vend2 = _testData.Vendors.Skip(4).First();
            vend.Name = vend2.Name;

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            bool result = service.NameExists(vend2);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestNameExistsFalse()
        {
            // Fabricate test data
            Vendor vend = _testData.Vendors.Skip(3).First();
            vend.Name = "TestVendName";

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            bool result = service.NameExists(vend);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestAbsorptionSucceeds()
        {
            // Fabricate test data
            Guid userId = _testData.Users.First().Id;
            Vendor absorbVend = GenerateVendorWithTransactions("absorbVend", userId, false);
            Vendor absorbedVend = GenerateVendorWithTransactions("absorbedVend", userId, false);

            // Initialize service and call method
            _tdc = new TestDataContext(_testData);
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.Absorption(absorbedVend.Id, absorbVend.Id, userId);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
            Assert.IsTrue(_testData.Transactions.Any(t => t.VendorId == absorbVend.Id));
            Assert.IsFalse(_testData.Transactions.Any(t => t.VendorId == absorbedVend.Id));
        }

        [TestMethod]
        public void TestCreateDefaultsSucceeds()
        {
            // Fabricate test data
            Guid userId = Guid.NewGuid(); // New user so no defaults

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.CreateDefaults(userId);

            // Verify mock and assert (just verify, nothing to assert)
            int defaultVendCount = Globals.DefaultVendors.GetAllDevaultVendors().Count();
            _tdc.VendorSet.Verify(m => m.Add(It.IsAny<Vendor>()), Times.Exactly(defaultVendCount));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());            
        }
        
        [TestMethod]
        public void TestDefaultsExistTheyDo()
        {
            // Fabricate test (use existing customer with defaults)
            Guid userId = _testData.Users.First().Id;

            // Initialize mock and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            bool result = service.DefaultsExist(userId);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDefaultsExistTheyDoNot()
        {
            // Fabricate test
            Guid userId = Guid.NewGuid();

            // Initialize mock and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            bool result = service.DefaultsExist(userId);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void TestValidateVendorSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor vendor = _testData.CreateTestVendor(true, user.Id, null, false);

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ValidateVendor(vendor);

            // Verify mock and assert (nothing to assert)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateVendorFailsNameExists()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateVendorFailsNameEmptyString()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ServiceTests
        [TestMethod]
        public void TestGetSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetDoesntExistReturnsNull()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetListSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestGetListDoesntExistReturnsEmpty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestAddVendor()
        {
            throw new NotImplementedException();

            // Failures covered in validation tests
        }

        [TestMethod]
        public void TestModifyVendorSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorVendorDoesntExist()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorDefault()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestDeleteVendorSucceeds()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteVendorVendorDoesntExist()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteVendorDefaultVendor()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteAbsorbingVendorDoesntExist()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PrivateHelpers
        private Vendor GenerateVendorWithTransactions(string vendName, Guid userId, bool isDefault)
        {
            Vendor vend = _testData.CreateTestVendor(true, userId, vendName,  isDefault);
            _testData.Vendors = _testData.Vendors.Concat(new List<Vendor> { vend });

            List<Transaction> transactions = new();
            for (int i = 0; i < 4; i++)
            {
                transactions.Add(_testData.CreateTestTransaction(
                    _testData.Accounts.Where(a => a.UserId == userId).First(),
                    true, 25.00M, 
                    _testData.Categories.Where(c => c.UserId == userId).Skip(2).First().Id,
                    vend.Id
                    ));
            }
            _testData.Transactions = _testData.Transactions.Concat(transactions);

            return vend;
        }
        #endregion
    }
}
