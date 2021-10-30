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
        private readonly ICategoryRepository _categoryRepository;
        
        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepo)
        {
            _logger = logger;
            _categoryRepository = categoryRepo;
        }

        //Get list

        [HttpGet]
        public IActionResult GetCategories()
        {            
            try
            {
                List<CategoryModel> categories = _categoryRepository
                    .GetList(Guid.Parse(User.Identity.Name))
                    .Select(c => new CategoryModel(c)).ToList();
                return Ok(categories);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            //TODO: Additional exception catching?
        }

        [HttpPost]
        public IActionResult AddCategory([FromBody] AddCategoryModel model)
        {            
            try
            {
                var category = model.ToDB(Guid.Parse(User.Identity.Name));
                _categoryRepository.AddCategory(category);
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
                _categoryRepository.ModifyCategory(model.ToDB(userId));
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
                _categoryRepository.DeleteCategory(
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
