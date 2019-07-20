using Entities;
using System;
using System.Numerics;

namespace Arena
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = Parser.ConvertToMap(@"C:\Users\Corium\Desktop\LSMaterial\Map1.txt");
            Renderer renderer = new Renderer();
            renderer.Render(map);
            Console.Read();
        }
    }
}
