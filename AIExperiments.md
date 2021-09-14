# AI Experiments

## What is this?
This is a fun branch for experimenting with AI behaviour, specifically genetic algorithms meeting ANNs.

## What's the plan?

- [ ] Create a simple neural network solver.
    - [ ] Neurons & Axons
    - [ ] Activation & Propagation (Sigmoid)
    - [ ] Connectivity in Layers (Fixed Size)
- [ ] Connect the input neurons to some raycast sensors and inputs on the ship.
    - [ ] Front-left, Front, Front-right
    - [ ] Engine height from ground, per engine.
    - [ ] Ship speed
    - [ ] Ship drift pressed y/n
    - [ ] Ship turbo pressed y/n
- [ ] Connect the output neurons to the controls. 
    - [ ] Left / Right
    - [ ] Drift
    - [ ] Turbo
- [ ] Serialize the network as a Genome
    - [ ] Genes control connectivity (inputs and outputs aside)
- [ ] Allow Genomes to crossbreed
    - [ ] Simple haploid 50/50 blends..?
- [ ] Take 50 ships and throw 'em in the scene and measure fitnesses
    - [ ] Fitness A = Closeness to target checkpoint
    - [ ] Fitness B = Time taken to get there
- [ ] Crossbreed top 5 to create next generation and repeat until we have Really Good Ships.
- [ ] Save the best gene so you can initialise with it next time or keep cool ones.
