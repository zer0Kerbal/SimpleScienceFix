using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleScienceFix
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class SimpleScienceFix : MonoBehaviour
    {
        private void Update() {
            var experiments = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>();
            if (!experiments.Any())
                return;

            string[] selectedNames = { "crewReport", "surfaceSample", "evaReport" };
            var selection =
                      from experiment in experiments
                      where (experiment.GetScienceCount() > 0)
                      where selectedNames.Contains(experiment.experimentID)
                      let storage = experiment.part.FindModuleImplementing<ModuleScienceContainer>()
                      where storage != null
                      select new { experiment, storage };
            if (!selection.Any())
                return;

            foreach (var s in selection) {
                if (s.experiment.GetData().All(data => s.storage.HasData(data)))
                    continue;
                s.storage.StoreData(new List<IScienceDataContainer> { s.experiment }, true);
                s.experiment.ResetExperiment();
            }
        }
    }
}
