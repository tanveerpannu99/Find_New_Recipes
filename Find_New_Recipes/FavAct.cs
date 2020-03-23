using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;
using System.Net;

namespace Find_New_Recipes
{
    // Set page orentation and no icon \\
    [Activity(Label = "FavAct", Theme = "@android:style/Theme.DeviceDefault.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class FavAct : Activity
    {
        // Item declaration \\
        Button GoToRec;
        TextView NameFavs;
        TextView IngredientsFavs;
        String Imageurl;
        String Recipeurl;
        ImageView ImgFavs;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Favourites);

            GoToRec = FindViewById<Button>(Resource.Id.btnToRecipe);
            NameFavs = FindViewById<TextView>(Resource.Id.txtNamefavs);
            ImgFavs = FindViewById<ImageView>(Resource.Id.imgFavs);
            IngredientsFavs = FindViewById<TextView>(Resource.Id.txtIngredientsfavs);

            NameFavs.Text = Intent.GetStringExtra("Name");
            IngredientsFavs.Text = Intent.GetStringExtra("Ingredients");
            Recipeurl = Intent.GetStringExtra("Recipe");
            Imageurl = Intent.GetStringExtra("Image");
            ImgFavs.SetImageBitmap(GetImageBitmapFromUrl(Imageurl));

            // Click events \\
            GoToRec.Click += GoToRec_Click;
        }

        private void GoToRec_Click(object sender, EventArgs e)
        {
            var uri = Android.Net.Uri.Parse(Recipeurl);
            var i = new Intent(Intent.ActionView, uri);
            StartActivity(i);
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            if (!(url == "null"))
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            return imageBitmap;
        }
    }
}
