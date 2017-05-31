'use strict';
var Generator = require('yeoman-generator');
var chalk = require('chalk');
var yosay = require('yosay');

module.exports = Generator.extend({
  prompting: function () {
    this.log(yosay(
      'Welcome to the ' + chalk.red('Kasbah web application') + ' generator!'
    ));

    var prompts = [{
      name: 'namespace',
      message: 'What will be the root namespace of this application?',
      default: 'KasbahWebApplication'
    },
    {
      name: 'alias',
      message: 'What will be the short-name alias application?',
      default: 'kasbahweb'
    }];

    return this.prompt(prompts).then(function (props) {
      // To access props later use this.props.someAnswer;
      this.props = props;
    }.bind(this));
  },

  writing: function () {
    this.fs.copyTpl(this.templatePath('src/Shared/**/*'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace), this.props);
    this.fs.copyTpl(this.templatePath('src/ContentDelivery/**/*'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace + '.ContentDelivery'), this.props);
    this.fs.copyTpl(this.templatePath('src/ContentManagement/**/*'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace + '.ContentManagement'), this.props);
    this.fs.copyTpl(this.templatePath('.vscode/**/*'), this.destinationPath(this.props.alias + '/.vscode/'), this.props);
    this.fs.copy(this.templatePath('.gitignore'), this.destinationPath(this.props.alias + '/.gitignore'));
    this.fs.copyTpl(this.templatePath('Shared.csproj'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace + '/' + this.props.namespace + '.csproj'), this.props);
    this.fs.copyTpl(this.templatePath('ContentDelivery.csproj'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace + '.ContentDelivery/' + this.props.namespace + '.ContentDelivery.csproj'), this.props);
    this.fs.copyTpl(this.templatePath('ContentManagement.csproj'), this.destinationPath(this.props.alias + '/src/' + this.props.namespace + '.ContentManagement/' + this.props.namespace + '.ContentManagement.csproj'), this.props);
    this.fs.copyTpl(this.templatePath('Solution.sln'), this.destinationPath(this.props.alias + '/' + this.props.namespace + '.sln'), this.props);
    // TODO: remove this once the package is released on NuGet
    this.fs.copy(this.templatePath('NuGet.config'), this.destinationPath(this.props.alias + '/NuGet.config'));
  },

  install: function () {
    this.spawnCommand('dotnet', ['restore', this.props.alias]);
  }
});
