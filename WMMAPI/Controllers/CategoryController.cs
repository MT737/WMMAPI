using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WMMAPI.Helpers;
using WMMAPI.Interfaces;
using WMMAPI.Models.CategoryModels;

namespace WMMAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;
        
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
                List<CategoryModel> categories = _categoryService
                    .GetList(Guid.Parse(User.Identity.Name));
                return Ok(categories);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] AddCategoryModel model)
        {            
            try
            {
                var category = model.ToDB(Guid.Parse(User.Identity.Name));
                _categoryService.AddCategory(category);
                return Ok(new CategoryModel(category));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult ModifyCategory([FromBody] CategoryModel model)
        {
            try
            {
                Guid userId = Guid.Parse(User.Identity.Name);
                _categoryService.ModifyCategory(model.ToDB(userId));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }            
        }

        [HttpDelete]
        public IActionResult DeleteCatgory([FromBody] DeleteCategoryModel model)
        {
            try
            {
                _categoryService.DeleteCategory(
                    model.AbsorbedId,
                    model.AbsorbingId,
                    Guid.Parse(User.Identity.Name));
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });                
            }
        }
    }
}
