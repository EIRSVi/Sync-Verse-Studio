using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SyncVerseStudio.Helpers
{
    /// <summary>
    /// Extension methods for Graphics operations
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Fill a rounded rectangle
        /// </summary>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, int radius)
        {
            if (graphics == null || brush == null)
                return;

            try
            {
                using (var path = GetRoundedRectanglePath(rect, radius))
                {
                    graphics.FillPath(brush, path);
                }
            }
            catch
            {
                // Fallback to regular rectangle
                graphics.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draw a rounded rectangle outline
        /// </summary>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, int radius)
        {
            if (graphics == null || pen == null)
                return;

            try
            {
                using (var path = GetRoundedRectanglePath(rect, radius))
                {
                    graphics.DrawPath(pen, path);
                }
            }
            catch
            {
                // Fallback to regular rectangle
                graphics.DrawRectangle(pen, rect);
            }
        }

        /// <summary>
        /// Create a GraphicsPath for a rounded rectangle
        /// </summary>
        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            
            if (radius <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            // Ensure radius doesn't exceed rectangle dimensions
            radius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height / 2));

            // Create rounded rectangle path
            path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Draw text with shadow effect
        /// </summary>
        public static void DrawTextWithShadow(this Graphics graphics, string text, Font font, Brush textBrush, Brush shadowBrush, PointF location, int shadowOffset = 1)
        {
            if (graphics == null || string.IsNullOrEmpty(text) || font == null)
                return;

            try
            {
                // Draw shadow
                if (shadowBrush != null)
                {
                    var shadowLocation = new PointF(location.X + shadowOffset, location.Y + shadowOffset);
                    graphics.DrawString(text, font, shadowBrush, shadowLocation);
                }

                // Draw main text
                graphics.DrawString(text, font, textBrush, location);
            }
            catch
            {
                // Fallback to regular text
                graphics.DrawString(text, font, textBrush, location);
            }
        }

        /// <summary>
        /// Draw a gradient background
        /// </summary>
        public static void FillGradientRectangle(this Graphics graphics, Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            if (graphics == null || rect.Width <= 0 || rect.Height <= 0)
                return;

            try
            {
                using (var brush = new LinearGradientBrush(rect, startColor, endColor, mode))
                {
                    graphics.FillRectangle(brush, rect);
                }
            }
            catch
            {
                // Fallback to solid color
                using (var brush = new SolidBrush(startColor))
                {
                    graphics.FillRectangle(brush, rect);
                }
            }
        }
    }
}