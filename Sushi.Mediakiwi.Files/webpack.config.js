/// <binding ProjectOpened='Watch - Development' />

var path = require('path');
var webpack = require('webpack');
const TerserPlugin = require('terser-webpack-plugin');
var spreadPlugin = require('babel-plugin-transform-object-rest-spread');
const VueLoaderPlugin = require('vue-loader/lib/plugin');

// All files to compile
const vueBundles = [
    { source: './app/main.js', target: "./app/dist/bundle.js", targetMin: "./app/dist/bundle.min.js" },
];

const minify = [
];

// Base configuration of the webpack jobs
var config = {
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    loaders: {
                    }
                    // other vue-loader options go here
                }
            },
            {
                test: /\.js$/,
                loader: 'babel-loader',
                exclude: /node_modules/
            },
            {
                test: /\.css$/,
                use: [
                    {
                        loader: 'vue-style-loader'
                    },
                    {
                        loader: 'css-loader',
                        options: {
                            modules: true,
                            localIdentName: '[local]'
                        }
                    }
                ]
            },
            {
                test: /\.(ttf|otf|eot|woff|woff2)$/,
                use: {
                    loader: "file-loader",
                    options: {
                        name: 'fonts/[name].[ext]?[hash]'
                    },
                },
            },
            {
                test: /\.(png|jpg|gif|svg)$/,
                use: {
                    loader: "file-loader",
                    options: {
                        name: 'images/[name].[ext]?[hash]'
                    },
                },
            }
        ]
    },
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        },
        extensions: ['*', '.js', '.vue', '.json']
    },
    devServer: {
        historyApiFallback: true,
        noInfo: true,
        overlay: true
    },
    performance: {
        hints: false
    },
    devtool: '#source-map',
    plugins: [
        new VueLoaderPlugin()
    ]
};

// define an [] for the apps
let apps = [];

// Loop through all entries
for (let _entry of vueBundles) {

    // add a config for the entries to the app array in development mode
    apps.push(Object.assign({}, config, {
        entry: _entry.source,
        output: {
            path: path.resolve(__dirname),
            filename: _entry.target,
        },
        devtool: '#source-map',
        mode: 'development',
    }));

    // add a config for the entries to the app array in production mode
    apps.push(Object.assign({}, config, {
        entry: _entry.source,
        output: {
            path: path.resolve(__dirname),
            filename: _entry.targetMin,
        },
        mode: 'production',
    }));
}

// Loop through all entries
for (let __entry of minify) {
    // add a config for the entries to the app array in production mode
    apps.push(Object.assign({}, config, {
        entry: __entry.source,
        output: {
            path: path.resolve(__dirname),
            filename: __entry.target,
        },
        mode: 'production',
    }));
}

// Return Array of Configurations 
module.exports = apps;
