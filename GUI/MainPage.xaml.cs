namespace GUI
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            //Start of columns
            for (int i = 64; i <= 90; i++)
            {
                if (i == 64)
                {
                    TopLabels.Add(
               new Border
               {
                   Stroke = Color.FromRgb(0, 0, 0),
                   StrokeThickness = 1,
                   HeightRequest = 20,
                   WidthRequest = 75,
                   HorizontalOptions = LayoutOptions.Center,
                   Content =
                       new Label
                       {
                           Text = "",
                           BackgroundColor = Color.FromRgb(200, 200, 250),
                           HorizontalTextAlignment = TextAlignment.Center
                       }
               }
               );
                    continue;

                }
                char label = (char)i;
                TopLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Center,
                    Content =
                        new Label
                        {
                            Text = $"{label}",
                            BackgroundColor = Color.FromRgb(200, 200, 250),
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }
            //Start of rows
            for (int i = 1; i <= 99; i++)
            {

                LeftLabels.Add(
                new Border
                {
                    Stroke = Color.FromRgb(0, 0, 0),
                    StrokeThickness = 1,
                    HeightRequest = 20,
                    WidthRequest = 75,
                    HorizontalOptions = LayoutOptions.Start,
                    Content =
                        new Label
                        {
                            Text = $"{i}",
                            BackgroundColor = Color.FromRgb(200, 200, 250),
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                }
                );
            }

        }
        void FileMenuNew(object sender, EventArgs e)
        {
        }

        void FileMenuOpenAsync(object sender, EventArgs e)
        {

        }

    }

}
