Public Interface ILottoRep
    Function GetLottoProdottoFinito(codditt As String, ar_codart As String, alo_lottox As String) As LottoDto
    Function GetNextLottoNumber(codditt As String) As Integer
    Function IsArtConfForLotto(codditt As String, ar_codart As String) As Boolean
    Sub CreaLotto(objLottoDto As LottoDto)
End Interface
