using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    [HtmlTargetElement("*", Attributes="vc")]
    public class ElementTagHelper : TagHelper
    {
        public override int Order
        {
            get { return int.MinValue; }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private static readonly ReadOnlyCollection<Type> _viewComponents = AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ViewComponent).IsAssignableFrom(p))
            .ToList()
            .AsReadOnly();

        private readonly IViewComponentHelper _viewComponentHelper;

        public ElementTagHelper(IViewComponentHelper viewComponentHelper)
        {
            _viewComponentHelper = viewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var componentName = Util.KebabToPascal(context.TagName);

            var matchingViewComponent = _viewComponents.FirstOrDefault(a =>
                a.Name == componentName ||
                a.FullName == componentName ||
                a.FullName.EndsWith(componentName)
            );

            if (matchingViewComponent == null)
            {
                output.Content.SetHtmlContent(
                    $"<span style=\"color:red;border:1px dashed red;\">view component '{componentName}' not found"
                );
                return;
            }

            SlotTagHelper.Contexts.Push(new SlotContext());

            var childContent = (await output.GetChildContentAsync()).GetContent();

            if (!string.IsNullOrWhiteSpace(childContent))
            {
                SlotTagHelper.Contexts.Peek().SlotPlacements.Add(new SlotPlacement
                {
                    Selector = string.Empty,
                    TagName = null,
                    Attributes = new TagHelperAttributeList(),
                    Content =  childContent,
                });
            }

            (_viewComponentHelper as IViewContextAware).Contextualize(ViewContext);

            output.Content.SetHtmlContent(
                await _viewComponentHelper.InvokeAsync(
                    matchingViewComponent,
                    Util.ExtractComponentArgs(context)
                )
            );

            SlotTagHelper.Contexts.Pop();
        }
    }
}