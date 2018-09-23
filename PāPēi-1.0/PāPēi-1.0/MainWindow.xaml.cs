using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using O2S.Components.PDFRender4NET.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace PāPēi_1._0
{
    public partial class MainWindow : Window
    {
        /* Visiting Directory */
        string originalDirectory = AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + "\\Resources\\";
        string navigationDirectory = "";
        int DirectoryLevel = 0;

        /* Color Scheme */
        Color Color_1 = Color.FromArgb(150, 3, 22, 52);
        Color Color_2 = Color.FromArgb(150, 3, 54, 73);
        Color Color_3 = Color.FromArgb(150, 3, 101, 100);
        Color Color_4 = Color.FromArgb(150, 205, 179, 128);
        Color Color_5 = Color.FromArgb(150, 232, 221, 203);

        bool Break = true;     /* Breaking */

        /* Main Window Initialization */
        public MainWindow()
        {
            InitializeComponent();
            CreatePack("PāPēi");

            /* Search Box Initialization*/
            Button btnSearch = SearchButton();
            TextBox txtSearch = SearchTextBox();
            Button btnClear = ClearButton();
            ProgressBar pbLoading = loadingBar();
            mainCanvas.Children.Add(btnSearch);
            mainCanvas.RegisterName(btnSearch.Name, btnSearch);
            mainCanvas.Children.Add(txtSearch);
            mainCanvas.RegisterName(txtSearch.Name, txtSearch);
            mainCanvas.Children.Add(btnClear);
            mainCanvas.RegisterName(btnClear.Name, btnClear);
            mainCanvas.Children.Add(pbLoading);
            mainCanvas.RegisterName(pbLoading.Name, pbLoading);
        }

        /* Utilities */
        private void RefreshCanvas()
        {
            mainCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            mainCanvas.Arrange(new Rect(0, 0, mainCanvas.DesiredSize.Width, mainCanvas.DesiredSize.Height));
        }

        private string Rename(string originalString)
        {
            return originalString.Replace("+", "加").Replace(".", "点").Replace("-", "杠").Replace("\\", "斜").Replace(":", "冒");
        }

        private string Dename(string originalString)
        {
            return originalString.Replace("加", "+").Replace("点", ".").Replace("杠", "-").Replace("斜", "\\").Replace("冒", ":");
        }

        private string GetFileType(string originalString)
        {
            return originalString.Substring(originalString.Length - 3);
        }

        private void GetDirectoryLevel()
        {
            DirectoryLevel = navigationDirectory.Count(c => c == '\\');
        }

        private void CreatePack(string PackName)
        {
            /* ScrollViewer And Panel Initialization */
            ScrollViewer scv = SenseSCV("scv" + PackName, 395, 545);
            mainCanvas.Children.Add(scv);
            mainCanvas.RegisterName(scv.Name, scv);
            StackPanel sp = SenseSP("sp" + PackName);
            scv.Content = sp;
            mainCanvas.RegisterName(sp.Name, sp);

            RefreshCanvas();
            Canvas.SetBottom(scv, 45);
            Canvas.SetRight(scv, 25);

            /* Menu Initialization */
            string packDirectory = Path.Combine(originalDirectory, navigationDirectory);
            DirectoryInfo dinfo = new DirectoryInfo(packDirectory);
            if (navigationDirectory.Count(c => c == '\\') < 4)
            {
                foreach (var folder in dinfo.GetDirectories())  /* Get Folders */
                {
                    customLabel childLabel = new customLabel();
                    if (folder.Name == "PaPei")   /* Big Title */
                    {
                        childLabel = SenseLabel("lbl" + folder.Name, "PāPēi", 100, 200, 80, navigationDirectory + folder.Name + "\\");
                    }
                    else   /* Normal Title */
                    {
                        childLabel = SenseLabel("lbl" + folder.Name, folder.Name, 60, 200, 40, navigationDirectory + folder.Name + "\\");
                    }
                    appear(childLabel, Color_4);
                    sp.Children.Add(childLabel);
                    sp.RegisterName(childLabel.Name, childLabel);
                }
            }
            else if (navigationDirectory.Count(c => c == '\\') == 4)
            {
                foreach (var file in dinfo.GetFiles())   /* Get Files */
                {
                    customLabel childLabel = SenseLabel("lbl" + Rename(file.Name), file.Name.Substring(0, file.Name.Length - 4), 60, 450, 40, navigationDirectory + Rename(file.Name));
                    sp.Children.Add(childLabel);
                    sp.RegisterName(childLabel.Name, childLabel);
                }
            }
        }

        private void ClearScrollViewer()
        {
            List<ScrollViewer> redundance = new List<ScrollViewer>();
            foreach (var child in mainCanvas.Children)
            {
                if (child.GetType() == typeof(ScrollViewer))
                {
                    ScrollViewer redundantSCV = (ScrollViewer)child;
                    redundance.Add(redundantSCV);
                    StackPanel redundantSP = (StackPanel)mainCanvas.FindName("sp" + redundantSCV.Name.Substring(3, redundantSCV.Name.Length - 3));
                    foreach (object childLabel in redundantSP.Children)
                    {
                        redundantSP.UnregisterName((childLabel as FrameworkElement).Name);
                    }
                    mainCanvas.UnregisterName(redundantSP.Name);
                }
            }

            foreach (var child in redundance)
            {
                ScrollViewer redundantSCV = child;
                mainCanvas.UnregisterName(redundantSCV.Name);
                mainCanvas.Children.Remove(redundantSCV);
            }
        }

        private void ClearStackPanel()
        {
            List<StackPanel> redundance = new List<StackPanel>();
            foreach (var child in mainCanvas.Children)
            {
                if (child.GetType() == typeof(StackPanel))
                {
                    StackPanel redundantSP = (StackPanel)child;
                    redundance.Add(redundantSP);
                }
            }

            foreach (var child in redundance)
            {
                StackPanel redundantSP = child;
                mainCanvas.UnregisterName(redundantSP.Name);
                mainCanvas.Children.Remove(redundantSP);
            }
        }

        private void ClearPDFLabel()
        {
            List<customLabel> redundance = new List<customLabel>();
            foreach (var child in mainCanvas.Children)
            {
                if (child.GetType() == typeof(customLabel))
                {
                    customLabel paperLabel = (customLabel)child;
                    if (GetFileType(paperLabel.Name) == "pdf")
                    {
                        redundance.Add(paperLabel);
                    }
                }
            }

            foreach (var child in redundance)
            {
                customLabel redundantLabel = child;
                mainCanvas.UnregisterName(Rename(redundantLabel.Name));    /* Avoid Invalid Naming */
                mainCanvas.Children.Remove(redundantLabel);
            }
        }

        /* Animation Functions */
        private void appear(object sender, Color color)
        {
            Label label = (Label)sender;
            ColorAnimation show = new ColorAnimation(Color.FromArgb(0, color.R, color.G, color.B), color, TimeSpan.FromSeconds(0.3));
            label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, show);
        }

        private void disappear(object sender)
        {
            Label label = (Label)sender;
            Color currrentColor = (Color)ColorConverter.ConvertFromString(label.Foreground.ToString());
            ColorAnimation disappear = new ColorAnimation(currrentColor, Color_4, TimeSpan.FromSeconds(0.3));
            label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, disappear);
        }

        private void shift(object sender, int senderX, int senderY, int position)
        {
            Label label = (Label)sender;
            TransformGroup groupTransform = new TransformGroup();
            Transform shiftTransform = new TranslateTransform();
            Transform shrinkTransform = new ScaleTransform();
            groupTransform.Children.Add(shiftTransform);
            groupTransform.Children.Add(shrinkTransform);
            double top = Canvas.GetTop(label);
            double left = Canvas.GetLeft(label);
            DoubleAnimation shiftX = new DoubleAnimation { From = 0, To = senderX - top - 15, Duration = TimeSpan.FromSeconds(0.3) };
            DoubleAnimation shiftY = new DoubleAnimation { From = 0, To = senderY - left + (position * 50), Duration = TimeSpan.FromSeconds(0.3) };
            DoubleAnimation shrink = new DoubleAnimation(1, 0.8, TimeSpan.FromSeconds(0.3));
            label.RenderTransform = groupTransform;
            shiftTransform.BeginAnimation(TranslateTransform.XProperty, shiftX);
            shiftTransform.BeginAnimation(TranslateTransform.YProperty, shiftY);
            shrinkTransform.BeginAnimation(ScaleTransform.ScaleXProperty, shrink);
            shrinkTransform.BeginAnimation(ScaleTransform.ScaleYProperty, shrink);
        }

        private void restore(object sender, Color color)
        {
            if (sender.GetType() == typeof(Label))
            {
                Label label = (Label)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(label.Foreground.ToString());
                ColorAnimation darken = new ColorAnimation(currrentColor, color, TimeSpan.FromSeconds(0.3));
                label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, darken);
            }
            else if (sender.GetType() == typeof(TextBox))
            {
                TextBox textbox = (TextBox)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(textbox.Background.ToString());
                ColorAnimation darken = new ColorAnimation(currrentColor, color, TimeSpan.FromSeconds(0.3));
                textbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, darken);
            }
            else if (sender.GetType() == typeof(Button))
            {
                Button button = (Button)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(button.Background.ToString());
                ColorAnimation lighten = new ColorAnimation(currrentColor, color, TimeSpan.FromSeconds(0.3));
                button.Background.BeginAnimation(SolidColorBrush.ColorProperty, lighten);
            }
        }

        private void lighten(object sender, Color color)
        {
            if (sender.GetType() == typeof(Label))
            {
                Label label = (Label)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(label.Foreground.ToString());
                ColorAnimation lighten = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A - 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, lighten);
            }
            else if (sender.GetType() == typeof(TextBox))
            {
                TextBox textbox = (TextBox)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(textbox.Background.ToString());
                ColorAnimation lighten = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A - 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                textbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, lighten);
            }
            else if (sender.GetType() == typeof(Button))
            {
                Button button = (Button)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(button.Background.ToString());
                ColorAnimation lighten = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A - 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                button.Background.BeginAnimation(SolidColorBrush.ColorProperty, lighten);
            }
        }

        private void darken(object sender, Color color)
        {
            if (sender.GetType() == typeof(Label))
            {
                Label label = (Label)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(label.Foreground.ToString());
                ColorAnimation darken = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A + 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                label.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, darken);
            }
            else if (sender.GetType() == typeof(TextBox))
            {
                TextBox textbox = (TextBox)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(textbox.Background.ToString());
                ColorAnimation darken = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A + 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                textbox.Background.BeginAnimation(SolidColorBrush.ColorProperty, darken);
            }
            else if (sender.GetType() == typeof(Button))
            {
                Button button = (Button)sender;
                Color currrentColor = (Color)ColorConverter.ConvertFromString(button.Background.ToString());
                ColorAnimation darken = new ColorAnimation(currrentColor, Color.FromArgb((byte)(color.A + 100), color.R, color.G, color.B), TimeSpan.FromSeconds(0.3));
                button.Background.BeginAnimation(SolidColorBrush.ColorProperty, darken);
            }
        }

        private void showImage(object sender)
        {
            Image img = (Image)sender;
            DoubleAnimation show = new DoubleAnimation(0, 0.9, TimeSpan.FromSeconds(0.3));
            img.BeginAnimation(OpacityProperty, show);
        }

        /* Custom Elements */
        public class customLabel : Label
        {
            public static readonly DependencyProperty labelDirectory = DependencyProperty.RegisterAttached("Directory", typeof(string), typeof(customLabel), new PropertyMetadata((string)""));

            public string Directory
            {
                get { return (string)GetValue(labelDirectory); }
                set { SetValue(labelDirectory, value); }
            }
        }

        private customLabel SenseLabel(string Name, string Content, int Height, int Width, int FontSize, string Directory)
        {
            customLabel label = new customLabel();
            label.Name = Name;
            label.Content = Content;
            label.Height = Height;
            label.Width = Width;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Components/#Rational");
            label.FontSize = FontSize;
            label.Foreground = new SolidColorBrush(Color_4);
            label.MouseEnter += SenseLabel_MouseEnter;
            label.MouseLeave += SenseLabel_MouseLeave;
            label.MouseDown += SenseLabel_MouseDown;
            label.Directory = Directory;
            return label;
        }

        private ScrollViewer SenseSCV(string Name, int Height, int Width)
        {
            ScrollViewer SCV = new ScrollViewer();
            SCV.Name = Name;
            SCV.Height = Height;
            SCV.Width = Width;
            SCV.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            SCV.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            SCV.Style = FindResource("PaPeiScrollViewer") as Style;
            return SCV;
        }

        private StackPanel SenseSP(string Name)
        {
            StackPanel SP = new StackPanel();
            SP.Name = Name;
            SP.VerticalAlignment = VerticalAlignment.Center;
            SP.HorizontalAlignment = HorizontalAlignment.Center;
            SP.CanVerticallyScroll = true;
            return SP;
        }

        private Label PreviewLabel()
        {
            Label label = new Label();
            label.Name = "lblPreview";
            label.Content = "Preview";
            label.Height = 48;
            label.Width = 140;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Components/#Rational");
            label.FontSize = 30;
            label.Foreground = new SolidColorBrush(Color_4);
            label.MouseEnter += SenseLabel_MouseEnter;
            label.MouseLeave += SenseLabel_MouseLeave;
            label.MouseDown += PreviewLabel_MouseDown;
            Canvas.SetTop(label, 35);
            Canvas.SetLeft(label, 625);
            appear(label, Color_4);
            return label;
        }

        private TextBox SearchTextBox()
        {
            TextBox textbox = new TextBox();
            textbox.Name = "txtSearch";
            textbox.Height = 30;
            textbox.Width = 140;
            textbox.Margin = new Thickness(55, 45, 0, 0);
            textbox.BorderThickness = new Thickness(0, 0, 0, 0);
            textbox.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Components/#Rational");
            textbox.FontSize = 20;
            textbox.Background = new SolidColorBrush(Color_3);
            textbox.VerticalContentAlignment = VerticalAlignment.Center;
            textbox.GotFocus += SearchTextBox_GotFocus;
            textbox.LostFocus += SearchTextBox_LostFocus;
            textbox.MouseEnter += SearchTextBox_MouseEnter;
            textbox.MouseLeave += SearchTextBox_MouseLeave;
            return textbox;
        }

        private Button SearchButton()
        {
            Button button = new Button();
            button.Name = "btnSearch";
            button.Height = 30;
            button.Width = 30;
            button.Margin = new Thickness(20, 45, 0, 0);
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + "\\Components\\Search.png"));
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(img);
            button.Content = sp;
            button.Background = new SolidColorBrush(Color_3);
            button.Style = FindResource("btnSearch") as Style;
            button.MouseEnter += SearchButton_MouseEnter;
            button.MouseLeave += SearchButton_MouseLeave;
            button.Click += SearchButton_Click;
            return button;
        }

        private Button ClearButton()
        {
            Button button = new Button();
            button.Name = "btnClear";
            button.Height = 30;
            button.Width = 30;
            button.Margin = new Thickness(200, 45, 0, 0);
            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + "\\Components\\Clear.png"));
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            sp.Children.Add(img);
            button.Content = sp;
            button.Background = new SolidColorBrush(Color_3);
            button.Style = FindResource("btnClear") as Style;
            button.MouseEnter += ClearButton_MouseEnter;
            button.MouseLeave += ClearButton_MouseLeave;
            button.Click += ClearButton_Click;
            return button;
        }

        private ProgressBar loadingBar()
        {
            ProgressBar PB = new ProgressBar();
            PB.Name = "pbLoading";
            PB.Height = 5;
            PB.Width = 800;
            PB.Margin = new Thickness(0);
            PB.BorderThickness = new Thickness(0);
            PB.Background = new SolidColorBrush(Colors.Transparent);
            PB.Foreground = new SolidColorBrush(Color_3);
            return PB;
        }

        private StackPanel AdImage()
        {
            StackPanel sp = new StackPanel();
            sp.Name = "spAD";
            sp.Height = 270;
            sp.Width = 480;
            sp.Margin = new Thickness(265, 100, 0, 0);
            DirectoryInfo dinfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + "\\Ad");
            Image img = new Image();
            int num = new Random().Next(1, dinfo.GetFiles().Count() + 1);
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory.Substring(0, AppDomain.CurrentDomain.BaseDirectory.Length - 11) + "\\Ad\\Ad_" + num + ".jpg"));
            sp.Children.Add(img);
            showImage(img);
            return sp;
        }

        /* Element Events */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /* Reset Focus */
            mainCanvas.Focus();
        }

        private void SenseLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            lighten(sender, Color_4);
        }

        private void SenseLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            restore(sender, Color_4);
        }

        private void SenseLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            customLabel label = (customLabel)sender;
            string directName = label.Name.Substring(3, label.Name.Length - 3);
            GetDirectoryLevel();

            if (label.Parent.GetType() == typeof(StackPanel))
            {
                StackPanel parent = (StackPanel)label.Parent;
                parent.Children.Remove(label);
                ClearScrollViewer();
                ClearPDFLabel();

                /* Save Clicked Label */
                if (label.Name == "lblPaPei")
                {
                    label.FontSize = 40;
                    label.Height = 60;
                    label.Width = 200;
                }
                mainCanvas.Children.Add(label);
                mainCanvas.RegisterName(label.Name, label);
                RefreshCanvas();
                Canvas.SetTop(label, (mainCanvas.ActualHeight - label.ActualHeight) / 2);
                Canvas.SetLeft(label, (mainCanvas.ActualWidth - label.ActualWidth) / 2);

                if (parent.Name != "spSearch")
                {
                    /* Alignment */
                    if (DirectoryLevel < 4)
                    {
                        shift(label, -100, 200, DirectoryLevel);
                        navigationDirectory += directName + "\\";
                        CreatePack(directName);
                    }
                    else
                    {
                        /* Create Preview Form */
                        shift(label, 300, -25, 0);
                        label.Foreground = new SolidColorBrush(Color.FromArgb(200, 205, 179, 128));
                        label.MouseEnter -= SenseLabel_MouseEnter;
                        label.MouseLeave -= SenseLabel_MouseLeave;
                        label.MouseDown -= SenseLabel_MouseDown;
                        navigationDirectory += Dename(directName);
                        Label previewLabel = PreviewLabel();
                        mainCanvas.Children.Add(previewLabel);
                        mainCanvas.RegisterName(previewLabel.Name, previewLabel);
                        StackPanel spAD = AdImage();
                        mainCanvas.Children.Add(spAD);
                        mainCanvas.RegisterName(spAD.Name, spAD);
                    }
                }
                else
                {
                    Break = true;
                    navigationDirectory = Dename(label.Directory);

                    DirectoryLevel = 0;
                    string backDirectory = navigationDirectory.Substring(0, navigationDirectory.LastIndexOf("\\") + 1);
                    string menuDirectory = "";
                    while (backDirectory.IndexOf("\\", 0) != -1)
                    {
                        string labelName = backDirectory.Substring(0, backDirectory.IndexOf("\\"));
                        menuDirectory += labelName + "\\";
                        if (mainCanvas.FindName("lbl" + labelName) == null)
                        {
                            customLabel childLabel = new customLabel();
                            if (labelName == "PaPei")
                            {
                                childLabel = SenseLabel("lbl" + labelName, "PāPēi", 60, 200, 40, menuDirectory); ;
                            }
                            else
                            {
                                childLabel = SenseLabel("lbl" + labelName, labelName, 60, 200, 40, menuDirectory);
                            }
                            mainCanvas.Children.Add(childLabel);
                            mainCanvas.RegisterName(childLabel.Name, childLabel);
                            RefreshCanvas();
                            Canvas.SetTop(childLabel, (mainCanvas.ActualHeight - childLabel.ActualHeight) / 2);
                            Canvas.SetLeft(childLabel, (mainCanvas.ActualWidth - childLabel.ActualWidth) / 2);
                            shift(childLabel, -100, 200, DirectoryLevel);
                        }
                        backDirectory = backDirectory.Remove(0, backDirectory.IndexOf("\\") + 1);
                        DirectoryLevel += 1;
                    }

                    shift(label, 300, -25, 0);
                    label.Foreground = new SolidColorBrush(Color.FromArgb(200, 205, 179, 128));
                    label.MouseEnter -= SenseLabel_MouseEnter;
                    label.MouseLeave -= SenseLabel_MouseLeave;
                    label.MouseDown -= SenseLabel_MouseDown;
                    Label previewLabel = PreviewLabel();
                    mainCanvas.Children.Add(previewLabel);
                    mainCanvas.RegisterName(previewLabel.Name, previewLabel);
                    StackPanel spAD = AdImage();
                    mainCanvas.Children.Add(spAD);
                    mainCanvas.RegisterName(spAD.Name, spAD);
                }
            }
            else
            {
                Break = true;

                string backDirectory = navigationDirectory.Substring(navigationDirectory.IndexOf(directName) + directName.Length + 1, navigationDirectory.Length - navigationDirectory.IndexOf(directName) - directName.Length - 1);
                navigationDirectory = navigationDirectory.Substring(0, navigationDirectory.IndexOf(directName) + directName.Length + 1);

                while (backDirectory.IndexOf("\\", 0) != -1)
                {
                    string labelName = backDirectory.Substring(0, backDirectory.IndexOf("\\"));
                    if (mainCanvas.FindName("lbl" + labelName) != null)
                    {
                        customLabel redundantLabel = (customLabel)mainCanvas.FindName("lbl" + labelName);
                        mainCanvas.UnregisterName(redundantLabel.Name);
                        mainCanvas.Children.Remove(redundantLabel);
                    }
                    backDirectory = backDirectory.Remove(0, backDirectory.IndexOf("\\") + 1);
                }

                if (mainCanvas.FindName("lblPreview") != null)
                {
                    Label previewLabel = (Label)mainCanvas.FindName("lblPreview");
                    mainCanvas.UnregisterName(previewLabel.Name);
                    mainCanvas.Children.Remove(previewLabel);
                }
                
                ClearScrollViewer();
                ClearStackPanel();
                ClearPDFLabel();
                CreatePack(directName);
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            darken(sender, Color_3);
            textbox.MouseEnter -= SearchTextBox_MouseEnter;
            textbox.MouseLeave -= SearchTextBox_MouseLeave;
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textbox = (TextBox)sender;
            restore(sender, Color_3);
            textbox.MouseEnter += SearchTextBox_MouseEnter;
            textbox.MouseLeave += SearchTextBox_MouseLeave;
        }

        private void SearchTextBox_MouseEnter(object sender, MouseEventArgs e)
        {
            darken(sender, Color_3);
        }

        private void SearchTextBox_MouseLeave(object sender, MouseEventArgs e)
        {
            restore(sender, Color_3);
        }

        private void SearchButton_MouseEnter(object sender, MouseEventArgs e)
        {
            darken(sender, Color_3);
        }

        private void SearchButton_MouseLeave(object sender, MouseEventArgs e)
        {
            restore(sender, Color_3);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox txtSearch = (TextBox)mainCanvas.FindName("txtSearch");
            if (txtSearch.Text != "")
            {
                Search();
            }
        }

        private void ClearButton_MouseEnter(object sender, MouseEventArgs e)
        {
            darken(sender, Color_3);
        }

        private void ClearButton_MouseLeave(object sender, MouseEventArgs e)
        {
            restore(sender, Color_3);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TextBox txtSearch = (TextBox)mainCanvas.FindName("txtSearch");
            txtSearch.Text = "";
        }

        private void PreviewLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string pdfDirectory = Path.Combine(originalDirectory, navigationDirectory);
            pdfFile = PDFFile.Open(pdfDirectory);
            Console.WriteLine(pdfDirectory);
            Process();
        }

        /* PDF Viewer */
        PDFFile pdfFile;

        private void Process()
        {
            IDocumentPaginatorSource source = pdfFile.GetFixedDocument(false);
            new PDFViewer() { PreviewDocument = source }.Show();
        }

        /* PDF Extracter */
        private string ExtractText(string path)
        {
            using (PdfReader reader = new PdfReader(path))
            {
                StringBuilder text = new StringBuilder();

                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }

        /* Search Engine */
        private void Search()
        {
            Break = false;
            ClearPDFLabel();
            if (navigationDirectory.Length > 0 && navigationDirectory.Substring(navigationDirectory.Length - 3) == "pdf")
            {
                if (mainCanvas.FindName("lblPreview") != null)
                {
                    Label previewLabel = (Label)mainCanvas.FindName("lblPreview");
                    mainCanvas.UnregisterName(previewLabel.Name);
                    mainCanvas.Children.Remove(previewLabel);
                }
            }

            List<string> fileList = new List<string>();
            string directory = Path.Combine(originalDirectory, navigationDirectory);
            DirectoryInfo di = new DirectoryInfo(directory);

            switch (navigationDirectory.Count(c => c == '\\'))
            {
                case 0:
                    foreach (var PāPēi in di.GetDirectories())
                    {
                        DirectoryInfo PāPēidi = new DirectoryInfo(directory + PāPēi.Name + "\\");
                        foreach (var grade in PāPēidi.GetDirectories())
                        {
                            DirectoryInfo gradedi = new DirectoryInfo(directory + PāPēi.Name + "\\" + grade.Name + "\\");
                            foreach (var subject in gradedi.GetDirectories())
                            {
                                DirectoryInfo subjectdi = new DirectoryInfo(directory + PāPēi.Name + "\\" + grade.Name + "\\" + subject.Name + "\\");
                                foreach (var year in subjectdi.GetDirectories())
                                {
                                    DirectoryInfo yeardi = new DirectoryInfo(directory + PāPēi.Name + "\\" + grade.Name + "\\" + subject.Name + "\\" + year.Name + "\\");
                                    foreach (var paper in yeardi.GetFiles())
                                    {
                                        fileList.Add(directory + PāPēi.Name + "\\" + grade.Name + "\\" + subject.Name + "\\" + year.Name + "\\" + paper.Name);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    foreach (var grade in di.GetDirectories())
                    {
                        DirectoryInfo gradedi = new DirectoryInfo(directory + grade.Name + "\\");
                        foreach (var subject in gradedi.GetDirectories())
                        {
                            DirectoryInfo subjectdi = new DirectoryInfo(directory + grade.Name + "\\" + subject.Name + "\\");
                            foreach (var year in subjectdi.GetDirectories())
                            {
                                DirectoryInfo yeardi = new DirectoryInfo(directory + grade.Name + "\\" + subject.Name + "\\" + year.Name + "\\");
                                foreach (var paper in yeardi.GetFiles())
                                {
                                    fileList.Add(directory + grade.Name + "\\" + subject.Name + "\\" + year.Name + "\\" + paper.Name);
                                }
                            }
                        }
                    }
                    break;
                case 2:
                    foreach (var subject in di.GetDirectories())
                    {
                        DirectoryInfo subjectdi = new DirectoryInfo(directory + subject.Name + "\\");
                        foreach (var year in subjectdi.GetDirectories())
                        {
                            DirectoryInfo yeardi = new DirectoryInfo(directory + subject.Name + "\\" + year.Name + "\\");
                            foreach (var paper in yeardi.GetFiles())
                            {
                                fileList.Add(directory + subject.Name + "\\" + year.Name + "\\" + paper.Name);
                            }
                        }
                    }
                    break;
                case 3:
                    foreach (var year in di.GetDirectories())
                    {
                        DirectoryInfo yeardi = new DirectoryInfo(directory + year.Name + "\\");
                        foreach (var paper in yeardi.GetFiles())
                        {
                            fileList.Add(directory + year.Name + "\\" + paper.Name);
                        }
                    }
                    break;
                default:
                    if (navigationDirectory.Substring(navigationDirectory.Length - 3, 3) != "pdf")
                    {
                        foreach (var paper in di.GetFiles())
                        {
                            fileList.Add(directory + paper.Name);
                        }
                    }
                    else
                    {
                        fileList.Add(directory);
                    }
                    break;
            }

            ClearScrollViewer();
            ScrollViewer scv = SenseSCV("scvSearch", 395, 575);
            mainCanvas.Children.Add(scv);
            mainCanvas.RegisterName(scv.Name, scv);
            mainCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            mainCanvas.Arrange(new Rect(0, 0, mainCanvas.DesiredSize.Width, mainCanvas.DesiredSize.Height));
            Canvas.SetBottom(scv, 45);
            Canvas.SetRight(scv, 25);
            StackPanel sp = SenseSP("spSearch");
            scv.Content = sp;
            mainCanvas.RegisterName(sp.Name, sp);
            ProgressBar pbLoading = (ProgressBar)mainCanvas.FindName("pbLoading");
            foreach (string fileDirectory in fileList)
            {
                if (Break)
                    break;

                TextBox txtSearch = (TextBox)mainCanvas.FindName("txtSearch");
                if (ExtractText(Path.Combine(originalDirectory,fileDirectory)).Contains(txtSearch.Text) == true)
                {
                    string pdf = fileDirectory.Substring(fileDirectory.LastIndexOf("\\") + 1, (fileDirectory.Length - fileDirectory.LastIndexOf("\\") - 5));
                    customLabel childLabel = SenseLabel("lbl" + Rename(pdf + ".pdf"), pdf, 60, 450, 40, fileDirectory.Substring(originalDirectory.Length, fileDirectory.Length - originalDirectory.Length));
                    appear(childLabel, Color_4);
                    sp.Children.Add(childLabel);
                    sp.RegisterName(childLabel.Name, childLabel);
                }
                pbLoading.Dispatcher.Invoke(() => pbLoading.Value += (pbLoading.Maximum / fileList.Count()), DispatcherPriority.Background);
            }
            pbLoading.Value = 0;
            Break = true;
        }
    }
}
