using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            Console.Write("\nInserte el path del archivo:\n");
            var path = Console.ReadLine();

            var imagesList = ReadExcelInputFileAsync(path);

            var outputList = CompareFiles(imagesList);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static async Task<List<InputData>> ReadExcelInputFileAsync(string excelFilesPath)
        {
            DirectoryInfo d = new DirectoryInfo(excelFilesPath);//Assuming Test is your Folder
            FileInfo[] Files = d.GetFiles("*.csv"); //Getting Text files
            var bulkList = new List<InputExcel>();

            foreach (FileInfo file in Files)
            {
                var str = excelFilesPath + "\\" + file.Name;

                using (var memoryStream = new MemoryStream())
                {
                    byte[] buffer = File.ReadAllBytes(str);

                    var data = new MemoryStream(buffer);

                    await data.CopyToAsync(memoryStream);

                    memoryStream.Position = 0;
                    TextReader textReader = new StreamReader(memoryStream);

                    var csv = new CsvReader(textReader);

                    csv.Configuration.HasHeaderRecord = true;

                    while (csv.Read())
                    {
                        var record = csv.GetRecord<InputExcel>();

                        bulkList.Add(record);
                    }
                }
            }
            return bulkList;
        }

        static List<OutputData> CompareFiles (List<InputData> inputPairs)
        {
            var outputDataList = new List<OutputData>();

            foreach(var pair in inputPairs)
            {
                outputDataList.Add(Compare(pair));
            }

            return outputDataList;
        }

        static OutputData Compare(InputData pairToCompair)
        {




            return new OutputData();
        }
        public class InputData
        {
            public string Image1 { get; set; }
            public string Image2 { get; set; }
        }

        public class OutputData
        {
            public string Image1 { get; set; }
            public string Image2 { get; set; }

            //Bjorn is entrusting you to figure out an appropriate scoring algorithm, although he is requesting that 0 indicates that the pair are the same image.
            public float Similar { get; set; }

            //time spent to calculate the Similar
            public float Elapsed { get; set; }
        }
    }
}
