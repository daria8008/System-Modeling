const Processor = require("./Processor.js");
const Create = require("./Create.js");
const LineProcess = require("./LineProcess.js");

module.exports = class System {
  constructor(props) {
    this.nextEventTime = 0;
    this.currentEventTime = this.nextEventTime;
    this.currentEvent = 0;
    this.elements = [];
    this.eventsPerTime = [];

    this.cost_1 = props.d1;
    this.optimizationCost_1 = props.z1;
    this.optimizationCount_1 = props.z1Count;
    this.cost_2 = props.d2;
    this.optimizationCost_2 = props.z2;
    this.optimizationCount_2 = props.z2Count;
    this.logs = [];

    this.simulatingTime = 100;
    this.stateLogs = [];
    // TODO System state logger for simulation animation
    this.systemState = {
      create_1: {
        isActive: false,
      },
      create_1: {
        isActive: false,
      },
      process: {
        isActive: false,
      },
      line_1: {
        isActive: false,
      },
      line_2: {
        isActive: false,
      },
    };
  }
  simulate(time) {
    this.simulatingTime = time;
    this.logs = [];
    while (this.currentEventTime < this.simulatingTime) {
      // Нова ітерація. Наступна подія не вибрана
      this.nextEventTime = Infinity;

      // Шукаємо найближчу подію в елементах системи
      this.elements.forEach((element, index) => {
        if (element.nextEventTime < this.nextEventTime) {
          this.nextEventTime = element.nextEventTime;
          this.currentEvent = index;
        }
      });

      // Переходимо в найближчу подію
      this.currentEventTime = this.nextEventTime;
      this.elements.forEach((element) => {
        element.currentTime = this.currentEventTime;
        if (element instanceof Processor) element.calcStats();
        if (element instanceof LineProcess) element.calcStats();
      });

      // Закінчуємо подію
      this.elements[this.currentEvent].outAct();
      // this.calcStats();
    }
    // Статистика
    let logs = { main: [...this.logs], result: this.resultLog() };
    return { logs, stateLogs: this.stateLogs };
  }
  calcStats() {
    let totalEvents = 0;
    this.elements.forEach((el) => {
      if (el instanceof Processor) {
        totalEvents += el.queue;
        if (el.state) totalEvents++;
      }
    });
    this.eventsPerTime.push(totalEvents);
  }
  log(msg) {
    this.logs.push(Array.isArray(msg) ? msg.join(" ") : msg);
  }
  stateLog(name, queue, quantity, failure) {
    this.stateLogs.push({ name, queue, quantity, failure });
  }
  resultLog() {
    let elementsStats = [];
    this.elements.forEach((el) => {
      let stat = {
        name: el.name,
      };
      if (el instanceof Processor || el instanceof LineProcess) {
        stat.quantity = el.quantity;
        stat.succeeded = el.succeeded;
        stat.failure = el.failure;
        stat.maxObservedQueueLength = el.maxObservedQueueLength;
        stat.averageWorkTime = (el.eventSumTime / el.succeeded).toFixed(2);
        stat.averageQueueLength =
          (el.queueLengths.reduce((sum, el) => (sum += el), 0) /
          el.queueLengths.length).toFixed(2);
        stat.failureProbability = (el.failure / el.quantity).toFixed(2);
      } else if (el instanceof Create) {
        stat.eventDelaySum = el.cumulativeEventArrivalPeriods;
        stat.eventDelayAverage = (el.cumulativeEventArrivalPeriods / el.quantity).toFixed(2);
        stat.quantity = el.quantity;
      }
      elementsStats.push(stat);
    });
    let globalStats = {};
    const getElement = (name) => {
      return this.elements.find((el) => el.name === name);
    }
    globalStats.quantity_1 = getElement("Прийшло повідомлення 1 типу").quantity;
    globalStats.quantity_2 = getElement("Прийшло повідомлення 2 типу").quantity;
    globalStats.income_1 =
      globalStats.quantity_1 *
      (this.cost_1 - this.optimizationCost_1 * this.optimizationCount_1);
    globalStats.income_2 =
      globalStats.quantity_2 *
      (this.cost_2 - this.optimizationCost_2 * this.optimizationCount_2);

    return { elementsStats, globalStats };
  }
};
