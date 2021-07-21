jQuery(function ($) {

    $("#StartDate").change(function () {
        $('#LeaveStartDate').val($('#StartDate').val());
    });

    $("#EndDate").change(function () {
        $('#LeaveEndDate').val($('#EndDate').val());
    });

    $('#viewleave-wizard-container').ace_wizard();

    $("#LeaveDaysApplied").keyup(function () {
        //Get Leave Quantity And ReturnDate
        if ($('#LeaveStartDate').val() !== null && $('#LeaveStartDate').val() !== '') {
            jQuery.ajax({
                url: '/Leaves/LeaveEndDateAndReturnDate',
                type: "POST",
                data: '{Code:"' + $('#LeaveType').val() + '",StartDate:"' + $('#LeaveStartDate').val() + '", LeaveDaysApplied:"' + $('#LeaveDaysApplied').val() + '"}',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (response) {

                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);

                        //set fields values
                        $('#EndDate').val(data.LeaveEndDate);
                        $('#LeaveEndDate').val(data.LeaveEndDate);
                        $('#ReturnDate').val(data.ReturnDate);
                    }
                }
            });
        }
    });


    //autofill leavetypes on LeaveType

    //$.ajax({
    //    url: '/Leaves/ListLeaveTypes',

    //    type: "POST",
    //    dataType: "json",
    //    contentType: "application/json; charset=utf-8",
    //    success: function (response) {
    //        if (response != null) {

    //            var data = $.parseJSON(response);

    //            $.each(data, function (i, item) {
    //                $("#LeaveType").append($('<option></option>').attr("value", item.Code).text(item.Description));
    //            });

    //        }
    //    },
    //    error: function (e) {
    //        console.log(e.responseText);
    //    }
    //});

    //get leave selected details
    $("#LeaveType").change(function () {
      
        $('#LeaveDaysEntitled').val('');
        $('#LeaveDaysTaken').val('');
        $('#LeaveBalance').val('');
        $('#LeaveAccruedDays').val('');
        $('#LeaveOpeningBalance').val('');

        jQuery.ajax({
            url: '/Leaves/LeaveTypeDetails',
            type: "POST",
            data: '{Code:"' + $('#LeaveType').val() + '" }',
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (response) {

                if (response != null ) {
                    //console.log(JSON.stringify(response)); //it comes out to be string 

                    //we need to parse it to JSON
                    var data = $.parseJSON(response);

                    //set fields values
                    $('#LeaveDaysEntitled').val(data.LeaveDaysEntitled);
                    $('#LeaveDaysTaken').val(data.LeaveDaysTaken);
                    $('#LeaveBalance').val(data.RemainingDays);
                    $('#LeaveAccruedDays').val(data.AccruedDays);
                    $('#LeaveOpeningBalance').val(data.OpeningBalance);
                }
            }
        });
    });



    $('[data-rel=tooltip]').tooltip();

    $('.select2').css('width', '400px').select2({ allowClear: true })
        .on('change', function () {
            $(this).closest('form').validate().element($(this));
        });


    var $validation = true;
    $('#fuelux-wizard-container')
        .ace_wizard({
            //step: 2 //optional argument. wizard will jump to step "2" at first
            //buttons: '.wizard-actions:eq(0)'
        })
        .on('actionclicked.fu.wizard', function (e, info) {
            if (info.step == 1 && $validation) {
                if (!$('#leaveselection-form').valid()) e.preventDefault();
                //save step 1
                var valdata = $("#leaveselection-form").serialize();

                jQuery.ajax({
                    url: '/Leaves/SaveSelection',
                    type: "POST",
                    data: valdata,
                    dataType: "json",
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    success: function (response) {
                        if (response != null) {
                            console.log(JSON.stringify(response)); //it comes out to be string 

                            //we need to parse it to JSON
                            var data = $.parseJSON(response);

                            if (data.Status == "000") {
                                $.gritter.add({
                                    title: 'Leave Notification',
                                    text: data.Message,
                                    class_name: 'gritter-info'
                                });
                            } else {
                                $.gritter.add({
                                    title: 'Leave Notification',
                                    text: data.Message,
                                    class_name: 'gritter-error'
                                });
                            }
                        }
                    },
                    error: function (e) {
                        console.log(e.responseText);
                    }
                });
            }
            if (info.step == 2 && $validation) {
                if (!$('#leavedaysselection-form').valid()) e.preventDefault();

                //save step 2
                var valdata2 = $("#leavedaysselection-form").serialize();

                jQuery.ajax({
                    url: '/Leaves/SaveLeaveSelection',
                    type: "POST",
                    data: valdata2,
                    dataType: "json",
                    contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                    success: function (response) {
                        if (response != null) {
                            //console.log(JSON.stringify(response)); //it comes out to be string 

                            //we need to parse it to JSON

                            var data = jQuery.parseJSON(response);                          

                            if (data.Status == "000") {
                                $.gritter.add({
                                    title: 'Leave Notification',
                                    text: data.Message,
                                    class_name: 'gritter-info'
                                });
                            } else {
                                $.gritter.add({
                                    title: 'Leave Notification',
                                    text: data.Message,
                                    class_name: 'gritter-error'
                                });
                            }
                        }
                    },
                    error: function (e) {
                        console.log(e.responseText);
                    }
                });
            }
            if (info.step == 3 && $validation) {
                if (!$('#leaveattachments-form').valid()) e.preventDefault();

                //Attachments
                var files = jQuery("#LeaveAttachments").get(0).files;
                var fileData = new FormData();

                for (var i = 0; i < files.length; i++) {
                    fileData.append("LeaveAttachments", files[i]);
                }

                jQuery.ajax({
                    type: "POST",
                    url: "/Leaves/SaveLeaveAttachments",
                    dataType: "json",
                    contentType: false, // Not to set any content header
                    processData: false, // Not to process data
                    data: fileData,
                    success: function (result, status, xhr) {
                        //  alert(result);

                        $.gritter.add({
                            title: 'Action Notification',
                            text: result,
                            class_name: 'gritter-info'
                        });
                       
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        })
        //.on('changed.fu.wizard', function() {
        //})
        .on('finished.fu.wizard', function (e) {
            //send application for approval here
            jQuery.ajax({
                url: '/Leaves/SubmitForApproval',
                type: "POST",
                  data: '{DocumentNo:"Leave001" }',
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (response) {

                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);

                        if (data.Status == "000") {
                            $.gritter.add({
                                title: 'Approval Notification',
                                text: data.Message,
                                class_name: 'gritter-success gritter-center'
                            });
                            var delay = 5000;
                            var url = '/Leaves/Index'
                            setTimeout(function () { window.location = url; }, delay);

                        } else {
                            $.gritter.add({
                                title: 'Approval Notification',
                                text: data.Message,
                                class_name: 'gritter-error gritter-center'
                            });
                        }
                    }
                }
            });
        }).on('stepclick.fu.wizard', function (e) {
            //e.preventDefault();//this will prevent clicking and selecting steps
        });


    $('#leaveselection-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            LeaveType: {
                required: true,
            },
            LeaveDaysEntitled: {
                required: true,
            },
            LeaveDaysTaken: {
                required: true,
            },
            LeaveBalance: {
                required: true
            },
            LeaveAccruedDays: {
                required: true
            },
            LeaveOpeningBalance: {
                required: true
            }
        },

        messages: {            
            LeaveType: "Please choose leave type"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });
    $('#leavedaysselection-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            StartDate: {
                required: true
            },
            LeaveDates: {
                required: true
            },
            LeaveDaysApplied: {
                required: true
            },
            LeaveStartDate: {
                required: true
            },
            LeaveEndDate: {
                required: true
            },
            ReturnDate: {
                required: true
            }
        },

        messages: {
            SelectionType: "Please choose selection type"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });

    $('#leaveattachments-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            LeaveAttachments: {
                required: false,
            },
            LeaveComment: {
                required: true,
            }
        },

        messages: {
            LeaveAttachments: "Please choose an attachment"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });



    $('#modal-wizard-container').ace_wizard();
    $('#modal-wizard .wizard-actions .btn[data-dismiss=modal]').removeAttr('disabled');


    /**
    $('#date').datepicker({autoclose:true}).on('changeDate', function(ev) {
        $(this).closest('form').validate().element($(this));
    });

    $('#mychosen').chosen().on('change', function(ev) {
        $(this).closest('form').validate().element($(this));
    });
    */


    $(document).one('ajaxloadstart.page', function (e) {
        //in ajax mode, remove remaining elements before leaving page
        $('[class*=select2]').remove();
    });

    //datepicker plugin
    //link
    $('.date-picker').datepicker({
        autoclose: true,
        todayHighlight: true,
        multidate: true
    })
        //show datepicker when clicking on the icon
        .next().on(ace.click_event, function () {
            $(this).prev().focus();
        });

    //or change it into a date range picker
    $('.input-daterange').datepicker({ autoclose: true });


    //to translate the daterange picker, please copy the "examples/daterange-fr.js" contents here before initialization
    $('input[name=date-range-picker]').daterangepicker({
        'applyClass': 'btn-sm btn-success',
        'cancelClass': 'btn-sm btn-default',
        locale: {
            applyLabel: 'Apply',
            cancelLabel: 'Cancel',
        }
    })
        .prev().on(ace.click_event, function () {
            $(this).next().focus();
        });


    $('#timepicker1').timepicker({
        minuteStep: 1,
        showSeconds: true,
        showMeridian: false,
        disableFocus: true,
        icons: {
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down'
        }
    }).on('focus', function () {
        $('#timepicker1').timepicker('showWidget');
    }).next().on(ace.click_event, function () {
        $(this).prev().focus();
    });




    if (!ace.vars['old_ie']) $('#date-timepicker1').datetimepicker({
        //format: 'MM/DD/YYYY h:mm:ss A',//use this option to display seconds
        icons: {
            time: 'fa fa-clock-o',
            date: 'fa fa-calendar',
            up: 'fa fa-chevron-up',
            down: 'fa fa-chevron-down',
            previous: 'fa fa-chevron-left',
            next: 'fa fa-chevron-right',
            today: 'fa fa-arrows ',
            clear: 'fa fa-trash',
            close: 'fa fa-times'
        }
    }).next().on(ace.click_event, function () {
        $(this).prev().focus();
    });

    //files
    $('#LeaveAttachments').ace_file_input({
        style: 'well',
        btn_choose: 'Drop files here or click to choose',
        btn_change: null,
        no_icon: 'ace-icon fa fa-cloud-upload',
        droppable: true,
        thumbnail: 'small'//large | fit
        //,icon_remove:null//set null, to hide remove/reset button
        /**,before_change:function(files, dropped) {
            //Check an example below
            //or examples/file-upload.html
            return true;
        }*/
        /**,before_remove : function() {
            return true;
        }*/
        ,
        preview_error: function (filename, error_code) {
            //name of the file that failed
            //error_code values
            //1 = 'FILE_LOAD_FAILED',
            //2 = 'IMAGE_LOAD_FAILED',
            //3 = 'THUMBNAIL_FAILED'
            //alert(error_code);
        }

    }).on('change', function () {
        //console.log($(this).data('ace_input_files'));
        //console.log($(this).data('ace_input_method'));
    });

    autosize($('textarea[class*=autosize]'));
    $('textarea.limited').inputlimiter({
        remText: '%n character%s remaining...',
        limitText: 'max allowed : %n.'
    });


    //Leaves datatables isht
    var leavestable =
        $('#leaves-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null, null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    leavestable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(leavestable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    leavestable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(leavestable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#leaves-table > thead > tr > th input[type=checkbox], #leaves-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#leaves-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) leavestable.row(row).select();
            else leavestable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#leaves-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) leavestable.row(row).deselect();
        else leavestable.row(row).select();
    });



    $(document).on('click', '#leaves-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });

    $("#leaves-table").on("click", ".submitleave", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-paper-plane'></i> Submit record for approval?",
            message: "Do you wish to submit this leave number " + docno + " for approval?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/Leaves/Submit',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);

                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $("#leaves-table").on("click", ".cancelleave", function (e) {
        e.preventDefault();
  
        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-times'></i>  Cancel approval request?",
            message: "Do you wish to cancel this leave number " + docno + " for approval?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/Leaves/Cancel',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $("#leaves-table").on("click", ".deleteleaves", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-trash'></i> Delete?",
            message: "Do you wish to delete this leave number " + docno + "?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/Leaves/Delete',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });

                                    window.setTimeout(function () {
                                        location.reload(true);
                                    }, 2000);
                                } else {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    //Approval Entries

    var approvalentriestable =
        $('#approvalentries-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null, null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    approvalentriestable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(approvalentriestable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    approvalentriestable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(approvalentriestable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#approvalentries-table > thead > tr > th input[type=checkbox], #approvalentries-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#approvalentries-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) approvalentriestable.row(row).select();
            else approvalentriestable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#approvalentries-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) approvalentriestable.row(row).deselect();
        else approvalentriestable.row(row).select();
    });

    $(document).on('click', '#approvalentries-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });
    $("#approvalentries-table").on("click", ".appoveapprovalentry", function (e) {
        e.preventDefault();

        var pid = $(this).attr('data-id');
        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-check'></i> Approve record?",
            message: "Do you wish to approve this approval entry for document number " + docno + "?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {                    

                    jQuery.ajax({
                        url: '/ApprovalEntries/Approve',
                        type: "POST",
                        data: '{EntryNo:"' + pid + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);

                                if (data.Status =="000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $("#approvalentries-table").on("click", ".rejectapprovalentry", function (e) {
        e.preventDefault();

        var pid = $(this).attr('data-id');
        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-times'></i> Reject record?",
            message: "Do you wish to reject this approval entry for document number " + docno +"?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/ApprovalEntries/Approve',
                        type: "POST",
                        data: '{EntryNo:"' + pid + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    //Leave Recalls scripts

    $leaverecallvalidation = true;

    $('#leaverecall-wizard').ace_wizard().on('actionclicked.fu.wizard', function (e, info) {
        if (info.step == 1 && $validation) {
            if (!$('#leaverecallleaveselection-form').valid()) e.preventDefault();
        }
        if (info.step == 2 && $validation) {
            if (!$('#leaverecalldayselection-form').valid()) e.preventDefault();
        }
        if (info.step == 3 && $validation) {
            if (!$('#leaverecalldayselection-form').valid()) e.preventDefault();
        }
    }).on('finished.fu.wizard', function (e) {
        bootbox.dialog({
            message: "Thank you! Your information was successfully saved!",
            buttons: {
                "success": {
                    "label": "OK",
                    "className": "btn-sm btn-primary"
                }
            }
        });
    }).on('stepclick.fu.wizard', function (e) {
        //e.preventDefault();//this will prevent clicking and selecting steps
    });


    $('#leaverecallleaveselection-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            ApprovedLeave: {
                required: true,
            },
            LeaveDaysApplied: {
                required: true,
            },
            LeaveStartDate: {
                required: true,
            },
            LeaveEndDate: {
                required: true
            },
            ReturnDate: {
                required: true
            }
        },

        messages: {
            ApprovedLeave: "Please choose the leave approved from the list"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });
    $('#leaverecalldayselection-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            DaysRecalled: {
                required: true,
            }
        },

        messages: {
            ApprovedLeave: "Please specify the number of days to recall here"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });
    $('#leaverecallattachments-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            Comment: {
                required: true,
            }
        },

        messages: {
            Comment: "Please specify a comment"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });

    //files
    $('#LeaveRecallAttachments').ace_file_input({
        style: 'well',
        btn_choose: 'Drop files here or click to choose',
        btn_change: null,
        no_icon: 'ace-icon fa fa-cloud-upload',
        droppable: true,
        thumbnail: 'small'//large | fit
        //,icon_remove:null//set null, to hide remove/reset button
        /**,before_change:function(files, dropped) {
            //Check an example below
            //or examples/file-upload.html
            return true;
        }*/
        /**,before_remove : function() {
            return true;
        }*/
        ,
        preview_error: function (filename, error_code) {
            //name of the file that failed
            //error_code values
            //1 = 'FILE_LOAD_FAILED',
            //2 = 'IMAGE_LOAD_FAILED',
            //3 = 'THUMBNAIL_FAILED'
            //alert(error_code);
        }

    }).on('change', function () {
        //console.log($(this).data('ace_input_files'));
        //console.log($(this).data('ace_input_method'));
    });

    //datatable
    var leaverecallstable =
        $('#leaverecalls-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null, null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    leaverecallstable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(leaverecallstable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    leaverecallstable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(leaverecallstable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#leaverecalls-table > thead > tr > th input[type=checkbox], #leaverecalls-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#leaverecalls-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) leaverecallstable.row(row).select();
            else leaverecallstable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#leaverecalls-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) leaverecallstable.row(row).deselect();
        else leaverecallstable.row(row).select();
    });



    $(document).on('click', '#leaverecalls-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });

    //actions

    $("#leaverecalls-table").on("click", ".submitleaverecall", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-paper-plane'></i> Submit record for approval?",
            message: "Do you wish to submit this leave recall number " + docno + " for approval?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/LeaveRecalls/Submit',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);

                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $("#leaverecalls-table").on("click", ".cancelleaverecall", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-times'></i> Cancel approval request?",
            message: "Do you wish to cancel this leave recall number " + docno + " for approval?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/LeaveRecalls/Cancel',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                } else {
                                    $.gritter.add({
                                        title: 'Approval Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $("#leaverecalls-table").on("click", ".deleteleaverecall", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-trash'></i> Delete?",
            message: "Do you wish to delete this leave recall number " + docno + "?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/LeaveRecalls/Delete',
                        type: "POST",
                        data: '{DocumentNo:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });
                                    window.setTimeout(function () {
                                        location.reload(true);
                                    }, 2000);
                                } else {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    //Reports

    $('#reports-form').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            Month: {
                required: function (element) {
                    return $("#Payslip").is(":checked");
                }
            },
            Report: {
                required: true,
            },
            Year: {
                required: true,
            }
        },

        messages: {
            Month: "Please choose a month you want to generate a payslip",
            Report: "Please choose the report"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });


    $("#Print").click(function (event) {

        if (!$('#reports-form').valid()) e.preventDefault();

        //Ajax call here
        if ($("#Payslip").is(":checked")) {

        }

        bootbox.dialog({
            message: "Thank you! Your report was successfully generated!<br><br><br><br> <a class='grey' href='/images/myw3schoolsimage.jpg' download='w3logo'>Download Here <i class='ace-icon fa fa-print bigger-130'></i></a>",
            buttons: {
                "success": {
                    "label": "OK",
                    "className": "btn-sm btn-primary"
                }
            }
        });

        event.preventDefault();
    });

    //deparments

    var departmentstable =
        $('#departments-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    departmentstable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(departmentstable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    departmentstable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(departmentstable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#departments-table > thead > tr > th input[type=checkbox], #departments-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#departments-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) departmentstable.row(row).select();
            else departmentstable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#departments-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) departmentstable.row(row).deselect();
        else departmentstable.row(row).select();
    });



    $(document).on('click', '#departments-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });
    $("#departments-table").on("click", ".deletedepartment", function (e) {
        e.preventDefault();

        var pid = $(this).attr('data-id');
        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-trash'></i> Delete Department?",
            message: "Do you wish to delete this department " + docno + "?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/Departments/Delete',
                        type: "POST",
                        data: '{DocumentNo:"' + pid + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);

                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Action Notification',
                                        text: data.Message,
                                        class_name: 'gritter-success gritter-center'
                                    });

                                    window.setTimeout(function () {
                                        location.reload(true);
                                    }, 2000);

                                } else {
                                    $.gritter.add({
                                        title: 'Action Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    //create department
   

    $('#departmentsform').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            Name: {
                required: true,
            }
        },

        messages: {
            Name: "Please give a department name"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });

    $("#SaveDepartment").click(function (event) {

        //var formAction = $("#electralpositionform").attr('action');

        //console.log(formAction);


        if ($('#departmentsform').valid()) {
            //Serialize the form datas.  
            var valdata = $("#departmentsform").serialize();
            //to get alert popup  	

            jQuery.ajax({
                url: '/Departments/CreateDepartment',
                type: "POST",
                data: valdata,
                dataType: "json",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);


                        if (data.Status == "000") {
                            $.gritter.add({
                                title: 'Action Notification',
                                text: data.Message,
                                class_name: 'gritter-success gritter-center'
                            });

                            window.setTimeout(function () {
                                window.location.href = '/Departments/Index';
                            }, 2000);

                        } else {
                            $.gritter.add({
                                title: 'Action Notification',
                                text: data.Message,
                                class_name: 'gritter-error gritter-center'
                            });
                        }
                    }
                },
                error: function (e) {
                    console.log(e.responseText);
                }
            });
        }

        event.preventDefault();
    });
    $("#UpdateDepartment").click(function (event) {

        //var formAction = $("#electralpositionform").attr('action');

        //console.log(formAction);


        if ($('#departmentsform').valid()) {
            //Serialize the form datas.  
            var valdata = $("#departmentsform").serialize();
            //to get alert popup  	

            jQuery.ajax({
                url: '/Departments/UpdateDepartment',
                type: "POST",
                data: valdata,
                dataType: "json",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);


                        if (data.Status == "000") {
                            $.gritter.add({
                                title: 'Action Notification',
                                text: data.Message,
                                class_name: 'gritter-info'
                            });

                            window.setTimeout(function () {
                                window.location.href = '/Departments/Index';
                            }, 2000);

                        } else {
                            $.gritter.add({
                                title: 'Action Notification',
                                text: data.Message,
                                class_name: 'gritter-error gritter-center'
                            });
                        }
                    }
                },
                error: function (e) {
                    console.log(e.responseText);
                }
            });
        }

        event.preventDefault();
    });

    //holidays

    var holidaystable =
        $('#holidays-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    holidaystable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(holidaystable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    holidaystable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(holidaystable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#holidays-table > thead > tr > th input[type=checkbox], #holidays-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#holidays-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) holidaystable.row(row).select();
            else holidaystable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#holidays-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) holidaystable.row(row).deselect();
        else holidaystable.row(row).select();
    });



    $(document).on('click', '#holidays-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });


    //employee

    $("#SaveEmployee").click(function (event) {

        if ($('#employeeform').valid()) {
            //Serialize the form datas.  
            var valdata = $("#employeeform").serialize();
            //to get alert popup  	

            jQuery.ajax({
                url: '/Employees/CreateEmployee',
                type: "POST",
                data: valdata,
                dataType: "json",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);

                        //console.log(data.Message);
                        bootbox.dialog({
                            message: data.Message,
                            buttons: {
                                "success": {
                                    "label": "OK",
                                    "className": "btn-sm btn-primary"
                                }
                            }
                        });
                    }
                },
                error: function (e) {
                    console.log(e.responseText);
                }
            });
        }

        event.preventDefault();
    });
    $("#UpdateEmployee").click(function (event) {

        if ($('#employeeform').valid()) {
            //Serialize the form datas.  
            var valdata = $("#employeeform").serialize();
            //to get alert popup  	

            jQuery.ajax({
                url: '/Employees/UpdateEmployee',
                type: "POST",
                data: valdata,
                dataType: "json",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);

                        //console.log(data.Message);
                        bootbox.dialog({
                            message: data.Message,
                            buttons: {
                                "success": {
                                    "label": "OK",
                                    "className": "btn-sm btn-primary"
                                }
                            }
                        });
                    }
                },
                error: function (e) {
                    console.log(e.responseText);
                }
            });
        }

        event.preventDefault();
    });

    $.mask.definitions['~'] = '[+-]';
    $('#Phone').mask('+254 999-999999');

    jQuery.validator.addMethod("Phone", function (value, element) {
        return this.optional(element) || /^\+\d{3}\ \d{3}\-\d{6}( x\d{1,6})?$/.test(value);
    }, "Enter a valid phone number.");

    $('#employeeform').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            Email: {
                required: true,
                email: true
            },
            Password: {
                required: true,
                minlength: 5
            },
            ConfirmPassword: {
                required: true,
                minlength: 5,
                equalTo: "#Password"
            },
            Phone: {
                required: true,
                Phone: 'required'
            },
            FirstName: {
                required: true
            },
            LastName: {
                required: true
            },
            Name: {
                required: true
            },
            Faculty: {
                required: true
            },
            Gender: {
                required: true,
            },
            YearOfStudy: {
                required: true,
            }
        },

        messages: {
            Email: {
                required: "Please provide a valid email.",
                email: "Please provide a valid email."
            },
            Password: {
                required: "Please specify a password.",
                minlength: "Please specify a secure password."
            },
            Phone: "Please provide a valid number",
            Faculty: "Please choose faculty",
            YearOfStudy: "Please choose year of study"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });

    var employeestable =
        $('#employees-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null, 
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    employeestable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(employeestable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    employeestable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(employeestable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#employees-table > thead > tr > th input[type=checkbox], #employees-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#employees-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) employeestable.row(row).select();
            else employeestable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#employees-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) employeestable.row(row).deselect();
        else employeestable.row(row).select();
    });



    $(document).on('click', '#employees-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });


    //Approval users

    var approvaluserstable =
        $('#approvalusers-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],
                select: {
                    style: 'multi'
                }
            });


    setTimeout(function () {
        $($('.tableTools-container')).find('a.dt-button').each(function () {
            var div = $(this).find(' > div').first();
            if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
            else $(this).tooltip({ container: 'body', title: $(this).text() });
        });
    }, 500);


    approvaluserstable.on('select', function (e, dt, type, index) {
        if (type === 'row') {
            $(approvaluserstable.row(index).node()).find('input:checkbox').prop('checked', true);
        }
    });
    approvaluserstable.on('deselect', function (e, dt, type, index) {
        if (type === 'row') {
            $(approvaluserstable.row(index).node()).find('input:checkbox').prop('checked', false);
        }
    });

    /////////////////////////////////
    //table checkboxes
    $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

    //select/deselect all rows according to table header checkbox
    $('#approvalusers-table > thead > tr > th input[type=checkbox], #approvalusers-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
        var th_checked = this.checked;//checkbox inside "TH" table header

        $('#approvalusers-table').find('tbody > tr').each(function () {
            var row = this;
            if (th_checked) approvaluserstable.row(row).select();
            else approvaluserstable.row(row).deselect();
        });
    });

    //select/deselect a row when the checkbox is checked/unchecked
    $('#approvalusers-table').on('click', 'td input[type=checkbox]', function () {
        var row = $(this).closest('tr').get(0);
        if (this.checked) approvaluserstable.row(row).deselect();
        else approvaluserstable.row(row).select();
    });



    $(document).on('click', '#approvalusers-table .dropdown-toggle', function (e) {
        e.stopImmediatePropagation();
        e.stopPropagation();
        e.preventDefault();
    });

    $("#approvalusers-table").on("click", ".deleteapprover", function (e) {
        e.preventDefault();

        var docno = $(this).attr('data-docno');

        bootbox.confirm({
            title: "<i class='fa fa-trash'></i> Delete?",
            message: "Do you wish to delete approver?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success'
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger'
                }
            },
            callback: function (result) {

                if (result == true) {

                    jQuery.ajax({
                        url: '/Settings/DeleteApprover',
                        type: "POST",
                        data: '{id:"' + docno + '" }',
                        dataType: "json",
                        contentType: "application/json; charset=utf-8",
                        success: function (response) {

                            if (response != null) {
                                //console.log(JSON.stringify(response)); //it comes out to be string 

                                //we need to parse it to JSON
                                var data = $.parseJSON(response);


                                if (data.Status == "000") {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-info gritter-center'
                                    });

                                    window.setTimeout(function () {
                                        location.reload(true);
                                    }, 2000);

                                } else {
                                    $.gritter.add({
                                        title: 'Delete Notification',
                                        text: data.Message,
                                        class_name: 'gritter-error gritter-center'
                                    });
                                }
                            }
                        }
                    });
                }
            }
        });
    });

    $('#createapproverform').validate({
        errorElement: 'div',
        errorClass: 'help-block',
        focusInvalid: false,
        ignore: "",
        rules: {
            DocumentType: {
                required: true
            },
            Approver: {
                required: true
            },
            ApproverEmail: {
                required: true,
                email: true
            },
            SubstituteApprover: {
                required: true
            },
            SubstituteApproverEmail: {
                required: true,
                email: true
            },
            ApprovalSequence: {
                required: true,
                number: true
            }
        },

        messages: {
            ApproverEmail: {
                required: "Please provide a valid email.",
                email: "Please provide a valid email."
            },
            SubstituteApproverEmail: {
                required: "Please provide a valid email.",
                email: "Please provide a valid email."
            },            
            YearOfStudy: "Please choose year of study"
        },


        highlight: function (e) {
            $(e).closest('.form-group').removeClass('has-info').addClass('has-error');
        },

        success: function (e) {
            $(e).closest('.form-group').removeClass('has-error');//.addClass('has-info');
            $(e).remove();
        },

        errorPlacement: function (error, element) {
            if (element.is('input[type=checkbox]') || element.is('input[type=radio]')) {
                var controls = element.closest('div[class*="col-"]');
                if (controls.find(':checkbox,:radio').length > 1) controls.append(error);
                else error.insertAfter(element.nextAll('.lbl:eq(0)').eq(0));
            }
            else if (element.is('.select2')) {
                error.insertAfter(element.siblings('[class*="select2-container"]:eq(0)'));
            }
            else if (element.is('.chosen-select')) {
                error.insertAfter(element.siblings('[class*="chosen-container"]:eq(0)'));
            }
            else error.insertAfter(element.parent());
        },

        submitHandler: function (form) {
        },
        invalidHandler: function (form) {
        }
    });

    $("#SaveApprovalUser").click(function (event) {

        if ($('#createapproverform').valid()) {
            //Serialize the form datas.  
            var valdata = $("#createapproverform").serialize();
            //to get alert popup  	

            jQuery.ajax({
                url: '/Settings/CreateApprovalUser',
                type: "POST",
                data: valdata,
                dataType: "json",
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (response) {
                    if (response != null) {
                        //console.log(JSON.stringify(response)); //it comes out to be string 

                        //we need to parse it to JSON
                        var data = $.parseJSON(response);

                        //console.log(data.Message);
                        bootbox.dialog({
                            message: data.Message,
                            buttons: {
                                "success": {
                                    "label": "OK",
                                    "className": "btn-sm btn-primary"
                                }
                            }
                        });
                    }
                },
                error: function (e) {
                    console.log(e.responseText);
                }
            });
        }

        event.preventDefault();
    });

})