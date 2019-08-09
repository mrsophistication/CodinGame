using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class BlowingFuse
{
    static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        int n = int.Parse(inputs[0]);
        int m = int.Parse(inputs[1]);
        int c = int.Parse(inputs[2]);
        inputs = Console.ReadLine().Split(' ');

        int[] currents = new int[n];

        for (int i = 0; i < n; i++)
            currents[i] = int.Parse(inputs[i]);

        int sum = 0;
        int max = 0;

        inputs = Console.ReadLine().Split(' ');
        for (int i = 0; i < m; i++)
        {
            int mx = int.Parse(inputs[i]) - 1;
            sum += currents[mx];
            currents[mx] *= -1;
            max = Math.Max(sum, max);

            if (max > c)
            {
                Console.WriteLine("Fuse was blown.");
                break;
            }
        }

        if (c > max)
        {
            Console.WriteLine("Fuse was not blown.");
            Console.WriteLine($"Maximal consumed current was {max} A.");
        }
        // Write an action using Console.WriteLine()
        // To debug: Console.Error.WriteLine("Debug messages...");
    }
}