using System.Collections.Generic;

public enum TraitType
{
    Durability,
    Mobility,
    Ferocity
}

public class Trait
{
    public TraitType type;
    public string name;
    public Trait dependsOn;
    //public List<Stat> statsIAffect?

    public Trait(TraitType typeOfTrait, string nameOfTrait)
    {
        type = typeOfTrait;
        name = nameOfTrait;
    }

}
