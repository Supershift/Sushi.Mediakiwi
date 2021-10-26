var uri = null;
var isNoClick = false;
var accordionOpened;
var rteIndex = 0;
function hookEventsForAccordionPanelForm(panel) {
    // reinit  date and timepicker
    panel.find(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });
    panel.find('textarea.rte').each(function () {
        var newID = $(this).attr('id') + "_" + rteIndex;
        $(this).attr('id', newID)
        rteIndex++;
        tinymce.execCommand('mceAddEditor', true, newID);
    });
    panel.find(".timepicker").timepicker({});
 

    // capture button presses
    panel.find('input[type="submit"]').click(function () {

        return false;
    });
    // capture button presses
    panel.find('input[type="submit"]').click(function () {
     
        accordionDoPanelPostBack(panel, $(this).attr('id'));
        return false;
    });
    panel.find('#iconBarz a').click(function () {
        var theA = $(this);
        // only capture postbacks.. url navs are allowed (like a popup header)
        if (theA.attr('href') == "#") {
        
            accordionDoPanelPostBack(panel, theA.attr('id'));
            return false;
        }
    });
}
function accordionDoPanelPostBack(panel, idOfElement) {
    var dataAccordion = panel.parent().prev().data('accordion');
    var formData = new FormData();
    // set button clicked    
    formData.append("autopostback", idOfElement);
     
    // gather all form values
    var inputs = panel.find('#gridsection').find("input, select, button, textarea"); 
    inputs.prop('disabled', true);
    // Send it    
    inputs.each(function (i, item) {
        var input = $(inputs[i]);
     
        if (  input.attr('type') != "submit" &&
            !(input.attr('type') == "radio" && input[0].checked == false) &&
            input.attr('name') != null) {
        
            if (input.hasClass("rte")) {
                 
                formData.append(input.attr('name'), tinyMCE.get(input.attr('id')).getContent());
            }
            else
                formData.append(input.attr('name'), input.val());

        }
    });
    $.ajax({
        url: document.forms[0].action + "&openinframe=1&item=" + dataAccordion,
        dataType: 'html',
        type: "POST",
        cache: false,
        mimeType: "multipart/form-data",
        contentType: false,
        processData: false,
        data: formData
    }
        ).then(function (html) {
            inputs.prop('disabled', false);
            accordionBindResponsePanel(panel, html, null)
        }, function () {
            alert('click failed');
            inputs.prop('disabled', false);
        })
}
function accordionBindResponsePanel(panel, html, onlyOneOpen) {
    var target = panel.find('article');
    target.html('');
    var storage = document.createElement('div');
    storage.innerHTML = html;
    target.html($(storage).find('#popupContent'));
    // TODO: work with all events
    hookEventsForAccordionPanelForm(panel);
    // open the panel
    panel.css('display', 'table-cell');
    // set this panel as opened if we only allow one to be opened
    if (onlyOneOpen != null && onlyOneOpen == true)
        accordionOpened = panel;

}
function handleAccordionClick(self, dataAccordion) {
    console.log('Click is accordion click');
    var panel = $(self).parent('tr').next().find('td');

    // closing
    if (panel.is(":visible")) {
        panel.css('display', 'none');

    }// opening
    else {
        // close any opend
        if (accordionOpened != null)
            accordionOpened.css('display', 'none');
        // retrieve the list load async
        $.ajax({
            url: document.forms[0].action + "&openinframe=1&item=" + dataAccordion,
            dataType: 'html',
            type: "get"
        }
            ).then(function (html) {

                accordionBindResponsePanel(panel, html, $(self).parent('tr').attr('data-accordionSinglePanelOpen') == "true")
            }, function () {
                alert('The list load of ' + dataAccordion + 'couldn\'t be loaded');
            })

    }
}
// the accordion header panel close button click
function handleAccordionSelfClose(self) {
    console.log('Self close');
    var panel = $(self).parents('td');
    panel.css('display', 'none');
    accordionOpened = null;
}
function SetClick(self) {

    if (isNoClick)
        return;


    /* for table cells that should not be clickable */
    /* REMOVED:  || $(self).find('input').length > 0 || $(self).find('a').length > 0 as this holds back the normal proces */
    if (($(self).attr('class') != null && $(self).attr('class').indexOf('nopt') > -1)) {
        return;
    }
    var dataAccordion = $(self).parent('tr').attr('data-accordion');
    if (dataAccordion != null && dataAccordion.length > 0) {
        handleAccordionClick(self, dataAccordion);
        return;
    }
    var isTable = true;
    var id = $(self).parent('tr').attr('id');
    //  Retry for custom HTML
    if (id == null) {
        id = $(self).attr('id');
        isTable = false;
    }
    if (id != null) {
        var spli = id.split('_')[1].split('$');
        var datalink = $(self).parent('tr').attr('data-link');
        var url = '';
        if (datalink != null) {

            if (datalink.indexOf('[KEY]') > -1)
                url = datalink.replace('[KEY]', spli[1]);
            else {
                if (datalink.indexOf('&item=') > -1)
                    url = datalink;
                else if (!datalink.indexOf('&item') > -1)
                    url = datalink + '=' + spli[1];
                else
                    url = datalink;
            }
        }
        else {
            var group = getUrlVars()["group"];
            var folder = getUrlVars()["folder"];

            if (folder != undefined)
                folder = '&folder=' + folder;
            else
                folder = '';

            if (group != undefined) {
                var groupitem = getUrlVars()["groupitem"];
                url = document.URL.split('?')[0] + '?group=' + group + folder + '&groupitem=' + groupitem + '&item=' + spli[1];
            }
            else
                url = document.URL.split('?')[0] + '?item=' + spli[1] + folder;
        }
        var datatarget = null;
        var title = null;
        if (isTable) {
            datatarget = $(self).parent('tr').parent('tbody').attr('data-layer');
            title = $(self).parent('tr').parent('tbody').attr('data-title');
        }
        else
            datatarget = $(self).attr('data-layer');

        if (datatarget != null) {
            if (url.indexOf('openinframe') == -1) {
                if (url.indexOf('?') > -1)
                    url += '&openinframe=2';
                else
                    url += '?openinframe=2';
            }
            mediakiwi.openLayer(url, $(self).parent('tr'), datatarget, title);
            return;
        }
        //  Add openinframe if present
        if (url.indexOf('openinframe') == -1) {
            var oif = getParam(location.href, 'openinframe');
            if (oif != null) {
                url += '&openinframe=' + oif;
            }
        }

        location.href = url;
    }


    /* for table rows having a checkbox or hidden text field in the first cell */
    var chk = $(self).parent('tr').find('td:first').find('input:first');
    if (chk != null) {
        var idv = chk.attr('id');
        var sel = chk.attr('value');
        var val = idv + '|' + sel;
        //chk.attr('checked', true);

        if (id != null)
            $(self).parent('tr').attr('class', 'parent selected');
        var ref = getUrlVars()["referid"];
        if (sel != null && idv != null && ref != null) {

            /* Remove the _ at the begin */

            var put = ref.substring(1, ref.length) + '$' + idv;
            var pnt = '#' + ref;
            var loc = parent.$(pnt).attr('class').indexOf('single');
            var addOnTop = parent.$(pnt).attr('class').indexOf('newItemsOnTop');
            console.log('Add on top : ' + addOnTop);

            if (loc > -1)
                parent.$(pnt + ' li').remove();
            else {
                $(self).parent('tr').hide('highlight', {}, 350, null);
            }

            //
            if (addOnTop > -1)
                parent.$(pnt).prepend('<li class="ui-state-default">' + sel + '<figure class="icon-x del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');
            else
                parent.$(pnt).append('<li class="ui-state-default">' + sel + '<figure class="icon-x del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');

            if (loc > -1) parent.$.fn.colorbox.close();
            var pbk = parent.$(pnt).attr('class').indexOf('postBack');
            if (pbk > -1) {
                if (parent.$('.async').length)
                    return parent.postValue();
                else
                    parent.document.forms[0].submit();
            }
        }
    }
}
function getUrl(u, a, b, c, d) {
    u = u.split('#')[0];
    var url = '';
    var hashes = u.slice(u.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        var hash = hashes[i].split('=');
        if (hash[0] == a) continue;
        if (hash[0] == c) continue;
        url += (url.length == 0 ? '?' : '&') + hash[0] + '=' + hash[1];
    }
    if (a == undefined)
        return window.location.href.split("?")[0] + url;
    if (c == undefined)
        return window.location.href.split("?")[0] + url + '&' + a + '=' + b;
    return window.location.href.split("?")[0] + url + '&' + a + '=' + b + '&' + c + '=' + d;
}
function getParam(u, n) {
    var url = '';
    var hashes = u.slice(u.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        if (hash[0] == n)
            return hash[1];
    }
    return null;
}
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

//  Part of sortable
function Switch(self) {
    //  Sortable
    var me = $(self).attr('id');
    var id = me.split('_')[1].split('$')[1];
    var found;
    $('.dataBlock table tbody tr').each(function () {
        if (found) {
            var url = '?list=' + me.split('_')[1].split('$')[0] + '&sortF=' + id + '&sortT=' + $(this).attr('id').split('_')[1].split('$')[1];
            $.get(url, function (data) { });
            return false;
        }
        if (me == $(this).attr('id')) {
            found = true;
        }
    });
}

function getParameters(x) {
    var settingsObject = {}, hash, hashes = $(x).attr('href').split('&'), i;
    for (i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        settingsObject[hash[0]] = hash[1];
    }
    return settingsObject;
}

/*Select/deselect all checkboxes and add active class*/
function toggleChecked(status) {
    $(".dataBlock input").each(function () {
        $(this).attr("checked", status);
        $(this).parent('tr').addClass("active");
        $(this).closest('tr').toggleClass("active", this.checked);
    });
}


var xhr;
function postValue(url) {
    var isGrid = true;
    if ($('#autopostback').val() == 'export_xls')
        $('#autopostback').val('');

    //if ($('#autopostback').val() == 'edit')
    //    isGrid = false;

    var frm = $(document.forms[0]);
    var dat = frm.serializeArray();

    if (url == null || url == '#') {
        if (isGrid)
            url = '?AJAX=1&' + window.location.href.slice(window.location.href.indexOf('?') + 1);
        else
            url = '?split=homeArticle&' + window.location.href.slice(window.location.href.indexOf('?') + 1);

    } else {
        var p = getParam(url, 'set');
        if (p > 0) {
            document.forms[0].action = getUrl(document.forms[0].action, "set", p);
        }
    }

    $("div#loader").fadeIn('slow');
    if (xhr && xhr.readystate != 4) {
        xhr.abort();
    }

    xhr = $.post(url, dat, function (data) {
        if (isGrid)
            $("#datagrid").html(data);
        else {
            $("#homeArticle").html(data);
            mediakiwi.initRTE();
            mediakiwi.initSourceCode();
        }

        mediakiwi.ASyncIsReady();
        $("abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
        $(".abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
        return false;
    })
      .always(function () {
          $("div#loader").stop();
          $('div#loader').fadeOut("fast");
          //top.$.colorbox.close();
          return false;
      });
}

$(document).on("click", "input.async", function () {
    postValue($(this).attr('href'));
    return false;
});

$(document).on("click", "a.async", function () {
    postValue($(this).attr('href'));
    return false;
});

$(function () {
    $("abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
    $(".abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
});


$(document).on("click", ".postBack", function () {
    var tag = $(this).get(0).tagName.toLowerCase();
    var type = $(this).get(0).type.toLowerCase();
    if (tag == 'select' || tag == 'text' || (tag == 'input' && type != 'submit')) return;
    $('#autopostback').val($(this).attr('id'));
    if ($(this).get(0).className.indexOf('async') > -1)
        return postValue();
    else
        document.forms[0].submit();
    return false;
});

// CB; mute optie voor postback, dit is te gebruiken bij async selects met een postback clear target
//      Dit is te zetten op het aantal keren dat de 
var mutePostbackEvent = 0;
$(document).on("change", ".postBack", function () {
    if (mutePostbackEvent > 0) {
        mutePostbackEvent--;
        return;
    }
    var tag = $(this).get(0).tagName.toLowerCase();

    if (tag != 'select' && tag != 'text' && tag != 'input') return;
    $('#autopostback').val($(this).attr('id'));
    //$("div#loader").fadeIn(1000);
    if ($('.async').length && $(this).get(0).className.indexOf('nosync') == -1)
        return postValue();
    else
        document.forms[0].submit();
});

$(document).on("click", ".dataBlock table td", function () {
    SetClick(this);
});

$(document).on("click", ".mk-dataitem", function () {
    SetClick(this);
});


// PLANNINGSTOOL
function SelectPlanningInput($this) {
    var input = parseFloat($this.val());
    if (isNaN(input)) {
        var td = $this.parents('td');
        var planned = parseFloat(td.data('planned'));
        var available = parseFloat(td.data('available'));
        if (isNaN(available))
            available = 0;
        $this.val(available);
    }
    $this.css('background-color', '#FFF');
    $this.css('border', '1px solid #FFF');
    $this.css('color', '#000');
}

$(document).on("click", "#inner input", function () {
    var $this = $(this);
    SelectPlanningInput($this);
    $this.select();
});
$(document).on("focus", "#inner input", function () {
    var $this = $(this);
    SelectPlanningInput($this);
});
$(document).on("blur", "#inner input", function () {
    var td = $(this).parents('td');
    var planned = parseFloat(td.data('planned'));
    var available = parseFloat(td.data('available'));

    var closestDIV = $(this).parent();
    var input = parseFloat($(this).val());
    if (isNaN(input)) {
        input = booked;
    }
    applyBackgroundColor(closestDIV, input);
    $(this).val(input.toFixed(2));
    $(this).css('background-color', 'transparent');
    $(this).css('border', '1px solid transparent');
    $(this).css('color', '#fff');
    var topSpan = $(this).prev('span');

    available -= (parseFloat(input) - planned);
    //available += planned;

    var roundedAvailability = Math.round(available * 100) / 100;

    if (roundedAvailability <= 0)
        $('.' + topSpan.attr("class")).text('');
    else
        $('.' + topSpan.attr("class")).text(roundedAvailability);

    var taskTotal = $('.task_' + td.data('taskid'));
    var total = parseFloat(taskTotal.first().text());
    total -= (parseFloat(input) - planned);

    taskTotal.each(function () {
        $(this).text(total.toFixed(2));
        $(this).css("color", (total < 0) ? "red" : "");
    });
    postSingleValue(input, td.data('resource'), td.data('date'), td.data('taskid'));
});

function applyBackgroundColor(div, inp) {
    if (inp != null) {
        if (inp >= 8) {
            div.addClass('full');
        }
        else if (inp <= 7 && inp > 0) {
            div.addClass('half');
        }
        else if (inp >= 0) {
            div.removeClass('half').removeClass('full');
        }
    }
}

function postSingleValue(newValue, resourceID, date, taskID) {
    $.ajax({
        url: '?AJAX=1&' + window.location.href.slice(window.location.href.indexOf('?') + 1),
        method: 'POST',
        data: {
            value: newValue,
            user: resourceID,
            task: taskID,
            date: date
        }
    });
}

$(document).ready(function () {
    $('.cplan').each(function () {
        if ($(this).data('date') == undefined)
            return;
        var i = $(this).data('date').substring(0, 10);
        var b = $(this).data('booked');
        var a = $(this).data('available');
        a = (a == '0') ? '' : a;

        var r = $(this).data('resource');
        var p = $(this).data('planned');
        var x = (p == '') ? a : p.toFixed(2);
        $(this).append('<div><span class="rs_' + r + '_' + i + '"  >' + a + '</span><input id="pl' + r + '.' + i + '"   class="noSubmit prs_' + r + '_' + i + '"  type="text" maxlength="5" min="0" step="1" pattern="\d+" /> <span>' + b + '</span></div>');
        applyBackgroundColor($(this).find('div'), p);
        if (p > 0)
            $(this).find('input').val(p.toFixed(2));
    });
});
// EIND PLANNINGSTOOLS



$(document).ready(function () {
    var loaded = 0;
    $(".async").each(function () {
        if (loaded == 0) {
            loaded = 1;
            var datalink = $(this).attr('data-link');
            var url = null;
            if (datalink != null)
                url = '?AJAX=1&' + datalink;
            postValue(url);
        }
    });

    $(".numeric").numeric({ negative: false });


    $('.uploadFile :file').bind('change', function () {
        var split = $(this).val().split('\\');
        $('#info_' + $(this).attr('id')).val(split[split.length - 1]);
    });
    $('.autoupload').bind('change', function () {
        $('#autopostback').val('save');
        document.forms[0].submit();
    });

    $('#export').bind('change', function () {
        if ($(this).val() != '') {
            $.colorbox({ iframe: true, transition: "none", width: '425px', opacity: '0.6', height: '226px', scrolling: false, fixed: true, href: "?list=161&openinframe=2&q=" + getUrlVars()["list"] });
            $(this).val('');
        }
    });
    $(".toggle").click(function () {
        $(".formfilters").slideToggle("fast");
    });
    $('.hundred #toggle').addClass('arrowD');
    $('.hundred #toggle').removeClass('arrowU');
    $('.hundred #sideNav').hide();
    $('.hundred #sideNav').css('z-index', '0');

    //$('#responsive-menu-button').sidr({
    //    name: 'sidr-main',
    //    source: '#mainMenu'
    //});

    //$(".footer ul").tinyNav();

    $('#toggle').bind('click', function () {
        var sideNav = $('#sideNav');
        if (sideNav.is(':hidden')) {
            $('#homeArticle').animate({
                paddingLeft: "190px", zIndex: "1"
            }, 600, null,
                function () {
                    $('#sideNav').css('z-index', '10');
                }
            );
            $('#toggle').addClass('arrowU');
            $('#toggle').removeClass('arrowD');
            sideNav.show();
        }
        else {
            $('#homeArticle').animate({
                paddingLeft: "40px", zIndex: "0"
            }, 600, null,
                function () {
                    $('#sideNav').css('z-index', '0');
                }
            );
            $('#toggle').addClass('arrowD');
            $('#toggle').removeClass('arrowU');
            sideNav.hide();
        }

    });

    $('#ascrail2000').show();
    $(".side").click(function () {

        if ($(this).next().hasClass('activeChannel')) {
            $(this).next().removeClass('activeChannel').slideUp('fast');
        }
        else {
            $(".activeChannel").fadeToggle('fast').toggleClass('activeChannel');
            $(this).next().fadeToggle('fast').toggleClass('activeChannel');
        }
        $("a.side").removeClass('active');
        $(this).addClass('active');
    });
    $(document).on('mousedown', function (e) {
        if ($(e.target).closest(".activeChannel").length === 0) {
            $(".activeChannel").hide();
            $(".activeChannel").removeClass('activeChannel').slideUp('fast');
        }
    });
    $(function () {
        $(".mail-1").toggle(function () {
            //Open menu     
            $(".inbox").stop().animate({ left: "-=233" }, 400)
        }, function () {
            //Close menu
            $(".inbox").stop().animate({ left: "+=233" }, 400)
        });
        $(".user-1").toggle(function () {
            //Open menu     
            $(".metaMenu").stop().animate({ left: "-=233" }, 400)
        }, function () {
            //Close menu
            $(".metaMenu").stop().animate({ left: "+=233" }, 400)
        });
        $(".magnifying-glass-1").toggle(function () {
            //Open menu     
            $(".searchFields").stop().animate({ left: "-=233" }, 400)
        }, function () {
            //Close menu
            $(".searchFields").stop().animate({ left: "+=233" }, 400)
        });
    });
    if (navigator.userAgent.match(/Trident.*rv:11\./)) {
        $('body').addClass('ie11');
    }

    $('.postparent').each(function () {
        var idv = $(this).attr('id');
        var sel = $(this).attr('value');
        var multiple = $(this).attr('data-multiple');
        var editUrl = $(this).attr('data-editurl');
        var editTitle = $(this).attr('data-listtitle');
        var ref = $(this).attr('data-target');

        if (ref == undefined)
            ref = getUrlVars()["referid"];

        var val = idv + '|' + sel;
        if (ref != undefined) {

            var put = ref.substring(1, ref.length) + '$' + idv;
            var pnt = '#' + ref;
            var att = parent.$(pnt).attr('class');
            if (att != undefined) {
                var isSingle = att.indexOf('single') > -1;
                var addOnTop = att.indexOf('newItemsOnTop');
                console.log('Add on top : ' + addOnTop);

                if (multiple != '1' && isSingle) {
                    var existingInput = parent.$(pnt).find(' > li > input').get(0);
                    var existingLink = parent.$(pnt).find(' > li > a').get(0);

                    // update existing Input field (if any)
                    if (existingInput) {
                        $(existingInput).attr('id', put);
                        $(existingInput).attr('name', put);
                        $(existingInput).attr('value', val);
                    }

                    // Update existing link (if any)
                    if (existingLink) {
                        $(existingLink).html(sel);
                    }

                    // When both dont exist, add them
                    if (!existingInput && !existingLink) {
                        var newEl = '<li class="instant">';

                        // Did we receive the edit list url ? Create a link if we did
                        if (editUrl) {
                            newEl += `<a class="openlayer" data-layer="width:790px,height:450px,iframe:true,scrolling:false" title="${editTitle}" href="${editUrl}">${sel}</a>`;
                        }
                        // Else just add the selected value
                        else {
                            newEl += sel;
                        }

                        // Add remove link and hidden input
                        newEl += `<figure class="icon-x del"></figure> <input type="hidden" id="${put}" name="${put}" value="${val}" /> </li>`;

                        // Add element to parent
                        parent.$(pnt).append(newEl);
                    }
                }
                else {

                    // Add new items on top, instead of bottom
                    if (addOnTop > -1) {
                        parent.$(pnt).prepend('<li class="ui-state-default">' + sel + '<figure class="icon-x del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');
                    }
                    else {
                        parent.$(pnt).append('<li class="ui-state-default">' + sel + '<figure class="icon-x del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');
                    }
                }
            }
        }
        mediakiwi.closeLayer();
    });
    $('.closeLayer').each(function () {
        mediakiwi.closeLayer();
    });
    $('.postParent').each(function () {
        mediakiwi.closeLayer();

        if (parent.$('.async').length)
            return parent.postValue();
        else {
            var url = $(this).attr('data-url');

            if (url == undefined || url == '')
                parent.document.forms[0].submit();
            else
                parent.document.location = url;
        }
    });
    $('.post').click(function () {
        var tag = $(this).get(0).tagName.toLowerCase();
        var type = $(this).get(0).type.toLowerCase();
        if (tag == 'select' || tag == 'text' || (tag == 'input' && type != 'submit')) return;
        $('#autopostback').val($(this).attr('id'));
        //document.forms[0].submit();
    });
    $('.post').change(function () {
        var tag = $(this).get(0).tagName.toLowerCase();
        if (tag != 'select' && tag != 'text' && tag != 'input') return;
        $('#autopostback').val($(this).attr('id'));
        //document.forms[0].submit();
    });

    $('.ambiance').each(function () {
        $(this).hide();
        $.ambiance({ message: $(this).html(), timeout: 5 });
    });
    $(".ui-tabs-nav").find(".ui-tabs-selected")

    /*Date/timepicker*/

    $(".datepicker").datepicker({
        changeMonth: true,
        changeYear: true
    });

    $(".timepicker").timepicker({});

    /*Expand tables*/
    $('tr.parent').hover(function () {
        var $this = $(this);
        var openen = $this.find('a.open');
        var thisId = $this.siblings('.child-' + this.id);

        openen.css("cursor", "pointer").attr("title", "Click to expand/collapse");

        openen.click(function () {
            thisId.toggle();
            thisId.toggleClass("activeSub")
            $this.toggleClass("active");
            openen.toggleClass("close");
        });
    });
    $('tr[class^=child]').hide().children('td');
    $(".checkbox").change(function () {
        $(this).closest('tr').toggleClass("active", this.checked);
    });

    $('input[type=text]').keypress(function (e) {
        if (e.which == 13) {
            if ($(this).hasClass('noSubmit') || e.target.id == 'FilterTitle') {
                $(this).blur();
                return false;
            }
            if ($('.async').length) {
                postValue();
                return false;
            }
            else
                document.forms[0].submit();
        }
    });

    $(".connectedSortable").sortable({
        connectWith: ".connectedSortable",
        placeholder: "ui-state-highlight"
    }).disableSelection();

    //var list = document.querySelectorAll(".connectedSortable");


    $('.multiSortable li figure.del').live('click', function () {
        var liParent = $(this).parent();
        var litem = liParent.clone();
        liParent.remove();
    });

    $('.simpleSortDown').live('click', function () {
        var cmsBox = $(this).parents('.cmsable');
        var nextBox = cmsBox.next();
        if (nextBox != null || nextBox[0] != null) {

            cmsBox.find('textarea.rte').each(function () {
                tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
            });

            cmsBox.detach(); // detatch;
            cmsBox.insertAfter(nextBox);

            cmsBox.find('textarea.rte').each(function () {
                tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
            });
        }
        return false;
    });

    $('.simpleSortUp').live('click', function () {
        var cmsBox = $(this).parents('.cmsable');
        var prevBox = cmsBox.prev();
        if (prevBox != null || prevBox[0] != null) {

            cmsBox.find('textarea.rte').each(function () {
                tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
            });

            cmsBox.detach(); // detatch;
            cmsBox.insertBefore(prevBox);

            cmsBox.find('textarea.rte').each(function () {
                tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
            });
        }
        return false;
    });

    //  Part of sortable
    var sortableIsAlive = false;
    $(document).on("click", ".sortOrder", function () {
        
        if ($(this).attr('class').indexOf('icon-sort') > -1) {
            if (sortableIsAlive) {
                $(".dataBlock table tbody").sortable("enable");
            }
            else {
                let wscrolltop = 0;
                $('.dataBlock table tbody').sortable({
                    placeholder: "ui-state-highlight",
                    items: "tr:not(.nosort)",
                    cancel: ".nosort",
                    start: function (event, ui) {
                        //jQuery Ui-Sortable Overlay Offset Fix
                        if ($.browser.webkit) {
                            wscrolltop = $(window).scrollTop();
                        }
                    },
                    sort: function (event, ui) {
                        //jQuery Ui-Sortable Overlay Offset Fix
                        if ($.browser.webkit) {
                            ui.helper.css({ 'top': ui.position.top + wscrolltop + 'px' });
                        }
                    },
                    update: function (event, ui) {
                        Switch(ui.item);
                    }
                }).disableSelection();
                sortableIsAlive = true;
            }
            $(this).attr('class', $(this).attr('class').replace("icon-sort", "icon-list2"));
            $('.dataBlock table td').off('click');
            $.ambiance({ message: 'Sorting is enabled.' });
        } else {
            $(".dataBlock table tbody").sortable("disable");
            $(this).attr('class', $(this).attr('class').replace("icon-list2", "icon-sort"));
            $('.dataBlock table td').click(function () {
                SetClick(this);
            });
            $.ambiance({ message: 'Sorting is disabled.' });
        }
        return false;
    });

    // Return a helper with preserved width of cells
    var fixHelper = function (e, ui) {
        ui.children().each(function () {
            $(this).width($(this).width());
        });
        return ui;
    };


});

//tinymce.get('textarea.rte').getBody().setAttribute('contenteditable', false);

function showLink() {
    var dom = tinymce.activeEditor.dom;
    var selectedElm = tinymce.activeEditor.selection.getNode();
    var anchorElm = dom.getParent(selectedElm, 'a[href]');
    var href = dom.getAttrib(anchorElm, 'href');
    var url = '?list=161b60c4-75a6-4b54-a710-1e1691f738b4&openinframe=1&referid=tmce&notitle=1';
    if (href != false && href.indexOf('wim:') == 0) {
        var linkID = href.replace('wim:', '');
        url += "&item=" + linkID;
        // console.log(url);
    }
    var title = 'Add link';
    $.colorbox({ href: url, title: title, iframe: true, fixed: true, width: "790px", height: "360px", transition: "elastic", scrolling: false });
}
function insertLink(href) {

    var inst = tinymce.activeEditor;
    var elm, elementArray, i;
    elm = inst.selection.getNode();
    elm = inst.dom.getParent(elm, "A");
    // Create new anchor elements
    if (elm == null) {
        inst.getDoc().execCommand("unlink", false, null);
        tinymce.execCommand("mceInsertLink", false, href, { skip_undo: 1 });

        elementArray = tinymce.grep(inst.dom.select("a"), function (n) { return inst.dom.getAttrib(n, 'href') == href; });

        for (i = 0; i < elementArray.length; i++) {

            elm = elementArray[i];
            setLinkData(elm, 'href', href);

            // Refresh in old MSIE
            if (tinymce.isMSIE5)
                elm.outerHTML = elm.outerHTML;
        }
    } else {
        setLinkData(elm, 'href', href);
    }
    // Don't move caret if selection was image
    if (elm.childNodes.length != 1 || elm.firstChild.nodeName != 'IMG') {
        inst.focus();
        inst.selection.select(elm);
        inst.selection.collapse(0);
    }
    tinymce.execCommand("mceEndUndoLevel");
    $.colorbox.close();
}

function removeLink() {
    tinymce.execCommand("unlink", false, null);
    $.colorbox.close();
}

function setLinkData(elm, attrib, value) {
    var dom = tinymce.activeEditor.dom;

    if (typeof (value) == "undefined" || value == null) {
        value = "";
    }
    // Clean up the style
    if (attrib == 'style')
        value = dom.serializeStyle(dom.parseStyle(value), 'a');

    dom.setAttrib(elm, attrib, value);
}

//  CLEAN CLEAN CLEAN CLEAN CLEAN CLEAN 

var mediakiwi = mediakiwi || {};


$(function () {
    $(document).ready(function () {
        //  Horizontal scrollbar with tables
        $(".inner").niceScroll({
            cursorcolor: "#a3acb4",
            cursoropacitymin: 0.3,
            cursorborder: "0",
            cursorwidth: 10,
            boxzoom: false,
            autohidemode: false,
            cursorminheight: 30,
            cursorwidth: 10
        });
        if (mediakiwi.GridScrollbar != undefined) {
            $('.inner').getNiceScroll(mediakiwi.GridScrollbar.index).doScrollLeft(mediakiwi.GridScrollbar.x, 1);
        }
        //  Shorten text when Abbr is applied in a tabe
        $('td abbr').shorten();
        $('th abbr').shorten();
    })
});

mediakiwi.moveGridScrollbarX = function (x, index) {
    mediakiwi.GridScrollbar = new Object();
    mediakiwi.GridScrollbar.x = x;
    mediakiwi.GridScrollbar.index = index;
};

mediakiwi.async = [];
mediakiwi.registerASyncReadyCallBack = function (name, f) {
    this.async[name] = f;
};
mediakiwi.ASyncIsReady = function () {
    //console.log(this.async);
    for (var key in this.async) {
        //console.log(key);
        this.async[key]();
    }
};

mediakiwi.collection = [];
mediakiwi.registerJsonCallBack = function (name, f) {
    this.collection[name] = f;
};
mediakiwi.callBack = function (name, selectedValue, data) {

    if (this.collection[name] != undefined)
        this.collection[name](selectedValue, data);
};
mediakiwi.registerJsonCallBack('foo', function (selectedValue, data) { alert('bar'); console.log(data); });
// mediakiwi.registerASyncReadyCallBack('foo', function () {  });

mediakiwi.openLayer = function (url, item, layerinfo, title, html) {
    if (url == undefined)
        url = item.attr('data-link');

    if (title == undefined)
        title = item.attr('data-title');

    var layerproperty = [];
    if (layerinfo == undefined)
        layerinfo = item.attr('data-layer');
    if (layerinfo != undefined) {
        jQuery.each(layerinfo.split(','), function (i, val) {
            var nv = val.split(':');
            layerproperty[nv[0]] = nv[1];
        });
    }
    if (layerproperty['class'] != undefined)
        $('#colorbox').removeClass().addClass(layerproperty['class']);
    else
        $('#colorbox').removeClass();
    //console.log(url);

    var iframe = (layerproperty['iframe'] == undefined || layerproperty['iframe'] == 'true') ? true : false;
    //console.log(iframe);

    $.colorbox({
        href: url,
        html: html,
        title: title,
        iframe: iframe,
        fixed: (layerproperty['fixed'] == undefined || layerproperty['fixed'] == 'true') ? true : false,
        width: (layerproperty['width'] == undefined ? '790px' : layerproperty['width']),
        height: layerproperty['height'],
        transition: "none",
        scrolling: (layerproperty['scrolling'] == undefined || layerproperty['scrolling'] == 'true') ? true : false
    });

};
// [CB: 24-06-2015] PageComponentControl  
mediakiwi.pageComponentControl = mediakiwi.pageComponentControl || {};
mediakiwi.pageComponentControl.animationSpeed = 600;

mediakiwi.pageComponentControl.moveComponentUp = function (elm, componentID) {
    var container = $(elm).parents('.OrderingField');
    var prevContainer = container.prev();

    if (prevContainer.find('.isSortable')[0] != null) { // has the sort class

        container.find('textarea.rte').each(function () {
            tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
        });

        container.detach(); // detatch;
        container.insertBefore(prevContainer);

        container.find('textarea.rte').each(function () {
            tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
        });
    }
    return false;
}

mediakiwi.pageComponentControl.moveComponentDown = function (elm, componentID) {
    var container = $(elm).parents('.OrderingField');
    var nextContainer = container.next();

    if (nextContainer.find('.isSortable')[0] != null) { // has the sort class

        container.find('textarea.rte').each(function () {
            tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
        });

        container.detach(); // detatch;
        container.insertAfter(nextContainer);

        container.find('textarea.rte').each(function () {
            tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
        });
    }
    return false;
}

mediakiwi.pageComponentControl.moveComponentDelete = function (elm, componentID, nameType, componentTemplateID) {

    if (confirm("Please confirm that the '" + nameType + "' may be deleted.")) {
        var container = $(elm).parents('.OrderingField');
        container.remove();

        if (mediakiwi.pageComponentControl.options != undefined) {
            var editorBlock = mediakiwi.pageComponentControl.options[componentTemplateID];
            if (editorBlock != undefined && editorBlock.indexOf('removeAfterUse="1"') > 0) // lousy but effective check for checking if option must return back in the cms
                $('#cms > div > div > a:last-child').after(editorBlock);
        }
    }
    return false;
}

mediakiwi.pageComponentControl.showOptions = function () {
    $('#addPageComponentControlItems').find('.cmsBlocks a').hide();
    $('#addPageComponentControlItems').show().width("0%").animate({ width: "100%" }, mediakiwi.pageComponentControl.animationSpeed, function () {
        $('#addPageComponentControlItems').find('.cmsBlocks a').show();
    });
    $('#addPageComponentControlAdder').hide();
}

mediakiwi.pageComponentControl.closeOptions = function () {
    $('#addPageComponentControlItems').find('.cmsBlocks a').hide();
    $('#addPageComponentControlItems').animate({ width: "0%" }, mediakiwi.pageComponentControl.animationSpeed, function () {
        $(this).hide();
        $('#addPageComponentControlAdder').show();
    });

}

mediakiwi.pageComponentControl.addItemToPage = function (elm, itemID, pageID, containerID) {
    var $elm = $(elm);


    $.ajax({
        url: getUrl(document.forms[0].action, "xml", "component") + "&id=" + itemID + "&cmpt=" + containerID,
        dataType: 'json',
        type: "POST",
        error: function (da) {
            // wierd why this ends in error because the status code is 200
            if (da.status == 200) {
                $('#cms').before(da.responseText);
                var container = $('#cms').prev();

                container.find('textarea.rte').each(function () {
                    tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
                });
                // if the component is fixed, hide option after use (maybe in the future we need it)
                if ($elm.attr('removeafteruse') == "1")
                    $elm.remove();
            }
        }

    });
    return false;
}

// end pageComponentControl
$(document).bind('cbox_complete', function () {
    $(".abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
});

mediakiwi.closeLayer = function () {
    parent.$.fn.colorbox.close();
};
mediakiwi.initRTE = function () {
    //  Tiny MCE controls
    tinymce.init({
        selector: "textarea.rte",
        plugins: [
                "autolink autosave lists",
                "visualblocks code nonbreaking",
                "autoresize link"
        ],
        browser_spellcheck: true,
        toolbar: "bold italic underline bullist numlist indent outdent | link unlink removeformat subscript superscript code",
        statusbar: false,
        table_cell_advtab: false,
        table_appearance_options: false,
        forced_root_block: false,
        menubar: false,
        toolbar_items_size: 'small'
    });
    tinymce.init({
        selector: "textarea.table_rte",
        plugins: [
                "autolink autosave lists",
                "visualblocks code nonbreaking",
                "autoresize link  table contextmenu"
        ],
        browser_spellcheck: true,
        toolbar: "bold italic underline bullist numlist indent outdent | link unlink removeformat subscript superscript code table",
        statusbar: false,
        //table_class_list: [
        //      { title: 'None', value: '' },
        //      { title: 'Dog', value: 'dog' },
        //      { title: 'Cat', value: 'cat' }
        //],
        table_cell_advtab: false,
        table_appearance_options: false,
        forced_root_block: false,
        menubar: false,
        toolbar_items_size: 'small'
    });
    //  Tiny MCE controls
    tinymce.init({
        selector: "textarea.fixed_rte",
        plugins: [
                "autolink autosave lists",
                "visualblocks code nonbreaking",
                "link"
        ],
        browser_spellcheck: true,
        toolbar1: "bold italic underline bullist numlist indent outdent | link unlink removeformat subscript superscript code",
        statusbar: false,
        forced_root_block: false,
        menubar: false,
        toolbar_items_size: 'small'
    });
};
mediakiwi.initSourceCode = function (container) {
    //  CodeMirror controls
    if (container == undefined)
        container = '';
    if (container.length > 0)
        container += ' ';

    //console.log(container);

    $((container + '.SourceCode')).each(function (index, element) {
        if ($(element).attr('data-done') == undefined || container == '') {
            $(element).attr('data-done', '1');

            //console.log(element);

            var editor = CodeMirror.fromTextArea(element, {
                wordwrapping: true,
                autoCloseTags: true,
                lineNumbers: true,
                fixedGutter: true,
                lineWrapping: true,
                readOnly: ($(this).attr("class").indexOf('editable') > -1 ? false : true),
                extraKeys: {
                    "Esc": function (cm) {
                        cm.setOption("fullScreen", !cm.getOption("fullScreen"));
                    }
                }
            });
            var pending;
            editor.on("change", function () {
                clearTimeout(pending);
                pending = setTimeout(update, 400);
            });
            function looksLikeX(code) {
                if (code == undefined)
                    return true;
                return /^</.test(code);
            }
            function update() {
                editor.setOption("mode", looksLikeX(editor.getValue()) ? "htmlmixed" : "clike");
            }
            if (editor) {
                update();
            }

        }
    });
};
mediakiwi.multifileUpload = function () {

    // init 
    var uploadRow = '<li><span>#filename#</span> - <span>#size#</span><span  class="remove"><a href="#" data-file="#filename#">Remove</a></span></li>';
    var progressRow = '<li><span>#filename#</span> - <span>#size#</span><br/><progress id="#index#" max="100"  value="0"></progress></li>';
    $('.multiBox').each(function (num, i) {
        var multiBox = $(i);
        multiBox.files = [];
        multiBox.currentXHRs = [];
        multiBox.userCancel = false;
        multiBox.uploading = false;


        var addButton = multiBox.find('.multiAssetUpload_Button');
        var uploadField = multiBox.find('input[type="file"]');
        var resultFiles = multiBox.find('.multiAssetUpload_resultFiles');
        var clearButton = multiBox.find('.clearButton');
        var uploadButton = multiBox.find('.uploadButton');
        var cancelButton = multiBox.find('.cancelButton');
        var messageBox = multiBox.find('.message');
        var resultCollection = multiBox.find('.resultCollection');
        cancelButton.hide();
        resultFiles.html("");

        multiBox.on('dragover', function (event) {
            event.preventDefault();
            event.stopPropagation();
            if (multiBox.uploading == false) {
                multiBox.addClass('dropTarget');
            }
        });
        multiBox.on('dragleave', function (event) {
            event.preventDefault();
            event.stopPropagation();
            if (multiBox.uploading == false) {
                multiBox.removeClass('dropTarget');
            }
        });
        multiBox.on('drop', function (event) {
            if (multiBox.uploading == false) {
                event.preventDefault();
                event.stopPropagation();
                var files = event.originalEvent.dataTransfer.files;
                $.merge(multiBox.files, files);

                multiBox.removeClass('dropTarget');

                bind();
            }
            return false;
        });

        addButton.on('click', function () {
            uploadField.click();
        });
        uploadField.on('change', function (e) {
            var input = $(e.target);
            var files = $.makeArray(input.prop('files'));
            $.merge(multiBox.files, files);
            bind();
        });
        cancelButton.on('click', function (e) {
            multiBox.userCancel = true;
            for (var i in multiBox.currentXHRs) {
                multiBox.currentXHRs[i].abort();
            }
            messageBox.html("Transfers have been canceled");
            addButton.show();
            clearButton.show();
            uploadButton.show();
            cancelButton.hide();
            bind();
            return false;
        });
        clearButton.on('click', function (e) {
            if (multiBox.files.length > 0) {
                if (confirm("Are you sure you want to clear the list?")) {
                    multiBox.files = [];
                    bind();
                }
            }
            return false;
        });
        uploadButton.on('click', function (e) {
            if (multiBox.files.length > 0) {
                multiBox.uploading = true;
                addButton.hide();
                clearButton.hide();
                uploadButton.hide();
                cancelButton.show();
                bindUploading();
            }
            else {
                alert('Please add 1 or more items to the collection');
            }
            return false;
        });
        function bindUploading() {
            multiBox.userCancel = false;
            var html = "";
            $(multiBox.files).each(function (i, item) {
                html += progressRow.replace(/#filename#/g, item.name).replace(/#size#/g, renderSize(item.size)).replace(/#index#/g, i);
            });
            resultFiles.html(html);

            $(multiBox.files).each(function (i, item) {
                item.done = false;
                var formData = new FormData();
                formData.append('file', item);
                var xhr = $.ajax({
                    url: 'multifileupload.ashx?gallery=' + uploadField.attr('galleryGUID') + "&source=" + resultCollection.attr('id'),
                    data: formData,
                    cache: false,
                    contentType: false,
                    processData: false,
                    type: 'POST',
                    xhr: function () {
                        var _xhr = new window.XMLHttpRequest();
                        //Upload progress
                        _xhr.upload.addEventListener("progress", function (evt) {
                            if (evt.lengthComputable) {
                                var percentComplete = Math.round(100 * (evt.loaded / evt.total));
                                var progressBar = resultFiles.find('#' + i);
                                console.log(percentComplete);
                                progressBar.attr('value', percentComplete);
                            }
                        }, false);
                        return _xhr;
                    }
                }).success(function (ID) {

                    resultCollection.val(resultCollection.val() + ',' + ID.toString());
                    var progressBar = resultFiles.find('#' + i);

                    progressBar.attr('value', 100);
                    item.done = true;
                    var bussy = false;
                    for (var index in multiBox.files) {
                        if (multiBox.files[index].done == false)
                            bussy = true;
                    }

                    if (bussy == false) {
                        cancelButton.hide();
                        messageBox.html("All files have been uploaded");
                        multiBox.currentXHRs = [];

                    }
                }).fail(function () {
                    if (multiBox.userCancel == false) {
                        var progressBar = resultFiles.find('#' + i);
                        progressBar.hide();
                        messageBox.html("One or more files were not transfered.");
                    }
                });
                multiBox.currentXHRs.push(xhr);
            });
        }
        function bind() {
            var html = "";
            $(multiBox.files).each(function (i, item) {
                html += uploadRow.replace(/#filename#/g, item.name).replace(/#size#/g, renderSize(item.size));
            });
            resultFiles.html(html);
            resultFiles.find('.remove a').on('click', function () {
                var fileToRemove = $(this).data('file');
                multiBox.files = $.grep(multiBox.files, function (a) {
                    return a.name != fileToRemove;
                });
                bind();
            });
        }

        function renderSize(bytes) {
            if (bytes < 1000) return bytes + " bytes";
            if (bytes < 1000000) return Math.round(bytes / 1000) + " KB";
            if (bytes < 1000000000) return Math.round(bytes / 1000000) + " MB";
            return Math.round(bytes / 1000000000) + " GB";
        }
    });
};


$(function () {
    $(document).ready(function () {
        mediakiwi.initRTE();
        mediakiwi.multifileUpload();

        $(document).on("click", ".cmsable .closer", function () {
            var that = $(this);
            var data = that.parent('h3').parent('div');

            // Check if we need to remove tiny MCE implementation first.
            var tinyMceImplementation = data.find('textarea.rte');
            if (tinyMceImplementation) {
                tinymce.execCommand('mceRemoveEditor', false, $(tinyMceImplementation).attr('id'));
            }

            data.remove();
            return false;
        });
        $(document).on("click", ".addContentType", function () {
            var that = $(this);
            var type = that.attr('data-type');
            var target = that.parent('div').parent('div');

            var width = target.attr('data-width');
            var gallery = target.attr('data-gallery');

            var name = target.attr('data-name');
            if (this.count == undefined)
                this.count = parseInt(target.attr('data-count')) + 1;
            else
                this.count++;
            var url = document.URL.replace("#", '') + '&xml=' + type + '&id=' + name + '&index=' + this.count + '&w=' + width + '&gallery=' + gallery;
            $.ajax({
                type: "GET",
                url: url,
                dataType: "html",
                success: function (data) {
                    target.parent('td').find('.multitarget').append(data);
                    mediakiwi.initRTE();
                    mediakiwi.initSourceCode('.multitarget');
                }
            });
            return false;
        })

        //CB: Page filter (?top=1)
        //    This filters client side all links and widget which can contain the searched text
        $.extend($.expr[':'], {
            'containsi': function (elem, i, match, array) {
                return (elem.textContent || elem.innerText || '').toLowerCase()
                .indexOf((match[3] || "").toLowerCase()) >= 0;
            }
        });

        $('#FilterTitle,.searchFilter').on('keyup', function () {
            var $searchFilterTarget = $(this).parents("article").find('#startWidgets');
            var toSearch = $(this).val();

            $searchFilterTarget.find("div>a:not(:containsi('" + toSearch + "'))").hide();
            var fItems = $searchFilterTarget.find("div>a:containsi('" + toSearch + "')");
            fItems.show();
            // Hide the widgets that don't have matches
            $searchFilterTarget.find(".widget").hide();
            fItems.parent().show();
        });
        // End Page Filter

        $('input[type=checkbox].checkall').bind('click', function () {
            var $th = $(this).parents('th');
            var nth = $th[0].cellIndex + 1;
            $th.parents('table').find('tr>td:nth-child(' + nth + ') input').click();
        });

        $('.openlayerauto').each(function () {
            var that = $(this);
            var url = that.attr('href');
            if (url == undefined)
                url = that.attr('data-url');

            mediakiwi.openLayer(url, that, that.attr('data-layer'), that.attr('title'));
            return false;
        });

        $(document).on("click", ".click", function () {
            var that = $(this);
            var url = that.attr('href');
            if (url == undefined)
                url = that.attr('data-link');
            location.href = url;
            return false;
        });

        //$('.openlayer').click(function () {
        $(document).on("click", ".openlayer", function () {
            var that = $(this);

            //  [9 nov 14:MM] Layer in layer temporary solution 
            if (window.parent.location != window.location) {
                parent.$.fn.colorbox.resize({ width: "870px", height: "90%" });
                $(document).bind('cbox_cleanup', function () {
                    parent.$.fn.colorbox.resize({ width: "790px", height: "90%" });
                });
            }
            mediakiwi.openLayer(that.attr('href'), that, that.attr('data-layer'), that.attr('data-title'));
            return false;
        });

        $(document).on("click", ".closelayer", function () {
            mediakiwi.closeLayer();
            return false;
        });
        $(document).on("click", ".confirm", function () {
            if ($(this).get(0).className.indexOf('async') > -1)
                return postValue();
            else
                document.forms[0].submit();
            return false;
        });
        $('.type_confirm').click(function () {
            var that = $(this);
            $('#autopostback').val($(this).attr('id'));
            var confirm = that.attr('data-confirm');
            var confirmTitle = that.attr('data-confirm-title');
            var confirm_n = that.attr('data-confirm-n');
            var confirm_y = that.attr('data-confirm-y');
            var async = (that.get(0).className.indexOf('async') > -1) ? ' async' : '';
            var html = '<section id="popupContent"><p style="min-height: 60px;">' + confirm + '</p>';
            if (confirm_y != undefined) html += '<input type="submit" id="submit" name="submit" value="' + confirm_y + '" class="submit action right confirm' + async + '" />';
            if (confirm_n != undefined) html += '<input type="submit" id="cancel" name="cancel" value="' + confirm_n + '" class="submit right closelayer' + async + '" />';
            html += '</section>';
            mediakiwi.openLayer(null, that, 'width:400px,height:200px,scrolling:true,iframe:false', confirmTitle, html);
            return false;
        });
        $('.Small').click(function () {
            var that = $(this);
            var h = that.attr('data-height');
            if (h == undefined)
                h = '414';
            mediakiwi.openLayer(that.attr('href'), that, 'width:790px,height:' + h + 'px,scrolling:true,iframe:true', that.attr('title'));
            return false;
        });
        $('.Normal').click(function () {
            var that = $(this);
            mediakiwi.openLayer(that.attr('href'), that, 'width:790px,height:90%,scrolling:true,iframe:true', that.attr('title'));
            return false;
        });
        $('.Tiny').click(function () {
            var that = $(this);
            var h = that.attr('data-height');
            if (h == undefined)
                h = '314';
            mediakiwi.openLayer(that.attr('href'), that, 'width:472px,height:' + h + 'px,scrolling:false,iframe:true', that.attr('title'));
            return false;
        });

        $('.atext').each(function () {
        }).on("change", function (e) {
            var elm = $(this);

            var data = $('form').serialize() + "&async_query=" + elm.val() + "&async=" + elm.attr('id');

            $.ajax({
                type: "POST",
                url: getUrl(document.forms[0].action, "json", "1"),
                data: data,
                dataType: 'json',
                success: function (data) {
                    elm.Data = data;
                    //  Real change so clear
                    if (elm.Data.Reset != undefined)
                        jQuery.each(elm.Reset, function (i, val) {
                            var item = $('#' + val);
                            item.val('');
                            item.select2('data', { id: null, text: null })
                        });
                    if (elm.Data.CallBack != undefined)
                        mediakiwi.callBack(elm.Data.CallBack, elm.val(), elm.Data);
                }
            });
        });


        $('select.styled').each(function () {
            var elm = $(this);
            var count = elm.attr('data-max');
            elm.select2({
                allowClear: true,
                placeholder: '',
                maximumSelectionLength: count,
                escapeMarkup: function (m) { return m; }, // we do not want to escape markup since we are displaying html 

            }).on("change", function (e) {
                var reset = $(this).attr('data-reset');
                if (reset != undefined) {
                    jQuery.each(reset.split(','), function (i, val) {
                        var item = $('#' + val);
                        item.prop('cacheDataSource', []);
                        item.val('');
                        item.select2('data', { id: null, text: null })
                    })
                }
            });
        });
        function formatRepo(repo) {
            if (repo.loading) return repo.text;

            var markup = "<div class='select2-result-repository clearfix'>" +
              "<div class='select2-result-repository__avatar'><img src='" + repo.owner.avatar_url + "' /></div>" +
              "<div class='select2-result-repository__meta'>" +
                "<div class='select2-result-repository__title'>" + repo.full_name + "</div>";

            if (repo.description) {
                markup += "<div class='select2-result-repository__description'>" + repo.description + "</div>";
            }

            markup += "<div class='select2-result-repository__statistics'>" +
              "<div class='select2-result-repository__forks'><i class='fa fa-flash'></i> " + repo.forks_count + " Forks</div>" +
              "<div class='select2-result-repository__stargazers'><i class='fa fa-star'></i> " + repo.stargazers_count + " Stars</div>" +
              "<div class='select2-result-repository__watchers'><i class='fa fa-eye'></i> " + repo.watchers_count + " Watchers</div>" +
            "</div>" +
            "</div></div>";

            return markup;
        }

        function formatRepoSelection(repo) {
            return repo.full_name || repo.text;
        }


        $('.aselect').each(function () {
            var elm = $(this)
            var self = this;
            var placeholder = elm.attr('data-ph');
            if (placeholder == undefined)
                placeholder = ' ';
            var id = elm.attr('id');
            var timer;
            var xhr;
            var postUrl = getUrl(document.forms[0].action, "json", "1");

            elm.select2({
                ajax: {
                    language: 'nl',
                    url: postUrl,
                    dataType: "json",
                    data: function (params) {
                        var data = $('form').serialize();

                        var returnObj = {
                            async: elm.attr('id'),
                            async_query: (params.term != null) ? params.term.toLowerCase() : ""
                        };
                        var items = $('form').serialize().split('&');
                        for (var i = 0; i < items.length; i++) {
                            var part = items[i];

                            var parts = part.split('=');
                            returnObj[parts[0]] = parts[1];
                        }
                        return returnObj;
                    },
                    processResults: function (data, params) {

                        if (data.Reset != undefined) {
                            elm.Reset = data.Reset;
                        }
                        else
                            elm.Reset = null;

                        var transformedData = [];
                        for (var i in data.Result) {
                            var item = data.Result[i];
                            transformedData.push({
                                id: item.Value,
                                text: item.Text
                            });
                        }
                        return {
                            results: transformedData
                        };
                    },
                    cache: false
                },
                placeholder: placeholder,
                allowClear: true,
                minimumInputLength: elm.attr('data-len'),
                formatNoMatches: function () { return "Geen resultaten gevonden"; },
                formatInputTooShort: function (input, min) { var n = min - input.length; return "Vul nog " + n + " karakter" + (n == 1 ? "" : "s") + " in"; },
                formatInputTooLong: function (input, max) { var n = input.length - max; return "Haal " + n + " karakter" + (n == 1 ? "" : "s") + " weg"; },
                formatSelectionTooBig: function (limit) { return "Maximaal " + limit + " item" + (limit == 1 ? "" : "s") + " toegestaan"; },
                formatLoadMore: function (pageNumber) { return "Meer resultaten laden…"; },
                formatSearching: function () { return "Zoeken…"; },

                escapeMarkup: function (m) { return m; }, // we do not want to escape markup since we are displaying html 
            }).on("change", function (e) {
                if (elm.Reset != null) {
                    jQuery.each(elm.Reset, function (i, val) {
                        var item = $('#' + val);
                        //console.log("Reset  " + val);
                        mutePostbackEvent += 1;
                        item.val(null).trigger('change');
                    });
                }
                if (elm.Data != null) {
                    if (elm.Data.CallBack != undefined)
                        mediakiwi.callBack(elm.Data.CallBack, value, elm.Data);
                    if (elm.Data.OnSelectCallBack) {
                        var data = $('form').serialize() + "&async_rid=" + e.val + "&async=" + elm.attr('id');
                        xhr = $.ajax({
                            type: "POST",
                            url: getUrl(document.forms[0].action, "json", "1"),
                            data: data,
                            dataType: 'json',
                            success: function (data) {
                                if (data.CallBack != undefined)
                                    mediakiwi.callBack(data.CallBack, e.val, data);
                            }
                        });
                    }
                }
            });



            self.cacheDataSource = [];


        });

    });
})