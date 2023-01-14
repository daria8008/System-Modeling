namespace lab3_hospital
{
    public class PatientType
    {
        public static PatientType TYPE1 = new(1, 15);
        public static PatientType TYPE2 = new(2, 40);
        public static PatientType TYPE3 = new(2, 30);

        public double RouteIndex { get; private set; }
        public int RegistrationTime { get; private set; }

        private PatientType(int routeIdx, int time)
        {
            RouteIndex = routeIdx;
            RegistrationTime = time;
        }
    }
}
