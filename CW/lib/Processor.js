const Element = require("./Element.js");

module.exports = class Processor extends (
  Element
) {
  constructor(props) {
    super(props);
    this.maxQueue = props.maxQueue || Infinity;
    this.queue = [];
    this.maxObservedQueueLength = this.queue.lenght;
    this.queueLengths = [];
    this.eventSumTime = 0;
    this.stateType = "";
  }
  inAct({ type }) {
    // Якщо елемент вільний оброблюємо подію
    super.inAct();
    if (!this.state) {
      // Додаємо в логи подію
      this.system.log(`Подія ${type} обробляється в ${this.name}`);
      // Час події.
      let eventTime = this.getRandom();
      // TEMP: Виводимо затримку створення події
      // console.log(this.name, " --> ", eventTime);

      // Сума тривалості подій
      this.eventSumTime += eventTime;

      this.state = 1;
      this.stateType = type;
      this.nextEventTime = this.currentTime + eventTime;
    } else {
      // Якщо елемент зайнятий перевіряємо скільки в черзі подій такого ж типу
      let queue = this.queue.filter((item) => item.type === type);
      if (queue.length < 3) {
        this.system.log(`Подія ${type} додається в чергу ${this.name}`);

        // Додаємо подію до черги
        this.queue.push({ type });
        // Якщо розмір черги більше максимального минулого розміру зберігаємо
        if (this.queue.length > this.maxObservedQueueLength) {
          this.maxObservedQueueLength = this.queue.length;
        }
      } else {
        // Якщо черга рівна максимальній - Знищуємо подію
        this.system.log(`Подія ${type} знищується в ${this.name}`);

        this.failure++;
      }
    }
    this.saveStatus();
  }
  outAct() {
    super.outAct();
    // Передаємо подію далі
    this.nextEventTime = Infinity;
    if (this.nextElements) {
      let nextElement = null;

      nextElement = this.nextElements.filter((item) => {
        if (item.messageType === this.stateType) {
          return item;
        }
      })[0];
      // Активуємо наступний елемент
      if (nextElement) {
        nextElement = this.system.elements.find((el) => {
          if (el.name === nextElement.el.name) return el;
        });
        nextElement.inAct({ type: this.stateType });
      }
    }
    // Звільняємо процессор
    this.state = 0;
    this.stateType = "";

    // Якщо є черга
    if (this.queue.length > 0) {
      // Вибираємо першу подію в черзі
      let eventType = this.queue.shift().type;
      this.quantity--;
      this.inAct({ type: eventType });
      // Обираємо наступний елемент
    }

    this.saveStatus();
  }
  saveStatus() {
    this.system.stateLog(this.name, this.queue.length, this.quantity, this.failure);
  }
  calcStats() {
    this.queueLengths.push(this.queue.length);
  }
};
