using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Creative.Web.Models;

namespace Creative.Web.Views.Resources
{
    public class IndexViewModel
    {
        public Song[] Songs { get; set; }
    }
}