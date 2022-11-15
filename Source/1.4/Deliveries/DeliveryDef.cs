using Empire_Rewritten.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Empire_Rewritten.Deliveries
{
    public class DeliveryDef : Def
    {
        private GraphicData iconData;
        private DeliveryWorker worker;
        private Type deliveryWorker;

        public DeliveryWorker Worker => worker ?? deliveryWorker.GetConstructor(new Type[0]).Invoke(new object[0]) as DeliveryWorker;

        public string IconPath => iconData.texPath;

        public override IEnumerable<string> ConfigErrors()
        {
            if (deliveryWorker != null && !deliveryWorker.IsSubclassOf(typeof(DeliveryWorker)))
            {
                yield return $"{deliveryWorker} does not inherit from {nameof(DeliveryWorker)}!";
            }

            foreach (string str in base.ConfigErrors())
            {
                yield return str;
            }
        }
    }
}
