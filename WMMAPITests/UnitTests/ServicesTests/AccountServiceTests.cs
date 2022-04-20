using WMMAPI.Services.AccountServices;
using WMMAPI.Services.AccountServices.AccountModels;

namespace WMMAPITests.UnitTests
{
    [TestClass]
    public class AccountServiceTests
    {
        private TestData _testData;
        private TestDataContext _tdc;
        
        [TestInitialize]
        public void Init()
        {
            _testData = new TestData();            
            _tdc = new TestDataContext(_testData);
        }

        #region Get
        [DataTestMethod]
        [DataRow(false, "10.00|credit;25.25|credit;75.25|debit;24.75|debit", 64.75)]
        [DataRow(true, "75.25|debit;24.75|debit", -100.00)]
        [DataRow(false, "75.25|debit;24.75|debit", 100.00)]
        [DataRow(true, "10.00|credit;25.25|credit", 35.25)]
        [DataRow(false, "10.00|credit;25.25|credit", -35.25)]
        public void TestGetSucceeds(bool isAsset, string trans, double expectedBalance)
        {
            // Create test account with transactions
            Account testAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = "TestingGetAccount",
                UserId = _testData.Users.First().Id,
                IsAsset = isAsset,
                IsActive = true
            };
            _testData.Accounts = _testData.Accounts.Concat( new List<Account> { testAccount });
            GenerateMockTrans(trans.Split(';'), testAccount);
                        
            // Arrange; Need to update the dbsets with the new data
            _tdc = new(_testData);
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            AccountModel result = service.Get(testAccount.Id, testAccount.UserId);
            
            // Assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Exactly(2));
            Assert.AreEqual(testAccount.Id, result.Id);
            Assert.AreEqual(testAccount.Name, result.Name);
            Assert.AreEqual((decimal)expectedBalance, result.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetAccountNotFound()
        {
            // Fabricate Test
            Guid userId = Guid.NewGuid();
            Guid accountId = Guid.NewGuid();

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.Get(userId, accountId);

            // Confirm mock and assert (not needed, exception expected)
        }

        [TestMethod] 
        public void TestGetListSucceeds()
        {
            // Create test account with transactions
            User testUser = _testData.CreateTestUser();
            List<Account> accounts = new List<Account>();
            for (int i = 0; i < 4; i++)
            {
                Account testAccount = new Account
                {
                    Id = Guid.NewGuid(),
                    Name = $"TestingGetAccount{i}",
                    UserId = testUser.Id,
                    IsAsset = i % 2 == 0,
                    IsActive = true
                };
                accounts.Add(testAccount);
            }
            _testData.Accounts = _testData.Accounts.Concat(accounts);

            foreach (var account in accounts)
            {
                GenerateMockTrans("10.00|credit;25.25|credit".Split(";"), account);
            }

            // Arrange; Need to update the dbsets with the new data
            _tdc = new(_testData);
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            ICollection<AccountModel> results = service.GetList(testUser.Id);

            // Assert
            _tdc.WMMContext.Verify(m => m.Accounts, Times.Once());
            _tdc.WMMContext.Verify(m => m.Transactions, Times.Exactly(8)); // TODO: change the get balance method to get balances to reduce calls to db
            foreach (var result in results)
            {
                Assert.IsTrue(accounts.Any(a => a.Name == result.Name));                
                Assert.AreEqual(result.IsAsset ? (decimal)35.25 : (decimal)-35.25, result.Balance);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestGetListFails()
        {
            // Fabricate test
            Guid userId = Guid.NewGuid();

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.GetList(userId);

            // Confirm mock and assert (not needed, exception expected)
        }
        #endregion

        #region Add
        [TestMethod]
        public void TestAddAccountSucceeds()
        {
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            Guid userId = _testData.Users.First().Id;

            service.AddAccount(_testData.CreateTestAccount(userId), 500M);

            _tdc.AccountSet.Verify(m => m.Add(It.IsAny<Account>()), Times.Once());
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestAddAccountNameExists()
        {
            // Fabricate test
            Guid userId = _testData.Users.First().Id;
            Account account = _testData.CreateTestAccount(userId);
            account.Name = _testData.Accounts.First(a => a.UserId == userId).Name;

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.AddAccount(account, 500M);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        [ExpectedException(typeof(AppException))]
        public void TestAddAccountNameNullOrWhiteSpace(string name)
        {
            // Fabricate test
            Guid userId = _testData.Users.First().Id;
            Account account = _testData.CreateTestAccount(userId);
            account.Name = name;

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.AddAccount(account, 500M);
        }
        #endregion

        #region Modify
        [TestMethod]
        public void TestModifyAccountSucceeds()
        {
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            Account dbAccount = _testData.Accounts.First();
            Account testingAccount = new Account
            {
                Name = "testAccount",
                Id = dbAccount.Id,
                UserId = dbAccount.UserId
            };

            service.ModifyAccount(testingAccount);
            _tdc.WMMContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyAccountFailsDueToNotExisting()
        {
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ModifyAccount(_testData.CreateTestAccount());
        }

        [TestMethod]
        [ExpectedException(typeof(AppException))]
        public void TestModifyAccountFailsNameExists()
        {
            // Fabricate test
            Guid userId = _testData.Users.First().Id;
            Account account = _testData.Accounts.First(a => a.UserId == userId);
            account.Name = _testData.Accounts.Last(a => a.UserId == userId).Name;

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ModifyAccount(account);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        [ExpectedException(typeof(AppException))]
        public void TestModifyAccountFailsNameNull(string name)
        {
            // Fabricate test
            Guid userId = _testData.Users.First().Id;
            Account account = _testData.Accounts.First(a => a.UserId == userId);
            account.Name = name;

            // Initialize service and call method
            IAccountService service = new AccountService(_tdc.WMMContext.Object);
            service.ModifyAccount(account);
        }
        #endregion

        #region private methods        
        private void GenerateMockTrans(string[] transStructure, Account account)
        {
            List<Transaction> transList = new();
            foreach (var split in transStructure)
            {
                var tran = split.Split('|');
                transList.Add(
                    _testData.CreateTestTransaction(
                        account, tran[1] == "debit", decimal.Parse(tran[0]), Guid.NewGuid(), Guid.NewGuid()));
            }
                        
            _testData.Transactions = _testData.Transactions.Concat(transList);
        }
        #endregion
    }
}
