using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SYF_Server.Datamaps;
using System.Drawing;
using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace SYF_Server.Validation
{
    class FaceImageValidator : IValidator
    {
        private SqlUser _User;
        public SqlUser User
        {
            get
            {
                return _User;
            }
        }

        private Bitmap _Image;
        public Bitmap Image
        {
            get
            {
                return _Image;
            }
        }

        public FaceImageValidator(string User, Bitmap Image)
        {
            _User = Database.GetInstance().GetUserByName(User);
            _Image = Image;
        }

        public FaceImageValidator(SqlUser User, Bitmap Image)
        {
            _User = User;
            _Image = Image;
        }

        public bool Validate()
        {
            FisherFaceRecognizer rec = new FisherFaceRecognizer(0, 3500);

            List<Image<Gray, Byte>> Images = new List<Image<Gray, Byte>>();
            List<int> Labels = new List<int>();

            int i = 0;
            foreach (Bitmap Image in User.FaceImages)
            {
                Images.Add(new Image<Gray, Byte>(Image));
                Labels.Add(i);
                i++;
            }

            rec.Train(Images.ToArray(), Labels.ToArray());

            Image<Gray, Byte> TestImage = new Image<Gray, Byte>(this.Image);

            FaceRecognizer.PredictionResult result = rec.Predict(TestImage);
            result.GetType();

            if (result.Label > 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public Messages.ErrorMessage LastErrorMessage()
        {
            throw new NotImplementedException();
        }
    }
}
