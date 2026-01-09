using System.Drawing;

namespace CodeGenerator.Shared.ExtensionMethods
{
    /// <summary>
    /// Extension methods for Image/Bitmap conversions
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Converts a Bitmap to an Icon
        /// </summary>
        /// <param name="bitmap">The bitmap to convert</param>
        /// <returns>An Icon instance</returns>
        public static Icon ToIcon(this Bitmap bitmap)
        {
            // Get the icon handle from the bitmap
            IntPtr hIcon = bitmap.GetHicon();
            
            // Create an Icon from the handle
            // Note: We clone the icon so we can destroy the original handle
            Icon tempIcon = Icon.FromHandle(hIcon);
            Icon resultIcon = (Icon)tempIcon.Clone();
            
            // Clean up the handle to avoid memory leaks
            DestroyIcon(hIcon);
            
            return resultIcon;
        }

        /// <summary>
        /// Converts a Bitmap to an Icon with a specific size
        /// </summary>
        /// <param name="bitmap">The bitmap to convert</param>
        /// <param name="size">The desired icon size</param>
        /// <returns>An Icon instance</returns>
        public static Icon ToIcon(this Bitmap bitmap, Size size)
        {
            using var resizedBitmap = new Bitmap(bitmap, size);
            return resizedBitmap.ToIcon();
        }

        /// <summary>
        /// Converts an Image to an Icon
        /// </summary>
        /// <param name="image">The image to convert</param>
        /// <returns>An Icon instance</returns>
        public static Icon ToIcon(this Image image)
        {
            using var bitmap = new Bitmap(image);
            return bitmap.ToIcon();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);
    }
}
