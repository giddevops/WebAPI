namespace GidIndustrial.Gideon.WebApi
{
	/// <summary>
	/// Container for QuickBooks options.
	/// </summary>
	public class QuickBooksOptions
	{
		#region Properties

		/// <summary>
		/// Gets or sets the URL of the QuickBooks APIl.
		/// </summary>
		public string ApiUrl { get; set; }

		/// <summary>
		/// Gets or sets the client ID of the app.
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// Gets or sets the client secret of the app.
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// Gets or sets the company ID.
		/// </summary>
		public string CompanyId { get; set; }

		/// <summary>
		/// Gets or sets the URL of the QuickBooks discovery document.
		/// </summary>
		public string DiscoveryUrl { get; set; }

		/// <summary>
		/// Gets or sets the refresh token of the app.
		/// </summary>
		public string RefreshToken { get; set; }

		#endregion Properties
	}
}