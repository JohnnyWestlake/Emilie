﻿// ******************************************************************
// Copyright 2016 (c) Project Emilie / J.Westlake
//
// This code is licensed under the MS-PL License
//
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Composition;

namespace Emilie.UWP.Media
{
    public class CompositionStoryboard : ICompositionTimeline
    {
        public Compositor Compositor => Storyboards.FirstOrDefault()?.Compositor;

        public IList<ICompositionTimeline> Storyboards { get; private set; }

        public CompositionStoryboard(params ICompositionTimeline[] storyboards)
        {
            Storyboards = new List<ICompositionTimeline>(storyboards);
        }

        public CompositionStoryboard(IList<ICompositionTimeline> storyboards)
        {
            Storyboards = storyboards ?? throw new ArgumentNullException(nameof(storyboards));
        }

        public CompositionStoryboard Add(ICompositionTimeline storyboard)
        {
            Storyboards.Add(storyboard);
            return this;
        }

        public CompositionStoryboard AddRange(IEnumerable<ICompositionTimeline> storyboards)
        {
            foreach (var sb in storyboards)
                Storyboards.Add(sb);
            return this;
        }

        public void Start()
        {
            for (int i = 0; i < Storyboards.Count; i++)
            {
                Storyboards[i].Start();
            }
        }

        public void Stop()
        {
            for (int i = 0; i < Storyboards.Count; i++)
            {
                Storyboards[i].Stop();
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    while (Storyboards.Count > 0)
                    {
                        ICompositionTimeline storyboard = Storyboards[0];
                        Storyboards.RemoveAt(0);
                        storyboard.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CompositionStoryboardGroup() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

    }
}
