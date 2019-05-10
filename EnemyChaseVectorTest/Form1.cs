using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EnemyChaseVectorTest
{
    public partial class Form1 : Form
    {
        protected Mob goodguy;
        protected List<Mob> badguys = new List<Mob>();
        protected List<Landscape> overallMap
            = new List<Landscape>();
        protected List<Arrow> arrows = new List<Arrow>();
        protected List<int> arrowsWhichHitGround = new List<int>();
        protected Timer tmr;
        public static Random rand = new Random();
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint, true);
            UpdateStyles();

            for (int i = 0; i < 10; i++)
            {
                int r = rand.Next(0, 101);
                MobType mt;
                if(r < 5)
                {
                    mt = MobType.Dragon;
                }
                else if (r < 15)
                {
                    mt = MobType.Ghost;
                }
                else if (r < 55)
                {
                    mt = MobType.Goblin;
                }
                else if (r < 85)
                {
                    mt = MobType.Orc;
                }
                else
                {
                    mt = MobType.Skeleton;
                }
                Mob badguy = new Mob(mt, new Vector(rand.Next(0, 1366),
                    rand.Next(0, 768)), new Vector(0, 0),
                    rand.Next(1, 7), 300,
                    new Vector(rand.Next(100, 1300), rand.Next(0, 768)));
                badguys.Add(badguy);
            }

            goodguy = new Mob(MobType.Human, new Vector(30, 30), new Vector(0, 3),
                10, 200, new Vector(30, 30));

            CreateForest(680, 350, 900, 350, 50);
            CreateWall(100, 100, 5, true);
            tmr = new Timer();
            tmr.Interval = 17;
            tmr.Tick += tmr_Tick;
            tmr.Start();
        }

        void tmr_Tick(object sender, EventArgs e)
        {
            Text = "HP = " + goodguy.HP;
            if (goodguy.HP <= 0)
            {
                tmr.Stop();
                MessageBox.Show("You died HP = " + goodguy.HP);
                double oldSpeed = goodguy.Speed;
                Mob undeadLink = new Mob(MobType.Skeleton, goodguy.Position, new Vector(0, 0),
                    10, 200, new Vector(0, 0));
                badguys.Add(undeadLink);
                goodguy = new Mob(MobType.Human, new Vector(30, 30), new Vector(0, 0),
                oldSpeed, 200, new Vector(0, 0));
                tmr.Start();
            }
            DetectNearbyMobs();
            DetectNearbyLandscapes();
            CalcNewGoal(goodguy);
            goodguy.Move(1.0);
            // iterate thru all badguys...
            foreach (Mob badguy in badguys)
            {
                Vector pointing2 = goodguy.Position - badguy.Position;
                double dist = pointing2.Magnitude;//calc distance to link
                if (dist < badguy.SightRange)
                {   // link is close enough to see, so target him
                    Vector unit2 = pointing2.Unitized;
                    badguy.Velocity = badguy.Speed * unit2;
                    if (dist < 5)
                    {
                        badguy.Velocity = new Vector(0, 0);
                    }
                    badguy.Angle = badguy.Velocity.Angle;
                }
                else
                {   // link is out of sight, so head back to the goal
                    CalcNewGoal(badguy);
                }
                badguy.Move(1.0);// allow badguy to move
            }

            arrowsWhichHitGround.Sort();
            for (int i = arrowsWhichHitGround.Count - 1; i >=0; i--)
            {
                if(arrows.Count > 0)
                {
                    arrows.RemoveAt(arrowsWhichHitGround[i]);
                }
            }
            arrowsWhichHitGround.Clear();

            foreach (Arrow ar in arrows)
            {
                ar.Move(1.0);
            }

            CheckCollisions();

            Invalidate();// bring on the painter...
        }

        private void CalcNewGoal(Mob m)
        {
            Vector pointing = m.Goal - m.Position;
            Vector unit = pointing.Unitized;
            m.Velocity = m.Speed * unit;
            if (pointing.Magnitude < 5)
            {
                m.Velocity = new Vector(0, 0);
            }
            m.Angle = m.Velocity.Angle;// orient the mob to face dir he's moving
        }

        private void DetectNearbyMobs()
        {
            // 1st determine which mobs are seeable by goodguy...
            List<Mob> newContacts = new List<Mob>();
            foreach (Mob m in badguys)
            {
                Vector pointing = m.Position - goodguy.Position;
                double dist = pointing.Magnitude;
                if (dist < goodguy.SightRange)
                {
                    newContacts.Add(m);
                }
            }
            goodguy.Sense(newContacts);// notify goodguy of who he can see
        }

        protected void DetectNearbyLandscapes()
        {
            List<Landscape> newScapes = new List<Landscape>();
            foreach (Landscape ls in overallMap)
            {
                Vector pointing = ls.Position - goodguy.Position;
                double dist = pointing.Magnitude;
                if (dist < goodguy.SightRange)
                {
                    newScapes.Add(ls);
                }
            }
            goodguy.Sense(newScapes);
//            Text = goodguy.Map.Count.ToString() + " landscapes seen";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics bob = e.Graphics;
            bob.Clear(Color.Black);

            foreach (Landscape ls in goodguy.Map)
            {
                bob.TranslateTransform((float)ls.Position.X, (float)ls.Position.Y);
                bob.TranslateTransform(-ls.Img.Width / 2, -ls.Img.Height / 2);
                bob.DrawImage(ls.Img, new Point());
                bob.ResetTransform();
            }

            foreach (Mob badguy in goodguy.Contacts)
            {
                bob.TranslateTransform((float)badguy.Position.X,
                    (float)badguy.Position.Y);
                bob.RotateTransform((float)badguy.Angle);
                bob.TranslateTransform(-badguy.Img.Width / 2, -badguy.Img.Height / 2);
                bob.DrawImage(badguy.Img, new Point());
                bob.ResetTransform();
            }

            foreach (Arrow ar in arrows)
            {
                bob.TranslateTransform((float)ar.Position.X, (float)ar.Position.Y);
                bob.RotateTransform((float)ar.Angle);
                bob.TranslateTransform(-ar.Img.Width / 2, -ar.Img.Height / 2);
                bob.DrawImage(ar.Img, new Point());
                bob.ResetTransform();
            }

            bob.TranslateTransform((float)goodguy.Position.X,
                (float)goodguy.Position.Y);
            bob.RotateTransform((float)goodguy.Angle);
            bob.TranslateTransform(-goodguy.Img.Width / 2, -goodguy.Img.Height / 2);
            bob.DrawImage(goodguy.Img, new Point());
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Vector pointing = new Vector(e.X, e.Y) - goodguy.Position;
                Vector unit = pointing.Unitized;
                Vector arVel = 20 * unit;// speed of the arrow
                int numArrows = arrows.Count;
                Arrow ar;
                if (numArrows % 4 == 0)
                {
                    ar = new Arrow(goodguy.Position, arVel, Properties.Resources.ArrowRed);
                }
                else
                {
                    ar = new Arrow(goodguy.Position, arVel, Properties.Resources.Arrow);
                }
                ar.HitGround += ar_HitGround;
                arrows.Add(ar);
            }
            else if (e.Button == MouseButtons.Right)
            {
                goodguy.Goal = new Vector(e.X, e.Y);
            }
        }

        void ar_HitGround(Arrow ar)
        {
            ar.Velocity = new Vector(0, 0);
            arrowsWhichHitGround.Add(arrows.IndexOf(ar));
        }

        protected void CheckCollisions()
        {
            List<int> mobsToDelete = new List<int>();
            List<int> arrowsToDelete = new List<int>();
                foreach (Mob m1 in badguys)
                {
                    foreach (Arrow ar in arrows)
                    {
                        Vector pointing = m1.Position - ar.Position;
                        double dist = pointing.Magnitude;
                        if (dist < (ar.Img.Height / 2 + m1.Img.Width / 2))
                        {   // we have collision
                            int damage = rand.Next(5, 10);
                            KnockBackFromArrow(m1, damage);
                            m1.HP -= damage;
                            if (m1.HP <= 0)
                            {
                                int idx = badguys.IndexOf(m1);
                                if (!mobsToDelete.Contains(idx))
                                {
                                    mobsToDelete.Add(idx);
                                }
                            }
                            int idxAr = arrows.IndexOf(ar);
                            if (!arrowsToDelete.Contains(idxAr))
                            {
                                arrowsToDelete.Add(idxAr);
                            }
                        }
                    }
                    // now check mob vs mob collision
                    foreach (Mob m2 in badguys)
                    {
                        if (m1 != m2)
                        {
                            Vector pointing = m2.Position - m1.Position;
                            double dist = pointing.Magnitude;
                            if (dist < (m1.Img.Width / 2 + m2.Img.Width / 2))
                            {// collided, so knock back from eachother
                                KnockBackFromMob(m1, m2);
                            }
                        }
                    }
                    // now check for goodguy vs mob collision
                    Vector pointingAtGoodGuy = goodguy.Position - m1.Position;
                    double distToGoodGuy = pointingAtGoodGuy.Magnitude;
                    if (distToGoodGuy < (goodguy.Img.Width / 2 + m1.Img.Width / 2))
                    {// collided, so knock back from eachother
                        KnockBackFromMob(goodguy, m1);
                    }

                    foreach (Landscape ls in overallMap)
                    {
                        if (ls.Passable == false)
                        {
                            Vector pointingToWall = ls.Position - m1.Position;
                            double dist = pointingToWall.Magnitude;
                            if (dist < (m1.Img.Width / 2 + ls.Img.Width / 2))
                            {
                                KnockBackFromArrow(m1, 0);
                            }
                        }
                    }
                }

                foreach (Landscape ls in overallMap)
                {
                    if (ls.Passable == false)
                    {
                        Vector pointingToWall = ls.Position - goodguy.Position;
                        double dist = pointingToWall.Magnitude;
                        if (dist < (goodguy.Img.Width / 2 + ls.Img.Width / 2))
                        {
                            KnockBackFromArrow(goodguy, 0);
                        }
                    }
                }
            mobsToDelete.Sort();
            for (int i = mobsToDelete.Count - 1; i >= 0; i-- )
            {
                badguys.RemoveAt(mobsToDelete[i]);
            }

            arrowsToDelete.Sort();
            for (int i = arrowsToDelete.Count - 1; i >= 0; i--)
            {
                arrows.RemoveAt(arrowsToDelete[i]);
            }
        }

        private void KnockBackFromArrow(Mob m, int damage)
        {
            //knock back the target of the arrow...
            if (m.Velocity.Magnitude > 0)
            {
                Vector reverse = -1 * m.Velocity;
                Vector unitReverse = reverse.Unitized;
                Vector moveBack;
                if (damage > 0)
                {
                    moveBack = damage * unitReverse;
                }
                else
                {
                    moveBack = new Vector(0, 0);
                }
                m.Position = m.Position + moveBack;
            }
        }
        protected void KnockBackFromLandscape(Mob m, Landscape ls)
        {
            Vector pointing;
        }

        protected void KnockBackFromMob(Mob m1, Mob m2)
        {
            Vector pointing = m1.Position - m2.Position;
            Vector unit = pointing.Unitized;
            Vector moveBack = m2.Damage * unit;
            m1.Position = m1.Position + moveBack;
            Vector moveBack2 = -m1.Damage * unit;
            m2.Position = m2.Position + moveBack;
            if (m1.Type == MobType.Human)
            {
                m1.HP -= m2.Damage;
            }
            else if (m2.Type == MobType.Human)
            {
                m2.HP -= m1.Damage;
            }
        }

        protected void GenerateRandomLandscapes(int num)
        {
            for (int i = 0; i < num; i++)
            {
                int r = rand.Next(0, 2);
                LandscapeType lt = (LandscapeType)r;
                Landscape ls = new Landscape(lt, new Vector(rand.Next(0, 1366), rand.Next(0, 768)));
                overallMap.Add(ls);
            }
        }

        protected void CreateWall(int startX, int startY, int num, bool hor)
        {
            int width = Properties.Resources.block.Width;
            int height = Properties.Resources.block.Height;
            if (hor)
            {
                for (int x = startX; x < startX + num * width; x += width)
                {
                    Landscape ls = new Landscape(LandscapeType.Wall,
                        new Vector(x, startY));
                    overallMap.Add(ls);
                }
            }
            else
            {
                for (int y = startY; y < startY + num * height; y += height)
                {
                    Landscape ls = new Landscape(LandscapeType.Wall,
                        new Vector(startX, y));
                    overallMap.Add(ls);
                }
            }
        }

        protected void CreateForest(int cx, int cy, int radX, int radY, int density)
        {
            double radAvg = (radX + radY) / 2;
            for (int y = cy - radY; y <= cy + radY; y+= (50 + rand.Next(-10, 11)))
            {
                for (int x = cx - radX; x <= cx + radX; x += (50 + rand.Next(-10, 11)))
                {   // calc dist to the center of the forest...
                    int dx = x - cx;
                    int dy = y - cy;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    double prob = density * (1 - (dist / radAvg));
                    int r = rand.Next(0, 101);
                    if (r <= prob)
                    {
                        Landscape ls = new Landscape(LandscapeType.Tree, new Vector(x, y));
                        overallMap.Add(ls);
                    }
                }
            }
        }
    }
}
