using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize]
    //[Authorize(Policy = "Admin")] 
    public abstract class AdminControllerBase : Controller
    {
        protected const int TableListItem = 12;


        protected void AlertSuccess(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                TempData["alert-message"] = message;
                TempData["alert-type"] = "success";
            }
        }

        protected void AlertError(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                TempData["alert-message"] = message;
                TempData["alert-type"] = "danger";
            }
        }

    }
}
