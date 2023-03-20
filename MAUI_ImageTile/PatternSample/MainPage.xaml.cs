
using Microsoft.Maui.Graphics;

namespace PatternSample
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            IPattern pattern;
            // Create a 10x10 template for the pattern
            using (PictureCanvas picture = new PictureCanvas(0, 0, 100, 100))
            {
                picture.StrokeColor = Colors.Silver;
                picture.DrawLine(0, 0, 100, 100);
                picture.DrawLine(0, 100, 100, 0);
                pattern = new PicturePattern(picture.Picture, 100, 100);
            }

            // Fill the rectangle with the 10x10 pattern
            PatternPaint patternPaint = new PatternPaint
            {
                Pattern = pattern
            };

            canvas.SetFillPattern()
            //StackLayout.SetFillPaint(patternPaint, RectF.Zero);
            //canvas.FillRectangle(10, 10, 250, 250);
        }
    }
}