using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using CurrencyConvert.Models;

namespace CurrencyConvert.Controllers
{
    public class CurrencyController:Controller
    {
        public ActionResult Index()
        {
            var list = GetCurrencies();
            return View(list);
        }
        public List<Currency> GetCurrencies()
        {
            var lines = ReadCurrenciesFromFile();
            var currencies = new List<Currency>();

            if (lines.Length <= 0) return null;
            foreach (var line in lines)
            {
                var currency = LineToCurrency(line);
                currencies.Add(currency);
            }
            currencies = currencies.OrderBy(currency => currency.OrderNum).ToList();
            return currencies;
        }
        
        public ActionResult AddCurrency()
        {
            return View();
        }
        
        
        [HttpPost]
        public ActionResult AddCurrency(string code, string name, string nameLatin, string orderNum)
        {
            var currency = new Currency()
            {
                Code = code, Name = name, NameLatin = nameLatin, OrderNum = Convert.ToInt32(orderNum)
            };
            try
            {
                var currentCurrencies = UpdateCurrenciesAdd(code, name, nameLatin, orderNum.ToString());
                WriteCurrenciesInFile(currentCurrencies);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            return View(currency);
        }
        
        
        public ActionResult EditCurrency()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult EditCurrency(string code, string name, string nameLatin, string orderNum)
        {
            var currency = GetCurrencies().Find(curr =>  curr.Code == code);
            try
            {
                if (ModelState.IsValid)
                {
                    DeleteConfirmed(code);
                    AddCurrency(code, name, nameLatin, orderNum);
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            return View(currency);
        }

        
        public ActionResult DeleteCurrency()
        {
            return View();
        }
        
        
        [HttpPost, ActionName("DeleteCurrency")]
        public ActionResult DeleteConfirmed(string code)
        {
            var currency = GetCurrencies().Find(curr =>  curr.Code == code);
            try
            { 
                var currencies = UpdateCurrenciesDelete(code);
                WriteCurrenciesInFile(currencies);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = string.Format("Error");
            }

            return View(currency);
        }

        
        private string[] UpdateCurrenciesDelete(string code)
        {
            var currencies = GetCurrencies();
            currencies = currencies.FindAll(currency => currency.Code != code).ToList();
            var curSize = currencies.Count;
            var lines = new string[curSize];
            for (int i = 0; i < curSize; i++)
            {
                lines[i] = CurrencyToLine(currencies[i]);
            }

            return lines;
        }

        
        private static string CurrencyToLine(Currency currency)
        {
            return currency.Code + " " + currency.Name + " " + currency.NameLatin + " " + currency.OrderNum;
        }


        private static string[] ReadCurrenciesFromFile()
        {
            var lines = System.IO.File.ReadAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/Currency");
            return lines;
        }


        private static Currency LineToCurrency(string line)
        {
            var values = line.Split(' ');
            var currency = new Currency
            {
                Code = values[0], Name = values[1], NameLatin = values[2], OrderNum = Convert.ToInt32(values[3])
            };
           return currency;
        } 
        

        private string[] UpdateCurrenciesAdd(string code, string name, string nameLatin, string orderNum)
        {
            var line = code + " " + name + " " + nameLatin + " " + orderNum;
            var currentCurrencies = ReadCurrenciesFromFile();
            var newListSize = currentCurrencies.Length + 1;
            var lines = new string[newListSize];
            for (int i = 0; i < newListSize-1; i++)
            {
                lines[i] = currentCurrencies[i];
            }
            lines[newListSize - 1] = line;
            return lines;
        }

        private static void WriteCurrenciesInFile(string[] currencies)
        {
            try
            {
                System.IO.File.WriteAllLines(@"/home/mari/Desktop/CurrencyConvert/CurrencyConvert/App_Data/Currency", currencies);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}