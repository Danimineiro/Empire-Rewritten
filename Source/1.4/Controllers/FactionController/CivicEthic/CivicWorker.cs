namespace Empire_Rewritten.Controllers.CivicEthic
{
    public class CivicWorker
    {
        public virtual float CalculateDistanceWeight(float distanceWeight)
        {
            return distanceWeight / 5f;
        }
    }
}