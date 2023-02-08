using Newtonsoft.Json;
using PcConfiguratorApplication;
using System.Text.Json.Serialization;

//Read information from JSON and deserialize

PsStoreInventoryInput? dToPcStore = ReadAndDeserializeJSON();

Console.WriteLine("PC Store");

//Render Pc Inventory

RenderInventory(dToPcStore);

//Ask the client to pick pats for configuration

string info;
string[] inputPartNumberCPU;

InputOnConsole(out info, out inputPartNumberCPU);

//Logic for parts that are compatible each other and make configuration easy for user and you can stop program with Buy word

while (inputPartNumberCPU[0] != "Buy")
{

    Console.WriteLine();

    var cpu = dToPcStore.CPUs.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[0]);
    var motherboard = dToPcStore.Motherboards.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[0]);
    var memory = dToPcStore.Memory.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[0]);

    //if there is one part CPU we show all possible configuration combinations and if part is npt valid ERROR

    if (inputPartNumberCPU.Length == 1)
    {
        if (cpu != null)
        {
            var motherboards = dToPcStore.Motherboards.Where(m => m.Socket == cpu.Socket).ToList();
            var memorys = dToPcStore.Memory.Where(m => m.Type == cpu.SupportedMemory).ToList();
            PrintCombinationsWithCpu(cpu, motherboards, memorys);
        }
        else
        {
            Console.WriteLine("ERROR: Please choose different component types: (1)CPU, (2)Motherboard, (3)Memory");
            continue;
        }
    }

    //If there is two parts that are compatible we show all possible configuration combinations 
    //and if they are not compatible or valid ERROR and message wwhich part that are not compatible

    else if (inputPartNumberCPU.Length == 2)
    {
        motherboard = dToPcStore.Motherboards.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[1]);

        if (cpu != null
            && motherboard != null
            && cpu.Socket == motherboard.Socket)
        {
            var memorys = dToPcStore.Memory.Where(m => m.Type == cpu.SupportedMemory).ToList();
            PrintCombinationsWithCpuAndMotherboard(cpu, motherboard, memorys);
        }
        else if (cpu.Socket != motherboard.Socket)
        {
            Console.WriteLine($"Motherboard of type {motherboard.Socket} is not compatible with the CPU");
        }
        else
        {
            Console.WriteLine("ERROR: Please choose different component types: (1)CPU, (2)Motherboard, (3)Memory");
            continue;
        }
    }

    //If there is three parts that are compatible we show all possible configuration combinations 
    //and if they are not compatible or valid ERROR and message which part that are not compatible

    else if (inputPartNumberCPU.Length == 3)
    {
        cpu = dToPcStore.CPUs.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[0]);
        motherboard = dToPcStore.Motherboards.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[1]);
        memory = dToPcStore.Memory.FirstOrDefault(c => c.PartNumber == inputPartNumberCPU[2]);

        if (cpu != null
            && motherboard != null
            && memory != null
            && cpu.Socket == motherboard.Socket
            && cpu.SupportedMemory == memory.Type)
        {
            PrintOrderWithThreeParts(cpu, motherboard, memory);
        }
        else if (cpu.Socket != motherboard.Socket)
        {
            Console.WriteLine($"Motherboard of type {motherboard.Socket} is not compatible with the CPU");
        }
        else if (cpu.SupportedMemory != memory.Type)
        {
            Console.WriteLine($"Memory of type {memory.Type} is not compatible with the CPU");
        }
        else
        {
            Console.WriteLine("ERROR: The selected configuration is not valid.");
            continue;
        }
    }
    Console.WriteLine(info);
    inputPartNumberCPU = Console.ReadLine().Split(", ");
}

static void RenderInventory(PsStoreInventoryInput? dToPcStore)
{
    foreach (var item in dToPcStore.CPUs)
    {
        Console.WriteLine("CPU - " + item.Name + " - " + item.Socket + ", " + item.SupportedMemory + " Price: " + item.Price + " Part Number: " + item.PartNumber);
    }
    Console.WriteLine();
    foreach (var item in dToPcStore.Motherboards)
    {
        Console.WriteLine("Motherboard - " + item.Name + " - " + item.Socket + " Price: " + item.Price + " - Part Number: " + item.PartNumber);
    }
    Console.WriteLine();
    foreach (var item in dToPcStore.Memory)
    {
        Console.WriteLine("Memory - " + item.Name + " - " + item.Type + " Price: " + item.Price + " - Part Number: " + item.PartNumber);
    }
}

static void PrintCombinationsWithCpu(CPUs? cpu, List<Motherboards> motherboards, List<Memory> memorys)
{
    int counterCombinations = 1;
    decimal price = 0;

    Console.WriteLine("There are possible combinations:");

    for (int i = 0; i < motherboards.Count; i++)
    {
        for (int l = memorys.Count - 1; l >= 0; l--)
        {
            var motherB = motherboards[i];
            var memorii = memorys[l];

            Console.WriteLine($"Combination {counterCombinations}");
            Console.WriteLine($"CPU - {cpu.Name} - {cpu.Socket}, {cpu.SupportedMemory}");
            Console.WriteLine($"Motherboard - {motherB.Name}, {motherB.Socket}");
            Console.WriteLine($"Memory - {memorii.Name} - {memorii.PartNumber}, {memorii.Type}");

            price = cpu.Price + motherB.Price + memorii.Price;

            Console.WriteLine($"Price: {(int)price}");
            Console.WriteLine();
            counterCombinations++;
            price = 0;
        }
    }

}

static void PrintCombinationsWithCpuAndMotherboard(CPUs? cpu, Motherboards? motherboard, List<Memory> memorys)
{
    int counterCombinations = 1;
    decimal price = 0;

    Console.WriteLine("There are possible combinations:");

    for (int l = memorys.Count - 1; l >= 0; l--)
    {
        var memorii = memorys[l];

        Console.WriteLine($"Combination {counterCombinations}");
        Console.WriteLine($"CPU - {cpu.Name} - {cpu.Socket}, {cpu.SupportedMemory}");
        Console.WriteLine($"Motherboard - {motherboard.Name}, {motherboard.Socket}");
        Console.WriteLine($"Memory - {memorii.Name} - {memorii.PartNumber}, {memorii.Type}");

        price = cpu.Price + motherboard.Price + memorii.Price;

        Console.WriteLine($"Price: {(int)price}");
        Console.WriteLine();
        counterCombinations++;
    }
}

static void PrintOrderWithThreeParts(CPUs? cpu, Motherboards? motherboard, Memory? memory)
{
    decimal price = 0;

    Console.WriteLine($"CPU - {cpu.Name} - {cpu.Socket}, {cpu.SupportedMemory}");
    Console.WriteLine($"Motherboard - {motherboard.Name}, {motherboard.Socket}");
    Console.WriteLine($"Memory - {memory.Name} - {memory.PartNumber}, {memory.Type}");
    price = cpu.Price + motherboard.Price + memory.Price;
    Console.WriteLine($"Price: {(int)price}");
    Console.WriteLine();
}

static void InputOnConsole(out string info, out string[] inputPartNumberCPU)
{
    Console.WriteLine();
    info = "Please enter part number(s):\r\n Example: (1)CPU, (2)Motherboard, (3)Memory \r\n If U write 'Buy' you end the program";
    Console.WriteLine(info);
    inputPartNumberCPU = Console.ReadLine().Split(", ");
}

static PsStoreInventoryInput? ReadAndDeserializeJSON()
{
    string pcStoreJson = File.ReadAllText("..//..//..//pc-store-inventory.json");
    var dToPcStore = JsonConvert.DeserializeObject<PsStoreInventoryInput>(pcStoreJson);
    return dToPcStore;
}
