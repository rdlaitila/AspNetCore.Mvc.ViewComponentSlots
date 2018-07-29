using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    internal class SlotPlacement
    {
        internal string Selector {get; set;}

        internal string TagName {get; set;}

        internal TagHelperAttributeList Attributes {get; set;}

        internal string Content {get; set;}
    }
}