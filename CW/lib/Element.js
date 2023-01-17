const {
  getExpRandom,
  getUniformRandom,
  getGammaRandom,
  getPoissonRandom,
} = require("./common.js");

module.exports = class Element {
  constructor(props) {
    this.name = props.name;
    this.delay = props.delay || null;
    this.low = props.low || null;
    this.high = props.high || null;
    this.shape = props.shape || null;
    this.randType = props.randType;

    this.nextEventTime = Infinity;
    this.currentTime = this.nextEventTime;
    this.state = 0;
    this.failure = 0;
    this.quantity = 0;
    this.succeeded = 0;
  }
  inAct() {
    this.quantity++;
  }
  outAct() {
    this.system.stateLog(this.name, 0, this.quantity);
    this.succeeded++;
  }
  getRandom() {
    switch (this.randType) {
      case "exp":
        return getExpRandom(this.delay);
        break;
      case "uniform":
        return getUniformRandom(this.low, this.high);
        break;
      case "gamma":
        return getGammaRandom(this.delay, this.shape);
        break;
      case "poisson":
        return getPoissonRandom(this.delay);
        break;
    }
  }
};
