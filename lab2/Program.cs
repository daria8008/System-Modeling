using lab2;

var c = new Create(2);
var p1 = new Process(5, 2);
var p2 = new Process(5, 2);
var p3 = new Process(5, 2);
var p4 = new Process(5, 2);

Console.WriteLine("id0 = " + c.Id
        + " id1=" + p1.Id
        + " id2=" + p2.Id
        + " id3=" + p3.Id
        + " id4=" + p4.Id);

c.NextElement = p1;
p1.SetNextElement(p2, 0.2);
p1.SetNextElement(p3, 0.8);
p2.SetNextElement(p4, 1);
p3.SetNextElement(p4, 1);
p4.SetNextElement(p1, 0.4);


p1.MaxQueue = 5;
p2.MaxQueue = 5;
p3.MaxQueue = 5;
p4.MaxQueue = 5;

c.Name = "CREATOR";
p1.Name = "PROCESSOR1";
p2.Name = "PROCESSOR2";
p3.Name = "PROCESSOR3";
p4.Name = "PROCESSOR4";


c.Distribution = "exp";
p1.Distribution = "exp";
p2.Distribution = "exp";
p3.Distribution = "exp";
p4.Distribution = "exp";

List<Element> list = new() { c, p1, p2, p3, p4 };

var model = new Model(list);
model.Simulate(1000.0);