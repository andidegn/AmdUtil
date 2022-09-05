using AMD.Util.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMD.Util.Files
{
  public class FileCompare
  {
    public class FileCompareDataList : List<FileCompareData>
    {
      public enum FCDLPrintRequest
      {
        NoFiles,
        EqualFiles,
        NotEqualFiles,
        AllFiles
      }

      public String SourceFilePath { get; set; }
      public bool? IsAllEqual
      {
        get
        {
          return 0 == CountNotEqual;
        }
      }

      public int CountEqual
      {
        get
        {
          return CountEquality(true);
        }
      }

      public int CountNotEqual
      {
        get
        {
          return CountEquality(false);
        }
      }

      private int CountEquality(bool? checkEqual)
      {
        return (from fcd in this
                where checkEqual == fcd.IsEqual
                select fcd).Count();
      }

      public FileCompareDataList(String sourceFilePath)
      {
        this.SourceFilePath = sourceFilePath;
      }

      public void Add(String compareFilePath)
      {
        this.Add(new FileCompareData(compareFilePath, SourceFilePath));
      }

      public void AddRange(params String[] compareFilePaths)
      {
        foreach (String file in compareFilePaths)
        {
          this.Add(file);
        }
      }

      public void AddRangeFromDirectory(String compareDirectoryPath)
      {
        this.AddRange(Directory.GetFiles(compareDirectoryPath));
      }

      public ICollection<String> GetEqualFilePaths()
      {
        List<String> retList = (from fcd in this
                                where true == fcd.IsEqual
                                select fcd.Path).ToList<String>();
        return retList;
      }

      public ICollection<String> GetNotEqualFilePaths()
      {
        List<String> retList = (from fcd in this
                                where false == fcd.IsEqual
                                select fcd.Path).ToList<String>();
        return retList;
      }

      public String ToString(FCDLPrintRequest request)
      {
        StringBuilder sb = new StringBuilder();

        switch (request)
        {
          case FCDLPrintRequest.EqualFiles:
            foreach (FileCompareData fileCompareData in this)
            {
              if (true == fileCompareData.IsEqual)
              {
                sb.AppendLine(fileCompareData.ToString());
              }
            }
            break;
          case FCDLPrintRequest.NotEqualFiles:
            foreach (FileCompareData fileCompareData in this)
            {
              if (false == fileCompareData.IsEqual)
              {
                sb.AppendLine(fileCompareData.ToString());
              }
            }
            break;
          case FCDLPrintRequest.AllFiles:
            foreach (FileCompareData fileCompareData in this)
            {
              sb.AppendLine(fileCompareData.ToString());
            }
            break;
          case FCDLPrintRequest.NoFiles:
          default:
            break;
        }

        sb.AppendLine($"{this.SourceFilePath} compared with {this.Count}. Equal: {this.CountEqual} - Not equal: {this.CountNotEqual}");
        return sb.ToString();
      }

      public override String ToString()
      {
        return this.ToString(FCDLPrintRequest.AllFiles);
      }
    }

    public class FileCompareData
    {
      public String Path { get; set; }
      public byte[] Content { get; set; }
      public bool? IsEqual { get; set; }

      public FileCompareData(String sourcePath, String compareFilePath)
      {
        this.Path = sourcePath;
        this.Content = File.ReadAllBytes(this.Path);
        this.IsEqual = this.Content.SequenceEqual<byte>(File.ReadAllBytes(compareFilePath));
      }

      public override bool Equals(object obj)
      {
        bool retVal = false;
        if (obj is FileCompareData)
        {
          FileCompareData other = obj as FileCompareData;
          retVal = Content.SequenceEqual<byte>(other.Content);
        }
        return retVal;
      }

      public override String ToString()
      {
        String checkChar;
        String equalTxt;

        switch (IsEqual)
        {
          case true:
            checkChar = "==";
            equalTxt = "equal";
            break;

          case false:
            checkChar = "!=";
            equalTxt = "not equal";
            break;

          default:
            checkChar = " ?";
            equalTxt = "not checked";
            break;
        }

        return $"{checkChar} - {Path} is {equalTxt}";
      }
    }
  }
}
