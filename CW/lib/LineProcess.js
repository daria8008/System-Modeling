const Element = require("./Element.js");

module.exports = class LineProcess extends (
  Element
) {
  constructor(props) {
    super(props);
    this.maxQueue = props.maxQueue || Infinity;
    this.queue = [];
    this.maxObservedQueueLength = this.queue.length;
    this.queueLengths = [];
    this.eventSumTime = 0;

    this.messageType = props.messageType;
  }
  inAct({ type }) {
    super.inAct();
    
    if (!this.state) {
      // Якщо елемент вільний оброблюємо подію
      this.system.log(`Подія ${type} обробляється в ${this.name}`);

      // Час події.
      let eventTime = this.delay;

      // TEMP: Виводимо затримку створення події
      // console.log(this.name, " --> ", eventTime);

      // Сума тривалості подій
      this.eventSumTime += eventTime;

      this.state = 1;
      this.nextEventTime = this.currentTime + eventTime;
    } else {
      // Якщо елемент зайнятий
      if (this.queue.length < this.maxQueue) {
        this.system.log(
          `Подія ${this.messageType} додається в чергу ${this.name}`
        );
        // Додаємо подію до черги
        this.queue.push({ type });
        // Якщо розмір черги більше максимального минулого розміру зберігаємо
        if (this.queue.length > this.maxObservedQueueLength) {
          this.maxObservedQueueLength = this.queue.length;
        }
      } else {
        // Якщо черга рівна максимальній - Знищуємо подію
        this.system.log(`Подія ${type} знищуеться в ${this.name}`);

        this.failure++;
      }
    }
    this.saveStatus();
  }
  outAct() {
    super.outAct();
    // Звільняємо Лінію
    this.nextEventTime = Infinity;
    this.state = 0;

    // Якщо є черга
    if (this.queue.length > 0) {
      // Вибираємо першу подію в черзі
      let eventType = this.queue.shift();
      this.quantity--;
      this.inAct({ type: eventType });
    }

    // Обираємо наступний елемент
    // if (this.nextElements) {
    //   let nextElement = null;

    //   nextElement = this.nextElements.filter((item) => {
    //     if (item.el.messageType === this.stateType) {
    //       return item;
    //     }
    //   })[0];
    //   // Activate next Element
    //   if (nextElement) {
    //     nextElement.el.inAct({ type: this.stateType });
    //   }
    // }
  }
  saveStatus() {
    // this.system.log([
    //   this.name,
    //   "Queue: ",
    //   this.queue,
    //   "Failure: ",
    //   this.failure,
    //   "Quantity: ",
    //   this.quantity,
    // ]);
    this.system.stateLog(this.name, this.queue.length, this.quantity, this.failure);
  }
  calcStats() {
    this.queueLengths.push(this.queue.length);
  }
};
