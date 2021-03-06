using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Services.CategoryServices.CategoryModels;
using static WMMAPI.Helpers.ClaimsHelpers;
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
        }

        [HttpGet]
        public IActionResult GetCategories()
        {
            try
            {
                UserId = GetUserId(UserId, User);

                IList<CategoryModel> categories = _categoryService.GetList(UserId);
                return Ok(categories);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryModel), StatusCodes.Status201Created)]
        public IActionResult AddCategory([FromBody] AddCategoryModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);

                var category = model.ToDB(UserId);
                _categoryService.AddCategory(category);
                return StatusCode(StatusCodes.Status201Created, new CategoryModel(category));
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ModifyCategory([FromBody] UpdateCategoryModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);

                _categoryService.ModifyCategory(model.ToDB(UserId));
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(new ExceptionResponse(GenericErrorMessage));
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteCatgory([FromBody] DeleteCategoryModel model)
        {
            try
            {
                UserId = GetUserId(UserId, User);

                _categoryService.DeleteCategory(
                    model.AbsorbedId,
                    model.AbsorbingId,
                    UserId);
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (AppException ex)
            {
                return BadRequest(new ExceptionResponse(ex.Message));                
            }
            catch (UnauthorizedAccessException ex)
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
