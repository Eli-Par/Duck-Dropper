# Duck-Dropper
A relaxing game about filling a yard with thousands of rubber ducks.

Enjoy jazz music while covering the environment in rubber ducks. Watch as they cascade down piles of ducks and off of roofs in a satisfying and calming experience. The game has been optimized to allow many ducks on screen at once and various settings to allow you to get the best experience possible on your hardware.

## Controls
Click with the left mouse button to drop a single duck
Hold the left mouse button to continuously drop ducks
Move the mouse around to control where the ducks fall
Press escape (Esc) to pause
Press F2 to take a screenshot
Press F to toggle a framerate counter

## Settings Recommendations
Change the quality and anti-aliasing settings first to get better framerates. If this isn't enough or the only option lower in quality is Very Low, try reducing the duck quality setting.

Here is what the settings do:
The quality option changes how good the graphics look.
The anti-aliasing option has different settings that smooth jagged pixels.
The duck quality setting changes how good a large numbers of ducks look. This does not change the quality of any individual duck but rather changes how they are optimized with lower settings being faster but at the cost of visual fidelity.

# Technical Info
There is a lot of logic involved in getting thousands of ducks to be on screen at once without incurring an extreme performance hit. The following is an overview of the tricks employed to achieve this.

There are 4 different quality levels that the ducks can have. A high poly model, a low poly model, a model that is two yellow cubes and a billboard that faces the camera. The game switches between these models using two systems, which allows the game to have visual fidelity while maintaining performance. The two systems are nicknamed the dynamic duck queue and the static duck queue. They are named this due to how the dynamic duck queue (often called the dynamic queue) is responsible for ducks that move. On the other hand the static duck queue (called the static queue) is responsible for ducks that don't move.

## Dynamic Duck Queue
When a new duck is requested due to a player clicking, the dynamic queue is responsible for adding it to the scene. It will instantiate a new duck of the highest quality at the location specified near the start of the game. Each newly added duck gets added to an array. Once the entire array is full of ducks, rather than instantiating a new duck when a request is received, the system will instead reuse an existing duck by moving it to the new location. It will then create or reuse a lower poly duck to replace the one it moved. This system ensures that the most recent ducks look good as this is where the player will be focused. It also helps performance by limiting the number of each quality of duck so there are never too many on screen. When a low poly duck is moved, the ducks old position is sent to the static duck queue to be replaced. The other instance where the static duck queue is asked to replace a duck the case where a high quality duck is not moving anymore and the static queue is able to keep it at a higher quality. In that case it will be replaced by a high quality duck from the static queue instead of a low poly duck from the dynamic queue.

## Static Duck Queue
The static duck queue is responsible for having the quality of the ducks be determined by how close they are to the camera. This is achieved by the playspace being split into a grid of sections used to track approximately where each duck is. When the static queue is told to add a duck to the scene, it will check how many ducks of the requested quality it has in the scene. If it doesn't have the max number of ducks of that quality then it will instantiate a new one, otherwise it will reuse an already existing duck. It will check through the sections, starting with the farthest from the camera to the closest, until it finds a section with a duck of that quality. It will then use that duck to fulfill the request and replace it using a recursive call to add a duck of the next quality down to the position of the old duck before it was moved. When the duck is created or moved, it is registered with the section it is in which keeps track of which ducks are inside of it. When a duck is registered with a section, the section has some logic to check if there are any ducks that can no longer be seen due to being covered by ducks in order sections. If it finds this, some ducks will be removed from the section and returned to a queue of recycled ducks tracked by the static queue. Before the static queue instantiates or reuses an existing duck, the static queue will check if there are any ducks in the recycled queue and use that duck instead.

## Sections
Sections serve two rolls. They store references to the ducks that are contained within them and they recycle ducks that can't be seen within them. The logic that initiates the recycle routine is run on a section that has had a new duck added to it, since this is the only place that has changed. It first checks if the new duck is in a higher position than the highest duck already in the section. It will only continue if this check succeeds. Then each of the sections adjacent to where the duck was added will figure out what the lowest height of it's adjacent sections are (using the sections value for their highest duck). It uses this value to figure out if a yellow block can be moved up to cover part of the section. If this value minus an offset if higher than the height of the block, the block is moved up and all ducks inside of it are recycled. This system ensures that any ducks that are recycled are fully encased by ducks in all adjacent sections. The blocks being yellow also help prevent any obvious gaps in the pile where tiny holes may poke through.

All this logic allows many ducks to be on screen with a minimal performance loss and performs better in this scenario then simply using a traditional LOD system (although one is also employed as well, despite not being talked about in depth). It allows for small numbers of ducks on screen to still have high fidelity, even if those few ducks are farther away, while preventing large numbers of ducks close to the camera from having a large performance hit.

# Credits
Special thanks to [Nubi](https://www.instagram.com/nubiibranch/) for drawing the duck silhouette and the large duck in the logo for me. I really appreciate the help.

Here are all the other [assets used](CREDITS.md).
