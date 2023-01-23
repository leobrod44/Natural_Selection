# Natural Selection Simulator with Genetic Algorithms

## Summary
Procedurally generated biomes and creatures controlled by neural networks for real-time sensor/action decision making. Use of Genetic Algorithms for creatures to learn to adapt to their environment by gene reproduction.
## Biome generation

### Natural habitat generation

- World generated randomly to ensure removal of fixed variables during reproduction process. Spawning tiles which can have various properties and contain elements such as food, obstacles, water and trees. Lakes generated with a doubly recursive function. Current implementation [Layout.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Layout.cs) and [Area.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Area.cs)


 medium nature density     |          low nature density  |     high nature density                  
:-------------------------:|:----------------------------:|:-------------------------:
![Screenshot 2023-01-16 154057](https://user-images.githubusercontent.com/65002959/212764644-402da6e2-a8bd-4b05-8286-8141488d8536.png) |  ![Screenshot 2023-01-16 154248](https://user-images.githubusercontent.com/65002959/212764676-71fdee57-4dad-4e42-b91d-144e48b784ec.png) | ![Screenshot 2023-01-16 154337](https://user-images.githubusercontent.com/65002959/212764755-b8768792-2efd-48c9-b279-3666f32e8097.png)

### Creature generation

 - Eventually, more complex character will be generated. For now, random sized and colored cubic animals with randomly generated latin names. Current implementation [Body.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Body.cs). The animal's character control will happen through [Animal.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Animal.cs)

 Sulyha Sotaenis           |          Maejubis Gijolis    |     Qeshusis Raso & Pebyshaho Haeqisheqa               
:-------------------------:|:----------------------------:|:-------------------------:
![Capture](https://user-images.githubusercontent.com/65002959/212765592-271b6f71-ed96-4a74-8e43-2dab3cc0e4f5.PNG) | ![Screenshot 2023-01-16 154742](https://user-images.githubusercontent.com/65002959/212765602-ef46d13a-01dc-477f-92ae-920f123e5835.png) |![Screenshot 2023-01-16 155040](https://user-images.githubusercontent.com/65002959/212765606-4bde5723-cbfa-4f33-adf2-59cce2cc1f70.png)


## Genetic Algorithm Design
- Genetic algorithm for gene reproduction and optimization. This project takes an alternative path to optimization. GA's do not use traditional backpropagation to ajust network weights but rather "breed" them to obtain the mean value between 2 seperate good solutions. Implementation in progress [Brain.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/Brain.cs)

![GA](https://user-images.githubusercontent.com/65002959/213335497-5d64a079-3540-4692-8a88-ba18220bb3bb.png)
![GA1](https://user-images.githubusercontent.com/65002959/213335502-b2e4254e-0f0b-43d6-a0ae-43d763b4bfa6.png)

# Neural Network

 - ## Functionality
      - Characters will always be in movement unless they are eating, drinking or breeding. Depending on their Network, creatures will be responding to situations solely  by changing directions
 - ## Current Sensor and Actor Neurons
      - Neurons implemented in [ActionNeuron.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/ActionNeuron.cs) and [SensorNeuron.cs](https://github.com/leobrod44/Natural_Selection_Simulator/blob/main/Animals/Assets/Scripts/SensorNeuron.cs). All implementing methods to be assigned to a delegate according to the neural net during runtime.
![Neurons](https://user-images.githubusercontent.com/65002959/213335512-dfb6ca9f-9f5f-475a-b31f-0c51b91d9945.png)
