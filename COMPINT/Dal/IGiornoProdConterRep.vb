Public Interface IGiornoProdConterRep
    Function GetNextProdCounter(DataProduzione As DateTime) As String
    Sub InsertIncrementaleGDiProd(IncrementaleGDiProdConter As Int32, DataDiProduzione As DateTime)
End Interface
