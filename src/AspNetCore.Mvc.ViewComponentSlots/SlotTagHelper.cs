using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    [HtmlTargetElement("vc:slot")]
    public class SlotTagHelper : TagHelper
    {
        internal static Stack<SlotContext> Contexts = new Stack<SlotContext>();

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var selector = Util.ExtractSlotSelector(context, "name");
            var slotPlacement = Contexts.Peek().SlotPlacements.FirstOrDefault(a => a.Selector == selector);

            if (slotPlacement != null)
            {
                output.TagName = slotPlacement.TagName;
                output.Attributes.Clear();

                slotPlacement.Attributes.ToList().ForEach(attr =>
                    output.Attributes.Add(attr.Name, attr.Value)
                );

                output.Content.SetHtmlContent(slotPlacement.Content);
                return;
            }

            // default slot content if no pending slot placement is present
            output.TagName = null;
            output.Content.SetHtmlContent(
                (await output.GetChildContentAsync()).GetContent()
            );
        }
    }
}