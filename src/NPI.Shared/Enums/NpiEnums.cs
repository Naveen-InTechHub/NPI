namespace NPI.Shared.Enums;

public enum NpiStatus
{
    Draft = 0,
    InProgress = 1,
    PlannerLaunched = 2,
    Launched = 3,
    Completed = 4,
    Cancelled = 5
}

public enum ProductType
{
    Tablet = 0,
    Capsule = 1,
    Powder = 2,
    Liquid = 3,
    Softgel = 4,
    Gummy = 5
}

public enum PackageType
{
    Bottles = 0,
    Blisters = 1,
    Sachets = 2,
    Pouches = 3,
    Jars = 4
}

public enum ComponentType
{
    Bottle = 0,
    Cap = 1,
    Label = 2,
    Shipper = 3,
    Insert = 4,
    Seal = 5
}

public enum LaborCategory
{
    Formula = 0,
    Packout = 1
}

public enum PlannerCategory
{
    RawMaterials = 0,
    Components = 1
}

public enum QAQuestionCategory
{
    Package = 0,
    Quality = 1
    
}

public enum DocumentSlot
{
    EQM = 0,
    CustomerProduct = 1,
    SpecSheet1 = 2,
    SpecSheet2 = 3,
    SpecSheet3 = 4,
    CustomerPO = 5
}

