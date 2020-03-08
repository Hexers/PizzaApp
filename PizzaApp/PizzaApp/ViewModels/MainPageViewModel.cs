using PizzaApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace PizzaApp.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ToppingCombination> pizzas;
        public ObservableCollection<ToppingCombination> Pizzas
        {
            get
            {
                return pizzas;
            }
            set
            {
                pizzas = value;
                NotifyPropertyChanged("Pizzas");
            }
        }

        public MainPageViewModel()
        {
            GetOrderData();

            //Print Collection to Console
            for (int i = 0; i < Pizzas.Count; i++)
            {
                Console.WriteLine($"Rank: #{i + 1} - # of orders: {Pizzas[i].OrderCount} - Toppings: {Pizzas[i].Toppings}");
            }
        }

        private void GetOrderData()
        {

            List<Pizza> unSortedPizzas = new List<Pizza>();
            string url = "http://files.olo.com/pizzas.json";
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                try
                {
                    Stream stream = httpWebResponse.GetResponseStream();
                    string json = new StreamReader(stream).ReadToEnd();
                    stream.Close();

                    unSortedPizzas = JsonConvert.DeserializeObject<List<Pizza>>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"MainPageViewModel(), GetOrderData() threw an exception {ex} : HttpWebResponseResultp -> " +

                        $"Status - {httpWebResponse.StatusCode} Description- {httpWebResponse.StatusDescription} ");
                }
            }

            var orderedPizzaToppings = unSortedPizzas.Select(p => p.Toppings.OrderBy(t => t));
            IEnumerable<ToppingCombination> groupedToppings = orderedPizzaToppings.Select((toppings => toppings.Aggregate((x, y) => x + "," + y)))
            .GroupBy(toppingsGroup => toppingsGroup)
            .Select(toppingsGroup => new ToppingCombination()
            {
                Toppings = toppingsGroup.Key,
                OrderCount = toppingsGroup.Count()
            });

            IEnumerable<ToppingCombination> mostPopularToppings = groupedToppings.OrderByDescending(ag => ag.OrderCount).Take(20); // Take(20) - References top 20
            this.Pizzas = new ObservableCollection<ToppingCombination>(mostPopularToppings.OrderByDescending(x => x.OrderCount).ToList());

        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
