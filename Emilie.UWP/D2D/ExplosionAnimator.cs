using Emilie.Core.Maths;
using Emilie.UWP.Maths;
using Microsoft.Graphics.Canvas;
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;

namespace Emilie.UWP.D2D
{
    public class ExplosionAnimator
    {
        static long DEFAULT_DURATION = 0x400;

        private static EasingFunctionBase DEFAULT_INTERPOLATOR = new ExponentialEase { Exponent = 6 };
        private static float END_VALUE = 1.4f;
        private static float X = 5f;
        private static float Y = 20f;
        private static float V = 2f;
        private static float W = 1f;
        private Particle[] mParticles;
        private Rect mBound;

        bool isStarted = false;

        static Random random = new Random();

        public ExplosionAnimator(CanvasBitmap bitmap, Rect explosionBounds)
        {

            mBound = explosionBounds;
            int partLen = 15;
            mParticles = new Particle[partLen * partLen];
            int w = (int)(bitmap.Bounds.Width / (partLen + 2));
            int h = (int)(bitmap.Bounds.Height / (partLen + 2));

            Color[] pixel = bitmap.GetPixelColors();

            for (int i = 0; i < partLen; i++)
            {
                for (int j = 0; j < partLen; j++)
                {
                    mParticles[(i * partLen) + j] = generateParticle(GetPixel(bitmap, pixel, (j + 1) * w, (i + 1) * h), random);
                }
            }

        }

        Color GetPixel(CanvasBitmap bitmap, Color[] pixels, float x, float y)
        {
            return pixels[(int)((y * bitmap.Bounds.Width) + x)];
        }





        public void Draw(CanvasDrawingSession session, float progress)
        {
            //if (!isStarted)
            //    return false;


            foreach (Particle particle in mParticles)
            {
                particle.Advance((progress), END_VALUE);
                if (particle.Alpha > 0f)
                {
                    session.FillCircle(particle.Cx, particle.Cy, particle.Radius, particle.ComputedColor);
                }
            }
        }



        private Particle generateParticle(Color color, Random random)
        {
            Particle particle = new Particle();
            particle.color = color;
            particle.Radius = V;

            if (random.NextFloat() < 0.2f)
            {
                particle.baseRadius = V + ((X - V) * random.NextFloat());
            }
            else
            {
                particle.baseRadius = W + ((V - W) * random.NextFloat());
            }

            float nextFloat = random.NextFloat();
            particle.top = (float)mBound.Height * ((0.18f * random.NextFloat()) + 0.2f);
            particle.top = nextFloat < 0.2f ? particle.top : particle.top + ((particle.top * 0.2f) * random.NextFloat());
            particle.bottom = ((float)mBound.Height * (random.NextFloat() - 0.5f)) * 1.8f;

            float f = nextFloat < 0.2f ? particle.bottom : nextFloat < 0.8f ? particle.bottom * 0.6f : particle.bottom * 0.3f;
            particle.bottom = f;
            particle.mag = 4.0f * particle.top / particle.bottom;
            particle.neg = (-particle.mag) / particle.bottom;

            f = mBound.GetCenter().X + (Y * (random.NextFloat() - 0.5f));
            particle.baseCx = f;
            particle.Cx = f;

            f = mBound.GetCenter().Y + (Y * (random.NextFloat() - 0.5f));
            particle.baseCy = f;
            particle.Cy = f;

            particle.life = END_VALUE / 10 * random.NextFloat();
            particle.overflow = 0.4f * random.NextFloat();
            particle.Alpha = 1f;
            return particle;
        }
    }
}
