namespace lab3_hospital
{
    public class Patient
    {
        public PatientType Type { get; set; }
        public double TIn { get; set; }
        
        public Patient(PatientType type)
        {
            Type = type;
        }
    }
}