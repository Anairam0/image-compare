using ImageCompare.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common
{
    public interface IHelpers
    {
        void WriteExcelOutputFile(string filePath, string fileName, List<OutputData> listOutputData);
        Task<List<InputData>> ReadExcelInputFileAsync(string excelFilesPath, string fileName);
        List<OutputData> CompareFiles(List<InputData> inputPairs);
        OutputData Compare(InputData pairToCompair);
    }
}
