using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;
namespace ExportCanvasWMF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void Export(string filePath)
        {
            int w = Convert.ToInt32(this.MyCanvas.Width);
            int h = Convert.ToInt32(this.MyCanvas.Height);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);
            Bitmap bmp = new Bitmap(w, h);
            Graphics gs = Graphics.FromImage(bmp);
            Metafile mf = new Metafile(filePath, gs.GetHdc(), rect, MetafileFrameUnit.Pixel);
            Graphics g = Graphics.FromImage(mf);
            WPFPainter painter = new WPFPainter(g, MyCanvas);
            painter.Draw();
            g.Save();
            g.Dispose();
            mf.Dispose();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string filepath = this.txtFilePath.Text.Trim();
            Export(filepath);
        }
    }
    public class WPFPainter
    {
        public System.Drawing.Graphics DrawGraphics;
        public Canvas Container = new Canvas();
        List<Shape> ShapeList = new List<Shape>();
        public WPFPainter(System.Drawing.Graphics _DrawGraphics, Canvas _Container)
        {
            DrawGraphics = _DrawGraphics;
            Container = _Container;
            //Get the Shape elements in the Canvas
            ShapeList = AnalyseShape();
        }
        public List<Shape> AnalyseShape()
        {
            List<Shape> list = new List<Shape>();
            //Container.Children;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                UIElement element = Container.Children[i];
                Type t = element.GetType();
                if (t == typeof(System.Windows.Shapes.Ellipse))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Rectangle))
                {
                    list.Add(element as Shape);
                }
                // You can write more code to support more type here
                //... ...
            }
            return list;
        }
        public void Draw()
        {
            //Convert WPF shape to GDI+ shape and Draw GDI+
            for (int i = 0; i < ShapeList.Count; i++)
            {
                Shape element = ShapeList[i];
                Type t = element.GetType();
                float x = Convert.ToSingle(Canvas.GetLeft(element));
                float y = Convert.ToSingle(Canvas.GetTop(element));
                float w = Convert.ToSingle(element.Width);
                float h = Convert.ToSingle(element.Height);
                System.Drawing.SolidBrush GDIStroke = ConvertSolidColorBrush(element.Stroke as SolidColorBrush);
                System.Drawing.SolidBrush GDIFill = ConvertSolidColorBrush(element.Fill as SolidColorBrush);
                float Thickness = Convert.ToSingle(element.StrokeThickness);
                System.Drawing.Pen pen = new System.Drawing.Pen(GDIStroke, Thickness);
                if (t == typeof(System.Windows.Shapes.Ellipse))
                {
                    DrawEllipse(x, y, w, h, pen, GDIFill);
                }
                else if (t == typeof(System.Windows.Shapes.Rectangle))
                {
                    DrawRectangle(x, y, w, h, pen, GDIFill);
                }
            }
        }
        public void DrawEllipse(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawEllipse(pen, x, y, w, h);
           
            DrawGraphics.FillEllipse(GDIFill, x, y, w, h);
        }
        public void DrawRectangle(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawRectangle(pen, x, y, w, h);
            DrawGraphics.FillRectangle(GDIFill, x, y, w, h);
        }
        public System.Drawing.SolidBrush ConvertSolidColorBrush(SolidColorBrush scb)
        {
            return new SolidBrush(ConvertColor(scb.Color));
        }
        public System.Drawing.Color ConvertColor(System.Windows.Media.Color WPFColor)
        {
            return System.Drawing.Color.FromArgb(WPFColor.A, WPFColor.R, WPFColor.G, WPFColor.B);
        }
    }
}
