using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.IO;

namespace PicEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private BitmapSource originalImage;
    private double currentScale = 1.0;
    private double currentRotation = 0.0;
    private Border imageContainer;

    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.SizeChanged += MainWindow_SizeChanged;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // 获取图片容器
        imageContainer = (Border)FindName("ImageContainer");
    }

    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (MainImage.Source != null && imageContainer != null)
        {
            // 窗口大小改变时，重新计算缩放比例
            currentScale = CalculateInitialScale();
            ScaleSlider.Value = currentScale;
            UpdateImageTransform();
        }
    }

    private double CalculateInitialScale()
    {
        if (MainImage.Source == null || imageContainer == null) return 1.0;

        // 获取图片实际尺寸
        double imageWidth = MainImage.Source.Width;
        double imageHeight = MainImage.Source.Height;

        // 获取显示区域尺寸（考虑边距）
        double containerWidth = imageContainer.ActualWidth - 20; // 减去边距
        double containerHeight = imageContainer.ActualHeight - 20;

        // 计算水平和垂直方向的缩放比例
        double scaleX = containerWidth / imageWidth;
        double scaleY = containerHeight / imageHeight;

        // 取较小的比例，确保图片完全显示
        return Math.Min(scaleX, scaleY);
    }

    private void OpenImage_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp|所有文件|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                MainImage.Source = originalImage;
                
                // 计算并设置初始缩放比例
                currentScale = CalculateInitialScale();
                currentRotation = 0.0;
                ScaleSlider.Value = currentScale;
                RotateSlider.Value = 0.0;
                UpdateImageTransform();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开图片: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void SaveImage_Click(object sender, RoutedEventArgs e)
    {
        if (MainImage.Source == null)
        {
            MessageBox.Show("没有图片可以保存", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "PNG图片|*.png|JPEG图片|*.jpg|BMP图片|*.bmp",
            DefaultExt = ".png"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                BitmapSource source = (BitmapSource)MainImage.Source;
                using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = saveFileDialog.FilterIndex switch
                    {
                        1 => new PngBitmapEncoder(),
                        2 => new JpegBitmapEncoder(),
                        3 => new BmpBitmapEncoder(),
                        _ => new PngBitmapEncoder()
                    };
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存图片失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MainImage.Source != null)
        {
            currentScale = e.NewValue;
            UpdateImageTransform();
        }
    }

    private void RotateSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (MainImage.Source != null)
        {
            currentRotation = e.NewValue;
            UpdateImageTransform();
        }
    }

    private void UpdateImageTransform()
    {
        if (MainImage.Source == null) return;

        // 获取图片的实际尺寸
        double imageWidth = MainImage.Source.Width;
        double imageHeight = MainImage.Source.Height;

        // 创建变换组
        TransformGroup transformGroup = new TransformGroup();

        // 添加缩放变换
        ScaleTransform scaleTransform = new ScaleTransform(currentScale, currentScale);
        transformGroup.Children.Add(scaleTransform);

        // 创建旋转变换
        RotateTransform rotateTransform = new RotateTransform(currentRotation);
        transformGroup.Children.Add(rotateTransform);

        // 应用变换
        MainImage.RenderTransform = transformGroup;
    }

    private void RotateLeftButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainImage.Source != null)
        {
            currentRotation -= 90;
            if (currentRotation < 0) currentRotation += 360;
            RotateSlider.Value = currentRotation;
            UpdateImageTransform();
        }
    }

    private void RotateRightButton_Click(object sender, RoutedEventArgs e)
    {
        if (MainImage.Source != null)
        {
            currentRotation += 90;
            if (currentRotation >= 360) currentRotation -= 360;
            RotateSlider.Value = currentRotation;
            UpdateImageTransform();
        }
    }

    private void RotateImage_Click(object sender, RoutedEventArgs e)
    {
        if (MainImage.Source != null)
        {
            currentRotation += 90;
            if (currentRotation >= 360) currentRotation = 0;
            RotateSlider.Value = currentRotation;
            UpdateImageTransform();
        }
    }
}