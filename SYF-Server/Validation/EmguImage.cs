using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using Emgu.CV;
using System.Drawing.Imaging;

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
            get 
            {
                IntPtr retPtr = IntPtr.Zero;
                BitmapData data = _Bitmap.LockBits(
                        new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height),
                        ImageLockMode.ReadWrite,
                        _Bitmap.PixelFormat);

                try
                {
                    retPtr = data.Scan0;
                }
                catch (Exception ex) {}
                finally
                {
                    _Bitmap.UnlockBits(data);
                }

                return retPtr;
            }
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
