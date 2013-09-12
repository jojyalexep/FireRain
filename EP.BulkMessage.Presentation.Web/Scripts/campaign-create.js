function GetUrl(action) {
    return $("#hdnController").val() + action;
}

function GetWithBaseUrl(relUrl) {
    return $("#hdnBase").val() + relUrl;
}



function SelectCampaignType(type) {
    $("#divCreateDetail").load(GetUrl(type), function (e) {

        $("#divCampaignType").hide('slow');
    });
}


function UploadFile(element) {
    $("#spanFileName").html($("#divUploadLoad").html());
    var options = {
        //beforeSubmit: beforeSubmit,
        target: '#liHeader',   // target element to update
        success: function (e) {
            //EnableDragHeaders(element.files[0].name);
            $(".itemHeader").draggable({
                helper: 'clone'
            });
            EnableDragHeaders(element.value);  // post-submit callback

        },
        error: function() { alert('Error in uploading file. Check file and try again.'); }

    };
    $("#formUpload").ajaxSubmit(options);
}


function EnableDragHeaders(fileName) {

    var options = {
        accept: ".itemHeader",
        drop: function (ev, ui) {
            insertAtCaret($(this).get(0), "{" + ui.draggable.eq(0).text() + "}");
        }
    };
    var contentOptions = {
        accept: ".itemHeader",
        drop: function (ev, ui) {
            if ($("#hdnType").val() == "Email") {
                tinymce.activeEditor.execCommand('mceInsertContent', false, "{" + ui.draggable.eq(0).text() + "}");
            }
            else {
                insertAtCaret($(this).find("textarea").get(0), "{" + ui.draggable.eq(0).text() + "}");
            }
        }
    };

    $("#liContent").droppable(contentOptions);
    $("input").droppable(options);

    $("#hdnFileName").val(fileName);
    $("#spanFileName").html(fileName);
}


function ShowReport() {
    $("#divReport").show();
    $("#divCreateDetail").hide('slow');

}


function insertAtCaret(area, text) {
    var scrollPos = area.scrollTop;
    var strPos = 0;
    var br = ((area.selectionStart || area.selectionStart == '0') ? "ff" : (document.selection ? "ie" : false));
    if (br == "ie") {
        area.focus();
        var range = document.selection.createRange(); alert(area);
        range.moveStart('character', -(area.value.length));
        strPos = range.text.length;
    } else if (br == "ff")
        strPos = area.selectionStart;
    var front = (area.value).substring(0, strPos);
    var back = (area.value).substring(strPos, area.value.length);
    area.value = front + text + back;
    strPos = strPos + text.length;
    if (br == "ie") {
        area.focus();
        var range = document.selection.createRange();
        range.moveStart('character', -(area.value.length));
        range.moveStart('character', strPos);
        range.moveEnd('character', 0);
        range.select();
    } else if (br == "ff") {
        area.selectionStart = strPos;
        area.selectionEnd = strPos;
        area.focus();
    }
    area.scrollTop = scrollPos;
}