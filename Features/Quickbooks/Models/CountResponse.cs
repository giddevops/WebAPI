using System;

namespace QuickBooks.Models {
    public class CountResponse {
        public QuickBooksCountQueryResposnse QueryResponse { get; set; }
        public DateTime? Time { get; set; }
    }
    public class QuickBooksCountQueryResposnse {
        public int totalCount { get; set; }
    }
}