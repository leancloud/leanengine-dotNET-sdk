using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeanCloud.Engine.AspNetDemo
{

    public static class CloudFunctionsMiddleware
    {
        public static Cloud EquipFunctions(this Cloud cloud)
        {
            cloud.Define<string, double>("AverageStars", SampleServices.AverageStars);
            cloud.UseFunction<MovieService>(new MovieService(new string[] { "夏洛特烦恼", "功夫", "大话西游之月光宝盒" }));
            cloud.UseFunction<MovieService>();
            cloud.UseFunction<StaticSampleService>();

            return cloud;
        }
    }

    // case 1. use Cloud.Define
    public class SampleServices
    {
        public static double AverageStars(string movieName)
        {
            if (movieName == "夏洛特烦恼")
                return 3.8;
            return 0;
        }
    }

    // case 2. use EngineFunctionAttribute
    public class MovieService
    {
        public List<string> ValidMovieNames { get; set; }
        public MovieService(string[] someMovieNames)
        {
            ValidMovieNames = someMovieNames.ToList();
        }

        [EngineFunction("AverageStars2")]
        public double AverageStars([EngineFunctionParameter("movieName")]string movieName)
        {
            if (!ValidMovieNames.Contains(movieName)) return 0;
            if (movieName == "夏洛特烦恼")
                return 3.8;
            return 0;
        }
    }

    // case 3. use global(static) method
    public class StaticSampleService
    {
        [EngineFunction("AverageStars2")]
        public static double AverageStars([EngineFunctionParameter("movieName")]string movieName)
        {
            if (movieName == "夏洛特烦恼")
                return 3.8;
            return 0;
        }
    }
}
