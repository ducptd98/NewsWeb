using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using News.Models;

namespace News.Ultilities
{
    [HtmlTargetElement("ul",Attributes = "page-model")]
    public class PagingLinkTagHelper:TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public PagingLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }

        public string PageValue { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("ul");

            for (int i = 1; i <= PageModel.totalPage; i++)
            {
                TagBuilder tagli = new TagBuilder("li");
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.urlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                //tag.Attributes["asp-route-id"] = PageValue;
                //tag.Attributes["asp-action"] = PageAction;
                if (PageClassesEnabled)
                {
                    tagli.AddCssClass(i == PageModel.Curpage ? PageClassSelected : PageClass);
                    tag.AddCssClass(PageClassNormal);
                }

                tag.InnerHtml.Append(i.ToString());
                tagli.InnerHtml.AppendHtml(tag);
                result.InnerHtml.AppendHtml(tagli);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
