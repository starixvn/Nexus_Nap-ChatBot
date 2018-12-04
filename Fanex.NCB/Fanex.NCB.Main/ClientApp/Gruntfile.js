var webpack = require("webpack");
var webpackDevelopment = require("./webpackconfig/build/webpack.dev.conf.js");
var webpackProduction = require("./webpackconfig/build/webpack.pro.conf.js");

var reactDevelopment = require("./webpackconfig/build/webpack.react.dev.conf.js");
var reactProduction = require("./webpackconfig/build/webpack.react.pro.conf.js");

module.exports = function (grunt) {
    grunt.file.defaultEncoding = "utf-8";
    grunt.file.preserveBOM = true;

    var configs = grunt.file.readJSON('GruntFilters.json');
    var jsFilters = configs.jsFilters;

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),              
        webpack: {
            dev: webpackDevelopment,
            pro: webpackProduction,
            both: [webpackDevelopment, webpackProduction],
            reactdev: reactDevelopment,
            reactpro: reactProduction,
            reactboth: [reactDevelopment, reactProduction]
        }
    });

    grunt.loadNpmTasks('grunt-webpack');
};