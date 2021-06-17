using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CurrencyConvert.Models;
using System.Web.UI.WebControls;


namespace CurrencyConvert.Controllers
{
    public class ExchangeRateController:Controller
    {
        private int _curId = 0;
        
        public ActionResult Index()
        {
            var list = GetExchangeRates();
            return View(list);
        }
        
        public List<ExchangeRate> GetExchangeRates()
        {
            var lines = ReadExchangeRatesFromFile();
            var rates = new List<ExchangeRate>();

            if (lines.Length <= 0) return rates;
            foreach (var line in lines)
            {
                var rate = LineToExchangeRate(line);
                rates.Add(rate);
            }
            rates = rates.OrderBy(rate => rate.Id).ToList();
            return rates;
        }
        
        
        public ActionResult AddExchangeRate()
        {
            return View();
        }
        
        
        [HttpPost]
        public ActionResult AddExchangeRate(string currencyCode, double buyRate, double sellRate)
        {
            try
            {
                _curId = GetExchangeRates().Count;
                _curId += 1;
                var currentRates = UpdateExchangeRatesAdd(currencyCode, buyRate.ToString(CultureInfo.CurrentCulture),
                    sellRate.ToString(CultureInfo.CurrentCulture));
                WriteExchangeRatesInFile(currentRates);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            var rate = new ExchangeRate()
            {
                Id = _curId, BuyRate = buyRate, SellRate = sellRate, CurrencyCode = currencyCode
            };

            return View(rate);
        }

        
        public ActionResult DeleteExchangeRate()
        {
            return View();
        }
        
        
        [HttpPost, ActionName("DeleteExchangeRate")]
        public ActionResult DeleteExchangeRateConfirmed(string code)
        {
            var rate = GetExchangeRates().Find(curr =>  curr.CurrencyCode == code);
            try
            { 
                _curId = GetExchangeRates().Count;
                _curId -= 1;
                var lines = UpdateExchangeRateDelete(code, rate.Id);
                WriteExchangeRatesInFile(lines);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            return View(rate);
        }

        
        public ActionResult EditExhangeRate(string code)
        {
            var rate = GetExchangeRates().Find(exchangeRate => exchangeRate.CurrencyCode == code);
            return View(rate);
        }
        
        
        [HttpPost]
        public ActionResult EditExhangeRate(string currencyCode, double buyRate, double sellRate)
        {
            var rate = GetExchangeRates().Find(exchangeRate => exchangeRate.CurrencyCode == currencyCode);
            try
            {
                if (ModelState.IsValid)
                {
                    DeleteExchangeRateConfirmed(currencyCode);
                    AddExchangeRate(currencyCode, buyRate, sellRate);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            return View(rate);
        }


        public double GetSellRate(string currencyCode)
        {
            var exchangeRate = GetExchangeRates().Find(rate => rate.CurrencyCode == currencyCode);
            return exchangeRate.SellRate;
        }


        public double GetBuyRate(string currencyCode)
        {
            var exchangeRate = GetExchangeRates().Find(rate => rate.CurrencyCode == currencyCode);
            return exchangeRate.BuyRate;
        }
        private string[] UpdateExchangeRateDelete(string code, int id)
        {
            var rates = GetExchangeRates();
            rates = rates.FindAll(rate => rate.CurrencyCode != code).ToList();
            var curSize = rates.Count;
            var lines = new string[curSize];
            for (int i = 0; i < curSize; i++)
            {
                if (rates[i].Id > id)
                {
                    rates[i].Id -= 1;
                }
                lines[i] = ExchangeRateToLine(rates[i]);
            }

            return lines;
        }

        private string ExchangeRateToLine(ExchangeRate rate)
        {
            return rate.Id + " " + rate.CurrencyCode + " " + rate.BuyRate + " " + rate.SellRate;
        }


        private static void WriteExchangeRatesInFile(string[] rates)
        {
            try
            {
                System.IO.File.WriteAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/ExchangeRate", rates);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string[] UpdateExchangeRatesAdd(string currencyCode, string buyRate, string sellRate)
        {
            var line = _curId + " " + currencyCode + " " + buyRate + " " + sellRate;
            var currentRates = ReadExchangeRatesFromFile();
            var newListSize = currentRates.Length + 1;
            var lines = new string[newListSize];
            for (int i = 0; i < newListSize-1; i++)
            {
                lines[i] = currentRates[i];
            }
            lines[newListSize - 1] = line;
            return lines;
        }

        private ExchangeRate LineToExchangeRate(string line)
        {
            var values = line.Split(' ');
            var id = Convert.ToInt32(values[0]);
            if (id > _curId) _curId = id;
            var rate = new ExchangeRate()
            {
                Id = id, CurrencyCode = values[1], BuyRate = Convert.ToDouble(values[2]), SellRate = Convert.ToDouble(values[3])
            };
            return rate;
        }

        private static string[] ReadExchangeRatesFromFile()
        {
            var lines = System.IO.File.ReadAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/ExchangeRate");
            return lines;
        }
    }
}