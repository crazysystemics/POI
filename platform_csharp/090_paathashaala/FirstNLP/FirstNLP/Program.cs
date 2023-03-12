// See https://aka.ms/new-console-template for more information
using System.Text;
using System.IO;

//string s = UTF8Encoding.GetEncoding()
#pragma warning disable CS8600
Console.InputEncoding = Encoding.Unicode;
Console.OutputEncoding = Encoding.Unicode;
int scenario_id = 0;
Console.Write("Please Enter Scenario Id: ");
scenario_id = int.Parse(Console.ReadLine());
// Converting null literal or possible null value to non-nullable type.
if (scenario_id == 1)
{
    Console.WriteLine("ಮೊದಲೊಂದಿಪೆ ನಿನಗೆ ಗಣನಾಥ  ");
}
else if (scenario_id == 2)
{
    string ಪದ್ಯ = "ಒಂದು ಎರಡು-ಬಾಳೆಯ ಹರಡು, ಮೂರು ನಾಲ್ಕು-ಅನ್ನ ಹಾಕು, ಐದು ಆರು-ಬೇಳೆ ಸಾರು, ಏಳು ಎಂಟು-ಪಲ್ಯಕೆ ದಂಟು";
    ಪದ್ಯ += ",ಒಂಬತ್ತು ಹತ್ತು-ಎಲೆ ಮುದಿರೆತ್ತು, ಒಂದರಿಂರ ಹತ್ತು ಹೀಗಿತ್ತು-ಊಟದ ಆಟ ಮುಗಿದಿತ್ತು";
     Dictionary<string,int> anka_number = new Dictionary<string,int>();
    string[] meals_stages = ಪದ್ಯ.Split(',');
    string[] meal_array = new string[11];
    
    meal_array[0] = "ಹಸಿವು";
    meal_array[10] = "ಹೊಟ್ಟೆ ತುಂಬಿತು";
    foreach(string meal in meals_stages)
    {
        string[] ankas = meal.Split('-');
        int anka0 = int.Parse(ankas[0].Split(' ')[0]);
        int anka1 = int.Parse(ankas[0].Split(' ')[1]);
        meal_array[anka0] = ankas[1];
        meal_array[anka1] = ankas[1];
   // https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
    }
}
else
{
    Console.WriteLine("Undefined Scenario");
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.