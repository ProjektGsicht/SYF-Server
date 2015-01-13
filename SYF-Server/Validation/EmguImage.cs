using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Emgu.CV;

namespace SYF_Server.Validation
{
    class EmguImage : IImage
    {
        private Bitmap _Bitmap;
        public Bitmap Bitmap
        {
            get { return _Bitmap; }
        }

        public EmguImage(Bitmap Image)
        {
            _Bitmap = Image;
        }

        public void MinMax(out double[] minValues, out double[] maxValues, out Point[] minLocations, out Point[] maxLocations)
        {
            throw new NotImplementedException();
        }

        public int NumberOfChannels
        {
            get 
            { 
                // ARGB
                return 4; 
            }
        }

        public IntPtr Ptr
        {
            get { throw new NotImplementedException(); }
        }

        public void Save(string fileName)
        {
            Bitmap.Save(fileName);
        }

        public Size Size
        {
            get { return Bitmap.Size; }
        }

        public IImage[] Split()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            Bitmap.Dispose();
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new EmguImage((Bitmap)Bitmap.Clone());
        }
    }
}
