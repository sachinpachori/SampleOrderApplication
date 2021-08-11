//Path to CloudFormation SAM template
const templatefilePath = './Sample.Order.BE.Api/serverless.template';

// Transform template to set code uri to expected publish folder
var fs = require('fs');
var template = fs.readFileSync(templatefilePath, 'utf8');
template = template.replace(/"CodeUri": "",/g, '"CodeUri": "bin/Release/netcoreapp3.1/publish/",');
fs.writeFileSync(templatefilePath, template);