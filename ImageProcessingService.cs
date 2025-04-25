using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PicEditor
{
    public static class ImageProcessingService
    {
        public static BitmapSource RotateImage(BitmapSource source, double angle)
        {
            if (source == null) return null;

            var transform = new RotateTransform(angle);
            var transformedBitmap = new TransformedBitmap(source, transform);
            return transformedBitmap;
        }

        public static BitmapSource ScaleImage(BitmapSource source, double scale)
        {
            if (source == null) return null;

            var transform = new ScaleTransform(scale, scale);
            var transformedBitmap = new TransformedBitmap(source, transform);
            return transformedBitmap;
        }

        public static BitmapSource CropImage(BitmapSource source, Int32Rect cropArea)
        {
            if (source == null) return null;

            var croppedBitmap = new CroppedBitmap(source, cropArea);
            return croppedBitmap;
        }

        public static BitmapSource ApplyTransformations(BitmapSource source, double scale, double rotation)
        {
            if (source == null) return null;

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(scale, scale));
            transformGroup.Children.Add(new RotateTransform(rotation));

            var transformedBitmap = new TransformedBitmap(source, transformGroup);
            return transformedBitmap;
        }

        public static BitmapSource CreateBitmapSourceFromBitmap(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null) return null;

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                var bitmapSource = BitmapSource.Create(
                    bitmapData.Width,
                    bitmapData.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgr32,
                    null,
                    bitmapData.Scan0,
                    bitmapData.Stride * bitmapData.Height,
                    bitmapData.Stride);

                return bitmapSource;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }
    }
} 