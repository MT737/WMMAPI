using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using WMMAPI.Controllers;
using WMMAPI.Database.Entities;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.CategoryModels;
using WMMAPITests.DataHelpers;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPITests.UnitTests.ControllerTests
{
    [TestClass]
    public class CategoryControllerTests
    {
        private TestData _td;
        private Mock<ILogger<CategoryController>> _mockLogger;
        private Mock<ICategoryService> _mockCategoryService;

        [TestInitialize]
        public void Initialize()
        {
            _td = new TestData();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _mockCategoryService = new Mock<ICategoryService>();
        }


        #region Get
        [TestMethod]
        public void GetShouldReturnViewModel()
        {
            // Arrange test
            var userId = Guid.NewGuid();
            var testCategories = GenerateTestCategoryModels(userId);
            _mockCategoryService.Setup(m => m.GetList(userId)).Returns(testCategories);
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetCategories();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(IList<CategoryModel>));
            var categories = (IList<CategoryModel>)obj.Value;
            Assert.AreEqual(5, categories.Count);
        }

        [TestMethod]
        public void GetAppExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockCategoryService.Setup(m => m.GetList(userId)).Throws(new AppException());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetCategories();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void GetExceptionHandled()
        {
            // Arrange test
            Guid userId = Guid.NewGuid();
            _mockCategoryService.Setup(m => m.GetList(userId)).Throws(new Exception());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = userId;

            // Call action
            var result = controller.GetCategories();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void GetEmptyUserGuidHandled()
        {
            // Arrange test
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.GetCategories();

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Add
        [TestMethod]
        public void AddShouldReturnViewModel()
        {
            // Arrange test
            AddCategoryModel model = GenerateAddCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.AddCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var obj = (OkObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(CategoryModel));
            var categoryModel = (CategoryModel)obj.Value;
            Assert.AreEqual(model.Name, categoryModel.Name);
        }

        [TestMethod]
        public void AddAppExceptionHandled()
        {
            // Arrange test
            AddCategoryModel model = GenerateAddCategoryModel();
            _mockCategoryService.Setup(m => m.AddCategory(It.IsAny<Category>())).Throws(new AppException());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.AddCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void AddExceptionHandled()
        {
            // Arrange test
            AddCategoryModel model = GenerateAddCategoryModel();
            _mockCategoryService.Setup(m => m.AddCategory(It.IsAny<Category>())).Throws(new Exception());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.AddCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void AddEmptyUserGuidHandled()
        {
            // Arrange test
            AddCategoryModel model = GenerateAddCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.AddCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Modify
        [TestMethod]
        public void ModifyShouldReturnEmptyOk()
        {
            // Arrange test
            Category category = _td.CreateTestCategory(true);
            CategoryModel model = new CategoryModel(category);
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void ModifyAppExceptionHandled()
        {
            // Arrange test
            Category category = _td.CreateTestCategory(true);
            CategoryModel model = new CategoryModel(category);
            _mockCategoryService.Setup(m => m.ModifyCategory(It.IsAny<Category>())).Throws(new AppException());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void ModifyExceptionHandled()
        {
            // Arrange test
            Category category = _td.CreateTestCategory(true);
            CategoryModel model = new CategoryModel(category);
            _mockCategoryService.Setup(m => m.ModifyCategory(It.IsAny<Category>())).Throws(new Exception());
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.ModifyCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void ModifyEmptyUserGuidHandled()
        {
            // Arrange test
            Category category = _td.CreateTestCategory(true);
            CategoryModel model = new CategoryModel(category);
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.Empty;

            // Call action
            var result = controller.ModifyCategory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteShouldReturnEmptyOk()
        {
            // Arrange test
            DeleteCategoryModel model = GenerateDeleteCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();

            // Call action
            var result = controller.DeleteCatgory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public void DeleteAppExceptionHandled()
        {
            // Arrange test
            DeleteCategoryModel model = GenerateDeleteCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();
            _mockCategoryService.Setup(m => m.DeleteCategory(model.AbsorbedId, model.AbsorbingId, controller.UserId))
                .Throws(new AppException());

            // Call action
            var result = controller.DeleteCatgory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual("Exception of type 'WMMAPI.Helpers.AppException' was thrown.", resp.Message);
        }

        [TestMethod]
        public void DeleteExceptionHandled()
        {
            // Arrange test
            DeleteCategoryModel model = GenerateDeleteCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.NewGuid();
            _mockCategoryService.Setup(m => m.DeleteCategory(model.AbsorbedId, model.AbsorbingId, controller.UserId))
                .Throws(new Exception());

            // Call action
            var result = controller.DeleteCatgory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(GenericErrorMessage, resp.Message);
        }

        [TestMethod]
        public void DeleteEmptyUserGuidHandled()
        {
            // Arrange test
            DeleteCategoryModel model = GenerateDeleteCategoryModel();
            CategoryController controller = new(_mockLogger.Object, _mockCategoryService.Object);
            controller.UserId = Guid.Empty;            

            // Call action
            var result = controller.DeleteCatgory(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var obj = (BadRequestObjectResult)result;
            Assert.IsInstanceOfType(obj.Value, typeof(ExceptionResponse));
            var resp = (ExceptionResponse)obj.Value;
            Assert.AreEqual(AuthenticationError, resp.Message);
        }
        #endregion

        #region TestHelpers
        private IList<CategoryModel> GenerateTestCategoryModels(Guid userId)
        {
            IList<CategoryModel> models = new List<CategoryModel>();
            for (int i = 0; i < 5; i++)
            {
                models.Add(new CategoryModel(_td.CreateTestCategory(true, userId: userId)));
            }
            return models;
        }

        private AddCategoryModel GenerateAddCategoryModel()
        {
            return new AddCategoryModel
            {
                Name = "TestCategoryName"
            };
        }

        private DeleteCategoryModel GenerateDeleteCategoryModel()
        {
            return new DeleteCategoryModel
            {
                AbsorbedId = Guid.NewGuid(),
                AbsorbingId = Guid.NewGuid()
            };
        }
        #endregion
    }
}
