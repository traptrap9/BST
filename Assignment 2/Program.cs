using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        List<int> treeSizes = new List<int> { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
        int numberOfIterations = 1000;

        Dictionary<string, TreeType> treeTypes = new Dictionary<string, TreeType>
    {
        { "Random", TreeType.Random },
        { "Balanced", TreeType.Balanced },
        { "Unbalanced", TreeType.Unbalanced },
        { "Sorted", TreeType.Sorted }
    };

        foreach (var treeType in treeTypes)
        {
            Console.WriteLine($"Testing {treeType.Key} trees");
            foreach (int size in treeSizes)
            {
                var tree = GenerateTree(size, treeType.Value);
                var collection = new MovieCollection();
                InsertTreeIntoCollection(tree, collection);

                PerformTests($"{treeType.Key} Tree size {size}", numberOfIterations, collection);
            }
            Console.WriteLine();
        }
    }

    static void PerformTests(string description, int numberOfIterations, MovieCollection collection)
    {
        int totalOperationCount = 0;

        for (int i = 0; i < numberOfIterations; i++)
        {
            int operationCount = collection.NoDVDs();
            totalOperationCount += operationCount;
        }

        double averageOperationCount = totalOperationCount / (double)numberOfIterations;
        Console.WriteLine($"{description}: Average count: {averageOperationCount}");
    }


    static BTreeNode? GenerateSortedTree(int numberOfNodes)
    {
        List<IMovie> sortedMovies = new List<IMovie>();

        for (int i = 0; i < numberOfNodes; i++)
        {
            string movieTitle = $"Movie {i}";
            int availableCopies = 1;

            MovieGenre genre = (MovieGenre)(i % Enum.GetValues(typeof(MovieGenre)).Length);
            MovieClassification classification = (MovieClassification)(i % Enum.GetValues(typeof(MovieClassification)).Length);
            int duration = 60 + (i * 2);

            IMovie movie = new Movie(movieTitle, genre, classification, duration, availableCopies);
            sortedMovies.Add(movie);
        }

        return GenerateBalancedTree(sortedMovies, 0, sortedMovies.Count - 1);
    }


    static BTreeNode? GenerateBalancedTree(List<IMovie> movies, int start, int end)
    {
        if (start > end)
        {
            return null;
        }

        int mid = (start + end) / 2;
        BTreeNode newNode = new BTreeNode(movies[mid]);

        newNode.LChild = GenerateBalancedTree(movies, start, mid - 1);
        newNode.RChild = GenerateBalancedTree(movies, mid + 1, end);

        return newNode;
    }

    static BTreeNode? GenerateUnbalancedTree(List<IMovie> movies, bool reverse)
    {
        BTreeNode? root = null;

        // Sort the movies by title
        movies.Sort((m1, m2) => string.Compare(m1.Title, m2.Title));

        if (reverse)
        {
            movies.Reverse();
        }

        foreach (IMovie movie in movies)
        {
            BTreeNode newNode = new BTreeNode(movie);
            root = InsertNode(root, newNode);
        }

        return root;
    }

    enum TreeType
    {
        Random,
        Balanced,
        Unbalanced,
        Sorted
    }

    static BTreeNode? GenerateTree(int numberOfNodes, TreeType treeType)
    {
        // Generate random movies
        List<IMovie> movies = new List<IMovie>();
        Random random = new Random();

        for (int i = 0; i < numberOfNodes; i++)
        {
            // Generate a random movie title and number of DVDs available
            string movieTitle = $"Movie {i}";
            int availableCopies = random.Next(1, 11);

            // Generate random genre, classification, and duration
            MovieGenre genre = (MovieGenre)random.Next(0, Enum.GetValues(typeof(MovieGenre)).Length);
            MovieClassification classification = (MovieClassification)random.Next(0,
                Enum.GetValues(typeof(MovieClassification)).Length);
            int duration = random.Next(60, 180);

            IMovie movie = new Movie(movieTitle, genre, classification, duration, availableCopies);
            movies.Add(movie);
        }

        if (treeType == TreeType.Sorted)
        {
            return GenerateSortedTree(numberOfNodes);
        }
        else if (treeType == TreeType.Balanced)
        {
            // Sort the movies by title
            movies.Sort((m1, m2) => string.Compare(m1.Title, m2.Title));
            return GenerateBalancedTree(movies, 0, movies.Count - 1);
        }
        else if (treeType == TreeType.Unbalanced)
        {
            return GenerateUnbalancedTree(movies, false); // Use 'true' for reverse-sorted unbalanced tree
        }
        else // TreeType.Random
        {
            BTreeNode? root = null;
            foreach (IMovie movie in movies)
            {
                BTreeNode newNode = new BTreeNode(movie);
                root = InsertNode(root, newNode);
            }
            return root;
        }
    }


    static BTreeNode? InsertNode(BTreeNode? root, BTreeNode newNode)
    {
        if (root == null)
        {
            return newNode;
        }

        if (string.Compare(newNode.Movie.Title, root.Movie.Title) < 0)
        {
            root.LChild = InsertNode(root.LChild, newNode);
        }
        else
        {
            root.RChild = InsertNode(root.RChild, newNode);
        }

        return root;
    }

    static void InsertTreeIntoCollection(BTreeNode? root, MovieCollection collection)
    {
        if (root != null)
        {
            collection.Insert(root.Movie);
            InsertTreeIntoCollection(root.LChild, collection);
            InsertTreeIntoCollection(root.RChild, collection);
        }
    }


}


//MovieCollection collection = new MovieCollection();

//// Test IsEmpty method
//Console.WriteLine("Is collection empty? " + collection.IsEmpty());
//Console.WriteLine();

//// Test Insert method
//Movie movie1 = new Movie("The Godfather", MovieGenre.Drama, MovieClassification.M15Plus, 175, 10);
//collection.Insert(movie1);
//Movie movie2 = new Movie("The Shawshank Redemption", MovieGenre.Drama, MovieClassification.M15Plus, 142, 8);
//collection.Insert(movie2);
//Movie movie3 = new Movie("Pulp Fiction", MovieGenre.Action, MovieClassification.M15Plus, 154, 12);
//collection.Insert(movie3);

//// Test NoDVDs method
//Console.WriteLine("Total number of DVDs: " + collection.NoDVDs());
//Console.WriteLine();

//// Test Search method
//IMovie? searchResult = collection.Search("The Shawshank Redemption");
//Console.WriteLine("Search result for 'The Shawshank Redemption': " + searchResult);
//Console.WriteLine();

//// Test ToArray method
//IMovie[] movieArray = collection.ToArray();
//Console.WriteLine("Movies in the collection (sorted):");
//foreach (IMovie movie in movieArray)
//{
//    Console.WriteLine(movie);
//    Console.WriteLine();
//}

//// Test Delete method
//collection.Delete(movie2);
//Console.WriteLine("Deleted 'The Shawshank Redemption'.");
//Console.WriteLine();

//// Test ToArray method after deletion
//movieArray = collection.ToArray();
//Console.WriteLine("Movies in the collection after deletion (sorted):");
//foreach (IMovie movie in movieArray)
//{
//    Console.WriteLine(movie);
//    Console.WriteLine();
//}

//// Test Clear method
//collection.Clear();
//Console.WriteLine("Cleared the movie collection.");
//Console.WriteLine("Is collection empty? " + collection.IsEmpty());
//Console.WriteLine();