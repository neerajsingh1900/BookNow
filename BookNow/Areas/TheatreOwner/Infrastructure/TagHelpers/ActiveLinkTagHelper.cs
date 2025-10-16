using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace BookNow.Web.Areas.TheatreOwner.Infrastructure.TagHelpers
{
    /// <summary>
    /// Custom Tag Helper to automatically add an 'active' CSS class to a navigation element 
    /// if its target controller/action matches the current request route.
    /// Usage: <li is-active-link asp-area="TheatreOwner" asp-controller="Theatre" asp-action="Index">...</li>
    /// </summary>
    [HtmlTargetElement(Attributes = "is-active-link")]
    public class ActiveLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public ActiveLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        // Must be supplied for all Tag Helpers that interact with MVC ViewContext
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        // Input properties matching standard ASP.NET Core MVC routing attributes
        [HtmlAttributeName("asp-controller")]
        public string? Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string? Action { get; set; }

        [HtmlAttributeName("asp-area")]
        public string? Area { get; set; }

        [HtmlAttributeName("active-class")]
        public string ActiveClass { get; set; } = "active"; // Default class name

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Do not render the "is-active-link" attribute in the final HTML
            output.Attributes.RemoveAll("is-active-link");

            // Get the route data from the current request
            RouteValueDictionary routeData = ViewContext.RouteData.Values;

            // 1. Get current route values
            string? currentController = routeData["controller"]?.ToString();
            string? currentAction = routeData["action"]?.ToString();
            string? currentArea = routeData["area"]?.ToString();

            // 2. Normalize input values (use current if not explicitly set on the tag)
            string targetController = Controller ?? currentController ?? "";
            string targetAction = Action ?? currentAction ?? "";
            string targetArea = Area ?? currentArea ?? "";

            // 3. Perform comparison (case-insensitive)
            bool isControllerMatch = currentController?.Equals(targetController, StringComparison.OrdinalIgnoreCase) ?? false;
            bool isActionMatch = currentAction?.Equals(targetAction, StringComparison.OrdinalIgnoreCase) ?? false;
            bool isAreaMatch = currentArea?.Equals(targetArea, StringComparison.OrdinalIgnoreCase) ?? false;

            // 4. Check if the link should be active
            // We check the Area, then the Controller, and finally the Action.
            if (isAreaMatch && isControllerMatch && isActionMatch)
            {
                // If the element already has a class attribute, append the active class.
                if (output.Attributes.ContainsName("class"))
                {
                    string existingClasses = output.Attributes["class"].Value.ToString() ?? "";
                    output.Attributes.SetAttribute("class", existingClasses + " " + ActiveClass);
                }
                else
                {
                    // Otherwise, just set the class attribute
                    output.Attributes.Add("class", ActiveClass);
                }
            }
        }
    }
}
