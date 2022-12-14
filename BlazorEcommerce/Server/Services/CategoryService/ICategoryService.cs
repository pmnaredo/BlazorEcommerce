﻿using BlazorEcommerce.Shared;

namespace BlazorEcommerce.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<Category>>> GetCategoriesAsync();
        Task<ServiceResponse<List<Category>>> GetAdminCategoriesAsync();
        Task<ServiceResponse<List<Category>>> AddCategory(Category category);
        Task<ServiceResponse<List<Category>>> DeleteCategory(int categoryId);
        Task<ServiceResponse<List<Category>>> UpdateCategory(Category category);
    }

}
