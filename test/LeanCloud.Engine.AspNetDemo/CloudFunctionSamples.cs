using System;
using System.Threading.Tasks;

namespace LeanCloud.Engine.AspNetDemo
{

    public static class CloudFunctionsMiddleware
    {
        public static Cloud EquipFunctions(this Cloud cloud)
        {
            cloud.Define<string, double>("AverageStars1", SampleServices.Singleton.AverageStars);
            cloud.UseFunction<MovieService>();
            return cloud;
        }
    }

    // case 1. use Cloud.Define
    public class SampleServices
    {
        public static SampleServices Singleton = new SampleServices();

        public double AverageStars(string movieName)
        {
            if (movieName == "夏洛特烦恼")
                return 3.8;
            return 0;
        }
    }

    // case 2. use EngineFunctionAttribute
    public class MovieService
    {
        [EngineFunction("AverageStars2")]
        public double AverageStars([EngineFunctionParameter("movieName")]string movieName)
        {
            if (movieName == "夏洛特烦恼")
                return 3.8;
            return 0;
        }
    }
}
