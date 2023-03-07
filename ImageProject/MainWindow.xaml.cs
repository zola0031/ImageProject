using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
using Size = System.Windows.Size;

namespace ImageProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Method for Initialize the App
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Proprties to key track of the rectangles, starting, ending point of mouse drag
        /// </summary>
        SolidColorBrush myBrush = new SolidColorBrush(Colors.Black);
        public string MouseActionType = "drawing";
        private List<Rectangle> DragRectangles = new List<Rectangle>();
        private Rectangle DragRectangle = null;
        private Point StartPoint, LastPoint, MouseClick;


        /// <summary>
        /// Used for selecting an Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == true)
            {
                string selectedFileName = dlg.FileName;
                FileNameLabel.Content = selectedFileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedFileName);
                bitmap.EndInit();
                ImageViewer1.Source = bitmap;
            }
        }


        /// <summary>
        /// Method for changing the color of rectangles 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClrPcker_Background_SelectedColorChanged(object sender, MouseButtonEventArgs e)
        {
            myBrush = new SolidColorBrush(ClrPcker_Background.SelectedColor.Value);
        }

        /// <summary>
        /// Method for setting the starting point of rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canDraw_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = Mouse.GetPosition(canDraw);
            LastPoint = StartPoint;
            DragRectangle = new Rectangle();
            DragRectangle.Width = 1;
            DragRectangle.Height = 1;
            DragRectangle.Stroke = Brushes.Red;
            DragRectangle.StrokeThickness = 1;
            DragRectangle.Cursor = Cursors.Cross;

            canDraw.Children.Add(DragRectangle);
            Canvas.SetLeft(DragRectangle, StartPoint.X);
            Canvas.SetTop(DragRectangle, StartPoint.Y);

            canDraw.MouseMove += canDraw_MouseMove;
            canDraw.MouseUp += canDraw_MouseUp;
            canDraw.CaptureMouse();
        }

        /// <summary>
        /// Method for tracking the mouse movement 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canDraw_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseActionType == "drawing")
            {
                CalculateMouseMovement(sender, e);
            }
            else
            {
                LastPoint = Mouse.GetPosition(canDraw);
            }
        }

        /// <summary>
        /// Method for tracking how much the mouse moved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateMouseMovement(object sender, MouseEventArgs e)
        {
            LastPoint = Mouse.GetPosition(canDraw);
            DragRectangle.Width = Math.Abs(LastPoint.X - StartPoint.X);
            DragRectangle.Height = Math.Abs(LastPoint.Y - StartPoint.Y);
            Canvas.SetLeft(DragRectangle, Math.Min(LastPoint.X, StartPoint.X));
            Canvas.SetTop(DragRectangle, Math.Min(LastPoint.Y, StartPoint.Y));
        }

        /// <summary>
        /// Method for calculating a distance the mouse travelled and creating a rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canDraw_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseActionType == "drawing")
            {
                DrawRectangle(sender, e);
            }
            else if (MouseActionType == "delete")
            {
                DeleteRectangle(sender, e);
            }
            else
            {
                MoveRectangle(sender, e);
            }
        }

        /// <summary>
        /// Method for drawing the rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawRectangle(object sender, MouseButtonEventArgs e)
        {
            canDraw.ReleaseMouseCapture();
            canDraw.MouseMove -= canDraw_MouseMove;
            canDraw.MouseUp -= canDraw_MouseUp;
            canDraw.Children.Remove(DragRectangle);

            if (LastPoint.X < 0) LastPoint.X = 0;
            if (LastPoint.X >= canDraw.Width) LastPoint.X = canDraw.Width - 1;
            if (LastPoint.Y < 0) LastPoint.Y = 0;
            if (LastPoint.Y >= canDraw.Height) LastPoint.Y = canDraw.Height - 1;

            int width = (int)Math.Abs(LastPoint.X - StartPoint.X) + 1;
            int height = (int)Math.Abs(LastPoint.Y - StartPoint.Y) + 1;

            Rectangle rec = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };

            canDraw.Children.Add(rec);

            Canvas.SetTop(rec, StartPoint.Y);
            Canvas.SetLeft(rec, StartPoint.X);

            DragRectangles.Add(rec);
        }

        /// <summary>
        /// Method for moving the rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveRectangle(object sender, MouseButtonEventArgs e)
        {
            canDraw.ReleaseMouseCapture();
            canDraw.MouseMove -= canDraw_MouseMove;
            canDraw.MouseUp -= canDraw_MouseUp;

            foreach (var item in DragRectangles)
            {
                if ((StartPoint.X > Canvas.GetLeft(item)) && (StartPoint.X < Canvas.GetLeft(item) + item.Width) && (StartPoint.Y > Canvas.GetTop(item)) && (StartPoint.Y < Canvas.GetTop(item) + item.Height))
                {
                    Rectangle rec1 = item;

                    Canvas.SetTop(rec1, LastPoint.Y);
                    Canvas.SetLeft(rec1, LastPoint.X);
                }
            }

        }

        /// <summary>
        /// Method for deleting the rectangle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteRectangle(object sender, MouseButtonEventArgs e)
        {
            canDraw.ReleaseMouseCapture();
            canDraw.MouseMove -= canDraw_MouseMove;
            canDraw.MouseUp -= canDraw_MouseUp;

            Rectangle rec1 = default;

            foreach (var item in DragRectangles)
            {
                if ((StartPoint.X > Canvas.GetLeft(item)) && (StartPoint.X < Canvas.GetLeft(item) + item.Width) && (StartPoint.Y > Canvas.GetTop(item)) && (StartPoint.Y < Canvas.GetTop(item) + item.Height))
                {
                    rec1 = item;

                    rec1.Height = 0;
                    rec1.Width = 0;

                    Canvas.SetTop(rec1, LastPoint.Y);
                    Canvas.SetLeft(rec1, LastPoint.X);
                }
            }

            if (rec1 != null)
            {
                DragRectangles.Remove(rec1);
            }
        }

        /// <summary>
        /// Method for changing the color of a rectangle by right clicking
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void canDraw_Mouse(object sender, MouseEventArgs e)
        {

            MouseClick = Mouse.GetPosition(canDraw);

            foreach (var item in DragRectangles)
            {
                if ((MouseClick.X > Canvas.GetLeft(item)) && (MouseClick.X < Canvas.GetLeft(item) + item.Width) && (MouseClick.Y > Canvas.GetTop(item)) && (MouseClick.Y < Canvas.GetTop(item) + item.Height))
                {
                    Rectangle rec1 = item;
                    rec1.Fill = myBrush;
                }
            }
        }

        /// <summary>
        /// Used for opening save as dialog
        /// </summary>
        SaveFileDialog sfdImage = new SaveFileDialog();

        /// <summary>
        /// Method for saving the image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Method for saving image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            sfdImage.Filter = "Image Files|*.bmp;*.gif;*.jpg;*.png;*.tif|All files (*.*)|*.*";
            sfdImage.DefaultExt = "png";
            if (sfdImage.ShowDialog().Value)
            {
                canDraw.LayoutTransform = null;

                Size size = new Size(ImageViewer1.Width, ImageViewer1.Height);
                canDraw.Measure(size);
                canDraw.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(canDraw);

                using FileStream outStream = new FileStream(sfdImage.FileName, FileMode.Create);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(outStream);
            }
        }

        /// <summary>
        /// Method for toggling between draw and drag action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggle_Dragging(object sender, RoutedEventArgs e)
        {
            if (MouseActionType == "draging")
            {
                MouseActionType = "drawing";
                Dragging.Background = new SolidColorBrush(Colors.Silver);
            }
            else
            {
                MouseActionType = "draging";
                Delete.Background = new SolidColorBrush(Colors.Silver);
                Dragging.Background = new SolidColorBrush(Colors.Teal);
            }
        }

        /// <summary>
        /// Method for toggling between draw and delete action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggle_Delete(object sender, RoutedEventArgs e)
        {
            if (MouseActionType == "delete")
            {
                MouseActionType = "drawing";
                Delete.Background = new SolidColorBrush(Colors.Silver);
            }
            else
            {
                MouseActionType = "delete";
                Delete.Background = new SolidColorBrush(Colors.Teal);
                Dragging.Background = new SolidColorBrush(Colors.Silver);
            }
        }
    }
}
