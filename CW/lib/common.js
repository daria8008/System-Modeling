let jStat = require('jstat');
// Exponential distribution
module.exports.getExpRandom = (lambda) => {
    return Math.log(Math.random()) * (1 / lambda) * (-1);
}
// Uniform distribution
module.exports.getUniformRandom = (low, high) => {
  return jStat.normal.sample(low, high);
}

// Poisson distribution
module.exports.getPoissonRandom = (lambda) => {
  return jStat.poisson.sample(lambda);
};