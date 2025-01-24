using System;
using System.Collections.Generic;

namespace QuickBooks.Models {

    public class IncomeAccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class ExpenseAccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class AssetAccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class QuickBooksMetaData {
        public DateTime? CreateTime { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }

    public class Item {
        public string Name { get; set; }
        public string Sku { get; set; }
        public string ItemCategoryType { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string FullyQualifiedName { get; set; }
        public bool Taxable { get; set; }
        public int UnitPrice { get; set; }
        public string Type { get; set; }
        public IncomeAccountRef IncomeAccountRef { get; set; }
        public string PurchaseDesc { get; set; }
        public int PurchaseCost { get; set; }
        public ExpenseAccountRef ExpenseAccountRef { get; set; }
        public AssetAccountRef AssetAccountRef { get; set; }
        public bool TrackQtyOnHand { get; set; }
        public int QtyOnHand { get; set; }
        public string InvStartDate { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
    }

    public class ItemResponse {
        public Item Item { get; set; }
    }

    public class ItemQueryResponseContainer {
        public ItemQueryResponse QueryResponse { get; set; }
    }
    public class ItemQueryResponse {
        public List<Item> Item { get; set; }
    }
}