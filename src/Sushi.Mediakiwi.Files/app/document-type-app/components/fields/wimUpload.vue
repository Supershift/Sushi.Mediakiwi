<template>
    <div :class="classname">
        <label v-if="prefix(field)" v-html="prefix(field)"></label>
        <div class="multiSortable select">
            <input type="hidden" name="LogoAssetID" value="_MK$PH_">
            <ul id="_LogoAssetID" class="single  add ">
                <li class="instant">
                    <a class="openlayer" data-layer="width:790px,height:350px,iframe:true,scrolling:true" title="Documents"
                       href="//portal-tst.genbadigital.io//2/repository/wim/portal.ashx?list=7&amp;openinframe=2&amp;referid=_LogoAssetID&amp;gallery=/&amp;item=16">
                        supershift_280_60 <span>(280px / 60px)</span>
                    </a><figure class="icon-x del"></figure>
                    <input type="hidden" id="LogoAssetID$16$0" name="LogoAssetID$16$0" value="16|supershift_280_60 (280px / 60px)">
                </li>
            </ul>
        </div>
        <div class="buttonContainer">
            <a class="openlayer" data-layer="width:790px,height:350px,iframe:true,scrolling:true" href="//portal-tst.genbadigital.io//2/repository/wim/portal.ashx?list=7&amp;openinframe=2&amp;referid=_LogoAssetID&amp;gallery=/&amp;item=0" title="Documents"><figure class="icon-plus"></figure></a>
        </div>
        <div class="clear"></div>


        <label v-if="suffix(field)" v-html="suffix(field)"></label>
    </div>
</template>
<script>
    import { shared } from './input';

    export default {
        props: ['field', 'classname'],
        mixins: [shared],
        data() {
            return {

            }
        },
        mounted: function () {

        },
        watch: {

        },
        methods: {



        },
        destroyed: function () {

        }
    }
</script>
