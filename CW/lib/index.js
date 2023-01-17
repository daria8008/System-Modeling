const System = require("./System.js");
const Create = require("./Create.js");
const Processor = require("./Processor.js");
const LineProcess = require("./LineProcess.js");

module.exports.simulateSystem = (time, isAlternativeProps, z1, z2) => {
  let CREATE_1;
  let CREATE_2;
  
  // CREATOR для повідомлення першого типу
  CREATE_1 = new Create({
    name: "Прийшло повідомлення 1 типу",
    low: 6,
    high: 1,
    randType: "uniform",
    eventType: "message_1",
  });
  // CREATOR для повідомлення другого типу
  CREATE_2 = new Create({
    name: "Прийшло повідомлення 2 типу",
    low: 1.5,
    high: 1,
    randType: "uniform",
    eventType: "message_2",
  });

  // Процессор для обробки повідомлень
  let PROCESSOR = new Processor({
    name: "Процессор",
    low: 2,
    high: 1,
    randType: "uniform",
  });

  // Лінія для повідомлень першого типу
  let LINE_1 = new LineProcess({
    name: "Лінія 1",
    maxQueue: 3,
    delay: 7 - z1,
    randType: "exp",
    messageType: "message_1",
  });
  // Лінія для повідомлень другого типу
  let LINE_2 = new LineProcess({
    name: "Лінія 2",
    maxQueue: 3,
    delay: 8 - z2,
    randType: "exp",
    messageType: "message_2",
  });

  // Всі повідомлення 1 типу 100% переходять до процессора
  CREATE_1.nextElements = [{ el: PROCESSOR, probability: 1 }];
  // Всі повідомлення 2 типу 100% переходять до процессора
  CREATE_2.nextElements = [{ el: PROCESSOR, probability: 1 }];

  // Повідомлення з процессора переходять до двох ліній передачі повідомлень
  PROCESSOR.nextElements = [
    { el: LINE_1, messageType: "message_1" },
    { el: LINE_2, messageType: "message_2" },
  ];

  // Ініціалізуємо систему
  let SYSTEM = new System({
    d1: 20,
    d2: 40,
    z1: 2,
    z2: 4,
    z1Count: z1,
    z2Count: z2,
  });

  // Додаємо елементи до системи
  SYSTEM.elements = [CREATE_1, CREATE_2, PROCESSOR, LINE_1, LINE_2];

  // Додаємо елементам систему
  SYSTEM.elements.forEach((el) => (el.system = SYSTEM));
  return SYSTEM.simulate(time);
};
