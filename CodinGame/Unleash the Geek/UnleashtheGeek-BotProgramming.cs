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
            int area = 6;

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');
                int myScore = int.Parse(inputs[0]); // Amount of ore delivered
                int opponentScore = int.Parse(inputs[1]);
                cells.Clear();
                for (int i = 0; i < height; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    for (int j = 0; j < width; j++)
                    {
                        string ore = inputs[2 * j];// amount of ore or "?" if unknown
                        int hole = int.Parse(inputs[2 * j + 1]);// 1 if cell has a hole
                        cells.Add(new Cell(i, j, ore == "?" ? -1 : int.Parse(ore), hole));
                    }
                }
                robots.Clear();
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

                    if (type == 0)
                    {
                        robots.Add(new Robot(id, x, y, item));
                    }
                }
                bool reqR = false;
                for (int i = 0; i < 5; i++)
                {
                    // Write an action using Console.WriteLine()
                    // To debug: Console.Error.WriteLine("Debug messages...");

                    reqR = robots.Where(r => r.Item == 2).Any();

                    Robot robo = robots.Where(r => r.ID == i).FirstOrDefault();

                    if (robo.Item == 4)
                    {
                        robo.BackToHeadquarter();
                    }
                    else if (robo.Item == 2)
                    {
                        Console.Error.WriteLine($"robo{robo.ID}({robo.X}{robo.Y})");
                        continue;
                        //robo.Dig();
                    }
                    else
                    {
                        if (robo.Item == -1)
                        {
                            var veins = cells.Where(c => c.OreNum > 0);
                            if (veins.Any())
                            {
                                var vein = veins.Aggregate((m, n) => GetDistance(robo, m) < GetDistance(robo, n) ? m : n);

                                if (vein != null)
                                {
                                    robo.X = vein.X;
                                    robo.Y = vein.Y;
                                    robo.Dig();
                                    cells.Where(c => c.X == vein.X && c.Y == vein.Y).First().Decrease();
                                    continue;
                                }
                            }
                            else
                            {
                                if (robo.IsInHeadQuarter())
                                {
                                    if (radarCooldown > 0)
                                    {
                                        robo.Wait();
                                    }
                                    if (reqR)
                                    {
                                        robo.RequestRADAR();
                                        Console.Error.WriteLine($"robo{robo.ID}. calculating radar position.");
                                        reqR = true;
                                        robo.X = (area + 5) / 2;
                                        robo.Y = height / 2;
                                        area += 5;
                                        robo.Dig();
                                    }
                                }
                                else
                                {
                                    robo.BackToHeadquarter();
                                }
                            }
                        }
                    }

                    //Console.WriteLine("WAIT"); // WAIT|MOVE x y|DIG x y|REQUEST item
                }
            }
        }

        private static double GetDistance(Robot robo, Cell cell)
        {
            int a = (robo.X - cell.Y) * 2;
            int b = (robo.Y - cell.Y) * 2;
            double c = Math.Sqrt(a + b);
            Console.Error.WriteLine($"{c}");
            return c;
        }

    }

    public class Cell
    {
        public int X, Y;
        public int OreNum;
        public int hole;

        public Cell(int x, int y, int ore, int hole)
        {
            X = x;
            Y = y;
            OreNum = ore;
        }

        public void Decrease()
        {
            OreNum--;
        }
    }

    public class Robot
    {
        public int Item;
        public readonly int ID;
        public int X, Y;
        public bool job = false;


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
            Console.Error.WriteLine($"robo{ID}. Request RADAR.");
            Console.WriteLine($"REQUEST RADAR");
        }

        public void RequestTRAP()
        {
            Console.Error.WriteLine($"robo{ID}. Request TRAP.");
            Console.WriteLine($"REQUEST TRAP");
        }

        public void Move()
        {
            Console.Error.WriteLine($"robo{ID}. Moving to {X},{Y}.");
            Console.WriteLine($"DIG {X} {Y}");
        }

        public void BackToHeadquarter()
        {
            Console.Error.WriteLine($"robo{ID}. Back to headquarter.");
            X = 0;
            Move();
        }

        public bool IsInHeadQuarter()
        {
            return X == 0;
        }

        public void Dig()
        {
            Console.Error.WriteLine($"robo{ID}. Digging at {X},{Y}.");
            Console.WriteLine($"DIG {X} {Y}");
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
