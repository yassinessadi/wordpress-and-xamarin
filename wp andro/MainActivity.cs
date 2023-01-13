using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.SwipeRefreshLayout.Widget;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using wp_andro.Adapters;
using wp_andro.data;

namespace wp_andro
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity,SwipeRefreshLayout.IOnRefreshListener
    {
        List<wpPost> _posts;
        RecyclerView PostRecycler;
        postsAdapter postsadapters;
        SwipeRefreshLayout PullToRefresh;
        int counter = 1;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            PostRecycler = (RecyclerView)FindViewById(Resource.Id.postrecycleview);
            PullToRefresh = (SwipeRefreshLayout)FindViewById(Resource.Id.PullToRefresh);
            wp(10);
            PullToRefresh.SetOnRefreshListener(this);
        }
        public async void wp( int number)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://le12.ma");
                var res = await client.GetAsync("/wp-json/wp/v2/posts?per_page="+number);
                if (res.IsSuccessStatusCode)
                {
                    string postText = await res.Content.ReadAsStringAsync();
                    var posts = JsonConvert.DeserializeObject<List<wpPost>>(postText);
                    _posts = new List<wpPost>(posts);
                    //foreach (var item in posts)
                    //{
                    //    _posts.Add(item);
                    //}
                    PostRecycler.SetLayoutManager(new LinearLayoutManager(PostRecycler.Context));
                    postsadapters = new postsAdapter(_posts);
                    PostRecycler.SetAdapter(postsadapters);
                }
            }
        }

        void SwipeRefreshLayout.IOnRefreshListener.OnRefresh()
        {
            wp(counter++);
            PullToRefresh.Refreshing = false;
        }
    }
}