using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using GirlyMerely.Core.Models;
using GirlyMerely.Core.Utility;
using GirlyMerely.Infrastructure;
using GirlyMerely.Web.Models;
using Microsoft.AspNet.Identity;

namespace GirlyMerely.Web.Providers
{
    public class CustomerAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Customer/Auth/Login/?returnUrl=/Shop/Checkout");
        }
        //public virtual void HandleAuthorizedRequest(AuthorizationContext filterContext)
        //{
        //    if (filterContext == null)
        //    {
        //        throw new ArgumentNullException("filterContext");
        //    }

        //    if (AuthorizeCore(filterContext.HttpContext))
        //    {
        //        // ** IMPORTANT **
        //        // Since we're performing authorization at the action level, the authorization code runs
        //        // after the output caching module. In the worst case this could allow an authorized user
        //        // to cause the page to be cached, then an unauthorized user would later be served the
        //        // cached page. We work around this by telling proxies not to cache the sensitive page,
        //        // then we hook our custom authorization code into the caching mechanism so that we have
        //        // the final say on whether a page should be served from the cache.
        //        var userId = filterContext.HttpContext.User.Identity.GetUserId();
        //        using (var _context = new MyDbContext())
        //        {
        //            var userIsCustomer = _context.UserRoles.Any(ur =>
        //                ur.UserId == userId && ur.RoleId == StaticVariables.CustomerRoleId);
        //            if (userIsCustomer == false)
        //            {
        //                filterContext.Result = new RedirectResult("/Customer/Auth/Login");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        HandleUnauthorizedRequest(filterContext);
        //    }
        //}
    }

}