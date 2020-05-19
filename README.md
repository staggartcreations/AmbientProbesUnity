# Ambient probes for Unity
Similar to light probes, but samples a flat ambient color and does not require any baking. This can be benefitial to specify ambient light for dynamic objects in specific locations.

[Example video](https://gfycat.com/mixedethicalindianglassfish)

Instructions
------------

Create an empty object and `Add Component->Rendering->Ambient Probes->Group`. Click "Edit Probes" to create a first probe, from this point on you can duplicate the select probes to create more. It's important to surround an area by probes that are set to "Global", this is so it's possible to transition to whatever ambient light color the scene uses.

On any object, go to `Add Component->Rendering->Ambient Probes->Sampler`. And assign any renderers that should be affected. 

The `Property Name` field represents the color parameter that's being set on the materials. By default this will use _Color, but the system requires a shader with a custom lighting model so ambient light can be overridden.

Limitations
-------
- Sampling is done based on the sampler's position, meaning the renderers are affected as a whole, not just parts
- Sampling can only happen between 2 probes (source and destination). Doing so for multiple probes requires fetching all nearby probes and sorting them by distance, this requires some form of space partitioning for performance.
- Probe positions do not move with the probe group

License
-------
MIT License (see [LICENSE](LICENSE.md))
