# WhatIF1

## Summary

WhatIF1 is a Formula 1 post race strategy tool which allows alternate tyre strategies and scenarios to be modelled and analysed.

- Users can load historical race data from races and view significant events during the race through a clean,
single-page user interface. 
- The program allows experimentation of different race scenarios by
tweaking parameters and viewing how this would affect the race outcome.
  - Scenarios include adjustments to the tyre choices made during a driver’s pit stop or changes to when a pit stop occurs.
- Changes are modelled using a novel spatial interpolation approach - effects such as tyre degradation (the decrease in car efficiency of tyres as their lap usage increases), the impact of fuel mass on performance and the effect of traffic (the decrease in car efficiency while closely following another car) are considered - these are not fully considered in previous solutions.

- Historical race data is loadable in real-time through an API, with the corresponding model viewable in the UI.
- The user can scroll through the race time with a scrollbar and view the drivers' positions on track at this time.
- A focus was placed on the UX - users can tweak an event (such as a pit stop timing) and see how their changes affect the race outcome.
- A realistic model of tyre degradation, the traffic effect and fuel depletion is implemented. This model utilises a previously unexplored method of categorising and placing laps in an n-dimensional space and generating new laps by interpolating within this space.

## Report
See the associated report [here](https://drive.google.com/file/d/1lp4KjNkhlgvyfC1Px_MAAZHDlQKkypxD/view).
