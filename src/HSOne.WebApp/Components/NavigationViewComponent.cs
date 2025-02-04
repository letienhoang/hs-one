using HSOne.Core.Repositories;
using HSOne.Core.SeedWorks;
using HSOne.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public NavigationViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _unitOfWork.PostCategories.GetAllAsync();
            var navItems = model.Select(x => new NavigationItemViewModel
            {
                Name = x.Name,
                Slug = x.Slug,
                Children = model.Where(y => y.ParentId == x.Id).Select(y => new NavigationItemViewModel
                {
                    Name = y.Name,
                    Slug = y.Slug
                }).ToList()
            }).ToList();
            return View(navItems);
        }
    }
}
