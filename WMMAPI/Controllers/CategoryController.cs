using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.CategoryModels;
using static WMMAPI.Helpers.Globals.ErrorMessages;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public Guid UserId { get; set; }
        
        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;

            UserId = User != null ? Guid.Parse(User.Identity.Name) : Guid.Empty;
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                IList<CategoryModel> categories = _categoryService.GetList(UserId);
                return Ok(categories);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] AddCategoryModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                var category = model.ToDB(UserId);
                _categoryService.AddCategory(category);
                return Ok(new CategoryModel(category));
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        public IActionResult ModifyCategory([FromBody] CategoryModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                _categoryService.ModifyCategory(model.ToDB(UserId));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete]
        public IActionResult DeleteCatgory([FromBody] DeleteCategoryModel model)
        {
            if (UserId == Guid.Empty)
                return BadRequest(new ExceptionResponse(AuthenticationError));

            try
            {
                _categoryService.DeleteCategory(
                    model.AbsorbedId,
                    model.AbsorbingId,
                    UserId);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));                
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }
    }
}
