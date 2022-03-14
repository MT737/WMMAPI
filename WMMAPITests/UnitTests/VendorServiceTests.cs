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
            Vendor vendor = _testData.CreateTestVendor(true, user.Id, false, null);

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
            // Fabricate test
            Vendor vendor = _testData.Vendors.First();
            Vendor vendor2 = _testData.Vendors.Skip(1).First();
            vendor2.Name = vendor.Name;

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ValidateVendor(vendor2);

            // Verify mock and assert (nothing to assert)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestValidateVendorFailsNameEmptyString()
        {
            // Fabricate test
            Vendor vend = _testData.Vendors.First();
            vend.Name = "";

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ValidateVendor(vend);

            // Verify mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }
        #endregion

        #region ServiceTests
        [TestMethod]
        public void TestGetSucceeds()
        {
            // Fabricate test
            Vendor testVend = _testData.Vendors.First();

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            Vendor result = service.Get(testVend.Id, testVend.UserId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.AreEqual(testVend.Id, result.Id);
            Assert.AreEqual(testVend.UserId, result.UserId);
            Assert.AreEqual(testVend.Name, result.Name);
        }

        [TestMethod]
        public void TestGetDoesntExistReturnsNull()
        {
            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            Vendor result = service.Get(Guid.NewGuid(), Guid.NewGuid());

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetListSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            IList<Vendor> vendors = service.GetList(user.Id);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(vendors.Count > 0);
            Assert.IsFalse(vendors.Any(v => v.UserId != user.Id));
        }

        [TestMethod]
        public void TestGetListDoesntExistReturnsEmpty()
        {
            // Fabricate test
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            IList<Vendor> vendors = service.GetList(userId);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            Assert.IsTrue(vendors.Count == 0);
        }

        [TestMethod]
        public void TestAddVendorSucceeds() // Failures covered in validation tests
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor vendor = _testData.CreateTestVendor(true, user.Id, false, "TestAddVendor");

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.AddVendor(vendor);

            // Confirm mock and assert (no assert required, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
            _tdc.VendorSet.Verify(m => m.Add(It.IsAny<Vendor>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());            
        }

        [TestMethod]
        public void TestModifyVendorSucceeds()
        {
            // Fabricate test
            Vendor vend = _testData.Vendors.First(v => !v.IsDefault);
            vend.Name = "Modified name";
            vend.IsDisplayed = !vend.IsDisplayed;

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert (no assert necessary, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorVendorDoesntExist()
        {
            // Fabricate test
            Vendor vend = _testData.CreateTestVendor(true, Guid.NewGuid());
            vend.Name = "modified Name";

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert (no assert necessary, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyVendorDefault()
        {
            // Fabricate test
            Vendor vend = _testData.Vendors.First(v => v.IsDefault);
            vend.Name = "Modified name";

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.ModifyVendor(vend);

            // Confirm mock and assert
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Once());
        }

        [TestMethod]
        public void TestDeleteVendorSucceeds()
        {
            // Fabricate test
            User user = _testData.Users.First();
            List<Vendor> vendors = _testData.Vendors.Where(v => !v.IsDefault && v.UserId == user.Id).Take(2).ToList();
            Vendor absorb = vendors.First();
            Vendor absorbed = vendors.Last();

            // Initialize service and call method
            _tdc.VendorSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns(absorbed);
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to assert, just confirm mock)
            _tdc.WMMContext.Verify(m => m.Vendors, Times.Exactly(2));
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Exactly(2)); // TODO Need to return to this and settle on a single save call
            _tdc.VendorSet.Verify(m => m.Find(absorbed.Id), Times.Once());
            _tdc.VendorSet.Verify(m => m.Remove(absorbed), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestDeleteAbsorbVendorDoesntExist()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.CreateTestVendor(true, user.Id, false);
            Vendor absorbed = _testData.Vendors.First(v => !v.IsDefault && v.UserId == user.Id);

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert ( no need to confirm mock or assert)
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteVendorDefaultVendor()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.Vendors.First(v => v.IsDefault && v.UserId == user.Id);
            Vendor absorbed = _testData.Vendors.Where(v => v.IsDefault && v.UserId == user.Id).Skip(1).First();

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to confirm mock or assert)
        }

        [TestMethod]
        [ExpectedException(typeof (AppException))]
        public void TestDeleteAbsorbedVendorDoesntExist()
        {
            // Fabricate test
            User user = _testData.Users.First();
            Vendor absorb = _testData.Vendors.First(v => v.IsDefault && v.UserId == user.Id);
            Vendor absorbed = _testData.CreateTestVendor(true, user.Id, false);

            // Initialize service and call method
            VendorService service = new VendorService(_tdc.WMMContext.Object);
            service.DeleteVendor(absorbed.Id, absorb.Id, user.Id);

            // Confirm mock and assert (no need to confirm mock or assert)
        }
        #endregion

        #region PrivateHelpers
        private Vendor GenerateVendorWithTransactions(string vendName, Guid userId, bool isDefault)
        {
            Vendor vend = _testData.CreateTestVendor(true, userId, isDefault, vendName);
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
