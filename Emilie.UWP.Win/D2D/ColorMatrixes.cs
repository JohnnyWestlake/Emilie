using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Emilie.UWP.D2D
{
    public class ColorMatrixes
    {
        public static Matrix5x4 CreateOpacityMatrix(float opacity)
        {
            return new Matrix5x4
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = opacity,
                M51 = 0,
                M52 = 0,
                M53 = 0,
                M54 = 0,
            };
        }

        public static Matrix5x4 CreateBrightnessMatrix(float brightness)
        {
            if (brightness > 1 || brightness < 0)
            {
                throw new ArgumentOutOfRangeException("brightness", "brightness should be between -100 and 100.");
            }

            float brightnessFactor = (brightness * 2) - 1;

            return new Matrix5x4
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 1,
                M51 = brightnessFactor,
                M52 = brightnessFactor,
                M53 = brightnessFactor,
                M54 = 0
            };
        }

        public static Matrix5x4 CreateContrastMatrix(float contrast)
        {
            if (contrast > 1 || contrast < 0)
            {
                throw new ArgumentOutOfRangeException("contrast", "contrast should be between -100 and 100.");
            }

            float contrastFactor = (contrast * 2) - 1;


            // Stop at -1 to prevent inversion. 
            contrastFactor++;
            float factorTransform = 0.5f * (1.0f - contrastFactor);


            return new Matrix5x4
            {
                M11 = contrastFactor,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = contrastFactor,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = contrastFactor,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = contrastFactor,
                M51 = factorTransform,
                M52 = factorTransform,
                M53 = factorTransform,
                M54 = 0,
            };
        }

        public static Matrix5x4 Greyscale
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 0.33f,
                    M12 = 0.33f,
                    M13 = 0.33f,
                    M14 = 0,
                    M21 = 0.59f,
                    M22 = 0.59f,
                    M23 = 0.59f,
                    M24 = 0,
                    M31 = 0.11f,
                    M32 = 0.11f,
                    M33 = 0.11f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1,
                    M51 = 0,
                    M52 = 0,
                    M53 = 0,
                    M54 = 0,
                };
            }
        }

        public static Matrix5x4 Sepia
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 0.393f,
                    M12 = 0.349f,
                    M13 = 0.272f,
                    M14 = 0,
                    M21 = 0.769f,
                    M22 = 0.686f,
                    M23 = 0.534f,
                    M24 = 0,
                    M31 = 0.189f,
                    M32 = 0.168f,
                    M33 = 0.131f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1,
                    M51 = 0,
                    M52 = 0,
                    M53 = 0,
                    M54 = 0,
                };
            }
        }

        public static Matrix5x4 Negative
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = -1f,
                    M12 = 0f,
                    M13 = 0f,
                    M14 = 0,
                    M21 = 0,
                    M22 = -1f,
                    M23 = 0,
                    M24 = 0,
                    M31 = 0,
                    M32 = 0,
                    M33 = -1f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = 1f,
                    M52 = 1f,
                    M53 = 1f,
                    M54 = 1f,
                };
            }
        }

        public static Matrix5x4 BlackAndWhite
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 1.5f,
                    M12 = 1.5f,
                    M13 = 1.5f,
                    M14 = 0,
                    M21 = 1.5f,
                    M22 = 1.5f,
                    M23 = 1.5f,
                    M24 = 0,
                    M31 = 1.5f,
                    M32 = 1.5f,
                    M33 = 1.5f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = -1f,
                    M52 = -1f,
                    M53 = -1f,
                    M54 = 0f,
                };
            }
        }

        public static Matrix5x4 Polaroid
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 1.438f,
                    M12 = -0.062f,
                    M13 = -0.062f,
                    M14 = 0,
                    M21 = -0.122f,
                    M22 = 1.378f,
                    M23 = -0.122f,
                    M24 = 0,
                    M31 = -0.016f,
                    M32 = -0.016f,
                    M33 = 1.483f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = -0.03f,
                    M52 = 0.05f,
                    M53 = -0.02f,
                    M54 = 0f
                };
            }
        }

        public static Matrix5x4 PolaroidAlt
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 1.638f,
                    M12 = -0.062f,
                    M13 = -0.262f,
                    M14 = 0,
                    M21 = -0.122f,
                    M22 = 1.378f,
                    M23 = -0.122f,
                    M24 = 0,
                    M31 = -0.016f,
                    M32 = -0.016f,
                    M33 = 1.383f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = -0.06f,
                    M52 = 0.05f,
                    M53 = -0.05f,
                    M54 = 0f
                };
            }
        }

        public static Matrix5x4 Lomograph
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 1.5f,
                    M12 = 0f,
                    M13 = 0f,
                    M14 = 0,
                    M21 = 0f,
                    M22 = 1.45f,
                    M23 = 0f,
                    M24 = 0,
                    M31 = 0f,
                    M32 = 0f,
                    M33 = 1.09f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = -0.1f,
                    M52 = 0.05f,
                    M53 = -0.08f,
                    M54 = 0f
                };
            }
        }


        public static Matrix5x4 Cool
        {

            get
            {
                return new Matrix5x4
                {
                    M11 = 0.3f,
                    M12 = 0.3f,
                    M13 = 0.33f,
                    M14 = -0.3f,
                    M21 = 0.4f,
                    M22 = 1f,
                    M23 = 0.1f,
                    M24 = -0.13f,
                    M31 = -0.01f,
                    M32 = 0f,
                    M33 = 1f,
                    M34 = 0.07f,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = 0f,
                    M52 = -0.1f,
                    M53 = 0f,
                    M54 = -0.09f
                };
            }
        }

        public static Matrix5x4 Vintage
        {
            get
            {
                return new Matrix5x4
                {
                    M11 = 2f,
                    M12 = 0f,
                    M13 = 0f,
                    M14 = 0,
                    M21 = 0f,
                    M22 = 2f,
                    M23 = 0f,
                    M24 = 0,
                    M31 = 0f,
                    M32 = 0f,
                    M33 = 1f,
                    M34 = 0,
                    M41 = 0,
                    M42 = 0,
                    M43 = 0,
                    M44 = 1f,
                    M51 = -0.055f,
                    M52 = -0.2f,
                    M53 = -0.3f,
                    M54 = 0f
                };
            }
        }
    }
}
