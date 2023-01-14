using lab3_bank;

var c = new Create(0.2);
var p1 = new Process(0.3);
var p2 = new Process(0.3);

c.SetNextElement(p1, 1);
c.SetNextElement(p2, 2);

p1.MaxQueue = 3;
p2.MaxQueue = 3;

c.Name = "CREATOR";
p1.Name = "CASHIER1";
p2.Name = "CASHIER2";


c.Distribution = "exp";
p1.Distribution = "exp";
p2.Distribution = "exp";

var list = new List<Element>() { c, p1, p2 };

var model = new Model(list);
var queryChangeElements = new int[] { list.IndexOf(p1), list.IndexOf(p2) };
model.setQueueChangeProcessIndexes(queryChangeElements);
model.simulate(1000.0);
