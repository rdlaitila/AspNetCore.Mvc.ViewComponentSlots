using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    internal class SlotContext
    {
        internal List<SlotPlacement> Placements = new List<SlotPlacement>();
    }
}