
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Listing {
        /// <summary>
        /// Gets or sets the category of the listing.
        /// </summary>
        public string Category {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the commission rate of the listing.
        /// </summary>
        public decimal? CommissionRate {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the condition of the item in the listing.
        /// </summary>
        public string Condition {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the currency of the current price of the listing.
        /// </summary>
        public string CurrentPriceCurrency {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the current price of the listing.
        /// </summary>
        public decimal? CurrentPriceValue {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the current price of the listing in USD.
        /// </summary>
        public decimal? CurrentPriceValueUsd {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the database ID of the listing.
        /// </summary>
        public long DatabaseId {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the end date of this listing.
        /// </summary>
        public DateTimeOffset? DateEnding {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date when this listing was retrieved.
        /// </summary>
        public DateTimeOffset DateRetrieved {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the start date of this listing.
        /// </summary>
        public DateTimeOffset? DateStarted {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the features of the item in this listing.
        /// </summary>
        public string[] Features {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the unique ID of this listing.
        /// </summary>
        public string Id {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URLs of images of this listing.
        /// </summary>
        public string[] ImageUrls {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URLs of small versions of the images of this listing.
        /// </summary>
        public string[] ImageUrlsSmall {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location of the item in this listing.
        /// </summary>
        public string Location {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the manufacturer of the item in this listing.
        /// </summary>
        public string Manufacturer {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the metadata of this listing.
        /// </summary>
        public Dictionary<string, string> Metadata {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the item in this listing.
        /// </summary>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the package type of the listing (for example, "piece" or "lot").
        /// </summary>
        public string PackageType {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the portal that contains this listing.
        /// </summary>
        public string Portal {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container for information regarding the purchasing of this listing.
        /// </summary>
        public PurchasingInfo Purchasing {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quantity of the item in this listing.
        /// </summary>
        public decimal? Quantity {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quantity available of the item in this listing.
        /// </summary>
        public decimal? QuantityAvailable {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search term that was provided to the portal for this listing.
        /// </summary>
        public string SearchTerm {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container for information regarding the seller of this listing.
        /// </summary>
        public SellerInfo Seller {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the container for information regarding the shipping of this listing.
        /// </summary>
        public ShippingInfo Shipping {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the subtitle of this listing.
        /// </summary>
        public string Subtitle {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title of this listing.
        /// </summary>
        public string Title {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of this listing.
        /// </summary>
        public string Type {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the URL of this listing.
        /// </summary>
        public string Url {
            get;
            set;
        }


        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return this.Title;
        }

        /// <summary>
        /// Container for information regarding the purchasing of a listing.
        /// </summary>
        public class PurchasingInfo {
            /// <summary>
            /// Initializes a new instance of the <see cref="PurchasingInfo" /> class.
            /// </summary>
            public PurchasingInfo() {
                this.ReturnPolicy = new ReturnPolicyInfo();
            }

            /// <summary>
            /// Gets or sets whether the listing has a "best offer" option enabled.
            /// </summary>
            public bool? BestOfferEnabled {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of bids that have been made for the listing.
            /// </summary>
            public int? Bids {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the currency of the instant-buy price of the listed item.
            /// </summary>
            public string BuyItNowPriceCurrency {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value of the instant-buy price of the listed item.
            /// </summary>
            public decimal? BuyItNowPriceValue {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value of the instant-buy price of the listed item in USD.
            /// </summary>
            public decimal? BuyItNowPriceValueUsd {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the minimum number of units that must be ordered.
            /// </summary>
            public decimal? MinimumOrderUnits {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the payment methods accepted for the listing.
            /// </summary>
            public string[] PaymentMethods {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the return policy for the listing.
            /// </summary>
            public ReturnPolicyInfo ReturnPolicy {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets whether returns will be accepted for the listing.
            /// </summary>
            public bool? ReturnsAccepted {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of units contained per order.
            /// </summary>
            public decimal? UnitsPerOrder {
                get;
                set;
            }

            /// <summary>
            /// Container for information regarding the return policy of a listing.
            /// </summary>
            public class ReturnPolicyInfo {

                /// <summary>
                /// Gets or sets how many days the buyer has to return the listed item after purchase.
                /// </summary>
                public int? DaysToReturn {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the description of the return policy of the listing.
                /// </summary>
                public string Description {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the refund options for the listed item.
                /// </summary>
                public string RefundOptions {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the restocking fee percent for the listing.
                /// </summary>
                public decimal? RestockingFeePercent {
                    get;
                    set;
                }

                /// <summary>
                /// Gets or sets the party responsible for paying return shipping for the listing.
                /// </summary>
                public string ShippingPaidBy {
                    get;
                    set;
                }

            }

        }

        /// <summary>
        /// Container for information regarding the seller of a listing.
        /// </summary>
        public class SellerInfo {
            /// <summary>
            /// Symbols used by Taobao to represent seller ratings.
            /// </summary>
            private static readonly string[] TaobaoRatingSymbols = new[] { "❤️", "💎", "💍", "👑" };

            /// <summary>
            /// Initializes a new instance of the <see cref="SellerInfo" /> class.
            /// </summary>
            public SellerInfo() {
            }

            /// <summary>
            /// Gets or sets the name of the seller.
            /// </summary>
            public string Name {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the percentage of positive feedback of the seller.
            /// </summary>
            public decimal? PositiveFeedbackPercent {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of sales the seller has made of this particular item.
            /// </summary>
            public int? SalesOfCurrentItem {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the score of the seller.
            /// </summary>
            public long? Score {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the URL of the seller.
            /// </summary>
            public string Url {
                get;
                set;
            }

            /// <summary>
            /// Returns <see cref="Score" /> as represented by Taobao's rating symbols.
            /// </summary>
            /// <returns>Combination of emoji that correspond with Taobao's rating symbols.</returns>
            public string GetScoreAsTaobaoSymbols() {
                int score = Convert.ToInt32(this.Score ?? 0);
                score = score < 0 ? 0 : score > 20 ? 20 : score;

                if (score == 0)
                    return string.Empty;

                return string.Concat(Enumerable.Repeat(TaobaoRatingSymbols[Convert.ToInt32(Math.Floor((score - 1) / 5m))], score % 5 == 0 ? 5 : score % 5));
            }

        }

        /// <summary>
        /// Container for information regarding the shipping of a listing.
        /// </summary>
        public class ShippingInfo {
            /// <summary>
            /// Initializes a new instance of the <see cref="ShippingInfo" /> class.
            /// </summary>
            public ShippingInfo() {
            }

            /// <summary>
            /// Gets or sets the currency of the shipping cost of the listing.
            /// </summary>
            public string CostCurrency {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value of the shipping cost of the listing.
            /// </summary>
            public decimal? CostValue {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the value of the shipping cost of the listing in USD.
            /// </summary>
            public decimal? CostValueUsd {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets whether the listing has an expedited shipping option available.
            /// </summary>
            public bool? ExpeditedAvailable {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of days expected for handling of the listing.
            /// </summary>
            public int? HandlingDays {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets whether the listing has a one-day delivery option available.
            /// </summary>
            public bool? OneDayAvailable {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the locations to which the item in the listing can be shipped.
            /// </summary>
            public string[] ShipsTo {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the type of shipping offered for the listing.
            /// </summary>
            public string Type {
                get;
                set;
            }

        }
    }
}