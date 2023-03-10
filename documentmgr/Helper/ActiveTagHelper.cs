using documentmgr.business.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace documentmgr.Helpers
{
    [HtmlTargetElement(Attributes = AttributeName)]
    [HtmlTargetElement(Attributes = MatchController)]
    public class ActiveTagHelper : TagHelper
    {

        private const string AttributeName = "active-when";
        private const string MatchController = "active-when-controller";

        [HtmlAttributeName(AttributeName)]
        public string ActiveWhen { get; set; }

        [HtmlAttributeName(MatchController)]
        public string ActiveWhenController { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ActiveWhen == null && ActiveWhenController == null)
                return;

            bool controllerOnly = ActiveWhen.IsEmpty() && ActiveWhenController.IsNotEmpty();
            var segments = controllerOnly ? new string[] { } : ActiveWhen.Split("/");

            string targetController = controllerOnly ? ActiveWhenController.ToLower() : segments[1].ToLower();
            string targetAction = segments.Length > 1 ? segments[2].ToLower() : "";

            var currentController = (ViewContextData.RouteData.Values["controller"]?.ToString() ?? "").ToLower();
            var currentAction = (ViewContextData.RouteData.Values["action"]?.ToString() ?? "").ToLower();

            if (currentController.Equals(targetController) && (controllerOnly || currentAction.Equals(targetAction)))
            {
                if (output.Attributes.ContainsName("class"))
                {
                    output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} active");
                }
                else
                {
                    output.Attributes.SetAttribute("class", "active");
                }
            }
        }
    }
}