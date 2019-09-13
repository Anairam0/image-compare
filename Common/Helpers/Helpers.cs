using Common.Helpers;
using CsvHelper;
using ImageCompare.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Common
{
    public class Helpers : IHelpers
    {
        public void WriteExcelOutputFile(string filePath, string fileName, List<OutputData> listOutputData)
        {
            using (var writer = new StreamWriter(filePath + fileName + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(listOutputData);
            }
        }

        public async Task<List<InputData>> ReadExcelInputFileAsync(string excelFilesPath, string fileName)
        {
            var d = new DirectoryInfo(excelFilesPath);
            var Files = d.GetFiles(fileName + ".csv");
            var bulkList = new List<InputData>();

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
                        var record = csv.GetRecord<InputData>();

                        bulkList.Add(record);
                    }
                }
            }
            return bulkList;
        }

        public List<OutputData> CompareFiles(List<InputData> inputPairs)
        {
            var outputDataList = new List<OutputData>();

            foreach (var pair in inputPairs)
            {
                outputDataList.Add(Compare(pair));
            }

            return outputDataList;
        }

        public OutputData Compare(InputData pairToCompair)
        {
            return new OutputData();
        }
    }
}
