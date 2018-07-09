using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeanCloud.Engine.AspNetDemo
{
    public class MyCloud : Cloud
    {
        public override void Start()
        {
            base.Start();
            Console.WriteLine("started");
        }
    }
}
