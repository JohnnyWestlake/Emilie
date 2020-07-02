using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Emilie.UWP.D2D
{
    public class Particle
    {
        const float V = 2f;

        public Color color;
        public float Alpha;
        public float Cx;
        public float Cy;
        public float Radius;
        public float baseCx;
        public float baseCy;
        public float baseRadius;
        public float top;
        public float bottom;
        public float mag;
        public float neg;
        public float life;
        public float overflow;


        public Color ComputedColor
            => Color.FromArgb((byte)(255 * Alpha * ((double)color.A / 255d)), color.R, color.G, color.B);

        public Color colorBaseColor;


        public void Advance(float factor, float END_VALUE)
        {
            float f = 0f;
            float normalization = factor / END_VALUE;
            if (normalization < life || normalization > 1f - overflow)
            {
                Alpha = 0f;
                return;
            }
            normalization = (normalization - life) / (1f - life - overflow);
            float f2 = normalization * END_VALUE;
            if (normalization >= 0.7f)
            {
                f = (normalization - 0.7f) / 0.3f;
            }
            Alpha = 1f - f;
            f = bottom * f2;
            Cx = baseCx + f;
            Cy = (float)(baseCy - this.neg * Math.Pow(f, 2.0)) - f * mag;
            Radius = V + (baseRadius - V) * f2;
        }
    }
}
