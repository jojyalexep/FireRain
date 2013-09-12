$(document).ready(function (e) {
    $(".btnDelete").click(function (e) {
        e.preventDefault();
        var id = $(this).parent("td").find(".hdnCampId").val();
        $("#dialogDelete").dialog({
            buttons: {
                "Confirm": function () {
                    $.get(GetUrl("Delete"), {
                        id: id,
                    }, function (data) {
                        if (data) {
                            document.location.href = GetUrl("?messageId=3");
                        }
                        else {
                            alert("Error in deleting the campaign. Please try again!!!")
                            $(this).dialog("close");
                        }
                    });
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        $("#dialogDelete").dialog("open");
    });

    $(".btnReject").click(function (e) {
        e.preventDefault();
        var id = $(this).parent("td").find(".hdnCampId").val();
        $("#dialogReject").dialog({
            buttons: {
                "Confirm": function () {
                    $.get(GetUrl("Reject"), {
                        id: id,
                    }, function (data) {
                        if (data) {
                            document.location.href = GetUrl("Approval?messageId=3");
                        }
                        else {
                            alert("Error in rejecting the campaign. Please try again!!!")
                            $(this).dialog("close");
                        }
                    });
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        $("#dialogDelete").dialog("open");
    });

    $(".btnReSchedule").click(function (e) {
        e.preventDefault();
        Schedule(this);

    });

    $(".btnScheduleNew").click(function (e) {
        e.preventDefault();
        Schedule(this);

    });

    $(".btnApprove").click(function (e) {
        e.preventDefault();
        var id = $(this).parent("td").find(".hdnCampId").val();
        var id = $(this).parent("td").find(".hdnCampId").val();
        $("#dialogApprove").dialog({
            buttons: {
                "Confirm": function () {
                    $.get(GetUrl("Approve"), {
                        id: id,
                    }, function (data) {
                        if (data) {
                            document.location.href = GetUrl("Approval?messageId=7");
                        }
                        else {
                            alert("Error in approving the campaign. Please try again!!!")
                            $(this).dialog("close");
                        }
                    });
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

    });

    $(".btnStop").click(function (e) {
        e.preventDefault();
        var id = $(this).parent("td").find(".hdnCampId").val();
        var id = $(this).parent("td").find(".hdnCampId").val();
        $("#dialogStop").dialog({
            buttons: {
                "Confirm": function () {
                    $.get(GetUrl("Approve"), {
                        id: id,
                    }, function (data) {
                        if (data) {
                            document.location.href = GetUrl("?messageId=5");
                        }
                        else {
                            alert("Error in stopping the campaign. Please try again!!!")
                            $(this).dialog("close");
                        }
                    });
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });
    });


    $(".btnTestMessage").click(function (e) {
        e.preventDefault();
        var id = $(this).parent("td").find(".hdnCampId").val();
        var msgType = ($(this).parent("td").find(".hdnTypeId").val() == 1) ? "Email Id" : "Mobile Number";
        $("#lblMsgType").html(msgType);

        $("#dialogTestMsg").dialog({
            width: 525,
            height: 250,
            buttons: {
                "Send": function () {
                    $.get(GetUrl("SendTestMsg"), {
                        id: id,
                        recepient: $("#txtRecepient").val()
                    }, function (data) {
                        if (data) {
                            document.location.href = GetUrl("?messageId=6");
                        }
                        else {
                            alert("Error in sending test message. Please try again!!!")
                            $(this).dialog("close");
                        }
                    });
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });
        $("#dialogTestMsg").dialog("open");
    });
});

function Schedule(element) {

    $(".datepicker").datepicker({
        minDate: 0,
        onSelect: function (dateText) {
            var todayDate = $.datepicker.formatDate('mm/dd/yy', new Date());
            if (dateText == todayDate) {

                var hour = new Date().getHours() + 1;
                $('.timepicker').timepicker('option', { 'minTime': hour + ':00', 'maxTime': '21:00' });
            }
            else {
                $('.timepicker').timepicker('option', { 'minTime': '07:00', 'maxTime': '23:50' });
            }
        }
    });
    $('.timepicker').timepicker({ 'scrollDefaultNow': true, 'minTime': '07:00', 'maxTime': '21:00' });
    var id = $(element).parent("td").find(".hdnCampId").val();
    $("#dialogSchedule").dialog({
        width: 525,
        height: 250,
        buttons: {
            "Set": function () {
                $.get(GetUrl("Schedule"), {
                    id: id,
                    scheduledDate: $(this).find(".datepicker").val(),
                    scheduledTime: $(this).find(".timepicker").val()
                }, function (data) {
                    if (data) {
                        document.location.href = GetUrl("?messageId=4");
                    }
                    else {
                        alert("Error in scheduling the campaign. Please try again!!!")
                        $(this).dialog("close");
                    }
                });
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });
    $("#dialogSchedule").dialog("open");
}

function GetUrl(action){

    return $("#hdnController").val() + action;
}
