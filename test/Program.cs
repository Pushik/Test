﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using test.Models;
using System.Timers;
using System.Collections;


namespace test
{

    class Program
    {
        private static Timer aTimer;
        static string codequote { get; set; }
        public  static IStockExchangeService asd;
      static void Main(string[] args)
        {
            
            int timertime;
            IStockExchangeService asd = new StockExchangeService();
           
            
        // Чтение кода котировки
            Console.WriteLine("Enter Quote code:");
            codequote = Console.ReadLine();
            // Чтение интервала времени для запроса
            Console.WriteLine("Enter Time Period in second:");
            timertime = Convert.ToInt16(Console.ReadLine());

        // Создание таймера с принимаемым интервалом в переменной timertime
            aTimer = new System.Timers.Timer();
            aTimer.Interval = (timertime * 1000);
            aTimer.Elapsed += OnTimedEvent;  // Событие по истечению таймера.
            aTimer.AutoReset = true;  // Повторить события таймера (true is the default)
            aTimer.Enabled = true;  // Start the timer
            
            Console.ReadKey();
            Console.WriteLine(asd.Adjclose);
            
         }
        // Событие по таймеру
          public static void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
          {
            Console.WriteLine();
            Console.WriteLine("Данные от сервера");
            Console.WriteLine("Локальное время запроса {0}", e.SignalTime);
            var data2 = DataYohoo(codequote);
            Console.WriteLine();

            //data2.GetInfo();
            //list.Add(data2.Adjclose);
                     
          }
        
        // Получение данных от сервера Yahoo
           public static ResultYohoo DataYohoo(string codequote)
        {
            WebRequest wrGETURL = WebRequest.Create($"https://query1.finance.yahoo.com/v8/finance/chart/{codequote.ToUpperInvariant()}?interval=1d");
            // Пример получение данных по указанной котировке - MU
            // WebRequest wrGETURL = WebRequest.Create($"https://query1.finance.yahoo.com/v8/finance/chart/MU?interval=1d");
            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();
            StreamReader objReader = new StreamReader(objStream);
            var json = objReader.ReadToEnd();


            try
            {
                // присваиваем ответ от сервера 
                var result = JsonConvert.DeserializeObject<Result>(json);
                
                if (result?.Chart?.Data == null || result.Chart.Data.Length == 0 ||
                    result.Chart.Data[0].Indicator?.CurrentValue == null ||
                    result.Chart.Data[0].Indicator?.CurrentValue.Length == 0 ||
                    result.Chart.Data[0].Indicator?.CurrentValue[0].Value.Length == 0)
                {
                    Console.WriteLine("Bad object format");
                }
                else
                {   // заполняем модель ResultYohoo 
                    var data1 = new ResultYohoo();
                    data1.Currency = result.Chart.Data[0].Metadata.Currency.ToString();
                    data1.ExchangeName = result.Chart.Data[0].Metadata.ExchangeName.ToString();
                    data1.Adjclose = result.Chart.Data[0].Indicator.Quotes[0].Valueclose[0].ToString("#,#00.000");
                    // возврат модели
                    return data1;
                                   
                }

            }
            catch (Exception ex)
            {
              Console.WriteLine($"Cannot deserialize string due an error {ex.Message}");
              return null;
            }
            finally
            {
              aTimer.AutoReset = true;
            }
            return null;
        }
    }
}
