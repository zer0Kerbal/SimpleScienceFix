using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleScienceFix
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SimpleScienceFix : MonoBehaviour
    {
        private void Update()
        {
            var experiments = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>();
            if (!experiments.Any())
                return;

            string[] selectedNames = {"crewReport", "surfaceSample", "evaReport"};
            var selectedExperiments = 
                      from exp in experiments
                      where selectedNames.Contains(exp.experimentID)
                      where (exp.part.FindModuleImplementing<ModuleScienceContainer>() != null)
                      where (exp.GetData().Any())
                      select exp;
            if (!selectedExperiments.Any())
                return;

            foreach (var exp in selectedExperiments)
            {
                var container = exp.part.FindModuleImplementing<ModuleScienceContainer>();

                // String log = string.Concat("Storing ", exp.experimentID, " from ", exp.part.partInfo.title, " in ", container.part.partInfo.title, "\n");
                // ScreenMessages.PostScreenMessage(log, 10f, ScreenMessageStyle.UPPER_LEFT);

                if (exp.GetData().All(data => container.HasData(data)))
                    continue;
                container.StoreData(new List<IScienceDataContainer> { exp }, true);
                exp.ResetExperiment();
            }
        }

    }
}
