module.exports = {
  default: {
    format: ['progress-bar', 'html:reports/cucumber.html'],
    paths: ['features/**/*.feature'],
    require: ['steps.js'],
    timeout: 30000
  }
};
