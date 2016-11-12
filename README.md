#ThrottleUp

*Minimum thrusts for bipropellant engines.*

![ThrottleUp logo](https://github.com/Kerbas-ad-astra/ThrottleUp/raw/master/ThrottleUp%20logo.png)

##Features

I feel kind of guilty when I set my leftover booster engine to 5% thrust multiplier and flick the 'shift' button to use it for fine course corrections and orbit adjustment, when I really ought to have RCS thrusters or at least an OMS engine for that stuff, and I feel especially guilty when I do that in my RSS install, since SMURFF is buffing engines to have more thrust and less mass than stock with no downsides.  

However, while I want to enforce some limitations on the minimum impulse bit of my lifter engines, I'm not interested in dealing with limited ignitions or ullage, and I also don't want to depend on RealFuels having configs for each and every engine that I use.  Much like SMURFF, I want a patch that will work with any engine from any mod.

The "minThrust" variable in ModuleEngines is ohhh so tempting, but it turns out that it really means what it says -- that's the minimum thrust.  While I want my lifter engines (for example) to be constrained between 60 and 100%, I'd also like to be able to cut them to zero without having to shut them down.

The result of these desires (plus a few evenings) is ThrottleUp -- a set of patches to add minThrust to bipropellant engines, and a helper plugin to provide the zero-thrust-at-minimum-throttle behavior.  (Monopropellant and ion engines can be throttled very deeply, almost down to zero.  They do lose some efficiency when operated at low thrust, but this effect is minimal in monopropellant engines, which retain over 90% of their optimum Isp at less than 10% thrust, and can also be mitigated by duty-cycling, that is, switching from full thrust to zero and back again to produce a lower average thrust.  On spacecraft, this is handled in the background by a lower-level controller, so I don't feel like modeling this effect in KSP.)  It will attempt to identify engines by their tags into turbopump-fed and pressure-fed (i.e. can / cannot be deeply throttled) categories to assign an appropriate minThrust.

Engines tagged with "sustain", "ascent", "main", "boost", and "launch" are assumed to be turbopump-fed engines, like the Space Shuttle Main Engines.  The SSMEs could be throttled from 109% to 67%, so these engines get a minThrust of 67/1.09 ~ 60% of max.

Engines tagged with "orbit", "land", "vernier", "vacuum", and "maneuver" are assumed to be pressure-fed engines, like the Shuttle Orbit Maneuvering System, Apollo Service Propulsion System and Lunar Module Descent Propulsion System.  Since Wikipedia tells me that the DPS could be throttled between 10 and 65%, I'm assigning them a minThrust of 10/0.65 ~ 15% max.  (The DPS avoided the 65-100% regime to minimize wear, and since KSP parts don't wear out, I'm assuming their full-thrust is actually equivalent to the DPS's 65% -- that is, the maximum thrust sustainable without hurting lifetime too much.)

Any (bipropellant) engines left over get 30%.

The order in which patches are applied is: vernier, land, maneuver, then all of the turbopump tags, then orbit, vacuum, "LFO leftovers", and then monoprop.  Whichever one hits an engine first "sticks", so an engine with "vernier boost" gets 15% minThrust, while "vacuum boost" gets 60%.

##Dependencies

ThrottleUp depends on [**Module Manager**](http://forum.kerbalspaceprogram.com/threads/55219).

##Download and install

* [**GitHub**](https://github.com/Kerbas-ad-astra/ThrottleUp/releases)
* CurseForge

From there, just unzip the "ThrottleUp" folder into your GameData directory.

##Known and anticipated issues

Stock:

* Parts will continue playing their effects even when they are at minimum throttle .  (Presumably they are linked to the throttle, which never goes below 15/30/60 percent.)  I promise, thrust and fuel consumption *are* zero (or close enough to it).

As for mods, just about anything that relies on an engine's throttle can potentially get confused by having a nonzero minThrust, because the throttle will never go below minThrust/maxThrust.  ThrottleUp can potentially exacerbate those issues because it sets up the engine to produce zero thrust at a nonzero throttle.  Two addons I'm aware of that will definitely get confused:

* Kerbal Engineer thinks that engines produce zero delta-V when throttled down.  I've made a fix to the logic to resolve this, but I haven't yet made a PR to cybutek.
* DangIt looks at the state of the throttle (among other things) to decide if an engine is off or on (and thus aging and becoming more likely to fail).  With a nonzero minThrust, the minimum throttle value is always above 0, so if the engine is active, even if ThrottleUp is suppressing the thrust at minimum throttle, DangIt thinks the engine is running.

Please let me know in [**the forum thread**](http://forum.kerbalspaceprogram.com/threads/XXXX) or on [**the GitHub issue tracker**](https://github.com/Kerbas-ad-astra/ThrottleUp/issues) if you find any more!

##Version history and changelog

* 02016 11 xx (1.0): Initial release

##Roadmap

ThrottleUp does what I want it to, and doesn't do anything I don't want it to.  If I find a way to deal with the effects, I will, but that's only a cosmetic issue.  I might adjust the list of tags to look for in each category, and the order in which they are applied, to make engines "fit in" properly, but otherwise it is what it is.

##Credits

Huge thanks to the Squad devs (especially the community members who contributed to the 1.2 release, and especially especially JPLRepo for clarifying this feature to me) for improving engine behavior!

Many thanks as well to ialdeboath and sarbian for Module Manager.

Finally, thanks are owed to the RealFuels developers, for developing an impressive system for enhancing the challenge and realism of rocket launches...and making me wish I had something to get most of the challenge with less of the overhead.  :wink:

##License

ThrottleUp is copyright 2016 Kerbas_ad_astra and released under the [**GNU GPL v3**](https://www.gnu.org/licenses/gpl-3.0) (or any later version).  If you make a fork or redistribution (unless it's intended to be merged with the master or if I'm handing over central control to someone else), you must give it a different name in addition to the other anti-user-confusion provisions of the GPL (see sections 5a and 7).  All other rights (e.g. the ThrottleUp logo) reserved.