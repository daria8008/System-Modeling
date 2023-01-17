const Element = require("./Element.js");
const Processor = require("./Processor.js");

module.exports = class Create extends (
  Element
) {
  constructor(props) {
    super(props);
    this.currentTime = 0;
    this.nextEventTime = 0;
    this.nextElements = [];
    this.eventType = props.eventType;
    this.cumulativeEventArrivalPeriods = 0;
  }
  outAct() {
    super.outAct();
    // Створюємо подію
    let creationDelay = this.getRandom();
    this.quantity++;

    // Додаємо в логи подію
    this.system.log([this.name]);

    // TEMP: Загальна тривалість очікування події
    this.cumulativeEventArrivalPeriods += creationDelay;
    // Наступна подія
    this.nextEventTime = this.currentTime + creationDelay;

    // Вибираємо процессор наступним елементом
    let nextElement = this.system.elements.find(el => {
      if (el instanceof Processor) return el;
    });

    // Викликаємо наступний елемент
    nextElement.inAct({ type: this.eventType });
  }

  printStatus() {
    this.system.log([this.name, "Quantity: ", this.quantity]);
  }
};
