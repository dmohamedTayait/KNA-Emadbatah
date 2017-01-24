
function AjaxEndMethod() {
    // change the date language
    $.datetimepicker.setLocale('ar');
    // date picker
    $(".Calender").datetimepicker({
        timepicker: false,
        defaultDate: new Date(),
        format: 'm/d/Y'
    });
    // time picker
    $(".timePicker").datetimepicker({
        datepicker: false,
        defaultDate: new Date(),
        format: 'H:i'
    });
}

$(document).ready(function () {

    AjaxEndMethod();
    del_init();
    att_init();
    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }


    $("a.aPopupAddSessionComm").click(function (e) {
        $(".txtAddDetails").val('');
        $(".txtCreatedAt").val('');
        $(".lblCommName").html($(this).attr("commname"));
        $(".txtCommName").val($(this).attr("commname"));
        $(".txtSID").val($(this).attr("sid"));
        $(".txtCommID").val($(this).attr("commid"));
        $(".popupoverlay").show();
        $(".popupAddSessionComm").show();
        AjaxEndMethod();
        e.preventDefault();
    });


    $(".btnAddSessionComm").click(function (e) {
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'CommitteeHandler.ashx',
            data: {
                funcname: 'AddSessionCommittee',
                commname: $(".txtCommName").val(),
                scommdetails: $(".txtAddDetails").val(),
                scommcreatedat: $(".txtCreatedAt").val(),
                sid: $(".txtSID").val(),
                commid: $(".txtCommID").val()
            },
            dataType: 'json',
            success: function (response) {
                if (response != "0") {

                    var divContent = " <div id=\"div_" + response + "\" class=\"grid_20 sessioncomm\">" +
                                     " <div style=\"margin-top:10px\"></div>" +
                                       "<div class=\"row\">" +
                                            "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>اسم اللجنة :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtCommName").val() + "</div>" +
                                            " <div class=\"grid_4 h2\"><a href='javascript:void(0)' class=\"aPopupDelSessionComm\" scommid=\"" + response + "\" >حذف</a></div>" +
                                            " <div class=\"grid_4 h2\"><a href='javascript:void(0)' class=\"aPopupTakeSessionCommAtt\" scommid=\"" + response + "\" commid=\"" + $(".txtCommID").val() + "\" sid=\"" + $(".txtSID").val() + "\" >أخذ الغياب</a></div>" +
                                            " <div class=\"clear\"></div>" +
                                       " </div>" +
                                        "<div class=\"row\">" +
                                           "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>تاريخ الانشاء :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtCreatedAt").val() + "</div>" +
                                             "<div class=\"clear\"></div>" +
                                        "</div>" +
                                        "<div class=\"row\">" +
                                            "<div class=\"grid_3 h2\" style=\"color:#0134cb\"><span>التفاصيل :</span></div>" +
                                            "<div class=\"grid_6 h2\">" + $(".txtAddDetails").val() + "</div>" +
                                            "<div class=\"clear\"></div>" +
                                        "</div>" +
                                     "</div>  <div class=\"clear\"></div>";
                    var x = "divContent" + $(".txtCommID").val().trim();
                    $("#" + x).append(divContent).show();
                    $(".popupoverlay").hide();
                    $(".popupAddSessionComm").hide();
                    del_init();
                    att_init();
                    e.preventDefault();
                }
            },
            error: function (response) {
                alert("Error");
            }
        });
    }); // End Add

    function del_init() {
        $(".aPopupDelSessionComm").on("click", function (e) {
            var scommid = $(this).attr("scommid");
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'CommitteeHandler.ashx',
                data: {
                    funcname: 'DeleteSessionCommittee',
                    scommid: scommid
                },
                dataType: 'json',
                success: function (response) {
                    if (response != "0") {
                        var x = "div_" + scommid;
                        console.log(x);
                        $("#" + x).remove();
                    }
                },
                error: function (response) {
                    alert("Error");
                }
            });
        });
    }

    function att_init() {
        $("a.aPopupTakeSessionCommAtt").click(function (e) {
            jQuery.ajax({
                cache: false,
                type: 'post',
                url: 'CommitteeHandler.ashx',
                data: {
                    funcname: 'GetSessionCommAtt',
                    sid: $(this).attr("sid"),
                    commid: $(this).attr("commid")
                },
                dataType: 'json',
                success: function (response) {

                    tbl = $("#tbl_Att_Status");
                    tr = "<tr id=\"trAttID\">" +
                "<td style=\"padding-right: 10px;\"><span>AttName</span></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='2' class='radio_list'/></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='3' class='radio_list'/></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='3' class='radio_list'/> </td>" +
                "</tr>";
                    var tmpTr = "";
                    for (i = 0; i < response.length; i++) {
                        tmpTr = tr.replace("AttID", response[i].ID).replace("AttName", response[i].AttendantTitle + " " + response[i].LongName);
                        tbl.append(tmpTr);
                    }

                    $(".popupoverlay").show();
                    $(".popupAttendant").show();
                    e.preventDefault();
                },
                error: function (response) { }
            });
            e.preventDefault();
        });
    }

    $(".btnAddSessionCommAtt").click(function (e) {
        jQuery.ajax({
            cache: false,
            type: 'post',
            url: 'CommitteeHandler.ashx',
            data: {
                funcname: 'GetSessionCommAtt',
                sid: $(this).attr("sid"),
                commid: $(this).attr("commid")
            },
            dataType: 'json',
            success: function (response) {

                tbl = $("#tbl_Att_Status");
                tr = "<tr id=\"trAttID\">" +
                "<td style=\"padding-right: 10px;\"><span>AttName</span></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='2' class='radio_list' rd2/></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='3' class='radio_list' rd3/></td>" +
                "<td> <input type=\"radio\" name=\"rdAttID\" value='3' class='radio_list' rd4/> </td>" +
                "</tr>";
                var tmpTr = "";
                for (i = 0; i < response.length; i++) {
                    tmpTr = tr.replace("AttID", response[i].ID).replace("AttName", response[i].AttendantTitle + " " + response[i].LongName);
                    if (response[i].status == "2")
                        tmpTr = tmpTr.replace("rd2", "checked").replace("rd3", "").replace("rd4", "")
                    if (response[i].status == "3")
                        tmpTr = tmpTr.replace("rd3", "checked").replace("rd2", "").replace("rd4", "")
                    if (response[i].status == "4")
                        tmpTr = tmpTr.replace("rd4", "checked").replace("rd2", "").replace("rd3", "")
                    tbl.append(tmpTr);
                }

                $(".popupoverlay").show();
                $(".popupAttendant").show();
                e.preventDefault();
            },
            error: function (response) { }
        });
        e.preventDefault();
    });



    // on click on the arrows
    $('.hoverArrow').click(function () {
        var curent = $(this);
        $('.hoverArrow').not(curent).removeClass('up').addClass('down');
        $('.sessioncomms').hide();
        $(this).toggleClass('down up');
        if (curent.hasClass("up")) {
            $('#divContent' + curent.attr("commid").trim()).show();
        }
    })

    $(".close_btn").click(function () {
        $(".popupoverlay").hide();
        $(".reviewpopup_cont").hide();
        $(".reviewpopup_cont-st1").hide();
    });


});
