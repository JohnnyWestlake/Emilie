using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Emilie.UWP.Maths;

namespace Emilie.UWP.D2D
{
    public class ExplodeableImage
    {
        private CanvasBitmap _bitmap;
        private Vector2 _origin;
        private ExplosionAnimator _animator;
        private Single _scale;
        private Single _explosionScale;



        public ExplodeableImage(CanvasBitmap bitmap, Vector2 origin, float scale = 1.0f, float explosionScale = 1.0f)
        {
            _bitmap = bitmap;
            _origin = origin;
            _scale = scale;
            _explosionScale = explosionScale;

            _animator = new ExplosionAnimator(_bitmap, _bitmap.Bounds.CenterScale(explosionScale));
        }

        public void Draw(CanvasDrawingSession ds, float progress)
        {
            

            if (System.Math.Abs(progress) < float.Epsilon)
            {
                Vector2 cent = _bitmap.Bounds.GetCenter();
                ds.Transform = Matrix3x2.CreateScale(_scale, cent) * Matrix3x2.CreateTranslation(_origin);
                ds.DrawImage(_bitmap);
            }
            else
            {
                ds.Transform = Matrix3x2.CreateTranslation(_origin);
                _animator.Draw(ds, progress);
            }
        }
    }
}
