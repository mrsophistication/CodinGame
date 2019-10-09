using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodinGame.Unleash_the_Geek
{
    public class Player
    {
        static void Main(string[] args)
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int width = int.Parse(inputs[0]);
            int height = int.Parse(inputs[1]); // size of the map
            List<Robot> robots = new List<Robot>();
            List<Cell> cells = new List<Cell>();

            bool firstTurn = true;

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');
                int myScore = int.Parse(inputs[0]); // Amount of ore delivered
                int opponentScore = int.Parse(inputs[1]);
                for (int i = 0; i < height; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    for (int j = 0; j < width; j++)
                    {
                        string ore = inputs[2 * j];// amount of ore or "?" if unknown
                        int hole = int.Parse(inputs[2 * j + 1]);// 1 if cell has a hole

                        if (!firstTurn)
                        {
                            foreach (var c in cells)
                                if (c.X == i && c.Y == j)
                                    c.Update(ore, hole);
                        }
                        else
                        {
                            cells.Add(new Cell(i, j, ore, hole));
                        }
                    }
                }

                inputs = Console.ReadLine().Split(' ');
                int entityCount = int.Parse(inputs[0]); // number of entities visible to you
                int radarCooldown = int.Parse(inputs[1]); // turns left until a new radar can be requested
                int trapCooldown = int.Parse(inputs[2]); // turns left until a new trap can be requested
                for (int i = 0; i < entityCount; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    int id = int.Parse(inputs[0]); // unique id of the entity
                    int type = int.Parse(inputs[1]); // 0 for your robot, 1 for other robot, 2 for radar, 3 for trap
                    int x = int.Parse(inputs[2]);
                    int y = int.Parse(inputs[3]); // position of the entity
                    int item = int.Parse(inputs[4]); // if this entity is a robot, the item it is carrying (-1 for NONE, 2 for RADAR, 3 for TRAP, 4 for ORE)

                    if (!firstTurn)
                    {
                        Console.Error.WriteLine($"update robot's list");
                        if (type == 0)
                            foreach (var r in robots)
                                if (id == r.ID)
                                {
                                    r.X = x;
                                    r.Y = y;
                                    r.Item = item;
                                }
                    }
                    else
                    {
                        if (type == 0)
                        {
                            Console.Error.WriteLine($"add robot {id} to list. count={robots.Count}");
                            robots.Add(new Robot(id, x, y, item));
                        }
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    Console.Error.WriteLine($"{robots.Count}/////////////////////ROBOT {i}/////////////////////");
                    // Write an action using Console.WriteLine()
                    // To debug: Console.Error.WriteLine("Debug messages...");

                    Robot robo = robots.Where(r => r.ID == i).FirstOrDefault();

                    if (robo != null)
                    {
                        if (robo.Item == 4)
                        {
                            Console.Error.WriteLine($"****************BackToHeadquarter*****************");
                            robo.BackToHeadquarter();
                            Console.Error.WriteLine($"****************END BackToHeadquarter*****************");
                        }
                        else if (robo.Item == -1)
                        {
                            if (robo.destinationX > -1 && robo.destinationY > -1)
                            {
                                robo.destinationX = -1;
                                robo.destinationY = -1;
                            }

                            Console.Error.WriteLine($"**************FIND VEIN*******************");

                            var veins = cells.Where(v => v.OreNum != "?").ToArray();//find all seeable vains.
                            if (veins != null && veins.Length > 0)
                            {
                                Console.Error.WriteLine($"got {veins.Length} veins.");
                                if (veins.Length > 1)
                                {
                                    var nearestVein = veins.Aggregate((f, s) => GetDistance(robo, f) > GetDistance(robo, f) ? f : s);//find the nearest vein.
                                    robo.X = nearestVein.X;
                                    robo.Y = nearestVein.Y;
                                    robo.Dig(nearestVein);
                                }
                                else if (veins.Length == 1)
                                    robo.Dig(veins[0]);
                            }
                            else if (radarCooldown == 0)
                            {
                                Console.Error.WriteLine($"no vein. get radar.");
                                if (robo.X == 0)
                                    robo.RequestRADAR();
                                else
                                    robo.Move(new Cell(0, robo.Y));
                            }
                            Console.Error.WriteLine($"**************END FIND VEIN*******************");
                        }
                        else if (robo.Item == 2)
                        {
                            Console.Error.WriteLine($"***************PLANT RADAR******************");
                            //Plant radar.
                            Console.Error.WriteLine($"destinationX={robo.destinationX},destinationY={robo.destinationY}");
                            if (robo.destinationX == -1 && robo.destinationY == -1)
                            {
                                Random rnd = new Random();
                                robo.destinationX = rnd.Next(10, 25);
                                robo.destinationY = rnd.Next(3, 13);
                                robo.Dig(new Cell(robo.destinationX, robo.destinationY));
                            }
                            Console.Error.WriteLine($"***************END PLANT RADAR******************");
                        }
                    }

                    //Console.WriteLine("WAIT"); // WAIT|MOVE x y|DIG x y|REQUEST item
                    Console.Error.WriteLine($"/////////////////////ROBOT {i} END/////////////////////");
                }
                firstTurn = false;
            }
        }

        private static double GetDistance(Robot robo, Cell cell)
        {
            int a = (robo.X - cell.Y) * 2;
            int b = (robo.Y - cell.Y) * 2;
            double c = Math.Sqrt(a + b);

            Console.Error.WriteLine($"----------------------");
            Console.Error.WriteLine($"robo=={robo.ID}");
            Console.Error.WriteLine($"cell=={cell.X},{cell.Y}");
            Console.Error.WriteLine($"a={a}");
            Console.Error.WriteLine($"b={b}");
            Console.Error.WriteLine($"c={c}");
            Console.Error.WriteLine($"----------------------");

            return c;
        }

    }



    public class Cell
    {
        public int X, Y;
        public string OreNum;
        public int hole;

        public Cell(int x, int y, string ore, int hole)
        {
            X = x;
            Y = y;
            OreNum = ore;
        }

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Update(string ore,int h)
        {
            OreNum = ore;
            hole = h;
        }
    }

    public class Robot
    {
        public int Item;
        public readonly int ID;
        public int X, Y;
        public bool job = false;
        public int destinationX = -1;
        public int destinationY = -1;


        public Robot(int id, int x, int y, int item)
        {
            ID = id;
            X = x;
            Y = y;
            Item = item;
        }

        public void Request(string req)
        {
            Console.WriteLine($"REQUEST {req}");
        }

        public void RequestRADAR()
        {
            Console.Error.WriteLine($"Robo{ID} is requesting a radar.");
            Console.WriteLine($"REQUEST RADAR");
        }

        public void RequestTRAP()
        {
            Console.Error.WriteLine($"robo{ID}. Request TRAP.");
            Console.WriteLine($"REQUEST TRAP");
        }

        public void Move(Cell cell)
        {
            Console.Error.WriteLine($"robo{ID}. Moving to {cell.X},{cell.Y}.");
            Console.WriteLine($"DIG {cell.X} {cell.Y}");
        }

        public void BackToHeadquarter()
        {
            Console.Error.WriteLine($"robo{ID}. Back to headquarter.");
            Move(new Cell(0, Y));
        }

        public bool IsInHeadQuarter()
        {
            return X == 0;
        }

        public void Dig(Cell cell)
        {
            Console.Error.WriteLine($"robo{ID}. Digging at {cell.X},{cell.Y}.");
            Console.WriteLine($"DIG {cell.X} {cell.Y}");
        }

        public void DigNorth()
        {
            Console.WriteLine($"DIG {X} {Y - 1} dig north");
        }

        public void DigSouth()
        {
            Console.WriteLine($"DIG {X} {Y + 1} dig south");
        }

        public void DigEast()
        {
            Console.WriteLine($"DIG {X - 1} {Y} dig east");
        }

        public void DigWest()
        {
            Console.WriteLine($"DIG {X + 1} {Y} dig west");
        }

        public void Wait()
        {
            Console.WriteLine("WAIT");
        }
    }
}