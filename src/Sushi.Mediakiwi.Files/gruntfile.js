
/// <binding ProjectOpened='watch' />
module.exports = function (grunt) {
    grunt.loadNpmTasks("grunt-contrib-less");
    grunt.loadNpmTasks("grunt-contrib-watch");
    grunt.loadNpmTasks('grunt-newer');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-copy');
    grunt.loadNpmTasks('grunt-contrib-concat');

    var src_less = 'styles';
    var dest_less = 'compiled';

    var src_css = 'styles';
    var dest_css = 'compiled';

    var src_js = 'scripts';
    var dest_js = 'compiled';

    grunt.initConfig({
        less: { all: { files: [{ expand: true, cwd: src_less, src: ['*.less'], dest: dest_less, ext: '.css' }] } },
        cssmin: {
            all: {
                files: [
                    { expand: true, cwd: src_css, src: ['*.css', '!*.min.css'], dest: dest_css, ext: '.min.css' },
                    { expand: true, cwd: dest_less, src: ['*.css', '!*.min.css'], dest: dest_css, ext: '.min.css' }
                ]
            }
        },
        uglify: {
            all: {
                files: [
                    { expand: true, cwd: src_js, src: ['*.js', '!*.min.js'], dest: dest_js, ext: '.min.js' }
                ]
            },
            tinyMce: {
                files: [
                    { expand: true, cwd: src_js + "/tinymce/plugins/link", src: ['*.js', '!*.min.js'], dest: src_js + "/tinymce/plugins/link", ext: '.min.js' }
                ]
            }
        },
        copy: {
            all: {
                files: [
                    { cwd: src_js, expand: true, src: ['*.min.js'], dest: 'compiled/' },
                    { cwd: src_css, expand: true, src: ['*.min.css'], dest: 'compiled/' },
                ]
            }
        },
        concat: {
            all: {
                files: [
                    {
                        src: [
                            'compiled/mediakiwiForm.min.css',
                            'compiled/stylesFlatv2.min.css',
                            'compiled/mainMenuFlatv2.min.css',
                            'compiled/fontello.min.css',
                            'compiled/colorbox.min.css',
                            'compiled/solid.min.css',
                            'compiled/formalizev2.min.css',
                            'compiled/jquery-ui-1-8-16-custom.min.css'
                        ], dest: 'compiled/bundel.min.css'
                    },
                    {
                        src: [
                            'compiled/stylesFlatv2.min.css',
                            'compiled/jquery-ui-1-8-17-custom.min.css',
                            'compiled/formalizev2.min.css',
                            'compiled/colorbox.min.css',
                            'compiled/search.min.css',
                        ], dest: 'compiled/bundel_login.min.css'
                    },
                    {
                        src: [
                            'compiled/easyauth.min.js',
                            'compiled/jquery-1-7-1.min.js',
                            'compiled/jquery-ui-1-10-3-custom.min.js',
                            'compiled/jquery-ui-datepicker-nl.min.js',
                            'compiled/jquery-shorten.min.js',
                            'compiled/jquery-tipTip.min.js',
                            'compiled/jquery-slimscroll.min.js',
                            'compiled/jquery-numeric.min.js',
                            'compiled/jquery-formalize.min.js',
                            'compiled//jquery-colorbox.min.js',
                            'compiled/jquery-ui-timepicker-addon.min.js',
                            'compiled/jquery-hoverIntent.min.js',
                            'compiled/jquery-curtainMenu.min.js',
                            'compiled/jquery-ambiance.min.js',
                            'compiled/fixedBar.min.js',
                            'compiled/jquery-nicescroll.min.js',
                            'compiled/testdrivev2.min.js',
                            'compiled/select2.min.js'
                        ], dest: 'compiled/bundel.nl.min.js'
                    },
                    {
                        src: [
                            'compiled/easyauth.min.js',
                            'compiled/jquery-1-7-1.min.js',
                            'compiled/jquery-ui-1-10-3-custom.min.js',
                            'compiled/jquery-ui-datepicker-gb.min.js',
                            'compiled/jquery-shorten.min.js',
                            'compiled/jquery-tipTip.min.js',
                            'compiled/jquery-slimscroll.min.js',
                            'compiled/jquery-numeric.min.js',
                            'compiled/jquery-formalize.min.js',
                            'compiled//jquery-colorbox.min.js',
                            'compiled/jquery-ui-timepicker-addon.min.js',
                            'compiled/jquery-hoverIntent.min.js',
                            'compiled/jquery-curtainMenu.min.js',
                            'compiled/jquery-ambiance.min.js',
                            'compiled/fixedBar.min.js',
                            'compiled/jquery-nicescroll.min.js',
                            'compiled/testdrivev2.min.js',
                            'compiled/select2.min.js'
                        ], dest: 'compiled/bundel.gb.min.js'
                    },
                    {
                        src: [
                            'compiled/easyauth.min.js',
                            'compiled/jquery-1-7-1.min.js',
                            'compiled/jquery-ui-1-10-3-custom.min.js',
                            'compiled/jquery-shorten.min.js',
                            'compiled/jquery-tipTip.min.js',
                            'compiled/jquery-slimscroll.min.js',
                            'compiled/jquery-numeric.min.js',
                            'compiled/jquery-formalize.min.js',
                            'compiled//jquery-colorbox.min.js',
                            'compiled/jquery-ui-timepicker-addon.min.js',
                            'compiled/jquery-hoverIntent.min.js',
                            'compiled/jquery-curtainMenu.min.js',
                            'compiled/jquery-ambiance.min.js',
                            'compiled/fixedBar.min.js',
                            'compiled/jquery-nicescroll.min.js',
                            'compiled/testdrivev2.min.js',
                            'compiled/select2.min.js'
                        ], dest: 'compiled/bundel.en.min.js'

                    },
                    {
                        src: [
                            'compiled/easyauth.min.js',
                            'compiled/jquery-1-7-1.min.js',
                            'compiled/head-load-min.js',
                        ], dest: 'compiled/bundel_login.min.js'
                    }

                ]
            }
        },
        watch: {
            less: { files: src_less + '/*.less', tasks: ['newer:less:all'] },
            cssmin: { files: [dest_less + '/*.css', src_css + '/*.css'], tasks: ['newer:cssmin:all'] },
            uglify: { files: [src_js + '/*.js'], tasks: ['newer:uglify:all'] },
            copy: { files: [src_js + '/*.js', src_css + '*.min.css'], tasks: ['newer:copy:all'] },
            concat: { files: [dest_less + '/*.*'], tasks: ['concat:all'] },

        }
    });
};