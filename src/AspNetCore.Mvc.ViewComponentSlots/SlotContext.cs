using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    internal class SlotContext
    {
        internal List<SlotPlacement> SlotPlacements = new List<SlotPlacement>();
    }
}