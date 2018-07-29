using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    [HtmlTargetElement("*", Attributes="vc:slot")]
    public class SlotSelectorTagHelper : TagHelper
    {
        public override int Order
        {
            get { return int.MaxValue; }
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var selector = Util.ExtractSlotSelector(context, "vc:slot");

            if (selector == string.Empty)
                return;

            var attrs = new TagHelperAttributeList(
                context.AllAttributes.Where(a => a.Name != "vc:slot").ToArray()
            );

            SlotTagHelper.Contexts.Peek().SlotPlacements.Add(new SlotPlacement
            {
                Selector = selector,
                TagName = context.TagName,
                Attributes = attrs,
                Content = (await output.GetChildContentAsync()).GetContent(),
            });

            output.SuppressOutput();
        }
    }
}