using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace EnemyChaseVectorTest
{
    public enum MobType
    {
        Human, Dragon, Ghost, Goblin, Orc, Skeleton
    }

    public class Mob
    {
        public MobType Type;
        public Vector Position, Velocity, Goal;
        public double Speed, SightRange, Angle = 0;// how far can he detect
        public Image Img;
        public List<Mob> Contacts = new List<Mob>();
        public List<Landscape> Map = new List<Landscape>();
        public int HP, Damage;
        public Mob(MobType type, Vector pos, Vector vel,
            double spd, double sight,
            Vector goal)
        {
            Type = type;
            Position = pos;
            Velocity = vel;
            Angle = Velocity.Angle;
            if (type == MobType.Human)
            {
                Img = Properties.Resources.Link;
                HP = 1000;
            }
            else if (type == MobType.Dragon)
            {
                Img = Properties.Resources.Dragon;
                HP = 200;
            }
            else if(type == MobType.Ghost)
            {
                Img = Properties.Resources.Ghost;
                HP = 80;
            }
            else if(type == MobType.Goblin)
            {
                Img = Properties.Resources.Goblin;
                HP = 10;
            }
            else if (type == MobType.Orc)
            {
                Img = Properties.Resources.Orc;
                HP = 20;
            }
            else if (type == MobType.Skeleton)
            {
                Img = Properties.Resources.Skeleton;
                HP = 5;
            }
            if (HP < 5)
            {
                Damage = 1;
            }
            else
            {
                Damage = HP / 5;
            }
            Speed = spd;
            SightRange = sight;
            Goal = goal;
        }

        public void Move(double time)
        {
            Position = Position + Velocity * time;
        }

        public void Sense(List<Mob> newContacts)
        {   // replace old list of contacts with new one...
            Contacts = newContacts;
        }

        public void Sense(List<Landscape> newLandscapes)
        {
            foreach (Landscape ls in newLandscapes)
            {
                if (!Map.Contains(ls))
                {
                    Map.Add(ls);
                }
            }
        }

    }
}
