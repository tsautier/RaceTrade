using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RaceTrade
{
    public class KnightRiderScanner : Control
    {
        private Timer animationTimer;
        private int currentPosition = 0;
        private bool movingRight = true;
        private int ledCount = 8;
        private Color scannerColor = Color.FromArgb(255, 0, 0); // KITT red
        private int ledWidth = 4;
        private int spacing = 2;

        public KnightRiderScanner()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);

            animationTimer = new Timer();
            animationTimer.Interval = 100; // Smooth KITT speed
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();

            this.Size = new Size(90, 20);
            this.BackColor = Color.Transparent;
        }

        public Color ScannerColor
        {
            get => scannerColor;
            set
            {
                scannerColor = value;
                Invalidate();
            }
        }

        public int LEDCount
        {
            get => ledCount;
            set
            {
                ledCount = value;
                Invalidate();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Move the scanner position
            if (movingRight)
            {
                currentPosition++;
                if (currentPosition >= ledCount - 1)
                {
                    movingRight = false;
                }
            }
            else
            {
                currentPosition--;
                if (currentPosition <= 0)
                {
                    movingRight = true;
                }
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int totalWidth = (ledWidth * ledCount) + (spacing * (ledCount - 1));
            int startX = 0;
            int centerY = Height / 2;
            int ledHeight = Math.Min(Height - 4, 12);

            // Draw all LEDs
            for (int i = 0; i < ledCount; i++)
            {
                int x = startX + (i * (ledWidth + spacing));
                int y = centerY - (ledHeight / 2);

                // Calculate glow intensity based on distance from current position
                int distance = Math.Abs(i - currentPosition);
                int alpha;

                if (distance == 0)
                {
                    // Brightest LED (current position)
                    alpha = 255;
                }
                else if (distance == 1)
                {
                    // Bright glow on adjacent LEDs
                    alpha = 180;
                }
                else if (distance == 2)
                {
                    // Medium glow
                    alpha = 80;
                }
                else if (distance == 3)
                {
                    // Faint glow
                    alpha = 30;
                }
                else
                {
                    // Off or very dim
                    alpha = 10;
                }

                Color ledColor = Color.FromArgb(alpha, scannerColor);

                using (SolidBrush brush = new SolidBrush(ledColor))
                {
                    // Draw rounded LED
                    e.Graphics.FillRoundedRectangle(
                        brush,
                        x, y, ledWidth, ledHeight,
                        2
                    );
                }

                // Add extra glow for the active LED
                if (distance == 0)
                {
                    using (Pen glowPen = new Pen(Color.FromArgb(100, scannerColor), 2))
                    {
                        e.Graphics.DrawRoundedRectangle(
                            glowPen,
                            x - 1, y - 1, ledWidth + 2, ledHeight + 2,
                            3
                        );
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                animationTimer?.Stop();
                animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    // Extension methods for rounded rectangles
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush,
            float x, float y, float width, float height, float radius)
        {
            using (GraphicsPath path = GetRoundedRectPath(x, y, width, height, radius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen,
            float x, float y, float width, float height, float radius)
        {
            using (GraphicsPath path = GetRoundedRectPath(x, y, width, height, radius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        private static GraphicsPath GetRoundedRectPath(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2;

            path.AddArc(x, y, diameter, diameter, 180, 90);
            path.AddArc(x + width - diameter, y, diameter, diameter, 270, 90);
            path.AddArc(x + width - diameter, y + height - diameter, diameter, diameter, 0, 90);
            path.AddArc(x, y + height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}