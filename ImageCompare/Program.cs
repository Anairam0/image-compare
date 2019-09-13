using Common;
using System;

namespace ImageCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            var commonHelper = new Helpers();

            Console.Clear();

            Console.Write("\nPlease insert the file path:\n");
            var filePath = Console.ReadLine();

            Console.Write("\nPlease insert the file name:\n");
            var fileName = Console.ReadLine();

            var imagesList = commonHelper.ReadExcelInputFileAsync(filePath, fileName).Result;

            if(imagesList.Count > 0)
            {
                var outputList = commonHelper.CompareFiles(imagesList);

                commonHelper.WriteExcelOutputFile(filePath, fileName, outputList);
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
