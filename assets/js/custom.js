jQuery(function ($) {

    $('#MultipleSelection').change(function () {

        $('#DateRanges').hide();
        $('#Multiples').show();
    });
    $('#RangeSelection').change(function () {

        $('#Multiples').hide();
        $('#DateRanges').show();

    });

    $("#StartDate").change(function () {
        $('#LeaveStartDate').val($('#StartDate').val());
    });

    $("#EndDate").change(function () {
        $('#LeaveEndDate').val($('#EndDate').val());
    });

    if ($("#RangeSelection").is(":checked")) {
        $("#EndDate").change(function () {
            //Get Leave Quantity And ReturnDate

        });
    }



    //autofill leavetypes on LeaveType

    $.ajax({
        url: '/Leaves/ListLeaveTypes',

        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            if (response != null) {

                var data = $.parseJSON(response);

                $.each(data, function (i, item) {
                    $("#LeaveType").append($('<option></option>').attr("value", item.Code).text(item.Description));
                });

            }
        },
        error: function (e) {
            console.log(e.responseText);
        }
    });

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
            }
            if (info.step == 2 && $validation) {
                if (!$('#leavedaysselection-form').valid()) e.preventDefault();
            }
            if (info.step == 3 && $validation) {
                if (!$('#leaveattachments-form').valid()) e.preventDefault();
            }
        })
        //.on('changed.fu.wizard', function() {
        //})
        .on('finished.fu.wizard', function (e) {
            //submit forms here
            //Serialize the form datas.  
            var valdata = $("#leaveselection-form").serialize();

            jQuery.ajax({
            url: '/Leaves/SaveSelection',
            type: "POST",
            data: valdata,
            dataType: "json",
            contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
            success: function (response) {
                if (response != null) {
                    //console.log(JSON.stringify(response)); //it comes out to be string 

                    //we need to parse it to JSON
                    var data = $.parseJSON(response);

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

                                //Attachments
                                var files = jQuery("#LeaveAttachments").get(0).files;
                                var fileData = new FormData();

                                for (var i = 0; i < files.length; i++) {
                                    fileData.append("LeaveAttachments", files[i]);
                                }

                                jQuery.ajax({
                                    type: "POST",
                                    url: "/Leaves/UploadFiles",
                                    dataType: "json",
                                    contentType: false, // Not to set any content header
                                    processData: false, // Not to process data
                                    data: fileData,
                                    success: function (result, status, xhr) {
                                        alert(result);
                                    },
                                    error: function (xhr, status, error) {
                                        alert(status);
                                    }
                                });





                                //console.log(data.Message);

                                //bootbox.dialog({
                                //    message: data.Message,
                                //    buttons: {
                                //        "success": {
                                //            "label": "OK",
                                //            "className": "btn-sm btn-primary"
                                //        }
                                //    }
                                //});


                            }
                        },
                        error: function (e) {
                            console.log(e.responseText);
                        }
                    });


                }
            },
            error: function (e) {
                console.log(e.responseText);
            }
        });

            //bootbox.dialog({
            //    message: "Thank you! Your information was successfully saved!",
            //    buttons: {
            //        "success": {
            //            "label": "OK",
            //            "className": "btn-sm btn-primary"
            //        }
            //    }
            //});
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
            SelectionType: {
                required: true,
            },
            StartDate: {
                required: function (element) {
                    return $("#RangeSelection").is(":checked");
                }
            },
            LeaveDates: {
                required: function (element) {
                    return $("#MultipleSelection").is(":checked");
                }
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
                required: true,
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

    var myTable =
        $('#dynamic-table')
            //.wrap("<div class='dataTables_borderWrap' />")   //if you are applying horizontal scrolling (sScrollX)
            .DataTable({
                bAutoWidth: false,
                "aoColumns": [
                    { "bSortable": false },
                    null, null, null, null, null, null, null,
                    { "bSortable": false }
                ],
                "aaSorting": [],


                //"bProcessing": true,
                //"bServerSide": true,
                //"sAjaxSource": "http://127.0.0.1/table.php"	,

                //,
                //"sScrollY": "200px",
                //"bPaginate": false,

                //"sScrollX": "100%",
                //"sScrollXInner": "120%",
                //"bScrollCollapse": true,
                //Note: if you are applying horizontal scrolling (sScrollX) on a ".table-bordered"
                //you may want to wrap the table inside a "div.dataTables_borderWrap" element

                //"iDisplayLength": 50


                select: {
                    style: 'multi'
                }
            });
            ////$.fn.dataTable.Buttons.defaults.dom.container.className = 'dt-buttons btn-overlap btn-group btn-overlap';

            ////new $.fn.dataTable.Buttons(myTable, {
            ////    buttons: [
            ////        {
            ////            "extend": "colvis",
            ////            "text": "<i class='fa fa-search bigger-110 blue'></i> <span class='hidden'>Show/hide columns</span>",
            ////            "className": "btn btn-white btn-primary btn-bold",
            ////            columns: ':not(:first):not(:last)'
            ////        },
            ////        {
            ////            "extend": "copy",
            ////            "text": "<i class='fa fa-copy bigger-110 pink'></i> <span class='hidden'>Copy to clipboard</span>",
            ////            "className": "btn btn-white btn-primary btn-bold"
            ////        },
            ////        {
            ////            "extend": "csv",
            ////            "text": "<i class='fa fa-database bigger-110 orange'></i> <span class='hidden'>Export to CSV</span>",
            ////            "className": "btn btn-white btn-primary btn-bold"
            ////        },
            ////        {
            ////            "extend": "excel",
            ////            "text": "<i class='fa fa-file-excel-o bigger-110 green'></i> <span class='hidden'>Export to Excel</span>",
            ////            "className": "btn btn-white btn-primary btn-bold"
            ////        },
            ////        {
            ////            "extend": "pdf",
            ////            "text": "<i class='fa fa-file-pdf-o bigger-110 red'></i> <span class='hidden'>Export to PDF</span>",
            ////            "className": "btn btn-white btn-primary btn-bold"
            ////        },
            ////        {
            ////            "extend": "print",
            ////            "text": "<i class='fa fa-print bigger-110 grey'></i> <span class='hidden'>Print</span>",
            ////            "className": "btn btn-white btn-primary btn-bold",
            ////            autoPrint: false,
            ////            message: 'This print was produced using the Print button for DataTables'
            ////        }
            ////    ]
            ////});
            ////myTable.buttons().container().appendTo($('.tableTools-container'));

            //////style the message box
            ////var defaultCopyAction = myTable.button(1).action();
            ////myTable.button(1).action(function (e, dt, button, config) {
            ////    defaultCopyAction(e, dt, button, config);
            ////    $('.dt-button-info').addClass('gritter-item-wrapper gritter-info gritter-center white');
            ////});


            ////var defaultColvisAction = myTable.button(0).action();
            ////myTable.button(0).action(function (e, dt, button, config) {

            //    defaultColvisAction(e, dt, button, config);


            //    if ($('.dt-button-collection > .dropdown-menu').length == 0) {
            //        $('.dt-button-collection')
            //            .wrapInner('<ul class="dropdown-menu dropdown-light dropdown-caret dropdown-caret" />')
            //            .find('a').attr('href', '#').wrap("<li />")
            //    }
            //    $('.dt-button-collection').appendTo('.tableTools-container .dt-buttons')
            //});

            ////

            setTimeout(function () {
                $($('.tableTools-container')).find('a.dt-button').each(function () {
                    var div = $(this).find(' > div').first();
                    if (div.length == 1) div.tooltip({ container: 'body', title: div.parent().text() });
                    else $(this).tooltip({ container: 'body', title: $(this).text() });
                });
            }, 500);





            myTable.on('select', function (e, dt, type, index) {
                if (type === 'row') {
                    $(myTable.row(index).node()).find('input:checkbox').prop('checked', true);
                }
            });
            myTable.on('deselect', function (e, dt, type, index) {
                if (type === 'row') {
                    $(myTable.row(index).node()).find('input:checkbox').prop('checked', false);
                }
            });




            /////////////////////////////////
            //table checkboxes
            $('th input[type=checkbox], td input[type=checkbox]').prop('checked', false);

            //select/deselect all rows according to table header checkbox
            $('#dynamic-table > thead > tr > th input[type=checkbox], #dynamic-table_wrapper input[type=checkbox]').eq(0).on('click', function () {
                var th_checked = this.checked;//checkbox inside "TH" table header

                $('#dynamic-table').find('tbody > tr').each(function () {
                    var row = this;
                    if (th_checked) myTable.row(row).select();
                    else myTable.row(row).deselect();
                });
            });

            //select/deselect a row when the checkbox is checked/unchecked
            $('#dynamic-table').on('click', 'td input[type=checkbox]', function () {
                var row = $(this).closest('tr').get(0);
                if (this.checked) myTable.row(row).deselect();
                else myTable.row(row).select();
            });



            $(document).on('click', '#dynamic-table .dropdown-toggle', function (e) {
                e.stopImmediatePropagation();
                e.stopPropagation();
                e.preventDefault();
            });

})