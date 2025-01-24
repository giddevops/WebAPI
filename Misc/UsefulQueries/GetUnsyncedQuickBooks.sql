-- you need to download all quickbooks invoices first
select 
	QuickBooksInvoice.DocNumber as "QuickBooksInvoice.DocNumber", 
	InvoiceByQuickBooksId.Id as "SyncedGideonInvoice.Id", 
	PossiblyMatchingInvoiceInGideon.Id as "PossiblyMatchingInvoiceInGideon.Id"
from QuickBooksInvoice

left join Invoice as InvoiceByQuickBooksId on InvoiceByQuickBooksId.QuickBooksId = QuickBooksInvoice.Id
left join Invoice as PossiblyMatchingInvoiceInGideon on cast(PossiblyMatchingInvoiceInGideon.Id as nvarchar(max)) = QuickBooksInvoice.DocNumber

where QuickBooksInvoice.TxnDate > '2018-09-01'
and InvoiceByQuickBooksId.QuickBooksId is null

order by "SyncedGideonInvoice.Id" asc