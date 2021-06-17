using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CurrencyConvert.Models;

namespace CurrencyConvert.Controllers
{
    public class RecordController:Controller
    {
        private int _curId = 0;
        
        
        public ActionResult Index()
        {
            var list = GetRecords();
            return View(list);
        }
        
        
        public List<Record> GetRecords()
        {
            var lines = ReadRecordsFromFile();
            var records = new List<Record>();
        
            if (lines.Length <= 0) return null;
            foreach (var line in lines)
            {
                var record = LineToRecord(line);
                records.Add(record);
            }

            records = records.OrderByDescending(record => record.Date).ToList();
            return records;
        }


        public Record GetRecordById(int id)
        {
            var records = GetRecords();
            return records.Find(record => record.Id == id);
        }
        
        
        public ActionResult AddRecord()
        {
            return View();
        }
        
        
        [HttpPost]
        public ActionResult AddRecord(string currencyIn, string currencyOut, double amountIn)
        {
            _curId = GetRecords().Count;
            _curId += 1;
            var comment = "";
            // if (CalculateRate(currencyIn) * amountIn > 3000) AddComment(_curId);
            // else comment += "N/A";
            comment += "N/A";
            var amountOut = Calculate(currencyIn, currencyOut, amountIn);
            // var amountOut = 3200;
            var currentRecords = UpdateRecordsAdd(currencyIn, currencyOut, amountIn.ToString(), amountOut.ToString(), comment, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            WriteRecordsInFile(currentRecords);
            return View();
        }

        
        public double CalculateRate(string currencyIn)
        {
            var erc = new ExchangeRateController();
            return erc.GetBuyRate(currencyIn);
        }

        public double Calculate(string currencyIn, string currencyOut, double amountIn)
        {
            var erc = new ExchangeRateController();
            var curInRate = erc.GetSellRate(currencyIn);
            var curOutRate = erc.GetSellRate(currencyOut);
            var rate = curInRate / curOutRate;
            return amountIn * rate;
        }


        public List<Record> SortDoubtfulRecords()
        {
            var records = GetRecords();
            foreach (var record in records)
            {
                record.AmountInLari = record.AmountIn * CalculateRate(record.CurrencyIn);
            }
            records = records.OrderByDescending(record => record.AmountInLari).ToList();
            // records = records.OrderByDescending(record => record.AmountIn).ToList();
            if (records.Count < 3) return records;
            var topThreeAmounts = new List<Record>();
            for (int i = 0; i < 3; i++)
            {
                topThreeAmounts.Add(records[i]);
            }

            return topThreeAmounts;
        }


        public List<Record> SortByDate()
        {
            var records = GetRecords();
            return  records.OrderByDescending(record => record.Date).ToList();
        }
        
        
        private void WriteRecordsInFile(string[] currentRecords)
        {
            try
            {
                System.IO.File.WriteAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/Record", currentRecords);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        

        private string[] UpdateRecordsAdd(string currencyIn, string currencyOut, string amountIn, string amountOut, string comment, string date)
        {
            var line = _curId + " " + currencyIn + " " + currencyOut + " " + amountIn + " " + amountOut +
                       " " + comment + " " + date;
            var currentRecords = ReadRecordsFromFile();
            var newListSize = currentRecords.Length + 1;
            var lines = new string[newListSize];
            for (int i = 0; i < newListSize-1; i++)
            {
                lines[i] = currentRecords[i];
            }
            lines[newListSize - 1] = line;
            return lines;
        }
        

        private string[] ReadRecordsFromFile()
        {
            var lines = System.IO.File.ReadAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/Record");
            return lines;
        }
        

        public Record LineToRecord(string line)
        {
            var values = line.Split(' ');
            var id = Convert.ToInt32(values[0]);
            if (id > _curId) _curId = id;
            var record = new Record()
            {
                Id = id, CurrencyIn = values[1], CurrencyOut = values[2], AmountIn = Convert.ToDouble(values[3]),
                AmountOut = Convert.ToDouble(values[4]), Comment = values[5], Date = Convert.ToDateTime(values[6] + " " + values[7])
            };
            return record;
        }
    }
}