using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Components
{
    public class NavigationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
