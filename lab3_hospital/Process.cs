using System.Collections.ObjectModel;

namespace lab3_hospital
{
    public class Process : Element
    {
        public int MaxQueue { get; set; } = Int32.MaxValue;
        public int @Queue { get; set; }
        public int Failure { get; private set; }

        private double[] patientTInSystem;
        private double[] numOfPatientsOut;
        private double tArrival;
        private double tLastArrival;
        private HashSet<Patient> queueElements = new();
        private int busyChannels;
        private ProcessType processType;

        private readonly List<double> tnexts = new();
        private readonly Dictionary<double, Patient> patientsIn = new();
        private readonly Dictionary<double, Element> nextElements = new();
        private readonly int _numOfChannels;
        private readonly RouteSelectionMethod _routeSelectionMethod;

        public Process(double delay)
            : this(delay, 1, RouteSelectionMethod.NONE)
        {
        }
        public Process(double delay, int numOfChannels)
            : this(delay, numOfChannels, RouteSelectionMethod.NONE)
        {
        }
        public Process(double delay, RouteSelectionMethod routeSelectionMethod)
        : this(delay, 1, routeSelectionMethod)
        {
        }
        public Process(double delay, int numOfChannels, RouteSelectionMethod routeSelectionMethod)
            : base(delay)
        {
            _numOfChannels = numOfChannels;
            _routeSelectionMethod = routeSelectionMethod;
            processType = ProcessType.NONE;
            patientTInSystem = new double[] { 0, 0 };
            numOfPatientsOut = new double[] { 0, 0 };
            tArrival = 0;
            tLastArrival = -1;
        }

        public override void InAct(Patient patient)
        {
            DoArrivalStatistics();
            if (busyChannels != _numOfChannels)
            {
                State = 1;
                busyChannels++;
                double delay = GetDelay(patient);
                AddTnext(TCurr + delay);
                patientsIn[TCurr + delay] = patient;
            }
            else
            {
                if (Queue < MaxQueue)
                {
                    Queue++;
                    queueElements.Add(patient);
                }
                else
                {
                    Failure++;
                }
            }
        }

        public override void OutAct()
        {
            base.OutAct();
            busyChannels--;
            double removedTnext = RemoveTnext();
            patientsIn.Remove(removedTnext, out var processedPatient);
            CheckState();
            ChangePatientType(processedPatient);
            if (Queue > 0)
            {
                Queue--;
                State = 1;
                busyChannels++;
                var patient = GetPatientFromQueue();
                var delay = GetDelay(patient);
                AddTnext(TCurr + delay);
                patientsIn[TCurr + delay] = patient;
            }
            if (nextElements.Any())
            {
                var next = FindNextElement(processedPatient);
                if (next != null)
                {
                    next.InAct(processedPatient);
                }
                else
                {
                    UpdateTimeInSystemStatistics(processedPatient);
                }
            }
            else
            {
                UpdateTimeInSystemStatistics(processedPatient);
            }
        }
        private void UpdateTimeInSystemStatistics(Patient patient)
        {
            double time = TCurr - patient.TIn;
            if (patient.Type == PatientType.TYPE1)
            {
                patientTInSystem[0] += time;
                numOfPatientsOut[0]++;
            }
            else if (patient.Type == PatientType.TYPE3)
            {
                patientTInSystem[1] += time;
                numOfPatientsOut[1]++;
            }
        }

        private void DoArrivalStatistics()
        {
            if (processType == ProcessType.REGISTRY)
            {
                ManageTimeBetweenLabArrival();
            }
        }

        private Patient GetPatientFromQueue()
        {
            foreach (var p in queueElements)
            {
                if (p.Type == PatientType.TYPE1)
                {
                    queueElements.Remove(p);
                    return p;
                }
            }
            return queueElements.First();
        }

        public void SetNextElement(Element nextElement)
        {
            if (_routeSelectionMethod == RouteSelectionMethod.NONE)
            {
                SetNextElement(nextElement, 1.0);
            }
        }

        public void SetNextElement(Element nextElement, double routeParameter)
        {
            if (_routeSelectionMethod == RouteSelectionMethod.NONE && nextElements.Any())
            {
                nextElements[1.0] = nextElement;
                return;
            }
            if (_routeSelectionMethod == RouteSelectionMethod.PROBABILITY && routeParameter > 1)
            {
                return;
            }
            nextElements[routeParameter] = nextElement;
        }

        private Element? FindNextElement(Patient patient)
        {
            if (_routeSelectionMethod == RouteSelectionMethod.PROBABILITY)
            {
                return FindNextProbabilityElement();
            }
            else if (_routeSelectionMethod == RouteSelectionMethod.PRIORITY)
            {
                return FindNextPriorityElement();
            }
            else if (_routeSelectionMethod == RouteSelectionMethod.PATIENT_TYPE)
            {
                return FindNextPatientTypeElement(patient);
            }
            if (nextElements.Count == 1)
            {
                return nextElements[1.0];
            }
            return null;
        }

        private Element? FindNextPatientTypeElement(Patient patient)
        {
            return nextElements.GetValueOrDefault(patient.Type.RouteIndex);
        }

        private Element FindNextProbabilityElement()
        {
            var rand = new Random().NextDouble();
            Double accumulator = 0.0;
            foreach (var p in nextElements.Keys)
            {
                if (accumulator < rand && rand < accumulator + p)
                {
                    return nextElements[p];
                }
                accumulator += p;

            }
            return null;
        }
        private Element FindNextPriorityElement()
        {
            if (QueuesAreFull())
            {
                Failure++;
                return null;
            }
            if (QueuesAreEqual())
            {
                return GetPriorityElement();
            }
            return GetLeastQueueElement();
        }
        private void ChangePatientType(Patient patient)
        {
            if (this.processType == ProcessType.LABORATORY && patient.Type == PatientType.TYPE2)
            {
                patient.Type = PatientType.TYPE1;
            }
        }

        public override double TNext
        {
            get
            {
                if (!tnexts.Any())
                {
                    return Double.MaxValue;
                }
                return tnexts.First();
            }
        }

        private void AddTnext(double tnext)
        {
            tnexts.Add(tnext);
            tnexts.Sort();
        }

        private double RemoveTnext()
        {
            if (tnexts.Any())
            {
                double removedTnext = tnexts.First();
                tnexts.RemoveAt(0);
                return removedTnext;
            }
            return 0;
        }

        private void CheckState()
        {
            State = (busyChannels == 0 ? 0 : 1);
        }

        private bool QueuesAreFull()
        {
            foreach (var element in nextElements.Values)
            {
                if (((Process)element).Queue != ((Process)element).MaxQueue)
                {
                    return false;
                }
            }
            return true;
        }
        private bool QueuesAreEqual()
        {
            var elementSet = nextElements.Values;
            int queueAcc = ((Process)elementSet.First()).Queue;
            foreach (var element in elementSet)
            {
                if (((Process)element).Queue != queueAcc)
                {
                    return false;
                }
            }
            return true;
        }
        private Element GetPriorityElement()
        {
            var minPriority = nextElements.Keys.Min();
            return nextElements[minPriority];
        }
        private Element GetLeastQueueElement()
        {
            var accQueue = Int32.MaxValue;
            var accElement = new Element();
            foreach (var element in nextElements.Values)
            {
                int queue = ((Process)element).Queue;
                if (queue < accQueue)
                {
                    accQueue = queue;
                    accElement = element;
                }
            }
            return accElement;
        }
        public double GetDelay(Patient patient)
        {
            if (processType == ProcessType.DOCTORS_ON_DUTY)
            {
                return patient.Type.RegistrationTime;
            }
            return base.GetDelay();
        }
        public void SetProcessType(ProcessType type)
        {
            this.processType = type;
        }

        public override void PrintInfo()
        {
            base.PrintInfo();
            Console.WriteLine($"failure = {Failure}");
        }

        public double GetMeanPatientTimeInSystem(PatientType type)
        {
            if (processType == ProcessType.LABORATORY || processType == ProcessType.ACCOMPANYING_DOCTORS)
            {
                if (type == PatientType.TYPE1)
                {
                    if (numOfPatientsOut[0] == 0) return 0;
                    return patientTInSystem[0] / numOfPatientsOut[0];
                }
                else if (type == PatientType.TYPE3)
                {
                    if (numOfPatientsOut[1] == 0) return 0;
                    return patientTInSystem[1] / numOfPatientsOut[1];
                }
            }
            return 0;
        }

        public ProcessType GetProcessType() => processType;

        private void ManageTimeBetweenLabArrival()
        {
            if (tLastArrival == -1)
            {
                tLastArrival = TCurr;
            }
            else
            {
                double newInterval = TCurr - tLastArrival;
                tArrival += newInterval;
                tLastArrival = TCurr;
            }
        }

        public double GetMeanTBetweenArrival() => tArrival / (Quantity - 1);
    }
}