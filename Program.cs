using USS.Tool.GenCRUD;
if(SQLProj.IsValid)
{
    Console.WriteLine("Starting CRUD generation...");
    SolutionInfo.Generate(true);
    Console.WriteLine("CRUD generation completed.");
}
else
{
    Console.WriteLine("SQL project is not valid.");
    return;
}
