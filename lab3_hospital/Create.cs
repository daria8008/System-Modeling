namespace lab3_hospital
{
    public class Create : Element
    {
        private readonly Dictionary<double, PatientType> _patients = new();
        private readonly int[] _numOfPatientsEachType;

        public Create(double delay)
                : base(delay)
        {
            InititalizePatients();
            _numOfPatientsEachType = new int[] { 0, 0, 0 };
        }

        private void InititalizePatients()
        {
            _patients[0.5] = PatientType.TYPE1;
            _patients[0.1] = PatientType.TYPE2;
            _patients[0.4] = PatientType.TYPE3;

        }
        public override void OutAct()
        {
            base.OutAct();
            TNext = (TCurr + base.GetDelay());
            var newPatient = GetProbabilityPatient();
            newPatient.TIn = TCurr;
            base.NextElement.InAct(newPatient);
        }

        public Patient GetProbabilityPatient()
        {
            var rand = new Random().NextDouble();
            double acc = 0;
            var probabilities = _patients.Keys;
            foreach (var p in probabilities)
            {
                if (acc < rand && rand < acc + p)
                {
                    var type = _patients[p];
                    CollectPatientTypeInfo(type);
                    return new Patient(type);
                }
                acc += p;

            }
            return null;

        }
        private void CollectPatientTypeInfo(PatientType type)
        {
            if (type == PatientType.TYPE1) _numOfPatientsEachType[0]++;
            else if (type == PatientType.TYPE2) _numOfPatientsEachType[1]++;
            else _numOfPatientsEachType[2]++;
        }

        public string GetNumOfTypes() => $"type1: {_numOfPatientsEachType[0]}, type2: {_numOfPatientsEachType[1]}, type3: {_numOfPatientsEachType[2]}";
    }
}