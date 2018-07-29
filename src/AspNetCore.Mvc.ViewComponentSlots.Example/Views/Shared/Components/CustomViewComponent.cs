using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Mvc.ViewComponentSlots.Example.Views.Shared.Components
{
    public class CustomViewComponent : ViewComponent
    {
        private const string _view = "~/Views/Shared/Components/CustomViewComponent.cshtml";

        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View(_view, this));
        }
    }
}