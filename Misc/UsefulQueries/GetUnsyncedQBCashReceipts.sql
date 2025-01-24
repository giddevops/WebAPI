select 
	QuickBooksPayment.Id as "QuickBooksPayment.Id", 
	QuickBooksPayment.TotalAmt as "QuickBooksPayment.TotalAmt", 
	CashReceipt.Id as "CashReceipt.Id",
	QuickBooksPayment.TxnDate as "QuickBooksTransactionDate", 
	CONVERT(date,PossiblyMatchingGideonCashReceipt.DateReceived) as "PossiblyMatchingGideonCashReceipt.DateReceived",
	PossiblyMatchingGideonCashReceipt.Id as "PossiblyMatchingGideonCashReceipt.Id"
from QuickBooksPayment 

left join CashReceipt on CashReceipt.QuickBooksId = QuickBooksPayment.Id
full outer join CashReceipt as PossiblyMatchingGideonCashReceipt on PossiblyMatchingGideonCashReceipt.Amount = QuickBooksPayment.TotalAmt

where TxnDate > '2018-09-01'

order by CashReceipt.Id asc, QuickBooksPayment.Id asc