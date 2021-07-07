$(document).ready(function () {
    //Examples of how to assign the ColorBox event to elements
    $(".popNew").colorbox({ iframe: true, fixed: true, width: "440px", height: "440px", transition: "none", scrolling: true });
    $(".pop").colorbox({ iframe: true, width: "750px", height: "600px", transition: "none", scrolling: false });
    $(".pop1").colorbox({ iframe: true, width: "864px", height: "614px", transition: "none" });
    //$(".pop2").colorbox({ iframe: true, width: "864px", height: "614px", transition: "none", scrolling: true });
    $(".pop3").colorbox({ iframe: true, width: "425px", height: "212px", transition: "none", scrolling: false });
    $(".pop4").colorbox({ iframe: true, width: "425px", height: "246px", transition: "none", scrolling: false });
    $(".pop5").colorbox({ iframe: true, width: "425px", height: "410px", transition: "none", scrolling: false });
    $(".pop6").colorbox({ iframe: true, transition: "none", scrolling: false });
    $(".popSentiment").colorbox({ iframe: false, width: "840px", transition: "none", scrolling: false }).click(function () {
        $('#colorbox').removeClass().addClass('senti');
    });
    $(".popHelp").colorbox({ inline: true, width: "840px", transition: "none", scrolling: false }).click(function () {
        $('#colorbox').removeClass().addClass('senti');
    });
    $(".popConfirm").colorbox({ inline: true, width: "420px", transition: "none", height: "400px", scrolling: false }).click(function () {
        $('#colorbox').removeClass().addClass('senti');
    });
    $(".popConfirms").colorbox({ iframe: true, width: "800px", height: "500px", transition: "none", scrolling: false });
    $("#e1").select2({ containerCssClass: "blue", dropdownCssClass: "blue" });
    $("#e2").select2({ containerCssClass: "big", dropdownCssClass: "big" });
    //$("select.normal").select2({ containerCssClass: "normal", dropdownCssClass: "normal" });
    ////$("select.half").select2();
    //$("select.formField").select2({ containerCssClass: "large", dropdownCssClass: "large" });
    $(".side").hover(function () {

        if ($(this).next().hasClass('activeChannel')) {
            
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
    })
    $('.activeChannel').live("mouseleave", function () {         
            $(this).removeClass('activeChannel').slideUp('fast');    
    });
    $(".toggle").click(function () {
        $(".formfilters").slideToggle("fast");
    });
    /*$('.raters').barrating();*/
    $('.closer').click(function () {
        $('.controls').hide('scale', { percent: 0 }, 600);
    });
    $('.plusBtn').click(function () {
        $('.controls').show('scale', { percent: 100 }, 600);
    });
    $(".open").click(function () {
        if ($(".extra").hasClass('show')) {
            $(".extra").animate({ height: 40 }, 200).removeClass('show');
            $(".extra").animate({ height: 40 }, 200).addClass('hide');
            $(".open").text("More ▲");
        } else {
            $(".extra").animate({ height: 80 }, 200).addClass('show');
            $(".extra").animate({ height: 80 }, 200).removeClass('hide');
            $(".open").text("Less ▼");
        }
    });
    

    //$('#frmDatum').multiDatesPicker();
    $("#sortable").sortable();
    $("#sortable").disableSelection();

    $(".popNew").colorbox({ iframe: true, fixed: true, width: "440px", height: "440px", transition: "none", scrolling: true });
    $(".popUpload").colorbox({ iframe: true, fixed: true, width: "548px", height: "454px", transition: "none", scrolling: true });
    $(".popConfirm").colorbox({ iframe: false, fixed: true, width: "448px", height: "424px", transition: "none", scrolling: true });
    $(".popImage").colorbox({ iframe: true, fixed: true, width: "624px", height: "784px", transition: "none", scrolling: true });
    $(".popMulti").colorbox({ iframe: true, fixed: true, width: "550px", height: "484px", transition: "none", scrolling: true });
    $(".popPlan").colorbox({ iframe: true, fixed: true, width: "1100px",height: "830px", transition: "none", scrolling: true });
    $(".popLayer").colorbox({ transition: "none" });
    $(".pop2").colorbox({ iframe: true, fixed: true, width: "760px", height: "414px", transition: "none", scrolling: true });

    $('.tags_3').tagsInput({
        width: 'auto', height: '30px', 'autocomplete': {'1' : 'fet', '2': 'fff' },
    });

});



