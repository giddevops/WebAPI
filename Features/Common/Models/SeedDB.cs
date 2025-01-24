using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class SeedDB {
        // public enum CompanyAddressTypes
        // {
        //     Shipping = 1,
        //     Billing = 2
        // }
        public static void InsertOrUpdateList(AppDBContext context, List<dynamic> items) {
            if (items.Count == 0)
                return;
            Type type = items.First().GetType();
            var tableName = context.Model.FindEntityType(type.ToString()).SqlServer().TableName;

            using (var transaction = context.Database.BeginTransaction()) {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + tableName + " ON");
                context.SaveChanges();
                foreach (var item in items) {
                    string id = item.Id.ToString();
                    var rowFound = false;
                    using (var dr = context.Database.ExecuteSqlQuery("SELECT Id FROM " + tableName + " WHERE Id=" + id)) {
                        var reader = dr.DbDataReader;
                        while (reader.Read()) {
                            rowFound = true;
                            break;
                        }
                    }
                    if (rowFound == false)
                        context.Add(item);
                }
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT " + tableName + " OFF");
                try {
                    Console.WriteLine("Committing changes");
                    transaction.Commit();
                }
                catch (Exception ex) {
                    transaction.Rollback();
                }
            }
            Console.WriteLine("it is done");
        }
        public static void Seed(AppDBContext context) {

            //add full text index to product description

            // using (var transaction = context.Database.BeginTransaction()) {
            // context.Database.ExecuteSqlCommand("CREATE FULLTEXT CATALOG IF NOT EXISts FullText AS DEFAULT");
            // context.Database.ExecuteSqlCommand("CREATE FULLTEXT INDEX IF NOT EXISTS ON dbo.Product(Description) KEY INDEX UI_Product_Description WITH STOPLIST = SYSTEM");
            // context.SaveChanges();
            // }

            // using (var transaction = context.Database.BeginTransaction()) {
            //     context.Database.ExecuteSqlCommand("Update Lead SET OldId = Id WHERE OldId IS NULL AND Id < 40000");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("Update Lead SET NewId = (OldId+20000) WHERE NewId IS NULL AND Id < 40000");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadChatMessage NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadAttachment NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadEventLogEntry NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadLineItem NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadNote NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadRoutingAction NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE Quote NOCHECK CONSTRAINT ALL");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE SalesOrder NOCHECK CONSTRAINT ALL");
            //     // context.Database.ExecuteSqlCommand("EXEC sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Lead ON");
            //     context.SaveChanges();
            //     var leads = context.Leads.AsNoTracking().Where(item => item.Id < 40000);
            //     int i = 0;
            //     foreach(var lead in leads){
            //         lead.Id = (int)lead.NewId;
            //         context.Leads.Add(lead);
            //         if(i++ % 100 == 0)
            //             context.SaveChanges();
            //     }
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Lead OFF");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("Update LeadChatMessage SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update LeadAttachment SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update LeadEventLogEntry SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update LeadLineItem SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update LeadNote SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update LeadRoutingAction SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update Quote SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.Database.ExecuteSqlCommand("Update SalesOrder SET LeadId = (LeadId+20000) WHERE LeadId IS NOT NULL and LeadId < 40000");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadChatMessage WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadAttachment WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadEventLogEntry WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadLineItem WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadNote WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE LeadRoutingAction WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE Quote WITH CHECK CHECK CONSTRAINT all");
            //     context.Database.ExecuteSqlCommand("ALTER TABLE SalesOrder WITH CHECK CHECK CONSTRAINT all");
            //     // context.Database.ExecuteSqlCommand("exec sp_MSforeachtable @command1=\"print '?'\", @command2=\"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"");
            //     context.SaveChanges();
            //     context.Database.ExecuteSqlCommand("DELETE FROM Lead WHERE Id < 40000");
            //     transaction.Commit();
            // }

            //Need to do stuf

            // var incorrectSalesOrders = context.SalesOrders.Include(item => item.BillingAddress).Where(item => item.BillingAddressId == item.ShippingAddressId && item.BillingAddressId != null).ToList();
            // Console.WriteLine("The number iss " + incorrectSalesOrders.Count);
            // var q = 0;
            // foreach(SalesOrder salesOrder in incorrectSalesOrders){
            //     Console.WriteLine("NUmber is ... " + q++);
            //     salesOrder.ShippingAddress = null;
            //     salesOrder.ShippingAddressId = null;
            //     var newAddress = new Address{
            //         Address1 = salesOrder.BillingAddress.Address1,
            //         Address2 = salesOrder.BillingAddress.Address2,
            //         Address3 = salesOrder.BillingAddress.Address3,
            //         Attention = salesOrder.BillingAddress.Attention,
            //         City = salesOrder.BillingAddress.City,
            //         State = salesOrder.BillingAddress.State,
            //         CountryId = salesOrder.BillingAddress.CountryId,
            //         CreatedAt = salesOrder.BillingAddress.CreatedAt,
            //         Name = salesOrder.BillingAddress.Name,
            //         PhoneNumber = salesOrder.BillingAddress.PhoneNumber,
            //         ZipPostalCode = salesOrder.BillingAddress.ZipPostalCode
            //     };

            //     context.Addresses.Add(newAddress);
            //     context.SaveChanges();

            //     salesOrder.ShippingAddressId = newAddress.Id;
            //     context.SaveChanges();
            // }



            if (!context.QuoteAddressTypes.Any())
                foreach (string type in SeedDB.QuoteAddressTypes)
                    context.QuoteAddressTypes.Add(new QuoteAddressType { Value = type, CreatedAt = DateTime.UtcNow });

            //Allow identity insert
            if (!context.CompanyAddressTypes.Any()) {
                using (var transaction = context.Database.BeginTransaction()) {
                    foreach (string type in SeedDB.CompanyAddressTypes)
                        context.CompanyAddressTypes.Add(new CompanyAddressType {
                            Id = SeedDB.CompanyAddressTypes.IndexOf(type) + 1,
                            Value = type,
                            Locked = true
                        });
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CompanyAddressType ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CompanyAddressType OFF");
                    transaction.Commit();
                }
            }
            // if (!context.AttachmentTypes.Any())
            // {
            //     using (var transaction = context.Database.BeginTransaction())
            //     {
            //         foreach (string type in SeedDB.AttachmentTypes)
            //             context.AttachmentTypes.Add(new AttachmentType
            //             {
            //                 Id = SeedDB.AttachmentTypes.IndexOf(type) + 1,
            //                 Value = type,
            //                 Locked = true
            //             });
            //         context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT AttachmentType ON");
            //         context.SaveChanges();
            //         context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT AttachmentType OFF");
            //         transaction.Commit();
            //     }
            // }
            if (!context.LeadOriginOptions.Any()) {
                using (var transaction = context.Database.BeginTransaction()) {
                    foreach (string option in SeedDB.LeadOriginOptions)
                        context.LeadOriginOptions.Add(new LeadOriginOption {
                            Id = SeedDB.LeadOriginOptions.IndexOf(option) + 1,
                            Value = option,
                            Locked = true
                        });
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT LeadOriginOption ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT LeadOriginOption OFF");
                    transaction.Commit();
                }
            }
            if (!context.InventoryItemLocationOptions.Any()) {
                using (var transaction = context.Database.BeginTransaction()) {
                    foreach (InventoryItemLocationOptions inventoryItemLocationOptionId in Enum.GetValues(typeof(InventoryItemLocationOptions))) {
                        Regex r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
                        var name = Enum.GetName(typeof(InventoryItemLocationOptions), inventoryItemLocationOptionId);
                        name = r.Replace(name, " ");
                        context.InventoryItemLocationOptions.Add(new InventoryItemLocationOption {
                            Id = (int)inventoryItemLocationOptionId,
                            Value = name,
                            Locked = true,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT InventoryItemLocationOption ON");
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT InventoryItemLocationOption OFF");
                    transaction.Commit();
                }
            }

            if (context.EmailTemplateTypes.Count() != SeedDB.EmailTemplateTypes.Count) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT EmailTemplateType ON");
                    context.SaveChanges();
                    foreach (string option in SeedDB.EmailTemplateTypes) {
                        if (!context.EmailTemplateTypes.Any(item => item.Id == SeedDB.EmailTemplateTypes.IndexOf(option) + 1)) {
                            context.EmailTemplateTypes.Add(new EmailTemplateType {
                                Id = SeedDB.EmailTemplateTypes.IndexOf(option) + 1,
                                Value = option,
                                Locked = true
                            });
                        }
                    }
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT EmailTemplateType OFF");
                    transaction.Commit();
                }
            }

            if (context.GidLocationOptions.Count() != SeedDB.GidLocationOptions.Count) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT GidLocationOption ON");
                    context.SaveChanges();
                    foreach (var gidLocationOption in SeedDB.GidLocationOptions) {
                        if (!context.GidLocationOptions.Any(item => item.Id == gidLocationOption.Id))
                            context.GidLocationOptions.Add(gidLocationOption);
                    }
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT GidLocationOption OFF");
                    transaction.Commit();
                }
            } else {
                foreach (var gidLocationOption in SeedDB.GidLocationOptions) {
                    var dbGidLocationOption = context.GidLocationOptions
                        .Include(item => item.MainAddress)
                        .FirstOrDefault(item => item.Id == gidLocationOption.Id);
                    if (dbGidLocationOption.MainAddress == null) {
                        dbGidLocationOption.MainAddress = gidLocationOption.MainAddress;
                    }
                    if (dbGidLocationOption.BankingInfo == null) {
                        dbGidLocationOption.BankingInfo = gidLocationOption.BankingInfo;
                    }
                    context.SaveChanges();
                    context.Entry(dbGidLocationOption).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }

            if (context.Configs.FirstOrDefault(item => item.Id == 1) == null) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Config ON");
                    context.SaveChanges();
                    context.Configs.Add(new Config {
                        Id = 1,
                        Data = ""
                    });
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Config OFF");
                    transaction.Commit();
                }
            }

            if (context.Permissions.Count() != SeedDB.Permissions.Count) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Permission ON");
                    context.SaveChanges();
                    foreach (var permission in SeedDB.Permissions) {
                        if (!context.Permissions.Any(item => item.Id == permission.Id)) {
                            context.Permissions.Add(permission);
                        }
                    }
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT Permission OFF");
                    transaction.Commit();
                }
            }

            if (context.InventoryItemStatusOptions.Count() != SeedDB.InventoryItemStatusOptions.Count) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT InventoryItemStatusOption ON");
                    context.SaveChanges();
                    var i = 0;
                    foreach (string value in SeedDB.InventoryItemStatusOptions) {
                        ++i;
                        if (!context.InventoryItemStatusOptions.Any(item => item.Id == i)) {
                            context.InventoryItemStatusOptions.Add(new InventoryItemStatusOption {
                                Id = i,
                                CreatedAt = DateTime.UtcNow,
                                Locked = true,
                                Value = value
                            });
                        }
                    }
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT InventoryItemStatusOption OFF");
                    transaction.Commit();
                }
            }
            if (context.CashDisbursementReasonOptions.Count() != SeedDB.CashDisbursementReasonOptions.Count) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CashDisbursementReasonOption ON");
                    context.SaveChanges();
                    var i = 0;
                    foreach (string value in SeedDB.CashDisbursementReasonOptions) {
                        ++i;
                        if (!context.CashDisbursementReasonOptions.Any(item => item.Id == i)) {
                            context.CashDisbursementReasonOptions.Add(new CashDisbursementReasonOption {
                                Id = i,
                                CreatedAt = DateTime.UtcNow,
                                Locked = true,
                                Value = value
                            });
                        }
                    }
                    context.SaveChanges();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CashDisbursementReasonOption OFF");
                    transaction.Commit();
                }
            }

            using (var transaction = context.Database.BeginTransaction()) {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT LineItemServiceType ON");
                context.SaveChanges();
                var i = 0;
                foreach (string value in SeedDB.LineItemServiceTypes) {
                    ++i;
                    if (!context.LineItemServiceTypes.Any(item => item.Id == i)) {
                        context.LineItemServiceTypes.Add(new LineItemServiceType {
                            Id = i,
                            CreatedAt = DateTime.UtcNow,
                            Locked = true,
                            Value = value
                        });
                    }
                }
                context.SaveChanges();
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT LineItemServiceType OFF");
                transaction.Commit();
            }
            SeedDB.InsertOrUpdateList(context, WorkLogItemActivityOptions);

            //auto add any new ones
            foreach (string type in SeedDB.AttachmentTypes) {
                if (context.AttachmentTypes.FirstOrDefault(item => item.Value == type) == null) {
                    context.AttachmentTypes.Add(new AttachmentType {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        Locked = true
                    });
                }
            }
            foreach (string type in SeedDB.CashReceiptTypes) {
                if (context.CashReceiptTypes.FirstOrDefault(item => item.Value == type) == null) {
                    context.CashReceiptTypes.Add(new CashReceiptType {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        // Locked = true
                    });
                }
            }
            foreach (string type in SeedDB.LeadStatusOptions) {
                if (context.LeadStatusOptions.FirstOrDefault(item => item.Value == type) == null) {
                    context.LeadStatusOptions.Add(new LeadStatusOption {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        Locked = true
                    });
                }
            }
            foreach (string type in SeedDB.QuoteStatusOptions) {
                if (context.QuoteStatusOptions.FirstOrDefault(item => item.Value == type) == null) {
                    context.QuoteStatusOptions.Add(new QuoteStatusOption {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        Locked = true
                    });
                }
            }
            foreach (string type in SeedDB.SalesOrderStatusOptions) {
                if (context.SalesOrderStatusOptions.FirstOrDefault(item => item.Value == type) == null) {
                    context.SalesOrderStatusOptions.Add(new SalesOrderStatusOption {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        Locked = true
                    });
                }
            }
            foreach (string type in SeedDB.SalesOrderPaymentMethods) {
                if (context.SalesOrderPaymentMethods.FirstOrDefault(item => item.Value == type) == null) {
                    context.SalesOrderPaymentMethods.Add(new SalesOrderPaymentMethod {
                        Value = type,
                        CreatedAt = DateTime.UtcNow,
                        Locked = true
                    });
                }
            }
            foreach (var option in SeedDB.CurrencyOptions) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CurrencyOption ON");
                    context.SaveChanges();
                    if (context.CurrencyOptions.FirstOrDefault(item => item.Id == option.Id) == null) {
                        context.CurrencyOptions.Add(option);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT CurrencyOption OFF");
                    context.SaveChanges();
                }
            }
            foreach (var value in SeedDB.ProductConditionOptions) {
                if (context.ProductConditionOptions.FirstOrDefault(item => item.Value == value) == null) {
                    context.ProductConditionOptions.Add(new ProductConditionOption {
                        Value = value
                    });
                }
            }
            foreach (var value in SeedDB.PurchaseOrderStatusOptions) {
                using (var transaction = context.Database.BeginTransaction()) {
                    if (context.PurchaseOrderStatusOptions.FirstOrDefault(item => item.Value == value) == null) {
                        context.PurchaseOrderStatusOptions.Add(new PurchaseOrderStatusOption {
                            Value = value
                        });
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            foreach (var option in SeedDB.PurchaseOrderReasonOptions) {
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT PurchaseOrderReasonOption ON");
                    context.SaveChanges();
                    if (context.PurchaseOrderReasonOptions.FirstOrDefault(item => item.Id == option.Id) == null) {
                        context.PurchaseOrderReasonOptions.Add(option);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT PurchaseOrderReasonOption OFF");
                    context.SaveChanges();
                }
            }
            if (!context.ShippingCarriers.Any()) {
                foreach (ShippingCarrier shippingCarrier in SeedDB.ShippingCarriers) {
                    var i = 0;
                    foreach (var shippingCarrierMethod in shippingCarrier.ShippingMethods) {
                        shippingCarrier.ShippingMethods[i].SortPosition = ++i;
                    }
                    context.ShippingCarriers.Add(shippingCarrier);
                }
            }

            foreach (var option in SeedDB.ScannerLabelTypes) {
                //Set the db primary key to 1000
                //but if there is stuff greater than 1000, automatically set it
                context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('ScannerLabelType', RESEED, 1000); DBCC CHECKIDENT ('ScannerLabelType', RESEED);");
                using (var transaction = context.Database.BeginTransaction()) {
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ScannerLabelType ON");
                    context.SaveChanges();
                    if (context.ScannerLabelTypes.FirstOrDefault(item => item.Id == option.Id) == null) {
                        context.ScannerLabelTypes.Add(option);
                    }
                    context.SaveChanges();
                    transaction.Commit();
                    context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT ScannerLabelType OFF");
                    context.SaveChanges();
                }
            }

            if (!context.LeadWebsites.Any())
                foreach (string type in SeedDB.LeadWebsites)
                    context.LeadWebsites.Add(new LeadWebsite { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.ShippingTypes.Any())
                foreach (string type in SeedDB.ShippingTypes)
                    context.ShippingTypes.Add(new ShippingType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.ContactAddressTypes.Any())
                foreach (string type in SeedDB.ContactAddressTypes)
                    context.ContactAddressTypes.Add(new ContactAddressType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.Portals.Any())
                foreach (string type in SeedDB.Portals)
                    context.Portals.Add(new Portal { Name = type, CreatedAt = DateTime.UtcNow });
            if (!context.CustomerTypes.Any())
                foreach (string type in SeedDB.CustomerTypes)
                    context.CustomerTypes.Add(new CustomerType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.ShippingMethods.Any())
                foreach (string type in SeedDB.ShippingMethods)
                    context.ShippingMethods.Add(new ShippingMethod { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.CompanyPhoneNumberTypes.Any())
                foreach (string type in SeedDB.CompanyPhoneNumberTypes)
                    context.CompanyPhoneNumberTypes.Add(new CompanyPhoneNumberType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.CompanyEmailAddressTypes.Any())
                foreach (string type in SeedDB.CompanyEmailAddressTypes)
                    context.CompanyEmailAddressTypes.Add(new CompanyEmailAddressType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.ContactPhoneNumberTypes.Any())
                foreach (string type in SeedDB.ContactPhoneNumberTypes)
                    context.ContactPhoneNumberTypes.Add(new ContactPhoneNumberType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.ContactEmailAddressTypes.Any())
                foreach (string type in SeedDB.ContactEmailAddressTypes)
                    context.ContactEmailAddressTypes.Add(new ContactEmailAddressType { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.InventoryItemConditionOptions.Any())
                foreach (string type in SeedDB.InventoryItemConditionOptions)
                    context.InventoryItemConditionOptions.Add(new InventoryItemConditionOption { Value = type, CreatedAt = DateTime.UtcNow });
            if (!context.Companies.Any())
                foreach (Company company in SeedDB.Companies)
                    context.Companies.Add(company);
            if (!context.Countries.Any()) {
                foreach (KeyValuePair<string, string> entry in SeedDB.CountryNames) {
                    context.Countries.Add(new Country {
                        Value = entry.Value,
                        CountryCode = entry.Key
                    });
                }
            }

            context.SaveChanges();
        }

        public static List<PurchaseOrderReasonOption> PurchaseOrderReasonOptions = new List<PurchaseOrderReasonOption>{
            new PurchaseOrderReasonOption{ Id = 1, Value = "Sales Order", Locked = true},
            new PurchaseOrderReasonOption{ Id = 2, Value = "Inventory"},
            new PurchaseOrderReasonOption{ Id = 3, Value = "Asset"},
        };
        public static List<GidLocationOption> GidLocationOptions = new List<GidLocationOption> {
            new GidLocationOption{
                Id = 1,
                Value = "GID Industrial",
                DefaultCurrency = "USD",
                CreatedAt = DateTime.UtcNow,
                DefaultShippingAddress = new Address{
                    Name = "GID Industrial",
                    Address1 = "1211 Executive Drive East",
                    City = "Richardson",
                    State = "TX",
                    ZipPostalCode = "75081"
                },
                MainAddress = new Address {
                    Name = "GID Industrial",
                    Address1 = "1218 Executive Drive West",
                    City = "Richardson",
                    State = "TX",
                    ZipPostalCode = "75081"
                },
                BillingAddress = new Address {
                    Name = "GID Industrial",
                    Address1 = "1213 Executive Drive East",
                    City = "Richardson",
                    State = "TX",
                    ZipPostalCode = "75081"
                },
                BankingInfo = @"Capital One Bank, NA
101 Stacy Road
Fairview, TX 75069-1519 USA
Beneficiary: GID Industrial
Account Number: 4670297290
Routing Number: 111901014
SWIFT Code: HIBKUS44"
            },
            new GidLocationOption{
                Id = 2,
                Value = "GID Industrial (Europe), Ltd.",
                DefaultCurrency = "EUR",
                CreatedAt = DateTime.UtcNow,
                DefaultShippingAddress = new Address{
                    Name = "GID Industrial (Europe), Ltd.",
                    Address1 = "Unit 3, Building 1",
                    Address2 = "Bundoran Retail Park",
                    City = "Bundoran",
                    State = "Co. Donegal"
                },
                MainAddress = new Address{
                    Name = "GID Industrial (Europe), Ltd.",
                    Address1 = "Unit 3, Building 1",
                    Address2 = "Bundoran Retail Park",
                    City = "Bundoran",
                    State = "Co. Donegal"
                },
                BillingAddress = new Address{
                    Name = "GID Industrial (Europe), Ltd.",
                    Address1 = "Unit 3, Building 1",
                    Address2 = "Bundoran Retail Park",
                    City = "Bundoran",
                    State = "Co. Donegal"
                },
                BankingInfo = @"AIB Bank
DUBLIN 1
DUBLIN, IE
BIC: AIBKIE2D
Sort Code: 93-73-04
Beneficiary: GID Industrial (Europe), Ltd.
IBAN: IE66AIBK93730431191022
Address: Unit 3, Building 1
Bundoran Retail Park
Bundoran, Co. Donegal"
            },
            new GidLocationOption{
                Id = 3,
                Value = "Gasliter",
                DefaultCurrency = "USD",
                CreatedAt = DateTime.UtcNow,
                DefaultShippingAddress = null,
                MainAddress = null,
                BankingInfo = ""
            },
        };
        public static List<ScannerLabelType> ScannerLabelTypes = new List<ScannerLabelType> {
            // new ScannerLabelType{
            //     Id=1,
            //     Locked=true,
            //     CreatedAt = DateTime.UtcNow,
            //     Name="Finish Action",
            //     ScannerLabelTypeClass=ScannerLabelTypeClass.EVENT,
            // }
        };
        public static List<dynamic> WorkLogItemActivityOptions = new List<dynamic> {
            new WorkLogItemActivityOption{Id=1,Locked=true,Value="Other"}
        };
        public static List<string> CashDisbursementReasonOptions = new List<string> { "Bill", "Refund" };
        public static List<string> ProductConditionOptions = new List<string> { "New", "Used", "Refurb" };
        public static List<string> SalesOrderPaymentMethods = new List<string> { "Credit Card", "Wire Transfer", "PayPal", "Check", "Terms" };
        public static List<string> ShippingTypes = new List<string> { "Direct", "Drop", "Blind" };
        public static string[] Portals = { "Ebay", "Broker Bin", "Net Components", "IC Source", "TaoBao", "Ali Express", "Alibaba" };
        public static string[] QuoteAddressTypes = { "Shipping", "Billing" };
        public static List<string> LeadStatusOptions = new List<string> { "Open", "Assigned", "Converted", "Duplicate", "No Bid", "Product Unavailable", "Asian Sourcing" };
        public static List<string> SalesOrderStatusOptions = new List<string> { "Open", "Order Confirmed", "Partially Shipped", "Shipment Complete", "Cancelled" };
        public static List<string> QuoteStatusOptions = new List<string> { "Pending", "Sent", "Converted" };
        public static List<string> LineItemConditionTypes = new List<string> { "New", "Used", "Refurbished", "GID Gold Standard" };
        public static List<string> CompanyAddressTypes = new List<string> { "Shipping", "Billing" };
        public static List<string> ContactAddressTypes = new List<string> { "Shipping", "Billing" };
        public static List<string> CustomerTypes = new List<string> { "Corporate End User", "Value-Added Reseller", "Reseller", "Broker", "Agent", "Manufacturer", "Distributor", "Government", "Military", "Educational", "Contractor", "Other" };
        public static List<string> AttachmentTypes = new List<string> { "Quote PDF", "Pro-Forma Invoice PDF", "Purchase Order PDF", "Cancel Purchase Order PDF", "Sales Order PDF", "Cancel Sales Order PDF", "Sales Order Customer Upload", "Product Image", "Invoice PDF", "Cancel Invoice PDF", "Shipping Label", "Repair Authorization PDF" };
        public static List<string> LeadOriginOptions = new List<string> { "Web" };
        public static List<string> ShippingMethods = new List<string> { "Ground", "Best Way", "DHL Express", "Overnight Early AM", "2nd Day", "3rd Day", "FedEx International First", "FedEx International Economy", "UPS Worldwide Expedited", "UPS Worldwide Express Plus", "UPS Red", "USPS Priority Express", "USPS Priority" };
        public static List<Permission> Permissions = new List<Permission> {
            new Permission{
                Id = 1,
                Name = "EditDropdownOptions",
                AllowedGroups = JsonConvert.SerializeObject(new string[]{
                "9dfe3e60-24fc-401d-8063-ac41b4ab5bd4", //Executive security Group
                "4b84ef34-f6e1-449c-96b9-80dc8a3882bc" //IT Security Group
            })},
            new Permission{
                Id = 2,
                Name = "ManagePermissions", AllowedGroups = JsonConvert.SerializeObject(new string[]{
                "9dfe3e60-24fc-401d-8063-ac41b4ab5bd4", //Executive security Group
                "4b84ef34-f6e1-449c-96b9-80dc8a3882bc" //IT Security Group
            })},
            new Permission{
                Id = 3,
                Name = "EditUsers", AllowedGroups = JsonConvert.SerializeObject(new string[]{
                "9dfe3e60-24fc-401d-8063-ac41b4ab5bd4", //Executive security Group
                "4b84ef34-f6e1-449c-96b9-80dc8a3882bc" //IT Security Group
            })},
            new Permission{
                Id = 4,
                Name = "ManageBilling", AllowedGroups = JsonConvert.SerializeObject(new string[]{
                "9dfe3e60-24fc-401d-8063-ac41b4ab5bd4", //Executive security Group
                "4b84ef34-f6e1-449c-96b9-80dc8a3882bc", //IT Security Group
                "aa12a2ed-1a42-48b2-a877-0822d58f33d9", //Finance security group
            })},
            new Permission{
                Id = 5,
                Name = "EditViews", AllowedGroups = JsonConvert.SerializeObject(new string[]{
                "9dfe3e60-24fc-401d-8063-ac41b4ab5bd4", //Executive security Group
                "4b84ef34-f6e1-449c-96b9-80dc8a3882bc", //IT Security Group
            })}
        };
        public static List<string> PurchaseOrderStatusOptions = new List<string> { "Open", "Needs Tracking", "Canceled", "Closed", "In-Route", "Nonconforming", "Received", "Partially Received", "Refunded", "Return To Vendor", "Sent" };
        public static List<string> CompanyPhoneNumberTypes = new List<string> { "Main", "Sales", "Account Payable", "Account Receivable", "Customer Service" };
        public static List<string> CompanyEmailAddressTypes = new List<string> { "Main", "Sales", "Account Payable", "Account Receivable", "Customer Service" };
        public static List<string> ContactPhoneNumberTypes = new List<string> { "Work", "Mobile", "Home" };
        public static List<string> ContactEmailAddressTypes = new List<string> { "Work", "Personal" };
        public static List<string> EmailTemplateTypes = new List<string> { "Quote", "Purchase Order", "Cancel Purchase Order", "RMA", "Pro Forma Invoice", "Sales Order", "Cancel Sales Order", "Lead No Bid", "Signature", "Lead Product Unavailable", "Invoice", "Cancel Invoice", "Lead Auto Response", "Repair Authorization" };
        public static List<string> LineItemServiceTypes = new List<string> { "Sales", "Repair" };
        public static List<string> LeadWebsites = new List<string> {
            "direct.parts","eol.parts","eolfinder.com","industrial.net","industrialaccessories.net","industrialcontrols.net","industrialdevices.net","industrialmachineparts.net","industrialmaterials.net","industrialmotorsdrives.net","industrialpressuresystems.net","industrialregulators.net","industrialsensors.net","industrialstock.net"
        };
        // public static List<string> InventoryItemLocationOptions = new List<string> { "Receiving", "Shipped Out", "Main Warehouse (GID)" };
        public static List<string> InventoryItemStatusOptions = new List<string> { "Inbound", "Available", "Committed", "Shipped" };
        public static List<string> InventoryItemConditionOptions = new List<string> { "New", "Used", "Refurbished" };
        public static List<Company> Companies = new List<Company> {
            new Company{ Name="GID Industrial", Addresses = new List<CompanyAddress>{
                new CompanyAddress{
                    Address = new Address{
                        Name = "GID Industrial",
                        Address1 = "1211 Executive Drive East",
                        City = "Richardson",
                        State = "TX",
                        ZipPostalCode = "75081"
                    }
                }
            }}
        };
        public static List<string> CashReceiptTypes = new List<string> { "Credit Card", "Check", "Wire" };

        public static List<ShippingCarrier> ShippingCarriers = new List<ShippingCarrier> {
            new ShippingCarrier{
                Name = "UPS",
                AccountNumberLength = 6,
                TrackingNumberLink = "https://wwwapps.ups.com/tracking/tracking.cgi?tracknum={tracking_number}",
                ShippingMethods = new List<ShippingCarrierShippingMethod>{
                    new ShippingCarrierShippingMethod{ Name = "Ground" },
                    new ShippingCarrierShippingMethod{ Name = "3 Day Select" },
                    new ShippingCarrierShippingMethod{ Name = "2nd Day Air" },
                    new ShippingCarrierShippingMethod{ Name = "Next Day Air Saver" },
                    new ShippingCarrierShippingMethod{ Name = "Next Day Air Early AM" },
                    new ShippingCarrierShippingMethod{ Name = "Standard" },
                    new ShippingCarrierShippingMethod{ Name = "Worldwide Expedited" },
                    new ShippingCarrierShippingMethod{ Name = "Worldwide Saver" },
                    new ShippingCarrierShippingMethod{ Name = "Worldwide Express" },
                    new ShippingCarrierShippingMethod{ Name = "Worldwide Express Plus" },
                }
            },
            new ShippingCarrier{
                Name = "FedEx",
                AccountNumberLength = 9,
                TrackingNumberLink = "https://www.fedex.com/apps/fedextrack/index.html?tracknumbers={tracking_number}&cntry_code=us",
                ShippingMethods = new List<ShippingCarrierShippingMethod>{
                    new ShippingCarrierShippingMethod{ Name = "Ground" },
                    new ShippingCarrierShippingMethod{ Name = "Express Saver" },
                    new ShippingCarrierShippingMethod{ Name = "2Day" },
                    new ShippingCarrierShippingMethod{ Name = "Standard Overnight" },
                    new ShippingCarrierShippingMethod{ Name = "Priority Overnight" },
                    new ShippingCarrierShippingMethod{ Name = "First Overnight" },
                    new ShippingCarrierShippingMethod{ Name = "International Economy" },
                    new ShippingCarrierShippingMethod{ Name = "International Priority" },
                    new ShippingCarrierShippingMethod{ Name = "International First" },
                }
            },
            new ShippingCarrier{
                Name = "DHL",
                AccountNumberLength = 9,
                TrackingNumberLink = "http://www.dhl.com/en/express/tracking.html?AWB={tracking_number}&brand=DHL",
                ShippingMethods = new List<ShippingCarrierShippingMethod>{
                    new ShippingCarrierShippingMethod{ Name = "Express" },
                }
            }
        };

        public static List<CurrencyOption> CurrencyOptions = new List<CurrencyOption>{
            new CurrencyOption{ Id=1, Value = "USD", Symbol = "$", Locked = true },
            new CurrencyOption{ Id=2, Value = "EUR", Symbol = "€", Locked = true },
        };

        public static ReadOnlyDictionary<string, string> CountryNames = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>()
            {
                { "AF", "Afghanistan" },
                { "AL", "Albania" },
                { "DZ", "Algeria" },
                { "AS", "American Samoa" },
                { "AD", "Andorra" },
                { "AO", "Angola" },
                { "AI", "Anguilla" },
                { "AQ", "Antarctica" },
                { "AG", "Antigua and Barbuda" },
                { "AR", "Argentina" },
                { "AM", "Armenia" },
                { "AW", "Aruba" },
                { "AU", "Australia" },
                { "AT", "Austria" },
                { "AZ", "Azerbaijan" },
                { "BS", "Bahamas" },
                { "BH", "Bahrain" },
                { "BD", "Bangladesh" },
                { "BB", "Barbados" },
                { "BY", "Belarus" },
                { "BE", "Belgium" },
                { "BZ", "Belize" },
                { "BJ", "Benin" },
                { "BM", "Bermuda" },
                { "BT", "Bhutan" },
                { "BO", "Bolivia, Plurinational State of" },
                { "BQ", "Bonaire, Sint Eustatius and Saba" },
                { "BA", "Bosnia and Herzegovina" },
                { "BW", "Botswana" },
                { "BV", "Bouvet Island" },
                { "BR", "Brazil" },
                { "IO", "British Indian Ocean Territory" },
                { "BN", "Brunei Darussalam" },
                { "BG", "Bulgaria" },
                { "BF", "Burkina Faso" },
                { "BI", "Burundi" },
                { "KH", "Cambodia" },
                { "CM", "Cameroon" },
                { "CA", "Canada" },
                { "CV", "Cape Verde" },
                { "KY", "Cayman Islands" },
                { "CF", "Central African Republic" },
                { "TD", "Chad" },
                { "CL", "Chile" },
                { "CN", "China" },
                { "CX", "Christmas Island" },
                { "CC", "Cocos (Keeling) Islands" },
                { "CO", "Colombia" },
                { "KM", "Comoros" },
                { "CG", "Congo" },
                { "CD", "Congo, the Democratic Republic of the" },
                { "CK", "Cook Islands" },
                { "CR", "Costa Rica" },
                { "HR", "Croatia" },
                { "CU", "Cuba" },
                { "CW", "Curaçao" },
                { "CY", "Cyprus" },
                { "CZ", "Czech Republic" },
                { "CI", "Côte d'Ivoire" },
                { "DK", "Denmark" },
                { "DJ", "Djibouti" },
                { "DM", "Dominica" },
                { "DO", "Dominican Republic" },
                { "EC", "Ecuador" },
                { "EG", "Egypt" },
                { "SV", "El Salvador" },
                { "GQ", "Equatorial Guinea" },
                { "ER", "Eritrea" },
                { "EE", "Estonia" },
                { "ET", "Ethiopia" },
                { "FK", "Falkland Islands (Malvinas)" },
                { "FO", "Faroe Islands" },
                { "FJ", "Fiji" },
                { "FI", "Finland" },
                { "FR", "France" },
                { "GF", "French Guiana" },
                { "PF", "French Polynesia" },
                { "TF", "French Southern Territories" },
                { "GA", "Gabon" },
                { "GM", "Gambia" },
                { "GE", "Georgia" },
                { "DE", "Germany" },
                { "GH", "Ghana" },
                { "GI", "Gibraltar" },
                { "GR", "Greece" },
                { "GL", "Greenland" },
                { "GD", "Grenada" },
                { "GP", "Guadeloupe" },
                { "GU", "Guam" },
                { "GT", "Guatemala" },
                { "GG", "Guernsey" },
                { "GN", "Guinea" },
                { "GW", "Guinea-Bissau" },
                { "GY", "Guyana" },
                { "HT", "Haiti" },
                { "HM", "Heard Island and McDonald Islands" },
                { "VA", "Holy See (Vatican City State)" },
                { "HN", "Honduras" },
                { "HK", "Hong Kong" },
                { "HU", "Hungary" },
                { "IS", "Iceland" },
                { "IN", "India" },
                { "ID", "Indonesia" },
                { "IR", "Iran, Islamic Republic of" },
                { "IQ", "Iraq" },
                { "IE", "Ireland" },
                { "IM", "Isle of Man" },
                { "IL", "Israel" },
                { "IT", "Italy" },
                { "JM", "Jamaica" },
                { "JP", "Japan" },
                { "JE", "Jersey" },
                { "JO", "Jordan" },
                { "KZ", "Kazakhstan" },
                { "KE", "Kenya" },
                { "KI", "Kiribati" },
                { "KP", "Korea, Democratic People's Republic of" },
                { "KR", "Korea, Republic of" },
                { "KW", "Kuwait" },
                { "KG", "Kyrgyzstan" },
                { "LA", "Lao People's Democratic Republic" },
                { "LV", "Latvia" },
                { "LB", "Lebanon" },
                { "LS", "Lesotho" },
                { "LR", "Liberia" },
                { "LY", "Libya" },
                { "LI", "Liechtenstein" },
                { "LT", "Lithuania" },
                { "LU", "Luxembourg" },
                { "MO", "Macao" },
                { "MK", "Macedonia, the Former Yugoslav Republic of" },
                { "MG", "Madagascar" },
                { "MW", "Malawi" },
                { "MY", "Malaysia" },
                { "MV", "Maldives" },
                { "ML", "Mali" },
                { "MT", "Malta" },
                { "MH", "Marshall Islands" },
                { "MQ", "Martinique" },
                { "MR", "Mauritania" },
                { "MU", "Mauritius" },
                { "YT", "Mayotte" },
                { "MX", "Mexico" },
                { "FM", "Micronesia, Federated States of" },
                { "MD", "Moldova, Republic of" },
                { "MC", "Monaco" },
                { "MN", "Mongolia" },
                { "ME", "Montenegro" },
                { "MS", "Montserrat" },
                { "MA", "Morocco" },
                { "MZ", "Mozambique" },
                { "MM", "Myanmar" },
                { "NA", "Namibia" },
                { "NR", "Nauru" },
                { "NP", "Nepal" },
                { "NL", "Netherlands" },
                { "NC", "New Caledonia" },
                { "NZ", "New Zealand" },
                { "NI", "Nicaragua" },
                { "NE", "Niger" },
                { "NG", "Nigeria" },
                { "NU", "Niue" },
                { "NF", "Norfolk Island" },
                { "MP", "Northern Mariana Islands" },
                { "NO", "Norway" },
                { "OM", "Oman" },
                { "PK", "Pakistan" },
                { "PW", "Palau" },
                { "PS", "Palestine, State of" },
                { "PA", "Panama" },
                { "PG", "Papua New Guinea" },
                { "PY", "Paraguay" },
                { "PE", "Peru" },
                { "PH", "Philippines" },
                { "PN", "Pitcairn" },
                { "PL", "Poland" },
                { "PT", "Portugal" },
                { "PR", "Puerto Rico" },
                { "QA", "Qatar" },
                { "RO", "Romania" },
                { "RU", "Russian Federation" },
                { "RW", "Rwanda" },
                { "RE", "Réunion" },
                { "BL", "Saint Barthélemy" },
                { "SH", "Saint Helena, Ascension and Tristan da Cunha" },
                { "KN", "Saint Kitts and Nevis" },
                { "LC", "Saint Lucia" },
                { "MF", "Saint Martin (French part)" },
                { "PM", "Saint Pierre and Miquelon" },
                { "VC", "Saint Vincent and the Grenadines" },
                { "WS", "Samoa" },
                { "SM", "San Marino" },
                { "ST", "Sao Tome and Principe" },
                { "SA", "Saudi Arabia" },
                { "SN", "Senegal" },
                { "RS", "Serbia" },
                { "SC", "Seychelles" },
                { "SL", "Sierra Leone" },
                { "SG", "Singapore" },
                { "SX", "Sint Maarten (Dutch part)" },
                { "SK", "Slovakia" },
                { "SI", "Slovenia" },
                { "SB", "Solomon Islands" },
                { "SO", "Somalia" },
                { "ZA", "South Africa" },
                { "GS", "South Georgia and the South Sandwich Islands" },
                { "SS", "South Sudan" },
                { "ES", "Spain" },
                { "LK", "Sri Lanka" },
                { "SD", "Sudan" },
                { "SR", "Suriname" },
                { "SJ", "Svalbard and Jan Mayen" },
                { "SZ", "Swaziland" },
                { "SE", "Sweden" },
                { "CH", "Switzerland" },
                { "SY", "Syrian Arab Republic" },
                { "TW", "Taiwan, Province of China" },
                { "TJ", "Tajikistan" },
                { "TZ", "Tanzania, United Republic of" },
                { "TH", "Thailand" },
                { "TL", "Timor-Leste" },
                { "TG", "Togo" },
                { "TK", "Tokelau" },
                { "TO", "Tonga" },
                { "TT", "Trinidad and Tobago" },
                { "TN", "Tunisia" },
                { "TR", "Turkey" },
                { "TM", "Turkmenistan" },
                { "TC", "Turks and Caicos Islands" },
                { "TV", "Tuvalu" },
                { "UG", "Uganda" },
                { "UA", "Ukraine" },
                { "AE", "United Arab Emirates" },
                { "GB", "United Kingdom" },
                { "US", "United States" },
                { "UM", "United States Minor Outlying Islands" },
                { "UY", "Uruguay" },
                { "UZ", "Uzbekistan" },
                { "VU", "Vanuatu" },
                { "VE", "Venezuela, Bolivarian Republic of" },
                { "VN", "Viet Nam" },
                { "VG", "Virgin Islands, British" },
                { "VI", "Virgin Islands, U.S." },
                { "WF", "Wallis and Futuna" },
                { "EH", "Western Sahara" },
                { "YE", "Yemen" },
                { "ZM", "Zambia" },
                { "ZW", "Zimbabwe" },
                { "AX", "Åland Islands" }
            });
    }
}
