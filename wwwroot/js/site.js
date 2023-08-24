function showModalFilmeSerie(id) {
    $('#staticBackdropLabel').html("Baixar no Servidor");
    $('.modal-body').html("<p>O conteúdo é um filme ou série?</p>" +
        "<form id='filmeSerie'>"+
        "<label>"+
            "<input id='radioFilme' type='radio' name='tipo' value='filme' checked>"+
                "Filme"+
        "</label>" +
        "</br>"+
        "<label>" +       
            "<input type='radio' name='tipo' value='serie'>"+
                "Série"+
            "</label>"+
        "</form> ");

    $('#btnPrimary').attr("onclick", "downloadTorrent(" + id +   ")").html("Baixar");
    $('#btnSecondary').html("Cancelar");
}