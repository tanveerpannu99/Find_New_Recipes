using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using SQLite;
using System.Net;
using static Find_New_Recipes.Response;

namespace Find_New_Recipes
{
    // Set page orentation and no icon \\
    [Activity(Label = "Find_New_Recipes", MainLauncher = false, Theme = "@android:style/Theme.DeviceDefault.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        string path = System.IO.Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Food.db");

        // Item declaration \\
        EditText SearchMain;
        Button btnSearchMain;
        ImageView imgFoodMain;
        TextView txtTitleMain;
        TextView txtIngredientsMain;
        Button btnDislikeMain;
        Button btnLikeMain;
        Button FavouritesMain;

        RecipesFinder food;
        RootObject root;

        int recipenum = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            txtTitleMain = FindViewById<TextView>(Resource.Id.txtTitleMain);
            txtIngredientsMain = FindViewById<TextView>(Resource.Id.txtIngredientsMain);

            imgFoodMain = FindViewById<ImageView>(Resource.Id.imgFoodMain);

            btnLikeMain = FindViewById<Button>(Resource.Id.btnLikeMain);
            btnDislikeMain = FindViewById<Button>(Resource.Id.btnDislikeMain);
            FavouritesMain = FindViewById<Button>(Resource.Id.btnFavouritesMain);

            SearchMain = FindViewById<EditText>(Resource.Id.txtSearchMain);
            btnSearchMain = FindViewById<Button>(Resource.Id.btnSearchMain);

            food = new RecipesFinder();

            // Click events \\
            btnDislikeMain.Click += BtnDislikeMain_Click;
            btnLikeMain.Click += BtnLikeMain_Click;
            FavouritesMain.Click += FavouritesMain_Click;
            btnSearchMain.Click += BtnSearchMain_Click;

            btnLikeMain.Enabled = false;
            btnDislikeMain.Enabled = false;
        }

        private void BtnSearchMain_Click(object sender, System.EventArgs e)
        {
            try
            {
                string FoodSearch = SearchMain.Text;
                RecipesFinder objrest = new RecipesFinder();
                root = objrest.ExecuteRequest(FoodSearch);
                GetRecipe();
                btnLikeMain.Enabled = true;
                btnDislikeMain.Enabled = true;
            }
            catch
            {
                Toast.MakeText(this, SearchMain.Text + " No Recipes found. Please try a new search.", ToastLength.Long).Show();
                return;
            }
        }

        private void FavouritesMain_Click(object sender, System.EventArgs e)
        {
            Intent NextActivity = new Intent(this, typeof(FavsPageAct));
            StartActivity(NextActivity);
        }

        private void BtnLikeMain_Click(object sender, System.EventArgs e)
        {
            // Setup Connection \\
            var db = new SQLiteConnection(path);

            // Setup Table \\
            db.CreateTable<Ingredients>();

            // Create New Contact \\
            Ingredients food = new Ingredients();

            food.RecipeName = txtTitleMain.Text;
            food.imageurl = root.hits[recipenum].recipe.image;
            food.Recipeurl = root.hits[recipenum].recipe.url;
            food.ingredients = txtIngredientsMain.Text;

            db.Insert(food);

            recipenum = recipenum + 1;
            GetRecipe();
        }

        private void BtnDislikeMain_Click(object sender, System.EventArgs e)
        {
            recipenum = recipenum + 1;
            GetRecipe();
        }

        public void GetRecipe()
        {
            txtTitleMain.Text = root.hits[recipenum].recipe.label;
            txtIngredientsMain.Text = "";

            foreach (var ing in root.hits[recipenum].recipe.ingredients)
            {
                txtIngredientsMain.Text += "\n" + ing.text;
            }

            string imgurl = root.hits[recipenum].recipe.image;

            imgFoodMain.SetImageBitmap(GetImageBitmapFromUrl(imgurl));
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
