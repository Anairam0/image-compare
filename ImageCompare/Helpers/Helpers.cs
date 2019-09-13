using CsvHelper;
using ImageCompare.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            long initTicks = DateTime.Now.Ticks;

            List<bool> brHash1 = GetBrightHash(pairToCompair.Image1);
            List<bool> brHash2 = GetBrightHash(pairToCompair.Image2);

            int unequalElements = brHash1.Zip(brHash2, (i, j) => i != j).Count(eq => eq);

            float similarity = unequalElements * 1.00f / brHash1.Count;
            long finishTicks = DateTime.Now.Ticks;


            return new OutputData()
            {
                Image1 = pairToCompair.Image1,
                Image2 = pairToCompair.Image2,
                Similar = similarity,
                Elapsed = (finishTicks - initTicks) / TimeSpan.TicksPerMillisecond
            };
        }

        private List<bool> GetBrightHash(string filePath)
        {

            Bitmap bmpMin = new Bitmap(new Bitmap(filePath), new Size(32, 32));

            var data = bmpMin.LockBits(
                new Rectangle(Point.Empty, bmpMin.Size),
                ImageLockMode.ReadWrite, bmpMin.PixelFormat);
            var pixelSize = data.PixelFormat == PixelFormat.Format32bppArgb ? 4 : 3;
            var padding = data.Stride - (data.Width * pixelSize);
            var bytes = new byte[data.Height * data.Stride];

            // copy the bytes from bitmap to array
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            var index = 0;
            List<bool> brightHash = new List<bool>();

            for (var y = 0; y < data.Height; y++)
            {
                for (var x = 0; x < data.Width; x++)
                {
                    Color pixelColor = Color.FromArgb(
                        pixelSize == 3 ? 255 : bytes[index + 3], // A component if present
                        bytes[index + 2], // R
                        bytes[index + 1], // G
                        bytes[index]      // B
                        );

                    var bright = pixelColor.GetBrightness();
                    brightHash.Add(bright < 0.5f);

                    index += pixelSize;
                }

                index += padding;
            }

            // copy back the bytes from array to the bitmap
            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            return brightHash;
        }
    }
}
