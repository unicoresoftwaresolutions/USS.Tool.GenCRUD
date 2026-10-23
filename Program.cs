using USS.Tool.GenCRUD;
if(SolutionInfo.IsAPIProj || SolutionInfo.IsSQLProj)
{
    Console.WriteLine("Starting CRUD generation...");
    Console.WriteLine(SolutionInfo.Name);
    Console.WriteLine("CRUD generation completed.");
}
else
{
    Console.WriteLine("project is not valid.");
    return;
}
