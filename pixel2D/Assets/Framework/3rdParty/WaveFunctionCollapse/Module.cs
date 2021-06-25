/*
** Created by fengling
** DateTime:    2021-06-25 10:14:35
** Description: TODO 
*/

using UnityEngine;

namespace Framework._3rdParty.WaveFunctionCollapse
{
    public class ModuleData
    {
        public string Module;
        public string[][] PossibleNeighborsArray = new string[4][];
    }
    public class Module
    {
        public string Name;
        // public Sprite Sprite; 
        public Texture Texture;
        // public Module[][] PossibleNeighborsArray = new Module[4][];
        
        public string[][] PossibleNeighborsArray;
        public Module(string name, Texture texture, string[][] possibleNeighborsArray)
        {
            Name = name;
            Texture = texture;
            this.PossibleNeighborsArray = possibleNeighborsArray;
        }
    }
}