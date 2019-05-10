using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Res = EnemyChaseVectorTest.Properties.Resources;
namespace EnemyChaseVectorTest
{
    public enum LandscapeType
    {
        Tree, Rock, TreasureChest, House, Wall, Stairs
    }

    public class Landscape
    {
        public LandscapeType Type;
        public Vector Position;
        public bool Passable;
        public Image Img;

        public Landscape(LandscapeType type, Vector pos)
        {
            Type = type;
            Position = pos;
            if (type == LandscapeType.House || type == LandscapeType.Rock
                || type == LandscapeType.Wall)
            {
                Passable = false;
            }
            else
            {
                Passable = true;
            }

            SetImage(type);
        }

        private void SetImage(LandscapeType type)
        {
/*            if (type == LandscapeType.House)
            {
                Img = Properties.Resources.House;
            }
            else if (type == LandscapeType.Rock)
            {
                Img = Properties.Resources.rock;
            }
            else if (type == LandscapeType.Stairs)
            {
                Img = Properties.Resources.stairs;
            }
            else if (type == LandscapeType.TreasureChest)
            {
                Img = Properties.Resources.chest;
            }
            else*/
            if (type == LandscapeType.Tree)
            {
                int r = Form1.rand.Next(0, 20);
                if(r == 0)
                {
                    Img = Res.foliage1;
                }
                else if (r == 1)
                {
                    Img = Res.foliage2;
                }
                else if(r == 2)
                {
                    Img = Res.foliage3;
                }
                else if (r == 3)
                {
                    Img = Res.foliage4;
                }
                else if (r == 4)
                {
                    Img = Res.palmtree1;
                }
                else if (r == 5)
                {
                    Img = Res.palmtree2;
                }
                else if (r == 6)
                {
                    Img = Res.palmtree3;
                }
                else if (r == 7)
                {
                    Img = Res.palmtree4;
                }
                else if (r == 8)
                {
                    Img = Res.palmtree5;
                }
                else if (r == 9)
                {
                    Img = Res.palmtree6;
                }
                else if (r == 10)
                {
                    Img = Res.tree1;
                }
                else if (r == 11)
                {
                    Img = Res.tree2;
                }
                else if (r == 12)
                {
                    Img = Res.tree3;
                }
                else if (r == 13)
                {
                    Img = Res.tree4;
                }
                else if (r == 14)
                {
                    Img = Res.tree5;
                }
                else if (r == 15)
                {
                    Img = Res.tree6;
                }
                else if (r == 16)
                {
                    Img = Res.tree7;
                }
                else if (r == 17)
                {
                    Img = Res.tree8;
                }
                else if (r == 18)
                {
                    Img = Res.tree9;
                }
                else if (r == 19)
                {
                    Img = Res.treeDead;
                }
                //                Img = Properties.Resources.Tree;
            }
            else
            {
                Img = Properties.Resources.block;
            }
        }
    }
}
