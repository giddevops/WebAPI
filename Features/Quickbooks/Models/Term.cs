using System;
using System.Collections.Generic;

namespace QuickBooks.Models {
    public class Term {
        public string SyncToken { get; set; }
        public string domain { get; set; }
        public string Name { get; set; }
        public decimal DiscountPercent { get; set; }
        public int? DiscountDays { get; set; }
        public string Type { get; set; }
        public bool sparse { get; set; }
        public bool Active { get; set; }
        public int DueDays { get; set; }
        public string Id { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
    }
    public class TermResponse {
        public Term Term { get; set; }
    }
    public class TermQueryResponseContainer {
        public TermQueryResponse QueryResponse { get; set; }
    }
    public class TermQueryResponse {
        public List<Term> Term { get; set; }
    }
}