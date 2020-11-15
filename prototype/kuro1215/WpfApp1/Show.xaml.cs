using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class Show : Window
    {
        private int FrameCount { get; }

        public Show(int frame)
        {
            InitializeComponent();
            FrameCount = frame;
        }

        public void SetImage(Bitmap bitmap, int pos)
        {
            IntPtr hbmp = bitmap.GetHbitmap();
            MovieImage.Source = Imaging.CreateBitmapSourceFromHBitmap(hbmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            MovieSlider.Value = pos * 100.0 / FrameCount;
        }
    }
}
