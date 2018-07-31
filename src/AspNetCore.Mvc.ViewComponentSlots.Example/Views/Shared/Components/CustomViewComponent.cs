using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Mvc.ViewComponentSlots.Example.Views.Shared.Components
{
    public class CustomViewComponent : ViewComponent
    {
        public string Foo {get; set;}

        public float Bar {get; set;}

        private const string _view = "~/Views/Shared/Components/CustomViewComponent.cshtml";

        public Task<IViewComponentResult> InvokeAsync(string foo = "default", float bar = 1.0f)
        {
            Foo = foo;
            Bar = bar;

            return Task.FromResult<IViewComponentResult>(View(_view, this));
        }
    }
}