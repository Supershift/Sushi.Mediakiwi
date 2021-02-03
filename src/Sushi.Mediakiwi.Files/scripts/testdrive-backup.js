var uri = null;

function StartLayer(datatarget, url, title) {
    $('div#loader').fadeOut("fast");
    if (url == '#') return;
    switch (datatarget) {
        case "Normal":
            $.colorbox({ href: url, title: title, iframe: true, fixed: true, width: "864px", height: "614px", transition: "none", scrolling: true });
            break;
        case "Small":
            $.colorbox({ href: url, title: title, iframe: true, fixed: true, width: "760px", height: "414px", transition: "none", scrolling: true });
            break;
        case "Tiny":
            $.colorbox({ href: url, title: title, iframe: true, fixed: true, width: "472px", height: "314px", transition: "none", scrolling: false });
            break;
    }
}

function SetClick(self) {
    /* for table cells that should not be clickable */
    if ($(self).attr('class') == 'nopt')
        return;

    var id = $(self).parent('tr').attr('id');
    if (id != null) {
        $("div#loader").fadeIn(3000);
        var spli = id.split('_')[1].split('$');

        var datalink = $(self).parent('tr').attr('data-link');
     
        var url = '';
        if (datalink != null) {
            url = document.URL.split('?')[0] + '?' + datalink + '=' + spli[1];
        }
        else {
            var group = getUrlVars()["group"];
            if (group != undefined) {
                var groupitem = getUrlVars()["groupitem"];
                url = document.URL.split('?')[0] + '?group=' + group + '&groupitem=' + groupitem + '&list=' + spli[0] + '&item=' + spli[1];
            }
            else
                url = document.URL.split('?')[0] + '?list=' + spli[0] + '&item=' + spli[1];
        }
        var datatarget = $(self).parent('tr').attr('data-target');
        if (datatarget != null) {

            if (url.indexOf('openinframe') == -1) {
                if (url.indexOf('?') > -1)
                    url += '&openinframe=2';
                else
                    url += '?openinframe=2';
            }

            StartLayer(datatarget, url);
            return;
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
        $(self).parent('tr').attr('class', 'parent selected');

        if (sel != null && idv != null) {
            var ref = getUrlVars()["referid"];
            /* Remove the _ at the begin */
            var put = ref.substring(1, ref.length) + '$' + idv;
            var pnt = '#' + ref;
            var loc = parent.$(pnt).attr('class').indexOf('single');
            if (loc > -1) parent.$(pnt + ' li').remove();
            parent.$(pnt).append('<li class="ui-state-default">' + sel + '<figure class="flaticon solid x-1 icon check del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');
            if (loc > -1) parent.$.fn.colorbox.close();
        }
    }
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

$(document).ready(function () {
    $(".numeric").numeric({ negative: false });

    $('#ButtonExport').bind('click', function () {
        var frm = $(document.forms[0]);
        var dat = frm.serializeArray();
        var url = '?xls=1&xp=1&list=' + getUrlVars()["q"];
        $("div#loader").fadeIn();
        $.post(url, dat, function (data) {
            //location.href = data;
            console.log(data);
            top.CloseLayer(data);
        })
          .always(function () {
              $("div#loader").stop();
              $('div#loader').fadeOut("fast");
              //top.$.colorbox.close();
              return false;
          });
    });
    $('#ButtonCancel').bind('click', function () {
        top.CloseLayer();
    });

    $('.uploadFile #File').bind('change', function () {
        $('#autopostback').val('save');
        $("div#loader").fadeIn(1000);
        document.forms[0].submit();
    });


    $('#export').bind('change', function () {
        if ($(this).val() != '') {
            $.colorbox({ iframe: true, transition: "none", width: '425px', opacity: '0.6', height: '226px', scrolling: false, fixed: true, href: "?list=161&openinframe=2&q=" + getUrlVars()["list"] });
            $(this).val('');
        }
    });

    $('#toggle').bind('click', function () {
        var sideNav = $('#sideNav');
        if (sideNav.is(':hidden')) {
            $('#homeArticle').animate({                 
                paddingLeft: "250px",zIndex: "1"
            }, 600,null,
                function () {
                    $('#sideNav').css('z-index','10');
                }
            );
            $('#toggle').addClass('arrowU');
            $('#toggle').removeClass('arrowD');
            sideNav.show();
        }
        else {
            $('#homeArticle').animate({                 
                paddingLeft: "30px", zIndex: "0"
            }, 600,null,
                function () {
                    $('#sideNav').css('z-index', '0');
                }
            );
            $('#toggle').addClass('arrowD');
            $('#toggle').removeClass('arrowU');
            sideNav.hide();
        }

    });

    /*$(function () {
        $('#tabsflow').scroller({
            proportion: 0.92,

            html: {
                buttonLeft: 'a',
                buttonRight: 'b'
            }

        });
        $('.scroller-button').css({ border: 0 });
        $('#tabsFlow').scroller({
            proportion: 0.92,

            html: {
                buttonLeft: '<',
                buttonRight: '>'
            }

        });
        $('.scroller-button').css({ border: 0 });
        $('#fix').click(function () {
            $('#tabsflow').scroller('repositionButton');
        });
    });*/
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
});

function CloseLayer(u) {
    top.$.colorbox.close();
    if (u != null) location.href = u;
}

//var dataModel = function () {

//    this.items = ko.observableArray([]);
//    this.total = ko.observableArray([]);
//    this.pages = ko.observableArray([]);
//    this.all = ko.observable(true);
//    this.set = ko.observable(true);
//    this.max = ko.observable(true);
//    this.nfo = ko.observable(true);
//    this.Max = 0;
//    this.url = null;
//    var self = this;

//    $('.pager').bind('click', function () {
//        var page = $(this).html();
//        if (page == '&lt;') page = parseInt($('#set').val()) - 1;
//        else if (page == '&gt;') page = parseInt($('#set').val()) + 1;
//        else page = parseInt(page);

//        uri = window.location.toString().split('?')[0] + '?';
//        var ps = window.location.search.split(/\?|&/);
//        for (var i = 0; i < ps.length; i++) {
//            if (ps[i]) {
//                var p = ps[i].split(/=/);
//                if (p[0] == 'set') p[1] = page;
//                uri += (i == 1 ? '' : '&') + p[0] + '=' + p[1];
//            }
//        }
//        uri += '&JSON=1';
//        if (self.Max < page)
//            $('#set').val(self.Max);
//        else if (page < 1)
//            $('#set').val(1);
//        else {
//            $('#set').val(page);
//            self.load();
//        }
//        return false;
//    });

//    this.load = function () {
//        self.url = uri;
//        if (self.url == null) return;
//        $("div#loader").fadeIn(1000);
//        var frm = $(document.forms[0]);
//        var dat = frm.serializeArray();
//        $.post(self.url, dat, function (data) {
//            if (self.items().length > 0) self.items.removeAll();

//            if (data.NO_ACCESS == '1')
//                return;

//            if (data.Rows == undefined)
//                return;

//            for (var i = 0; i < data.Rows.length; i++)
//                self.items.push(data.Rows[i]);

//            self.total.removeAll();
//            if (data.Sum != null)
//                self.total.push(data.Sum);

//            if (data.Pages != null) {
//                if (self.pages().length > 0) self.pages.removeAll();
//                for (var i = 0; i < data.Pages.length; i++)
//                    self.pages.push(data.Pages[i]);
//            }

//            self.all(data.All);
//            self.set(data.Set);
//            self.max(data.Max);
//            self.Max = data.Max;

//            var setT = (data.Set * data.Lst);
//            var setF = (setT - data.Lst) + 1;
//            if (setT > data.Tot) setT = data.Tot;
//            self.nfo('Showing ' + setF + ' - ' + setT + ' of ' + data.Tot + ' items');

//            if (data.Max == '1') $(".footer ul li.last").hide();
//            else $(".footer ul li.last").show();

//            self.items().length == 0
//                ? $('#grid').hide()
//                : $('#grid').show();

//            $('.dataBlock table td').click(function () {
//                SetClick(this);
//            });

//            $(".dataBlock table tr td a").each(function () {
//                $(this).parent().addClass("nopt");
//            });
//        })
//        .fail(function () {
//            var note = '<article class="error">Something went wrong while fetching the data, please review the notification log for the occurred error.</article>';
//            $('#gridsection').html(($('#gridsection').html() + note));
//        })
//        .always(function () {
//            $("div#loader").stop();
//            $('div#loader').fadeOut("fast");
//        });
//        ;
//    }
//};

//ko.bindingHandlers.stripe = {
//    update: function (element, valueAccessor, allBindingsAccessor) {
//        var value = ko.utils.unwrapObservable(valueAccessor()); //creates the dependency
//        var allBindings = allBindingsAccessor();
//        var even = allBindings.evenClass;
//        var odd = allBindings.oddClass;
//        //update odd rows
//        $(element).children(":nth-child(odd)").addClass(odd).removeClass(even);
//        //update even rows
//        $(element).children(":nth-child(even)").addClass(even).removeClass(odd);
//    }
//}

    
    $('.alert').each(function () {
        $(this).hide();
        $.ambiance({ message: $(this).html(), timeout: 5 });
    });

    $(".ui-tabs-nav").find(".ui-tabs-selected")


    /*Multi selection*/
    //$(".multiselect").multiselect();

    //$(".connectedSortable").sortable({
    //    connectWith: ".connectedSortable",
    //    placeholder: "ui-state-highlight"
    //}).disableSelection();

    //$('.multiSortable li figure.del').live('click', function () {
     //   var liParent = $(this).parent();
     //   var litem = liParent.clone();
     //   liParent.remove();
    //});

    /*Date/timepicker*/
    $(".datepicker").datepicker();
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

    $(".pop").colorbox({ iframe: true, fixed: true, opacity: '0.6', transition: "none", scrolling: false });
    $('.pop').bind('click', function () {
        $(this).colorbox($.extend({}, getParameters(this)));
    });


    $('.Normal').bind('click', function () {
        StartLayer('Normal', $(this).attr('href'), $(this).attr('title'));
        return false;
    });
    $('.Small').bind('click', function () {
        StartLayer('Small', $(this).attr('href'), $(this).attr('title'));
        return false;
    });
    $('.Tiny').bind('click', function () {
        StartLayer('Tiny', $(this).attr('href'), $(this).attr('title'));
        return false;
    });
    //$(".Normal").colorbox({ iframe: true, fixed: true, width: "864px", height: "614px", transition: "none", scrolling: true });
    //$(".Small").colorbox({ iframe: true, fixed: true, width: "760px", height: "414px", transition: "none", scrolling: true });
    //$(".Tiny").colorbox({ iframe: true, fixed: true, width: "472px", height: "314px", transition: "none", scrolling: false });
    $(".pop2").colorbox({ iframe: true, fixed: true, width: "760px", height: "414px", transition: "none", scrolling: true });

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
    $('input[type=text]').keypress(function (e) {
        if (e.which == 13) {
            document.forms[0].submit();
        }
    });

    $('.postBack').click(function () {
        var tag = $(this).get(0).tagName.toLowerCase();
        var type = $(this).get(0).type.toLowerCase();
        if (tag == 'select' || tag == 'text' || (tag == 'input' && type != 'submit')) return;
        $('#autopostback').val($(this).attr('id'));
        $("div#loader").fadeIn(1000);
        document.forms[0].submit();
    });
    $('.postBack').change(function () {
        console.log('test')
        var tag = $(this).get(0).tagName.toLowerCase();
        if (tag != 'select' && tag != 'text' && tag != 'input') return;
        $('#autopostback').val($(this).attr('id'));
        $("div#loader").fadeIn(1000);
        document.forms[0].submit();
    });

    $('.postparent').each(function () {
        var idv = $(this).attr('id');
        var sel = $(this).attr('value');
        var val = idv + '|' + sel;
        var ref = getUrlVars()["referid"];
        /* Remove the _ at the begin */
        //var put = ref + '$' + idv;
        var put = ref.substring(1, ref.length) + '$' + idv;
        var pnt = '#' + ref;
        var loc = parent.$(pnt).attr('class').indexOf('single');
        if (loc > -1) parent.$(pnt + ' li').remove();
        parent.$(pnt).append('<li class="ui-state-default">' + sel + '<figure class="flaticon solid x-1 icon check del"> <input type="hidden" id="' + put + '" name="' + put + '" value="' + val + '" /> </li>');
        if (loc > -1) parent.$.fn.colorbox.close();
    });

    $('.closeLayer').each(function () {
        parent.$.fn.colorbox.close();
    });
    $('.postParent').each(function () {
        parent.$.fn.colorbox.close();
        $("div#loader").fadeIn(3000);
        parent.document.forms[0].submit();
    });


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

    //  Part of sortable
    var sortableIsAlive = false;
    $('#sortOrder').click(function () {
        if ($(this).attr('class').indexOf('repeat-4') > -1) {
            if (sortableIsAlive)
                $(".dataBlock table tbody").sortable("enable");
            else {
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
            $(this).attr('class', $(this).attr('class').replace("repeat-4", "x-2"));
            $('.dataBlock table td').off('click');
            $.ambiance({ message: 'Sorteren staat aan' });
         
        } else {
            $(".dataBlock table tbody").sortable("disable");
            $(this).attr('class', $(this).attr('class').replace("x-2", "repeat-4"));
            $('.dataBlock table td').click(function () {
                SetClick(this);
            });
            $.ambiance({ message: 'Sorteren staat uit.' });
        }
        return false;
    });

    $('.dataBlock table td').click(function () {
        SetClick(this);
    });

    // Return a helper with preserved width of cells
    var fixHelper = function (e, ui) {
        ui.children().each(function () {
            $(this).width($(this).width());
        });
        return ui;
    };

    /*$(".sortable tbody").sortable({
         helper: fixHelper
     }).disableSelection();*/

    $(function () {
        $("abbr").tipTip({ maxWidth: "auto", edgeOffset: 10 });
    });

    //$("#sortable1, #sortable2, #sortable3, #sortable4, #sortable5, #sortable6").sortable({
    //    connectWith: ".connectedSortable",
    //    placeholder: "ui-state-highlight"
    //}).disableSelection();

    //$("#sortable").sortable({
    //    connectWith: ".connectedSortable",
    //    placeholder: "sorttable-highlight"
    //}).disableSelection();
    
    /*
    "ui-state-highlight"
    $('#slims').slimScroll({
        height: '220px'
    });*/

    //var list = new dataModel();
    //ko.applyBindings(list);

    ////  Knockout start
    //list.load();