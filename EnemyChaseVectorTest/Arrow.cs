using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EnemyChaseVectorTest
{
    public delegate void HitGroundMsg(Arrow ar);

    public class Arrow
    {
        public Vector Position, Velocity;
        public Vector Origin;
        public Image Img;
        public double Angle;
        public event HitGroundMsg HitGround;

        public Arrow(Vector pos, Vector vel,
            Image img)
        {
            Origin = pos;
            Position = pos;
            Velocity = vel;
            Img = img;
            Angle = Velocity.Angle;
        }

        public void Move(double time)
        {
            Position = Position + Velocity * time;
            Vector pointing = Position - Origin;
            double dist = pointing.Magnitude;
            if (dist > 500)
            {
                if (HitGround != null)
                {
                    HitGround(this);//uses delegate as msg format
                }
            }
        }
    }
}
