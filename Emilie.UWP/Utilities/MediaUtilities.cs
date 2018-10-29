using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Emilie.UWP.Utilities
{
    public static class MediaUtilities
    {
        public static Guid GetDecoderId(ref StorageFile file)
        {
            switch (file.FileType)
            {
                case (".jpeg"):
                case (".jpg"):
                    return BitmapDecoder.JpegDecoderId;
                case (".bmp"):
                    return BitmapDecoder.BmpDecoderId;
                case (".tiff"):
                    return BitmapDecoder.TiffDecoderId;
                case (".jxr"):
                case (".hdp"):
                case (".wdp"):
                    return BitmapDecoder.JpegXRDecoderId;
                case (".gif"):
                    return BitmapDecoder.GifDecoderId;
                case (".png"):
                    return BitmapDecoder.PngDecoderId;
                case (".ico"):
                    return BitmapDecoder.IcoDecoderId;
                default:
                    throw new ArgumentException("File type does not have a matching decoder ID. Please verify the file name has a valid file extension");
            }
        }

        public static async Task<StorageFile> CreateCroppedBitmapAsync(
            StorageFile file,
            Rect croppedArea,
            Guid encoderId,
            StorageFile outputfile,
            uint minWidth = 400,
            uint maxWidth = 400)
        {
            Guid decoderID = GetDecoderId(ref file);

            // 1. Open the existing image file
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                // 2. Create decoder to read the existing file
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(decoderID, stream);

                // 3. Calculate the correct scaling values that we'll use to resize the image

                uint scaledwidth = (uint)decoder.PixelWidth;
                uint scaledheight = (uint)decoder.PixelHeight;
                double ratio = (double)scaledwidth / (double)scaledheight;
                double scale = 1.0d;

                scale = (double)minWidth / (double)scaledwidth;

                scaledwidth = minWidth;
                scaledheight = (uint)((double)scaledwidth / (double)ratio);

                double scaledX = (double)croppedArea.X * scale;
                double scaledY = (double)croppedArea.Y * scale;

                double boundHeight = (double)croppedArea.Height * scale;
                double boundWidth = (double)croppedArea.Width * scale;

                double multi = minWidth / boundWidth;

                // 4. Create the bitmap transform that will apply the crop and scaling
                BitmapTransform transform = new BitmapTransform()
                {
                    ScaledHeight = (uint)(scaledheight * multi),
                    ScaledWidth = (uint)(scaledwidth * multi),
                    Bounds = new BitmapBounds()
                    {
                        X = (uint)(scaledX * multi),
                        Y = (uint)(scaledY * multi),
                        Width = (uint)(boundHeight * multi),
                        Height = (uint)(boundWidth * multi)
                    },
                    InterpolationMode = BitmapInterpolationMode.Fant
                };


                // 6. Get pixel data from existing file
                PixelDataProvider pix = await decoder.GetPixelDataAsync(
                    decoder.BitmapPixelFormat,
                    decoder.BitmapAlphaMode,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.ColorManageToSRgb);

                // 7. Open the output file for writing
                using (var saveStream = await outputfile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    // 8. Create our target encoder & set encoding properties
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, saveStream);
                    encoder.SetPixelData(
                        decoder.BitmapPixelFormat,
                        decoder.BitmapAlphaMode,
                        transform.Bounds.Width,
                        transform.Bounds.Height,
                        decoder.DpiX,
                        decoder.DpiY,
                        pix.DetachPixelData());

                    // 9. Persist the encoded data to the file stream
                    await encoder.FlushAsync();
                }
            }

            return outputfile;
        }
    }
}
