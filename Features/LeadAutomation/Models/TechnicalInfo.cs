using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LeadAutomation.Pigeon.Exchange.Entities
{
	/// <summary>
	/// Container for an entity's technical information.
	/// </summary>
	public class TechnicalInfo
	{
		#region Fields

		/// <summary>
		/// <see cref="TechnicalInfoGeolocation" /> container of this <see cref="TechnicalInfo" />.
		/// </summary>
		private TechnicalInfoGeolocation geolocation = null;

		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TechnicalInfo" /> class.
		/// </summary>
		public TechnicalInfo()
		{
			this.Geolocation = new TechnicalInfoGeolocation();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// Gets or sets the additional data of this <see cref="TechnicalInfo" />.
		/// </summary>
		public string AdditionalData
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the browser used to create the entity.
		/// </summary>
		[MaxLength(32)]
		public string BrowserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the version of the browser to create the entity.
		/// </summary>
		[MaxLength(16)]
		public string BrowserVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the container for the IP-based geolocation data of the entity that created this entity.
		/// </summary>
		public TechnicalInfoGeolocation Geolocation
		{
			get { return this.geolocation; }
			set { this.geolocation = value ?? new TechnicalInfoGeolocation(); }
		}

		/// <summary>
		/// Gets or sets the IP address of the entity that created the entity.
		/// </summary>
		[MaxLength(64)]
		public string IPAddress
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the unique key related to the <see cref="Origin" /> of the entity.
		/// </summary>
		[MaxLength(128)]
		public string Key
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the origin of the entity.
		/// </summary>
		[MaxLength(128)]
		public string Origin
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URL that linked to the page that generated the entity.
		/// </summary>
		[MaxLength(512)]
		public string ReferrerUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URL used to create the entity.
		/// </summary>
		[MaxLength(512)]
		public string SourceUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user agent used to create the entity.
		/// </summary>
		[MaxLength(512)]
		public string UserAgent
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the web server used to create the entity.
		/// </summary>
		[MaxLength(64)]
		public string WebServerName
		{
			get;
			set;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return string.Format("{0}",
				/* 0 */ string.Join(", ",
				/* 1 */ new[]
					{
						!string.IsNullOrWhiteSpace(this.Origin)
							? string.Format("Origin = {0}", this.Origin)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.Key)
							? string.Format("Key = {0}", this.Key)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.BrowserName)
							? string.Format("BrowserName = {0}", this.BrowserName)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.BrowserVersion)
							? string.Format("BrowserVersion = {0}", this.BrowserVersion)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.IPAddress)
							? string.Format("IPAddress = {0}", this.IPAddress)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.SourceUrl)
							? string.Format("SourceUrl = {0}", this.SourceUrl)
							: string.Empty,
						!string.IsNullOrWhiteSpace(this.ReferrerUrl)
							? string.Format("ReferrerUrl = {0}", this.ReferrerUrl)
							: string.Empty
					}.Where(s => !string.IsNullOrWhiteSpace(s))));
		}

		#endregion Methods

		#region Classes

		/// <summary>
		/// Container for the IP-based geolocation data of an entity's creator.
		/// </summary>
		public class TechnicalInfoGeolocation
		{
			#region Constructors

			/// <summary>
			/// Initializes a new instance of the <see cref="TechnicalInfoGeolocation" /> class.
			/// </summary>
			public TechnicalInfoGeolocation()
			{
			}

			#endregion Constructors

			#region Properties

			/// <summary>
			/// Gets or sets the city of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(128)]
			public string City
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the country code of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(2)]
			public string CountryCode
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the country name of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(64)]
			public string CountryName
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the latitude of the entity's creator, as determined by geolocation.
			/// </summary>
			public decimal? Latitude
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the longitude of the entity's creator, as determined by geolocation.
			/// </summary>
			public decimal? Longitude
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the postal code of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(16)]
			public string PostalCode
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the region code of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(16)]
			public string RegionCode
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the state/province/region name of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(128)]
			public string RegionName
			{
				get;
				set;
			}

			/// <summary>
			/// Gets or sets the time zone of the entity's creator, as determined by geolocation.
			/// </summary>
			[MaxLength(64)]
			public string TimeZone
			{
				get;
				set;
			}

			#endregion Properties

			#region Methods

			/// <summary>
			/// Returns a string that represents the current object.
			/// </summary>
			/// <returns>A string that represents the current object.</returns>
			public override string ToString()
			{
				return string.Format("{0}",
					/* 0 */ string.Join(", ",
					/* 1 */ new[]
						{
							!string.IsNullOrWhiteSpace(this.CountryCode)
								? string.Format("CountryCode = {0}", this.CountryCode)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.CountryName)
								? string.Format("CountryName = {0}", this.CountryName)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.RegionCode)
								? string.Format("RegionCode = {0}", this.RegionCode)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.RegionName)
								? string.Format("RegionName = {0}", this.RegionName)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.City)
								? string.Format("City = {0}", this.City)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.PostalCode)
								? string.Format("PostalCode = {0}", this.PostalCode)
								: string.Empty,
							!string.IsNullOrWhiteSpace(this.TimeZone)
								? string.Format("TimeZone = {0}", this.TimeZone)
								: string.Empty,
							this.Latitude.HasValue
								? string.Format("Latitude = {0}", this.Latitude.Value)
								: string.Empty,
							this.Longitude.HasValue
								? string.Format("Longitude = {0}", this.Longitude.Value)
								: string.Empty
						}.Where(s => !string.IsNullOrWhiteSpace(s))));
			}

			#endregion Methods
		}

		#endregion Classes
	}
}