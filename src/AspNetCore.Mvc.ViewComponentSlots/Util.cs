using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AspNetCore.Mvc.ViewComponentSlots
{
    public static class Util
    {
        internal static string KebabToPascal(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return string.Join(
                string.Empty,
                str.Split("-")
                    .Select(a => char.ToUpper(a[0]) + a.Substring(1))
                    .ToArray()
            );
        }

        internal static IDictionary<string, object> ExtractComponentArgs(ParameterInfo[] parameters, IDictionary<string, object> args)
        {
            var result = new Dictionary<string, object>();

            foreach(var @param in parameters)
            {
                var arg = args.FirstOrDefault(kv =>
                    KebabToPascal(kv.Key).ToLower() == @param.Name.ToLower()
                );

                if (arg.Value == null) continue;

                result[@param.Name] = arg.Value;
            }

            return result;
        }

        internal static string ExtractSlotSelector(TagHelperContext context, string selectorName)
        {
            var attr = context
                .AllAttributes
                .FirstOrDefault(a => a.Name.ToLower() == selectorName);

            if (attr == null)
                return string.Empty;

            if (attr.Value.GetType() != typeof(HtmlString))
                return string.Empty;

            var attrValue = ((HtmlString)attr.Value).Value;

            if (string.IsNullOrWhiteSpace(attrValue))
                return string.Empty;

            return attrValue;
        }
    }
}