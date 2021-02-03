<template>
    <section id="datagrid" class="searchTable">        
        <article class="dataBlock">
            <table class="selections">
                <thead>
                    <tr class="first">
                        <th v-for="column in grid.columns"
                            v-html="column.title"
                            v-bind:width="column.width"
                            :class="alignmentClass(column)"></th>
                    </tr> 
                </thead>
                <tbody>
                    <tr class="parent hand" v-for="row in grid.rows" :id="row.rowID">
                        <td v-for="gridItem in row.gridItems"
                            :class="alignmentClass(grid.columns[gridItem.column])">                            
                            <component :is="gridItem.vueType"
                                       :value="gridItem.value"></component>
                        </td>
                    </tr>
                </tbody>
            </table>
            <menu class="pager">
                <li>
                    {{grid.rows.length}} results
                            </li>
                            </menu>
                            <br class="clear">
</article>
    </section>

</template>
<script>
    import { shared } from './../shared';
    import wimLabel from './columns/wimLabel.vue';
    import wimButton from './columns/wimButton.vue';
    import wimIcon from './columns/wimIcon.vue';

    export default {
        props: ["grid"],
        mixins: [shared],
        components: {
            wimLabel,
            wimButton,
            wimIcon,
        },
        methods: {
            alignmentClass: function (column) {
                if (column.align === 1)
                    return 'txt-l';
                else if (column.align === 2)
                    return 'txt-c';
                else if (column.align === 3)
                    return 'txt-r';
            },
        },
        computed: {

        }
    }
</script>