using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public class MenuViewModel
    {
        public string DisplayName { get; set; }

        public string Url { get; set; }

        public string[] Keys { get; set; }

        public bool IsDivider { get; private set; }

        public IList<MenuViewModel> Childrens { get; }

        public MenuViewModel(string displayName, string url, params string[] keys)
        {
            this.DisplayName = displayName;
            this.Url = url;
            this.Keys = keys;

            this.Childrens = new List<MenuViewModel>();
        }

        protected MenuViewModel()
        {

        }

        public MenuViewModel AddChildrenMenu(MenuViewModel menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            this.Childrens.Add(menu);

            return this;
        }

        public MenuViewModel AddDivider()
        {
            return new MenuViewModel() { IsDivider = true };
        }

        public MenuViewModel AddChildrenMenu(string displayName, string url, params string[] keys)
        {
            return AddChildrenMenu(new MenuViewModel(displayName, url, keys));
        }







        public static IList<MenuViewModel> GetAllMenus()
        {
            var list = new List<MenuViewModel>() {
                 new MenuViewModel("New", "")
                    .AddChildrenMenu("Post", "")
                    .AddChildrenMenu("Page", "")
                    .AddDivider()
                    .AddChildrenMenu("Category", ""),

                 new MenuViewModel("Post", "")
                    .AddChildrenMenu("List", "")
                    .AddChildrenMenu("Comments", "")
                    .AddDivider()
                    .AddChildrenMenu("New", ""),

                 new MenuViewModel("Post", "")
                    .AddChildrenMenu("List", "")
                    .AddDivider()
                    .AddChildrenMenu("New", ""),

                 new MenuViewModel("Category", "")
                    .AddChildrenMenu("List", "")
                    .AddDivider()
                    .AddChildrenMenu("New", ""),

                 new MenuViewModel("Tags",""),

                 new MenuViewModel("Users",""),

                 new MenuViewModel("Roles",""),

                 new MenuViewModel("Setting", "")
                    .AddChildrenMenu("Basic", ""),
        };



            return null;
        }

    }
}
