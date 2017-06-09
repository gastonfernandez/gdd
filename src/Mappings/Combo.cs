using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UberFrba
{
    class Combo
    {
         public string Name {get; set;}
    public int Value{get; set;}
 
    public Combo(string name, int value)
    {
        Name = name; 
        Value = value;
    }
    public override string ToString()
    {
        return Name;
    }
    }
}
