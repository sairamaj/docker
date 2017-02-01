var colors = require('colors')
const commandLineArgs = require('command-line-args')

colors.setTheme({
    silly: 'rainbow',
    input: 'grey',
    verbose: 'cyan',
    prompt: 'grey',
    info: 'green',
    data: 'grey',
    help: 'cyan',
    warn: 'yellow',
    debug: 'blue',
    error: 'red'
});

/*
    utility functions.
*/

/*
    c# like string.Format
*/
stringFormat = function (format) {
    var args = Array.prototype.slice.call(arguments, 1);
    return format.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined'
            ? args[number]
            : match
            ;
    });
};

/*
    removes namespaces prefixes.
*/
function removeNamespaces(input) {
    return input.replace(/<(\/?)([^:>\s]*:)?([^>]+)>/g, "<$1$3>")
}

/*
    extracts value given object and its path.
*/
function objectResolve(path, obj) {
    return path.split('.').reduce(function (prev, curr) {
        return prev ? prev[curr] : undefined
    }, obj || self)
}

function logError(msg) {
    console.log(msg.error);
}

function logSuccess(msg) {
    console.log(msg.info);
}

function logIgnore(msg) {
    console.log(msg.gray);
}

function logWarn(msg) {
    console.log(msg.warn)
}
/*
    Command line parses and returns options object.
*/
function parseCommandLine() {
    const optionDefinitions = [
        { name: 'server', type: String },
        { name: 'testfile', alias: 't', type: String },
        { name: "outfile", alias: 'o', type: String }
    ]

    try {
        const options = commandLineArgs(optionDefinitions)
        if (options.server == undefined || options.testfile == undefined) {
            printUsage()
            process.exit(-1)
        }

        if( options.testfile.endsWith("json")){
            options.fileType = "json"
        }else{
            options.fileType = "yaml"
        }

        return options
    } catch (err) {
        console.log(err)
        printUsage()
        process.exit(-1)
    }
}

/*
    prints usage.
*/
function printUsage() {
    console.log("usage node app.js --server <server> --testfile <yamltestfile/jsonfile> [ -outfile <report file>]".help)
}


module.exports.stringFormat = stringFormat
module.exports.removeNamespaces = removeNamespaces
module.exports.objectResolve = objectResolve
module.exports.logError = logError
module.exports.logSuccess = logSuccess
module.exports.logIgnore = logIgnore
module.exports.parseCommandLine = parseCommandLine
module.exports.logWarn = logWarn
