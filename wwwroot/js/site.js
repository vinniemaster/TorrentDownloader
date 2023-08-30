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
};

var loadingToF = false;

function showHideLoading(id) {
    loadingToF = !loadingToF;
    if (loadingToF) {
        $(id).html("<img src='/img/loading.gif' style='position: relative; left: 50%; top: 50%; width: 50px; padding-top: 20px;  transform: translate(-50%, -50%);' alt='Aguarde...''>");
    }
    else {
        $(id).html("");
    }
}

function ConvertSizeFile(number) {
    var retorno = "";
    if (number.length >= 13 && number.length < 16) {
        retorno = ((((number / 1000) / 1000) / 1000) / 1000);
        retorno = retorno.toFixed(2);
        retorno = retorno + " TB";
    }
    else if (number.length >= 10 && number.length < 13) {
        retorno = (((number / 1000) / 1000) / 1000);
        retorno = retorno.toFixed(2);
        retorno = retorno + " GB";
    }
    else if (number.length >= 7 && number.length < 10) {
        retorno = ((number / 1000) / 1000);
        retorno = retorno.toFixed(2);
        retorno = retorno + " MB";
    }
    else if (number.length >= 4 && number.length < 7) {
        retorno = number / 1000;
        retorno = retorno.toFixed(2);
        retorno = retorno + " KB";
    }
    else if (number.length < 4) {
        retorno = number + " bytes";
    }

    return retorno;
}