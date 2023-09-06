let downloadId = "";




function showModalFilmeSerie(id) {
    $('#staticBackdropLabel').html("Baixar no Servidor");
    $('.modal-body').html("<p>O conteúdo é um filme ou série?</p>" +
        "<form id='filmeSerie'>"+
        "<label>"+
            "<input id='radioFilme' type='radio' name='tipo' value='filme' checked>"+
                "Filme"+
        "</label>" +
        "</br>"+
        "<label id='radioSerie'>" +       
            "<input  type='radio' name='tipo' value='serie'>"+
                "Série"+
        "</label>" +
        "</form> " +
        "<div id='SelectSerie'></div>");

    $('#btnPrimary').attr("onclick", "downloadTorrent(" + id + ")").html("Baixar").prop('disabled', false);
    $('#btnSecondary').html("Cancelar");

    downloadId = "downloadTorrent(" + id;
    
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

function pauseDownload(index) {
    $.ajax(
        {
            type: 'POST',
            url: '/Home/PauseDownload?index=' + index,
        });
}

function retomarDownload(index) {
    $.ajax(
        {
            type: 'POST',
            url: '/Home/RetomarDownload?index=' + index,
        });
}


function pararDownload(index) {
    $.ajax(
        {
            type: 'POST',
            url: '/Home/PararDownload?index=' + index,
        });
}