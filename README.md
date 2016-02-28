# GP4Sim

GP4Sim is a series of plugins for the [HeuristicLab](http://dev.heuristiclab.com) optimization environment.
They introduce three features:
* Classes for the implementation of Genetic Programming optimzation problems within HeuristicLab, complementing the implementations for regression and classification problems HL offers out of the box.
* A ready-to-use HL implementation for evolving genetic programming financial trading strategies 
* An Interpreter for GP trees making use of the C# Expression Tree feature
<p>
GP4Sim is entirely developed in C# and is intended to run on Windows systems only, just like HeuristicLab.<br>
It requires .NET 4.5 or greater and HeuristicLab 3.3.13.<br>
</p>

## Installation

### Prerequisites
* .NET 4.5
* [HeuristicLab](http://dev.heuristiclab.com)

### From Sources
* [Download] (https://github.com/lospooky/GP4Sim/archive/master.zip) the source OR Clone the repo, `git clone https://github.com/lospooky/GP4Sim.git`
* Open GP4Sim.sln with Visual Studio 2015 and fix the HeuristicLab references for all the packages
* Build
* Put the compiled .dlls in your Heuristiclab Installation directory

### From Binaries
* [Download](https://github.com/lospooky/GP4Sim/releases/download/v1.0/GP4Sim-v1.0.zip) the compiled binaries, unzip and place in your HeuristicLab Installation directory

## Usage

## Credits & License
[blog.spook.ee/GP4Sim](blog.spook.ee/GP4Sim)<br>
© Simone Cirillo. 2016.<br>
SynoShows is distributed under the [MIT License](https://opensource.org/licenses/MIT).<br>
