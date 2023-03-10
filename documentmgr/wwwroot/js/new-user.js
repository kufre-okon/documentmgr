const MAX_FILE = 20;

function getFileName(e) {
    if (e.target.files[0]) {
        return e.target.files[0].name;
    }
    return null;
}

function isValidFileSize(e) {
    if (e.target.files[0]) {
        return (e.target.files[0].size / (1000 * 1024)) <= MAX_FILE;
    }
    return true;
}

const setUploadStatus = function (totalUploaded, totalFileSize) {
    const statusEle = $('.lbl-progress');
    statusEle.html('Uploading ' + totalUploaded + '%' + " of " + totalFileSize);
}

const toggleUploadBtn = function (isDisabled) {
    const uploadBtn = $('.btn-add-document');
    uploadBtn.prop('disabled', isDisabled);
}

const toggleStatusEle = function (isShown) {
    const statusEle = $('.lbl-progress');
    if (isShown)
        statusEle.show();
    else
        statusEle.hide();
}

const clearFileControl = function () {
    const input = $('#fileupload_ctrl');
    input.val('');
    input.siblings('label').html('Choose file');
}

const progressHandler = function ({ loaded, total }) {  
    let fileLoaded = Math.floor((loaded / total) * 100);
    let fileTotal = Math.floor(total / 1000);
    let totalFileSize;
    if (fileTotal < 1024)
        totalFileSize = (fileTotal).toFixed(2) + " KB";
    else
        totalFileSize = (fileTotal / (1024)).toFixed(2) + " MB";
   
    setUploadStatus(fileLoaded, totalFileSize);
}

const uploadEnd = function () {
    toggleStatusEle(false);
    toggleUploadBtn(false);
    
}

const uploadSuccess = function ({ target }) {  
    if (target.status !== 200)
        return alert(target.responseText || target.statusText || "An error uploading your document");
    const response = JSON.parse(target.responseText);
   
    if (response.status)
        addUploadedFile(response.payload);
    else
        alert(response.message);

    clearFileControl();
}

const uploadFail = function (e) {
    console.log(e);
}

const handleUpload =function handleUpload(file) {
   
    let xhrRequest = new XMLHttpRequest();
    const endpoint = "/user/upload";
    xhrRequest.open("POST", endpoint);
    
    xhrRequest.upload.addEventListener("progress", progressHandler)
    xhrRequest.addEventListener("loadend", uploadEnd)
    xhrRequest.addEventListener("error ", uploadFail)
    xhrRequest.addEventListener('load', uploadSuccess);

    let data = new FormData();
    data.append("file", file);

    toggleStatusEle(true);
    toggleUploadBtn(true);
    xhrRequest.send(data);
}

const addUploadedFile = function (data) {
    const list = $('.lst-documents');
    list.show();
    const child = `<li class="list-group-item pt-2 pb-2">${data.title}${data.extension}</li>`;
    $(child).appendTo(list);

    const docEle = $('[data-tag="documents"]');
    const index = docEle.children('[data-id]').length;
   
    const inputs = `<div data-id="${index}">
        <input data-val="true" id="Documents_${index}__Id" name="Documents[${index}].Id" type="hidden" value="${data.id}">
            <input id="Documents_${index}__Title" name="Documents[${index}].Title" type="hidden" value="${data.title}">
                <input id="Documents_${index}__Extension" name="Documents[${index}].Extension" type="hidden" value="${data.extension}">
                </div>`;
    $(inputs).appendTo(docEle);
}

$(function () {
    $('#fileupload_ctrl').on('change', function (e) {
        if (!isValidFileSize(e))
            return alert('File too large');
        const fileName = getFileName(e);
        $(e.target).siblings('label').html(fileName || 'Choose file');
    })

    $('.btn-add-document').on('click', function () {
        const file = document.getElementById('fileupload_ctrl');
        const selectFile = file.files[0];
        if (!selectFile)
            return alert('Please select a document to upload.');
        handleUpload(selectFile);
    })
})