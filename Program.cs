const string GASBOY_IN_PATH = @"Gasboy_IN\";
const string GASBOY_OUT_PATH = @"Gasboy_OUT\";
const string EXTENSION = "*.csv";

string filialeNumber = "200";
string dishel = "DISHEL";
string diesel = "2";
int transactionCount = 0;

//Créer les dossiers s'ils n'existe pas.
Directory.CreateDirectory(GASBOY_IN_PATH);
Directory.CreateDirectory(GASBOY_OUT_PATH);

//Récupère tous les fichiers .CSV dans le dossier Gasboy_IN
string[] CSVfiles = Directory.GetFiles(GASBOY_IN_PATH, EXTENSION);

foreach (var file in CSVfiles)
{
    try
    {
        StreamReader sr = new StreamReader(file);

        //Skip la première ligne qui est le header.
        string header = sr.ReadLine();
        string[] headerValues = header.Split(',');
        string line;
        string fileName = "";

        //Lie le restant du fichier source.
        while ((line = sr.ReadLine()) != null)
        {
            transactionCount += 1;
            string[] values = line.Split(",");

            //Je trouve l'index de chaqun des champs à partir de l'index du header qui lui correspond.
            string driverLastName = values[Array.IndexOf(headerValues, "Driver Last Name")];
            string customVehicleAssetID = values[Array.IndexOf(headerValues, "Custom Vehicle/Asset ID")];
            string transactionDate = values[Array.IndexOf(headerValues, "Transaction Date")];
            string parsedTransactionDate = DateTime.Parse(transactionDate).ToString("yyyyMMdd");
            string odometer = values[Array.IndexOf(headerValues, "Odometer")];
            string units = values[Array.IndexOf(headerValues, "Units")];
            string authorizationAmount = values[Array.IndexOf(headerValues, "Authorization Amount")];
            string authorizationCode = values[Array.IndexOf(headerValues, "Authorization Code")];

            //Vérifie si le numero du conducteur est valide.
            string verifiedLastName = (driverLastName == "SPARE") ? "0" : driverLastName;

            DateTime date = DateTime.Now;
            fileName = $"{filialeNumber}_{date:yyyyMMdd}_{date:HHmmss}.txt";

            //Écrit les données du fichié source dans le fichier traité.
            StreamWriter sw = new StreamWriter(GASBOY_OUT_PATH + fileName, true);

            sw.WriteLine($"{ filialeNumber };{transactionCount};{filialeNumber};" +
                $"{verifiedLastName};{customVehicleAssetID};{parsedTransactionDate};{odometer};" +
                $"{units};{diesel};{authorizationAmount};{dishel};{authorizationCode}");

            sw.Close();
        }

        sr.Close();

        //Change l'entension du fichier source.
        var bakFile = Path.ChangeExtension(file, ".bak");
        File.Move(file, bakFile);

        //Affiche le Log s'il a plus qu'une transaction.
        if (transactionCount > 0)
        {
            Console.Write("\n");
            Console.WriteLine($"Date: { DateTime.Now:yyyy/mm/dd HH:mm}");
            Console.WriteLine($"Nombre de transaction exportée: {transactionCount}");
            Console.WriteLine($"Numéro de filiale: {filialeNumber}");
            Console.WriteLine($"Fichier source: {file} ");
            Console.WriteLine($"Fichier traité: {fileName}");
            Console.Write("\n");
        }

        transactionCount = 0;
        //Attend une secondes pour permettre au programme de créer un nouveau fichier.
        Thread.Sleep(1000);
    }
    catch (Exception e)
    {
        Console.WriteLine("Il a eu un erreur lors de la lecture du fichier :");
        Console.WriteLine(e.Message);
    }  
}

Console.WriteLine("Appuyer sur une touche pour quitter.");
Console.ReadLine();