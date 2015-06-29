# Kerbal Trashman
A KSP plugin that removes orbital debris that dip below the atmosphere with one click of a button.

### How to use
1. In the tracking station screen, click the trash can icon in the stock toolbar.
2. The window will display which objects are available for removal.
3. Click **Yes** to remove the list of objects.

### Removal criteria
* Object must be classified as *Debris*.
* Object's orbit must have an eccentricity less than 1 (aka no hyperbolic trajectories).
* Object must not have an encounter with another body.
* Object must be in orbit around a planet with an atmosphere.
* Object's periapsis must dip into the atmosphere.
* Object must not be crewed.

### License
BSD 2-Clause
