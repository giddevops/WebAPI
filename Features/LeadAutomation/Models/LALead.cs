// using LeadAutomation.Firefly.Exchange.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LeadAutomation.Pigeon.Exchange.Entities {
    /// <summary>
    /// Container for a lead and its details.
    /// </summary>
    public class LALead {
        #region Fields

        /// <summary>
        /// <see cref="LeadCustomerInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        private LeadCustomerInfo customerInfo = null;

        /// <summary>
        /// <see cref="LeadProductInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        private LeadProductInfo productInfo = null;

        /// <summary>
        /// <see cref="TechnicalInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        private TechnicalInfo technicalInfo = null;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LALead" /> class.
        /// </summary>
        public LALead() {
            this.CustomerInfo = new LeadCustomerInfo();
            this.ProductInfo = new LeadProductInfo();
            this.TechnicalInfo = new TechnicalInfo();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether an auto response was sent.
        /// </summary>
        public bool AutoResponseSent { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LeadCustomerInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        public LeadCustomerInfo CustomerInfo {
            get { return this.customerInfo; }
            set { this.customerInfo = value ?? new LeadCustomerInfo(); }
        }

        /// <summary>
        /// Gets or sets the time of creation of this <see cref="LALead" />.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of this <see cref="LALead" />.
        /// </summary>
        public long? Id { get; set; }
        
        //mongodb identifier for ones that come from the new system
        public string _Id { get; set; }

        // /// <summary>
        // /// Gets or sets the <see cref="LeadNobid" /> of this <see cref="Lead" />.
        // /// </summary>
        // public LeadNobid Nobid
        // {
        // 	get;
        // 	set;
        // }

        /// <summary>
        /// Gets or sets the internal notes of this <see cref="Lead" />.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LeadProductInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        public LeadProductInfo ProductInfo {
            get { return this.productInfo; }
            set { this.productInfo = value ?? new LeadProductInfo(); }
        }

        public List<LeadProductInfo> Products { get; set; }

        /// <summary>
        /// Gets or sets the quality of this <see cref="LALead" />.
        /// </summary>
        public int? Quality { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TechnicalInfo" /> container of this <see cref="LALead" />.
        /// </summary>
        public TechnicalInfo TechnicalInfo {
            get { return this.technicalInfo; }
            set { this.technicalInfo = value ?? new TechnicalInfo(); }
        }

        public string Emoji { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return string.Format("{0}: {1}",
                /* 0 */ this.Id,
                /* 1 */ string.Join(", ",
                    new[]
                    {
                        string.Format("DateCreated = {0}", this.DateCreated),
                        string.Format("Quality = {0}",
                            this.Quality.HasValue
                                ? this.Quality.Value.ToString()
                                : "(???)")
                    }.Where(s => !string.IsNullOrWhiteSpace(s))));
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Container for a <see cref="LALead" />'s customer information.
        /// </summary>
        public class LeadCustomerInfo {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LeadCustomerInfo" /> class.
            /// </summary>
            public LeadCustomerInfo() {
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets the first address line of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Address1 { get; set; }

            /// <summary>
            /// Gets or sets the second address line of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Address2 { get; set; }

            /// <summary>
            /// Gets or sets the third address line of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Address3 { get; set; }

            /// <summary>
            /// Gets or sets the city of the customer.
            /// </summary>
            [MaxLength(128)]
            public string City { get; set; }

            /// <summary>
            /// Gets or sets the comments provided by the customer.
            /// </summary>
            public string Comments { get; set; }

            /// <summary>
            /// Gets or sets the company of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Company { get; set; }

            /// <summary>
            /// Gets or sets the ISO 3166-1-Alpha-2 two-letter country code of the customer.
            /// </summary>
            [MaxLength(2)]
            public string CountryCode { get; set; }

            /// <summary>
            /// Gets or sets the name of the country of the customer.
            /// </summary>
            [MaxLength(64)]
            public string CountryName { get; set; }

            /// <summary>
            /// Gets or sets the customer type of the customer.
            /// </summary>
            [MaxLength(32)]
            public string CustomerType { get; set; }

            /// <summary>
            /// Gets or sets the DUNS number of the customer.
            /// </summary>
            [MaxLength(9)]
            public string DunsNumber { get; set; }

            /// <summary>
            /// Gets or sets the email address of the customer.
            /// </summary>
            [MaxLength(64)]
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets the full name of the customer.
            /// </summary>
            [MaxLength(128)]
            public string FullName { get; set; }

            /// <summary>
            /// Gets or sets the phone number of the customer.
            /// </summary>
            [MaxLength(64)]
            public string Phone { get; set; }

            /// <summary>
            /// Gets or sets the position of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Position { get; set; }

            /// <summary>
            /// Gets or sets the postal code of the customer.
            /// </summary>
            [MaxLength(16)]
            public string PostalCode { get; set; }

            /// <summary>
            /// Gets or sets the state/province/region name of the customer.
            /// </summary>
            [MaxLength(128)]
            public string RegionName { get; set; }

            /// <summary>
            /// Gets or sets the website of the customer.
            /// </summary>
            [MaxLength(128)]
            public string Website { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString() {
                return string.Format("{0}",
                    /* 0 */ string.Join(", ",
                    /* 1 */ new[]
                        {
                            !string.IsNullOrWhiteSpace(this.FullName)
                                ? string.Format("FullName = {0}", this.FullName)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Position)
                                ? string.Format("Position = {0}", this.Position)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Company)
                                ? string.Format("Company = {0}", this.Company)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.CustomerType)
                                ? string.Format("CustomerType = {0}", this.CustomerType)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Email)
                                ? string.Format("Email = {0}", this.Email)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Phone)
                                ? string.Format("Phone = {0}", this.Phone)
                                : string.Empty
                        }.Where(s => !string.IsNullOrWhiteSpace(s))));
            }

            #endregion Methods
        }

        /// <summary>
        /// Container for a <see cref="LALead" />'s product information.
        /// </summary>
        public class LeadProductInfo {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="LeadProductInfo" /> class.
            /// </summary>
            public LeadProductInfo() {
            }

            #endregion Constructors

            #region Properties

            public string _Id { get; set; }

            /// <summary>
            /// Gets or sets the category of the requested product.
            /// </summary>
            [MaxLength(128)]
            public string Category { get; set; }

            /// <summary>
            /// Gets or sets the manufacturer of the requested product.
            /// </summary>
            [MaxLength(128)]
            public string Manufacturer { get; set; }

            /// <summary>
            /// Gets or sets the part number of the requested product.
            /// </summary>
            [MaxLength(128)]
            public string PartNumber { get; set; }

            /// <summary>
            /// Gets or sets the quantity of the requested product.
            /// </summary>
            public decimal? Quantity { get; set; }

            /// <summary>
            /// Gets or sets the number of weeks until required delivery of the requested product.
            /// </summary>
            public int? RequiredDeliveryWeeks { get; set; }

            /// <summary>
            /// Gets or sets the service to be performed for the requested product (such as Sales or Repair).
            /// </summary>
            [MaxLength(32)]
            public string Service { get; set; }

            /// <summary>
            /// Gets or sets whether the product entered was entered freeform
            /// </summary>
            public bool Freeform { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Returns a string that represents the current object.
            /// </summary>
            /// <returns>A string that represents the current object.</returns>
            public override string ToString() {
                return string.Format("{0}",
                    /* 0 */ string.Join(", ",
                    /* 1 */ new[]
                        {
                            !string.IsNullOrWhiteSpace(this.PartNumber)
                                ? string.Format("PartNumber = {0}", this.PartNumber)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Manufacturer)
                                ? string.Format("Manufacturer = {0}", this.Manufacturer)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Category)
                                ? string.Format("Category = {0}", this.Category)
                                : string.Empty,
                            this.Quantity.HasValue
                                ? string.Format("Quantity = {0}", this.Quantity.Value)
                                : string.Empty,
                            this.RequiredDeliveryWeeks.HasValue
                                ? string.Format("RequiredDeliveryWeeks = {0}", this.RequiredDeliveryWeeks.Value)
                                : string.Empty,
                            !string.IsNullOrWhiteSpace(this.Service)
                                ? string.Format("Service = {0}", this.Service)
                                : string.Empty,
                        }.Where(s => !string.IsNullOrWhiteSpace(s))));
            }

            #endregion Methods
        }

        #endregion Classes
    }

}