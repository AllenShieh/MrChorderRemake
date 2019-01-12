function upload() {
    //alert("upload");
    var file_data = $('#file').get(0).files;
    var formData = new FormData();
    if (file_data.length > 0) {
        formData.append('UploadedFiles', file_data[0], file_data[0].name);
        //alert(file_data[0].name);
    }
    $.ajax({
        type: "POST",
        url: "/Chord/GetFile",
        contentType: false,
        data: formData,
        dataType: 'text',
        encode: true,
        async: false,
        processData: false,
        cache: false,
        success: function (res) {
            NProgress.start();
            displayPDF();
            //alert(res);
        },
        error: function (request, s, error) {
            alert(request.responseText);
        }
    });
}

function getFile() {
    //alert("getFile");
    document.getElementById("file").click();
}

function submitFile() {
    //alert("submitFile");
    upload();
}

function displayPDF() {
    $.ajax({
        type: "GET",
        url: "/Chord/SendFile",
        contentType: false,
        dataType: 'text',
        encode: true,
        async: true,
        processData: false,
        cache: false,
        success: function (res) {
            NProgress.done();
            document.getElementById("result").innerHTML = "<iframe src='/Generate/AnalyseResult.pdf'></iframe>";
            //alert(res);
        },
        error: function (request, s, error) {
            alert(request.responseText);
        }
    });
}