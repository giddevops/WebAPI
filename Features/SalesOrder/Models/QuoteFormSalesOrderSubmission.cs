using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class QuoteFormSalesOrderSubmission {
        public string BillingName;
        public string BillingAttention;
        public string BillingAddress1;
        public string BillingAddress2;
        public string BillingAddress3;
        public string BillingCity;
        public string BillingState;
        public string BillingPostalCode;
        public string BillingCountryCode;
        public string ShippingName;
        public string ShippingAttention;
        public string ShippingAddress1;
        public string ShippingAddress2;
        public string ShippingAddress3;
        public string ShippingCity;
        public string ShippingState;
        public string ShippingPostalCode;
        public string ShippingCountryCode;

        public int? ShippingCarrierId;
        public int? ShippingCarrierShippingMethodId;
        public int? ShippingTypeId;
        

        public string ShippingAccountNumber;
        public bool PartialShipAccepted;
        public bool SaturdayDeliveryAccepted;

        public string CreditCardName;
        public string CreditCardNumber;
        public string CreditCardSecurityCode;
        public string CreditCardExpirationMonth;
        public string CreditCardExpirationYear;
        public string PaypalEmail;

        public int? SalesOrderPaymentMethodId;
        public int? PaymentTermId;


        public string InternalReferenceNumber;
        public string Notes;

        public int? QuoteId;
        
    }
}
