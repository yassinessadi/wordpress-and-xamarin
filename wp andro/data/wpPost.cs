using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wp_andro.data
{
    public class wpPost
    {
        public int id;
        public Rendered title { get; set; }
        public Rendered content { get; set; }
        public Rendered excerpt { get; set; }
        public int featured_media { get; set; }
    }
}