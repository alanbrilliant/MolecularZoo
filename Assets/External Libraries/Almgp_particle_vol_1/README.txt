Almgp_particle_vol_1
==========================


High-performance particle kit.

include:
2 explosion 
41 loop effects
4 particle shader (mesh-dissolve, multyemmision, face angle power, particle glow)
demo scene

POST PROCESS NOT INCLUDE


---------------------

shaders:


 Almgp/vfx1/mesh_dissolve_particel  - shader for explosion, dissolve include

	parameters:

 Freshnel color - rimlight in side color
 Gradient - gradient for mapping by mask
 power - if NOT use "UseVertexAlpha" - dissolve controller 
 Noise  - sample for procedural noise for dissolve and gradient
 exponent - freshnel exponent
 emmis_power - emmision power
 UseVertexAlpha - if enable -vertex alpha (particle system/color) controll dissolve effect



 Almgp/vfx1/face_power_particle  - freshnel emmision shader for mesh particle

	parameters: 

 	  color - rimlight in side color
	  exp  - freshnel exponent
	  emmis_power - emission power






 Almgp/vfx1/emmision_particle  - non transparent emmision shader for mesh particle

	parameters: 

 	  color - color
	  MainTex - main texture	  
	  emmis_power - emission power






 Almgp/vfx1/multypower_particle  -additive emmision shader large emmision in bloom ( need use HDR in main camera + bloom)

	parameters: 

 	  color - color
	  MainTex - main texture	  
	  emmis_power - emission power