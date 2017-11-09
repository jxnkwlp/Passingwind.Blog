using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace Passingwind.Blog.Web.Services
{
    public class FormRequiredAttribute : ActionMethodSelectorAttribute
    {
        private string[] _keys;

        public FormRequiredAttribute(params string[] keys)
        {
            if (keys != null)
                _keys = keys.ToArray();
            else
                _keys = new string[] { };
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            throw new NotImplementedException();
        }
    }
}
