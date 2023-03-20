using Microsoft.Maui.Controls.Shapes;
using static Android.Graphics.Shader;

namespace MAUI_ImageTile;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    private int _version;
    private ImageBrush _imageBrush;
    public double DpiX { get; set; }
    public double DpiY { get; set; }

    public double ScaleX { get; set; }
    public double ScaleY { get; set; }

    public Size ImageSize { get; set; }

    public Size ShapeSize { get; set; }

    public double DpiScaleX => DefaultDpi / DpiX;

    public double DpiScaleY => DefaultDpi / DpiY;

    private const double DefaultDpi = 96;

    public ImageTile(ShapeBase shape)
    {
        InitializeComponent();
        _imageBrush = shape.Fill.Clone() as ImageBrush;

        InitOffsetSlider(OffsetXSliderSetter.Slider);
        InitOffsetSlider(OffsetYSliderSetter.Slider);
        InitScaleSlider(ScaleXSliderSetter.Slider);
        InitScaleSlider(ScaleYSliderSetter.Slider);
        InitComboBox();

        OffsetXSliderSetter.Slider.ValueChanged += (sender, args) => UpdateImageBrush(null);
        OffsetYSliderSetter.Slider.ValueChanged += (sender, args) => UpdateImageBrush(null);
        ScaleXSliderSetter.Slider.ValueChanged += (sender, args) => UpdateImageBrush(null);
        ScaleYSliderSetter.Slider.ValueChanged += (sender, args) => UpdateImageBrush(null);
        AlignmentComboBox.SelectionChanged += (sender, args) => UpdateImageBrush(null);
        FlipTypeComboBox.SelectionChanged += (sender, args) => UpdateImageBrush(null);

        var imageFilePath = _imageBrush.ImageSource.ToString().Split("///").Last();

        //file:///C:/Users/11019/Pictures/4K图片.jpg


        InitImageTile(imageFilePath, shape);

    }

    private void InitScaleSlider(Slider slider)
    {
        slider.Minimum = 0;
        slider.Maximum = 100;
        slider.Value = 100;
    }

    private void InitOffsetSlider(Slider slider)
    {
        slider.Minimum = -100;
        slider.Maximum = 100;
        slider.Value = 0;
    }

    private void InitComboBox()
    {
        AlignmentComboBox.ItemsSource = new List<string>()
            {
                "左上对齐",
                "靠上",
                "右上对齐",
                "靠左",
                "居中",
                "靠右",
                "左下对齐",
                "靠右",
                "右下对齐"
            };
        AlignmentComboBox.SelectedValue = "左上对齐";

        FlipTypeComboBox.ItemsSource = new List<string>()
            {
                "无",
                "水平",
                "垂直",
                "两者"
            };
        FlipTypeComboBox.SelectedValue = "无";


    }

    private void InitImageTile(string imageFilePath, Shape shape)
    {
        var image = new ImagePaint();
        image.BeginInit();
        image.UriSource = new Uri(imageFilePath);
        image.EndInit();
        ImageSize = new Size(image.PixelWidth, image.PixelHeight);
        ShapeSize = new Size(shape.Width, shape.Height);
        DpiX = image.DpiX;//分辨率
        DpiY = image.DpiY;//分辨率

        var imageBrush = new ImageBrush(image)
        {
            TileMode = TileMode.Tile,
            Viewport = new Rect(0, 0, ImageSize.Width / ShapeSize.Width * DpiScaleX,
                ImageSize.Height / ShapeSize.Height * DpiScaleY),
        };

        ShapeGeometry = Geometry.Parse(shape.GetShapePath());
        Stroke = shape.Stroke.Clone();
        StrokeThickness = shape.StrokeThickness;
        ImageBrushFill = imageBrush;

        ShapePathGeometry.Data = ShapeGeometry;
        ShapePathGeometry.Stroke = shape.Stroke.Clone();
        ShapePathGeometry.StrokeThickness = shape.StrokeThickness;
        ShapePathGeometry.Fill = ImageBrushFill;

    }


    public event EventHandler<ImageBrush> ImageBrushChanged;

    private async void UpdateImageBrush(object arg)
    {
        _version++;
        await UpdateImageBrushInner(_version);
    }

    private async Task UpdateImageBrushInner(int version)
    {
        if (version != _version)
        {
            return;
        }

        ScaleX = ScaleXSliderSetter.Slider.Value / 100.0;
        ScaleY = ScaleYSliderSetter.Slider.Value / 100.0;
        var alignmentType = AlignmentComboBox.SelectedValue as string;
        var flipType = FlipTypeComboBox.SelectedValue as string;

        var scaleX = DpiScaleX * ScaleX;
        var scaleY = DpiScaleY * ScaleY;

        var offsetX = OffsetXSliderSetter.Slider.Value / 100.0 * (ImageSize.Width / ShapeSize.Width);
        var offsetY = OffsetYSliderSetter.Slider.Value / 100.0 * (ImageSize.Height / ShapeSize.Height);

        if (ShapePathGeometry.Fill is ImageBrush imageBrush)
        {

            var tileMode = GetTileMode(flipType);
            imageBrush.TileMode = tileMode;
            var (aOffsetX, aOffsetY) = GetOffsetByAlignmentType(alignmentType);
            offsetX += aOffsetX;
            offsetY += aOffsetY;
            //var (alignmentX, alignmentY) = GetAlignmentType(alignmentType);
            //imageBrush.AlignmentX = alignmentX;
            //imageBrush.AlignmentY = alignmentY;
            imageBrush.Stretch = Stretch.Fill;
            imageBrush.Viewport = new Rect(offsetX, offsetY, ImageSize.Width / ShapeSize.Width * scaleX,
                ImageSize.Height / ShapeSize.Height * scaleY);
            ShapePathGeometry.InvalidateVisual();
            ImageBrushFill = imageBrush.Clone();

            //var border = new Border();
            //var boundsSize = ShapePathGeometry.Data.Bounds.Size;
            //border.Child = new System.Windows.Shapes.Rectangle
            //{
            //    Width = boundsSize.Width,
            //    Height = boundsSize.Height,
            //    Fill = imageBrush,
            //    Margin = new System.Windows.Thickness(left, top, right, bottom)
            //};
            //var visualBrush = new VisualBrush(border);
            //ShapePathGeometry.Fill=visualBrush;
        }

        await Task.CompletedTask;
    }

    private TileMode GetTileMode(string flipType)
    {
        switch (flipType)
        {
            case "无":
                return TileMode.Tile;
            case "水平":
                return TileMode.FlipX;
            case "垂直":
                return TileMode.FlipY;
            case "两者":
                return TileMode.FlipXY;
            default:
                return TileMode.None;
        }
    }

    private (AlignmentX alignmentX, AlignmentY alignmentY) GetAlignmentType(string alignmentType)
    {
        AlignmentX alignmentX;
        AlignmentY alignmentY;
        switch (alignmentType)
        {
            case "左上对齐":
                alignmentX = AlignmentX.Left;
                alignmentY = AlignmentY.Top;
                break;
            case "靠上":
                alignmentX = AlignmentX.Center;
                alignmentY = AlignmentY.Top;
                break;
            case "右上对齐":
                alignmentX = AlignmentX.Right;
                alignmentY = AlignmentY.Top;
                break;
            case "靠左":
                alignmentX = AlignmentX.Left;
                alignmentY = AlignmentY.Center;
                break;
            case "居中":
                alignmentX = AlignmentX.Center;
                alignmentY = AlignmentY.Center;
                break;
            case "靠右":
                alignmentX = AlignmentX.Right;
                alignmentY = AlignmentY.Center;
                break;
            case "靠下":
                alignmentX = AlignmentX.Center;
                alignmentY = AlignmentY.Bottom;
                break;
            case "右下对齐":
                alignmentX = AlignmentX.Right;
                alignmentY = AlignmentY.Bottom;
                break;
            default:
                alignmentX = AlignmentX.Left;
                alignmentY = AlignmentY.Top;
                break;
        }

        return (alignmentX, alignmentY);
    }

    private (double offsetX, double offsetY) GetOffsetByAlignmentType(string alignmentType)
    {
        var offsetX = 0d;
        var offsetY = 0d;
        switch (alignmentType)
        {
            case "左上对齐":
                offsetX = 0d;
                offsetY = 0d;
                break;
            case "靠上":
                offsetX = 0.5;
                offsetY = 0;
                break;
            case "右上对齐":
                offsetX = 1;
                offsetY = 0;
                break;
            case "靠左":
                offsetX = 0;
                offsetY = 1;
                break;
            case "居中":
                offsetX = 0.5;
                offsetY = 0.5;
                break;
            case "靠右":
                offsetX = 1;
                offsetY = 0.5;
                break;
            case "靠下":
                offsetX = 0.5;
                offsetY = 1;
                break;
            case "左下对齐":
                offsetX = 0;
                offsetY = 1;
                break;
            case "右下对齐":
                offsetX = 1;
                offsetY = 1;
                break;
            default:
                offsetX = 0;
                offsetY = 0;
                break;
        }


        return (offsetX, offsetY);
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        ImageBrushChanged?.Invoke(null, ImageBrushFill);
        Close();
    }



    public ImageBrush ImageBrushFill
    {
        get { return (ImageBrush)GetValue(ImageBrushFillProperty); }
        set { SetValue(ImageBrushFillProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ImageBrushFill.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ImageBrushFillProperty =
        DependencyProperty.Register("ImageBrushFill", typeof(ImageBrush), typeof(ImageTile), new PropertyMetadata(default(ImageBrush)));



    public Geometry ShapeGeometry
    {
        get { return (Geometry)GetValue(ShapeGeometryProperty); }
        set { SetValue(ShapeGeometryProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShapeGeometry.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShapeGeometryProperty =
        DependencyProperty.Register("ShapeGeometry", typeof(Geometry), typeof(ImageTile), new PropertyMetadata(default(Geometry)));



    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    // Using a DependencyProperty as the backing store for StrokeThickness.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ImageTile), new PropertyMetadata(default(double)));





    public Brush Stroke
    {
        get { return (Brush)GetValue(StrokeProperty); }
        set { SetValue(StrokeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Stroke.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register("Stroke", typeof(Brush), typeof(ImageTile), new PropertyMetadata(default(Brush)));





}

