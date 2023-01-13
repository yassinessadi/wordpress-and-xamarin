using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using wp_andro.data;


namespace wp_andro.Adapters
{
    internal class postsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<postsClickEventArgs> ItemClick;
        public event EventHandler<postsClickEventArgs> ItemLongClick;
        List<wpPost> posts;

        public postsAdapter(List<wpPost> data)
        {
            posts = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);
            var postview = Resource.Layout.postview;
            itemView = LayoutInflater.From(parent.Context).Inflate(postview, parent, false);

            var vh = new postsViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override async void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = posts[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as postsViewHolder;
            //holder.TextView.Text = items[position];
            holder.PostTitle.Text =  item.title.rendered;
            var content = Regex.Replace(item.excerpt.rendered, "<.*?>", String.Empty);
            if (content.Length > 110)
            {
                holder.PostExcerpt.Text = content.Substring(0, 110) + "...";
            }
            else
            {
                holder.PostExcerpt.Text = content + "...";
            }
            var img = item.featured_media;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://le12.ma");
                var res = await client.GetAsync($"/wp-json/wp/v2/media/{img}");
                if (res.IsSuccessStatusCode)
                {
                    string postText = await res.Content.ReadAsStringAsync();
                    var media = JObject.Parse(postText);
                    var link = media["guid"]["rendered"].ToString();
                    GetImage(link,holder.PostImg);
                }
            }
        }
        void GetImage(string url,ImageView postimg)
        {
            ImageService.Instance.LoadUrl(url)
                .Retry(3, 200)
                .DownSample(400, 400)
                .Into(postimg);

        }

        public override int ItemCount => posts.Count;

        void OnClick(postsClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(postsClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }
    public class postsViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView PostTitle { get; set; }
        public TextView PostExcerpt { get;set; }
        public ImageView PostImg { get; set; }


        public postsViewHolder(View itemView, Action<postsClickEventArgs> clickListener,
                            Action<postsClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            PostTitle = (TextView)itemView.FindViewById(Resource.Id.PostTitle);
            PostExcerpt = (TextView)itemView.FindViewById(Resource.Id.PostExcerpt);
            PostImg = (ImageView)itemView.FindViewById(Resource.Id.PostImg);

            itemView.Click += (sender, e) => clickListener(new postsClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new postsClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class postsClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}