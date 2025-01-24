using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace LeadAutomation.Firefly.Exchange.Helpers
{
	/// <summary>
	/// Tools for currency exchange rates.
	/// </summary>
	public static class ExchangeRates
	{
		#region Fields

		/// <summary>
		/// Internal storage of the retrieved exchange rates.
		/// </summary>
		private readonly static ConcurrentDictionary<string, decimal> ExchangeRatesStore = new ConcurrentDictionary<string, decimal>();

		/// <summary>   
		/// Client to use for accessing the rates.
		/// </summary>
		private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }) { Timeout = System.Threading.Timeout.InfiniteTimeSpan };

		/// <summary>
		/// Timer that should kick once every 24 hours to retrieve the latest exchange rates.
		/// </summary>
		private static Timer timer = null;

		#endregion Fields

		#region Methods

		/// <summary>
		/// Converts a value in USD to another currency.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="to">Currency to convert to.</param>
		/// <returns>Decimal representation of <paramref name="value" /> after being converted from USD.</returns>
		public static decimal ConvertFromUsd(decimal value, string to)
		{
			if (string.IsNullOrWhiteSpace(to))
				throw new ArgumentNullException("to");
			else if (to.Equals("USD", StringComparison.OrdinalIgnoreCase))
				return value;
			else if (ExchangeRatesStore.Count == 0)
				throw new InvalidOperationException("Exchange rates have not yet been successfully retrieved.");
			else if (!ExchangeRatesStore.ContainsKey(to.ToUpperInvariant()))
				throw new InvalidOperationException($"Exchange rate for currency symbol \"{to}\" could not be found.");

			return value * ExchangeRatesStore[to.ToUpperInvariant()];
		}

		/// <summary>
		/// Converts a value in another currency into USD.
		/// </summary>
		/// <param name="value">Value to convert.</param>
		/// <param name="from">Currency to convert from.</param>
		/// <returns>Decimal representation of <paramref name="value" /> after being converted to USD.</returns>
		public static decimal ConvertToUsd(decimal value, string from)
		{
			if (string.IsNullOrWhiteSpace(from))
				throw new ArgumentNullException("from");
			else if (from.Equals("USD", StringComparison.OrdinalIgnoreCase))
				return value;
			else if (ExchangeRatesStore.Count == 0)
				throw new InvalidOperationException("Exchange rates have not yet been successfully retrieved.");
			else if (!ExchangeRatesStore.ContainsKey(from.ToUpperInvariant()))
				throw new InvalidOperationException($"Exchange rate for currency symbol \"{from}\" could not be found.");

			return value / ExchangeRatesStore[from.ToUpperInvariant()];
		}

		/// <summary>
		/// Gets all currently-cached exchange rates for converting USD $1 into one unit of another currency.
		/// </summary>
		/// <returns>Dictionary{string, decimal} of all currently-cached exchange rates.</returns>
		public static Dictionary<string, decimal> GetAllRates()
		{
			if (ExchangeRatesStore.Count == 0)
				throw new InvalidOperationException("Exchange rates have not yet been successfully retrieved.");

			return ExchangeRatesStore
				.OrderBy(kvp => kvp.Key)
				.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		/// <summary>
		/// Gets the exchange rate for converting USD $1 into one unit of another currency.
		/// </summary>
		/// <param name="to">Currency symbol to get the exchange rate for.</param>
		/// <returns>Decimal representation of the exchange rate.</returns>
		public static decimal GetRateFromUsd(string to)
		{
			if (string.IsNullOrWhiteSpace(to))
				throw new ArgumentNullException("to");
			else if (to.Equals("USD", StringComparison.OrdinalIgnoreCase))
				return 1;
			else if (ExchangeRatesStore.Count == 0)
				throw new InvalidOperationException("Exchange rates have not yet been successfully retrieved.");
			else if (!ExchangeRatesStore.ContainsKey(to.ToUpperInvariant()))
				throw new InvalidOperationException($"Exchange rate for currency symbol \"{to}\" could not be found.");

			return ExchangeRatesStore[to.ToUpperInvariant()];
		}

		/// <summary>
		/// Performs basic initialization tasks to facilitate proper utilization of exchange rates.
		/// </summary>
		/// <param name="timeout">How long to wait for the exchange rates to be retrieved before timing out and throwing an exception.</param>
		public static void Initialize(TimeSpan? timeout = null)
		{
			timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
			timer.Elapsed += (s, e) =>
				{
					timer.Interval = GetInterval();

					try
					{
						RefreshRates();
					}
					catch
					{
						timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
					}
				};
			timer.Start();

			if (!Task.Run(async () =>
				{
					while (true)
					{
						lock (ExchangeRatesStore)
							if (ExchangeRatesStore.Count > 0)
								break;

						await Task.Delay(100);
					}
				}).Wait(timeout ?? System.Threading.Timeout.InfiniteTimeSpan))
				throw new TimeoutException("Exchange rates could not be successfully retrieved prior to timeout.");
		}

		/// <summary>
		/// Downloads the latest exchange rates and caches them.
		/// </summary>
		public static void RefreshRates()
		{
			lock (ExchangeRatesStore)
			{
				// https://www.ecb.europa.eu/stats/policy_and_exchange_rates/euro_reference_exchange_rates/html/index.en.html
				//string xml = HttpClient.GetStringAsync("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml").Result;
				string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<gesmes:Envelope xmlns:gesmes=""http://www.gesmes.org/xml/2002-08-01"" xmlns=""http://www.ecb.int/vocabulary/2002-08-01/eurofxref"">
	<gesmes:subject>Reference rates</gesmes:subject>
	<gesmes:Sender>
		<gesmes:name>European Central Bank</gesmes:name>
	</gesmes:Sender>
	<Cube>
		<Cube time='2024-03-13'>
			<Cube currency='USD' rate='1.0939'/>
			<Cube currency='JPY' rate='161.83'/>
			<Cube currency='BGN' rate='1.9558'/>
			<Cube currency='CZK' rate='25.273'/>
			<Cube currency='DKK' rate='7.4573'/>
			<Cube currency='GBP' rate='0.85451'/>
			<Cube currency='HUF' rate='396.90'/>
			<Cube currency='PLN' rate='4.2860'/>
			<Cube currency='RON' rate='4.9683'/>
			<Cube currency='SEK' rate='11.1930'/>
			<Cube currency='CHF' rate='0.9599'/>
			<Cube currency='ISK' rate='148.50'/>
			<Cube currency='NOK' rate='11.4805'/>
			<Cube currency='TRY' rate='35.1136'/>
			<Cube currency='AUD' rate='1.6542'/>
			<Cube currency='BRL' rate='5.4506'/>
			<Cube currency='CAD' rate='1.4756'/>
			<Cube currency='CNY' rate='7.8678'/>
			<Cube currency='HKD' rate='8.5574'/>
			<Cube currency='IDR' rate='17039.08'/>
			<Cube currency='ILS' rate='3.9947'/>
			<Cube currency='INR' rate='90.5940'/>
			<Cube currency='KRW' rate='1439.01'/>
			<Cube currency='MXN' rate='18.3472'/>
			<Cube currency='MYR' rate='5.1282'/>
			<Cube currency='NZD' rate='1.7770'/>
			<Cube currency='PHP' rate='60.583'/>
			<Cube currency='SGD' rate='1.4581'/>
			<Cube currency='THB' rate='39.036'/>
			<Cube currency='ZAR' rate='20.4535'/>
		</Cube>
	</Cube>
</gesmes:Envelope>";

                XmlDocument xmlDocument = new XmlDocument();
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("gesmes", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
				xmlDocument.LoadXml(xml);
				XmlNodeList xmlNodeList = xmlDocument.DocumentElement.SelectNodes("//gesmes:Cube/gesmes:Cube/gesmes:Cube", xmlNamespaceManager);

				decimal usd = 0;
				foreach (XmlNode xmlNode in xmlNodeList.Cast<XmlNode>())
					if (xmlNode.Attributes["currency"].Value.Equals("USD"))
					{
						usd = Convert.ToDecimal(xmlNode.Attributes["rate"].Value);
						ExchangeRatesStore.AddOrUpdate("USD", 1, (x, y) => 1);
						ExchangeRatesStore.AddOrUpdate("EUR", Math.Round(1 / usd, 4), (x, y) => Math.Round(1 / usd, 4));
					}
					else
						ExchangeRatesStore.AddOrUpdate(xmlNode.Attributes["currency"].Value, Math.Round(Convert.ToDecimal(xmlNode.Attributes["rate"].Value) / usd, 4), (x, y) => Math.Round(Convert.ToDecimal(xmlNode.Attributes["rate"].Value) / usd, 4));
			}
		}

		/// <summary>
		/// Gets the number of milliseconds until 4:30 PM CET (UTC +1), which is when the exchange rates should be refreshed each day.
		/// </summary>
		/// <returns>Milliseconds until 4:30 PM CET (UTC +1).</returns>
		private static double GetInterval()
		{
			// http://stackoverflow.com/a/17383456/2766752
			// ecb says they update the rates each day around 4 PM CET (UTC +1). therefore, we want the next one to occur at 4:30 PM CET
			double interval = (new TimeSpan(16, 30, 0) - DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(1)).TimeOfDay).TotalMilliseconds;
			if (interval <= -0.0001)
				interval += TimeSpan.FromHours(24).TotalMilliseconds;

			return interval;
		}

		#endregion Methods
	}
}