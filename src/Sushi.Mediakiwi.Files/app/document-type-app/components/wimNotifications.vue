<template>
    <v-container>
        <article class="error" v-show="errorList.length">
            <div class="content">
                <div v-for="error in errorList" v-html="error.message"></div>
            </div>
        </article>
        <article class="note" v-show="notificationList.length">
            <div class="content">
                <div v-for="notification in notificationList" v-html="notification.message"></div>
            </div>
        </article>
    </v-container>
</template>
<script>
    export default {
        props: ['notifications'],
        computed: {
            notificationList() {
                if (this.notifications && this.notifications.length)
                    return this.notifications.filter(notification => !notification.isError);
                return [];
            },
            errorList() {
                if (this.notifications && this.notifications.length)
                    return this.notifications.filter(notification => notification.isError);
                return [];
            },
        }
    }
</script>