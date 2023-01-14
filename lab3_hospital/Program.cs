using lab3_hospital;

var c = new Create(15);
var doctorsOnDuty = new Process(1, 2, RouteSelectionMethod.PATIENT_TYPE);
var additional1 = new Process(1, Int32.MaxValue);
var accompanyingDoctors = new Process(1, 3);
var registry = new Process(4.5);
var laboratory = new Process(4, 2, RouteSelectionMethod.PATIENT_TYPE);

var additional2 = new Process(1, Int32.MaxValue);

/*-------------*/

doctorsOnDuty.MaxQueue = Int32.MaxValue;
accompanyingDoctors.MaxQueue = Int32.MaxValue;
registry.MaxQueue = Int32.MaxValue;
laboratory.MaxQueue = Int32.MaxValue;

additional1.MaxQueue = Int32.MaxValue;
additional2.MaxQueue = Int32.MaxValue;

/*-------------*/

c.Name = "CREATOR";
doctorsOnDuty.Name = "DOCTORS_ON_DUTY";
accompanyingDoctors.Name = "ACCOMPANYING_DOCTORS";
registry.Name = "REGISTRY";
laboratory.Name = "LABORATORY";

additional1.Name = "ADDITIONAL1";
additional2.Name = "ADDITIONAL2";

/*-------------*/
laboratory.SetProcessType(ProcessType.LABORATORY);
doctorsOnDuty.SetProcessType(ProcessType.DOCTORS_ON_DUTY);
registry.SetProcessType(ProcessType.REGISTRY);
additional1.SetProcessType(ProcessType.ADDITIONAL);
accompanyingDoctors.SetProcessType(ProcessType.ACCOMPANYING_DOCTORS);
additional2.SetProcessType(ProcessType.ADDITIONAL);
/*-------------*/

c.Distribution = "exp";

accompanyingDoctors.Distribution = "unif";
accompanyingDoctors.DelayMean = 3;
accompanyingDoctors.DelayDev = 8;

registry.SetDistribution("erlang", 3);
laboratory.SetDistribution("erlang", 2);

additional1.Distribution = "unif";
additional1.DelayMean = 2;
additional1.DelayDev = 5;

additional2.Distribution = "unif";
additional2.DelayMean = 2;
additional2.DelayDev = 5;

/*-------------*/

c.NextElement = doctorsOnDuty;
doctorsOnDuty.SetNextElement(accompanyingDoctors, 1);
doctorsOnDuty.SetNextElement(additional1, 2);
additional1.SetNextElement(registry);
registry.SetNextElement(laboratory);
laboratory.SetNextElement(additional2, 1);
additional2.SetNextElement(doctorsOnDuty);


var list = new List<Element>() { c, doctorsOnDuty, additional1, accompanyingDoctors, registry, laboratory, additional2 };
var model = new Model(list);
model.Simulate(1000.0);