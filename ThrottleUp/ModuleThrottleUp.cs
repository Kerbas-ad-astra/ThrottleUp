//	Copyright 2015-2016 Kerbas_ad_astra
//
//	This program is free software: you can redistribute it and/or modify
//	it under the terms of the GNU General Public License as published by
//	the Free Software Foundation, either version 3 of the License, or
//	(at your option) any later version.
//	
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//	GNU General Public License for more details.
//	
//	You should have received a copy of the GNU General Public License
//	along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
namespace ThrottleUp
{
	public class ModuleThrottleUp : PartModule
	{
		List<ModuleEngines> engines = new List<ModuleEngines>();
		MultiModeEngine mmeng;
		ModuleEngines engine;
		float oldMultFlow = 1f;
		bool squash = false;

		public override void OnAwake()
		{
			engines.AddUniqueRange(part.FindModulesImplementing<ModuleEngines>().FindAll(x => x.minThrust > 0));
			engines.AddUniqueRange(part.FindModulesImplementing<ModuleEnginesFX>().ConvertAll(x => x as ModuleEngines).FindAll(x => x.minThrust > 0));
			// Only looking for engines with nonzero minthrusts, so e.g. only the secondary mode of the RAPIER.

			if (part.FindModuleImplementing<MultiModeEngine>() != null)
			{
				mmeng = part.FindModuleImplementing<MultiModeEngine>();

			}

			if (engines.Count == 1)
			{
				engine = engines[0];
			}
			else
			{
				if (mmeng != null)
				{
					engine = engines.Find(x => x.engineID.Equals(mmeng.primaryEngineID));
				}
				else
				{
					engine = engines[0];
				}
			}
		}

		public override void OnFixedUpdate()
		{
			if ((mmeng != null) && (engines.Count > 1))  // In case it's a multi-mode engine with two "valid" [i.e. min-thrust] modes.
			{
				engine = engines.Find(x => x.engineID.Equals(mmeng.mode));
			}

			if (engine != null && engine.EngineIgnited)
			{
				if (!(engine.currentThrottle > engine.throttleMin))
				{
					if (!squash) //we weren't squashing before
					{
						oldMultFlow = engine.multFlow; //not sure what else messes with multFlow, but let's be ready to put back our mess
						squash = true;
					}

					//API says multFlow is a "Multiplier to final flow as calculated", so hopefully it's meant for applications like mine -- as a final adjustment for mods to tinker with.
					engine.multFlow = engine.CLAMP; // The API says ModifyFlow is clamped to not quiiite reach zero, so let's do the same here.

					//engine.DeactivatePowerFX(); //No joy here.
					//engine.DeactivateRunningFX();
					//engine.DeactivateLoopingFX(); //Since there's no "ActivateLoopingFX()", I don't see a reason to bother deactivating it.
				}
				else
				{
					if (squash) // if we were squashing just before, put everything back, otherwise there's no need to act
					{
						engine.multFlow = oldMultFlow;
						squash = false;
						//engine.ActivatePowerFX(); Thought I'd need to activate and then deactivate, but it turns out the effects activate just fine on their own -- I have to keep hammering the "deactivate" methods to keep them off!
						//engine.ActivateRunningFX();      
					}
				}
			}
		}

		public override string GetInfo()
		{
			if (mmeng == null)
			{
				return string.Format("Minimum engine throttle: {0:N1} kN ({1:P0})", engine.minThrust, engine.throttleMin);
			}
			else
			{
				if (engines.Count == 1)
				{
					return string.Format("Minimum engine throttle (for {2} mode only): {0:N1} kN ({1:P0})", engine.minThrust, engine.throttleMin, engine.engineID);
				}
				else
				{
					object[] args = { engines[0].minThrust, engines[0].throttleMin, engines[0].engineID, engines[1].minThrust, engines[1].throttleMin, engines[1].engineID };
					return string.Format("Minimum engine throttle (for {2} mode): {0:N1} kN ({1:P0})\nMinimum engine throttle (for {5} mode): {3:N1} kN ({4:P0})", args);
				}
			}
		}
	}
}